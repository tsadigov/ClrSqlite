using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    using System.Diagnostics;
    using codec_ctx = Sqlite3.crypto.codec_ctx;

    #region enum

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
    ///OPEN:
    ///<param name=""></param>
    ///The pager starts up in this state. Nothing is guaranteed in this
    ///state the file may or may not be locked and the database size is
    ///unknown. The database may not be read or written.

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
    ///<param name="The Pager.errCode variable is set to something other than SqlResult.SQLITE_OK.">The Pager.errCode variable is set to something other than SqlResult.SQLITE_OK.</param>
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
    ///<param name="Normally, a connection open in exclusive mode is never in PagerState.PAGER_OPEN">Normally, a connection open in exclusive mode is never in PagerState.PAGER_OPEN</param>
    ///<param name="state. There are two exceptions: immediately after exclusive">mode has</param>
    ///<param name="been turned on (and before any read or write transactions are ">been turned on (and before any read or write transactions are </param>
    ///<param name="executed), and when the pager is leaving the "error state".">executed), and when the pager is leaving the "error state".</param>
    ///<param name=""></param>
    ///<param name="See also: assert_pager_state().">See also: assert_pager_state().</param>
    ///<param name=""></param>

    //#define PagerState.PAGER_OPEN                  0
    //#define PagerState.PAGER_READER                1
    //#define PagerState.PAGER_WRITER_LOCKED         2
    //#define PagerState.PAGER_WRITER_CACHEMOD       3
    //#define PagerState.PAGER_WRITER_DBMOD          4
    //#define PagerState.PAGER_WRITER_FINISHED       5
    //#define PagerState.PAGER_ERROR                 6

    public enum PagerState : byte
    {
        PAGER_OPEN = 0,

        PAGER_READER = 1,

        PAGER_WRITER_LOCKED = 2,

        PAGER_WRITER_CACHEMOD = 3,

        PAGER_WRITER_DBMOD = 4,

        PAGER_WRITER_FINISHED = 5,

        PAGER_ERROR = 6,
    }
    #endregion


    
    public partial class Sqlite3
    {

        ///<summary>
        /// Return TRUE if the page given in the argument was previously passed
        /// to PagerMethods.sqlite3PagerWrite().  In other words, return TRUE if it is ok
        /// to change the content of the page.
        ///
        ///</summary>
        static bool sqlite3PagerIswriteable(PgHdr pPg)
#if !NDEBUG
																																						    static bool sqlite3PagerIswriteable( DbPage pPg )
    {
      return ( pPg.flags & PGHDR_DIRTY ) != 0;
    }
#else

        {
            return true;
        }

#endif

        

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
        ///<param name="by PagerMethods.sqlite3PagerWrite() when the file">size is larger than</param>
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
        ///<param name="It is valid in PagerState.PAGER_READER and higher states (all states except for">It is valid in PagerState.PAGER_READER and higher states (all states except for</param>
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
        ///<param name="PagerState.PAGER_WRITER_LOCKED and higher. dbOrigSize is a copy of the dbSize">PagerState.PAGER_WRITER_LOCKED and higher. dbOrigSize is a copy of the dbSize</param>
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
        ///<param name="The Pager.errCode variable is only ever used in PagerState.PAGER_ERROR state. It">The Pager.errCode variable is only ever used in PagerState.PAGER_ERROR state. It</param>
        ///<param name="is set to zero in all other states. In PagerState.PAGER_ERROR state, Pager.errCode ">is set to zero in all other states. In PagerState.PAGER_ERROR state, Pager.errCode </param>
        ///<param name="is always set to SQLITE_FULL, SQLITE_IOERR or one of the SQLITE_IOERR_XXX ">is always set to SQLITE_FULL, SQLITE_IOERR or one of the SQLITE_IOERR_XXX </param>
        ///<param name="sub">codes.</param>
        ///<param name=""></param>

        public  class Pager
        {

            public sqlite3_vfs pVfs;

            ///<summary>
            ///OS functions to use for IO 
            ///</summary>

            public bool exclusiveMode;

            ///<summary>
            ///Boolean. True if locking_mode==EXCLUSIVE 
            ///</summary>

            public u8 journalMode;

            ///<summary>
            ///One of the PAGER_JOURNALMODE_* values 
            ///</summary>

            public u8 useJournal;

            ///<summary>
            ///Use a rollback journal on this file 
            ///</summary>

            public u8 noReadlock;

            ///<summary>
            ///Do not bother to obtain readlocks 
            ///</summary>

            public bool noSync;

            ///<summary>
            ///Do not sync the journal if true 
            ///</summary>

            public bool fullSync;

            ///<summary>
            ///Do extra syncs of the journal for robustness 
            ///</summary>

            public u8 ckptSyncFlags;

            ///<summary>
            ///SYNC_NORMAL or SYNC_FULL for checkpoint 
            ///</summary>

            public u8 syncFlags;

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

            public PagerState eState;

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

            public SqlResult errCode;

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

            public sqlite3_backup pBackup
            {
                get
                {
                    return _pBackup;
                }
                set
                {
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



            ///<summary>
                /// The size of the of each page record in the journal is given by
                /// the following macro.
                ///
                ///</summary>
                //#define JOURNAL_PG_SZ(pPager)  ((pPager.pageSize) + 8)
                
                public int JOURNAL_PG_SZ()
                {
                    return (this.pageSize + 8);
                }

                public///<summary>
                    /// The journal header size for this pager. This is usually the same
                    /// size as a single disk sector. See also setSectorSize().
                    ///
                    ///</summary>
                    //#define JOURNAL_HDR_SZ(pPager) (pPager.sectorSize)
                u32 JOURNAL_HDR_SZ()
                {
                    return (this.sectorSize);
                }
                ///<summary>
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
                public bool pagerUseWal()
                {
                    return false;
                }
#endif
                public SqlResult pagerRollbackWal()
                {
                    return 0;
                }

                public SqlResult pagerWalFrames(PgHdr w, Pgno x, int y, int z)
                {
                    return (SqlResult)0;
                }

                public SqlResult pagerOpenWalIfPresent()
                {
                    return SqlResult.SQLITE_OK;
                }

                public SqlResult pagerBeginReadTransaction()
                {
                    return SqlResult.SQLITE_OK;
                }



                public bool assert_pager_state()
                {
                    Pager pPager = this;
                    ///
                    ///<summary>
                    ///State must be valid. 
                    ///</summary>

                    Debug.Assert(this.eState == PagerState.PAGER_OPEN || this.eState == PagerState.PAGER_READER || this.eState == PagerState.PAGER_WRITER_LOCKED || this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_WRITER_FINISHED || this.eState == PagerState.PAGER_ERROR);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Regardless of the current state, a temp">file connection always behaves</param>
                    ///<param name="as if it has an exclusive lock on the database file. It never updates">as if it has an exclusive lock on the database file. It never updates</param>
                    ///<param name="the change">counter field, so the changeCountDone flag is always set.</param>
                    ///<param name=""></param>

                    Debug.Assert(this.tempFile == false || this.eLock == EXCLUSIVE_LOCK);
                    Debug.Assert(this.tempFile == false || pPager.changeCountDone);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the useJournal flag is clear, the journal">mode must be "OFF". </param>
                    ///<param name="And if the journal">mode is "OFF", the journal file must not be open.</param>
                    ///<param name=""></param>

                    Debug.Assert(this.journalMode == PAGER_JOURNALMODE_OFF || this.useJournal != 0);
                    Debug.Assert(this.journalMode != PAGER_JOURNALMODE_OFF || !this.jfd.isOpen);
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
)
                    {
                        Debug.Assert(this.noSync);
                        Debug.Assert(this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_MEMORY);
                        Debug.Assert(this.eState != PagerState.PAGER_ERROR && this.eState != PagerState.PAGER_OPEN);
                        Debug.Assert(this.pagerUseWal() == false);
                    }
                    ///
                    ///<summary>
                    ///If changeCountDone is set, a RESERVED lock or greater must be held
                    ///on the file.
                    ///
                    ///</summary>

                    Debug.Assert(pPager.changeCountDone == false || pPager.eLock >= RESERVED_LOCK);
                    Debug.Assert(this.eLock != PENDING_LOCK);
                    switch (this.eState)
                    {
                        case PagerState.PAGER_OPEN:
                            Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																							0==MEMDB
#else
0 == pPager.memDb
#endif
);
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            Debug.Assert(PCacheMethods.sqlite3PcacheRefCount(pPager.pPCache) == 0 || pPager.tempFile);
                            break;
                        case PagerState.PAGER_READER:
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            Debug.Assert(this.eLock != UNKNOWN_LOCK);
                            Debug.Assert(this.eLock >= SHARED_LOCK || this.noReadlock != 0);
                            break;
                        case PagerState.PAGER_WRITER_LOCKED:
                            Debug.Assert(this.eLock != UNKNOWN_LOCK);
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            if (!pPager.pagerUseWal())
                            {
                                Debug.Assert(this.eLock >= RESERVED_LOCK);
                            }
                            Debug.Assert(pPager.dbSize == pPager.dbOrigSize);
                            Debug.Assert(pPager.dbOrigSize == pPager.dbFileSize);
                            Debug.Assert(pPager.dbOrigSize == pPager.dbHintSize);
                            Debug.Assert(pPager.setMaster == 0);
                            break;
                        case PagerState.PAGER_WRITER_CACHEMOD:
                            Debug.Assert(this.eLock != UNKNOWN_LOCK);
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            if (!pPager.pagerUseWal())
                            {
                                ///
                                ///<summary>
                                ///It is possible that if journal_mode=wal here that neither the
                                ///journal file nor the WAL file are open. This happens during
                                ///a rollback transaction that switches from journal_mode=off
                                ///to journal_mode=wal.
                                ///
                                ///</summary>

                                Debug.Assert(this.eLock >= RESERVED_LOCK);
                                Debug.Assert(this.jfd.isOpen || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
                            }
                            Debug.Assert(pPager.dbOrigSize == pPager.dbFileSize);
                            Debug.Assert(pPager.dbOrigSize == pPager.dbHintSize);
                            break;
                        case PagerState.PAGER_WRITER_DBMOD:
                            Debug.Assert(this.eLock == EXCLUSIVE_LOCK);
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            Debug.Assert(!pPager.pagerUseWal());
                            Debug.Assert(this.eLock >= EXCLUSIVE_LOCK);
                            Debug.Assert(this.jfd.isOpen || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
                            Debug.Assert(pPager.dbOrigSize <= pPager.dbHintSize);
                            break;
                        case PagerState.PAGER_WRITER_FINISHED:
                            Debug.Assert(this.eLock == EXCLUSIVE_LOCK);
                            Debug.Assert(pPager.errCode == SqlResult.SQLITE_OK);
                            Debug.Assert(!pPager.pagerUseWal());
                            Debug.Assert(this.jfd.isOpen || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
                            break;
                        case PagerState.PAGER_ERROR:
                            ///
                            ///<summary>
                            ///There must be at least one outstanding reference to the pager if
                            ///in ERROR state. Otherwise the pager should have already dropped
                            ///back to OPEN state.
                            ///
                            ///</summary>

                            Debug.Assert(pPager.errCode != SqlResult.SQLITE_OK);
                            Debug.Assert(PCacheMethods.sqlite3PcacheRefCount(pPager.pPCache) > 0);
                            break;
                    }
                    return true;
                }

                ///<summary>
                /// Unlock the database file to level eLock, which must be either NO_LOCK
                /// or SHARED_LOCK. Regardless of whether or not the call to xUnlock()
                /// succeeds, set the Pager.eLock variable to match the (attempted) new lock.
                ///
                /// Except, if Pager.eLock is set to UNKNOWN_LOCK when this function is
                /// called, do not modify it. See the comment above the #define of
                /// UNKNOWN_LOCK for an explanation of this.
                ///
                ///</summary>
                public SqlResult pagerUnlockDb(int eLock)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    Debug.Assert(!this.exclusiveMode || this.eLock == eLock);
                    Debug.Assert(eLock == NO_LOCK || eLock == SHARED_LOCK);
                    Debug.Assert(eLock != NO_LOCK || this.pagerUseWal() == false);
                    if (this.fd.isOpen  )
                    {
                        Debug.Assert(this.eLock >= eLock);
                        rc = os.sqlite3OsUnlock(this.fd, eLock);
                        if (this.eLock != UNKNOWN_LOCK)
                        {
                            this.eLock = (u8)eLock;
                        }
                        sqliteinth.IOTRACE("UNLOCK %p %d\n", this, eLock);
                    }
                    return rc;
                }

                public SqlResult pagerLockDb(int eLock)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    Debug.Assert(eLock == SHARED_LOCK || eLock == RESERVED_LOCK || eLock == EXCLUSIVE_LOCK);
                    if (this.eLock < eLock || this.eLock == UNKNOWN_LOCK)
                    {
                        rc = os.sqlite3OsLock(this.fd, eLock);
                        if (rc == SqlResult.SQLITE_OK && (this.eLock != UNKNOWN_LOCK || eLock == EXCLUSIVE_LOCK))
                        {
                            this.eLock = (u8)eLock;
                            sqliteinth.IOTRACE("LOCK %p %d\n", this, eLock);
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
                i64 journalHdrOffset()
                {
                    i64 offset = 0;
                    i64 c = this.journalOff;
                    if (c != 0)
                    {
                        offset = (int)(((c - 1) / this.sectorSize + 1) * this.sectorSize);
                        //offset = ((c-1)/JOURNAL_HDR_SZ(pPager) + 1) * JOURNAL_HDR_SZ(pPager);
                    }
                    Debug.Assert(offset % this.sectorSize == 0);
                    //Debug.Assert(offset % JOURNAL_HDR_SZ(pPager) == 0);
                    Debug.Assert(offset >= c);
                    Debug.Assert((offset - c) < this.sectorSize);
                    //Debug.Assert( (offset-c)<JOURNAL_HDR_SZ(pPager) );
                    return offset;
                }

                public void seekJournalHdr()
                {
                    this.journalOff = this.journalHdrOffset();
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
                    /// Otherwise, return SqlResult.SQLITE_OK.
                    ///
                    ///</summary>
                SqlResult zeroJournalHdr(int doTruncate)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    Debug.Assert(this.jfd.isOpen);
                    if (this.journalOff != 0)
                    {
                        i64 iLimit = this.journalSizeLimit;
                        ///
                        ///<summary>
                        ///Local cache of jsl 
                        ///</summary>

                        sqliteinth.IOTRACE("JZEROHDR %p\n", this);
                        if (doTruncate != 0 || iLimit == 0)
                        {
                            rc = os.sqlite3OsTruncate(this.jfd, 0);
                        }
                        else
                        {
                            byte[] zeroHdr = new byte[28];
                            // = {0};
                            rc = os.sqlite3OsWrite(this.jfd, zeroHdr, zeroHdr.Length, 0);
                        }
                        if (rc == SqlResult.SQLITE_OK && !this.noSync)
                        {
                            rc = os.sqlite3OsSync(this.jfd, SQLITE_SYNC_DATAONLY | this.syncFlags);
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

                        if (rc == SqlResult.SQLITE_OK && iLimit > 0)
                        {
                            i64 sz = 0;
                            rc = os.sqlite3OsFileSize(this.jfd, ref sz);
                            if (rc == SqlResult.SQLITE_OK && sz > iLimit)
                            {
                                rc = os.sqlite3OsTruncate(this.jfd, iLimit);
                            }
                        }
                    }
                    return rc;
                }







                ///<summary>
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
                public SqlResult writeJournalHdr()//-----<<-----<<-----<<-----<<
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
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

                    Debug.Assert(this.jfd.isOpen);
                    ///
                    ///<summary>
                    ///Journal file must be open. 
                    ///</summary>

                    if (nHeader > this.JOURNAL_HDR_SZ())
                    {
                        nHeader = this.JOURNAL_HDR_SZ();
                    }
                    ///
                    ///<summary>
                    ///If there are active savepoints and any of them were created
                    ///since the most recent journal header was written, update the
                    ///PagerSavepoint.iHdrOffset fields now.
                    ///
                    ///</summary>

                    for (ii = 0; ii < this.nSavepoint; ii++)
                    {
                        if (this.aSavepoint[ii].iHdrOffset == 0)
                        {
                            this.aSavepoint[ii].iHdrOffset = this.journalOff;
                        }
                    }
                    this.journalHdr = this.journalOff = this.journalHdrOffset();
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

                    Debug.Assert(this.fd.isOpen   || this.noSync);
                    if (this.noSync || (this.journalMode == PAGER_JOURNALMODE_MEMORY) || (os.sqlite3OsDeviceCharacteristics(this.fd) & SQLITE_IOCAP_SAFE_APPEND) != 0)
                    {
                        aJournalMagic.CopyTo(zHeader, 0);
                        // memcpy(zHeader, aJournalMagic, sizeof(aJournalMagic));
                         Converter.put32bits(zHeader, aJournalMagic.Length, 0xffffffff);
                    }
                    else
                    {
                        Array.Clear(zHeader, 0, aJournalMagic.Length + 4);
                        //memset(zHeader, 0, sizeof(aJournalMagic)+4);
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="The random check">hash initialiser </param>

                    i64 i64Temp = 0;
                    sqlite3_randomness(sizeof(i64), ref i64Temp);
                    this.cksumInit = (u32)i64Temp;
                    Converter.put32bits(zHeader, aJournalMagic.Length + 4, this.cksumInit);
                    ///
                    ///<summary>
                    ///The initial database size 
                    ///</summary>

                    Converter.put32bits(zHeader, aJournalMagic.Length + 8, this.dbOrigSize);
                    ///
                    ///<summary>
                    ///The assumed sector size for this process 
                    ///</summary>

                    Converter.put32bits(zHeader, aJournalMagic.Length + 12, this.sectorSize);
                    ///
                    ///<summary>
                    ///The page size 
                    ///</summary>

                    Converter.put32bits(zHeader, aJournalMagic.Length + 16, (u32)this.pageSize);
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
                    Array.Clear(zHeader, aJournalMagic.Length + 20, (int)nHeader - (aJournalMagic.Length + 20));
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
                    ///<param name="bytes in size, more than one call to os.sqlite3OsWrite() may be required">bytes in size, more than one call to os.sqlite3OsWrite() may be required</param>
                    ///<param name="to populate the entire journal header sector.">to populate the entire journal header sector.</param>
                    ///<param name=""></param>

                    for (nWrite = 0; rc == SqlResult.SQLITE_OK && nWrite < this.JOURNAL_HDR_SZ(); nWrite += nHeader)
                    {
                        sqliteinth.IOTRACE("JHDR %p %lld %d\n", this, this.journalHdr, nHeader);
                        rc = os.sqlite3OsWrite(this.jfd, zHeader, (int)nHeader, this.journalOff);
                        Debug.Assert(this.journalHdr <= this.journalOff);
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
                      /// is set to the value read from the journal header. SqlResult.SQLITE_OK is returned
                      /// in this case.
                      ///
                      /// If the journal header file appears to be corrupted, SQLITE_DONE is
                      /// returned and *pNRec and *PDbSize are undefined.  If JOURNAL_HDR_SZ bytes
                      /// cannot be read from the journal file an error code is returned.
                      ///
                      ///</summary>
                SqlResult readJournalHdr(///
                    ///Pager object 

                int isHot, i64 journalSize, ///
                    ///Size of the open journal file in bytes 

                out u32 pNRec, ///
                    ///OUT: Value read from the nRec field 

                out u32 pDbSize///
                    ///OUT: Value of original database size field 

                )
                {
                    SqlResult rc;
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

                    Debug.Assert(this.jfd.isOpen);
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

                    this.journalOff = this.journalHdrOffset();
                    if (this.journalOff + this.JOURNAL_HDR_SZ() > journalSize)
                    {
                        return SqlResult.SQLITE_DONE;
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

                    if (isHot != 0 || iHdrOff != this.journalHdr)
                    {
                        rc = os.sqlite3OsRead(this.jfd, aMagic, aMagic.Length, iHdrOff);
                        if (rc != (SqlResult)0)
                        {
                            return rc;
                        }
                        if (_Custom.memcmp(aMagic, aJournalMagic, aMagic.Length) != 0)
                        {
                            return SqlResult.SQLITE_DONE;
                        }
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Read the first three 32">bit fields of the journal header: The nRec</param>
                    ///<param name="field, the checksum">initializer and the database size at the start</param>
                    ///<param name="of the transaction. Return an error code if anything goes wrong.">of the transaction. Return an error code if anything goes wrong.</param>
                    ///<param name=""></param>

                    if (SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(this.jfd, iHdrOff + 8, ref pNRec)) || SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(this.jfd, iHdrOff + 12, ref this.cksumInit)) || SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(this.jfd, iHdrOff + 16, ref pDbSize)))
                    {
                        return rc;
                    }
                    if (this.journalOff == 0)
                    {
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

                        if (SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(this.jfd, iHdrOff + 20, ref iSectorSize)) || SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(this.jfd, iHdrOff + 24, ref iPageSize)))
                        {
                            return rc;
                        }
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Versions of SQLite prior to 3.5.8 set the page">size field of the</param>
                        ///<param name="journal header to zero. In this case, assume that the Pager.pageSize">journal header to zero. In this case, assume that the Pager.pageSize</param>
                        ///<param name="variable is already set to the correct page size.">variable is already set to the correct page size.</param>
                        ///<param name=""></param>

                        if (iPageSize == 0)
                        {
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

                        if (iPageSize < 512 || iSectorSize < 32 || iPageSize > SQLITE_MAX_PAGE_SIZE || iSectorSize > MAX_SECTOR_SIZE || ((iPageSize - 1) & iPageSize) != 0 || ((iSectorSize - 1) & iSectorSize) != 0)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="If the either the page">header is</param>
                            ///<param name="invalid, then the process that wrote the journal">header must have</param>
                            ///<param name="crashed before the header was synced. In this case stop reading">crashed before the header was synced. In this case stop reading</param>
                            ///<param name="the journal file here.">the journal file here.</param>
                            ///<param name=""></param>

                            return SqlResult.SQLITE_DONE;
                        }
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Update the page">size to match the value read from the journal.</param>
                        ///<param name="Use a sqliteinth.testcase() macro to make sure that malloc failure within">Use a sqliteinth.testcase() macro to make sure that malloc failure within</param>
                        ///<param name="PagerSetPagesize() is tested.">PagerSetPagesize() is tested.</param>
                        ///<param name=""></param>

                        rc = this.sqlite3PagerSetPagesize(ref iPageSize, -1);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
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
                    this.journalOff += (int)this.JOURNAL_HDR_SZ();
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
                SqlResult writeMasterJournal(string zMaster)
                {
                    SqlResult rc;
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

                    Debug.Assert(this.setMaster == 0);
                    Debug.Assert(!this.pagerUseWal());
                    if (null == zMaster || this.journalMode == PAGER_JOURNALMODE_MEMORY || this.journalMode == PAGER_JOURNALMODE_OFF)
                    {
                        return SqlResult.SQLITE_OK;
                    }
                    this.setMaster = 1;
                    Debug.Assert(this.jfd.isOpen);
                    Debug.Assert(this.journalHdr <= this.journalOff);
                    ///
                    ///<summary>
                    ///Calculate the length in bytes and the checksum of zMaster 
                    ///</summary>

                    for (nMaster = 0; nMaster < zMaster.Length && zMaster[nMaster] != 0; nMaster++)
                    {
                        cksum += zMaster[nMaster];
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If in full">sync mode, advance to the next disk sector before writing</param>
                    ///<param name="the master journal name. This is in case the previous page written to">the master journal name. This is in case the previous page written to</param>
                    ///<param name="the journal has already been synced.">the journal has already been synced.</param>
                    ///<param name=""></param>

                    if (this.fullSync)
                    {
                        this.journalOff = this.journalHdrOffset();
                    }
                    iHdrOff = this.journalOff;
                    ///
                    ///<summary>
                    ///Write the master journal data to the end of the journal file. If
                    ///an error occurs, return the error code to the caller.
                    ///
                    ///</summary>

                    if ((0 != (rc = PagerMethods.write32bits(this.jfd, iHdrOff, (u32)PAGER_MJ_PGNO(this)))) || (0 != (rc = os.sqlite3OsWrite(this.jfd, Encoding.UTF8.GetBytes(zMaster), nMaster, iHdrOff + 4))) || (0 != (rc = PagerMethods.write32bits(this.jfd, iHdrOff + 4 + nMaster, (u32)nMaster))) || (0 != (rc = PagerMethods.write32bits(this.jfd, iHdrOff + 4 + nMaster + 4, cksum))) || (0 != (rc = os.sqlite3OsWrite(this.jfd, aJournalMagic, 8, iHdrOff + 4 + nMaster + 8))))
                    {
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

                    if (SqlResult.SQLITE_OK == (rc = os.sqlite3OsFileSize(this.jfd, ref jrnlSize)) && jrnlSize > this.journalOff)
                    {
                        rc = os.sqlite3OsTruncate(this.jfd, this.journalOff);
                    }
                    return rc;
                }

                public///<summary>
                    /// Find a page in the hash table given its page number. Return
                    /// a pointer to the page or NULL if the requested page is not
                    /// already in memory.
                    ///
                    ///</summary>
                PgHdr pager_lookup(u32 pgno)
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

                    PCacheMethods.sqlite3PcacheFetch(this.pPCache, pgno, 0, ref p);
                    return p;
                }

                public///<summary>
                    /// Discard the entire contents of the in-memory page-cache.
                    ///
                    ///</summary>
                void pager_reset()
                {
                    if (null != this.pBackup)
                        this.pBackup.sqlite3BackupRestart();
                    PCacheMethods.sqlite3PcacheClear(this.pPCache);
                }

                public///<summary>
                    /// Set the bit number pgno in the PagerSavepoint.pInSavepoint
                    /// bitvecs of all open savepoints. Return SqlResult.SQLITE_OK if successful
                    /// or SQLITE_NOMEM if a malloc failure occurs.
                    ///
                    ///</summary>
                SqlResult addToSavepointBitvecs(u32 pgno)
                {
                    int ii;
                    ///
                    ///<summary>
                    ///Loop counter 
                    ///</summary>

                    var rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Result code 
                    ///</summary>

                    for (ii = 0; ii < this.nSavepoint; ii++)
                    {
                        PagerSavepoint p = this.aSavepoint[ii];
                        if (pgno <= p.nOrig)
                        {
                            rc |= sqlite3BitvecSet(p.pInSavepoint, pgno);
                            sqliteinth.testcase(rc == SqlResult.SQLITE_NOMEM);
                            Debug.Assert(rc == SqlResult.SQLITE_OK || rc == SqlResult.SQLITE_NOMEM);
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
                void releaseAllSavepoints()
                {
                    int ii;
                    ///
                    ///<summary>
                    ///Iterator for looping through Pager.aSavepoint 
                    ///</summary>

                    for (ii = 0; ii < this.nSavepoint; ii++)
                    {
                        sqlite3BitvecDestroy(ref this.aSavepoint[ii].pInSavepoint);
                    }
                    if (!this.exclusiveMode || memjrnl.sqlite3IsMemJournal(this.sjfd))
                    {
                        os.sqlite3OsClose(this.sjfd);
                    }
                    //malloc_cs.sqlite3_free( ref pPager.aSavepoint );
                    this.aSavepoint = null;
                    this.nSavepoint = 0;
                    this.nSubRec = 0;
                }

                public///<summary>
                    /// This function is a no-op if the pager is in exclusive mode and not
                    /// in the ERROR state. Otherwise, it switches the pager to PagerState.PAGER_OPEN
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
                void pager_unlock()
                {
                    Debug.Assert(this.eState == PagerState.PAGER_READER || this.eState == PagerState.PAGER_OPEN || this.eState == PagerState.PAGER_ERROR);
                    sqlite3BitvecDestroy(ref this.pInJournal);
                    this.pInJournal = null;
                    this.releaseAllSavepoints();
                    if (this.pagerUseWal())
                    {
                        Debug.Assert(!this.jfd.isOpen);
                        wal.sqlite3WalEndReadTransaction(this.pWal);
                        this.eState = PagerState.PAGER_OPEN;
                    }
                    else
                        if (!this.exclusiveMode)
                        {
                            SqlResult rc;
                            ///
                            ///<summary>
                            ///Error code returned by pagerUnlockDb() 
                            ///</summary>

                            int iDc = this.fd.isOpen   ? os.sqlite3OsDeviceCharacteristics(this.fd) : 0;
                            ///
                            ///<summary>
                            ///If the operating system support deletion of open files, then
                            ///close the journal file when dropping the database lock.  Otherwise
                            ///another connection with journal_mode=delete might delete the file
                            ///out from under us.
                            ///
                            ///</summary>

                            Debug.Assert((PAGER_JOURNALMODE_MEMORY & 5) != 1);
                            Debug.Assert((PAGER_JOURNALMODE_OFF & 5) != 1);
                            Debug.Assert((PAGER_JOURNALMODE_WAL & 5) != 1);
                            Debug.Assert((PAGER_JOURNALMODE_DELETE & 5) != 1);
                            Debug.Assert((PAGER_JOURNALMODE_TRUNCATE & 5) == 1);
                            Debug.Assert((PAGER_JOURNALMODE_PERSIST & 5) == 1);
                            if (0 == (iDc & SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN) || 1 != (this.journalMode & 5))
                            {
                                os.sqlite3OsClose(this.jfd);
                            }
                            ///
                            ///<summary>
                            ///If the pager is in the ERROR state and the call to unlock the database
                            ///file fails, set the current lock to UNKNOWN_LOCK. See the comment
                            ///above the #define for UNKNOWN_LOCK for an explanation of why this
                            ///is necessary.
                            ///
                            ///</summary>

                            rc = this.pagerUnlockDb(NO_LOCK);
                            if (rc != SqlResult.SQLITE_OK && this.eState == PagerState.PAGER_ERROR)
                            {
                                this.eLock = UNKNOWN_LOCK;
                            }
                            ///
                            ///<summary>
                            ///The pager state may be changed from PagerState.PAGER_ERROR to PagerState.PAGER_OPEN here
                            ///</summary>
                            ///<param name="without clearing the error code. This is intentional "> the error</param>
                            ///<param name="code is cleared and the cache reset in the block below.">code is cleared and the cache reset in the block below.</param>
                            ///<param name=""></param>

                            Debug.Assert(this.errCode != 0 || this.eState != PagerState.PAGER_ERROR);
                            this.changeCountDone = false;
                            this.eState = PagerState.PAGER_OPEN;
                        }
                    ///
                    ///<summary>
                    ///If Pager.errCode is set, the contents of the pager cache cannot be
                    ///trusted. Now that there are no outstanding references to the pager,
                    ///it can safely move back to PagerState.PAGER_OPEN state. This happens in both
                    ///</summary>
                    ///<param name="normal and exclusive">locking mode.</param>
                    ///<param name=""></param>

                    if (this.errCode != 0)
                    {
                        Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
0 == this.memDb
#endif
);
                        this.pager_reset();
                        this.changeCountDone = this.tempFile;
                        this.eState = PagerState.PAGER_OPEN;
                        this.errCode = SqlResult.SQLITE_OK;
                    }
                    this.journalOff = 0;
                    this.journalHdr = 0;
                    this.setMaster = 0;
                }

            ///<summary>
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
                public SqlResult pager_error(SqlResult rc)
                {
                    SqlResult rc2 = rc & (SqlResult)0xff;
                    Debug.Assert(rc == SqlResult.SQLITE_OK ||
#if SQLITE_OMIT_MEMORYDB
																																																																						0==MEMDB
#else
 0 == this.memDb
#endif
);
                    Debug.Assert(this.errCode == SqlResult.SQLITE_FULL || this.errCode == SqlResult.SQLITE_OK || (this.errCode & (SqlResult)0xff) == SqlResult.SQLITE_IOERR);
                    if (rc2 == SqlResult.SQLITE_FULL || rc2 == SqlResult.SQLITE_IOERR)
                    {
                        this.errCode = rc;
                        this.eState = PagerState.PAGER_ERROR;
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
                    /// This routine is never called in PagerState.PAGER_ERROR state. If it is called
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
                    ///     The journal file is closed and deleted using os.sqlite3OsDelete().
                    ///
                    ///     If the pager is running in exclusive mode, this method of finalizing
                    ///     the journal file is never used. Instead, if the journalMode is
                    ///     DELETE and the pager is in exclusive mode, the method described under
                    ///     journalMode==PERSIST is used instead.
                    ///
                    /// After the journal is finalized, the pager moves to PagerState.PAGER_READER state.
                    /// If running in non-exclusive rollback mode, the lock on the file is
                    /// downgraded to a SHARED_LOCK.
                    ///
                    /// SqlResult.SQLITE_OK is returned if no error occurs. If an error occurs during
                    /// any of the IO operations to finalize the journal file or unlock the
                    /// database then the IO error code is returned to the user. If the
                    /// operation to finalize the journal file fails, then the code still
                    /// tries to unlock the database file if not in exclusive mode. If the
                    /// unlock operation fails as well, then the first error code related
                    /// to the first error encountered (the journal finalization one) is
                    /// returned.
                    ///
                    ///</summary>
                SqlResult pager_end_transaction(int hasMaster)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Error code from journal finalization operation 
                    ///</summary>

                    SqlResult rc2 = SqlResult.SQLITE_OK;
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
                    ///<param name="read">transaction, this function is called with eState==PagerState.PAGER_READER </param>
                    ///<param name="and eLock==EXCLUSIVE_LOCK when the read">transaction is closed.</param>
                    ///<param name=""></param>

                    Debug.Assert(this.assert_pager_state());
                    Debug.Assert(this.eState != PagerState.PAGER_ERROR);
                    if (this.eState < PagerState.PAGER_WRITER_LOCKED && this.eLock < RESERVED_LOCK)
                    {
                        return SqlResult.SQLITE_OK;
                    }
                    this.releaseAllSavepoints();
                    Debug.Assert(this.jfd.isOpen || this.pInJournal == null);
                    if (this.jfd.isOpen)
                    {
                        Debug.Assert(!this.pagerUseWal());
                        ///
                        ///<summary>
                        ///Finalize the journal file. 
                        ///</summary>

                        if (memjrnl.sqlite3IsMemJournal(this.jfd))
                        {
                            Debug.Assert(this.journalMode == PAGER_JOURNALMODE_MEMORY);
                            os.sqlite3OsClose(this.jfd);
                        }
                        else
                            if (this.journalMode == PAGER_JOURNALMODE_TRUNCATE)
                            {
                                if (this.journalOff == 0)
                                {
                                    rc = SqlResult.SQLITE_OK;
                                }
                                else
                                {
                                    rc = os.sqlite3OsTruncate(this.jfd, 0);
                                }
                                this.journalOff = 0;
                            }
                            else
                                if (this.journalMode == PAGER_JOURNALMODE_PERSIST || (this.exclusiveMode && this.journalMode != PAGER_JOURNALMODE_WAL))
                                {
                                    rc = this.zeroJournalHdr(hasMaster);
                                    this.journalOff = 0;
                                }
                                else
                                {
                                    ///
                                    ///<summary>
                                    ///This branch may be executed with Pager.journalMode==MEMORY if
                                    ///</summary>
                                    ///<param name="a hot">journal was just rolled back. In this case the journal</param>
                                    ///<param name="file should be closed and deleted. If this connection writes to">file should be closed and deleted. If this connection writes to</param>
                                    ///<param name="the database file, it will do so using an in">memory journal. </param>
                                    ///<param name=""></param>

                                    Debug.Assert(this.journalMode == PAGER_JOURNALMODE_DELETE || this.journalMode == PAGER_JOURNALMODE_MEMORY || this.journalMode == PAGER_JOURNALMODE_WAL);
                                    os.sqlite3OsClose(this.jfd);
                                    if (!this.tempFile)
                                    {
                                        rc = os.sqlite3OsDelete(this.pVfs, this.zJournal, 0);
                                    }
                                }
                    }
#if SQLITE_CHECK_PAGES
																																																																						sqlite3PcacheIterateDirty(pPager.pPCache, pager_set_pagehash);
if( pPager.dbSize==0 && sqlite3PcacheRefCount(pPager.pPCache)>0 ){
PgHdr p = pager_lookup(pPager, 1);
if( p != null ){
p.pageHash = null;
PagerMethods.sqlite3PagerUnref(p);
}
}
#endif
                    sqlite3BitvecDestroy(ref this.pInJournal);
                    this.pInJournal = null;
                    this.nRec = 0;
                    PCacheMethods.sqlite3PcacheCleanAll(this.pPCache);
                    PCacheMethods.sqlite3PcacheTruncate(this.pPCache, this.dbSize);
                    if (this.pagerUseWal())
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Drop the WAL write">lock, if any. Also, if the connection was in </param>
                        ///<param name="locking_mode=exclusive mode but is no longer, drop the EXCLUSIVE ">locking_mode=exclusive mode but is no longer, drop the EXCLUSIVE </param>
                        ///<param name="lock held on the database file.">lock held on the database file.</param>
                        ///<param name=""></param>

                        rc2 = wal.sqlite3WalEndWriteTransaction(this.pWal);
                        Debug.Assert(rc2 == SqlResult.SQLITE_OK);
                    }
                    if (!this.exclusiveMode && (!this.pagerUseWal() || wal.sqlite3WalExclusiveMode(this.pWal, 0)))
                    {
                        rc2 = this.pagerUnlockDb(SHARED_LOCK);
                        this.changeCountDone = false;
                    }
                    this.eState = PagerState.PAGER_READER;
                    this.setMaster = 0;
                    return (rc == SqlResult.SQLITE_OK ? rc2 : rc);
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
                void pagerUnlockAndRollback()
                {
                    if (this.eState != PagerState.PAGER_ERROR && this.eState != PagerState.PAGER_OPEN)
                    {
                        Debug.Assert(this.assert_pager_state());
                        if (this.eState >= PagerState.PAGER_WRITER_LOCKED)
                        {
                            sqlite3BeginBenignMalloc();
                            this.sqlite3PagerRollback();
                            sqlite3EndBenignMalloc();
                        }
                        else
                            if (!this.exclusiveMode)
                            {
                                Debug.Assert(this.eState == PagerState.PAGER_READER);
                                this.pager_end_transaction(0);
                            }
                    }
                    this.pager_unlock();
                }

                public u32 pager_cksum(byte[] aData)
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

                    while (i > 0)
                    {
                        cksum += aData[i];
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
                void pagerReportSize()
                {
                    if (this.xCodecSizeChng != null)
                    {
                        this.xCodecSizeChng(this.pCodec, this.pageSize, this.nReserve);
                    }
                }

            ///<summary>
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
            /// skipped and SqlResult.SQLITE_OK is returned.
            ///
            /// If pDone is not NULL, then it is a record of pages that have already
            /// been played back.  If the page at *pOffset has already been played back
            /// (if the corresponding pDone bit is set) then skip the playback.
            /// Make sure the pDone bit corresponding to the *pOffset page is set
            /// prior to returning.
            ///
            /// If the page record is successfully read from the (sub-)journal file
            /// and played back, then SqlResult.SQLITE_OK is returned. If an IO error occurs
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
            public SqlResult pager_playback_one_page(///
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
                    SqlResult rc;
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

                    Debug.Assert((isMainJrnl & ~1) == 0);
                    ///
                    ///<summary>
                    ///isMainJrnl is 0 or 1 
                    ///</summary>

                    Debug.Assert((isSavepnt & ~1) == 0);
                    ///
                    ///<summary>
                    ///isSavepnt is 0 or 1 
                    ///</summary>

                    Debug.Assert(isMainJrnl != 0 || pDone != null);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="pDone always used on sub">journals </param>

                    Debug.Assert(isSavepnt != 0 || pDone == null);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="pDone never used on non">savepoint </param>

                    aData = this.pTmpSpace;
                    Debug.Assert(aData != null);
                    ///
                    ///<summary>
                    ///Temp storage must have already been allocated 
                    ///</summary>

                    Debug.Assert(this.pagerUseWal() == false || (0 == isMainJrnl && isSavepnt != 0));
                    ///
                    ///<summary>
                    ///Either the state is greater than PagerState.PAGER_WRITER_CACHEMOD (a transaction 
                    ///or savepoint rollback done at the request of the caller) or this is
                    ///</summary>
                    ///<param name="a hot">journal rollback, the pager</param>
                    ///<param name="is in state OPEN and holds an EXCLUSIVE lock. Hot">journal rollback</param>
                    ///<param name="only reads from the main journal, not the sub">journal.</param>
                    ///<param name=""></param>

                    Debug.Assert(this.eState >= PagerState.PAGER_WRITER_CACHEMOD || (this.eState == PagerState.PAGER_OPEN && this.eLock == EXCLUSIVE_LOCK));
                    Debug.Assert(this.eState >= PagerState.PAGER_WRITER_CACHEMOD || isMainJrnl != 0);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Read the page number and page data from the journal or sub">journal</param>
                    ///<param name="file. Return an error code to the caller if an IO error occurs.">file. Return an error code to the caller if an IO error occurs.</param>
                    ///<param name=""></param>

                    jfd = isMainJrnl != 0 ? this.jfd : this.sjfd;
                    rc = PagerMethods.read32bits(jfd, pOffset, ref pgno);
                    if (rc != SqlResult.SQLITE_OK)
                        return rc;
                    rc = os.sqlite3OsRead(jfd, aData, this.pageSize, (pOffset) + 4);
                    if (rc != SqlResult.SQLITE_OK)
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

                    if (pgno == 0 || pgno == PAGER_MJ_PGNO(this))
                    {
                        Debug.Assert(0 == isSavepnt);
                        return SqlResult.SQLITE_DONE;
                    }
                    if (pgno > this.dbSize || sqlite3BitvecTest(pDone, pgno) != 0)
                    {
                        return SqlResult.SQLITE_OK;
                    }
                    if (isMainJrnl != 0)
                    {
                        rc = PagerMethods.read32bits(jfd, (pOffset) - 4, ref cksum);
                        if (rc != 0)
                            return rc;
                        if (0 == isSavepnt && this.pager_cksum(aData) != cksum)
                        {
                            return SqlResult.SQLITE_DONE;
                        }
                    }
                    ///
                    ///<summary>
                    ///If this page has already been played by before during the current
                    ///rollback, then don't bother to play it back again.
                    ///
                    ///</summary>

                    if (pDone != null && (rc = sqlite3BitvecSet(pDone, pgno)) != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///When playing back page 1, restore the nReserve setting
                    ///
                    ///</summary>

                    if (pgno == 1 && this.nReserve != (aData)[20])
                    {
                        this.nReserve = (aData)[20];
                        this.pagerReportSize();
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
                    ///<param name="not dirty. Since this code is only executed in PagerState.PAGER_OPEN state for">not dirty. Since this code is only executed in PagerState.PAGER_OPEN state for</param>
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

                    if (this.pagerUseWal())
                    {
                        pPg = null;
                    }
                    else
                    {
                        pPg = this.pager_lookup(pgno);
                    }
                    Debug.Assert(pPg != null ||
#if SQLITE_OMIT_MEMORYDB
																																																																						0==MEMDB
#else
 this.memDb == 0
#endif
);
                    Debug.Assert(this.eState != PagerState.PAGER_OPEN || pPg == null);
                    PAGERTRACE("PLAYBACK %d page %d hash(%08x) %s\n", PagerMethods.PAGERID(this), pgno, PagerMethods.pager_datahash(this.pageSize, aData), (isMainJrnl != 0 ? "main-journal" : "sub-journal"));
                    if (isMainJrnl != 0)
                    {
                        isSynced = this.noSync || (pOffset <= this.journalHdr);
                    }
                    else
                    {
                        isSynced = (pPg == null || 0 == (pPg.flags & PGHDR_NEED_SYNC));
                    }
                    if (this.fd.isOpen   && (this.eState >= PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_OPEN) && isSynced)
                    {
                        i64 ofst = (pgno - 1) * this.pageSize;
                        sqliteinth.testcase(0 == isSavepnt && pPg != null && (pPg.flags & PGHDR_NEED_SYNC) != 0);
                        Debug.Assert(!this.pagerUseWal());
                        rc = os.sqlite3OsWrite(this.fd, aData, this.pageSize, ofst);
                        if (pgno > this.dbFileSize)
                        {
                            this.dbFileSize = pgno;
                        }
                        if (this.pBackup != null)
                        {
                            if (PagerMethods.CODEC1(this, aData, pgno, crypto.SQLITE_DECRYPT))
                                rc = SqlResult.SQLITE_NOMEM;
                            // CODEC1( pPager, aData, pgno, 3, rc = SQLITE_NOMEM );
                            this.pBackup.sqlite3BackupUpdate(pgno, (u8[])aData);
                            if (PagerMethods.CODEC2(this, aData, pgno, crypto.SQLITE_ENCRYPT_READ_CTX, ref aData))
                                rc = SqlResult.SQLITE_NOMEM;
                            //CODEC2( pPager, aData, pgno, 7, rc = SQLITE_NOMEM, aData);
                        }
                    }
                    else
                        if (0 == isMainJrnl && pPg == null)
                        {
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

                            Debug.Assert(isSavepnt != 0);
                            Debug.Assert(this.doNotSpill == 0);
                            this.doNotSpill++;
                            rc = this.sqlite3PagerAcquire(pgno, ref pPg, 1);
                            Debug.Assert(this.doNotSpill == 1);
                            this.doNotSpill--;
                            if (rc != SqlResult.SQLITE_OK)
                                return rc;
                            pPg.flags &= ~PGHDR_NEED_READ;
                            PCacheMethods.sqlite3PcacheMakeDirty(pPg);
                        }
                    if (pPg != null)
                    {
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
                        Buffer.BlockCopy(aData, 0, pData, 0, this.pageSize);
                        // memcpy(pData, (u8[])aData, pPager.pageSize);
                        this.xReiniter(pPg);
                        if (isMainJrnl != 0 && (0 == isSavepnt || pOffset <= this.journalHdr))
                        {
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

                            Debug.Assert(!this.pagerUseWal());
                            PCacheMethods.sqlite3PcacheMakeClean(pPg);
                        }
                        pPg.pager_set_pagehash();
                        ///
                        ///<summary>
                        ///If this was page 1, then restore the value of Pager.dbFileVers.
                        ///Do this before any decoding. 
                        ///</summary>

                        if (pgno == 1)
                        {
                            Buffer.BlockCopy(pData, 24, this.dbFileVers, 0, this.dbFileVers.Length);
                            //memcpy(pPager.dbFileVers, ((u8*)pData)[24], sizeof(pPager.dbFileVers));
                        }
                        ///
                        ///<summary>
                        ///Decode the page just read from disk 
                        ///</summary>

                        if (PagerMethods.CODEC1(this, pData, pPg.pgno, crypto.SQLITE_DECRYPT))
                            rc = SqlResult.SQLITE_NOMEM;
                        //PagerMethods.CODEC1(pPager, pData, pPg.pgno, 3, rc=SQLITE_NOMEM);
                        PCacheMethods.sqlite3PcacheRelease(pPg);
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
                      /// the file-system using os.sqlite3OsDelete().
                      ///
                      /// If an IO error within this function, an error code is returned. This
                      /// function allocates memory by calling malloc_cs.sqlite3Malloc(). If an allocation
                      /// fails, SQLITE_NOMEM is returned. Otherwise, if no IO or malloc errors
                      /// occur, SqlResult.SQLITE_OK is returned.
                      ///
                      /// TODO: This function allocates a single block of memory to load
                      /// the entire contents of the master journal file. This could be
                      /// a couple of kilobytes or so - potentially larger than the page
                      /// size.
                      ///
                      ///</summary>
                SqlResult pager_delmaster(string zMaster)
                {
                    sqlite3_vfs pVfs = this.pVfs;
                    SqlResult rc;
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

                    pMaster = new sqlite3_file();
                    // (sqlite3_file*)malloc_cs.sqlite3MallocZero( pVfs.szOsFile * 2 );
                    pJournal = new sqlite3_file();
                    // (sqlite3_file*)( ( (u8*)pMaster ) + pVfs.szOsFile );
                    //if ( null == pMaster )
                    //{
                    //  rc = SQLITE_NOMEM;
                    //}
                    //else
                    {
                        const int flags = (SQLITE_OPEN_READONLY | SQLITE_OPEN_MASTER_JOURNAL);
                        int iDummy = 0;
                        rc = os.sqlite3OsOpen(pVfs, zMaster, pMaster, flags, ref iDummy);
                        //TODO --
                        ///
                        ///<summary>
                        ///Load the entire master journal file into space obtained from
                        ///sqlite3_malloc() and pointed to by zMasterJournal.   Also obtain
                        ///sufficient space (in zMasterPtr) to hold the names of master
                        ///</summary>
                        ///<param name="journal files extracted from regular rollback">journals.</param>
                        ///<param name=""></param>

                        //rc = os.sqlite3OsFileSize(pMaster, &nMasterJournal);
                        //if (rc != SqlResult.SQLITE_OK) goto delmaster_out;
                        //nMasterPtr = pVfs.mxPathname + 1;
                        //  zMasterJournal = malloc_cs.sqlite3Malloc((int)nMasterJournal + nMasterPtr + 1);
                        //  if ( !zMasterJournal )
                        //  {
                        //    rc = SQLITE_NOMEM;
                        //    goto delmaster_out;
                        //  }
                        //  zMasterPtr = &zMasterJournal[nMasterJournal+1];
                        //  rc = os.sqlite3OsRead( pMaster, zMasterJournal, (int)nMasterJournal, 0 );
                        //  if ( rc != SqlResult.SQLITE_OK ) goto delmaster_out;
                        //  zMasterJournal[nMasterJournal] = 0;
                        //  zJournal = zMasterJournal;
                        //  while ( ( zJournal - zMasterJournal ) < nMasterJournal )
                        //  {
                        //    int exists;
                        //    rc = os.sqlite3OsAccess( pVfs, zJournal, SQLITE_ACCESS_EXISTS, &exists );
                        //    if ( rc != SqlResult.SQLITE_OK )
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
                        //      rc = os.sqlite3OsOpen( pVfs, zJournal, pJournal, flags, 0 );
                        //      if ( rc != SqlResult.SQLITE_OK )
                        //      {
                        //        goto delmaster_out;
                        //      }
                        //      rc = readMasterJournal( pJournal, zMasterPtr, nMasterPtr );
                        //      os.sqlite3OsClose( pJournal );
                        //      if ( rc != SqlResult.SQLITE_OK )
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
                        //os.sqlite3OsClose(pMaster);
                        //rc = os.sqlite3OsDelete( pVfs, zMaster, 0 );
                        //malloc_cs.sqlite3_free( ref zMasterJournal );
                    }
                    if (rc != SqlResult.SQLITE_OK)
                        goto delmaster_out;
                    Debugger.Break();
                    goto delmaster_out;
                delmaster_out:
                    if (pMaster != null)
                    {
                        os.sqlite3OsClose(pMaster);
                        Debug.Assert(!pJournal.isOpen);
                        //malloc_cs.sqlite3_free( ref pMaster );
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
                      /// If successful, return SqlResult.SQLITE_OK. If an IO error occurs while modifying
                      /// the database file, return the error code to the caller.
                      ///
                      ///</summary>
                SqlResult pager_truncate(u32 nPage)
                {
                    var rc = SqlResult.SQLITE_OK;
                    Debug.Assert(this.eState != PagerState.PAGER_ERROR);
                    Debug.Assert(this.eState != PagerState.PAGER_READER);
                    if (this.fd.isOpen   && (this.eState >= PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_OPEN))
                    {
                        i64 currentSize = 0, newSize;
                        int szPage = this.pageSize;
                        Debug.Assert(this.eLock == EXCLUSIVE_LOCK);
                        ///
                        ///<summary>
                        ///TODO: Is it safe to use Pager.dbFileSize here? 
                        ///</summary>

                        rc = os.sqlite3OsFileSize(this.fd, ref currentSize);
                        newSize = szPage * nPage;
                        if (rc == SqlResult.SQLITE_OK && currentSize != newSize)
                        {
                            if (currentSize > newSize)
                            {
                                rc = os.sqlite3OsTruncate(this.fd, newSize);
                            }
                            else
                            {
                                byte[] pTmp = this.pTmpSpace;
                                Array.Clear(pTmp, 0, szPage);
                                //memset( pTmp, 0, szPage );
                                sqliteinth.testcase((newSize - szPage) < currentSize);
                                sqliteinth.testcase((newSize - szPage) == currentSize);
                                sqliteinth.testcase((newSize - szPage) > currentSize);
                                rc = os.sqlite3OsWrite(this.fd, pTmp, szPage, newSize - szPage);
                            }
                            if (rc == SqlResult.SQLITE_OK)
                            {
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
                void setSectorSize()
                {
                    Debug.Assert(this.fd.isOpen   || this.tempFile);
                    if (!this.tempFile)
                    {
                        ///
                        ///<summary>
                        ///Sector size doesn't matter for temporary files. Also, the file
                        ///may not have been opened yet, in which case the OsSectorSize()
                        ///call will segfault.
                        ///
                        ///</summary>

                        this.sectorSize = (u32)os.sqlite3OsSectorSize(this.fd);
                    }
                    if (this.sectorSize < 32)
                    {
                        Debug.Assert(MAX_SECTOR_SIZE >= 512);
                        this.sectorSize = 512;
                    }
                    if (this.sectorSize > MAX_SECTOR_SIZE)
                    {
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
                      /// is then deleted and SqlResult.SQLITE_OK returned, just as if no corruption had
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
                SqlResult pager_playback(int isHot)
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

                    SqlResult rc;
                    ///
                    ///<summary>
                    ///Result code of a subroutine 
                    ///</summary>

                    int res = 1;
                    ///
                    ///<summary>
                    ///Value returned by os.sqlite3OsAccess() 
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

                    Debug.Assert(this.jfd.isOpen);
                    rc = os.sqlite3OsFileSize(this.jfd, ref szJ);
                    if (rc != SqlResult.SQLITE_OK)
                    {
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
                    rc = PagerMethods.readMasterJournal(this.jfd, zMaster, (u32)this.pVfs.mxPathname + 1);
                    if (rc == SqlResult.SQLITE_OK && zMaster[0] != 0)
                    {
                        rc = os.sqlite3OsAccess(pVfs, Encoding.UTF8.GetString(zMaster, 0, zMaster.Length), SQLITE_ACCESS_EXISTS, ref res);
                    }
                    zMaster = null;
                    if (rc != SqlResult.SQLITE_OK || res == 0)
                    {
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

                    while (true)
                    {
                        ///
                        ///<summary>
                        ///Read the next journal header from the journal file.  If there are
                        ///not enough bytes left in the journal file for a complete header, or
                        ///it is corrupted, then a process must have failed while writing it.
                        ///This indicates nothing more needs to be rolled back.
                        ///
                        ///</summary>

                        rc = this.readJournalHdr(isHot, szJ, out nRec, out mxPg);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            if (rc == SqlResult.SQLITE_DONE)
                            {
                                rc = SqlResult.SQLITE_OK;
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

                        if (nRec == 0xffffffff)
                        {
                            Debug.Assert(this.journalOff == this.JOURNAL_HDR_SZ());
                            nRec = (u32)((szJ - this.JOURNAL_HDR_SZ()) / this.JOURNAL_PG_SZ());
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

                        if (nRec == 0 && 0 == isHot && this.journalHdr + this.JOURNAL_HDR_SZ() == this.journalOff)
                        {
                            nRec = (u32)((szJ - this.journalOff) / this.JOURNAL_PG_SZ());
                        }
                        ///
                        ///<summary>
                        ///If this is the first header read from the journal, truncate the
                        ///database file back to its original size.
                        ///
                        ///</summary>

                        if (this.journalOff == this.JOURNAL_HDR_SZ())
                        {
                            rc = this.pager_truncate(mxPg);
                            if (rc != SqlResult.SQLITE_OK)
                            {
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

                        for (u = 0; u < nRec; u++)
                        {
                            if (needPagerReset != 0)
                            {
                                this.pager_reset();
                                needPagerReset = 0;
                            }
                            rc = this.pager_playback_one_page(ref this.journalOff, null, 1, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                if (rc == SqlResult.SQLITE_DONE)
                                {
                                    rc = SqlResult.SQLITE_OK;
                                    this.journalOff = szJ;
                                    break;
                                }
                                else
                                    if (rc == SqlResult.SQLITE_IOERR_SHORT_READ)
                                    {
                                        ///
                                        ///<summary>
                                        ///If the journal has been truncated, simply stop reading and
                                        ///processing the journal. This might happen if the journal was
                                        ///not completely written and synced prior to a crash.  In that
                                        ///case, the database should have never been written in the
                                        ///first place so it is OK to simply abandon the rollback. 
                                        ///</summary>

                                        rc = SqlResult.SQLITE_OK;
                                        goto end_playback;
                                    }
                                    else
                                    {
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
                    Debug.Assert(this.fd.pMethods == null || os.sqlite3OsFileControl(this.fd, SQLITE_FCNTL_DB_UNCHANGED, ref iDummy) >= SqlResult.SQLITE_OK);
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
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        zMaster = new byte[this.pVfs.mxPathname + 1];
                        //pPager.pTmpSpace );
                        rc = PagerMethods.readMasterJournal(this.jfd, zMaster, (u32)this.pVfs.mxPathname + 1);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                    }
                    if (rc == SqlResult.SQLITE_OK && (this.eState >= PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_OPEN))
                    {
                        rc = this.sqlite3PagerSync();
                    }
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = this.pager_end_transaction(zMaster[0] != '\0' ? 1 : 0);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                    }
                    if (rc == SqlResult.SQLITE_OK && zMaster[0] != '\0' && res != 0)
                    {
                        ///
                        ///<summary>
                        ///If there was a master journal and this routine will return success,
                        ///see if it is possible to delete the master journal.
                        ///
                        ///</summary>

                        rc = this.pager_delmaster(Encoding.UTF8.GetString(zMaster, 0, zMaster.Length));
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                    }
                    ///
                    ///<summary>
                    ///The Pager.sectorSize variable may have been updated while rolling
                    ///back a journal created by a process with a different sector size
                    ///value. Reset it to the correct value for this process.
                    ///
                    ///</summary>

                    this.setSectorSize();
                    return rc;
                }

            ///<summary>
            /// This function is called as part of the transition from PagerState.PAGER_OPEN
            /// to PagerState.PAGER_READER state to determine the size of the database file
            /// in pages (assuming the page size currently stored in Pager.pageSize).
            ///
            /// If no error occurs, SqlResult.SQLITE_OK is returned and the size of the database
            /// in pages is stored in *pnPage. Otherwise, an error code (perhaps
            /// SQLITE_IOERR_FSTAT) is returned and *pnPage is left unmodified.
            ///
            ///</summary>
                public SqlResult  pagerPagecount(ref Pgno pnPage)
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

                    Debug.Assert(this.eState == PagerState.PAGER_OPEN);
                    Debug.Assert(this.eLock >= SHARED_LOCK || this.noReadlock != 0);
                    nPage = wal.sqlite3WalDbsize(this.pWal);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the database size was not available from the WAL sub">system,</param>
                    ///<param name="determine it based on the size of the database file. If the size">determine it based on the size of the database file. If the size</param>
                    ///<param name="of the database file is not an integer multiple of the page">size,</param>
                    ///<param name="round down to the nearest page. Except, any file larger than 0">round down to the nearest page. Except, any file larger than 0</param>
                    ///<param name="bytes in size is considered to contain at least one page.">bytes in size is considered to contain at least one page.</param>
                    ///<param name=""></param>

                    if (nPage == 0)
                    {
                        i64 n = 0;
                        ///
                        ///<summary>
                        ///Size of db file in bytes 
                        ///</summary>

                        Debug.Assert(this.fd.isOpen   || this.tempFile);
                        if (this.fd.isOpen  )
                        {
                            var rc = os.sqlite3OsFileSize(this.fd, ref n);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                        }
                        nPage = (Pgno)(n / this.pageSize);
                        if (nPage == 0 && n > 0)
                        {
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

                    if (nPage > this.mxPgno)
                    {
                        this.mxPgno = (Pgno)nPage;
                    }
                    pnPage = nPage;
                    return SqlResult.SQLITE_OK;
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
                SqlResult pagerPlaybackSavepoint(PagerSavepoint pSavepoint)
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

                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    Bitvec pDone = null;
                    ///
                    ///<summary>
                    ///Bitvec to ensure pages played back only once 
                    ///</summary>

                    Debug.Assert(this.eState != PagerState.PAGER_ERROR);
                    Debug.Assert(this.eState >= PagerState.PAGER_WRITER_LOCKED);
                    ///
                    ///<summary>
                    ///Allocate a bitvec to use to store the set of pages rolled back 
                    ///</summary>

                    if (pSavepoint != null)
                    {
                        pDone = Bitvec.sqlite3BitvecCreate(pSavepoint.nOrig);
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
                    if (!pSavepoint && this.pagerUseWal())
                    {
                        return this.pagerRollbackWal();
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
                    Debug.Assert(this.pagerUseWal() == false || szJ == 0);
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

                    if (pSavepoint != null && !this.pagerUseWal())
                    {
                        iHdrOff = pSavepoint.iHdrOffset != 0 ? pSavepoint.iHdrOffset : szJ;
                        this.journalOff = pSavepoint.iOffset;
                        while (rc == SqlResult.SQLITE_OK && this.journalOff < iHdrOff)
                        {
                            rc = this.pager_playback_one_page(ref this.journalOff, pDone, 1, 1);
                        }
                        Debug.Assert(rc != SqlResult.SQLITE_DONE);
                    }
                    else
                    {
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

                    while (rc == SqlResult.SQLITE_OK && this.journalOff < szJ)
                    {
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
                        rc = this.readJournalHdr(0, (int)szJ, out nJRec, out dummy);
                        Debug.Assert(rc != SqlResult.SQLITE_DONE);
                        ///
                        ///<summary>
                        ///The "pPager.journalHdr+JOURNAL_HDR_SZ(pPager)==pPager.journalOff"
                        ///test is related to ticket #2565.  See the discussion in the
                        ///pager_playback() function for additional information.
                        ///
                        ///</summary>

                        if (nJRec == 0 && this.journalHdr + this.JOURNAL_HDR_SZ() >= this.journalOff)
                        {
                            nJRec = (u32)((szJ - this.journalOff) / this.JOURNAL_PG_SZ());
                        }
                        for (ii = 0; rc == SqlResult.SQLITE_OK && ii < nJRec && this.journalOff < szJ; ii++)
                        {
                            rc = this.pager_playback_one_page(ref this.journalOff, pDone, 1, 1);
                        }
                        Debug.Assert(rc != SqlResult.SQLITE_DONE);
                    }
                    Debug.Assert(rc != SqlResult.SQLITE_OK || this.journalOff >= szJ);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Finally,  rollback pages from the sub">journal.  Page that were</param>
                    ///<param name="previously rolled back out of the main journal (and are hence in pDone)">previously rolled back out of the main journal (and are hence in pDone)</param>
                    ///<param name="will be skipped.  Out">range pages are also skipped.</param>
                    ///<param name=""></param>

                    if (pSavepoint != null)
                    {
                        u32 ii;
                        ///
                        ///<summary>
                        ///Loop counter 
                        ///</summary>

                        i64 offset = pSavepoint.iSubRec * (4 + this.pageSize);
                        if (this.pagerUseWal())
                        {
                            rc = wal.sqlite3WalSavepointUndo(this.pWal, pSavepoint.aWalData);
                        }
                        for (ii = pSavepoint.iSubRec; rc == SqlResult.SQLITE_OK && ii < this.nSubRec; ii++)
                        {
                            Debug.Assert(offset == ii * (4 + this.pageSize));
                            rc = this.pager_playback_one_page(ref offset, pDone, 0, 1);
                        }
                        Debug.Assert(rc != SqlResult.SQLITE_DONE);
                    }
                    sqlite3BitvecDestroy(ref pDone);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        this.journalOff = (int)szJ;
                    }
                    return rc;
                }

                public void sqlite3PagerSetCachesize(int mxPage)
                {
                    PCacheMethods.sqlite3PcacheSetCachesize(this.pPCache, mxPage);
                }

                public///<summary>
                    /// Adjust the robustness of the database to damage due to OS crashes
                    /// or power failures by changing the number of syncs()s when writing
                    /// the rollback journal.  There are three levels:
                    ///
                    ///    OFF       os.sqlite3OsSync() is never called.  This is the default
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
                void sqlite3PagerSetSafetyLevel(///
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
                    Debug.Assert(level >= 1 && level <= 3);
                    this.noSync = (level == 1 || this.tempFile);
                    this.fullSync = (level == 3 && !this.tempFile);
                    if (this.noSync)
                    {
                        this.syncFlags = 0;
                        this.ckptSyncFlags = 0;
                    }
                    else
                        if (bFullFsync != 0)
                        {
                            this.syncFlags = SQLITE_SYNC_FULL;
                            this.ckptSyncFlags = SQLITE_SYNC_FULL;
                        }
                        else
                            if (bCkptFullFsync != 0)
                            {
                                this.syncFlags = SQLITE_SYNC_NORMAL;
                                this.ckptSyncFlags = SQLITE_SYNC_FULL;
                            }
                            else
                            {
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
                    /// Write the file descriptor into *pFile. Return SqlResult.SQLITE_OK on success
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
                SqlResult pagerOpentemp(///
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
                    SqlResult rc;
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
                    rc = os.sqlite3OsOpen(this.pVfs, null, pFile, vfsFlags, ref dummy);
                    Debug.Assert(rc != SqlResult.SQLITE_OK || pFile.isOpen);
                    return rc;
                }

                public///<summary>
                    /// Set the busy handler function.
                    ///
                    /// The pager invokes the busy-handler if os.sqlite3OsLock() returns
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
                void sqlite3PagerSetBusyhandler(///
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
                      /// In all other cases, SqlResult.SQLITE_OK is returned.
                      ///
                      /// If the page size is not changed, either because one of the enumerated
                      /// conditions above is not true, the pager was in error state when this
                      /// function was called, or because the memory allocation attempt failed,
                      /// then *pPageSize is set to the old, retained page size before returning.
                      ///
                      ///</summary>
                SqlResult sqlite3PagerSetPagesize(ref u32 pPageSize, int nReserve)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///It is not possible to do a full assert_pager_state() here, as this
                    ///function may be called from within PagerOpen(), before the state
                    ///of the Pager object is internally consistent.
                    ///
                    ///At one point this function returned an error if the pager was in 
                    ///PagerState.PAGER_ERROR state. But since PagerState.PAGER_ERROR state guarantees that
                    ///there is at least one outstanding page reference, this function
                    ///</summary>
                    ///<param name="is a no">op for that case anyhow.</param>
                    ///<param name=""></param>

                    u32 pageSize = pPageSize;
                    Debug.Assert(pageSize == 0 || (pageSize >= 512 && pageSize <= SQLITE_MAX_PAGE_SIZE));
                    if ((this.memDb == 0 || this.dbSize == 0) && PCacheMethods.sqlite3PcacheRefCount(this.pPCache) == 0 && pageSize != 0 && pageSize != (u32)this.pageSize)
                    {
                        //char *pNew = NULL;             /* New temp space */
                        i64 nByte = 0;
                        if (this.eState > PagerState.PAGER_OPEN && this.fd.isOpen  )
                        {
                            rc = os.sqlite3OsFileSize(this.fd, ref nByte);
                        }
                        //if ( rc == SqlResult.SQLITE_OK )
                        //{
                        //pNew = (char *)sqlite3PageMalloc(pageSize);
                        //if( !pNew ) rc = SQLITE_NOMEM;
                        //}
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            this.pager_reset();
                            this.dbSize = (Pgno)(nByte / pageSize);
                            this.pageSize = (int)pageSize;
                            sqlite3PageFree(ref this.pTmpSpace);
                            this.pTmpSpace = malloc_cs.sqlite3Malloc(pageSize);
                            // pNew;
                            PCacheMethods.sqlite3PcacheSetPageSize(this.pPCache, (int)pageSize);
                        }
                    }
                    pPageSize = (u32)this.pageSize;
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        if (nReserve < 0)
                            nReserve = this.nReserve;
                        Debug.Assert(nReserve >= 0 && nReserve < 1000);
                        this.nReserve = (i16)nReserve;
                        this.pagerReportSize();
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
                byte[] sqlite3PagerTempSpace()
                {
                    return this.pTmpSpace;
                }

                public Pgno sqlite3PagerMaxPageCount(int mxPage)
                {
                    if (mxPage > 0)
                    {
                        this.mxPgno = (Pgno)mxPage;
                    }
                    Debug.Assert(this.eState != PagerState.PAGER_OPEN);
                    ///
                    ///<summary>
                    ///Called only by OP_MaxPgcnt 
                    ///</summary>

                    Debug.Assert(this.mxPgno >= this.dbSize);
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
                    /// zeroed and SqlResult.SQLITE_OK returned. The rationale for this is that this
                    /// function is used to read database headers, and a new transient or
                    /// zero sized database has a header than consists entirely of zeroes.
                    ///
                    /// If any IO error apart from SQLITE_IOERR_SHORT_READ is encountered,
                    /// the error code is returned to the caller and the contents of the
                    /// output buffer undefined.
                    ///</summary>
                SqlResult sqlite3PagerReadFileheader(int N, byte[] pDest)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    Array.Clear(pDest, 0, N);
                    //memset(pDest, 0, N);
                    Debug.Assert(this.fd.isOpen   || this.tempFile);
                    ///
                    ///<summary>
                    ///This routine is only called by btree immediately after creating
                    ///the Pager object.  There has not been an opportunity to transition
                    ///to WAL mode yet.
                    ///
                    ///</summary>

                    Debug.Assert(!this.pagerUseWal());
                    if (this.fd.isOpen  )
                    {
                        sqliteinth.IOTRACE("DBHDR %p 0 %d\n", this, N);
                        rc = os.sqlite3OsRead(this.fd, pDest, N, 0);
                        if (rc == SqlResult.SQLITE_IOERR_SHORT_READ)
                        {
                            rc = SqlResult.SQLITE_OK;
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
                void sqlite3PagerPagecount(out Pgno pnPage)
                {
                    Debug.Assert(this.eState >= PagerState.PAGER_READER);
                    Debug.Assert(this.eState != PagerState.PAGER_WRITER_FINISHED);
                    pnPage = this.dbSize;
                }

                public SqlResult pager_wait_on_lock(int locktype)
                {
                    SqlResult rc;
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

                    Debug.Assert((this.eLock >= locktype) || (this.eLock == NO_LOCK && locktype == SHARED_LOCK) || (this.eLock == RESERVED_LOCK && locktype == EXCLUSIVE_LOCK));
                    do
                    {
                        rc = this.pagerLockDb(locktype);
                    }
                    while (rc == SqlResult.SQLITE_BUSY && this.xBusyHandler(this.pBusyHandlerArg) != 0);
                    return rc;
                }

                public void assertTruncateConstraint()
                {
                }

                public///<summary>
                    /// Truncate the in-memory database file image to nPage pages. This
                    /// function does not actually modify the database file on disk. It
                    /// just sets the internal state of the pager object so that the
                    /// truncation will be done when the current transaction is committed.
                    ///</summary>
                void sqlite3PagerTruncateImage(u32 nPage)
                {
                    Debug.Assert(this.dbSize >= nPage);
                    Debug.Assert(this.eState >= PagerState.PAGER_WRITER_CACHEMOD);
                    this.dbSize = nPage;
                    this.assertTruncateConstraint();
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
                      /// If everything goes as planned, SqlResult.SQLITE_OK is returned. Otherwise,
                      /// an SQLite error code.
                      ///
                      ///</summary>
                SqlResult pagerSyncHotJournal()
                {
                    var rc = SqlResult.SQLITE_OK;
                    if (!this.noSync)
                    {
                        rc = os.sqlite3OsSync(this.jfd, SQLITE_SYNC_NORMAL);
                    }
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = os.sqlite3OsFileSize(this.jfd, ref this.journalHdr);
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
                SqlResult sqlite3PagerClose()
                {
                    u8[] pTmp = this.pTmpSpace;
#if SQLITE_TEST
																																																																						      disable_simulated_io_errors();
#endif
                    sqlite3BeginBenignMalloc();
                    ///
                    ///<summary>
                    ///pPager.errCode = 0; 
                    ///</summary>

                    this.exclusiveMode = false;
#if !SQLITE_OMIT_WAL
																																																																						sqlite3WalClose(pPager->pWal, pPager->ckptSyncFlags, pPager->pageSize, pTmp);
pPager.pWal = 0;
#endif
                    this.pager_reset();
                    if (
#if SQLITE_OMIT_MEMORYDB
																																																																						1==MEMDB
#else
1 == this.memDb
#endif
)
                    {
                        this.pager_unlock();
                    }
                    else
                    {
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

                        if (this.jfd.isOpen)
                        {
                            this.pager_error(this.pagerSyncHotJournal());
                        }
                        this.pagerUnlockAndRollback();
                    }
                    sqlite3EndBenignMalloc();
#if SQLITE_TEST
																																																																						      enable_simulated_io_errors();
#endif
                    PAGERTRACE("CLOSE %d\n", PagerMethods.PAGERID(this));
                    sqliteinth.IOTRACE("CLOSE %p\n", this);
                    os.sqlite3OsClose(this.jfd);
                    os.sqlite3OsClose(this.fd);
                    //malloc_cs.sqlite3_free( ref pTmp );
                    PCacheMethods.sqlite3PcacheClose(this.pPCache);
#if SQLITE_HAS_CODEC
                    if (this.xCodecFree != null)
                        this.xCodecFree(ref this.pCodec);
#endif
                    Debug.Assert(null == this.aSavepoint && !this.pInJournal);
                    Debug.Assert(!this.jfd.isOpen && !this.sjfd.isOpen);
                    //malloc_cs.sqlite3_free( ref pPager );
                    return SqlResult.SQLITE_OK;
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
                      /// page currently held in memory before returning SqlResult.SQLITE_OK. If an IO
                      /// error is encountered, then the IO error code is returned to the caller.
                      ///
                      ///</summary>
                SqlResult syncJournal(int newHdr)
                {
                    var rc = SqlResult.SQLITE_OK;
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD);
                    Debug.Assert(this.assert_pager_state());
                    Debug.Assert(!this.pagerUseWal());
                    rc = this.sqlite3PagerExclusiveLock();
                    if (rc != SqlResult.SQLITE_OK)
                        return rc;
                    if (!this.noSync)
                    {
                        Debug.Assert(!this.tempFile);
                        if (this.jfd.isOpen && this.journalMode != PAGER_JOURNALMODE_MEMORY)
                        {
                            int iDc = os.sqlite3OsDeviceCharacteristics(this.fd);
                            Debug.Assert(this.jfd.isOpen);
                            if (0 == (iDc & SQLITE_IOCAP_SAFE_APPEND))
                            {
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
                                aJournalMagic.CopyTo(zHeader, 0);
                                // memcpy(zHeader, aJournalMagic, sizeof(aJournalMagic));
                                Converter.put32bits(zHeader, aJournalMagic.Length, this.nRec);
                                iNextHdrOffset = this.journalHdrOffset();
                                rc = os.sqlite3OsRead(this.jfd, aMagic, 8, iNextHdrOffset);
                                if (rc == SqlResult.SQLITE_OK && 0 == _Custom.memcmp(aMagic, aJournalMagic, 8))
                                {
                                    u8[] zerobyte = new u8[1];
                                    rc = os.sqlite3OsWrite(this.jfd, zerobyte, 1, iNextHdrOffset);
                                }
                                if (rc != SqlResult.SQLITE_OK && rc != SqlResult.SQLITE_IOERR_SHORT_READ)
                                {
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

                                if (this.fullSync && 0 == (iDc & SQLITE_IOCAP_SEQUENTIAL))
                                {
                                    PAGERTRACE("SYNC journal of %d\n", PagerMethods.PAGERID(this));
                                    sqliteinth.IOTRACE("JSYNC %p\n", this);
                                    rc = os.sqlite3OsSync(this.jfd, this.syncFlags);
                                    if (rc != SqlResult.SQLITE_OK)
                                        return rc;
                                }
                                sqliteinth.IOTRACE("JHDR %p %lld\n", this, this.journalHdr);
                                rc = os.sqlite3OsWrite(this.jfd, zHeader, zHeader.Length, this.journalHdr);
                                if (rc != SqlResult.SQLITE_OK)
                                    return rc;
                            }
                            if (0 == (iDc & SQLITE_IOCAP_SEQUENTIAL))
                            {
                                PAGERTRACE("SYNC journal of %d\n", PagerMethods.PAGERID(this));
                                sqliteinth.IOTRACE("JSYNC %p\n", this);
                                rc = os.sqlite3OsSync(this.jfd, this.syncFlags | (this.syncFlags == SQLITE_SYNC_FULL ? SQLITE_SYNC_DATAONLY : 0));
                                if (rc != SqlResult.SQLITE_OK)
                                    return rc;
                            }
                            this.journalHdr = this.journalOff;
                            if (newHdr != 0 && 0 == (iDc & SQLITE_IOCAP_SAFE_APPEND))
                            {
                                this.nRec = 0;
                                rc = this.writeJournalHdr();
                                if (rc != SqlResult.SQLITE_OK)
                                    return rc;
                            }
                        }
                        else
                        {
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

                    PCacheMethods.sqlite3PcacheClearSyncFlags(this.pPCache);
                    this.eState = PagerState.PAGER_WRITER_DBMOD;
                    Debug.Assert(this.assert_pager_state());
                    return SqlResult.SQLITE_OK;
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
                      /// If everything is successful, SqlResult.SQLITE_OK is returned. If an IO error
                      /// occurs, an IO error code is returned. Or, if the EXCLUSIVE lock cannot
                      /// be obtained, SQLITE_BUSY is returned.
                      ///
                      ///</summary>
                SqlResult pager_write_pagelist(PgHdr pList)
                {
                    var rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    ///
                    ///<summary>
                    ///This function is only called for rollback pagers in WRITER_DBMOD state. 
                    ///</summary>

                    Debug.Assert(!this.pagerUseWal());
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_DBMOD);
                    Debug.Assert(this.eLock == EXCLUSIVE_LOCK);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the file is a temp">file has not yet been opened, open it now. It</param>
                    ///<param name="is not possible for rc to be other than SqlResult.SQLITE_OK if this branch">is not possible for rc to be other than SqlResult.SQLITE_OK if this branch</param>
                    ///<param name="is taken, as pager_wait_on_lock() is a no">files.</param>
                    ///<param name=""></param>

                    if (!this.fd.isOpen  )
                    {
                        Debug.Assert(this.tempFile && rc == SqlResult.SQLITE_OK);
                        rc = this.pagerOpentemp(ref this.fd, (int)this.vfsFlags);
                    }
                    ///
                    ///<summary>
                    ///Before the first write, give the VFS a hint of what the final
                    ///file size will be.
                    ///
                    ///</summary>

                    Debug.Assert(rc != SqlResult.SQLITE_OK || this.fd.isOpen  );
                    if (rc == SqlResult.SQLITE_OK && this.dbSize > this.dbHintSize)
                    {
                        sqlite3_int64 szFile = this.pageSize * (sqlite3_int64)this.dbSize;
                        os.sqlite3OsFileControl(this.fd, SQLITE_FCNTL_SIZE_HINT, ref szFile);
                        this.dbHintSize = this.dbSize;
                    }
                    while (rc == SqlResult.SQLITE_OK && pList)
                    {
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

                        if (pList.pgno <= this.dbSize && 0 == (pList.flags & PGHDR_DONT_WRITE))
                        {
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

                            Debug.Assert((pList.flags & PGHDR_NEED_SYNC) == 0);
                            if (pList.pgno == 1)
                                pList.pager_write_changecounter();
                            ///
                            ///<summary>
                            ///Encode the database 
                            ///</summary>

                            if (PagerMethods.CODEC2(this, pList.pData, pgno, crypto.SQLITE_ENCRYPT_WRITE_CTX, ref pData))
                                return SqlResult.SQLITE_NOMEM;
                            //     PagerMethods.CODEC2(pPager, pList.pData, pgno, 6, return SQLITE_NOMEM, pData);
                            ///
                            ///<summary>
                            ///Write out the page data. 
                            ///</summary>

                            rc = os.sqlite3OsWrite(this.fd, pData, this.pageSize, offset);
                            ///
                            ///<summary>
                            ///If page 1 was just written, update Pager.dbFileVers to match
                            ///the value now stored in the database file. If writing this
                            ///page caused the database file to grow, update dbFileSize.
                            ///
                            ///</summary>

                            if (pgno == 1)
                            {
                                Buffer.BlockCopy(pData, 24, this.dbFileVers, 0, this.dbFileVers.Length);
                                // memcpy(pPager.dbFileVers, pData[24], pPager.dbFileVers).Length;
                            }
                            if (pgno > this.dbFileSize)
                            {
                                this.dbFileSize = pgno;
                            }
                            ///
                            ///<summary>
                            ///Update any backup objects copying the contents of this pager. 
                            ///</summary>

                            this.pBackup.sqlite3BackupUpdate(pgno, pList.pData);
                            PAGERTRACE("STORE %d page %d hash(%08x)\n", PagerMethods.PAGERID(this), pgno, pList.pager_pagehash());
                            sqliteinth.IOTRACE("PGOUT %p %d\n", this, pgno);
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
                        else
                        {
                            PAGERTRACE("NOSTORE %d page %d\n", PagerMethods.PAGERID(this), pgno);
                        }
                        pList.pager_set_pagehash();
                        pList = pList.pDirty;
                    }
                    return rc;
                }

                public///<summary>
                    /// Ensure that the sub-journal file is open. If it is already open, this
                    /// function is a no-op.
                    ///
                    /// SqlResult.SQLITE_OK is returned if everything goes according to plan. An
                    /// SQLITE_IOERR_XXX error code is returned if a call to os.sqlite3OsOpen()
                    /// fails.
                    ///
                    ///</summary>
                SqlResult openSubJournal()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    if (!this.sjfd.isOpen )
                    {
                        if (this.journalMode == PAGER_JOURNALMODE_MEMORY || this.subjInMemory != 0)
                        {
                            memjrnl.sqlite3MemJournalOpen(this.sjfd);
                        }
                        else
                        {
                            rc = this.pagerOpentemp(ref this.sjfd, SQLITE_OPEN_SUBJOURNAL);
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
                      /// just deleted using OsDelete, *pExists is set to 0 and SqlResult.SQLITE_OK
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
                      /// SqlResult.SQLITE_OK returned. If no hot-journal file is present, *pExists is
                      /// set to 0 and SqlResult.SQLITE_OK returned. If an IO error occurs while trying
                      /// to determine whether or not a hot-journal file exists, the IO error
                      /// code is returned and the value of *pExists is undefined.
                      ///
                      ///</summary>
                SqlResult hasHotJournal(ref int pExists)
                {
                    sqlite3_vfs pVfs = this.pVfs;
                    var rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    int exists = 1;
                    ///
                    ///<summary>
                    ///True if a journal file is present 
                    ///</summary>

                    int jrnlOpen = this.jfd.isOpen ? 1 : 0;
                    Debug.Assert(this.useJournal != 0);
                    Debug.Assert(this.fd.isOpen  );
                    Debug.Assert(this.eState == PagerState.PAGER_OPEN);
                    Debug.Assert(jrnlOpen == 0 || (os.sqlite3OsDeviceCharacteristics(this.jfd) & SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN) != 0);
                    pExists = 0;
                    if (0 == jrnlOpen)
                    {
                        rc = os.sqlite3OsAccess(pVfs, this.zJournal, SQLITE_ACCESS_EXISTS, ref exists);
                    }
                    if (rc == SqlResult.SQLITE_OK && exists != 0)
                    {
                        int locked = 0;
                        ///
                        ///<summary>
                        ///True if some process holds a RESERVED lock 
                        ///</summary>

                        ///
                        ///<summary>
                        ///Race condition here:  Another process might have been holding the
                        ///the RESERVED lock and have a journal open at the os.sqlite3OsAccess()
                        ///call above, but then delete the journal and drop the lock before
                        ///we get to the following os.sqlite3OsCheckReservedLock() call.  If that
                        ///is the case, this routine might think there is a hot journal when
                        ///</summary>
                        ///<param name="in fact there is none.  This results in a false">positive which will</param>
                        ///<param name="be dealt with by the playback routine.  Ticket #3883.">be dealt with by the playback routine.  Ticket #3883.</param>
                        ///<param name=""></param>

                        rc = os.sqlite3OsCheckReservedLock(this.fd, ref locked);
                        if (rc == SqlResult.SQLITE_OK && locked == 0)
                        {
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

                            rc = this.pagerPagecount(ref nPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                if (nPage == 0)
                                {
                                    sqlite3BeginBenignMalloc();
                                    if (this.pagerLockDb(RESERVED_LOCK) == SqlResult.SQLITE_OK)
                                    {
                                        os.sqlite3OsDelete(pVfs, this.zJournal, 0);
                                        if (!this.exclusiveMode)
                                            this.pagerUnlockDb(SHARED_LOCK);
                                    }
                                    sqlite3EndBenignMalloc();
                                }
                                else
                                {
                                    ///
                                    ///<summary>
                                    ///The journal file exists and no other connection has a reserved
                                    ///or greater lock on the database file. Now check that there is
                                    ///</summary>
                                    ///<param name="at least one non">zero bytes at the start of the journal file.</param>
                                    ///<param name="If there is, then we consider this journal to be hot. If not,">If there is, then we consider this journal to be hot. If not,</param>
                                    ///<param name="it can be ignored.">it can be ignored.</param>
                                    ///<param name=""></param>

                                    if (0 == jrnlOpen)
                                    {
                                        int f = SQLITE_OPEN_READONLY | SQLITE_OPEN_MAIN_JOURNAL;
                                        rc = os.sqlite3OsOpen(pVfs, this.zJournal, this.jfd, f, ref f);
                                    }
                                    if (rc == SqlResult.SQLITE_OK)
                                    {
                                        u8[] first = new u8[1];
                                        rc = os.sqlite3OsRead(this.jfd, first, 1, 0);
                                        if (rc == SqlResult.SQLITE_IOERR_SHORT_READ)
                                        {
                                            rc = SqlResult.SQLITE_OK;
                                        }
                                        if (0 == jrnlOpen)
                                        {
                                            os.sqlite3OsClose(this.jfd);
                                        }
                                        pExists = (first[0] != 0) ? 1 : 0;
                                    }
                                    else
                                        if (rc == SqlResult.SQLITE_CANTOPEN)
                                        {
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
                                            rc = SqlResult.SQLITE_OK;
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
                      ///   1) If the pager is currently in PagerState.PAGER_OPEN state (no lock held
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
                      /// If everything is successful, SqlResult.SQLITE_OK is returned. If an IO error
                      /// occurs while locking the database, checking for a hot-journal file or
                      /// rolling back a journal file, the IO error code is returned.
                      ///
                      ///</summary>
                SqlResult sqlite3PagerSharedLock()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
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

                    Debug.Assert(PCacheMethods.sqlite3PcacheRefCount(this.pPCache) == 0);
                    Debug.Assert(this.assert_pager_state());
                    Debug.Assert(this.eState == PagerState.PAGER_OPEN || this.eState == PagerState.PAGER_READER);
                    if (NEVER(
#if SQLITE_OMIT_MEMORYDB
																																																																						0!=MEMDB
#else
0 != this.memDb
#endif
 && this.errCode != 0))
                    {
                        return this.errCode;
                    }
                    if (!this.pagerUseWal() && this.eState == PagerState.PAGER_OPEN)
                    {
                        int bHotJournal = 1;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="True if there exists a hot journal">file </param>

                        Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
0 == this.memDb
#endif
);
                        Debug.Assert(this.noReadlock == 0 || this.readOnly);
                        if (this.noReadlock == 0)
                        {
                            rc = this.pager_wait_on_lock(SHARED_LOCK);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                Debug.Assert(this.eLock == NO_LOCK || this.eLock == UNKNOWN_LOCK);
                                goto failed;
                            }
                        }
                        ///
                        ///<summary>
                        ///If a journal file exists, and there is no RESERVED lock on the
                        ///database file, then it either needs to be played back or deleted.
                        ///
                        ///</summary>

                        if (this.eLock <= SHARED_LOCK)
                        {
                            rc = this.hasHotJournal(ref bHotJournal);
                        }
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            goto failed;
                        }
                        if (bHotJournal != 0)
                        {
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

                            rc = this.pagerLockDb(EXCLUSIVE_LOCK);
                            if (rc != SqlResult.SQLITE_OK)
                            {
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

                            if (!this.jfd.isOpen)
                            {
                                sqlite3_vfs pVfs = this.pVfs;
                                int bExists = 0;
                                ///
                                ///<summary>
                                ///True if journal file exists 
                                ///</summary>

                                rc = os.sqlite3OsAccess(pVfs, this.zJournal, SQLITE_ACCESS_EXISTS, ref bExists);
                                if (rc == SqlResult.SQLITE_OK && bExists != 0)
                                {
                                    int fout = 0;
                                    int f = SQLITE_OPEN_READWRITE | SQLITE_OPEN_MAIN_JOURNAL;
                                    Debug.Assert(!this.tempFile);
                                    rc = os.sqlite3OsOpen(pVfs, this.zJournal, this.jfd, f, ref fout);
                                    Debug.Assert(rc != SqlResult.SQLITE_OK || this.jfd.isOpen);
                                    if (rc == SqlResult.SQLITE_OK && (fout & SQLITE_OPEN_READONLY) != 0)
                                    {
                                        rc = sqliteinth.SQLITE_CANTOPEN_BKPT();
                                        os.sqlite3OsClose(this.jfd);
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

                            if (this.jfd.isOpen)
                            {
                                Debug.Assert(rc == SqlResult.SQLITE_OK);
                                rc = this.pagerSyncHotJournal();
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    rc = this.pager_playback(1);
                                    this.eState = PagerState.PAGER_OPEN;
                                }
                            }
                            else
                                if (!this.exclusiveMode)
                                {
                                    this.pagerUnlockDb(SHARED_LOCK);
                                }
                            if (rc != SqlResult.SQLITE_OK)
                            {
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
                                ///<param name="PagerState.PAGER_ERROR now. This is not actually counted as a transition">PagerState.PAGER_ERROR now. This is not actually counted as a transition</param>
                                ///<param name="to ERROR state in the state diagram at the top of this file,">to ERROR state in the state diagram at the top of this file,</param>
                                ///<param name="since we know that the same call to pager_unlock() will very">since we know that the same call to pager_unlock() will very</param>
                                ///<param name="shortly transition the pager object to the OPEN state. Calling">shortly transition the pager object to the OPEN state. Calling</param>
                                ///<param name="assert_pager_state() would fail now, as it should not be possible">assert_pager_state() would fail now, as it should not be possible</param>
                                ///<param name="to be in ERROR state when there are zero outstanding page ">to be in ERROR state when there are zero outstanding page </param>
                                ///<param name="references.">references.</param>
                                ///<param name=""></param>

                                this.pager_error(rc);
                                goto failed;
                            }
                            Debug.Assert(this.eState == PagerState.PAGER_OPEN);
                            Debug.Assert((this.eLock == SHARED_LOCK) || (this.exclusiveMode && this.eLock > SHARED_LOCK));
                        }
                        if (!this.tempFile && (this.pBackup != null || PCacheMethods.sqlite3PcachePagecount(this.pPCache) > 0))
                        {
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
                            rc = this.pagerPagecount(ref nPage);
                            if (rc != 0)
                                goto failed;
                            if (nPage > 0)
                            {
                                sqliteinth.IOTRACE("CKVERS %p %d\n", this, dbFileVers.Length);
                                rc = os.sqlite3OsRead(this.fd, dbFileVers, dbFileVers.Length, 24);
                                if (rc != SqlResult.SQLITE_OK)
                                {
                                    goto failed;
                                }
                            }
                            else
                            {
                                Array.Clear(dbFileVers, 0, dbFileVers.Length);
                                // memset( dbFileVers, 0, sizeof( dbFileVers ) );
                            }
                            if (_Custom.memcmp(this.dbFileVers, dbFileVers, dbFileVers.Length) != 0)
                            {
                                this.pager_reset();
                            }
                        }
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If there is a WAL file in the file">system, open this database in WAL</param>
                        ///<param name="mode. Otherwise, the following function call is a no">op.</param>
                        ///<param name=""></param>

                        rc = this.pagerOpenWalIfPresent();
#if !SQLITE_OMIT_WAL
																																																																																									Debug.Assert( pPager.pWal == null || rc == SqlResult.SQLITE_OK );
#endif
                    }
                    if (this.pagerUseWal())
                    {
                        Debug.Assert(rc == SqlResult.SQLITE_OK);
                        rc = this.pagerBeginReadTransaction();
                    }
                    if (this.eState == PagerState.PAGER_OPEN && rc == SqlResult.SQLITE_OK)
                    {
                        rc = this.pagerPagecount(ref this.dbSize);
                    }
                failed:
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
0 == this.memDb
#endif
);
                        this.pager_unlock();
                        Debug.Assert(this.eState == PagerState.PAGER_OPEN);
                    }
                    else
                    {
                        this.eState = PagerState.PAGER_READER;
                    }
                    return rc;
                }

                public void pagerUnlockIfUnused()
                {
                    if (PCacheMethods.sqlite3PcacheRefCount(this.pPCache) == 0)
                    {
                        this.pagerUnlockAndRollback();
                    }
                }

                public///<summary>
                      /// Acquire a reference to page number pgno in pager pPager (a page
                      /// reference has type DbPage*). If the requested reference is
                      /// successfully obtained, it is copied to *ppPage and SqlResult.SQLITE_OK returned.
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
                      /// point in the future, using a call to PagerMethods.sqlite3PagerWrite(), its contents
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
                SqlResult sqlite3PagerGet(///
                    ///The pager open on the database file 

                u32 pgno, ///
                    ///Page number to fetch 

                ref DbPage ppPage///
                    ///Write a pointer to the page here 

                )
                {
                    return this.sqlite3PagerAcquire(pgno, ref ppPage, 0);
                }

                public SqlResult sqlite3PagerAcquire(///
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
                    SqlResult rc;
                    PgHdr pPg = null;
                    Debug.Assert(this.eState >= PagerState.PAGER_READER);
                    Debug.Assert(this.assert_pager_state());
                    if (pgno == 0)
                    {
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    }
                    ///
                    ///<summary>
                    ///If the pager is in the error state, return an error immediately. 
                    ///Otherwise, request the page from the PCache layer. 
                    ///</summary>

                    if (this.errCode != SqlResult.SQLITE_OK)
                    {
                        rc = this.errCode;
                    }
                    else
                    {
                        rc = PCacheMethods.sqlite3PcacheFetch(this.pPCache, pgno, 1, ref ppPage);
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        ///
                        ///<summary>
                        ///Either the call to sqlite3PcacheFetch() returned an error or the
                        ///</summary>
                        ///<param name="pager was already in the error">state when this function was called.</param>
                        ///<param name="Set pPg to 0 and jump to the exception handler.  ">Set pPg to 0 and jump to the exception handler.  </param>

                        pPg = null;
                        goto pager_acquire_err;
                    }
                    Debug.Assert((ppPage).pgno == pgno);
                    Debug.Assert((ppPage).pPager == this || (ppPage).pPager == null);
                    if ((ppPage).pPager != null && 0 == noContent)
                    {
                        ///
                        ///<summary>
                        ///In this case the pcache already contains an initialized copy of
                        ///the page. Return without further ado.  
                        ///</summary>

                        Debug.Assert(pgno <= PAGER_MAX_PGNO && pgno != PAGER_MJ_PGNO(this));
                        PagerMethods.PAGER_INCR(ref this.nHit);
                        return SqlResult.SQLITE_OK;
                    }
                    else
                    {
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
                        pPg.pExtra = new MemPage();
                        //memset(pPg.pExtra, 0, pPager.nExtra);
                        ///
                        ///<summary>
                        ///The maximum page number is 2^31. Return SQLITE_CORRUPT if a page
                        ///</summary>
                        ///<param name="number greater than this, or the unused locking">page, is requested. </param>

                        if (pgno > PAGER_MAX_PGNO || pgno == PAGER_MJ_PGNO(this))
                        {
                            rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                            goto pager_acquire_err;
                        }
                        if (
#if SQLITE_OMIT_MEMORYDB
																																																																																									1==MEMDB
#else
this.memDb != 0
#endif
 || this.dbSize < pgno || noContent != 0 || !this.fd.isOpen  )
                        {
                            if (pgno > this.mxPgno)
                            {
                                rc = SqlResult.SQLITE_FULL;
                                goto pager_acquire_err;
                            }
                            if (noContent != 0)
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="Failure to set the bits in the InJournal bit">vectors is benign.</param>
                                ///<param name="It merely means that we might do some extra work to journal a">It merely means that we might do some extra work to journal a</param>
                                ///<param name="page that does not need to be journaled.  Nevertheless, be sure">page that does not need to be journaled.  Nevertheless, be sure</param>
                                ///<param name="to test the case where a malloc error occurs while trying to set">to test the case where a malloc error occurs while trying to set</param>
                                ///<param name="a bit in a bit vector.">a bit in a bit vector.</param>
                                ///<param name=""></param>

                                sqlite3BeginBenignMalloc();
                                if (pgno <= this.dbOrigSize)
                                {
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																																																		              rc = sqlite3BitvecSet( pPager.pInJournal, pgno );          //TESTONLY( rc = ) sqlite3BitvecSet(pPager.pInJournal, pgno);
#else
                                    sqlite3BitvecSet(this.pInJournal, pgno);
#endif
                                    sqliteinth.testcase(rc == SqlResult.SQLITE_NOMEM);
                                }
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																															            rc = addToSavepointBitvecs( pPager, pgno ); //TESTONLY( rc = ) addToSavepointBitvecs(pPager, pgno);
#else
                                this.addToSavepointBitvecs(pgno);
#endif
                                sqliteinth.testcase(rc == SqlResult.SQLITE_NOMEM);
                                sqlite3EndBenignMalloc();
                            }
                            //memset(pPg.pData, 0, pPager.pageSize);
                            Array.Clear(pPg.pData, 0, this.pageSize);
                            sqliteinth.IOTRACE("ZERO %p %d\n", this, pgno);
                        }
                        else
                        {
                            Debug.Assert(pPg.pPager == this);
                            rc = PagerMethods.readDbPage(pPg);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                goto pager_acquire_err;
                            }
                        }
                        pPg.pager_set_pagehash();
                    }
                    return SqlResult.SQLITE_OK;
                pager_acquire_err:
                    Debug.Assert(rc != SqlResult.SQLITE_OK);
                    if (pPg != null)
                    {
                        PCacheMethods.sqlite3PcacheDrop(pPg);
                    }
                    this.pagerUnlockIfUnused();
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
                DbPage sqlite3PagerLookup(u32 pgno)
                {
                    PgHdr pPg = null;
                    Debug.Assert(this != null);
                    Debug.Assert(pgno != 0);
                    Debug.Assert(this.pPCache != null);
                    Debug.Assert(this.eState >= PagerState.PAGER_READER && this.eState != PagerState.PAGER_ERROR);
                    PCacheMethods.sqlite3PcacheFetch(this.pPCache, pgno, 0, ref pPg);
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
                    /// Return SqlResult.SQLITE_OK if everything is successful. Otherwise, return
                    /// SQLITE_NOMEM if the attempt to allocate Pager.pInJournal fails, or
                    /// an IO error code if opening or writing the journal file fails.
                    ///
                    ///</summary>
                SqlResult pager_open_journal()
                {
                    var rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    sqlite3_vfs pVfs = this.pVfs;
                    ///
                    ///<summary>
                    ///Local cache of vfs pointer 
                    ///</summary>

                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_LOCKED);
                    Debug.Assert(this.assert_pager_state());
                    Debug.Assert(this.pInJournal == null);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If already in the error state, this function is a no">op.  But on</param>
                    ///<param name="the other hand, this routine is never called if we are already in">the other hand, this routine is never called if we are already in</param>
                    ///<param name="an error state. ">an error state. </param>

                    if (NEVER((int)this.errCode) != 0)
                        return this.errCode;
                    if (!this.pagerUseWal() && this.journalMode != PAGER_JOURNALMODE_OFF)
                    {
                        this.pInJournal = Bitvec.sqlite3BitvecCreate(this.dbSize);
                        //if (pPager.pInJournal == null)
                        //{
                        //  return SQLITE_NOMEM;
                        //}
                        ///
                        ///<summary>
                        ///Open the journal file if it is not already open. 
                        ///</summary>

                        if (!this.jfd.isOpen)
                        {
                            if (this.journalMode == PAGER_JOURNALMODE_MEMORY)
                            {
                                memjrnl.sqlite3MemJournalOpen(this.jfd);
                            }
                            else
                            {
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
                                rc = os.sqlite3OsOpen(pVfs, this.zJournal, this.jfd, flags, ref int0);
#endif
                            }
                            Debug.Assert(rc != SqlResult.SQLITE_OK || this.jfd.isOpen);
                        }
                        ///
                        ///<summary>
                        ///Write the first journal header to the journal file and open
                        ///</summary>
                        ///<param name="the sub">journal if necessary.</param>
                        ///<param name=""></param>

                        if (rc == SqlResult.SQLITE_OK)
                        {
                            ///
                            ///<summary>
                            ///TODO: Check if all of these are really required. 
                            ///</summary>

                            this.nRec = 0;
                            this.journalOff = 0;
                            this.setMaster = 0;
                            this.journalHdr = 0;
                            rc = this.writeJournalHdr();
                        }
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        sqlite3BitvecDestroy(ref this.pInJournal);
                        this.pInJournal = null;
                    }
                    else
                    {
                        Debug.Assert(this.eState == PagerState.PAGER_WRITER_LOCKED);
                        this.eState = PagerState.PAGER_WRITER_CACHEMOD;
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
                SqlResult sqlite3PagerBegin(bool exFlag, int subjInMemory)
                {
                    var rc = SqlResult.SQLITE_OK;
                    if (this.errCode != 0)
                        return this.errCode;
                    Debug.Assert(this.eState >= PagerState.PAGER_READER && this.eState < PagerState.PAGER_ERROR);
                    this.subjInMemory = (u8)subjInMemory;
                    if (Sqlite3.ALWAYS(this.eState == PagerState.PAGER_READER))
                    {
                        Debug.Assert(this.pInJournal == null);
                        if (this.pagerUseWal())
                        {
                            ///
                            ///<summary>
                            ///If the pager is configured to use locking_mode=exclusive, and an
                            ///exclusive lock on the database is not already held, obtain it now.
                            ///
                            ///</summary>

                            if (this.exclusiveMode && wal.sqlite3WalExclusiveMode(this.pWal, -1))
                            {
                                rc = this.pagerLockDb(EXCLUSIVE_LOCK);
                                if (rc != SqlResult.SQLITE_OK)
                                {
                                    return rc;
                                }
                                wal.sqlite3WalExclusiveMode(this.pWal, 1);
                            }
                            ///
                            ///<summary>
                            ///Grab the write lock on the log file. If successful, upgrade to
                            ///PAGER_RESERVED state. Otherwise, return an error code to the caller.
                            ///</summary>
                            ///<param name="The busy">handler is not invoked if another connection already</param>
                            ///<param name="holds the write">lock. If possible, the upper layer will call it.</param>
                            ///<param name=""></param>

                            rc = wal.sqlite3WalBeginWriteTransaction(this.pWal);
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///Obtain a RESERVED lock on the database file. If the exFlag parameter
                            ///is true, then immediately upgrade this to an EXCLUSIVE lock. The
                            ///</summary>
                            ///<param name="busy">handler callback can be used when upgrading to the EXCLUSIVE</param>
                            ///<param name="lock, but not when obtaining the RESERVED lock.">lock, but not when obtaining the RESERVED lock.</param>
                            ///<param name=""></param>

                            rc = this.pagerLockDb(RESERVED_LOCK);
                            if (rc == SqlResult.SQLITE_OK && exFlag)
                            {
                                rc = this.pager_wait_on_lock(EXCLUSIVE_LOCK);
                            }
                        }
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            ///
                            ///<summary>
                            ///Change to WRITER_LOCKED state.
                            ///
                            ///WAL mode sets Pager.eState to PagerState.PAGER_WRITER_LOCKED or CACHEMOD
                            ///when it has an open transaction, but never to DBMOD or FINISHED.
                            ///This is because in those states the code to roll back savepoint 
                            ///</summary>
                            ///<param name="transactions may copy data from the sub">journal into the database </param>
                            ///<param name="file as well as into the page cache. Which would be incorrect in ">file as well as into the page cache. Which would be incorrect in </param>
                            ///<param name="WAL mode.">WAL mode.</param>
                            ///<param name=""></param>

                            this.eState = PagerState.PAGER_WRITER_LOCKED;
                            this.dbHintSize = this.dbSize;
                            this.dbFileSize = this.dbSize;
                            this.dbOrigSize = this.dbSize;
                            this.journalOff = 0;
                        }
                        Debug.Assert(rc == SqlResult.SQLITE_OK || this.eState == PagerState.PAGER_READER);
                        Debug.Assert(rc != SqlResult.SQLITE_OK || this.eState == PagerState.PAGER_WRITER_LOCKED);
                        Debug.Assert(this.assert_pager_state());
                    }
                    PAGERTRACE("TRANSACTION %d\n", PagerMethods.PAGERID(this));
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
                    /// PagerMethods.sqlite3PagerWrite() on page 1, then modifying the contents of the
                    /// page data. In this case the file will be updated when the current
                    /// transaction is committed.
                    ///
                    /// The isDirectMode flag may only be non-zero if the library was compiled
                    /// with the SQLITE_ENABLE_ATOMIC_WRITE macro defined. In this case,
                    /// if isDirect is non-zero, then the database file is updated directly
                    /// by writing an updated version of page 1 using a call to the
                    /// os.sqlite3OsWrite() function.
                    ///
                    ///</summary>
                SqlResult pager_incr_changecounter(bool isDirectMode)
                {
                    var rc = SqlResult.SQLITE_OK;
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD);
                    Debug.Assert(this.assert_pager_state());
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
                    Debug.Assert(isDirectMode == false);
                    Sqlite3.sqliteinth.UNUSED_PARAMETER(isDirectMode);
#else
																																																																						// define DIRECT_MODE isDirectMode
int DIRECT_MODE = isDirectMode;
#endif
                    if (!this.changeCountDone && this.dbSize > 0)
                    {
                        PgHdr pPgHdr = null;
                        ///
                        ///<summary>
                        ///Reference to page 1 
                        ///</summary>

                        Debug.Assert(!this.tempFile && this.fd.isOpen  );
                        ///
                        ///<summary>
                        ///Open page 1 of the file for writing. 
                        ///</summary>

                        rc = this.sqlite3PagerGet(1, ref pPgHdr);
                        Debug.Assert(pPgHdr == null || rc == SqlResult.SQLITE_OK);
                        ///
                        ///<summary>
                        ///If page one was fetched successfully, and this function is not
                        ///</summary>
                        ///<param name="operating in direct">mode, make page 1 writable.  When not in </param>
                        ///<param name="direct mode, page 1 is always held in cache and hence the PagerGet()">direct mode, page 1 is always held in cache and hence the PagerGet()</param>
                        ///<param name="above is always successful "> hence the ALWAYS on rc==SqlResult.SQLITE_OK.</param>
                        ///<param name=""></param>

                        if (!DIRECT_MODE && Sqlite3.ALWAYS(rc == SqlResult.SQLITE_OK))
                        {
                            rc = PagerMethods.sqlite3PagerWrite(pPgHdr);
                        }
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            ///
                            ///<summary>
                            ///Actually do the update of the change counter 
                            ///</summary>

                            pPgHdr.pager_write_changecounter();
                            ///
                            ///<summary>
                            ///If running in direct mode, write the contents of page 1 to the file. 
                            ///</summary>

                            if (DIRECT_MODE)
                            {
                                u8[] zBuf = null;
                                Debug.Assert(this.dbFileSize > 0);
                                if (PagerMethods.CODEC2(this, pPgHdr.pData, 1, crypto.SQLITE_ENCRYPT_WRITE_CTX, ref zBuf))
                                    return rc = SqlResult.SQLITE_NOMEM;
                                //PagerMethods.CODEC2(pPager, pPgHdr.pData, 1, 6, rc=SQLITE_NOMEM, zBuf);
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    rc = os.sqlite3OsWrite(this.fd, zBuf, this.pageSize, 0);
                                }
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    this.changeCountDone = true;
                                }
                            }
                            else
                            {
                                this.changeCountDone = true;
                            }
                        }
                        ///
                        ///<summary>
                        ///Release the page reference. 
                        ///</summary>

                        PagerMethods.sqlite3PagerUnref(pPgHdr);
                    }
                    return rc;
                }

                public///<summary>
                      /// Sync the database file to disk. This is a no-op for in-memory databases
                      /// or pages with the Pager.noSync flag set.
                      ///
                      /// If successful, or if called on a pager for which it is a no-op, this
                      /// function returns SqlResult.SQLITE_OK. Otherwise, an IO error code is returned.
                      ///
                      ///</summary>
                SqlResult sqlite3PagerSync()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    if (!this.noSync)
                    {
                        Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																									0 == MEMDB
#else
0 == this.memDb
#endif
);
                        rc = os.sqlite3OsSync(this.fd, this.syncFlags);
                    }
                    else
                        if (this.fd.isOpen  )
                        {
                            Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																																												0 == MEMDB
#else
0 == this.memDb
#endif
);
                            long sz=0;
                            os.sqlite3OsFileControl(this.fd, SQLITE_FCNTL_SYNC_OMITTED, ref sz);
                        }
                    return rc;
                }

                public///<summary>
                      /// This function may only be called while a write-transaction is active in
                      /// rollback. If the connection is in WAL mode, this call is a no-op.
                      /// Otherwise, if the connection does not already have an EXCLUSIVE lock on
                      /// the database file, an attempt is made to obtain one.
                      ///
                      /// If the EXCLUSIVE lock is already held or the attempt to obtain it is
                      /// successful, or the connection is in WAL mode, SqlResult.SQLITE_OK is returned.
                      /// Otherwise, either SQLITE_BUSY or an SQLITE_IOERR_XXX error code is
                      /// returned.
                      ///
                      ///</summary>
                SqlResult sqlite3PagerExclusiveLock()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_WRITER_LOCKED);
                    Debug.Assert(this.assert_pager_state());
                    if (false == this.pagerUseWal())
                    {
                        rc = this.pager_wait_on_lock(EXCLUSIVE_LOCK);
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
                SqlResult sqlite3PagerCommitPhaseOne(///
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
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_LOCKED || this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD || this.eState == PagerState.PAGER_ERROR);
                    Debug.Assert(this.assert_pager_state());
                    ///
                    ///<summary>
                    ///If a prior error occurred, report that error again. 
                    ///</summary>

                    if (NEVER(this.errCode != 0))
                        return this.errCode;
                    PAGERTRACE("DATABASE SYNC: File=%s zMaster=%s nSize=%d\n", this.zFilename, zMaster, this.dbSize);
                    ///
                    ///<summary>
                    ///If no database changes have been made, return early. 
                    ///</summary>

                    if (this.eState < PagerState.PAGER_WRITER_CACHEMOD)
                        return SqlResult.SQLITE_OK;
                    if (
#if SQLITE_OMIT_MEMORYDB
																																																																						0 != MEMDB
#else
0 != this.memDb
#endif
)
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If this is an in">memory db, or no pages have been written to, or this</param>
                        ///<param name="function has already been called, it is mostly a no">op.  However, any</param>
                        ///<param name="backup in progress needs to be restarted.">backup in progress needs to be restarted.</param>
                        ///<param name=""></param>

                        this.pBackup.sqlite3BackupRestart();
                    }
                    else
                    {
                        if (this.pagerUseWal())
                        {
                            PgHdr pList = PCacheMethods.sqlite3PcacheDirtyList(this.pPCache);
                            PgHdr pPageOne = null;
                            if (pList == null)
                            {
                                ///
                                ///<summary>
                                ///Must have at least one page for the WAL commit flag.
                                ///</summary>
                                ///<param name="Ticket [2d1a5c67dfc2363e44f29d9bbd57f] 2null11">18 </param>

                                rc = this.sqlite3PagerGet(1, ref pPageOne);
                                pList = pPageOne;
                                pList.pDirty = null;
                            }
                            Debug.Assert(rc == SqlResult.SQLITE_OK);
                            if (Sqlite3.ALWAYS(pList))
                            {
                                rc = this.pagerWalFrames(pList, this.dbSize, 1, (this.fullSync ? this.syncFlags : (byte)0));
                            }
                            PagerMethods.sqlite3PagerUnref(pPageOne);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                PCacheMethods.sqlite3PcacheCleanAll(this.pPCache);
                            }
                        }
                        else
                        {
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
if( rc==SqlResult.SQLITE_OK ){
rc = pager_incr_changecounter(pPager, 0);
}
}
#else
                            rc = this.pager_incr_changecounter(false);
#endif
                            if (rc != SqlResult.SQLITE_OK)
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
                            if (this.dbSize < this.dbOrigSize && this.journalMode != PAGER_JOURNALMODE_OFF)
                            {
                                Pgno i;
                                ///
                                ///<summary>
                                ///Iterator variable 
                                ///</summary>

                                Pgno iSkip = PAGER_MJ_PGNO(this);
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
                                for (i = dbSize + 1; i <= this.dbOrigSize; i++)
                                {
                                    if (0 == sqlite3BitvecTest(this.pInJournal, i) && i != iSkip)
                                    {
                                        PgHdr pPage = null;
                                        ///
                                        ///<summary>
                                        ///Page to journal 
                                        ///</summary>

                                        rc = this.sqlite3PagerGet(i, ref pPage);
                                        if (rc != SqlResult.SQLITE_OK)
                                            goto commit_phase_one_exit;
                                        rc = PagerMethods.sqlite3PagerWrite(pPage);
                                        PagerMethods.sqlite3PagerUnref(pPage);
                                        if (rc != SqlResult.SQLITE_OK)
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

                            rc = this.writeMasterJournal(zMaster);
                            if (rc != SqlResult.SQLITE_OK)
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

                            rc = this.syncJournal(0);
                            if (rc != SqlResult.SQLITE_OK)
                                goto commit_phase_one_exit;
                            rc = this.pager_write_pagelist(PCacheMethods.sqlite3PcacheDirtyList(this.pPCache));
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                Debug.Assert(rc != SqlResult.SQLITE_IOERR_BLOCKED);
                                goto commit_phase_one_exit;
                            }
                            PCacheMethods.sqlite3PcacheCleanAll(this.pPCache);
                            ///
                            ///<summary>
                            ///If the file on disk is not the same size as the database image,
                            ///then use pager_truncate to grow or shrink the file here.
                            ///
                            ///</summary>

                            if (this.dbSize != this.dbFileSize)
                            {
                                Pgno nNew = (Pgno)(this.dbSize - (this.dbSize == PAGER_MJ_PGNO(this) ? 1 : 0));
                                Debug.Assert(this.eState >= PagerState.PAGER_WRITER_DBMOD);
                                rc = this.pager_truncate(nNew);
                                if (rc != SqlResult.SQLITE_OK)
                                    goto commit_phase_one_exit;
                            }
                            ///
                            ///<summary>
                            ///Finally, sync the database file. 
                            ///</summary>

                            if (!noSync)
                            {
                                rc = this.sqlite3PagerSync();
                            }
                            sqliteinth.IOTRACE("DBSYNC %p\n", this);
                        }
                    }
                commit_phase_one_exit:
                    if (rc == SqlResult.SQLITE_OK && !this.pagerUseWal())
                    {
                        this.eState = PagerState.PAGER_WRITER_FINISHED;
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
                    /// moves into the error state. Otherwise, SqlResult.SQLITE_OK is returned.
                    ///
                    ///</summary>
                SqlResult sqlite3PagerCommitPhaseTwo()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
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

                    if (NEVER((int)this.errCode) != 0)
                        return this.errCode;
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_LOCKED || this.eState == PagerState.PAGER_WRITER_FINISHED || (this.pagerUseWal() && this.eState == PagerState.PAGER_WRITER_CACHEMOD));
                    Debug.Assert(this.assert_pager_state());
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

                    if (this.eState == PagerState.PAGER_WRITER_LOCKED && this.exclusiveMode && this.journalMode == PAGER_JOURNALMODE_PERSIST)
                    {
                        Debug.Assert(this.journalOff == this.JOURNAL_HDR_SZ() || 0 == this.journalOff);
                        this.eState = PagerState.PAGER_READER;
                        return SqlResult.SQLITE_OK;
                    }
                    PAGERTRACE("COMMIT %d\n", PagerMethods.PAGERID(this));
                    rc = this.pager_end_transaction(this.setMaster);
                    return this.pager_error(rc);
                }

                public///<summary>
                    /// If a write transaction is open, then all changes made within the
                    /// transaction are reverted and the current write-transaction is closed.
                    /// The pager falls back to PagerState.PAGER_READER state if successful, or PagerState.PAGER_ERROR
                    /// state if an error occurs.
                    ///
                    /// If the pager is already in PagerState.PAGER_ERROR state when this function is called,
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
                SqlResult sqlite3PagerRollback()
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    PAGERTRACE("ROLLBACK %d\n", PagerMethods.PAGERID(this));
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="PagerRollback() is a no">op if called in READER or OPEN state. If</param>
                    ///<param name="the pager is already in the ERROR state, the rollback is not ">the pager is already in the ERROR state, the rollback is not </param>
                    ///<param name="attempted here. Instead, the error code is returned to the caller.">attempted here. Instead, the error code is returned to the caller.</param>
                    ///<param name=""></param>

                    Debug.Assert(this.assert_pager_state());
                    if (this.eState == PagerState.PAGER_ERROR)
                        return this.errCode;
                    if (this.eState <= PagerState.PAGER_READER)
                        return SqlResult.SQLITE_OK;
                    if (this.pagerUseWal())
                    {
                        SqlResult rc2;
                        rc = this.sqlite3PagerSavepoint(sqliteinth.SAVEPOINT_ROLLBACK, -1);
                        rc2 = this.pager_end_transaction(this.setMaster);
                        if (rc == SqlResult.SQLITE_OK)
                            rc = rc2;
                        rc = this.pager_error(rc);
                    }
                    else
                        if (!this.jfd.isOpen || this.eState == PagerState.PAGER_WRITER_LOCKED)
                        {
                            var eState = this.eState;
                            rc = this.pager_end_transaction(0);
                            if (
#if SQLITE_OMIT_MEMORYDB
																																																																																																												0==MEMDB
#else
0 == this.memDb
#endif
 && eState > PagerState.PAGER_WRITER_LOCKED)
                            {
                                ///
                                ///<summary>
                                ///This can happen using journal_mode=off. Move the pager to the error 
                                ///state to indicate that the contents of the cache may not be trusted.
                                ///Any active readers will get SQLITE_ABORT.
                                ///
                                ///</summary>

                                this.errCode = SqlResult.SQLITE_ABORT;
                                this.eState = PagerState.PAGER_ERROR;
                                return rc;
                            }
                        }
                        else
                        {
                            rc = this.pager_playback(0);
                        }
                    Debug.Assert(this.eState == PagerState.PAGER_READER || rc != SqlResult.SQLITE_OK);
                    Debug.Assert(rc == SqlResult.SQLITE_OK || rc == SqlResult.SQLITE_FULL || (rc & (SqlResult)0xFF) == SqlResult.SQLITE_IOERR);
                    ///
                    ///<summary>
                    ///If an error occurs during a ROLLBACK, we can no longer trust the pager
                    ///cache. So call pager_error() on the way out to make any error persistent.
                    ///
                    ///</summary>

                    return this.pager_error(rc);
                }

                public///<summary>
                    /// Return TRUE if the database file is opened read-only.  Return FALSE
                    /// if the database is (in theory) writable.
                    ///
                    ///</summary>
                bool sqlite3PagerIsreadonly()
                {
                    return this.readOnly;
                }

                public///<summary>
                    /// Return the number of references to the pager.
                    ///
                    ///</summary>
                int sqlite3PagerRefcount()
                {
                    return PCacheMethods.sqlite3PcacheRefCount(this.pPCache);
                }

                public///<summary>
                    /// Return the approximate number of bytes of memory currently
                    /// used by the pager and its associated cache.
                    ///
                    ///</summary>
                int sqlite3PagerMemUsed()
                {
                    int perPageSize = this.pageSize + this.nExtra + 20;
                    //+ sizeof(PgHdr) + 5*sizeof(void*);
                    return perPageSize * PCacheMethods.sqlite3PcachePagecount(this.pPCache) + 0 // Not readily available under C#// malloc_cs.sqlite3MallocSize(pPager);
                    + this.pageSize;
                }

                public///<summary>
                    /// Return true if this is an in-memory pager.
                    ///</summary>
                bool sqlite3PagerIsMemdb()
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
                    /// returned. Otherwise, SqlResult.SQLITE_OK.
                    ///
                    ///</summary>
                SqlResult sqlite3PagerOpenSavepoint(int nSavepoint)
                {
                    SqlResult rc = SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    int nCurrent = this.nSavepoint;
                    ///
                    ///<summary>
                    ///Current number of savepoints 
                    ///</summary>

                    Debug.Assert(this.eState >= PagerState.PAGER_WRITER_LOCKED);
                    Debug.Assert(this.assert_pager_state());
                    if (nSavepoint > nCurrent && this.useJournal != 0)
                    {
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
                        Array.Resize(ref this.aSavepoint, nSavepoint);
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

                        for (ii = nCurrent; ii < nSavepoint; ii++)
                        {
                            aNew[ii] = new PagerSavepoint();
                            aNew[ii].nOrig = this.dbSize;
                            if (this.jfd.isOpen && this.journalOff > 0)
                            {
                                aNew[ii].iOffset = this.journalOff;
                            }
                            else
                            {
                                aNew[ii].iOffset = (int)this.JOURNAL_HDR_SZ();
                            }
                            aNew[ii].iSubRec = this.nSubRec;
                            aNew[ii].pInSavepoint = Bitvec.sqlite3BitvecCreate(this.dbSize);
                            //if ( null == aNew[ii].pInSavepoint )
                            //{
                            //  return SQLITE_NOMEM;
                            //}
                            if (this.pagerUseWal())
                            {
                                wal.sqlite3WalSavepoint(this.pWal, aNew[ii].aWalData);
                            }
                            this.nSavepoint = ii + 1;
                        }
                        Debug.Assert(this.nSavepoint == nSavepoint);
                        this.assertTruncateConstraint();
                    }
                    return rc;
                }

                public///<summary>
                      /// This function is called to rollback or release (commit) a savepoint.
                      /// The savepoint to release or rollback need not be the most recently
                      /// created savepoint.
                      ///
                      /// Parameter op is always either sqliteinth.SAVEPOINT_ROLLBACK or SAVEPOINT_RELEASE.
                      /// If it is SAVEPOINT_RELEASE, then release and destroy the savepoint with
                      /// index iSavepoint. If it is sqliteinth.SAVEPOINT_ROLLBACK, then rollback all changes
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
                      /// savepoint. If no errors occur, SqlResult.SQLITE_OK is returned.
                      ///
                      ///</summary>
                SqlResult sqlite3PagerSavepoint(int op, int iSavepoint)
                {
                    SqlResult rc = this.errCode;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    Debug.Assert(op == sqliteinth.SAVEPOINT_RELEASE || op == sqliteinth.SAVEPOINT_ROLLBACK);
                    Debug.Assert(iSavepoint >= 0 || op == sqliteinth.SAVEPOINT_ROLLBACK);
                    if (rc == SqlResult.SQLITE_OK && iSavepoint < this.nSavepoint)
                    {
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

                        nNew = iSavepoint + ((op == sqliteinth.SAVEPOINT_RELEASE) ? 0 : 1);
                        for (ii = nNew; ii < this.nSavepoint; ii++)
                        {
                            sqlite3BitvecDestroy(ref this.aSavepoint[ii].pInSavepoint);
                        }
                        this.nSavepoint = nNew;
                        ///
                        ///<summary>
                        ///If this is a release of the outermost savepoint, truncate 
                        ///</summary>
                        ///<param name="the sub">journal to zero bytes in size. </param>

                        if (op == sqliteinth.SAVEPOINT_RELEASE)
                        {
                            if (nNew == 0 && this.sjfd.isOpen)
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="Only truncate if it is an in">journal. </param>

                                if (memjrnl.sqlite3IsMemJournal(this.sjfd))
                                {
                                    rc = os.sqlite3OsTruncate(this.sjfd, 0);
                                    Debug.Assert(rc == SqlResult.SQLITE_OK);
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
                            if (this.pagerUseWal() || this.jfd.isOpen)
                            {
                                PagerSavepoint pSavepoint = (nNew == 0) ? null : this.aSavepoint[nNew - 1];
                                rc = this.pagerPlaybackSavepoint(pSavepoint);
                                Debug.Assert(rc != SqlResult.SQLITE_DONE);
                            }
                    }
                    return rc;
                }

                public///<summary>
                    /// Return the VFS structure for the pager.
                    ///
                    ///</summary>
                sqlite3_vfs sqlite3PagerVfs()
                {
                    return this.pVfs;
                }

                public///<summary>
                    /// Return the full pathname of the database file.
                    ///
                    ///</summary>
                string Filename()
                {
                    return this.zFilename;
                }

                public///<summary>
                    /// Return the file handle for the database file associated
                    /// with the pager.  This might return NULL if the file has
                    /// not yet been opened.
                    ///
                    ///</summary>
                sqlite3_file sqlite3PagerFile()
                {
                    return this.fd;
                }

                public///<summary>
                    /// Return true if fsync() calls are disabled for this pager.  Return FALSE
                    /// if fsync()s are executed normally.
                    ///
                    ///</summary>
                bool sqlite3PagerNosync()
                {
                    return this.noSync;
                }

                public///<summary>
                    /// Return the full pathname of the journal file.
                    ///
                    ///</summary>
                string sqlite3PagerJournalname()
                {
                    return this.zJournal;
                }

                public void sqlite3PagerSetCodec(dxCodec xCodec, //void *(*xCodec)(void*,void*,Pgno,int),
                dxCodecSizeChng xCodecSizeChng, //void (*xCodecSizeChng)(void*,int,int),
                dxCodecFree xCodecFree, //void (*xCodecFree)(void*),
                codec_ctx pCodec)
                {
                    if (this.xCodecFree != null)
                        this.xCodecFree(ref this.pCodec);
                    this.xCodec = (this.memDb != 0) ? null : xCodec;
                    this.xCodecSizeChng = xCodecSizeChng;
                    this.xCodecFree = xCodecFree;
                    this.pCodec = pCodec;
                    this.pagerReportSize();
                }

                public object sqlite3PagerGetCodec()
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
                    /// occurs. Otherwise, it returns SqlResult.SQLITE_OK.
                    ///</summary>
                SqlResult sqlite3PagerMovepage(DbPage pPg, u32 pgno, int isCommit)
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

                    SqlResult rc;
                    ///
                    ///<summary>
                    ///Return code 
                    ///</summary>

                    Pgno origPgno;
                    ///
                    ///<summary>
                    ///The original page number 
                    ///</summary>

                    Debug.Assert(pPg.nRef > 0);
                    Debug.Assert(this.eState == PagerState.PAGER_WRITER_CACHEMOD || this.eState == PagerState.PAGER_WRITER_DBMOD);
                    Debug.Assert(this.assert_pager_state());
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
)
                    {
                        rc = PagerMethods.sqlite3PagerWrite(pPg);
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

                    if ((pPg.flags & PGHDR_DIRTY) != 0 && pPg.subjRequiresPage() && SqlResult.SQLITE_OK != (rc = PagerMethods.subjournalPage(pPg)))
                    {
                        return rc;
                    }
                    PAGERTRACE("MOVE %d page %d (needSync=%d) moves to %d\n", PagerMethods.PAGERID(this), pPg.pgno, (pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0, pgno);
                    sqliteinth.IOTRACE("MOVE %p %d %d\n", this, pPg.pgno, pgno);
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

                    if (((pPg.flags & PGHDR_NEED_SYNC) != 0) && 0 == isCommit)
                    {
                        needSyncPgno = pPg.pgno;
                        Debug.Assert(pPg.pageInJournal() || pPg.pgno > this.dbOrigSize);
                        Debug.Assert((pPg.flags & PGHDR_DIRTY) != 0);
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
                    pPgOld = this.pager_lookup(pgno);
                    Debug.Assert(null == pPgOld || pPgOld.nRef == 1);
                    if (pPgOld != null)
                    {
                        pPg.flags |= (pPgOld.flags & PGHDR_NEED_SYNC);
                        if (
#if SQLITE_OMIT_MEMORYDB
																																																																																									1==MEMDB
#else
this.memDb != 0
#endif
)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="Do not discard pages from an in">memory database since we might</param>
                            ///<param name="need to rollback later.  Just move the page out of the way. ">need to rollback later.  Just move the page out of the way. </param>

                            PCacheMethods.sqlite3PcacheMove(pPgOld, this.dbSize + 1);
                        }
                        else
                        {
                            PCacheMethods.sqlite3PcacheDrop(pPgOld);
                        }
                    }
                    origPgno = pPg.pgno;
                    PCacheMethods.sqlite3PcacheMove(pPg, pgno);
                    PCacheMethods.sqlite3PcacheMakeDirty(pPg);
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
)
                    {
                        Debug.Assert(pPgOld);
                        PCacheMethods.sqlite3PcacheMove(pPgOld, origPgno);
                        PagerMethods.sqlite3PagerUnref(pPgOld);
                    }
                    if (needSyncPgno != 0)
                    {
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
                        rc = this.sqlite3PagerGet(needSyncPgno, ref pPgHdr);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            if (needSyncPgno <= this.dbOrigSize)
                            {
                                Debug.Assert(this.pTmpSpace != null);
                                u32[] pTemp = new u32[this.pTmpSpace.Length];
                                sqlite3BitvecClear(this.pInJournal, needSyncPgno, pTemp);
                                //pPager.pTmpSpace );
                            }
                            return rc;
                        }
                        pPgHdr.flags |= PGHDR_NEED_SYNC;
                        PCacheMethods.sqlite3PcacheMakeDirty(pPgHdr);
                        PagerMethods.sqlite3PagerUnref(pPgHdr);
                    }
                    return SqlResult.SQLITE_OK;
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
                bool sqlite3PagerLockingMode(int eMode)
                {
                    Debug.Assert(eMode == PAGER_LOCKINGMODE_QUERY || eMode == PAGER_LOCKINGMODE_NORMAL || eMode == PAGER_LOCKINGMODE_EXCLUSIVE);
                    Debug.Assert(PAGER_LOCKINGMODE_QUERY < 0);
                    Debug.Assert(PAGER_LOCKINGMODE_NORMAL >= 0 && PAGER_LOCKINGMODE_EXCLUSIVE >= 0);
                    Debug.Assert(this.exclusiveMode || false == wal.sqlite3WalHeapMemory(this.pWal));
                    if (eMode >= 0 && !this.tempFile && !wal.sqlite3WalHeapMemory(this.pWal))
                    {
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
                int sqlite3PagerSetJournalMode(int eMode)
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

                    Debug.Assert(eMode == PAGER_JOURNALMODE_DELETE || eMode == PAGER_JOURNALMODE_TRUNCATE || eMode == PAGER_JOURNALMODE_PERSIST || eMode == PAGER_JOURNALMODE_OFF || eMode == PAGER_JOURNALMODE_WAL || eMode == PAGER_JOURNALMODE_MEMORY);
                    ///
                    ///<summary>
                    ///This routine is only called from the OP_JournalMode opcode, and
                    ///the logic there will never allow a temporary file to be changed
                    ///to WAL mode.
                    ///
                    ///</summary>

                    Debug.Assert(this.tempFile == false || eMode != PAGER_JOURNALMODE_WAL);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Do allow the journalmode of an in">memory database to be set to</param>
                    ///<param name="anything other than MEMORY or OFF">anything other than MEMORY or OFF</param>
                    ///<param name=""></param>

                    if (
//#if SQLITE_OMIT_MEMORYDB
1==MEMDB
#else
1 == this.memDb
#endif
)
                    {
                        Debug.Assert(eOld == PAGER_JOURNALMODE_MEMORY || eOld == PAGER_JOURNALMODE_OFF);
                        if (eMode != PAGER_JOURNALMODE_MEMORY && eMode != PAGER_JOURNALMODE_OFF)
                        {
                            eMode = eOld;
                        }
                    }
                    if (eMode != eOld)
                    {
                        ///
                        ///<summary>
                        ///Change the journal mode. 
                        ///</summary>

                        Debug.Assert(this.eState != PagerState.PAGER_ERROR);
                        this.journalMode = (u8)eMode;
                        ///
                        ///<summary>
                        ///When transistioning from TRUNCATE or PERSIST to any other journal
                        ///mode except WAL, unless the pager is in locking_mode=exclusive mode,
                        ///delete the journal file.
                        ///
                        ///</summary>

                        Debug.Assert((PAGER_JOURNALMODE_TRUNCATE & 5) == 1);
                        Debug.Assert((PAGER_JOURNALMODE_PERSIST & 5) == 1);
                        Debug.Assert((PAGER_JOURNALMODE_DELETE & 5) == 0);
                        Debug.Assert((PAGER_JOURNALMODE_MEMORY & 5) == 4);
                        Debug.Assert((PAGER_JOURNALMODE_OFF & 5) == 0);
                        Debug.Assert((PAGER_JOURNALMODE_WAL & 5) == 5);
                        Debug.Assert(this.fd.isOpen   || this.exclusiveMode);
                        if (!this.exclusiveMode && (eOld & 5) == 1 && (eMode & 1) == 0)
                        {
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

                            os.sqlite3OsClose(this.jfd);
                            if (this.eLock >= RESERVED_LOCK)
                            {
                                os.sqlite3OsDelete(this.pVfs, this.zJournal, 0);
                            }
                            else
                            {
                                var rc = SqlResult.SQLITE_OK;
                                var state = this.eState;
                                Debug.Assert(state == PagerState.PAGER_OPEN || state == PagerState.PAGER_READER);
                                if (state == PagerState.PAGER_OPEN)
                                {
                                    rc = this.sqlite3PagerSharedLock();
                                }
                                if (this.eState == PagerState.PAGER_READER)
                                {
                                    Debug.Assert(rc == SqlResult.SQLITE_OK);
                                    rc = this.pagerLockDb(RESERVED_LOCK);
                                }
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    os.sqlite3OsDelete(this.pVfs, this.zJournal, 0);
                                }
                                if (rc == SqlResult.SQLITE_OK && state == PagerState.PAGER_READER)
                                {
                                    this.pagerUnlockDb(SHARED_LOCK);
                                }
                                else
                                    if (state == PagerState.PAGER_OPEN)
                                    {
                                        this.pager_unlock();
                                    }
                                Debug.Assert(state == this.eState);
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
                int sqlite3PagerGetJournalMode()
                {
                    return (int)this.journalMode;
                }

                public///<summary>
                    /// Return TRUE if the pager is in a state where it is OK to change the
                    /// journalmode.  Journalmode changes can only happen when the database
                    /// is unmodified.
                    ///
                    ///</summary>
                int sqlite3PagerOkToChangeJournalMode()
                {
                    Debug.Assert(this.assert_pager_state());
                    if (this.eState >= PagerState.PAGER_WRITER_CACHEMOD)
                        return 0;
                    if (NEVER(this.jfd.isOpen && this.journalOff > 0))
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
                i64 sqlite3PagerJournalSizeLimit(i64 iLimit)
                {
                    if (iLimit >= -1)
                    {
                        this.journalSizeLimit = iLimit;
                        wal.sqlite3WalLimit(this.pWal, iLimit);
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
                sqlite3_backup sqlite3PagerBackupPtr()
                {
                    return this.pBackup;
                }

                public void sqlite3pager_get_codec(ref codec_ctx ctx)
                {
                    ctx = this.pCodec;
                }

                public int sqlite3pager_is_mj_pgno(Pgno pgno)
                {
                    return (PAGER_MJ_PGNO(this) == pgno) ? 1 : 0;
                }

                public sqlite3_file sqlite3Pager_get_fd()
                {
                    return (this.fd.isOpen  ) ? this.fd : null;
                }

                public void sqlite3pager_sqlite3PagerSetCodec(dxCodec xCodec, dxCodecSizeChng xCodecSizeChng, dxCodecFree xCodecFree, codec_ctx pCodec)
                {
                    this.sqlite3PagerSetCodec(xCodec, xCodecSizeChng, xCodecFree, pCodec);
                }
        }
#endif
    }
}
