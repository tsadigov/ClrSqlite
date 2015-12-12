using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Paging
{
    #region enum


    /*
    ** The Pager.eState variable stores the current 'state' of a pager. A
    ** pager may be in any one of the seven states shown in the following
    ** state diagram.
    **
    **                            OPEN <------+------+
    **                              |         |      |
    **                              V         |      |
    **               +---------> READER-------+      |
    **               |              |                |
    **               |              V                |
    **               |<-------WRITER_LOCKED------> ERROR
    **               |              |                ^  
    **               |              V                |
    **               |<------WRITER_CACHEMOD-------->|
    **               |              |                |
    **               |              V                |
    **               |<-------WRITER_DBMOD---------->|
    **               |              |                |
    **               |              V                |
    **               +<------WRITER_FINISHED-------->+
    **
    **
    ** List of state transitions and the C [function] that performs each:
    ** 
    **   OPEN              -> READER              [sqlite3PagerSharedLock]
    **   READER            -> OPEN                [pager_unlock]
    **
    **   READER            -> WRITER_LOCKED       [sqlite3PagerBegin]
    **   WRITER_LOCKED     -> WRITER_CACHEMOD     [pager_open_journal]
    **   WRITER_CACHEMOD   -> WRITER_DBMOD        [syncJournal]
    **   WRITER_DBMOD      -> WRITER_FINISHED     [sqlite3PagerCommitPhaseOne]
    **   WRITER_***        -> READER              [pager_end_transaction]
    **
    **   WRITER_***        -> ERROR               [pager_error]
    **   ERROR             -> OPEN                [pager_unlock]
    ** 
    **
    **  OPEN:
    **
    **    The pager starts up in this state. Nothing is guaranteed in this
    **    state - the file may or may not be locked and the database size is
    **    unknown. The database may not be read or written.
    **
    **    * No read or write transaction is active.
    **    * Any lock, or no lock at all, may be held on the database file.
    **    * The dbSize, dbOrigSize and dbFileSize variables may not be trusted.
    **
    **  READER:
    **
    **    In this state all the requirements for reading the database in 
    **    rollback (non-WAL) mode are met. Unless the pager is (or recently
    **    was) in exclusive-locking mode, a user-level read transaction is 
    **    open. The database size is known in this state.
    **
    **    A connection running with locking_mode=normal enters this state when
    **    it opens a read-transaction on the database and returns to state
    **    OPEN after the read-transaction is completed. However a connection
    **    running in locking_mode=exclusive (including temp databases) remains in
    **    this state even after the read-transaction is closed. The only way
    **    a locking_mode=exclusive connection can transition from READER to OPEN
    **    is via the ERROR state (see below).
    ** 
    **    * A read transaction may be active (but a write-transaction cannot).
    **    * A SHARED or greater lock is held on the database file.
    **    * The dbSize variable may be trusted (even if a user-level read 
    **      transaction is not active). The dbOrigSize and dbFileSize variables
    **      may not be trusted at this point.
    **    * If the database is a WAL database, then the WAL connection is open.
    **    * Even if a read-transaction is not open, it is guaranteed that 
    **      there is no hot-journal in the file-system.
    **
    **  WRITER_LOCKED:
    **
    **    The pager moves to this state from READER when a write-transaction
    **    is first opened on the database. In WRITER_LOCKED state, all locks 
    **    required to start a write-transaction are held, but no actual 
    **    modifications to the cache or database have taken place.
    **
    **    In rollback mode, a RESERVED or (if the transaction was opened with 
    **    BEGIN EXCLUSIVE) EXCLUSIVE lock is obtained on the database file when
    **    moving to this state, but the journal file is not written to or opened 
    **    to in this state. If the transaction is committed or rolled back while 
    **    in WRITER_LOCKED state, all that is required is to unlock the database 
    **    file.
    **
    **    IN WAL mode, WalBeginWriteTransaction() is called to lock the log file.
    **    If the connection is running with locking_mode=exclusive, an attempt
    **    is made to obtain an EXCLUSIVE lock on the database file.
    **
    **    * A write transaction is active.
    **    * If the connection is open in rollback-mode, a RESERVED or greater 
    **      lock is held on the database file.
    **    * If the connection is open in WAL-mode, a WAL write transaction
    **      is open (i.e. sqlite3WalBeginWriteTransaction() has been successfully
    **      called).
    **    * The dbSize, dbOrigSize and dbFileSize variables are all valid.
    **    * The contents of the pager cache have not been modified.
    **    * The journal file may or may not be open.
    **    * Nothing (not even the first header) has been written to the journal.
    **
    **  WRITER_CACHEMOD:
    **
    **    A pager moves from WRITER_LOCKED state to this state when a page is
    **    first modified by the upper layer. In rollback mode the journal file
    **    is opened (if it is not already open) and a header written to the
    **    start of it. The database file on disk has not been modified.
    **
    **    * A write transaction is active.
    **    * A RESERVED or greater lock is held on the database file.
    **    * The journal file is open and the first header has been written 
    **      to it, but the header has not been synced to disk.
    **    * The contents of the page cache have been modified.
    **
    **  WRITER_DBMOD:
    **
    **    The pager transitions from WRITER_CACHEMOD into WRITER_DBMOD state
    **    when it modifies the contents of the database file. WAL connections
    **    never enter this state (since they do not modify the database file,
    **    just the log file).
    **
    **    * A write transaction is active.
    **    * An EXCLUSIVE or greater lock is held on the database file.
    **    * The journal file is open and the first header has been written 
    **      and synced to disk.
    **    * The contents of the page cache have been modified (and possibly
    **      written to disk).
    **
    **  WRITER_FINISHED:
    **
    **    It is not possible for a WAL connection to enter this state.
    **
    **    A rollback-mode pager changes to WRITER_FINISHED state from WRITER_DBMOD
    **    state after the entire transaction has been successfully written into the
    **    database file. In this state the transaction may be committed simply
    **    by finalizing the journal file. Once in WRITER_FINISHED state, it is 
    **    not possible to modify the database further. At this point, the upper 
    **    layer must either commit or rollback the transaction.
    **
    **    * A write transaction is active.
    **    * An EXCLUSIVE or greater lock is held on the database file.
    **    * All writing and syncing of journal and database data has finished.
    **      If no error occured, all that remains is to finalize the journal to
    **      commit the transaction. If an error did occur, the caller will need
    **      to rollback the transaction. 
    **
    **  ERROR:
    **
    **    The ERROR state is entered when an IO or disk-full error (including
    **    SQLITE_IOERR_NOMEM) occurs at a point in the code that makes it 
    **    difficult to be sure that the in-memory pager state (cache contents, 
    **    db size etc.) are consistent with the contents of the file-system.
    **
    **    Temporary pager files may enter the ERROR state, but in-memory pagers
    **    cannot.
    **
    **    For example, if an IO error occurs while performing a rollback, 
    **    the contents of the page-cache may be left in an inconsistent state.
    **    At this point it would be dangerous to change back to READER state
    **    (as usually happens after a rollback). Any subsequent readers might
    **    report database corruption (due to the inconsistent cache), and if
    **    they upgrade to writers, they may inadvertently corrupt the database
    **    file. To avoid this hazard, the pager switches into the ERROR state
    **    instead of READER following such an error.
    **
    **    Once it has entered the ERROR state, any attempt to use the pager
    **    to read or write data returns an error. Eventually, once all 
    **    outstanding transactions have been abandoned, the pager is able to
    **    transition back to OPEN state, discarding the contents of the 
    **    page-cache and any other in-memory state at the same time. Everything
    **    is reloaded from disk (and, if necessary, hot-journal rollback peformed)
    **    when a read-transaction is next opened on the pager (transitioning
    **    the pager into READER state). At that point the system has recovered 
    **    from the error.
    **
    **    Specifically, the pager jumps into the ERROR state if:
    **
    **      1. An error occurs while attempting a rollback. This happens in
    **         function sqlite3PagerRollback().
    **
    **      2. An error occurs while attempting to finalize a journal file
    **         following a commit in function sqlite3PagerCommitPhaseTwo().
    **
    **      3. An error occurs while attempting to write to the journal or
    **         database file in function pagerStress() in order to free up
    **         memory.
    **
    **    In other cases, the error is returned to the b-tree layer. The b-tree
    **    layer then attempts a rollback operation. If the error condition 
    **    persists, the pager enters the ERROR state via condition (1) above.
    **
    **    Condition (3) is necessary because it can be triggered by a read-only
    **    statement executed within a transaction. In this case, if the error
    **    code were simply returned to the user, the b-tree layer would not
    **    automatically attempt a rollback, as it assumes that an error in a
    **    read-only statement cannot leave the pager in an internally inconsistent 
    **    state.
    **
    **    * The Pager.errCode variable is set to something other than SQLITE_OK.
    **    * There are one or more outstanding references to pages (after the
    **      last reference is dropped the pager should move back to OPEN state).
    **    * The pager is not an in-memory pager.
    **    
    **
    ** Notes:
    **
    **   * A pager is never in WRITER_DBMOD or WRITER_FINISHED state if the
    **     connection is open in WAL mode. A WAL connection is always in one
    **     of the first four states.
    **
    **   * Normally, a connection open in exclusive mode is never in PAGER_OPEN
    **     state. There are two exceptions: immediately after exclusive-mode has
    **     been turned on (and before any read or write transactions are 
    **     executed), and when the pager is leaving the "error state".
    **
    **   * See also: assert_pager_state().
    */

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

}
