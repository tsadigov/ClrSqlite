using System;
using System.Diagnostics;
using System.Text;
using Pgno=System.UInt32;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	public partial class Sqlite3 {
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
		/// This file contains C code routines that are called by the parser
		/// to handle INSERT statements in SQLite.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
		///
		///
		///
		///</summary>
		//#include "sqliteInt.h"
		///<summary>
		/// Generate code that will open a table for reading.
		///
		///</summary>
		static void sqlite3OpenTable(Parse p,/* Generate code into this VDBE */int iCur,/* The cursor number of the table */int iDb,/* The database index in sqlite3.aDb[] */Table pTab,/* The table to be opened */int opcode/* OP_OpenRead or OP_OpenWrite */) {
			Vdbe v;
			if(IsVirtual(pTab))
				return;
			v=sqlite3GetVdbe(p);
			Debug.Assert(opcode==OP_OpenWrite||opcode==OP_OpenRead);
			sqlite3TableLock(p,iDb,pTab.tnum,(opcode==OP_OpenWrite)?(byte)1:(byte)0,pTab.zName);
			v.sqlite3VdbeAddOp3(opcode,iCur,pTab.tnum,iDb);
			v.sqlite3VdbeChangeP4(-1,(pTab.nCol),P4_INT32);
			//SQLITE_INT_TO_PTR( pTab.nCol ), P4_INT32 );
			VdbeComment(v,"%s",pTab.zName);
		}
		///<summary>
		/// Return a pointer to the column affinity string associated with index
		/// pIdx. A column affinity string has one character for each column in
		/// the table, according to the affinity of the column:
		///
		///  Character      Column affinity
		///  ------------------------------
		///  'a'            TEXT
		///  'b'            NONE
		///  'c'            NUMERIC
		///  'd'            INTEGER
		///  'e'            REAL
		///
		/// An extra 'b' is appended to the end of the string to cover the
		/// rowid that appears as the last column in every index.
		///
		/// Memory for the buffer containing the column index affinity string
		/// is managed along with the rest of the Index structure. It will be
		/// released when sqlite3DeleteIndex() is called.
		///
		///</summary>
		///<summary>
		/// Set P4 of the most recently inserted opcode to a column affinity
		/// string for table pTab. A column affinity string has one character
		/// for each column indexed by the index, according to the affinity of the
		/// column:
		///
		///  Character      Column affinity
		///  ------------------------------
		///  'a'            TEXT
		///  'b'            NONE
		///  'c'            NUMERIC
		///  'd'            INTEGER
		///  'e'            REAL
		///
		///</summary>
		///<summary>
		/// Return non-zero if the table pTab in database iDb or any of its indices
		/// have been opened at any point in the VDBE program beginning at location
		/// iStartAddr throught the end of the program.  This is used to see if
		/// a statement of the form  "INSERT INTO <iDb, pTab> SELECT ..." can
		/// run without using temporary table for the results of the SELECT.
		///
		///</summary>
		static bool readsTable(Parse p,int iStartAddr,int iDb,Table pTab) {
			Vdbe v=sqlite3GetVdbe(p);
			int i;
			int iEnd=v.sqlite3VdbeCurrentAddr();
			#if !SQLITE_OMIT_VIRTUALTABLE
			VTable pVTab=IsVirtual(pTab)?sqlite3GetVTable(p.db,pTab):null;
			#endif
			for(i=iStartAddr;i<iEnd;i++) {
				VdbeOp pOp=v.sqlite3VdbeGetOp(i);
				Debug.Assert(pOp!=null);
				if(pOp.opcode==OP_OpenRead&&pOp.p3==iDb) {
					Index pIndex;
					int tnum=pOp.p2;
					if(tnum==pTab.tnum) {
						return true;
					}
					for(pIndex=pTab.pIndex;pIndex!=null;pIndex=pIndex.pNext) {
						if(tnum==pIndex.tnum) {
							return true;
						}
					}
				}
				#if !SQLITE_OMIT_VIRTUALTABLE
				if(pOp.opcode==OP_VOpen&&pOp.p4.pVtab==pVTab) {
					Debug.Assert(pOp.p4.pVtab!=null);
					Debug.Assert(pOp.p4type==P4_VTAB);
					return true;
				}
				#endif
			}
			return false;
		}
		#if !SQLITE_OMIT_AUTOINCREMENT
		///<summary>
		/// Locate or create an AutoincInfo structure associated with table pTab
		/// which is in database iDb.  Return the register number for the register
		/// that holds the maximum rowid.
		///
		/// There is at most one AutoincInfo structure per table even if the
		/// same table is autoincremented multiple times due to inserts within
		/// triggers.  A new AutoincInfo structure is created if this is the
		/// first use of table pTab.  On 2nd and subsequent uses, the original
		/// AutoincInfo structure is used.
		///
		/// Three memory locations are allocated:
		///
		///   (1)  Register to hold the name of the pTab table.
		///   (2)  Register to hold the maximum ROWID of pTab.
		///   (3)  Register to hold the rowid in sqlite_sequence of pTab
		///
		/// The 2nd register is the one that is returned.  That is all the
		/// insert routine needs to know about.
		///</summary>
		static int autoIncBegin(Parse pParse,/* Parsing context */int iDb,/* Index of the database holding pTab */Table pTab/* The table we are writing to */) {
			int memId=0;
			/* Register holding maximum rowid */if((pTab.tabFlags&TF_Autoincrement)!=0) {
				Parse pToplevel=sqlite3ParseToplevel(pParse);
				AutoincInfo pInfo;
				pInfo=pToplevel.pAinc;
				while(pInfo!=null&&pInfo.pTab!=pTab) {
					pInfo=pInfo.pNext;
				}
				if(pInfo==null) {
					pInfo=new AutoincInfo();
					//sqlite3DbMallocRaw(pParse.db, sizeof(*pInfo));
					//if( pInfo==0 ) return 0;
					pInfo.pNext=pToplevel.pAinc;
					pToplevel.pAinc=pInfo;
					pInfo.pTab=pTab;
					pInfo.iDb=iDb;
					pToplevel.nMem++;
					/* Register to hold name of table */pInfo.regCtr=++pToplevel.nMem;
					/* Max rowid register */pToplevel.nMem++;
					/* Rowid in sqlite_sequence */}
				memId=pInfo.regCtr;
			}
			return memId;
		}
		///<summary>
		/// This routine generates code that will initialize all of the
		/// register used by the autoincrement tracker.
		///
		///</summary>
		static void sqlite3AutoincrementBegin(Parse pParse) {
			AutoincInfo p;
			/* Information about an AUTOINCREMENT */sqlite3 db=pParse.db;
			/* The database connection */Db pDb;
			/* Database only autoinc table */int memId;
			/* Register holding max rowid */int addr;
			/* A VDBE address */Vdbe v=pParse.pVdbe;
			/* VDBE under construction *//* This routine is never called during trigger-generation.  It is
      ** only called from the top-level */Debug.Assert(pParse.pTriggerTab==null);
			Debug.Assert(pParse==sqlite3ParseToplevel(pParse));
			Debug.Assert(v!=null);
			/* We failed long ago if this is not so */for(p=pParse.pAinc;p!=null;p=p.pNext) {
				pDb=db.aDb[p.iDb];
				memId=p.regCtr;
				Debug.Assert(sqlite3SchemaMutexHeld(db,0,pDb.pSchema));
				sqlite3OpenTable(pParse,0,p.iDb,pDb.pSchema.pSeqTab,OP_OpenRead);
				addr=v.sqlite3VdbeCurrentAddr();
				v.sqlite3VdbeAddOp4(OP_String8,0,memId-1,0,p.pTab.zName,0);
				v.sqlite3VdbeAddOp2(OP_Rewind,0,addr+9);
				v.sqlite3VdbeAddOp3(OP_Column,0,0,memId);
				v.sqlite3VdbeAddOp3(OP_Ne,memId-1,addr+7,memId);
				v.sqlite3VdbeChangeP5(SQLITE_JUMPIFNULL);
				v.sqlite3VdbeAddOp2(OP_Rowid,0,memId+1);
				v.sqlite3VdbeAddOp3(OP_Column,0,1,memId);
				v.sqlite3VdbeAddOp2(OP_Goto,0,addr+9);
				v.sqlite3VdbeAddOp2(OP_Next,0,addr+2);
				v.sqlite3VdbeAddOp2(OP_Integer,0,memId);
				v.sqlite3VdbeAddOp0(OP_Close);
			}
		}
		///<summary>
		/// Update the maximum rowid for an autoincrement calculation.
		///
		/// This routine should be called when the top of the stack holds a
		/// new rowid that is about to be inserted.  If that new rowid is
		/// larger than the maximum rowid in the memId memory cell, then the
		/// memory cell is updated.  The stack is unchanged.
		///
		///</summary>
		static void autoIncStep(Parse pParse,int memId,int regRowid) {
			if(memId>0) {
				pParse.pVdbe.sqlite3VdbeAddOp2(OP_MemMax,memId,regRowid);
			}
		}
		/*
    ** This routine generates the code needed to write autoincrement
    ** maximum rowid values back into the sqlite_sequence register.
    ** Every statement that might do an INSERT into an autoincrement
    ** table (either directly or through triggers) needs to call this
    ** routine just before the "exit" code.
    */static void sqlite3AutoincrementEnd(Parse pParse) {
			AutoincInfo p;
			Vdbe v=pParse.pVdbe;
			sqlite3 db=pParse.db;
			Debug.Assert(v!=null);
			for(p=pParse.pAinc;p!=null;p=p.pNext) {
				Db pDb=db.aDb[p.iDb];
				int j1,j2,j3,j4,j5;
				int iRec;
				int memId=p.regCtr;
				iRec=sqlite3GetTempReg(pParse);
				Debug.Assert(sqlite3SchemaMutexHeld(db,0,pDb.pSchema));
				sqlite3OpenTable(pParse,0,p.iDb,pDb.pSchema.pSeqTab,OP_OpenWrite);
				j1=v.sqlite3VdbeAddOp1(OP_NotNull,memId+1);
				j2=v.sqlite3VdbeAddOp0(OP_Rewind);
				j3=v.sqlite3VdbeAddOp3(OP_Column,0,0,iRec);
				j4=v.sqlite3VdbeAddOp3(OP_Eq,memId-1,0,iRec);
				v.sqlite3VdbeAddOp2(OP_Next,0,j3);
				v.sqlite3VdbeJumpHere(j2);
				v.sqlite3VdbeAddOp2(OP_NewRowid,0,memId+1);
				j5=v.sqlite3VdbeAddOp0(OP_Goto);
				v.sqlite3VdbeJumpHere(j4);
				v.sqlite3VdbeAddOp2(OP_Rowid,0,memId+1);
				v.sqlite3VdbeJumpHere(j1);
				v.sqlite3VdbeJumpHere(j5);
				v.sqlite3VdbeAddOp3(OP_MakeRecord,memId-1,2,iRec);
				v.sqlite3VdbeAddOp3(OP_Insert,0,iRec,memId+1);
				v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
				v.sqlite3VdbeAddOp0(OP_Close);
				sqlite3ReleaseTempReg(pParse,iRec);
			}
		}
		#else
																																								/*
** If SQLITE_OMIT_AUTOINCREMENT is defined, then the three routines
** above are all no-ops
*/
// define autoIncBegin(A,B,C) (0)
// define autoIncStep(A,B,C)
#endif
		/* Forward declaration *///static int xferOptimization(
		//  Parse pParse,        /* Parser context */
		//  Table pDest,         /* The table we are inserting into */
		//  Select pSelect,      /* A SELECT statement to use as the data source */
		//  int onError,          /* How to handle constraint errors */
		//  int iDbDest           /* The database of pDest */
		//);
		///<summary>
		/// This routine is call to handle SQL of the following forms:
		///
		///    insert into TABLE (IDLIST) values(EXPRLIST)
		///    insert into TABLE (IDLIST) select
		///
		/// The IDLIST following the table name is always optional.  If omitted,
		/// then a list of all columns for the table is substituted.  The IDLIST
		/// appears in the pColumn parameter.  pColumn is NULL if IDLIST is omitted.
		///
		/// The pList parameter holds EXPRLIST in the first form of the INSERT
		/// statement above, and pSelect is NULL.  For the second form, pList is
		/// NULL and pSelect is a pointer to the select statement used to generate
		/// data for the insert.
		///
		/// The code generated follows one of four templates.  For a simple
		/// select with data coming from a VALUES clause, the code executes
		/// once straight down through.  Pseudo-code follows (we call this
		/// the "1st template"):
		///
		///         open write cursor to <table> and its indices
		///         puts VALUES clause expressions onto the stack
		///         write the resulting record into <table>
		///         cleanup
		///
		/// The three remaining templates assume the statement is of the form
		///
		///   INSERT INTO <table> SELECT ...
		///
		/// If the SELECT clause is of the restricted form "SELECT * FROM <table2>" -
		/// in other words if the SELECT pulls all columns from a single table
		/// and there is no WHERE or LIMIT or GROUP BY or ORDER BY clauses, and
		/// if <table2> and <table1> are distinct tables but have identical
		/// schemas, including all the same indices, then a special optimization
		/// is invoked that copies raw records from <table2> over to <table1>.
		/// See the xferOptimization() function for the implementation of this
		/// template.  This is the 2nd template.
		///
		///         open a write cursor to <table>
		///         open read cursor on <table2>
		///         transfer all records in <table2> over to <table>
		///         close cursors
		///         foreach index on <table>
		///           open a write cursor on the <table> index
		///           open a read cursor on the corresponding <table2> index
		///           transfer all records from the read to the write cursors
		///           close cursors
		///         end foreach
		///
		/// The 3rd template is for when the second template does not apply
		/// and the SELECT clause does not read from <table> at any time.
		/// The generated code follows this template:
		///
		///         EOF <- 0
		///         X <- A
		///         goto B
		///      A: setup for the SELECT
		///         loop over the rows in the SELECT
		///           load values into registers R..R+n
		///           yield X
		///         end loop
		///         cleanup after the SELECT
		///         EOF <- 1
		///         yield X
		///         goto A
		///      B: open write cursor to <table> and its indices
		///      C: yield X
		///         if EOF goto D
		///         insert the select result into <table> from R..R+n
		///         goto C
		///      D: cleanup
		///
		/// The 4th template is used if the insert statement takes its
		/// values from a SELECT but the data is being inserted into a table
		/// that is also read as part of the SELECT.  In the third form,
		/// we have to use a intermediate table to store the results of
		/// the select.  The template is like this:
		///
		///         EOF <- 0
		///         X <- A
		///         goto B
		///      A: setup for the SELECT
		///         loop over the tables in the SELECT
		///           load value into register R..R+n
		///           yield X
		///         end loop
		///         cleanup after the SELECT
		///         EOF <- 1
		///         yield X
		///         halt-error
		///      B: open temp table
		///      L: yield X
		///         if EOF goto M
		///         insert row from R..R+n into temp table
		///         goto L
		///      M: open write cursor to <table> and its indices
		///         rewind temp table
		///      C: loop over rows of intermediate table
		///           transfer values form intermediate table into <table>
		///         end loop
		///      D: cleanup
		///
		///</summary>
		// OVERLOADS, so I don't need to rewrite parse.c
		///<summary>
		///Make sure "isView" and other macros defined above are undefined. Otherwise
		/// thely may interfere with compilation of other functions in this file
		/// (or in another file, if this file becomes part of the amalgamation).
		///</summary>
		//#if isView
		// #undef isView
		//#endif
		//#if pTrigger
		// #undef pTrigger
		//#endif
		//#if tmask
		// #undef tmask
		//#endif
		///<summary>
		/// Generate code to do constraint checks prior to an INSERT or an UPDATE.
		///
		/// The input is a range of consecutive registers as follows:
		///
		///    1.  The rowid of the row after the update.
		///
		///    2.  The data in the first column of the entry after the update.
		///
		///    i.  Data from middle columns...
		///
		///    N.  The data in the last column of the entry after the update.
		///
		/// The regRowid parameter is the index of the register containing (1).
		///
		/// If isUpdate is true and rowidChng is non-zero, then rowidChng contains
		/// the address of a register containing the rowid before the update takes
		/// place. isUpdate is true for UPDATEs and false for INSERTs. If isUpdate
		/// is false, indicating an INSERT statement, then a non-zero rowidChng
		/// indicates that the rowid was explicitly specified as part of the
		/// INSERT statement. If rowidChng is false, it means that  the rowid is
		/// computed automatically in an insert or that the rowid value is not
		/// modified by an update.
		///
		/// The code generated by this routine store new index entries into
		/// registers identified by aRegIdx[].  No index entry is created for
		/// indices where aRegIdx[i]==0.  The order of indices in aRegIdx[] is
		/// the same as the order of indices on the linked list of indices
		/// attached to the table.
		///
		/// This routine also generates code to check constraints.  NOT NULL,
		/// CHECK, and UNIQUE constraints are all checked.  If a constraint fails,
		/// then the appropriate action is performed.  There are five possible
		/// actions: ROLLBACK, ABORT, FAIL, REPLACE, and IGNORE.
		///
		///  Constraint type  Action       What Happens
		///  ---------------  ----------   ----------------------------------------
		///  any              ROLLBACK     The current transaction is rolled back and
		///                                sqlite3_exec() returns immediately with a
		///                                return code of SQLITE_CONSTRAINT.
		///
		///  any              ABORT        Back out changes from the current command
		///                                only (do not do a complete rollback) then
		///                                cause sqlite3_exec() to return immediately
		///                                with SQLITE_CONSTRAINT.
		///
		///  any              FAIL         Sqlite_exec() returns immediately with a
		///                                return code of SQLITE_CONSTRAINT.  The
		///                                transaction is not rolled back and any
		///                                prior changes are retained.
		///
		///  any              IGNORE       The record number and data is popped from
		///                                the stack and there is an immediate jump
		///                                to label ignoreDest.
		///
		///  NOT NULL         REPLACE      The NULL value is replace by the default
		///                                value for that column.  If the default value
		///                                is NULL, the action is the same as ABORT.
		///
		///  UNIQUE           REPLACE      The other row that conflicts with the row
		///                                being inserted is removed.
		///
		///  CHECK            REPLACE      Illegal.  The results in an exception.
		///
		/// Which action to take is determined by the overrideError parameter.
		/// Or if overrideError==OE_Default, then the pParse.onError parameter
		/// is used.  Or if pParse.onError==OE_Default then the onError value
		/// for the constraint is used.
		///
		/// The calling routine must open a read/write cursor for pTab with
		/// cursor number "baseCur".  All indices of pTab must also have open
		/// read/write cursors with cursor number baseCur+i for the i-th cursor.
		/// Except, if there is no possibility of a REPLACE action then
		/// cursors do not need to be open for indices where aRegIdx[i]==0.
		///
		///</summary>
		static void sqlite3GenerateConstraintChecks(Parse pParse,/* The parser context */Table pTab,/* the table into which we are inserting */int baseCur,/* Index of a read/write cursor pointing at pTab */int regRowid,/* Index of the range of input registers */int[] aRegIdx,/* Register used by each index.  0 for unused indices */int rowidChng,/* True if the rowid might collide with existing entry */bool isUpdate,/* True for UPDATE, False for INSERT */int overrideError,/* Override onError to this if not OE_Default */int ignoreDest,/* Jump to this label on an OE_Ignore resolution */out int pbMayReplace/* OUT: Set to true if constraint may cause a replace */) {
			int i;
			/* loop counter */Vdbe v;
			/* VDBE under constrution */int nCol;
			/* Number of columns */int onError;
			/* Conflict resolution strategy */int j1;
			/* Addresss of jump instruction */int j2=0,j3;
			/* Addresses of jump instructions */int regData;
			/* Register containing first data column */int iCur;
			/* Table cursor number */Index pIdx;
			/* Pointer to one of the indices */bool seenReplace=false;
			/* True if REPLACE is used to resolve INT PK conflict */int regOldRowid=(rowidChng!=0&&isUpdate)?rowidChng:regRowid;
			v=sqlite3GetVdbe(pParse);
			Debug.Assert(v!=null);
			Debug.Assert(pTab.pSelect==null);
			/* This table is not a VIEW */nCol=pTab.nCol;
			regData=regRowid+1;
			/* Test all NOT NULL constraints.
      */for(i=0;i<nCol;i++) {
				if(i==pTab.iPKey) {
					continue;
				}
				onError=pTab.aCol[i].notNull;
				if(onError==OE_None)
					continue;
				if(overrideError!=OE_Default) {
					onError=overrideError;
				}
				else
					if(onError==OE_Default) {
						onError=OE_Abort;
					}
				if(onError==OE_Replace&&pTab.aCol[i].pDflt==null) {
					onError=OE_Abort;
				}
				Debug.Assert(onError==OE_Rollback||onError==OE_Abort||onError==OE_Fail||onError==OE_Ignore||onError==OE_Replace);
				switch(onError) {
				case OE_Abort: {
					sqlite3MayAbort(pParse);
					goto case OE_Fail;
				}
				case OE_Rollback:
				case OE_Fail: {
					string zMsg;
					v.sqlite3VdbeAddOp3(OP_HaltIfNull,SQLITE_CONSTRAINT,onError,regData+i);
					zMsg=sqlite3MPrintf(pParse.db,"%s.%s may not be NULL",pTab.zName,pTab.aCol[i].zName);
					v.sqlite3VdbeChangeP4(-1,zMsg,P4_DYNAMIC);
					break;
				}
				case OE_Ignore: {
					v.sqlite3VdbeAddOp2(OP_IsNull,regData+i,ignoreDest);
					break;
				}
				default: {
					Debug.Assert(onError==OE_Replace);
					j1=v.sqlite3VdbeAddOp1(OP_NotNull,regData+i);
					sqlite3ExprCode(pParse,pTab.aCol[i].pDflt,regData+i);
					v.sqlite3VdbeJumpHere(j1);
					break;
				}
				}
			}
			/* Test all CHECK constraints
      */
			#if !SQLITE_OMIT_CHECK
			if(pTab.pCheck!=null&&(pParse.db.flags&SQLITE_IgnoreChecks)==0) {
				int allOk=v.sqlite3VdbeMakeLabel();
				pParse.ckBase=regData;
				sqlite3ExprIfTrue(pParse,pTab.pCheck,allOk,SQLITE_JUMPIFNULL);
				onError=overrideError!=OE_Default?overrideError:OE_Abort;
				if(onError==OE_Ignore) {
					v.sqlite3VdbeAddOp2(OP_Goto,0,ignoreDest);
				}
				else {
					if(onError==OE_Replace)
						onError=OE_Abort;
					/* IMP: R-15569-63625 */sqlite3HaltConstraint(pParse,onError,(string)null,0);
				}
				v.sqlite3VdbeResolveLabel(allOk);
			}
			#endif
			/* If we have an INTEGER PRIMARY KEY, make sure the primary key
** of the new record does not previously exist.  Except, if this
** is an UPDATE and the primary key is not changing, that is OK.
*/if(rowidChng!=0) {
				onError=pTab.keyConf;
				if(overrideError!=OE_Default) {
					onError=overrideError;
				}
				else
					if(onError==OE_Default) {
						onError=OE_Abort;
					}
				if(isUpdate) {
					j2=v.sqlite3VdbeAddOp3(OP_Eq,regRowid,0,rowidChng);
				}
				j3=v.sqlite3VdbeAddOp3(OP_NotExists,baseCur,0,regRowid);
				switch(onError) {
				default:
				{
					onError=OE_Abort;
					/* Fall thru into the next case */}
				goto case OE_Rollback;
				case OE_Rollback:
				case OE_Abort:
				case OE_Fail: {
					sqlite3HaltConstraint(pParse,onError,"PRIMARY KEY must be unique",P4_STATIC);
					break;
				}
				case OE_Replace: {
					/* If there are DELETE triggers on this table and the
              ** recursive-triggers flag is set, call GenerateRowDelete() to
              ** remove the conflicting row from the the table. This will fire
              ** the triggers and remove both the table and index b-tree entries.
              **
              ** Otherwise, if there are no triggers or the recursive-triggers
              ** flag is not set, but the table has one or more indexes, call 
              ** GenerateRowIndexDelete(). This removes the index b-tree entries 
              ** only. The table b-tree entry will be replaced by the new entry 
              ** when it is inserted.  
              **
              ** If either GenerateRowDelete() or GenerateRowIndexDelete() is called,
              ** also invoke MultiWrite() to indicate that this VDBE may require
              ** statement rollback (if the statement is aborted after the delete
              ** takes place). Earlier versions called sqlite3MultiWrite() regardless,
              ** but being more selective here allows statements like:
              **
              **   REPLACE INTO t(rowid) VALUES($newrowid)
              **
              ** to run without a statement journal if there are no indexes on the
              ** table.
              */Trigger pTrigger=null;
					if((pParse.db.flags&SQLITE_RecTriggers)!=0) {
						int iDummy;
						pTrigger=sqlite3TriggersExist(pParse,pTab,TK_DELETE,null,out iDummy);
					}
					if(pTrigger!=null||pParse.sqlite3FkRequired(pTab,null,0)!=0) {
						sqlite3MultiWrite(pParse);
						sqlite3GenerateRowDelete(pParse,pTab,baseCur,regRowid,0,pTrigger,OE_Replace);
					}
					else
						if(pTab.pIndex!=null) {
							sqlite3MultiWrite(pParse);
							sqlite3GenerateRowIndexDelete(pParse,pTab,baseCur,0);
						}
					seenReplace=true;
					break;
				}
				case OE_Ignore: {
					Debug.Assert(!seenReplace);
					v.sqlite3VdbeAddOp2(OP_Goto,0,ignoreDest);
					break;
				}
				}
				v.sqlite3VdbeJumpHere(j3);
				if(isUpdate) {
					v.sqlite3VdbeJumpHere(j2);
				}
			}
			/* Test all UNIQUE constraints by creating entries for each UNIQUE
      ** index and making sure that duplicate entries do not already exist.
      ** Add the new records to the indices as we go.
      */for(iCur=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,iCur++) {
				int regIdx;
				int regR;
				if(aRegIdx[iCur]==0)
					continue;
				/* Skip unused indices *//* Create a key for accessing the index entry */regIdx=sqlite3GetTempRange(pParse,pIdx.nColumn+1);
				for(i=0;i<pIdx.nColumn;i++) {
					int idx=pIdx.aiColumn[i];
					if(idx==pTab.iPKey) {
						v.sqlite3VdbeAddOp2(OP_SCopy,regRowid,regIdx+i);
					}
					else {
						v.sqlite3VdbeAddOp2(OP_SCopy,regData+idx,regIdx+i);
					}
				}
				v.sqlite3VdbeAddOp2(OP_SCopy,regRowid,regIdx+i);
				v.sqlite3VdbeAddOp3(OP_MakeRecord,regIdx,pIdx.nColumn+1,aRegIdx[iCur]);
				v.sqlite3VdbeChangeP4(-1,v.sqlite3IndexAffinityStr(pIdx),P4_TRANSIENT);
				sqlite3ExprCacheAffinityChange(pParse,regIdx,pIdx.nColumn+1);
				/* Find out what action to take in case there is an indexing conflict */onError=pIdx.onError;
				if(onError==OE_None) {
					sqlite3ReleaseTempRange(pParse,regIdx,pIdx.nColumn+1);
					continue;
					/* pIdx is not a UNIQUE index */}
				if(overrideError!=OE_Default) {
					onError=overrideError;
				}
				else
					if(onError==OE_Default) {
						onError=OE_Abort;
					}
				if(seenReplace) {
					if(onError==OE_Ignore)
						onError=OE_Replace;
					else
						if(onError==OE_Fail)
							onError=OE_Abort;
				}
				/* Check to see if the new index entry will be unique */regR=sqlite3GetTempReg(pParse);
				v.sqlite3VdbeAddOp2(OP_SCopy,regOldRowid,regR);
				j3=v.sqlite3VdbeAddOp4(OP_IsUnique,baseCur+iCur+1,0,regR,regIdx,//regR, SQLITE_INT_TO_PTR(regIdx),
				P4_INT32);
				sqlite3ReleaseTempRange(pParse,regIdx,pIdx.nColumn+1);
				/* Generate code that executes if the new index entry is not unique */Debug.Assert(onError==OE_Rollback||onError==OE_Abort||onError==OE_Fail||onError==OE_Ignore||onError==OE_Replace);
				switch(onError) {
				case OE_Rollback:
				case OE_Abort:
				case OE_Fail: {
					int j;
					StrAccum errMsg=new StrAccum(200);
					string zSep;
					string zErr;
					sqlite3StrAccumInit(errMsg,null,0,200);
					errMsg.db=pParse.db;
					zSep=pIdx.nColumn>1?"columns ":"column ";
					for(j=0;j<pIdx.nColumn;j++) {
						string zCol=pTab.aCol[pIdx.aiColumn[j]].zName;
						sqlite3StrAccumAppend(errMsg,zSep,-1);
						zSep=", ";
						sqlite3StrAccumAppend(errMsg,zCol,-1);
					}
					sqlite3StrAccumAppend(errMsg,pIdx.nColumn>1?" are not unique":" is not unique",-1);
					zErr=sqlite3StrAccumFinish(errMsg);
					sqlite3HaltConstraint(pParse,onError,zErr,0);
					sqlite3DbFree(errMsg.db,ref zErr);
					break;
				}
				case OE_Ignore: {
					Debug.Assert(!seenReplace);
					v.sqlite3VdbeAddOp2(OP_Goto,0,ignoreDest);
					break;
				}
				default: {
					Trigger pTrigger=null;
					Debug.Assert(onError==OE_Replace);
					sqlite3MultiWrite(pParse);
					if((pParse.db.flags&SQLITE_RecTriggers)!=0) {
						int iDummy;
						pTrigger=sqlite3TriggersExist(pParse,pTab,TK_DELETE,null,out iDummy);
					}
					sqlite3GenerateRowDelete(pParse,pTab,baseCur,regR,0,pTrigger,OE_Replace);
					seenReplace=true;
					break;
				}
				}
				v.sqlite3VdbeJumpHere(j3);
				sqlite3ReleaseTempReg(pParse,regR);
			}
			//if ( pbMayReplace )
			{
				pbMayReplace=seenReplace?1:0;
			}
		}
		///<summary>
		/// This routine generates code to finish the INSERT or UPDATE operation
		/// that was started by a prior call to sqlite3GenerateConstraintChecks.
		/// A consecutive range of registers starting at regRowid contains the
		/// rowid and the content to be inserted.
		///
		/// The arguments to this routine should be the same as the first six
		/// arguments to sqlite3GenerateConstraintChecks.
		///
		///</summary>
		static void sqlite3CompleteInsertion(Parse pParse,/* The parser context */Table pTab,/* the table into which we are inserting */int baseCur,/* Index of a read/write cursor pointing at pTab */int regRowid,/* Range of content */int[] aRegIdx,/* Register used by each index.  0 for unused indices */bool isUpdate,/* True for UPDATE, False for INSERT */bool appendBias,/* True if this is likely to be an append */bool useSeekResult/* True to set the USESEEKRESULT flag on OP_[Idx]Insert */) {
			int i;
			Vdbe v;
			int nIdx;
			Index pIdx;
			u8 pik_flags;
			int regData;
			int regRec;
			v=sqlite3GetVdbe(pParse);
			Debug.Assert(v!=null);
			Debug.Assert(pTab.pSelect==null);
			/* This table is not a VIEW */for(nIdx=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,nIdx++) {
			}
			for(i=nIdx-1;i>=0;i--) {
				if(aRegIdx[i]==0)
					continue;
				v.sqlite3VdbeAddOp2(OP_IdxInsert,baseCur+i+1,aRegIdx[i]);
				if(useSeekResult) {
					v.sqlite3VdbeChangeP5(OPFLAG_USESEEKRESULT);
				}
			}
			regData=regRowid+1;
			regRec=sqlite3GetTempReg(pParse);
			v.sqlite3VdbeAddOp3(OP_MakeRecord,regData,pTab.nCol,regRec);
			v.sqlite3TableAffinityStr(pTab);
			sqlite3ExprCacheAffinityChange(pParse,regData,pTab.nCol);
			if(pParse.nested!=0) {
				pik_flags=0;
			}
			else {
				pik_flags=OPFLAG_NCHANGE;
				pik_flags|=(isUpdate?OPFLAG_ISUPDATE:OPFLAG_LASTROWID);
			}
			if(appendBias) {
				pik_flags|=OPFLAG_APPEND;
			}
			if(useSeekResult) {
				pik_flags|=OPFLAG_USESEEKRESULT;
			}
			v.sqlite3VdbeAddOp3(OP_Insert,baseCur,regRec,regRowid);
			if(pParse.nested==0) {
				v.sqlite3VdbeChangeP4(-1,pTab.zName,P4_TRANSIENT);
			}
			v.sqlite3VdbeChangeP5(pik_flags);
		}
		///<summary>
		/// Generate code that will open cursors for a table and for all
		/// indices of that table.  The "baseCur" parameter is the cursor number used
		/// for the table.  Indices are opened on subsequent cursors.
		///
		/// Return the number of indices on the table.
		///
		///</summary>
		static int sqlite3OpenTableAndIndices(Parse pParse,/* Parsing context */Table pTab,/* Table to be opened */int baseCur,/* VdbeCursor number assigned to the table */int op/* OP_OpenRead or OP_OpenWrite */) {
			int i;
			int iDb;
			Index pIdx;
			Vdbe v;
			if(IsVirtual(pTab))
				return 0;
			iDb=sqlite3SchemaToIndex(pParse.db,pTab.pSchema);
			v=sqlite3GetVdbe(pParse);
			Debug.Assert(v!=null);
			sqlite3OpenTable(pParse,baseCur,iDb,pTab,op);
			for(i=1,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
				KeyInfo pKey=sqlite3IndexKeyinfo(pParse,pIdx);
				Debug.Assert(pIdx.pSchema==pTab.pSchema);
				v.sqlite3VdbeAddOp4(op,i+baseCur,pIdx.tnum,iDb,pKey,P4_KEYINFO_HANDOFF);
				#if SQLITE_DEBUG
																																																																																        VdbeComment( v, "%s", pIdx.zName );
#endif
			}
			if(pParse.nTab<baseCur+i) {
				pParse.nTab=baseCur+i;
			}
			return i-1;
		}
		#if SQLITE_TEST
																																								    /*
** The following global variable is incremented whenever the
** transfer optimization is used.  This is used for testing
** purposes only - to make sure the transfer optimization really
** is happening when it is suppose to.
*/
#if !TCLSH
																																								    static int sqlite3_xferopt_count = 0;
#else
																																								    static tcl.lang.Var.SQLITE3_GETSET sqlite3_xferopt_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_xferopt_count" );
#endif
																																								#endif
		#if !SQLITE_OMIT_XFER_OPT
		///<summary>
		/// Check to collation names to see if they are compatible.
		///</summary>
		static bool xferCompatibleCollation(string z1,string z2) {
			if(z1==null) {
				return z2==null;
			}
			if(z2==null) {
				return false;
			}
			return z1.Equals(z2,StringComparison.InvariantCultureIgnoreCase);
		}
		///<summary>
		/// Check to see if index pSrc is compatible as a source of data
		/// for index pDest in an insert transfer optimization.  The rules
		/// for a compatible index:
		///
		///    *   The index is over the same set of columns
		///    *   The same DESC and ASC markings occurs on all columns
		///    *   The same onError processing (OE_Abort, OE_Ignore, etc)
		///    *   The same collating sequence on each column
		///
		///</summary>
		static bool xferCompatibleIndex(Index pDest,Index pSrc) {
			int i;
			Debug.Assert(pDest!=null&&pSrc!=null);
			Debug.Assert(pDest.pTable!=pSrc.pTable);
			if(pDest.nColumn!=pSrc.nColumn) {
				return false;
				/* Different number of columns */}
			if(pDest.onError!=pSrc.onError) {
				return false;
				/* Different conflict resolution strategies */}
			for(i=0;i<pSrc.nColumn;i++) {
				if(pSrc.aiColumn[i]!=pDest.aiColumn[i]) {
					return false;
					/* Different columns indexed */}
				if(pSrc.aSortOrder[i]!=pDest.aSortOrder[i]) {
					return false;
					/* Different sort orders */}
				if(!xferCompatibleCollation(pSrc.azColl[i],pDest.azColl[i])) {
					return false;
					/* Different collating sequences */}
			}
			/* If no test above fails then the indices must be compatible */return true;
		}
		/*
    ** Attempt the transfer optimization on INSERTs of the form
    **
    **     INSERT INTO tab1 SELECT * FROM tab2;
    **
    ** This optimization is only attempted if
    **
    **    (1)  tab1 and tab2 have identical schemas including all the
    **         same indices and constraints
    **
    **    (2)  tab1 and tab2 are different tables
    **
    **    (3)  There must be no triggers on tab1
    **
    **    (4)  The result set of the SELECT statement is "*"
    **
    **    (5)  The SELECT statement has no WHERE, HAVING, ORDER BY, GROUP BY,
    **         or LIMIT clause.
    **
    **    (6)  The SELECT statement is a simple (not a compound) select that
    **         contains only tab2 in its FROM clause
    **
    ** This method for implementing the INSERT transfers raw records from
    ** tab2 over to tab1.  The columns are not decoded.  Raw records from
    ** the indices of tab2 are transfered to tab1 as well.  In so doing,
    ** the resulting tab1 has much less fragmentation.
    **
    ** This routine returns TRUE if the optimization is attempted.  If any
    ** of the conditions above fail so that the optimization should not
    ** be attempted, then this routine returns FALSE.
    */static int xferOptimization(Parse pParse,/* Parser context */Table pDest,/* The table we are inserting into */Select pSelect,/* A SELECT statement to use as the data source */int onError,/* How to handle constraint errors */int iDbDest/* The database of pDest */) {
			ExprList pEList;
			/* The result set of the SELECT */Table pSrc;
			/* The table in the FROM clause of SELECT */Index pSrcIdx,pDestIdx;
			/* Source and destination indices */SrcList_item pItem;
			/* An element of pSelect.pSrc */int i;
			/* Loop counter */int iDbSrc;
			/* The database of pSrc */int iSrc,iDest;
			/* Cursors from source and destination */int addr1,addr2;
			/* Loop addresses */int emptyDestTest;
			/* Address of test for empty pDest */int emptySrcTest;
			/* Address of test for empty pSrc */Vdbe v;
			/* The VDBE we are building */KeyInfo pKey;
			/* Key information for an index */int regAutoinc;
			/* Memory register used by AUTOINC */bool destHasUniqueIdx=false;
			/* True if pDest has a UNIQUE index */int regData,regRowid;
			/* Registers holding data and rowid */if(pSelect==null) {
				return 0;
				/* Must be of the form  INSERT INTO ... SELECT ... */}
			#if !SQLITE_OMIT_TRIGGER
			if(sqlite3TriggerList(pParse,pDest)!=null) {
				return 0;
				/* tab1 must not have triggers */}
			#endif
			if((pDest.tabFlags&TF_Virtual)!=0) {
				return 0;
				/* tab1 must not be a virtual table */}
			if(onError==OE_Default) {
				onError=OE_Abort;
			}
			if(onError!=OE_Abort&&onError!=OE_Rollback) {
				return 0;
				/* Cannot do OR REPLACE or OR IGNORE or OR FAIL */}
			Debug.Assert(pSelect.pSrc!=null);
			/* allocated even if there is no FROM clause */if(pSelect.pSrc.nSrc!=1) {
				return 0;
				/* FROM clause must have exactly one term */}
			if(pSelect.pSrc.a[0].pSelect!=null) {
				return 0;
				/* FROM clause cannot contain a subquery */}
			if(pSelect.pWhere!=null) {
				return 0;
				/* SELECT may not have a WHERE clause */}
			if(pSelect.pOrderBy!=null) {
				return 0;
				/* SELECT may not have an ORDER BY clause */}
			/* Do not need to test for a HAVING clause.  If HAVING is present but
      ** there is no ORDER BY, we will get an error. */if(pSelect.pGroupBy!=null) {
				return 0;
				/* SELECT may not have a GROUP BY clause */}
			if(pSelect.pLimit!=null) {
				return 0;
				/* SELECT may not have a LIMIT clause */}
			Debug.Assert(pSelect.pOffset==null);
			/* Must be so if pLimit==0 */if(pSelect.pPrior!=null) {
				return 0;
				/* SELECT may not be a compound query */}
			if((pSelect.selFlags&SelectFlags.Distinct)!=0) {
				return 0;
				/* SELECT may not be DISTINCT */}
			pEList=pSelect.pEList;
			Debug.Assert(pEList!=null);
			if(pEList.nExpr!=1) {
				return 0;
				/* The result set must have exactly one column */}
			Debug.Assert(pEList.a[0].pExpr!=null);
			if(pEList.a[0].pExpr.op!=TK_ALL) {
				return 0;
				/* The result set must be the special operator "*" */}
			/* At this point we have established that the statement is of the
      ** correct syntactic form to participate in this optimization.  Now
      ** we have to check the semantics.
      */pItem=pSelect.pSrc.a[0];
			pSrc=sqlite3LocateTable(pParse,0,pItem.zName,pItem.zDatabase);
			if(pSrc==null) {
				return 0;
				/* FROM clause does not contain a real table */}
			if(pSrc==pDest) {
				return 0;
				/* tab1 and tab2 may not be the same table */}
			if((pSrc.tabFlags&TF_Virtual)!=0) {
				return 0;
				/* tab2 must not be a virtual table */}
			if(pSrc.pSelect!=null) {
				return 0;
				/* tab2 may not be a view */}
			if(pDest.nCol!=pSrc.nCol) {
				return 0;
				/* Number of columns must be the same in tab1 and tab2 */}
			if(pDest.iPKey!=pSrc.iPKey) {
				return 0;
				/* Both tables must have the same INTEGER PRIMARY KEY */}
			for(i=0;i<pDest.nCol;i++) {
				if(pDest.aCol[i].affinity!=pSrc.aCol[i].affinity) {
					return 0;
					/* Affinity must be the same on all columns */}
				if(!xferCompatibleCollation(pDest.aCol[i].zColl,pSrc.aCol[i].zColl)) {
					return 0;
					/* Collating sequence must be the same on all columns */}
				if(pDest.aCol[i].notNull!=0&&pSrc.aCol[i].notNull==0) {
					return 0;
					/* tab2 must be NOT NULL if tab1 is */}
			}
			for(pDestIdx=pDest.pIndex;pDestIdx!=null;pDestIdx=pDestIdx.pNext) {
				if(pDestIdx.onError!=OE_None) {
					destHasUniqueIdx=true;
				}
				for(pSrcIdx=pSrc.pIndex;pSrcIdx!=null;pSrcIdx=pSrcIdx.pNext) {
					if(xferCompatibleIndex(pDestIdx,pSrcIdx))
						break;
				}
				if(pSrcIdx==null) {
					return 0;
					/* pDestIdx has no corresponding index in pSrc */}
			}
			#if !SQLITE_OMIT_CHECK
			if(pDest.pCheck!=null&&0!=sqlite3ExprCompare(pSrc.pCheck,pDest.pCheck)) {
				return 0;
				/* Tables have different CHECK constraints.  Ticket #2252 */}
			#endif
			#if !SQLITE_OMIT_FOREIGN_KEY
			/* Disallow the transfer optimization if the destination table constains
  ** any foreign key constraints.  This is more restrictive than necessary.
  ** But the main beneficiary of the transfer optimization is the VACUUM 
  ** command, and the VACUUM command disables foreign key constraints.  So
  ** the extra complication to make this rule less restrictive is probably
  ** not worth the effort.  Ticket [6284df89debdfa61db8073e062908af0c9b6118e]
  */if((pParse.db.flags&SQLITE_ForeignKeys)!=0&&pDest.pFKey!=null) {
				return 0;
			}
			#endif
			/* If we get this far, it means either:
      **
      **    *   We can always do the transfer if the table contains an
      **        an integer primary key
      **
      **    *   We can conditionally do the transfer if the destination
      **        table is empty.
      */
			#if SQLITE_TEST
																																																												#if !TCLSH
																																																												      sqlite3_xferopt_count++;
#else
																																																												      sqlite3_xferopt_count.iValue++;
#endif
																																																												#endif
			iDbSrc=sqlite3SchemaToIndex(pParse.db,pSrc.pSchema);
			v=sqlite3GetVdbe(pParse);
			sqlite3CodeVerifySchema(pParse,iDbSrc);
			iSrc=pParse.nTab++;
			iDest=pParse.nTab++;
			regAutoinc=autoIncBegin(pParse,iDbDest,pDest);
			sqlite3OpenTable(pParse,iDest,iDbDest,pDest,OP_OpenWrite);
			if((pDest.iPKey<0&&pDest.pIndex!=null)||destHasUniqueIdx) {
				/* If tables do not have an INTEGER PRIMARY KEY and there
        ** are indices to be copied and the destination is not empty,
        ** we have to disallow the transfer optimization because the
        ** the rowids might change which will mess up indexing.
        **
        ** Or if the destination has a UNIQUE index and is not empty,
        ** we also disallow the transfer optimization because we cannot
        ** insure that all entries in the union of DEST and SRC will be
        ** unique.
        */addr1=v.sqlite3VdbeAddOp2(OP_Rewind,iDest,0);
				emptyDestTest=v.sqlite3VdbeAddOp2(OP_Goto,0,0);
				v.sqlite3VdbeJumpHere(addr1);
			}
			else {
				emptyDestTest=0;
			}
			sqlite3OpenTable(pParse,iSrc,iDbSrc,pSrc,OP_OpenRead);
			emptySrcTest=v.sqlite3VdbeAddOp2(OP_Rewind,iSrc,0);
			regData=sqlite3GetTempReg(pParse);
			regRowid=sqlite3GetTempReg(pParse);
			if(pDest.iPKey>=0) {
				addr1=v.sqlite3VdbeAddOp2(OP_Rowid,iSrc,regRowid);
				addr2=v.sqlite3VdbeAddOp3(OP_NotExists,iDest,0,regRowid);
				sqlite3HaltConstraint(pParse,onError,"PRIMARY KEY must be unique",P4_STATIC);
				v.sqlite3VdbeJumpHere(addr2);
				autoIncStep(pParse,regAutoinc,regRowid);
			}
			else
				if(pDest.pIndex==null) {
					addr1=v.sqlite3VdbeAddOp2(OP_NewRowid,iDest,regRowid);
				}
				else {
					addr1=v.sqlite3VdbeAddOp2(OP_Rowid,iSrc,regRowid);
					Debug.Assert((pDest.tabFlags&TF_Autoincrement)==0);
				}
			v.sqlite3VdbeAddOp2(OP_RowData,iSrc,regData);
			v.sqlite3VdbeAddOp3(OP_Insert,iDest,regData,regRowid);
			v.sqlite3VdbeChangeP5(OPFLAG_NCHANGE|OPFLAG_LASTROWID|OPFLAG_APPEND);
			v.sqlite3VdbeChangeP4(-1,pDest.zName,0);
			v.sqlite3VdbeAddOp2(OP_Next,iSrc,addr1);
			for(pDestIdx=pDest.pIndex;pDestIdx!=null;pDestIdx=pDestIdx.pNext) {
				for(pSrcIdx=pSrc.pIndex;pSrcIdx!=null;pSrcIdx=pSrcIdx.pNext) {
					if(xferCompatibleIndex(pDestIdx,pSrcIdx))
						break;
				}
				Debug.Assert(pSrcIdx!=null);
				v.sqlite3VdbeAddOp2(OP_Close,iSrc,0);
				v.sqlite3VdbeAddOp2(OP_Close,iDest,0);
				pKey=sqlite3IndexKeyinfo(pParse,pSrcIdx);
				v.sqlite3VdbeAddOp4(OP_OpenRead,iSrc,pSrcIdx.tnum,iDbSrc,pKey,P4_KEYINFO_HANDOFF);
				#if SQLITE_DEBUG
																																																																																        VdbeComment( v, "%s", pSrcIdx.zName );
#endif
				pKey=sqlite3IndexKeyinfo(pParse,pDestIdx);
				v.sqlite3VdbeAddOp4(OP_OpenWrite,iDest,pDestIdx.tnum,iDbDest,pKey,P4_KEYINFO_HANDOFF);
				#if SQLITE_DEBUG
																																																																																        VdbeComment( v, "%s", pDestIdx.zName );
#endif
				addr1=v.sqlite3VdbeAddOp2(OP_Rewind,iSrc,0);
				v.sqlite3VdbeAddOp2(OP_RowKey,iSrc,regData);
				v.sqlite3VdbeAddOp3(OP_IdxInsert,iDest,regData,1);
				v.sqlite3VdbeAddOp2(OP_Next,iSrc,addr1+1);
				v.sqlite3VdbeJumpHere(addr1);
			}
			v.sqlite3VdbeJumpHere(emptySrcTest);
			sqlite3ReleaseTempReg(pParse,regRowid);
			sqlite3ReleaseTempReg(pParse,regData);
			v.sqlite3VdbeAddOp2(OP_Close,iSrc,0);
			v.sqlite3VdbeAddOp2(OP_Close,iDest,0);
			if(emptyDestTest!=0) {
				v.sqlite3VdbeAddOp2(OP_Halt,SQLITE_OK,0);
				v.sqlite3VdbeJumpHere(emptyDestTest);
				v.sqlite3VdbeAddOp2(OP_Close,iDest,0);
				return 0;
			}
			else {
				return 1;
			}
		}
	#endif
	}
}
