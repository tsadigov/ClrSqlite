using System;
using System.Diagnostics;
using System.Text;
using i64=System.Int64;
using u8=System.Byte;
using u32=System.UInt32;
using Pgno=System.UInt32;
namespace Community.CsharpSqlite {
	using sqlite3_int64=System.Int64;
	using DbPage=Sqlite3.PgHdr;
	public partial class Sqlite3 {
		///
		///<summary>
		///2009 January 28
		///
		///The author disclaims copyright to this source code.  In place of
		///a legal notice, here is a blessing:
		///
		///May you do good and not evil.
		///May you find forgiveness for yourself and forgive others.
		///May you share freely, never taking more than you give.
		///
		///
		///This file contains the implementation of the sqlite3_backup_XXX()
		///API functions and the related features.
		///
		///</summary>
		///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
		///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
		///<param name=""></param>
		///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e</param>
		///<param name=""></param>
		///<param name=""></param>
		//#include "sqliteInt.h"
		//#include "btreeInt.h"
		///<summary>
		///Macro to find the minimum of two numeric values.
		///</summary>
		#if !MIN
		//# define MIN(x,y) ((x)<(y)?(x):(y))
		#endif
		///
		///<summary>
		///Structure allocated for each backup operation.
		///</summary>
		public class sqlite3_backup {
			public sqlite3 pDestDb;
			///
			///<summary>
			///Destination database handle 
			///</summary>
			public Btree pDest;
			///
			///<summary>
			///</summary>
			///<param name="Destination b">tree file </param>
			public u32 iDestSchema;
			///
			///<summary>
			///Original schema cookie in destination 
			///</summary>
			public int bDestLocked;
			///
			///<summary>
			///</summary>
			///<param name="True once a write">transaction is open on pDest </param>
			public Pgno iNext;
			///
			///<summary>
			///Page number of the next source page to copy 
			///</summary>
			public sqlite3 pSrcDb;
			///
			///<summary>
			///Source database handle 
			///</summary>
			public Btree pSrc;
			///
			///<summary>
			///</summary>
			///<param name="Source b">tree file </param>
			public int rc;
			///
			///<summary>
			///Backup process error code 
			///</summary>
			///
			///<summary>
			///These two variables are set by every call to backup_step(). They are
			///read by calls to backup_remaining() and backup_pagecount().
			///
			///</summary>
			public Pgno nRemaining;
			///
			///<summary>
			///Number of pages left to copy 
			///</summary>
			public Pgno nPagecount;
			///
			///<summary>
			///Total number of pages to copy 
			///</summary>
			public int isAttached;
			///
			///<summary>
			///True once backup has been registered with pager 
			///</summary>
			public sqlite3_backup pNext;
			///
			///<summary>
			///Next backup associated with source pager 
			///</summary>
			public///<summary>
			/// This function is called after the contents of page iPage of the
			/// source database have been modified. If page iPage has already been
			/// copied into the destination database, then the data written to the
			/// destination is now invalidated. The destination copy of iPage needs
			/// to be updated with the new data before the backup operation is
			/// complete.
			///
			/// It is assumed that the mutex associated with the BtShared object
			/// corresponding to the source database is held when this function is
			/// called.
			///</summary>
			void sqlite3BackupUpdate(Pgno iPage,byte[] aData) {
				sqlite3_backup p;
				///
				///<summary>
				///Iterator variable 
				///</summary>
				for(p=this;p!=null;p=p.pNext) {
					Debug.Assert(sqlite3_mutex_held(p.pSrc.pBt.mutex));
					if(!isFatalError(p.rc)&&iPage<p.iNext) {
						///
						///<summary>
						///The backup process p has already copied page iPage. But now it
						///has been modified by a transaction on the source pager. Copy
						///the new data into the backup.
						///
						///</summary>
						int rc;
						Debug.Assert(p.pDestDb!=null);
						sqlite3_mutex_enter(p.pDestDb.mutex);
						rc=p.backupOnePage(iPage,aData);
						sqlite3_mutex_leave(p.pDestDb.mutex);
						Debug.Assert(rc!=SQLITE_BUSY&&rc!=SQLITE_LOCKED);
						if(rc!=SQLITE_OK) {
							p.rc=rc;
						}
					}
				}
			}
			///<summary>
			/// Copy nPage pages from the source b-tree to the destination.
			///</summary>
			public int sqlite3_backup_step(int nPage) {
				int rc;
				int destMode;
				///
				///<summary>
				///Destination journal mode 
				///</summary>
				int pgszSrc=0;
				///
				///<summary>
				///Source page size 
				///</summary>
				int pgszDest=0;
				///
				///<summary>
				///Destination page size 
				///</summary>
				sqlite3_mutex_enter(this.pSrcDb.mutex);
				sqlite3BtreeEnter(this.pSrc);
				if(this.pDestDb!=null) {
					sqlite3_mutex_enter(this.pDestDb.mutex);
				}
				rc=this.rc;
				if(!isFatalError(rc)) {
					Pager pSrcPager=sqlite3BtreePager(this.pSrc);
					///
					///<summary>
					///Source pager 
					///</summary>
					Pager pDestPager=sqlite3BtreePager(this.pDest);
					///
					///<summary>
					///Dest pager 
					///</summary>
					int ii;
					///
					///<summary>
					///Iterator variable 
					///</summary>
					Pgno nSrcPage=0;
					///
					///<summary>
					///Size of source db in pages 
					///</summary>
					int bCloseTrans=0;
					///
					///<summary>
					///True if src db requires unlocking 
					///</summary>
					///
					///<summary>
					///</summary>
					///<param name="If the source pager is currently in a write">transaction, return</param>
					///<param name="SQLITE_BUSY immediately.">SQLITE_BUSY immediately.</param>
					///<param name=""></param>
					if(this.pDestDb!=null&&this.pSrc.pBt.inTransaction==TransType.TRANS_WRITE) {
						rc=SQLITE_BUSY;
					}
					else {
						rc=SQLITE_OK;
					}
					///
					///<summary>
					///Lock the destination database, if it is not locked already. 
					///</summary>
					if(SQLITE_OK==rc&&this.bDestLocked==0&&SQLITE_OK==(rc=sqlite3BtreeBeginTrans(this.pDest,2))) {
						this.bDestLocked=1;
						this.iDestSchema=this.pDest.sqlite3BtreeGetMeta(BTREE_SCHEMA_VERSION);
					}
					///
					///<summary>
					///</summary>
					///<param name="If there is no open read">transaction on the source database, open</param>
					///<param name="one now. If a transaction is opened here, then it will be closed">one now. If a transaction is opened here, then it will be closed</param>
					///<param name="before this function exits.">before this function exits.</param>
					///<param name=""></param>
					if(rc==SQLITE_OK&&!this.pSrc.sqlite3BtreeIsInReadTrans()) {
						rc=sqlite3BtreeBeginTrans(this.pSrc,0);
						bCloseTrans=1;
					}
					///
					///<summary>
					///Do not allow backup if the destination database is in WAL mode
					///and the page sizes are different between source and destination 
					///</summary>
					pgszSrc=this.pSrc.sqlite3BtreeGetPageSize();
					pgszDest=this.pDest.sqlite3BtreeGetPageSize();
					destMode=sqlite3BtreePager(this.pDest).sqlite3PagerGetJournalMode();
					if(SQLITE_OK==rc&&destMode==PAGER_JOURNALMODE_WAL&&pgszSrc!=pgszDest) {
						rc=SQLITE_READONLY;
					}
					///
					///<summary>
					///</summary>
					///<param name="Now that there is a read">lock on the source database, query the</param>
					///<param name="source pager for the number of pages in the database.">source pager for the number of pages in the database.</param>
					///<param name=""></param>
					nSrcPage=this.pSrc.sqlite3BtreeLastPage();
					Debug.Assert(nSrcPage>=0);
					for(ii=0;(nPage<0||ii<nPage)&&this.iNext<=nSrcPage&&0==rc;ii++) {
						Pgno iSrcPg=this.iNext;
						///
						///<summary>
						///Source page number 
						///</summary>
						if(iSrcPg!=PENDING_BYTE_PAGE(this.pSrc.pBt)) {
							DbPage pSrcPg=null;
							///
							///<summary>
							///Source page object 
							///</summary>
							rc=pSrcPager.sqlite3PagerGet((u32)iSrcPg,ref pSrcPg);
							if(rc==SQLITE_OK) {
								rc=this.backupOnePage(iSrcPg,sqlite3PagerGetData(pSrcPg));
								sqlite3PagerUnref(pSrcPg);
							}
						}
						this.iNext++;
					}
					if(rc==SQLITE_OK) {
						this.nPagecount=nSrcPage;
						this.nRemaining=(nSrcPage+1-this.iNext);
						if(this.iNext>nSrcPage) {
							rc=SQLITE_DONE;
						}
						else
							if(0==this.isAttached) {
								this.attachBackupObject();
							}
					}
					///
					///<summary>
					///Update the schema version field in the destination database. This
					///</summary>
					///<param name="is to make sure that the schema">version really does change in</param>
					///<param name="the case where the source and destination databases have the">the case where the source and destination databases have the</param>
					///<param name="same schema version.">same schema version.</param>
					///<param name=""></param>
					if(rc==SQLITE_DONE&&(rc=this.pDest.sqlite3BtreeUpdateMeta(1,this.iDestSchema+1))==SQLITE_OK) {
						Pgno nDestTruncate;
						if(this.pDestDb!=null) {
							sqlite3ResetInternalSchema(this.pDestDb,-1);
						}
						///
						///<summary>
						///Set nDestTruncate to the final number of pages in the destination
						///database. The complication here is that the destination page
						///size may be different to the source page size.
						///
						///If the source page size is smaller than the destination page size,
						///round up. In this case the call to sqlite3OsTruncate() below will
						///fix the size of the file. However it is important to call
						///sqlite3PagerTruncateImage() here so that any pages in the
						///destination file that lie beyond the nDestTruncate page mark are
						///journalled by PagerCommitPhaseOne() before they are destroyed
						///by the file truncation.
						///
						///</summary>
						Debug.Assert(pgszSrc==this.pSrc.sqlite3BtreeGetPageSize());
						Debug.Assert(pgszDest==this.pDest.sqlite3BtreeGetPageSize());
						if(pgszSrc<pgszDest) {
							int ratio=pgszDest/pgszSrc;
							nDestTruncate=(Pgno)((nSrcPage+ratio-1)/ratio);
							if(nDestTruncate==(int)PENDING_BYTE_PAGE(this.pDest.pBt)) {
								nDestTruncate--;
							}
						}
						else {
							nDestTruncate=(Pgno)(nSrcPage*(pgszSrc/pgszDest));
						}
						pDestPager.sqlite3PagerTruncateImage(nDestTruncate);
						if(pgszSrc<pgszDest) {
							///
							///<summary>
							///</summary>
							///<param name="If the source page">size,</param>
							///<param name="two extra things may need to happen:">two extra things may need to happen:</param>
							///<param name=""></param>
							///<param name="The destination may need to be truncated, and">The destination may need to be truncated, and</param>
							///<param name=""></param>
							///<param name="Data stored on the pages immediately following the">Data stored on the pages immediately following the</param>
							///<param name="pending">byte page in the source database may need to be</param>
							///<param name="copied into the destination database.">copied into the destination database.</param>
							///<param name=""></param>
							int iSize=(int)(pgszSrc*nSrcPage);
							sqlite3_file pFile=pDestPager.sqlite3PagerFile();
							i64 iOff;
							i64 iEnd;
							Debug.Assert(pFile!=null);
							Debug.Assert((i64)nDestTruncate*(i64)pgszDest>=iSize||(nDestTruncate==(int)(PENDING_BYTE_PAGE(this.pDest.pBt)-1)&&iSize>=PENDING_BYTE&&iSize<=PENDING_BYTE+pgszDest));
							///
							///<summary>
							///This call ensures that all data required to recreate the original
							///database has been stored in the journal for pDestPager and the
							///journal synced to disk. So at this point we may safely modify
							///the database file in any way, knowing that if a power failure
							///occurs, the original database will be reconstructed from the 
							///journal file.  
							///</summary>
							rc=pDestPager.sqlite3PagerCommitPhaseOne(null,true);
							///
							///<summary>
							///Write the extra pages and truncate the database file as required. 
							///</summary>
							iEnd=MIN(PENDING_BYTE+pgszDest,iSize);
							for(iOff=PENDING_BYTE+pgszSrc;rc==SQLITE_OK&&iOff<iEnd;iOff+=pgszSrc) {
								PgHdr pSrcPg=null;
								u32 iSrcPg=(u32)((iOff/pgszSrc)+1);
								rc=pSrcPager.sqlite3PagerGet(iSrcPg,ref pSrcPg);
								if(rc==SQLITE_OK) {
									byte[] zData=sqlite3PagerGetData(pSrcPg);
									rc=sqlite3OsWrite(pFile,zData,pgszSrc,iOff);
								}
								sqlite3PagerUnref(pSrcPg);
							}
							if(rc==SQLITE_OK) {
								rc=pFile.backupTruncateFile((int)iSize);
							}
							///
							///<summary>
							///Sync the database file to disk. 
							///</summary>
							if(rc==SQLITE_OK) {
								rc=pDestPager.sqlite3PagerSync();
							}
						}
						else {
							rc=pDestPager.sqlite3PagerCommitPhaseOne(null,false);
						}
						///
						///<summary>
						///Finish committing the transaction to the destination database. 
						///</summary>
						if(SQLITE_OK==rc&&SQLITE_OK==(rc=sqlite3BtreeCommitPhaseTwo(this.pDest,0))) {
							rc=SQLITE_DONE;
						}
					}
					///
					///<summary>
					///If bCloseTrans is true, then this function opened a read transaction
					///on the source database. Close the read transaction here. There is
					///no need to check the return values of the btree methods here, as
					///</summary>
					///<param name=""committing" a read">only transaction cannot fail.</param>
					///<param name=""></param>
					if(bCloseTrans!=0) {
						#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																																	      //TESTONLY( int rc2 );
      //TESTONLY( rc2  = ) sqlite3BtreeCommitPhaseOne(p.pSrc, 0);
      //TESTONLY( rc2 |= ) sqlite3BtreeCommitPhaseTwo(p.pSrc);
      int rc2;
      rc2 = sqlite3BtreeCommitPhaseOne( p.pSrc, "" );
      rc2 |= sqlite3BtreeCommitPhaseTwo( p.pSrc, 0 );
      Debug.Assert( rc2 == SQLITE_OK );
#else
						sqlite3BtreeCommitPhaseOne(this.pSrc,null);
						sqlite3BtreeCommitPhaseTwo(this.pSrc,0);
						#endif
					}
					if(rc==SQLITE_IOERR_NOMEM) {
						rc=SQLITE_NOMEM;
					}
					this.rc=rc;
				}
				if(this.pDestDb!=null) {
					sqlite3_mutex_leave(this.pDestDb.mutex);
				}
				sqlite3BtreeLeave(this.pSrc);
				sqlite3_mutex_leave(this.pSrcDb.mutex);
				return rc;
			}
			///<summary>
			/// Release all resources associated with an sqlite3_backup* handle.
			///</summary>
			public int sqlite3_backup_finish() {
				sqlite3_backup pp;
				///
				///<summary>
				///Ptr to head of pagers backup list 
				///</summary>
				sqlite3_mutex mutex;
				///
				///<summary>
				///Mutex to protect source database 
				///</summary>
				int rc;
				///
				///<summary>
				///Value to return 
				///</summary>
				///
				///<summary>
				///Enter the mutexes 
				///</summary>
				if(this==null)
					return SQLITE_OK;
				sqlite3_mutex_enter(this.pSrcDb.mutex);
				sqlite3BtreeEnter(this.pSrc);
				mutex=this.pSrcDb.mutex;
				if(this.pDestDb!=null) {
					sqlite3_mutex_enter(this.pDestDb.mutex);
				}
				///
				///<summary>
				///Detach this backup from the source pager. 
				///</summary>
				if(this.pDestDb!=null) {
					this.pSrc.nBackup--;
				}
				if(this.isAttached!=0) {
					pp=sqlite3BtreePager(this.pSrc).sqlite3PagerBackupPtr();
					while(pp!=this) {
						pp=(pp).pNext;
					}
					sqlite3BtreePager(this.pSrc).pBackup=this.pNext;
				}
				///
				///<summary>
				///If a transaction is still open on the Btree, roll it back. 
				///</summary>
				sqlite3BtreeRollback(this.pDest);
				///
				///<summary>
				///Set the error code of the destination database handle. 
				///</summary>
				rc=(this.rc==SQLITE_DONE)?SQLITE_OK:this.rc;
				sqlite3Error(this.pDestDb,rc,0);
				///
				///<summary>
				///Exit the mutexes and free the backup context structure. 
				///</summary>
				if(this.pDestDb!=null) {
					sqlite3_mutex_leave(this.pDestDb.mutex);
				}
				sqlite3BtreeLeave(this.pSrc);
				if(this.pDestDb!=null) {
					///
					///<summary>
					///</summary>
					///<param name="EVIDENCE">21591 The sqlite3_backup object is created by a</param>
					///<param name="call to sqlite3_backup_init() and is destroyed by a call to">call to sqlite3_backup_init() and is destroyed by a call to</param>
					///<param name="sqlite3_backup_finish(). ">sqlite3_backup_finish(). </param>
					//sqlite3_free( ref p );
				}
				sqlite3_mutex_leave(mutex);
				return rc;
			}
			public///<summary>
			/// Return the number of pages still to be backed up as of the most recent
			/// call to sqlite3_backup_step().
			///</summary>
			int sqlite3_backup_remaining() {
				return (int)this.nRemaining;
			}
			public///<summary>
			/// Return the total number of pages in the source database as of the most
			/// recent call to sqlite3_backup_step().
			///</summary>
			int sqlite3_backup_pagecount() {
				return (int)this.nPagecount;
			}
			public///<summary>
			/// Parameter zSrcData points to a buffer containing the data for
			/// page iSrcPg from the source database. Copy this data into the
			/// destination database.
			///</summary>
			int backupOnePage(Pgno iSrcPg,byte[] zSrcData) {
				Pager pDestPager=sqlite3BtreePager(this.pDest);
				int nSrcPgsz=this.pSrc.sqlite3BtreeGetPageSize();
				int nDestPgsz=this.pDest.sqlite3BtreeGetPageSize();
				int nCopy=MIN(nSrcPgsz,nDestPgsz);
				i64 iEnd=(i64)iSrcPg*(i64)nSrcPgsz;
				#if SQLITE_HAS_CODEC
				int nSrcReserve=this.pSrc.sqlite3BtreeGetReserve();
				int nDestReserve=this.pDest.sqlite3BtreeGetReserve();
				#endif
				int rc=SQLITE_OK;
				i64 iOff;
				Debug.Assert(this.bDestLocked!=0);
				Debug.Assert(!isFatalError(this.rc));
				Debug.Assert(iSrcPg!=PENDING_BYTE_PAGE(this.pSrc.pBt));
				Debug.Assert(zSrcData!=null);
				///
				///<summary>
				///</summary>
				///<param name="Catch the case where the destination is an in">memory database and the</param>
				///<param name="page sizes of the source and destination differ.">page sizes of the source and destination differ.</param>
				///<param name=""></param>
				if(nSrcPgsz!=nDestPgsz&&pDestPager.sqlite3PagerIsMemdb()) {
					rc=SQLITE_READONLY;
				}
				#if SQLITE_HAS_CODEC
				///
				///<summary>
				///Backup is not possible if the page size of the destination is changing
				///and a codec is in use.
				///
				///</summary>
				if(nSrcPgsz!=nDestPgsz&&pDestPager.sqlite3PagerGetCodec()!=null) {
					rc=SQLITE_READONLY;
				}
				///
				///<summary>
				///Backup is not possible if the number of bytes of reserve space differ
				///between source and destination.  If there is a difference, try to
				///fix the destination to agree with the source.  If that is not possible,
				///then the backup cannot proceed.
				///
				///</summary>
				if(nSrcReserve!=nDestReserve) {
					u32 newPgsz=(u32)nSrcPgsz;
					rc=pDestPager.sqlite3PagerSetPagesize(ref newPgsz,nSrcReserve);
					if(rc==SQLITE_OK&&newPgsz!=nSrcPgsz)
						rc=SQLITE_READONLY;
				}
				#endif
				///
				///<summary>
				///This loop runs once for each destination page spanned by the source
				///page. For each iteration, variable iOff is set to the byte offset
				///of the destination page.
				///
				///</summary>
				for(iOff=iEnd-(i64)nSrcPgsz;rc==SQLITE_OK&&iOff<iEnd;iOff+=nDestPgsz) {
					DbPage pDestPg=null;
					u32 iDest=(u32)(iOff/nDestPgsz)+1;
					if(iDest==PENDING_BYTE_PAGE(this.pDest.pBt))
						continue;
					if(SQLITE_OK==(rc=pDestPager.sqlite3PagerGet(iDest,ref pDestPg))&&SQLITE_OK==(rc=sqlite3PagerWrite(pDestPg))) {
						//string zIn = &zSrcData[iOff%nSrcPgsz];
						byte[] zDestData=sqlite3PagerGetData(pDestPg);
						//string zOut = &zDestData[iOff % nDestPgsz];
						///
						///<summary>
						///Copy the data from the source page into the destination page.
						///Then clear the Btree layer MemPage.isInit flag. Both this module
						///and the pager code use this trick (clearing the first byte
						///of the page 'extra' space to invalidate the Btree layers
						///cached parse of the page). MemPage.isInit is marked
						///"MUST BE FIRST" for this purpose.
						///
						///</summary>
						Buffer.BlockCopy(zSrcData,(int)(iOff%nSrcPgsz),zDestData,(int)(iOff%nDestPgsz),nCopy);
						// memcpy( zOut, zIn, nCopy );
						sqlite3PagerGetExtra(pDestPg).isInit=0;
						// ( sqlite3PagerGetExtra( pDestPg ) )[0] = 0;
					}
					sqlite3PagerUnref(pDestPg);
				}
				return rc;
			}
			public///<summary>
			/// Register this backup object with the associated source pager for
			/// callbacks when pages are changed or the cache invalidated.
			///</summary>
			void attachBackupObject() {
				sqlite3_backup pp;
				Debug.Assert(sqlite3BtreeHoldsMutex(this.pSrc));
				pp=sqlite3BtreePager(this.pSrc).sqlite3PagerBackupPtr();
				this.pNext=pp;
				sqlite3BtreePager(this.pSrc).pBackup=this;
				//*pp = p;
				this.isAttached=1;
			}
			public///<summary>
			/// Attempt to set the page size of the destination to match the page size
			/// of the source.
			///</summary>
			int setDestPgsz() {
				int rc;
				rc=this.pDest.sqlite3BtreeSetPageSize(this.pSrc.sqlite3BtreeGetPageSize(),-1,0);
				return rc;
			}
		}
		///<summary>
		/// THREAD SAFETY NOTES:
		///
		///   Once it has been created using backup_init(), a single sqlite3_backup
		///   structure may be accessed via two groups of thread-safe entry points:
		///
		///     * Via the sqlite3_backup_XXX() API function backup_step() and
		///       backup_finish(). Both these functions obtain the source database
		///       handle mutex and the mutex associated with the source BtShared
		///       structure, in that order.
		///
		///     * Via the BackupUpdate() and BackupRestart() functions, which are
		///       invoked by the pager layer to report various state changes in
		///       the page cache associated with the source database. The mutex
		///       associated with the source database BtShared structure will always
		///       be held when either of these functions are invoked.
		///
		///   The other sqlite3_backup_XXX() API functions, backup_remaining() and
		///   backup_pagecount() are not thread-safe functions. If they are called
		///   while some other thread is calling backup_step() or backup_finish(),
		///   the values returned may be invalid. There is no way for a call to
		///   BackupUpdate() or BackupRestart() to interfere with backup_remaining()
		///   or backup_pagecount().
		///
		///   Depending on the SQLite configuration, the database handles and/or
		///   the Btree objects may have their own mutexes that require locking.
		///   Non-sharable Btrees (in-memory databases for example), do not have
		///   associated mutexes.
		///</summary>
		///<summary>
		/// Return a pointer corresponding to database zDb (i.e. "main", "temp")
		/// in connection handle pDb. If such a database cannot be found, return
		/// a NULL pointer and write an error message to pErrorDb.
		///
		/// If the "temp" database is requested, it may need to be opened by this
		/// function. If an error occurs while doing so, return 0 and write an
		/// error message to pErrorDb.
		///</summary>
		static Btree findBtree(sqlite3 pErrorDb,sqlite3 pDb,string zDb) {
			int i=sqlite3FindDbName(pDb,zDb);
			if(i==1) {
				Parse pParse;
				int rc=0;
				pParse=new Parse();
				//sqlite3StackAllocZero(pErrorDb, sizeof(*pParse));
				if(pParse==null) {
					sqlite3Error(pErrorDb,SQLITE_NOMEM,"out of memory");
					rc=SQLITE_NOMEM;
				}
				else {
					pParse.db=pDb;
					if(sqlite3OpenTempDatabase(pParse)!=0) {
						sqlite3Error(pErrorDb,pParse.rc,"%s",pParse.zErrMsg);
						rc=SQLITE_ERROR;
					}
					pErrorDb.sqlite3DbFree(ref pParse.zErrMsg);
					//sqlite3StackFree( pErrorDb, pParse );
				}
				if(rc!=0) {
					return null;
				}
			}
			if(i<0) {
				sqlite3Error(pErrorDb,SQLITE_ERROR,"unknown database %s",zDb);
				return null;
			}
			return pDb.aDb[i].pBt;
		}
		///<summary>
		/// Create an sqlite3_backup process to copy the contents of zSrcDb from
		/// connection handle pSrcDb to zDestDb in pDestDb. If successful, return
		/// a pointer to the new sqlite3_backup object.
		///
		/// If an error occurs, NULL is returned and an error code and error message
		/// stored in database handle pDestDb.
		///</summary>
		static public sqlite3_backup sqlite3_backup_init(sqlite3 pDestDb,///
		///<summary>
		///Database to write to 
		///</summary>
		string zDestDb,///
		///<summary>
		///Name of database within pDestDb 
		///</summary>
		sqlite3 pSrcDb,///
		///<summary>
		///Database connection to read from 
		///</summary>
		string zSrcDb///
		///<summary>
		///Name of database within pSrcDb 
		///</summary>
		) {
			sqlite3_backup p;
			///
			///<summary>
			///Value to return 
			///</summary>
			///
			///<summary>
			///Lock the source database handle. The destination database
			///handle is not locked in this routine, but it is locked in
			///sqlite3_backup_step(). The user is required to ensure that no
			///other thread accesses the destination handle for the duration
			///of the backup operation.  Any attempt to use the destination
			///database connection while a backup is in progress may cause
			///a malfunction or a deadlock.
			///
			///</summary>
			sqlite3_mutex_enter(pSrcDb.mutex);
			sqlite3_mutex_enter(pDestDb.mutex);
			if(pSrcDb==pDestDb) {
				sqlite3Error(pDestDb,SQLITE_ERROR,"source and destination must be distinct");
				p=null;
			}
			else {
				///
				///<summary>
				///Allocate space for a new sqlite3_backup object...
				///</summary>
				///<param name="EVIDENCE">21591 The sqlite3_backup object is created by a</param>
				///<param name="call to sqlite3_backup_init() and is destroyed by a call to">call to sqlite3_backup_init() and is destroyed by a call to</param>
				///<param name="sqlite3_backup_finish(). ">sqlite3_backup_finish(). </param>
				p=new sqlite3_backup();
				// (sqlite3_backup)sqlite3_malloc( sizeof( sqlite3_backup ) );
				//if ( null == p )
				//{
				//  sqlite3Error( pDestDb, SQLITE_NOMEM, 0 );
				//}
			}
			///
			///<summary>
			///If the allocation succeeded, populate the new object. 
			///</summary>
			if(p!=null) {
				// memset( p, 0, sizeof( sqlite3_backup ) );
				p.pSrc=findBtree(pDestDb,pSrcDb,zSrcDb);
				p.pDest=findBtree(pDestDb,pDestDb,zDestDb);
				p.pDestDb=pDestDb;
				p.pSrcDb=pSrcDb;
				p.iNext=1;
				p.isAttached=0;
				if(null==p.pSrc||null==p.pDest||p.setDestPgsz()==SQLITE_NOMEM) {
					///
					///<summary>
					///One (or both) of the named databases did not exist or an OOM
					///error was hit.  The error has already been written into the
					///pDestDb handle.  All that is left to do here is free the
					///sqlite3_backup structure.
					///
					///</summary>
					//sqlite3_free( ref p );
					p=null;
				}
			}
			if(p!=null) {
				p.pSrc.nBackup++;
			}
			sqlite3_mutex_leave(pDestDb.mutex);
			sqlite3_mutex_leave(pSrcDb.mutex);
			return p;
		}
		///<summary>
		/// Argument rc is an SQLite error code. Return true if this error is
		/// considered fatal if encountered during a backup operation. All errors
		/// are considered fatal except for SQLITE_BUSY and SQLITE_LOCKED.
		///</summary>
		static bool isFatalError(int rc) {
			return (rc!=SQLITE_OK&&rc!=SQLITE_BUSY&&ALWAYS(rc!=SQLITE_LOCKED));
		}
	#if !SQLITE_OMIT_VACUUM
	#endif
	}
}
