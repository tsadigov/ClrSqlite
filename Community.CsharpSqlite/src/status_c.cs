using System;
using System.Diagnostics;
using System.Text;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
		///
		///<summary>
		///2008 June 18
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
		///This module implements the sqlite3_status() interface and related
		///functionality.
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
		///<summary>
		/// Variables in which to record status information.
		///
		///</summary>
		//typedef struct sqlite3StatType sqlite3StatType;
		public class sqlite3StatType {
			public int[] nowValue=new int[10];
			///
			///<summary>
			///Current value 
			///</summary>
			public int[] mxValue=new int[10];
		///
		///<summary>
		///Maximum value 
		///</summary>
		}
		public static sqlite3StatType sqlite3Stat=new sqlite3StatType();
		///<summary>
		///The "wsdStat" macro will resolve to the status information
		/// state vector.  If writable static data is unsupported on the target,
		/// we have to locate the state vector at run-time.  In the more common
		/// case where writable static data is supported, wsdStat can refer directly
		/// to the "sqlite3Stat" state vector declared above.
		///
		///</summary>
		#if SQLITE_OMIT_WSD
																																								// define wsdStatInit  sqlite3StatType *x = &GLOBAL(sqlite3StatType,sqlite3Stat)
// define wsdStat x[0]
#else
		//# define wsdStatInit
		static void wsdStatInit() {
		}
		//# define wsdStat sqlite3Stat
		static sqlite3StatType wsdStat=sqlite3Stat;
		#endif
		///<summary>
		/// Return the current value of a status parameter.
		///</summary>
		static int sqlite3StatusValue(int op) {
			wsdStatInit();
			Debug.Assert(op>=0&&op<Sqlite3.ArraySize(wsdStat.nowValue));
			return wsdStat.nowValue[op];
		}
		///<summary>
		/// Add N to the value of a status record.  It is assumed that the
		/// caller holds appropriate locks.
		///
		///</summary>
		static void sqlite3StatusAdd(int op,int N) {
			wsdStatInit();
			Debug.Assert(op>=0&&op<Sqlite3.ArraySize(wsdStat.nowValue));
			wsdStat.nowValue[op]+=N;
			if(wsdStat.nowValue[op]>wsdStat.mxValue[op]) {
				wsdStat.mxValue[op]=wsdStat.nowValue[op];
			}
		}
		///<summary>
		/// Set the value of a status to X.
		///
		///</summary>
		static void sqlite3StatusSet(int op,int X) {
			wsdStatInit();
			Debug.Assert(op>=0&&op<Sqlite3.ArraySize(wsdStat.nowValue));
			wsdStat.nowValue[op]=X;
			if(wsdStat.nowValue[op]>wsdStat.mxValue[op]) {
				wsdStat.mxValue[op]=wsdStat.nowValue[op];
			}
		}
		///<summary>
		/// Query status information.
		///
		/// This implementation assumes that reading or writing an aligned
		/// 32-bit integer is an atomic operation.  If that assumption is not true,
		/// then this routine is not threadsafe.
		///
		///</summary>
		static int sqlite3_status(int op,ref int pCurrent,ref int pHighwater,int resetFlag) {
			wsdStatInit();
			if(op<0||op>=Sqlite3.ArraySize(wsdStat.nowValue)) {
				return Sqlite3.sqliteinth.SQLITE_MISUSE_BKPT();
			}
			pCurrent=wsdStat.nowValue[op];
			pHighwater=wsdStat.mxValue[op];
			if(resetFlag!=0) {
				wsdStat.mxValue[op]=wsdStat.nowValue[op];
			}
			return SQLITE_OK;
		}
		///
		///<summary>
		///Query status information for a single database connection
		///
		///</summary>
		static int sqlite3_db_status(sqlite3 db,///
		///<summary>
		///The database connection whose status is desired 
		///</summary>
		int op,///
		///<summary>
		///Status verb 
		///</summary>
		ref int pCurrent,///
		///<summary>
		///Write current value here 
		///</summary>
		ref int pHighwater,///
		///<summary>
		///</summary>
		///<param name="Write high">water mark here </param>
		int resetFlag///
		///<summary>
		///</summary>
		///<param name="Reset high">water mark if true </param>
		) {
			int rc=SQLITE_OK;
			///
			///<summary>
			///Return code 
			///</summary>
			sqlite3_mutex_enter(db.mutex);
			switch(op) {
			case SQLITE_DBSTATUS_LOOKASIDE_USED: {
				pCurrent=db.lookaside.nOut;
				pHighwater=db.lookaside.mxOut;
				if(resetFlag!=0) {
					db.lookaside.mxOut=db.lookaside.nOut;
				}
				break;
			}
			case SQLITE_DBSTATUS_LOOKASIDE_HIT:
			case SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE:
			case SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL: {
				sqliteinth.testcase(op==SQLITE_DBSTATUS_LOOKASIDE_HIT);
				sqliteinth.testcase(op==SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE);
				sqliteinth.testcase(op==SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL);
				Debug.Assert((op-SQLITE_DBSTATUS_LOOKASIDE_HIT)>=0);
				Debug.Assert((op-SQLITE_DBSTATUS_LOOKASIDE_HIT)<3);
				pCurrent=0;
				pHighwater=db.lookaside.anStat[op-SQLITE_DBSTATUS_LOOKASIDE_HIT];
				if(resetFlag!=0) {
					db.lookaside.anStat[op-SQLITE_DBSTATUS_LOOKASIDE_HIT]=0;
				}
				break;
			}
			///
			///<summary>
			///
			///Return an approximation for the amount of memory currently used
			///by all pagers associated with the given database connection.  The
			///highwater mark is meaningless and is returned as zero.
			///
			///</summary>
			case SQLITE_DBSTATUS_CACHE_USED: {
				int totalUsed=0;
				int i;
				sqlite3BtreeEnterAll(db);
				for(i=0;i<db.nDb;i++) {
					Btree pBt=db.aDb[i].pBt;
					if(pBt!=null) {
						Pager pPager=pBt.sqlite3BtreePager();
						totalUsed+=pPager.sqlite3PagerMemUsed();
					}
				}
				sqlite3BtreeLeaveAll(db);
				pCurrent=totalUsed;
				pHighwater=0;
				break;
			}
			///
			///<summary>
			///pCurrent gets an accurate estimate of the amount of memory used
			///to store the schema for all databases (main, temp, and any ATTACHed
			///databases.  *pHighwater is set to zero.
			///
			///</summary>
			case SQLITE_DBSTATUS_SCHEMA_USED: {
				int i;
				///
				///<summary>
				///Used to iterate through schemas 
				///</summary>
				int nByte=0;
				///
				///<summary>
				///Used to accumulate return value 
				///</summary>
				sqlite3BtreeEnterAll(db);
				//db.pnBytesFreed = nByte;
				for(i=0;i<db.nDb;i++) {
					Schema pSchema=db.aDb[i].pSchema;
					if(Sqlite3.ALWAYS(pSchema!=null)) {
						HashElem p;
						//nByte += (int)(Sqlite3.sqliteinth.sqlite3GlobalConfig.m.xRoundup(sizeof(HashElem)) * (
						//    pSchema.tblHash.count 
						//  + pSchema.trigHash.count
						//  + pSchema.idxHash.count
						//  + pSchema.fkeyHash.count
						//));
						//nByte += (int)malloc_cs.sqlite3MallocSize( pSchema.tblHash.ht );
						//nByte += (int)malloc_cs.sqlite3MallocSize( pSchema.trigHash.ht );
						//nByte += (int)malloc_cs.sqlite3MallocSize( pSchema.idxHash.ht );
						//nByte += (int)malloc_cs.sqlite3MallocSize( pSchema.fkeyHash.ht );
						for(p=sqliteHashFirst(pSchema.trigHash);p!=null;p=sqliteHashNext(p)) {
							Trigger t=(Trigger)sqliteHashData(p);
							sqlite3DeleteTrigger(db,ref t);
						}
						for(p=sqliteHashFirst(pSchema.tblHash);p!=null;p=sqliteHashNext(p)) {
							Table t=(Table)sqliteHashData(p);
							build.sqlite3DeleteTable(db,ref t);
						}
					}
				}
				db.pnBytesFreed=0;
				sqlite3BtreeLeaveAll(db);
				pHighwater=0;
				pCurrent=nByte;
				break;
			}
			///
			///<summary>
			///pCurrent gets an accurate estimate of the amount of memory used
			///to store all prepared statements.
			///pHighwater is set to zero.
			///
			///</summary>
			case SQLITE_DBSTATUS_STMT_USED: {
				Vdbe pVdbe;
				///
				///<summary>
				///Used to iterate through VMs 
				///</summary>
				int nByte=0;
				///
				///<summary>
				///Used to accumulate return value 
				///</summary>
				//db.pnBytesFreed = nByte;
				for(pVdbe=db.pVdbe;pVdbe!=null;pVdbe=pVdbe.pNext) {
                    vdbeaux.sqlite3VdbeDeleteObject(db, ref pVdbe);
				}
				db.pnBytesFreed=0;
				pHighwater=0;
				pCurrent=nByte;
				break;
			}
			default: {
				rc=SQLITE_ERROR;
				break;
			}
			}
			sqlite3_mutex_leave(db.mutex);
			return rc;
		}
	}
}
