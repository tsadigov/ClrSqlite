using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FILE=System.IO.TextWriter;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using unsigned=System.UIntPtr;
using Pgno=System.UInt32;
using i32=System.Int32;
using sqlite_int64=System.Int64;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar=System.Int16;
#else
using ynVar = System.Int32; 
#endif
///
///<summary>
///The yDbMask datatype for the bitmask of all attached databases.
///</summary>
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask=System.Int32;
#endif
namespace Community.CsharpSqlite {
	using Op=VdbeOp;
	using System.Text;
	using sqlite3_value=Mem;
	using System.Collections.Generic;
    using sqliteinth = Sqlite3.sqliteinth;
	public partial class Sqlite3 {
		///
		///<summary>
		///An instance of the virtual machine.  This structure contains the complete
		///state of the virtual machine.
		///
		///The "sqlite3_stmt" structure pointer that is returned by sqlite3_prepare()
		///is really a pointer to an instance of this structure.
		///
		///</summary>
		///<param name="The Vdbe.inVtabMethod variable is set to non">zero for the duration of</param>
		///<param name="any virtual table method invocations made by the vdbe program. It is">any virtual table method invocations made by the vdbe program. It is</param>
		///<param name="set to 2 for xDestroy method calls and 1 for all other methods. This">set to 2 for xDestroy method calls and 1 for all other methods. This</param>
		///<param name="variable is used for two purposes: to allow xDestroy methods to execute">variable is used for two purposes: to allow xDestroy methods to execute</param>
		///<param name=""DROP TABLE" statements and to prevent some nasty side effects of">"DROP TABLE" statements and to prevent some nasty side effects of</param>
		///<param name="malloc failure when SQLite is invoked recursively by a virtual table">malloc failure when SQLite is invoked recursively by a virtual table</param>
		///<param name="method function.">method function.</param>
		///<param name=""></param>
		public class Vdbe {
			public Vdbe() {
			}
			///
			///<summary>
			///The database connection that owns this statement 
			///</summary>
			public sqlite3 db;
			/** Space to hold the virtual machine's program */public Op[] aOp;
			///
			///<summary>
			///The memory locations 
			///</summary>
			public List<Op> lOp=new List<Op>();
			public Mem[] aMem;
			public Mem[] apArg;
			///
			///<summary>
			///Arguments to currently executing user function 
			///</summary>
			public Mem[] aColName;
			///
			///<summary>
			///Column names to return 
			///</summary>
			public Mem[] pResultSet;
			///
			///<summary>
			///Pointer to an array of results 
			///</summary>
			public int nMem;
			///
			///<summary>
			///Number of memory locations currently allocated 
			///</summary>
			public int nOp;
			///
			///<summary>
			///Number of instructions in the program 
			///</summary>
			public int nOpAlloc;
			///
			///<summary>
			///Number of slots allocated for aOp[] 
			///</summary>
			public int nLabel;
			///
			///<summary>
			///Number of labels used 
			///</summary>
			public int nLabelAlloc;
			///
			///<summary>
			///Number of slots allocated in aLabel[] 
			///</summary>
			public int[] aLabel;
			///
			///<summary>
			///Space to hold the labels 
			///</summary>
			public u16 nResColumn;
			///
			///<summary>
			///Number of columns in one row of the result set 
			///</summary>
			public u16 nCursor;
			///
			///<summary>
			///Number of slots in apCsr[] 
			///</summary>
			public VdbeMagic magic;
			///
			///<summary>
			///Magic number for sanity checking 
			///</summary>
			public string zErrMsg;
			///
			///<summary>
			///Error message written here 
			///</summary>
			public Vdbe pPrev;
			///
			///<summary>
			///Linked list of VDBEs with the same Vdbe.db 
			///</summary>
			public Vdbe pNext;
			///
			///<summary>
			///Linked list of VDBEs with the same Vdbe.db 
			///</summary>
			public VdbeCursor[] apCsr;
			///
			///<summary>
			///One element of this array for each open cursor 
			///</summary>
			public Mem[] aVar;
			///
			///<summary>
			///Values for the  OpCode.OP_Variable opcode. 
			///</summary>
			public string[] azVar;
			///
			///<summary>
			///Name of variables 
			///</summary>
			public ynVar nVar;
			///
			///<summary>
			///Number of entries in aVar[] 
			///</summary>
			public ynVar nzVar;
			///
			///<summary>
			///Number of entries in azVar[] 
			///</summary>
			public u32 cacheCtr;
			///
			///<summary>
			///VdbeCursor row cache generation counter 
			///</summary>
			/// <summary>
			/// The program counter 
			/// </summary>
			public int currentOpCodeIndex;
			/// <summary>
			/// Value to return
			/// </summary>
			public int rc;
			public SqlResult result {
				get {
					return (SqlResult)rc;
				}
				set {
					rc=(int)value;
				}
			}
			public OnConstraintError errorAction;
			///
			///<summary>
			///Recovery action to do in case of an error 
			///</summary>
			public int explain;
			///
			///<summary>
			///True if EXPLAIN present on SQL command 
			///</summary>
			public bool changeCntOn;
			///
			///<summary>
			///</summary>
			///<param name="True to update the change">counter </param>
			public bool expired;
			///
			///<summary>
			///True if the VM needs to be recompiled 
			///</summary>
			public u8 runOnlyOnce;
			///
			///<summary>
			///Automatically expire on reset 
			///</summary>
			public int minWriteFileFormat;
			///
			///<summary>
			///Minimum file format for writable database files 
			///</summary>
			public int inVtabMethod;
			///
			///<summary>
			///See comments above 
			///</summary>
			public bool usesStmtJournal;
			///
			///<summary>
			///True if uses a statement journal 
			///</summary>
			public bool readOnly;
			///
			///<summary>
			///</summary>
			///<param name="True for read">only statements </param>
			public int nChange;
			///
			///<summary>
			///Number of db changes made since last reset 
			///</summary>
			public bool isPrepareV2;
			///
			///<summary>
			///True if prepared with prepare_v2() 
			///</summary>
			public yDbMask btreeMask;
			///
			///<summary>
			///Bitmask of db.aDb[] entries referenced 
			///</summary>
			public yDbMask lockMask;
			///
			///<summary>
			///Subset of btreeMask that requires a lock 
			///</summary>
			public int iStatement;
			///
			///<summary>
			///Statement number (or 0 if has not opened stmt) 
			///</summary>
			public int[] aCounter=new int[3];
			///
			///<summary>
			///Counters used by sqlite3_stmt_status() 
			///</summary>
			#if !SQLITE_OMIT_TRACE
			public i64 startTime;
			///
			///<summary>
			///</summary>
			///<param name="Time when query started "> used for profiling </param>
			#endif
			public i64 nFkConstraint;
			///
			///<summary>
			///Number of imm. FK constraints this VM 
			///</summary>
			public i64 nStmtDefCons;
			///
			///<summary>
			///Number of def. constraints when stmt started 
			///</summary>
			public string zSql="";
			///
			///<summary>
			///Text of the SQL statement that generated this 
			///</summary>
			public object pFree;
			///
			///<summary>
			///Free this when deleting the vdbe 
			///</summary>
			#if SQLITE_DEBUG
																																																																														      public FILE trace;             /* Write an execution trace here, if not NULL */
#endif
			public VdbeFrame pFrame;
			///
			///<summary>
			///Parent frame 
			///</summary>
			public VdbeFrame pDelFrame;
			///
			///<summary>
			///List of frame objects to free on VM reset 
			///</summary>
			public int nFrame;
			///
			///<summary>
			///Number of frames in pFrame list 
			///</summary>
			public u32 expmask;
			///<summary>
			///Binding to these vars invalidates VM
			///</summary>
			public SubProgram pProgram;
			///<summary>
			///Linked list of all sub-programs used by VM
			///</summary>
			public Vdbe Copy() {
				Vdbe cp=(Vdbe)MemberwiseClone();
				return cp;
			}
			public void CopyTo(Vdbe ct) {
				ct.db=db;
				ct.pPrev=pPrev;
				ct.pNext=pNext;
				ct.nOp=nOp;
				ct.nOpAlloc=nOpAlloc;
				ct.lOp=lOp;
				ct.nLabel=nLabel;
				ct.nLabelAlloc=nLabelAlloc;
				ct.aLabel=aLabel;
				ct.apArg=apArg;
				ct.aColName=aColName;
				ct.nCursor=nCursor;
				ct.apCsr=apCsr;
				ct.aVar=aVar;
				ct.azVar=azVar;
				ct.nVar=nVar;
				ct.nzVar=nzVar;
				ct.magic=magic;
				ct.nMem=nMem;
				ct.aMem=aMem;
				ct.cacheCtr=cacheCtr;
				ct.currentOpCodeIndex=currentOpCodeIndex;
				ct.rc=rc;
				ct.errorAction=errorAction;
				ct.nResColumn=nResColumn;
				ct.zErrMsg=zErrMsg;
				ct.pResultSet=pResultSet;
				ct.explain=explain;
				ct.changeCntOn=changeCntOn;
				ct.expired=expired;
				ct.minWriteFileFormat=minWriteFileFormat;
				ct.inVtabMethod=inVtabMethod;
				ct.usesStmtJournal=usesStmtJournal;
				ct.readOnly=readOnly;
				ct.nChange=nChange;
				ct.isPrepareV2=isPrepareV2;
				#if !SQLITE_OMIT_TRACE
				ct.startTime=startTime;
				#endif
				ct.btreeMask=btreeMask;
				ct.lockMask=lockMask;
				aCounter.CopyTo(ct.aCounter,0);
				ct.zSql=zSql;
				ct.pFree=pFree;
				#if SQLITE_DEBUG
																																																																																																								        ct.trace = trace;
#endif
				ct.nFkConstraint=nFkConstraint;
				ct.nStmtDefCons=nStmtDefCons;
				ct.iStatement=iStatement;
				ct.pFrame=pFrame;
				ct.nFrame=nFrame;
				ct.expmask=expmask;
				ct.pProgram=pProgram;
				#if SQLITE_SSE
																																																																																																								ct.fetchId=fetchId;
ct.lru=lru;
#endif
				#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																																																																																																								ct.pLruPrev=pLruPrev;
ct.pLruNext=pLruNext;
#endif
			}
			public///<summary>
			/// Function prototypes
			///
			///</summary>
			//void sqlite3VdbeFreeCursor(Vdbe *, VdbeCursor);
			//void sqliteVdbePopStack(Vdbe*,int);
			//int sqlite3VdbeCursorMoveto(VdbeCursor);
			//#if (SQLITE_DEBUG) || defined(VDBE_PROFILE)
			//void sqlite3VdbePrintOp(FILE*, int, Op);
			//#endif
			//u32 sqlite3VdbeSerialTypeLen(u32);
			//u32 sqlite3VdbeSerialType(Mem*, int);
			//u32sqlite3VdbeSerialPut(unsigned char*, int, Mem*, int);
			//u32 sqlite3VdbeSerialGet(const unsigned char*, u32, Mem);
			//void sqlite3VdbeDeleteAuxData(VdbeFunc*, int);
			//int sqlite2BtreeKeyCompare(BtCursor *, const void *, int, int, int );
			//int sqlite3VdbeIdxKeyCompare(VdbeCursor*,UnpackedRecord*,int);
			//int sqlite3VdbeIdxRowid(sqlite3 *, i64 );
			//int sqlite3MemCompare(const Mem*, const Mem*, const CollSeq);
			//int sqlite3VdbeExec(Vdbe);
			//int sqlite3VdbeList(Vdbe);
			//int sqlite3VdbeHalt(Vdbe);
			//int sqlite3VdbeChangeEncoding(Mem *, int);
			//int sqlite3VdbeMemTooBig(Mem);
			//int sqlite3VdbeMemCopy(Mem*, const Mem);
			//void sqlite3VdbeMemShallowCopy(Mem*, const Mem*, int);
			//void sqlite3VdbeMemMove(Mem*, Mem);
			//int sqlite3VdbeMemNulTerminate(Mem);
			//int sqlite3VdbeMemSetStr(Mem*, const char*, int, u8, void()(void));
			//void sqlite3VdbeMemSetInt64(Mem*, i64);
			#if SQLITE_OMIT_FLOATING_POINT
																																																																						// define sqlite3VdbeMemSetDouble sqlite3VdbeMemSetInt64
#else
			//void sqlite3VdbeMemSetDouble(Mem*, double);
			#endif
			//void sqlite3VdbeMemSetNull(Mem);
			//void sqlite3VdbeMemSetZeroBlob(Mem*,int);
			//void sqlite3VdbeMemSetRowSet(Mem);
			//int sqlite3VdbeMemMakeWriteable(Mem);
			//int vdbemem_cs.sqlite3VdbeMemStringify(Mem*, int);
			//i64 Mem.sqlite3VdbeIntValue();
			//int Mem.sqlite3VdbeMemIntegerify();
			//double Mem.sqlite3VdbeRealValue();
			//void Mem.sqlite3VdbeIntegerAffinity();
			//int Mem.sqlite3VdbeMemRealify();
			//int Mem.sqlite3VdbeMemNumerify();
			//int sqlite3VdbeMemFromBtree(BtCursor*,int,int,int,Mem);
			//void Mem p.sqlite3VdbeMemRelease();
			//void Mem p.sqlite3VdbeMemReleaseExternal();
			//int sqlite3VdbeMemFinalize(Mem*, FuncDef);
			//string sqlite3OpcodeName(int);
			//int sqlite3VdbeMemGrow(Mem pMem, int n, int preserve);
			//int sqlite3VdbeCloseStatement(Vdbe *, int);
			//void sqlite3VdbeFrameDelete(VdbeFrame);
			//int sqlite3VdbeFrameRestore(VdbeFrame );
			//void sqlite3VdbeMemStoreType(Mem *pMem);  
			#if !(SQLITE_OMIT_SHARED_CACHE) && SQLITE_THREADSAFE
																																																																						  //void sqlite3VdbeEnter(Vdbe);
  //void sqlite3VdbeLeave(Vdbe);
#else
			//# define sqlite3VdbeEnter(X)
			void sqlite3VdbeEnter() {
			}
			public void sqlite3VdbeLeave() {
			}
			public int sqlite3VdbeAddOp3(int op,int p1,int p2,int p3) {
				return sqlite3VdbeAddOp3((OpCode)op,p1,p2,p3);
			}
			public int sqlite3VdbeAddOp3(OpCode op,int p1,int p2,int p3) {
				int i;
				VdbeOp pOp;
				i=this.nOp;
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				//Debug.Assert(op>0&&op<0xff);
				if(this.nOpAlloc<=i) {
					if(this.growOpArray()!=0) {
						return 1;
					}
				}
				this.nOp++;
				pOp=new VdbeOp();
				pOp.OpCode=op;
				pOp.p5=0;
				pOp.p1=p1;
				pOp.p2=p2;
				pOp.p3=p3;
				pOp.p4.p=null;
				pOp.p4type= P4Usage.P4_NOTUSED;
				lOp.Add(pOp);
				#if SQLITE_DEBUG
																																																																																									      pOp.zComment = null;
      if ( sqlite3VdbeAddopTrace )
        sqlite3VdbePrintOp( null, i, p.aOp[i] );
#endif
				#if VDBE_PROFILE
																																																																																									pOp.cycles = 0;
pOp.cnt = 0;
#endif
				return i;
			}

            public int sqlite3VdbeAddOp0(OpCode op)
            {
                return this.sqlite3VdbeAddOp3(op, 0, 0, 0);
            }
			public int sqlite3VdbeAddOp0(int op) {
				return this.sqlite3VdbeAddOp3(op,0,0,0);
			}
			public int sqlite3VdbeAddOp1(OpCode op,int p1) {
				return this.sqlite3VdbeAddOp3(op,p1,0,0);
			}
			public int sqlite3VdbeAddOp2(int op,int p1,bool b2) {
				return this.sqlite3VdbeAddOp2(op,p1,(int)(b2?1:0));
			}
			public int sqlite3VdbeAddOp2(int op,int p1,int p2) {
				return this.sqlite3VdbeAddOp3(op,p1,p2,0);
			}
			public int sqlite3VdbeAddOp2(OpCode op,int p1,int p2) {
				return this.sqlite3VdbeAddOp3(op,p1,p2,0);
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, i32 pP4, P4Usage p4type) {
                return sqlite3VdbeAddOp4((int)op,p1,p2,p3,pP4,p4type);
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i32 pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.i=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, char pP4, int p4type)
            {
                return sqlite3VdbeAddOp4(op, p1, p2, p3, pP4, (P4Usage)p4type);
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,char pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,StringBuilder pP4,P4Usage p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, StringBuilder pP4, P4Usage p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4.ToString();
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }


			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,string pP4,P4Usage p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, string pP4, P4Usage p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,byte[] pP4,P4Usage p4type) {
				Debug.Assert(op==(u8)OpCode.OP_Null||pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.z=Encoding.UTF8.GetString(pP4,0,pP4.Length);
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, byte[] pP4, P4Usage p4type)
            {
                Debug.Assert(op == OpCode.OP_Null || pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.z = Encoding.UTF8.GetString(pP4, 0, pP4.Length);
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,int[] pP4,P4Usage p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.ai=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, int[] pP4, P4Usage p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.ai = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i64 pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.pI64=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, i64 pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pI64 = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(OpCode op,int p1,int p2,int p3,double pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.pReal=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,FuncDef pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode opCode, yDbMask p1, yDbMask regAgg, yDbMask p2, FuncDef funcDef, P4Usage p4type)
            {
                return sqlite3VdbeAddOp4((int)opCode,p1,regAgg,p2,funcDef,p4type);
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,CollSeq pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, CollSeq pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,KeyInfo pP4,P4Usage p4type) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, KeyInfo pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,VTable pP4,P4Usage p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.pVtab=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, VTable pP4, P4Usage p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.pVtab = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4Int(///
                ///Add the opcode to this VM 
            int op,///
                ///The new opcode 
            int p1,///
                ///The P1 operand 
            int p2,///
                ///The P2 operand 
            int p3,///
                ///The P3 operand 
            int p4///
                ///The P4 operand as an integer 
            )
            {
                union_p4 _p4 = new union_p4();
                _p4.i = p4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4,  P4Usage.P4_INT32);
                return addr;
            }

            public int sqlite3VdbeAddOp4Int(///
                ///Add the opcode to this VM 
            OpCode op,///
                ///The new opcode 
            int p1,///
                ///The P1 operand 
            int p2,///
                ///The P2 operand 
            int p3,///
                ///The P3 operand 
            int p4///
                ///The P4 operand as an integer 
            )
            {
                union_p4 _p4 = new union_p4();
                _p4.i = p4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4,  P4Usage.P4_INT32);
                return addr;
            }

			public int sqlite3VdbeMakeLabel() {
				int i;
				i=this.nLabel++;
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				if(i>=this.nLabelAlloc) {
					int n=this.nLabelAlloc==0?15:this.nLabelAlloc*2+5;
					if(this.aLabel==null)
						this.aLabel=malloc_cs.sqlite3Malloc(this.aLabel,n);
					else
						Array.Resize(ref this.aLabel,n);
					//p.aLabel = sqlite3DbReallocOrFree(p.db, p.aLabel,
					//                                       n*sizeof(p.aLabel[0]));
					this.nLabelAlloc=this.aLabel.Length;
					//sqlite3DbMallocSize(p.db, p.aLabel)/sizeof(p.aLabel[0]);
				}
				if(this.aLabel!=null) {
					this.aLabel[i]=-1;
				}
				return -1-i;
			}
			public void sqlite3VdbeResolveLabel(int x) {
				int j=-1-x;
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				Debug.Assert(j>=0&&j<this.nLabel);
				if(this.aLabel!=null) {
					this.aLabel[j]=this.nOp;
				}
			}
			public void sqlite3VdbeRunOnlyOnce() {
				this.runOnlyOnce=1;
			}
			public void resolveP2Values(ref int pMaxFuncArgs) {
				int i;
				int nMaxArgs=pMaxFuncArgs;
				Op pOp;
				int[] aLabel=this.aLabel;
				this.readOnly=true;
				for(i=0;i<this.nOp;i++)//  for(pOp=p->aOp, i=p->nOp-1; i>=0; i--, pOp++)
				 {
					pOp=this.lOp[i];
					OpCode opcode=pOp.OpCode;
					pOp.opflags=sqlite3OpcodeProperty[(u8)opcode];
                    if (opcode == OpCode.OP_Function || opcode == OpCode.OP_AggStep)
                    {
						if(pOp.p5>nMaxArgs)
							nMaxArgs=pOp.p5;
					}
					else
                        if ((opcode == OpCode.OP_Transaction && pOp.p2 != 0) || opcode == OpCode.OP_Vacuum)
                        {
							this.readOnly=false;
							#if !SQLITE_OMIT_VIRTUALTABLE
						}
						else
                            if (opcode == OpCode.OP_VUpdate)
                            {
								if(pOp.p2>nMaxArgs)
									nMaxArgs=pOp.p2;
							}
							else
                                if (opcode == OpCode.OP_VFilter)
                                {
									int n;
									Debug.Assert(this.nOp-i>=3);
									Debug.Assert(this.lOp[i-1].OpCode==OpCode.OP_Integer);
									//pOp[-1].opcode==OpCode.OP_Integer );
									n=this.lOp[i-1].p1;
									//pOp[-1].p1;
									if(n>nMaxArgs)
										nMaxArgs=n;
									#endif
								}
                    if (((int)pOp.opflags & Sqlite3.OPFLG_JUMP) != 0 && pOp.p2 < 0)
                    {
						Debug.Assert(-1-pOp.p2<this.nLabel);
						pOp.p2=aLabel[-1-pOp.p2];
					}
				}
				this.db.sqlite3DbFree(ref this.aLabel);
				pMaxFuncArgs=nMaxArgs;
			}
			public int sqlite3VdbeCurrentAddr() {
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				return this.nOp;
			}
			public VdbeOp[] sqlite3VdbeTakeOpArray(ref int pnOp,ref int pnMaxArg) {
				List<VdbeOp> lOp=this.lOp;
				Debug.Assert(aOp!=null);
				// && 0==p.db.mallocFailed );
				///
				///<summary>
				///Check that sqlite3VdbeUsesBtree() was not called on this VM 
				///</summary>
				Debug.Assert(this.btreeMask==0);
				this.resolveP2Values(ref pnMaxArg);
				pnOp=this.nOp;
				this.lOp=null;
				return lOp.ToArray();
			}
			public int sqlite3VdbeAddOpList(int nOp,VdbeOpList[] aOp) {
				int addr;
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				if(this.nOp+nOp>this.nOpAlloc&&this.growOpArray()!=0) {
					return 0;
				}
				addr=this.nOp;
				if(Sqlite3.ALWAYS(nOp>0)) {
					int i;
					VdbeOpList pIn;
					for(i=0;i<nOp;i++) {
						pIn=aOp[i];
						int p2=pIn.p2;
						if(this.lOp[i+addr]==null)
							this.lOp[i+addr]=new VdbeOp();
						VdbeOp pOut=this.lOp[i+addr];
						pOut.opcode=pIn.opcode;
						pOut.p1=pIn.p1;
						if(p2<0&&(sqlite3OpcodeProperty[pOut.opcode]&(OpFlag)OPFLG_JUMP)!=0) {
							pOut.p2=addr+(-1-p2);
							// ADDR(p2);
						}
						else {
							pOut.p2=p2;
						}
						pOut.p3=pIn.p3;
						pOut.p4type= P4Usage.P4_NOTUSED;
						pOut.p4.p=null;
						pOut.p5=0;
						#if SQLITE_DEBUG
																																																																																																																																															          pOut.zComment = null;
          if ( sqlite3VdbeAddopTrace )
          {
            sqlite3VdbePrintOp( null, i + addr, p.aOp[i + addr] );
          }
#endif
					}
					this.nOp+=nOp;
				}
				return addr;
			}
			public void sqlite3VdbeChangeP1(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p1=val;
				}
			}
			public void sqlite3VdbeChangeP2(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p2=val;
				}
			}
			public void sqlite3VdbeChangeP3(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p3=val;
				}
			}
			public void sqlite3VdbeChangeP5(u8 val) {
				Debug.Assert(this!=null);
				if(this.lOp!=null) {
					Debug.Assert(this.nOp>0);
					this.lOp[this.nOp-1].p5=val;
				}
			}
            public void sqlite3VdbeChangeP5(OpFlag val)
            {
                sqlite3VdbeChangeP5((u8)val);
            }
            public void sqlite3VdbeChangeP5(OpCode val)
            {
                Debug.Assert(this != null);
                if (this.lOp != null)
                {
                    Debug.Assert(this.nOp > 0);
                    this.lOp[this.nOp - 1].p5 = (u8)val;
                }
            }
			public void sqlite3VdbeJumpHere(int addr) {
				Debug.Assert(addr>=0);
				this.sqlite3VdbeChangeP2(addr,this.nOp);
			}
			public void sqlite3VdbeChangeP4(int addr,CollSeq pColl,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pColl;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,FuncDef pFunc,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pFunc;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,KeyInfo pKeyInfo,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pKeyInfo;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,int i32n,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.i=i32n;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,char c,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.z=c.ToString();
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,Mem m,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.pMem=m;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,string z,dxDel   P4_Type) {
				union_p4 _p4=new union_p4();
				_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4, P4Usage.P4_DYNAMIC);
			}
			public void sqlite3VdbeChangeP4(int addr,SubProgram pProgram,P4Usage n) {
				union_p4 _p4=new union_p4();
				_p4.pProgram=pProgram;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
            public void sqlite3VdbeChangeP4(int addr, string z, P4Usage p4type){
                sqlite3VdbeChangeP4(addr,z,(int)p4type);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, int n)
            {
				union_p4 _p4=new union_p4();
				if(n>0&&n<=z.Length)
					_p4.z=z.Substring(0,n);
				else
					_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4,(P4Usage)n);
			}
			public void sqlite3VdbeChangeP4(int addr,union_p4 _p4,P4Usage n) {
				Op pOp;
				sqlite3 db;
				Debug.Assert(this!=null);
				db=this.db;
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				if(this.lOp==null///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				) {
					if(n!= P4Usage.P4_KEYINFO&&n!= P4Usage.P4_VTAB) {
                        vdbeaux.freeP4(db, n, _p4);
					}
					return;
				}
				Debug.Assert(this.nOp>0);
				Debug.Assert(addr<this.nOp);
				if(addr<0) {
					addr=this.nOp-1;
				}
				pOp=this.lOp[addr];
                vdbeaux.freeP4(db, pOp.p4type, pOp.p4.p);
				pOp.p4.p=null;
				if(n== P4Usage.P4_INT32) {
					///
					///<summary>
					///Note: this cast is safe, because the origin data point was an int
					///that was cast to a (string ). 
					///</summary>
					pOp.p4.i=_p4.i;
					// SQLITE_PTR_TO_INT(zP4);
					pOp.p4type= P4Usage.P4_INT32;
				}
				else
					if(n== P4Usage.P4_INT64) {
						pOp.p4.pI64=_p4.pI64;
						pOp.p4type=n;
					}
					else
						if(n== P4Usage.P4_REAL) {
							pOp.p4.pReal=_p4.pReal;
							pOp.p4type=n;
						}
						else
							if(_p4==null) {
								pOp.p4.p=null;
								pOp.p4type= P4Usage.P4_NOTUSED;
							}
							else
								if(n== P4Usage.P4_KEYINFO) {
									KeyInfo pKeyInfo;
									int nField,nByte;
									nField=_p4.pKeyInfo.nField;
									//nByte = sizeof(*pKeyInfo) + (nField-1)*sizeof(pKeyInfo.aColl[0]) + nField;
									pKeyInfo=new KeyInfo();
									//sqlite3DbMallocRaw(0, nByte);
									pOp.p4.pKeyInfo=pKeyInfo;
									if(pKeyInfo!=null) {
										//u8 *aSortOrder;
										// memcpy((char)pKeyInfo, zP4, nByte - nField);
										//aSortOrder = pKeyInfo.aSortOrder;
										//if( aSortOrder ){
										//  pKeyInfo.aSortOrder = (unsigned char)&pKeyInfo.aColl[nField];
										//  memcpy(pKeyInfo.aSortOrder, aSortOrder, nField);
										//}
										pKeyInfo=_p4.pKeyInfo.Copy();
										pOp.p4type= P4Usage.P4_KEYINFO;
									}
									else {
										//p.db.mallocFailed = 1;
										pOp.p4type= P4Usage.P4_NOTUSED;
									}
									pOp.p4.pKeyInfo=_p4.pKeyInfo;
									pOp.p4type= P4Usage.P4_KEYINFO;
								}
								else
									if(n== P4Usage.P4_KEYINFO_HANDOFF||n== P4Usage.P4_KEYINFO_STATIC) {
										pOp.p4.pKeyInfo=_p4.pKeyInfo;
										pOp.p4type= P4Usage.P4_KEYINFO;
									}
									else
										if(n== P4Usage.P4_FUNCDEF) {
											pOp.p4.pFunc=_p4.pFunc;
											pOp.p4type= P4Usage.P4_FUNCDEF;
										}
										else
											if(n== P4Usage.P4_COLLSEQ) {
												pOp.p4.pColl=_p4.pColl;
												pOp.p4type= P4Usage.P4_COLLSEQ;
											}
											else
												if(n== P4Usage.P4_DYNAMIC||n== P4Usage.P4_STATIC||n== P4Usage.P4_MPRINTF) {
													pOp.p4.z=_p4.z;
													pOp.p4type= P4Usage.P4_DYNAMIC;
												}
												else
													if(n== P4Usage.P4_MEM) {
														pOp.p4.pMem=_p4.pMem;
														pOp.p4type= P4Usage.P4_MEM;
													}
													else
														if(n== P4Usage.P4_INTARRAY) {
															pOp.p4.ai=_p4.ai;
															pOp.p4type= P4Usage.P4_INTARRAY;
														}
														else
															if(n== P4Usage.P4_SUBPROGRAM) {
																pOp.p4.pProgram=_p4.pProgram;
																pOp.p4type= P4Usage.P4_SUBPROGRAM;
															}
															else
																if(n== P4Usage.P4_VTAB) {
																	pOp.p4.pVtab=_p4.pVtab;
																	pOp.p4type= P4Usage.P4_VTAB;
																	vtab.sqlite3VtabLock(_p4.pVtab);
																	Debug.Assert((_p4.pVtab).db==this.db);
																}
																else
																	if(n<0) {
																		pOp.p4.p=_p4.p;
																		pOp.p4type=n;
																	}
																	else {
																		//if (n == 0) n =  n = StringExtensions.sqlite3Strlen30(zP4);
																		pOp.p4.z=_p4.z;
																		// sqlite3DbStrNDup(p.db, zP4, n);
																		pOp.p4type= P4Usage.P4_DYNAMIC;
																	}
			}
			public VdbeOp sqlite3VdbeGetOp(int addr) {
				///
				///<summary>
				///C89 specifies that the constant "dummy" will be initialized to all
				///zeros, which is correct.  MSVC generates a warning, nevertheless. 
				///</summary>
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				if(addr<0) {
					#if SQLITE_OMIT_TRACE
																																																																																																																				if( p.nOp==0 ) return dummy;
#endif
					addr=this.nOp-1;
				}
				Debug.Assert((addr>=0&&addr<this.nOp)///
				///<summary>
				///|| p.db.mallocFailed != 0 
				///</summary>
				);
				//if ( p.db.mallocFailed != 0 )
				//{
				//  return dummy;
				//}
				//else
				{
					return this.lOp[addr];
				}
			}
			public string sqlite3IndexAffinityStr(Index pIdx) {
				if(pIdx.zColAff==null||pIdx.zColAff[0]=='\0') {
					///
					///<summary>
					///The first time a column affinity string for a particular index is
					///required, it is allocated and populated here. It is then stored as
					///a member of the Index structure for subsequent use.
					///
					///The column affinity string will eventually be deleted by
					///sqliteDeleteIndex() when the Index structure itself is cleaned
					///up.
					///
					///</summary>
					int n;
					Table pTab=pIdx.pTable;
					sqlite3 db=this.sqlite3VdbeDb();
					StringBuilder pIdx_zColAff=new StringBuilder(pIdx.nColumn+2);
					// (char )sqlite3DbMallocRaw(0, pIdx->nColumn+2);
					//      if ( pIdx_zColAff == null )
					//      {
					//        db.mallocFailed = 1;
					//        return null;
					//      }
					for(n=0;n<pIdx.nColumn;n++) {
						pIdx_zColAff.Append(pTab.aCol[pIdx.aiColumn[n]].affinity);
					}
                    pIdx_zColAff.Append(sqliteinth.SQLITE_AFF_NONE);
					pIdx_zColAff.Append('\0');
					pIdx.zColAff=pIdx_zColAff.ToString();
				}
				return pIdx.zColAff;
			}
			public void sqlite3TableAffinityStr(Table pTab) {
				///
				///<summary>
				///The first time a column affinity string for a particular table
				///is required, it is allocated and populated here. It is then
				///stored as a member of the Table structure for subsequent use.
				///
				///The column affinity string will eventually be deleted by
				///build.sqlite3DeleteTable() when the Table structure itself is cleaned up.
				///
				///</summary>
				if(pTab.zColAff==null) {
					StringBuilder zColAff;
					int i;
					sqlite3 db=this.sqlite3VdbeDb();
					zColAff=new StringBuilder(pTab.nCol+1);
					// (char)sqlite3DbMallocRaw(0, pTab->nCol+1);
					if(zColAff==null) {
						////        db.mallocFailed = 1;
						return;
					}
					for(i=0;i<pTab.nCol;i++) {
						zColAff.Append(pTab.aCol[i].affinity);
					}
					//zColAff.Append( '\0' );
					pTab.zColAff=zColAff.ToString();
				}
				this.sqlite3VdbeChangeP4(-1,pTab.zColAff, P4Usage.P4_TRANSIENT);
			}
			public void sqlite3ColumnDefault(Table pTab,int i,int iReg) {
				Debug.Assert(pTab!=null);
				if(null==pTab.pSelect) {
					sqlite3_value pValue=new sqlite3_value();
					SqliteEncoding enc=sqliteinth.ENC(this.sqlite3VdbeDb());
					Column pCol=pTab.aCol[i];
					#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
					Debug.Assert(i<pTab.nCol);
                    vdbemem_cs.sqlite3ValueFromExpr(this.sqlite3VdbeDb(), pCol.pDflt, enc, pCol.affinity, ref pValue);
					if(pValue!=null) {
						this.sqlite3VdbeChangeP4(-1,pValue, P4Usage.P4_MEM);
					}
					#if !SQLITE_OMIT_FLOATING_POINT
                    if (iReg >= 0 && pTab.aCol[i].affinity == sqliteinth.SQLITE_AFF_REAL)
                    {
						this.sqlite3VdbeAddOp1(OpCode.OP_RealAffinity,iReg);
					}
					#endif
				}
			}
			public void sqlite3ExprCodeGetColumnOfTable(///
			///<summary>
			///The VDBE under construction 
			///</summary>
			Table pTab,///
			///<summary>
			///The table containing the value 
			///</summary>
			int iTabCur,///
			///<summary>
			///The cursor for this table 
			///</summary>
			int iCol,///
			///<summary>
			///Index of the column to extract 
			///</summary>
			int regOut///
			///<summary>
			///Extract the value into this register 
			///</summary>
			) {
				if(iCol<0||iCol==pTab.iPKey) {
                    this.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iTabCur, regOut);
				}
				else {
                    OpCode op = pTab.IsVirtual() ? OpCode.OP_VColumn : OpCode.OP_Column;
					this.sqlite3VdbeAddOp3(op,iTabCur,iCol,regOut);
				}
				if(iCol>=0) {
					this.sqlite3ColumnDefault(pTab,iCol,regOut);
				}
			}
			public bool vdbeSafety() {
				if(this.db==null) {
					io.sqlite3_log(SQLITE_MISUSE,"API called with finalized prepared statement");
					return true;
				}
				else {
					return false;
				}
			}
			public bool vdbeSafetyNotNull() {
				if(this==null) {
					io.sqlite3_log(SQLITE_MISUSE,"API called with NULL prepared statement");
					return true;
				}
				else {
					return this.vdbeSafety();
				}
			}
			public int growOpArray() {
				//VdbeOp pNew;
				int nNew=(this.nOpAlloc!=0?this.nOpAlloc*2:1024/4);
				//(int)(1024/sizeof(Op)));
				// pNew = sqlite3DbRealloc( p.db, p.aOp, nNew * sizeof( Op ) );
				//if (pNew != null)
				//{
				//      p.nOpAlloc = sqlite3DbMallocSize(p.db, pNew)/sizeof(Op);
				//  p.aOp = pNew;
				//}
				this.nOpAlloc=nNew;
				if(this.aOp==null)
					this.aOp=new VdbeOp[nNew];
				else
					Array.Resize(ref this.aOp,nNew);
				return (this.aOp!=null?Sqlite3.SQLITE_OK:SQLITE_NOMEM);
				//  return (pNew ? Sqlite3.SQLITE_OK : SQLITE_NOMEM);
			}
			public void sqlite3VdbeAddParseSchemaOp(int iDb,string zWhere) {
				int j;
				int addr=this.sqlite3VdbeAddOp3( OpCode.OP_ParseSchema,iDb,0,0);
				this.sqlite3VdbeChangeP4(addr,zWhere, P4Usage.P4_DYNAMIC);
				for(j=0;j<this.db.nDb;j++)
                    vdbeaux.sqlite3VdbeUsesBtree(this, j);
			}
			public void sqlite3VdbeLinkSubProgram(SubProgram p) {
				p.pNext=this.pProgram;
				this.pProgram=p;
			}
			public void sqlite3VdbeRewind() {
				#if (SQLITE_DEBUG) || (VDBE_PROFILE)
																																																																															      int i;
#endif
				Debug.Assert(this!=null);
				Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_INIT);
				///
				///<summary>
				///There should be at least one opcode.
				///
				///</summary>
				Debug.Assert(this.nOp>0);
				///
				///<summary>
				///Set the magic to VdbeMagic.VDBE_MAGIC_RUN sooner rather than later. 
				///</summary>
				this.magic=VdbeMagic.VDBE_MAGIC_RUN;
				#if SQLITE_DEBUG
																																																																															      for(i=1; i<p.nMem; i++){
        Debug.Assert( p.aMem[i].db==p.db );
      }
#endif
				this.currentOpCodeIndex=-1;
				this.rc=Sqlite3.SQLITE_OK;
				this.errorAction=OnConstraintError.OE_Abort;
				this.magic=VdbeMagic.VDBE_MAGIC_RUN;
				this.nChange=0;
				this.cacheCtr=1;
				this.minWriteFileFormat=255;
				this.iStatement=0;
				this.nFkConstraint=0;
				#if VDBE_PROFILE
																																																																															      for(i=0; i<p.nOp; i++){
        p.aOp[i].cnt = 0;
        p.aOp[i].cycles = 0;
      }
#endif
			}
			public void sqlite3VdbeSetNumCols(int nResColumn) {
				Mem pColName;
				int n;
				sqlite3 db=this.db;
                vdbeaux.releaseMemArray(this.aColName, this.nResColumn * COLNAME_N);
				db.sqlite3DbFree(ref this.aColName);
				n=nResColumn*COLNAME_N;
				this.nResColumn=(u16)nResColumn;
				this.aColName=new Mem[n];
				// (Mem)sqlite3DbMallocZero(db, Mem.Length * n);
				//if (p.aColName == 0) return;
				while(n-->0) {
					this.aColName[n]=malloc_cs.sqlite3Malloc(this.aColName[n]);
					pColName=this.aColName[n];
					pColName.flags=MemFlags.MEM_Null;
					pColName.db=this.db;
				}
			}
			public int sqlite3VdbeSetColName(///
			///<summary>
			///Vdbe being configured 
			///</summary>
			int idx,///
			///<summary>
			///Index of column zName applies to 
			///</summary>
			int var,///
			///<summary>
			///One of the COLNAME_* constants 
			///</summary>
			string zName,///
			///<summary>
			///Pointer to buffer containing name 
			///</summary>
			dxDel xDel///
			///<summary>
			///Memory management strategy for zName 
			///</summary>
			) {
				int rc;
				Mem pColName;
				Debug.Assert(idx<this.nResColumn);
				Debug.Assert(var<COLNAME_N);
				//if ( p.db.mallocFailed != 0 )
				//{
				//  Debug.Assert( null == zName || xDel != SQLITE_DYNAMIC );
				//  return SQLITE_NOMEM;
				//}
				Debug.Assert(this.aColName!=null);
				pColName=this.aColName[idx+var*this.nResColumn];
                rc = pColName.sqlite3VdbeMemSetStr(zName, -1, SqliteEncoding.UTF8, xDel);
				Debug.Assert(rc!=0||null==zName||(pColName.flags&MemFlags.MEM_Term)!=0);
				return rc;
			}
			public int sqlite3VdbeCloseStatement(int eOp) {
				sqlite3 db=this.db;
				int rc=Sqlite3.SQLITE_OK;
				///
				///<summary>
				///</summary>
				///<param name="If p">>iStatement is greater than zero, then this Vdbe opened a </param>
				///<param name="statement transaction that should be closed here. The only exception">statement transaction that should be closed here. The only exception</param>
				///<param name="is that an IO error may have occured, causing an emergency rollback.">is that an IO error may have occured, causing an emergency rollback.</param>
				///<param name="In this case (db">>nStatement==0), and there is nothing to do.</param>
				///<param name=""></param>
				if(db.nStatement!=0&&this.iStatement!=0) {
					int i;
					int iSavepoint=this.iStatement-1;
                    Debug.Assert(eOp == sqliteinth.SAVEPOINT_ROLLBACK || eOp == sqliteinth.SAVEPOINT_RELEASE);
					Debug.Assert(db.nStatement>0);
					Debug.Assert(this.iStatement==(db.nStatement+db.nSavepoint));
					for(i=0;i<db.nDb;i++) {
						int rc2=Sqlite3.SQLITE_OK;
						Btree pBt=db.aDb[i].pBt;
						if(pBt!=null) {
							if(eOp==sqliteinth.SAVEPOINT_ROLLBACK) {
								rc2=pBt.sqlite3BtreeSavepoint(sqliteinth.SAVEPOINT_ROLLBACK,iSavepoint);
							}
							if(rc2==Sqlite3.SQLITE_OK) {
                                rc2 = pBt.sqlite3BtreeSavepoint(sqliteinth.SAVEPOINT_RELEASE, iSavepoint);
							}
							if(rc==Sqlite3.SQLITE_OK) {
								rc=rc2;
							}
						}
					}
					db.nStatement--;
					this.iStatement=0;
					if(rc==Sqlite3.SQLITE_OK) {
						if(eOp==sqliteinth.SAVEPOINT_ROLLBACK) {
                            rc = vtab.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_ROLLBACK, iSavepoint);
						}
						if(rc==Sqlite3.SQLITE_OK) {
                            rc = vtab.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_RELEASE, iSavepoint);
						}
					}
					///
					///<summary>
					///If the statement transaction is being rolled back, also restore the 
					///database handles deferred constraint counter to the value it had when 
					///the statement transaction was opened.  
					///</summary>
					if(eOp==sqliteinth.SAVEPOINT_ROLLBACK) {
						db.nDeferredCons=this.nStmtDefCons;
					}
				}
				return rc;
			}
			public int sqlite3VdbeCheckFk(int deferred) {
				sqlite3 db=this.db;
				if((deferred!=0&&db.nDeferredCons>0)||(0==deferred&&this.nFkConstraint>0)) {
					this.rc=SQLITE_CONSTRAINT;
					this.errorAction=OnConstraintError.OE_Abort;
					malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"foreign key constraint failed");
					return Sqlite3.SQLITE_ERROR;
				}
				return Sqlite3.SQLITE_OK;
			}
			public int sqlite3VdbeHalt() {
				int rc;
				///
				///<summary>
				///Used to store transient return codes 
				///</summary>
				sqlite3 db=this.db;
				///
				///<summary>
				///This function contains the logic that determines if a statement or
				///transaction will be committed or rolled back as a result of the
				///execution of this virtual machine.
				///
				///If any of the following errors occur:
				///
				///SQLITE_NOMEM
				///SQLITE_IOERR
				///SQLITE_FULL
				///SQLITE_INTERRUPT
				///
				///Then the internal cache might have been left in an inconsistent
				///state.  We need to rollback the statement transaction, if there is
				///one, or the complete transaction if there is no statement transaction.
				///
				///</summary>
				//if ( p.db.mallocFailed != 0 )
				//{
				//  p.rc = SQLITE_NOMEM;
				//}
                vdbeaux.closeAllCursors(this);
				if(this.magic!=VdbeMagic.VDBE_MAGIC_RUN) {
					return Sqlite3.SQLITE_OK;
				}
                vdbeaux.checkActiveVdbeCnt(db);
				///
				///<summary>
				///No commit or rollback needed if the program never started 
				///</summary>
				if(this.currentOpCodeIndex>=0) {
					int mrc;
					///
					///<summary>
					///Primary error code from p.rc 
					///</summary>
					int eStatementOp=0;
					bool isSpecialError=false;
					///
					///<summary>
					///Set to true if a 'special' error 
					///</summary>
					///
					///<summary>
					///Lock all btrees used by the statement 
					///</summary>
					this.sqlite3VdbeEnter();
					///
					///<summary>
					///Check for one of the special errors 
					///</summary>
					mrc=this.rc&0xff;
					Debug.Assert(this.rc!=SQLITE_IOERR_BLOCKED);
					///
					///<summary>
					///This error no longer exists 
					///</summary>
					isSpecialError=mrc==SQLITE_NOMEM||mrc==SQLITE_IOERR||mrc==SQLITE_INTERRUPT||mrc==SQLITE_FULL;
					if(isSpecialError) {
						///
						///<summary>
						///</summary>
						///<param name="If the query was read">only and the error code is SQLITE_INTERRUPT, </param>
						///<param name="no rollback is necessary. Otherwise, at least a savepoint ">no rollback is necessary. Otherwise, at least a savepoint </param>
						///<param name="transaction must be rolled back to restore the database to a ">transaction must be rolled back to restore the database to a </param>
						///<param name="consistent state.">consistent state.</param>
						///<param name=""></param>
						///<param name="Even if the statement is read">only, it is important to perform</param>
						///<param name="a statement or transaction rollback operation. If the error ">a statement or transaction rollback operation. If the error </param>
						///<param name="occured while writing to the journal, sub">journal or database</param>
						///<param name="file as part of an effort to free up cache space (see function">file as part of an effort to free up cache space (see function</param>
						///<param name="pagerStress() in pager.c), the rollback is required to restore ">pagerStress() in pager.c), the rollback is required to restore </param>
						///<param name="the pager to a consistent state.">the pager to a consistent state.</param>
						///<param name=""></param>
						if(!this.readOnly||mrc!=SQLITE_INTERRUPT) {
							if((mrc==SQLITE_NOMEM||mrc==SQLITE_FULL)&&this.usesStmtJournal) {
								eStatementOp=sqliteinth.SAVEPOINT_ROLLBACK;
							}
							else {
								///
								///<summary>
								///We are forced to roll back the active transaction. Before doing
								///so, abort any other statements this handle currently has active.
								///
								///</summary>
                                vdbeaux.invalidateCursorsOnModifiedBtrees(db);
								sqlite3RollbackAll(db);
								sqlite3CloseSavepoints(db);
								db.autoCommit=1;
							}
						}
					}
					///
					///<summary>
					///Check for immediate foreign key violations. 
					///</summary>
					if(this.rc==Sqlite3.SQLITE_OK) {
						this.sqlite3VdbeCheckFk(0);
					}
					///
					///<summary>
					///</summary>
					///<param name="If the auto">commit flag is set and this is the only active writer</param>
					///<param name="VM, then we do either a commit or rollback of the current transaction.">VM, then we do either a commit or rollback of the current transaction.</param>
					///<param name=""></param>
					///<param name="Note: This block also runs if one of the special errors handled">Note: This block also runs if one of the special errors handled</param>
					///<param name="above has occurred.">above has occurred.</param>
					///<param name=""></param>
                    if (!sqliteinth.sqlite3VtabInSync(db) && db.autoCommit != 0 && db.writeVdbeCnt == ((this.readOnly == false) ? 1 : 0))
                    {
                        
						if(this.rc==Sqlite3.SQLITE_OK||(this.errorAction==OnConstraintError.OE_Fail&&!isSpecialError)) {
							rc=this.sqlite3VdbeCheckFk(1);
							if(rc!=Sqlite3.SQLITE_OK) {
								if(NEVER(this.readOnly)) {
									this.sqlite3VdbeLeave();
									return Sqlite3.SQLITE_ERROR;
								}
								rc=SQLITE_CONSTRAINT;
							}
							else {
								///
								///<summary>
								///</summary>
								///<param name="The auto">commit flag is true, the vdbe program was successful </param>
								///<param name="or hit an 'OR FAIL' constraint and there are no deferred foreign">or hit an 'OR FAIL' constraint and there are no deferred foreign</param>
								///<param name="key constraints to hold up the transaction. This means a commit ">key constraints to hold up the transaction. This means a commit </param>
								///<param name="is required. ">is required. </param>
								rc=vdbeaux.vdbeCommit(db,this);
							}
							if(rc==SQLITE_BUSY&&this.readOnly) {
								this.sqlite3VdbeLeave();
								return SQLITE_BUSY;
							}
							else
								if(rc!=Sqlite3.SQLITE_OK) {
									this.rc=rc;
									sqlite3RollbackAll(db);
								}
								else {
									db.nDeferredCons=0;
									build.sqlite3CommitInternalChanges(db);
								}
						}
						else {
							sqlite3RollbackAll(db);
						}
						db.nStatement=0;
					}
					else
						if(eStatementOp==0) {
							if(this.rc==Sqlite3.SQLITE_OK||this.errorAction==OnConstraintError.OE_Fail) {
                                eStatementOp = sqliteinth.SAVEPOINT_RELEASE;
							}
							else
								if(this.errorAction==OnConstraintError.OE_Abort) {
									eStatementOp=sqliteinth.SAVEPOINT_ROLLBACK;
								}
								else {
                                    vdbeaux.invalidateCursorsOnModifiedBtrees(db);
									sqlite3RollbackAll(db);
									sqlite3CloseSavepoints(db);
									db.autoCommit=1;
								}
						}
					///
					///<summary>
					///</summary>
					///<param name="If eStatementOp is non">zero, then a statement transaction needs to</param>
					///<param name="be committed or rolled back. Call sqlite3VdbeCloseStatement() to">be committed or rolled back. Call sqlite3VdbeCloseStatement() to</param>
					///<param name="do so. If this operation returns an error, and the current statement">do so. If this operation returns an error, and the current statement</param>
					///<param name="error code is Sqlite3.SQLITE_OK or SQLITE_CONSTRAINT, then promote the">error code is Sqlite3.SQLITE_OK or SQLITE_CONSTRAINT, then promote the</param>
					///<param name="current statement error code.">current statement error code.</param>
					///<param name=""></param>
					if(eStatementOp!=0) {
						rc=this.sqlite3VdbeCloseStatement(eStatementOp);
						if(rc!=0) {
							if(this.rc==Sqlite3.SQLITE_OK||this.rc==SQLITE_CONSTRAINT) {
								this.rc=rc;
								db.sqlite3DbFree(ref this.zErrMsg);
								this.zErrMsg=null;
							}
                            vdbeaux.invalidateCursorsOnModifiedBtrees(db);
							sqlite3RollbackAll(db);
							sqlite3CloseSavepoints(db);
							db.autoCommit=1;
						}
					}
					///
					///<summary>
					///If this was an INSERT, UPDATE or DELETE and no statement transaction
					///</summary>
					///<param name="has been rolled back, update the database connection change">counter.</param>
					///<param name=""></param>
					if(this.changeCntOn) {
						if(eStatementOp!=sqliteinth.SAVEPOINT_ROLLBACK) {
                            vdbeaux.sqlite3VdbeSetChanges(db, this.nChange);
						}
						else {
                            vdbeaux.sqlite3VdbeSetChanges(db, 0);
						}
						this.nChange=0;
					}
					///
					///<summary>
					///Rollback or commit any schema changes that occurred. 
					///</summary>
                    if (this.rc != Sqlite3.SQLITE_OK && (db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
                    {
						build.sqlite3ResetInternalSchema(db,-1);
                        db.flags = (db.flags | SqliteFlags.SQLITE_InternChanges);
					}
					///
					///<summary>
					///Release the locks 
					///</summary>
					this.sqlite3VdbeLeave();
				}
				///
				///<summary>
				///We have successfully halted and closed the VM.  Record this fact. 
				///</summary>
				if(this.currentOpCodeIndex>=0) {
					db.activeVdbeCnt--;
					if(!this.readOnly) {
						db.writeVdbeCnt--;
					}
					Debug.Assert(db.activeVdbeCnt>=db.writeVdbeCnt);
				}
				this.magic=VdbeMagic.VDBE_MAGIC_HALT;
                vdbeaux.checkActiveVdbeCnt(db);
				//if ( p.db.mallocFailed != 0 )
				//{
				//  p.rc = SQLITE_NOMEM;
				//}
				///
				///<summary>
				///</summary>
				///<param name="If the auto">commit flag is set to true, then any locks that were held</param>
				///<param name="by connection db have now been released. Call sqlite3ConnectionUnlocked()">by connection db have now been released. Call sqlite3ConnectionUnlocked()</param>
				///<param name="to invoke any required unlock">notify callbacks.</param>
				///<param name=""></param>
				if(db.autoCommit!=0) {
					sqliteinth.sqlite3ConnectionUnlocked(db);
				}
				Debug.Assert(db.activeVdbeCnt>0||db.autoCommit==0||db.nStatement==0);
				return (this.rc==SQLITE_BUSY?SQLITE_BUSY:Sqlite3.SQLITE_OK);
			}
			public void sqlite3VdbeResetStepResult() {
				this.rc=Sqlite3.SQLITE_OK;
			}
			public int sqlite3VdbeReset() {
				sqlite3 db;
				db=this.db;
				///
				///<summary>
				///If the VM did not run to completion or if it encountered an
				///error, then it might not have been halted properly.  So halt
				///it now.
				///
				///</summary>
				this.sqlite3VdbeHalt();
				///
				///<summary>
				///If the VDBE has be run even partially, then transfer the error code
				///and error message from the VDBE into the main database structure.  But
				///if the VDBE has just been set to run but has not actually executed any
				///instructions yet, leave the main database error information unchanged.
				///
				///</summary>
				if(this.currentOpCodeIndex>=0) {
					//if ( p.zErrMsg != 0 ) // Always exists under C#
					{
						sqlite3BeginBenignMalloc();
                        vdbemem_cs.sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg == null ? "" : this.zErrMsg, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
						sqlite3EndBenignMalloc();
						db.errCode=this.rc;
						db.sqlite3DbFree(ref this.zErrMsg);
						this.zErrMsg="";
						//else if ( p.rc != 0 )
						//{
						//  utilc.sqlite3Error( db, p.rc, 0 );
						//}
						//else
						//{
						//  utilc.sqlite3Error( db, Sqlite3.SQLITE_OK, 0 );
						//}
					}
					if(this.runOnlyOnce!=0)
						this.expired=true;
				}
				else
					if(this.rc!=0&&this.expired) {
						///
						///<summary>
						///The expired flag was set on the VDBE before the first call
						///to sqlite3_step(). For consistency (since sqlite3_step() was
						///called), set the database error in this case as well.
						///
						///</summary>
						utilc.sqlite3Error(db,this.rc,0);
                        vdbemem_cs.sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
						db.sqlite3DbFree(ref this.zErrMsg);
						this.zErrMsg="";
					}
				///
				///<summary>
				///Reclaim all memory used by the VDBE
				///
				///</summary>
                vdbeaux.Cleanup(this);
				///
				///<summary>
				///Save profiling information from this VDBE run.
				///
				///</summary>
				#if VDBE_PROFILE && TODO
																																																																															{
FILE *out = fopen("vdbe_profile.out", "a");
if( out ){
int i;
fprintf(out, "---- ");
for(i=0; i<p.nOp; i++){
fprintf(out, "%02x", p.aOp[i].opcode);
}
fprintf(out, "\n");
for(i=0; i<p.nOp; i++){
fprintf(out, "%6d %10lld %8lld ",
p.aOp[i].cnt,
p.aOp[i].cycles,
p.aOp[i].cnt>0 ? p.aOp[i].cycles/p.aOp[i].cnt : 0
);
sqlite3VdbePrintOp(out, i, p.aOp[i]);
}
fclose(out);
}
}
#endif
				this.magic=VdbeMagic.VDBE_MAGIC_INIT;
				return this.rc&db.errMask;
			}
			public sqlite3 sqlite3VdbeDb() {
				return this.db;
			}
			public sqlite3_value sqlite3VdbeGetValue(int iVar,u8 aff) {
				Debug.Assert(iVar>0);
				if(this!=null) {
					Mem pMem=this.aVar[iVar-1];
					if(0==(pMem.flags&MemFlags.MEM_Null)) {
						sqlite3_value pRet=vdbemem_cs.sqlite3ValueNew(this.db);
						if(pRet!=null) {
                            vdbemem_cs.sqlite3VdbeMemCopy((Mem)pRet, pMem);
							sqlite3ValueApplyAffinity(pRet,(char)aff,SqliteEncoding.UTF8);
							sqlite3VdbeMemStoreType((Mem)pRet);
						}
						return pRet;
					}
				}
				return null;
			}
			public void sqlite3VdbeSetVarmask(int iVar) {
				Debug.Assert(iVar>0);
				if(iVar>32) {
					this.expmask=0xffffffff;
				}
				else {
					this.expmask|=((u32)1<<(iVar-1));
				}
			}
			public void sqlite3VdbeCountChanges() {
				this.changeCntOn=true;
			}
			public int sqlite3VdbeExec(///
			///<summary>
			///The VDBE 
			///</summary>
			) {
				int opcodeIndex=0;
				///
				///<summary>
				///The program counter 
				///</summary>
				Op[] aOp=this.aOp;
				var lOp=this.lOp;
				Log.WriteHeader("Plan VdbeExec");
				Log.Indent();
				foreach(var item in lOp) {
					Log.WriteLine(item.ToString(this));
				}
				Log.WriteHeader("---");
				try {
					///
					///<summary>
					///Copy of p.aOp 
					///</summary>
					Op pOp;
					///
					///<summary>
					///Current operation 
					///</summary>
					int rc=Sqlite3.SQLITE_OK;
					///
					///<summary>
					///Value to return 
					///</summary>
					sqlite3 db=this.db;
					///
					///<summary>
					///The database 
					///</summary>
					u8 resetSchemaOnFault=0;
					///
					///<summary>
					///Reset schema after an error if positive 
					///</summary>
					SqliteEncoding encoding=sqliteinth.ENC(db);
					///
					///<summary>
					///The database encoding 
					///</summary>
					#if !SQLITE_OMIT_PROGRESS_CALLBACK
					bool checkProgress;
					///
					///<summary>
					///True if progress callbacks are enabled 
					///</summary>
					int nProgressOps=0;
					///
					///<summary>
					///Opcodes executed since progress callback. 
					///</summary>
					#endif
					Mem[] aMem=this.aMem;
					///
					///<summary>
					///Copy of p.aMem 
					///</summary>
					Mem pIn1=null;
					///
					///<summary>
					///1st input operand 
					///</summary>
					Mem pIn2=null;
					///
					///<summary>
					///2nd input operand 
					///</summary>
					Mem pIn3=null;
					///
					///<summary>
					///3rd input operand 
					///</summary>
					Mem pOut=null;
					///
					///<summary>
					///Output operand 
					///</summary>
					int iCompare=0;
					///
					///<summary>
					///Result of last  OpCode.OP_Compare operation 
					///</summary>
					int[] aPermute=null;
					///
					///<summary>
					///Permutation of columns for  OpCode.OP_Compare 
					///</summary>
					i64 lastRowid=db.lastRowid;
					///
					///<summary>
					///Saved value of the last insert ROWID 
					///</summary>
					#if VDBE_PROFILE
																																																																																		u64 start;                   /* CPU clock count at start of opcode */
int origPc;                  /* Program counter at start of opcode */
#endif
					///
					///<summary>
					///INSERT STACK UNION HERE **
					///</summary>
					Debug.Assert(this.magic==VdbeMagic.VDBE_MAGIC_RUN);
					///
					///<summary>
					///sqlite3_step() verifies this 
					///</summary>
					this.sqlite3VdbeEnter();
					if(this.rc==SQLITE_NOMEM) {
						///
						///<summary>
						///This happens if a malloc() inside a call to sqlite3_column_text() or
						///sqlite3_column_text16() failed.  
						///</summary>
						goto no_mem;
					}
					Debug.Assert(this.rc==Sqlite3.SQLITE_OK||this.rc==SQLITE_BUSY);
					this.rc=Sqlite3.SQLITE_OK;
					Debug.Assert(this.explain==0);
					this.pResultSet=null;
					db.busyHandler.nBusy=0;
					if(db.u1.isInterrupted)
						goto abort_due_to_interrupt;
					//CHECK_FOR_INTERRUPT;
					#if TRACE
																																																																																		sqlite3VdbeIOTraceSql( p );
#endif
					#if !SQLITE_OMIT_PROGRESS_CALLBACK
					checkProgress=db.xProgress!=null;
					#endif
					#if SQLITE_DEBUG
																																																																																		      sqlite3BeginBenignMalloc();
      if ( p.pc == 0
      && ( p.db.flags & SQLITE_VdbeListing ) != 0 )
      {
        int i;
        Console.Write( "VDBE Program Listing:\n" );
        sqlite3VdbePrintSql( p );
        for ( i = 0; i < p.nOp; i++ )
        {
          sqlite3VdbePrintOp( Console.Out, i, aOp[i] );
        }
      }
      sqlite3EndBenignMalloc();
#endif
					#region for
					for(opcodeIndex=this.currentOpCodeIndex;rc==Sqlite3.SQLITE_OK;opcodeIndex++) {
						Debug.Assert(opcodeIndex>=0&&opcodeIndex<this.nOp);
						//      if ( db.mallocFailed != 0 ) goto no_mem;
						#if VDBE_PROFILE
																																																																																																											origPc = pc;
start = sqlite3Hwtime();
#endif
						pOp=lOp[opcodeIndex];
						///
						///<summary>
						///Only allow tracing if SQLITE_DEBUG is defined.
						///
						///</summary>
						#if SQLITE_DEBUG
																																																																																																											        if ( p.trace != null )
        {
          if ( pc == 0 )
          {
            printf( "VDBE Execution Trace:\n" );
            sqlite3VdbePrintSql( p );
          }
          sqlite3VdbePrintOp( p.trace, pc, pOp );
        }
#endif
						///
						///<summary>
						///Check to see if we need to simulate an interrupt.  This only happens
						///if we have a special test build.
						///</summary>
						#if SQLITE_TEST
												#if !TCLSH
																																																																																																											        if ( sqlite3_interrupt_count > 0 )
        {
          sqlite3_interrupt_count--;
          if ( sqlite3_interrupt_count == 0 )
#else
																																																																																																											        if ( sqlite3_interrupt_count.iValue > 0 )
        {
          sqlite3_interrupt_count.iValue--;
          if ( sqlite3_interrupt_count.iValue == 0 )
#endif
																																																																																																											          {
            sqlite3_interrupt( db );
          }
        }
#endif
						#region check progress
						#if !SQLITE_OMIT_PROGRESS_CALLBACK
						///
						///<summary>
						///Call the progress callback if it is configured and the required number
						///of VDBE ops have been executed (either since this invocation of
						///sqlite3VdbeExec() or since last time the progress callback was called).
						///</summary>
						///<param name="If the progress callback returns non">zero, exit the virtual machine with</param>
						///<param name="a return code SQLITE_ABORT.">a return code SQLITE_ABORT.</param>
						if(checkProgress) {
							if(db.nProgressOps==nProgressOps) {
								int prc;
								prc=db.xProgress(db.pProgressArg);
								if(prc!=0) {
									rc=SQLITE_INTERRUPT;
									goto vdbe_error_halt;
								}
								nProgressOps=0;
							}
							nProgressOps++;
						}
						#endif
						#endregion
						///
						///<summary>
						///</summary>
						///<param name="On any opcode with the "out2">prerelase" tag, free any</param>
						///<param name="external allocations out of mem[p2] and set mem[p2] to be">external allocations out of mem[p2] and set mem[p2] to be</param>
						///<param name="an undefined integer.  Opcodes will either fill in the integer">an undefined integer.  Opcodes will either fill in the integer</param>
						///<param name="value or convert mem[p2] to a different type.">value or convert mem[p2] to a different type.</param>
						Debug.Assert(pOp.opflags==sqlite3OpcodeProperty[pOp.opcode]);
						if(((int)pOp.opflags&OPFLG_OUT2_PRERELEASE)!=0) {
							Debug.Assert(pOp.p2>0);
							Debug.Assert(pOp.p2<=this.nMem);
							pOut=aMem[pOp.p2];
							memAboutToChange(this,pOut);
							pOut.sqlite3VdbeMemReleaseExternal();
                            
							pOut.Flags=MemFlags.MEM_Int;
						}
						///
						///<summary>
						///Sanity checking on other operands 
						///</summary>
						///
						///<summary>
						///Sanity checking on other operands 
						///</summary>
						#if SQLITE_DEBUG
																																																																																																											        if ( ( pOp.opflags & OPFLG_IN1 ) != 0 )
        {
          Debug.Assert( pOp.p1 > 0 );
          Debug.Assert( pOp.p1 <= p.nMem );
          Debug.Assert( memIsValid( aMem[pOp.p1] ) );
          REGISTER_TRACE( p, pOp.p1, aMem[pOp.p1] );
        }
        if ( ( pOp.opflags & OPFLG_IN2 ) != 0 )
        {
          Debug.Assert( pOp.p2 > 0 );
          Debug.Assert( pOp.p2 <= p.nMem );
          Debug.Assert( memIsValid( aMem[pOp.p2] ) );
          REGISTER_TRACE( p, pOp.p2, aMem[pOp.p2] );
        }
        if ( ( pOp.opflags & OPFLG_IN3 ) != 0 )
        {
          Debug.Assert( pOp.p3 > 0 );
          Debug.Assert( pOp.p3 <= p.nMem );
          Debug.Assert( memIsValid( aMem[pOp.p3] ) );
          REGISTER_TRACE( p, pOp.p3, aMem[pOp.p3] );
        }
        if ( ( pOp.opflags & OPFLG_OUT2 ) != 0 )
        {
          Debug.Assert( pOp.p2 > 0 );
          Debug.Assert( pOp.p2 <= p.nMem );
          memAboutToChange( p, aMem[pOp.p2] );
        }
        if ( ( pOp.opflags & OPFLG_OUT3 ) != 0 )
        {
          Debug.Assert( pOp.p3 > 0 );
          Debug.Assert( pOp.p3 <= p.nMem );
          memAboutToChange( p, aMem[pOp.p3] );
        }
#endif
						Log.WriteLine(opcodeIndex.ToString().PadLeft(2)+":\t"+pOp.ToString(this));
						switch(pOp.OpCode) {
						///
						///<summary>
						///
						///What follows is a massive switch statement where each case implements a
						///separate instruction in the virtual machine.  If we follow the usual
						///indentation conventions, each case should be indented by 6 spaces.  But
						///that is a lot of wasted space on the left margin.  So the code within
						///</summary>
						///<param name="the switch statement will break with convention and be flush">left. Another</param>
						///<param name="big comment (similar to this one) will mark the point in the code where">big comment (similar to this one) will mark the point in the code where</param>
						///<param name="we transition back to normal indentation.">we transition back to normal indentation.</param>
						///<param name=""></param>
						///<param name="The formatting of each case is important.  The makefile for SQLite">The formatting of each case is important.  The makefile for SQLite</param>
						///<param name="generates two C files "opcodes.h" and "opcodes.c" by scanning this">generates two C files "opcodes.h" and "opcodes.c" by scanning this</param>
						///<param name="file looking for lines that begin with "case  OpCode.OP_".  The opcodes.h files">file looking for lines that begin with "case  OpCode.OP_".  The opcodes.h files</param>
						///<param name="will be filled with #defines that give unique integer values to each">will be filled with #defines that give unique integer values to each</param>
						///<param name="opcode and the opcodes.c file is filled with an array of strings where">opcode and the opcodes.c file is filled with an array of strings where</param>
						///<param name="each string is the symbolic name for the corresponding opcode.  If the">each string is the symbolic name for the corresponding opcode.  If the</param>
						///<param name="case statement is followed by a comment of the form "/# same as ... #/"">case statement is followed by a comment of the form "/# same as ... #/"</param>
						///<param name="that comment is used to determine the particular value of the opcode.">that comment is used to determine the particular value of the opcode.</param>
						///<param name=""></param>
						///<param name="Other keywords in the comment that follows each case are used to">Other keywords in the comment that follows each case are used to</param>
						///<param name="construct the OPFLG_INITIALIZER value that initializes opcodeProperty[].">construct the OPFLG_INITIALIZER value that initializes opcodeProperty[].</param>
						///<param name="Keywords include: in1, in2, in3, ref2_prerelease, ref2, ref3.  See">Keywords include: in1, in2, in3, ref2_prerelease, ref2, ref3.  See</param>
						///<param name="the mkopcodeh.awk script for additional information.">the mkopcodeh.awk script for additional information.</param>
						///<param name=""></param>
						///<param name="Documentation about VDBE opcodes is generated by scanning this file">Documentation about VDBE opcodes is generated by scanning this file</param>
						///<param name="for lines of that contain "Opcode:".  That line and all subsequent">for lines of that contain "Opcode:".  That line and all subsequent</param>
						///<param name="comment lines are used in the generation of the opcode.html documentation">comment lines are used in the generation of the opcode.html documentation</param>
						///<param name="file.">file.</param>
						///<param name=""></param>
						///<param name="SUMMARY:">SUMMARY:</param>
						///<param name=""></param>
						///<param name="Formatting is important to scripts that scan this file.">Formatting is important to scripts that scan this file.</param>
						///<param name="Do not deviate from the formatting style currently in use.">Do not deviate from the formatting style currently in use.</param>
						///<param name=""></param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode:  Goto * P2 * * *
						///
						///An unconditional jump to address P2.
						///The next instruction executed will be
						///the one at index P2 from the beginning of
						///the program.
						///</summary>
						case OpCode.OP_Goto: {
							///
							///<summary>
							///jump 
							///</summary>
							if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							opcodeIndex=pOp.p2-1;
							break;
						}
						///
						///<summary>
						///Opcode:  Gosub P1 P2 * * *
						///
						///Write the current address onto register P1
						///and then jump to address P2.
						///
						///</summary>
						case OpCode.OP_Gosub: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MemFlags.MEM_Dyn)==0);
							memAboutToChange(this,pIn1);
							pIn1.flags=MemFlags.MEM_Int;
							pIn1.u.i=opcodeIndex;
							REGISTER_TRACE(this,pOp.p1,pIn1);
							opcodeIndex=pOp.p2-1;
							break;
						}
						///
						///<summary>
						///Opcode:  Return P1 * * * *
						///
						///Jump to the next instruction after the address in register P1.
						///
						///</summary>
						case OpCode.OP_Return: {
                            OpCode_Return(ref opcodeIndex, pOp, aMem, ref pIn1);
							break;
						}
						///
						///<summary>
						///Opcode:  Yield P1 * * * *
						///
						///Swap the program counter with the value in register P1.
						///
						///</summary>
						case OpCode.OP_Yield: {
                            OpCode_Yield(ref opcodeIndex, pOp, aMem, ref pIn1);
							break;
						}
						///
						///<summary>
						///Opcode:  HaltIfNull  P1 P2 P3 P4 *
						///
						///Check the value in register P3.  If it is NULL then Halt using
						///parameter P1, P2, and P4 as if this were a Halt instruction.  If the
						///</summary>
						///<param name="value in register P3 is not NULL, then this routine is a no">op.</param>
						///<param name=""></param>
						case OpCode.OP_HaltIfNull: {
							///
							///<summary>
							///in3 
							///</summary>
							pIn3=aMem[pOp.p3];
							if((pIn3.flags&MemFlags.MEM_Null)==0)
								break;
							///
							///<summary>
							///Fall through into  OpCode.OP_Halt 
							///</summary>
							goto case OpCode.OP_Halt;
						}
						///
						///<summary>
						///Opcode:  Halt P1 P2 * P4 *
						///
						///Exit immediately.  All open cursors, etc are closed
						///automatically.
						///
						///P1 is the result code returned by sqlite3_exec(), sqlite3_reset(),
						///or sqlite3_finalize().  For a normal halt, this should be Sqlite3.SQLITE_OK (0).
						///For errors, it can be some other value.  If P1!=0 then P2 will determine
						///whether or not to rollback the current transaction.  Do not rollback
						///if P2==OnConstraintError.OE_Fail. Do the rollback if P2==OnConstraintError.OE_Rollback.  If P2==OnConstraintError.OE_Abort,
						///then back out all changes that have occurred during this execution of the
						///VDBE, but do not rollback the transaction.
						///
						///If P4 is not null then it is an error message string.
						///
						///There is an implied "Halt 0 0 0" instruction inserted at the very end of
						///every program.  So a jump past the last instruction of the program
						///is the same as executing Halt.
						///
						///</summary>
						case OpCode.OP_Halt: {
							pIn3=aMem[pOp.p3];
							if(pOp.p1==Sqlite3.SQLITE_OK&&this.pFrame!=null) {
								///
								///<summary>
								///</summary>
								///<param name="Halt the sub">program. Return control to the parent frame. </param>
								VdbeFrame pFrame=this.pFrame;
								this.pFrame=pFrame.pParent;
								this.nFrame--;
                                vdbeaux.sqlite3VdbeSetChanges(db, this.nChange);
								opcodeIndex=pFrame.sqlite3VdbeFrameRestore();
								lastRowid=db.lastRowid;
								if(pOp.p2==(int)OnConstraintError.OE_Ignore) {
									///
									///<summary>
									///</summary>
									///<param name="Instruction pc is the  OpCode.OP_Program that invoked the sub">program </param>
									///<param name="currently being halted. If the p2 instruction of this  OpCode.OP_Halt">currently being halted. If the p2 instruction of this  OpCode.OP_Halt</param>
									///<param name="instruction is set to OnConstraintError.OE_Ignore, then the sub">program is throwing</param>
									///<param name="an IGNORE exception. In this case jump to the address specified">an IGNORE exception. In this case jump to the address specified</param>
									///<param name="as the p2 of the calling  OpCode.OP_Program.  ">as the p2 of the calling  OpCode.OP_Program.  </param>
									opcodeIndex=this.lOp[opcodeIndex].p2-1;
								}
								lOp=this.lOp;
								aMem=this.aMem;
								break;
							}
							this.rc=pOp.p1;
							this.errorAction=(OnConstraintError)pOp.p2;
							this.currentOpCodeIndex=opcodeIndex;
							if(pOp.p4.z!=null) {
								Debug.Assert(this.rc!=Sqlite3.SQLITE_OK);
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"%s",pOp.p4.z);
								sqliteinth.testcase(Sqlite3.sqliteinth.sqlite3GlobalConfig.xLog!=null);
								io.sqlite3_log(pOp.p1,"abort at %d in [%s]: %s",opcodeIndex,this.zSql,pOp.p4.z);
							}
							else
								if(this.rc!=0) {
									sqliteinth.testcase(Sqlite3.sqliteinth.sqlite3GlobalConfig.xLog!=null);
									io.sqlite3_log(pOp.p1,"constraint failed at %d in [%s]",opcodeIndex,this.zSql);
								}
							rc=this.sqlite3VdbeHalt();
							Debug.Assert(rc==SQLITE_BUSY||rc==Sqlite3.SQLITE_OK||rc==Sqlite3.SQLITE_ERROR);
							if(rc==SQLITE_BUSY) {
								this.rc=rc=SQLITE_BUSY;
							}
							else {
								Debug.Assert(rc==Sqlite3.SQLITE_OK||this.rc==SQLITE_CONSTRAINT);
								Debug.Assert(rc==Sqlite3.SQLITE_OK||db.nDeferredCons>0);
								rc=this.rc!=0?Sqlite3.SQLITE_ERROR:SQLITE_DONE;
							}
							goto vdbe_return;
						}
						///
						///<summary>
						///Opcode: Integer P1 P2 * * *
						///
						///</summary>
						///<param name="The 32">bit integer value P1 is written into register P2.</param>
						///<param name=""></param>
						case OpCode.OP_Integer: {
                            OpCode_Integer(pOp, pOut);
							break;
						}
						///
						///<summary>
						///Opcode: Int64 * P2 * P4 *
						///
						///</summary>
						///<param name="P4 is a pointer to a 64">bit integer value.</param>
						///<param name="Write that value into register P2.">Write that value into register P2.</param>
						///<param name=""></param>
						case OpCode.OP_Int64: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							// Integer pointer always exists Debug.Assert( pOp.p4.pI64 != 0 );
							pOut.u.i=pOp.p4.pI64;
							break;
						}
						#if !SQLITE_OMIT_FLOATING_POINT
						///
						///<summary>
						///Opcode: Real * P2 * P4 *
						///
						///</summary>
						///<param name="P4 is a pointer to a 64">bit floating point value.</param>
						///<param name="Write that value into register P2.">Write that value into register P2.</param>
						case OpCode.OP_Real: {
							///
							///<summary>
							///</summary>
							///<param name="same as Sqlite3.TK_FLOAT, ref2">prerelease </param>
							pOut.flags=MemFlags.MEM_Real;
							Debug.Assert(!MathExtensions.sqlite3IsNaN(pOp.p4.pReal));
							pOut.r=pOp.p4.pReal;
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: String8 * P2 * P4 *
						///
						///</summary>
						///<param name="P4 points to a nul terminated UTF">8 string. This opcode is transformed</param>
						///<param name="into an  OpCode.OP_String before it is executed for the first time.">into an  OpCode.OP_String before it is executed for the first time.</param>
						case OpCode.OP_String8: {
							///
							///<summary>
							///</summary>
							///<param name="same as Sqlite3.TK_STRING, ref2">prerelease </param>
							Debug.Assert(pOp.p4.z!=null);
							pOp.OpCode=OpCode.OP_String;
							pOp.p1=StringExtensions.sqlite3Strlen30(pOp.p4.z);
							#if !SQLITE_OMIT_UTF16
																																																																																																																																				if( encoding!=SqliteEncoding.UTF8 ){
rc = sqlite3VdbeMemSetStr(pOut, pOp.p4.z, -1, SqliteEncoding.UTF8, SQLITE_STATIC);
if( rc==SQLITE_TOOBIG ) goto too_big;
if( Sqlite3.SQLITE_OK!=sqlite3VdbeChangeEncoding(pOut, encoding) ) goto no_mem;
Debug.Assert( pOut.zMalloc==pOut.z );
Debug.Assert( pOut.flags & MEM.MEM_Dyn );
pOut.zMalloc = 0;
pOut.flags |= MEM.MEM_Static;
pOut.flags &= ~MEM.MEM_Dyn;
if( pOp.p4type== P4Usage.P4_DYNAMIC ){
sqlite3DbFree(db, ref pOp.p4.z);
}
pOp.p4type =  P4Usage.P4_DYNAMIC;
pOp.p4.z = pOut.z;
pOp.p1 = pOut.n;
}
#endif
							if(pOp.p1>db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							///
							///<summary>
							///Fall through to the next case,  OpCode.OP_String 
							///</summary>
							goto case OpCode.OP_String;
						}
						///
						///<summary>
						///Opcode: String P1 P2 * P4 *
						///
						///The string value P4 of length P1 (bytes) is stored in register P2.
						///
						///</summary>
						case OpCode.OP_String: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							Debug.Assert(pOp.p4.z!=null);
							pOut.flags=MemFlags.MEM_Str|MemFlags.MEM_Static|MemFlags.MEM_Term;
							malloc_cs.sqlite3_free(ref pOut.zBLOB);
							pOut.z=pOp.p4.z;
							pOut.n=pOp.p1;
							#if SQLITE_OMIT_UTF16
							pOut.enc=SqliteEncoding.UTF8;
							#else
																																																																																																																																				              pOut.enc = encoding;
#endif
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Null * P2 * * *
						///
						///Write a NULL into register P2.
						///
						///</summary>
						case OpCode.OP_Null: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							pOut.flags=MemFlags.MEM_Null;
							break;
						}
						///
						///<summary>
						///Opcode: Blob P1 P2 * P4
						///
						///P4 points to a blob of data P1 bytes long.  Store this
						///blob in register P2.
						///
						///</summary>
						case OpCode.OP_Blob: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							Debug.Assert(pOp.p1<=db.aLimit[SQLITE_LIMIT_LENGTH]);
                            pOut.sqlite3VdbeMemSetStr(pOp.p4.z, pOp.p1, 0, null);
							pOut.enc=encoding;
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Variable P1 P2 * P4 *
						///
						///Transfer the values of bound parameter P1 into register P2
						///
						///If the parameter is named, then its name appears in P4 and P3==1.
						///The P4 value is used by sqlite3_bind_parameter_name().
						///
						///</summary>
						case OpCode.OP_Variable: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							Mem pVar;
							///
							///<summary>
							///Value being transferred 
							///</summary>
							Debug.Assert(pOp.p1>=0&&pOp.p1<=this.nVar);
							Debug.Assert(pOp.p4.z==null||pOp.p4.z==this.azVar[pOp.p1-1]);
							pVar=this.aVar[pOp.p1-1];
                            if (pVar.sqlite3VdbeMemTooBig())
                            {
								goto too_big;
							}
                            vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pVar, MemFlags.MEM_Static);
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Move P1 P2 P3 * *
						///
						///</summary>
						///<param name="Move the values in register P1..P1+P3">1 over into</param>
						///<param name="registers P2..P2+P3">1 are</param>
						///<param name="left holding a NULL.  It is an error for register ranges">left holding a NULL.  It is an error for register ranges</param>
						///<param name="P1..P1+P3">1 to overlap.</param>
						///<param name=""></param>
						case OpCode.OP_Move: {
                            OpCode_Move(pOp, aMem, ref pIn1, ref pOut);
							break;
						}
						///
						///<summary>
						///Opcode: Copy P1 P2 * * *
						///
						///Make a copy of register P1 into register P2.
						///
						///This instruction makes a deep copy of the value.  A duplicate
						///is made of any string or blob constant.  See also  OpCode.OP_SCopy.
						///
						///</summary>
						case OpCode.OP_Copy: {
							///
							///<summary>
							///in1, ref2 
							///</summary>
							pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							Debug.Assert(pOut!=pIn1);
							vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut,pIn1,MemFlags.MEM_Ephem);
							if((pOut.flags&MemFlags.MEM_Ephem)!=0&&pOut.sqlite3VdbeMemMakeWriteable()!=0) {
								goto no_mem;
							}
							//Deephemeralize( pOut );
							REGISTER_TRACE(this,pOp.p2,pOut);
							break;
						}
						///
						///<summary>
						///Opcode: SCopy P1 P2 * * *
						///
						///Make a shallow copy of register P1 into register P2.
						///
						///This instruction makes a shallow copy of the value.  If the value
						///is a string or blob, then the copy is only a pointer to the
						///original and hence if the original changes so will the copy.
						///Worse, if the original is deallocated, the copy becomes invalid.
						///Thus the program must guarantee that the original will not change
						///during the lifetime of the copy.  Use  OpCode.OP_Copy to make a complete
						///copy.
						///
						///</summary>
						case OpCode.OP_SCopy: {
                            OpCode_SCopy(pOp, aMem, ref pIn1, ref pOut);
							break;
						}
						///
						///<summary>
						///Opcode: ResultRow P1 P2 * * *
						///
						///</summary>
						///<param name="The registers P1 through P1+P2">1 contain a single row of</param>
						///<param name="results. This opcode causes the sqlite3_step() call to terminate">results. This opcode causes the sqlite3_step() call to terminate</param>
						///<param name="with an SQLITE_ROW return code and it sets up the sqlite3_stmt">with an SQLITE_ROW return code and it sets up the sqlite3_stmt</param>
						///<param name="structure to provide access to the top P1 values as the result">structure to provide access to the top P1 values as the result</param>
						///<param name="row.">row.</param>
						///<param name=""></param>
						case OpCode.OP_ResultRow: {
                            Debug.Assert(this.nResColumn == pOp.p2);
                            Debug.Assert(pOp.p1 > 0);
                            Debug.Assert(pOp.p1 + pOp.p2 <= this.nMem + 1);

                            rc = OpCode_ResultRow(opcodeIndex, pOp.p1, pOp.p2, rc, aMem);
							goto vdbe_return;
						}
						///
						///<summary>
						///Opcode: Concat P1 P2 P3 * *
						///
						///Add the text in register P1 onto the end of the text in
						///register P2 and store the result in register P3.
						///If either the P1 or P2 text are NULL then store NULL in P3.
						///
						///P3 = P2 || P1
						///
						///It is illegal for P1 and P3 to be the same register. Sometimes,
						///if P3 is the same register as P2, the implementation is able
						///to avoid a memcpy().
						///
						///</summary>
						case OpCode.OP_Concat: {
							///
							///<summary>
							///same as Sqlite3.TK_CONCAT, in1, in2, ref3 
							///</summary>
							i64 nByte;
							pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							pOut=aMem[pOp.p3];
							Debug.Assert(pIn1!=pOut);
							if(((pIn1.flags|pIn2.flags)&MemFlags.MEM_Null)!=0) {
                                pOut.sqlite3VdbeMemSetNull();
								break;
							}
							if(ExpandBlob(pIn1)!=0||ExpandBlob(pIn2)!=0)
								goto no_mem;
							if(((pIn1.flags&(MemFlags.MEM_Str|MemFlags.MEM_Blob))==0)&&vdbemem_cs.sqlite3VdbeMemStringify(pIn1,encoding)!=0) {
								goto no_mem;
							}
							// Stringify(pIn1, encoding);
							if(((pIn2.flags&(MemFlags.MEM_Str|MemFlags.MEM_Blob))==0)&&vdbemem_cs.sqlite3VdbeMemStringify(pIn2,encoding)!=0) {
								goto no_mem;
							}
							// Stringify(pIn2, encoding);
							nByte=pIn1.n+pIn2.n;
							if(nByte>db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							pOut.MemSetTypeFlag(MemFlags.MEM_Str);
							//if ( sqlite3VdbeMemGrow( pOut, (int)nByte + 2, ( pOut == pIn2 ) ? 1 : 0 ) != 0 )
							//{
							//  goto no_mem;
							//}
							//if ( pOut != pIn2 )
							//{
							//  memcpy( pOut.z, pIn2.z, pIn2.n );
							//}
							//memcpy( &pOut.z[pIn2.n], pIn1.z, pIn1.n );
							if(pIn2.z!=null&&pIn2.z.Length>=pIn2.n)
								if(pIn1.z!=null)
									pOut.z=pIn2.z.Substring(0,pIn2.n)+(pIn1.n<pIn1.z.Length?pIn1.z.Substring(0,pIn1.n):pIn1.z);
								else {
									if((pIn1.flags&MemFlags.MEM_Blob)==0)//String as Blob
									 {
										StringBuilder sb=new StringBuilder(pIn1.n);
										for(int i=0;i<pIn1.n;i++)
											sb.Append((byte)pIn1.zBLOB[i]);
										pOut.z=pIn2.z.Substring(0,pIn2.n)+sb.ToString();
									}
									else
										// UTF-8 Blob
										pOut.z=pIn2.z.Substring(0,pIn2.n)+Encoding.UTF8.GetString(pIn1.zBLOB,0,pIn1.zBLOB.Length);
								}
							else {
								pOut.zBLOB=malloc_cs.sqlite3Malloc(pIn1.n+pIn2.n);
								Buffer.BlockCopy(pIn2.zBLOB,0,pOut.zBLOB,0,pIn2.n);
								if(pIn1.zBLOB!=null)
									Buffer.BlockCopy(pIn1.zBLOB,0,pOut.zBLOB,pIn2.n,pIn1.n);
								else
									for(int i=0;i<pIn1.n;i++)
										pOut.zBLOB[pIn2.n+i]=(byte)pIn1.z[i];
							}
							//pOut.z[nByte] = 0;
							//pOut.z[nByte + 1] = 0;
							pOut.flags|=MemFlags.MEM_Term;
							pOut.n=(int)nByte;
							pOut.enc=encoding;
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Add P1 P2 P3 * *
						///
						///Add the value in register P1 to the value in register P2
						///and store the result in register P3.
						///If either input is NULL, the result is NULL.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Multiply P1 P2 P3 * *
						///
						///
						///Multiply the value in register P1 by the value in register P2
						///and store the result in register P3.
						///If either input is NULL, the result is NULL.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Subtract P1 P2 P3 * *
						///
						///Subtract the value in register P1 from the value in register P2
						///and store the result in register P3.
						///If either input is NULL, the result is NULL.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Divide P1 P2 P3 * *
						///
						///Divide the value in register P1 by the value in register P2
						///and store the result in register P3 (P3=P2/P1). If the value in 
						///register P1 is zero, then the result is NULL. If either input is 
						///NULL, the result is NULL.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Remainder P1 P2 P3 * *
						///
						///Compute the remainder after integer division of the value in
						///register P1 by the value in register P2 and store the result in P3.
						///If the value in register P2 is zero the result is NULL.
						///If either operand is NULL, the result is NULL.
						///
						///</summary>
						case OpCode.OP_Add:
						///
						///<summary>
						///same as Sqlite3.TK_PLUS, in1, in2, ref3 
						///</summary>
						case OpCode.OP_Subtract:
						///
						///<summary>
						///same as Sqlite3.TK_MINUS, in1, in2, ref3 
						///</summary>
						case OpCode.OP_Multiply:
						///
						///<summary>
						///same as Sqlite3.TK_STAR, in1, in2, ref3 
						///</summary>
						case OpCode.OP_Divide:
						///
						///<summary>
						///same as Sqlite3.TK_SLASH, in1, in2, ref3 
						///</summary>
						case OpCode.OP_Remainder: {
							///
							///<summary>
							///same as Sqlite3.TK_REM, in1, in2, ref3 
							///</summary>
							MemFlags flags;
							///
							///<summary>
							///Combined MEM.MEM_* flags from both inputs 
							///</summary>
							i64 iA;
							///
							///<summary>
							///Integer value of left operand 
							///</summary>
							i64 iB=0;
							///
							///<summary>
							///Integer value of right operand 
							///</summary>
							double rA;
							///
							///<summary>
							///Real value of left operand 
							///</summary>
							double rB;
							///
							///<summary>
							///Real value of right operand 
							///</summary>
							pIn1=aMem[pOp.p1];
							applyNumericAffinity(pIn1);
							pIn2=aMem[pOp.p2];
							applyNumericAffinity(pIn2);
							pOut=aMem[pOp.p3];
							flags=pIn1.flags|pIn2.flags;
							if((flags&MemFlags.MEM_Null)!=0)
								goto arithmetic_result_is_null;
							bool fp_math;
							if(!(fp_math=!((pIn1.Flags&pIn2.Flags&MemFlags.MEM_Int)==MemFlags.MEM_Int))) {
								iA=pIn1.u.i;
								iB=pIn2.u.i;
								switch(pOp.OpCode) {
								case OpCode.OP_Add: {
                                    if (utilc.sqlite3AddInt64(ref iB, iA) != 0)
										fp_math=true;
									// goto fp_math
									break;
								}
								case OpCode.OP_Subtract: {
									if(utilc.sqlite3SubInt64(ref iB,iA)!=0)
										fp_math=true;
									// goto fp_math
									break;
								}
								case OpCode.OP_Multiply: {
                                    if (utilc.sqlite3MulInt64(ref iB, iA) != 0)
										fp_math=true;
									// goto fp_math
									break;
								}
								case OpCode.OP_Divide: {
									if(iA==0)
										goto arithmetic_result_is_null;
									if(iA==-1&&iB==IntegerExtensions.SMALLEST_INT64) {
										fp_math=true;
										// goto fp_math
										break;
									}
									iB/=iA;
									break;
								}
								default: {
									if(iA==0)
										goto arithmetic_result_is_null;
									if(iA==-1)
										iA=1;
									iB%=iA;
									break;
								}
								}
							}
							if(!fp_math) {
								pOut.u.i=iB;
								pOut.MemSetTypeFlag(MemFlags.MEM_Int);
							}
							else {
								//fp_math:
								rA=pIn1.sqlite3VdbeRealValue();
                                rB = pIn2.sqlite3VdbeRealValue();
								switch(pOp.OpCode) {
								case OpCode.OP_Add:
								rB+=rA;
								break;
								case OpCode.OP_Subtract:
								rB-=rA;
								break;
								case OpCode.OP_Multiply:
								rB*=rA;
								break;
								case OpCode.OP_Divide: {
									///
									///<summary>
									///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
									///</summary>
									if(rA==(double)0)
										goto arithmetic_result_is_null;
									rB/=rA;
									break;
								}
								default: {
									iA=(i64)rA;
									iB=(i64)rB;
									if(iA==0)
										goto arithmetic_result_is_null;
									if(iA==-1)
										iA=1;
									rB=(double)(iB%iA);
									break;
								}
								}
								#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																																													pOut->u.i = rB;
MemSetTypeFlag(pOut, MEM.MEM_Int);
#else
								if(MathExtensions.sqlite3IsNaN(rB)) {
									goto arithmetic_result_is_null;
								}
								pOut.r=rB;
								pOut.MemSetTypeFlag(MemFlags.MEM_Real);
								if((flags&MemFlags.MEM_Real)==0) {
									pOut.sqlite3VdbeIntegerAffinity();
								}
								#endif
							}
							break;
							arithmetic_result_is_null:
							pOut.sqlite3VdbeMemSetNull();
							break;
						}
						///
						///<summary>
						///Opcode: CollSeq * * P4
						///
						///P4 is a pointer to a CollSeq struct. If the next call to a user function
						///or aggregate calls sqlite3GetFuncCollSeq(), this collation sequence will
						///</summary>
						///<param name="be returned. This is used by the built">in min(), max() and nullif()</param>
						///<param name="functions.">functions.</param>
						///<param name=""></param>
						///<param name="The interface used by the implementation of the aforementioned functions">The interface used by the implementation of the aforementioned functions</param>
						///<param name="to retrieve the collation sequence set by this opcode is not available">to retrieve the collation sequence set by this opcode is not available</param>
						///<param name="publicly, only to user functions defined in func.c.">publicly, only to user functions defined in func.c.</param>
						///<param name=""></param>
						case OpCode.OP_CollSeq: {
							Debug.Assert(pOp.p4type== P4Usage.P4_COLLSEQ);
							break;
						}
						///
						///<summary>
						///Opcode: Function P1 P2 P3 P4 P5
						///
						///Invoke a user function (P4 is a pointer to a Function structure that
						///defines the function) with P5 arguments taken from register P2 and
						///successors.  The result of the function is stored in register P3.
						///Register P3 must not be one of the function inputs.
						///
						///</summary>
						///<param name="P1 is a 32">bit bitmask indicating whether or not each argument to the</param>
						///<param name="function was determined to be constant at compile time. If the first">function was determined to be constant at compile time. If the first</param>
						///<param name="argument was constant then bit 0 of P1 is set. This is used to determine">argument was constant then bit 0 of P1 is set. This is used to determine</param>
						///<param name="whether meta data associated with a user function argument using the">whether meta data associated with a user function argument using the</param>
						///<param name="sqlite3_set_auxdata() API may be safely retained until the next">sqlite3_set_auxdata() API may be safely retained until the next</param>
						///<param name="invocation of this opcode.">invocation of this opcode.</param>
						///<param name=""></param>
						///<param name="See also: AggStep and AggFinal">See also: AggStep and AggFinal</param>
						///<param name=""></param>
						case OpCode.OP_Function: {
							int i;
							Mem pArg;
							sqlite3_context ctx=new sqlite3_context();
							sqlite3_value[] apVal;
							int n;
							n=pOp.p5;
							apVal=this.apArg;
							Debug.Assert(apVal!=null||n==0);
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pOut=aMem[pOp.p3];
							memAboutToChange(this,pOut);
							Debug.Assert(n==0||(pOp.p2>0&&pOp.p2+n<=this.nMem+1));
							Debug.Assert(pOp.p3<pOp.p2||pOp.p3>=pOp.p2+n);
							//pArg = aMem[pOp.p2];
							for(i=0;i<n;i++)//, pArg++)
							 {
								pArg=aMem[pOp.p2+i];
								Debug.Assert(pArg.memIsValid());
								apVal[i]=pArg;
								Deephemeralize(pArg);
								sqlite3VdbeMemStoreType(pArg);
								REGISTER_TRACE(this,pOp.p2+i,pArg);
							}
							Debug.Assert(pOp.p4type== P4Usage.P4_FUNCDEF||pOp.p4type== P4Usage.P4_VDBEFUNC);
							if(pOp.p4type== P4Usage.P4_FUNCDEF) {
								ctx.pFunc=pOp.p4.pFunc;
								ctx.pVdbeFunc=null;
							}
							else {
								ctx.pVdbeFunc=(VdbeFunc)pOp.p4.pVdbeFunc;
								ctx.pFunc=ctx.pVdbeFunc.pFunc;
							}
							ctx.s.flags=MemFlags.MEM_Null;
							ctx.s.db=db;
							ctx.s.xDel=null;
							//ctx.s.zMalloc = null;
							///
							///<summary>
							///The output cell may already have a buffer allocated. Move
							///</summary>
							///<param name="the pointer to ctx.s so in case the user">function can use</param>
							///<param name="the already allocated buffer instead of allocating a new one.">the already allocated buffer instead of allocating a new one.</param>
							///<param name=""></param>
                            vdbemem_cs.sqlite3VdbeMemMove(ctx.s, pOut);
							ctx.s.MemSetTypeFlag(MemFlags.MEM_Null);
							ctx.isError=0;
							if((ctx.pFunc.flags&FuncFlags.SQLITE_FUNC_NEEDCOLL)!=0) {
								Debug.Assert(opcodeIndex>1);
								//Debug.Assert(pOp > aOp);
								Debug.Assert(this.lOp[opcodeIndex-1].p4type== P4Usage.P4_COLLSEQ);
								//Debug.Assert(pOp[-1].p4type ==  P4Usage.P4_COLLSEQ);
                                Debug.Assert(this.lOp[opcodeIndex - 1].OpCode == OpCode.OP_CollSeq);
								//Debug.Assert(pOp[-1].opcode ==  OpCode.OP_CollSeq);
								ctx.pColl=this.lOp[opcodeIndex-1].p4.pColl;
								//ctx.pColl = pOp[-1].p4.pColl;
							}
							db.lastRowid=lastRowid;
							ctx.pFunc.xFunc(ctx,n,apVal);
							///* IMP: R-24505-23230 */
							lastRowid=db.lastRowid;
							///
							///<summary>
							///If any auxillary data functions have been called by this user function,
							///</summary>
							///<param name="immediately call the destructor for any non">static values.</param>
							///<param name=""></param>
							if(ctx.pVdbeFunc!=null) {
                                vdbeaux.sqlite3VdbeDeleteAuxData(ctx.pVdbeFunc, pOp.p1);
								pOp.p4.pVdbeFunc=ctx.pVdbeFunc;
								pOp.p4type= P4Usage.P4_VDBEFUNC;
							}
							//if ( db->mallocFailed )
							//{
							//  /* Even though a malloc() has failed, the implementation of the
							//  ** user function may have called an sqlite3_result_XXX() function
							//  ** to return a value. The following call releases any resources
							//  ** associated with such a value.
							//  */
							//   &u.ag.ctx.s .sqlite3VdbeMemRelease();
							//  goto no_mem;
							//}
							///
							///<summary>
							///If the function returned an error, throw an exception 
							///</summary>
							if(ctx.isError!=0) {
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,vdbeapi.sqlite3_value_text(ctx.s));
								rc=ctx.isError;
							}
							///
							///<summary>
							///Copy the result of the function into register P3 
							///</summary>
                            vdbemem_cs.sqlite3VdbeChangeEncoding(ctx.s, encoding);
                            vdbemem_cs.sqlite3VdbeMemMove(pOut, ctx.s);
                            if (pOut.sqlite3VdbeMemTooBig())
                            {
								goto too_big;
							}
							#if FALSE
																																																																																																																																				  /* The app-defined function has done something that as caused this
  ** statement to expire.  (Perhaps the function called sqlite3_exec()
  ** with a CREATE TABLE statement.)
  */
  if( p.expired ) rc = SQLITE_ABORT;
#endif
							REGISTER_TRACE(this,pOp.p3,pOut);
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: BitAnd P1 P2 P3 * *
						///
						///</summary>
						///<param name="Take the bit">wise AND of the values in register P1 and P2 and</param>
						///<param name="store the result in register P3.">store the result in register P3.</param>
						///<param name="If either input is NULL, the result is NULL.">If either input is NULL, the result is NULL.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: BitOr P1 P2 P3 * *
						///
						///</summary>
						///<param name="Take the bit">wise OR of the values in register P1 and P2 and</param>
						///<param name="store the result in register P3.">store the result in register P3.</param>
						///<param name="If either input is NULL, the result is NULL.">If either input is NULL, the result is NULL.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: ShiftLeft P1 P2 P3 * *
						///
						///Shift the integer value in register P2 to the left by the
						///number of bits specified by the integer in register P1.
						///Store the result in register P3.
						///If either input is NULL, the result is NULL.
						///
						///</summary>
						///
						///<summary>
						///Opcode: ShiftRight P1 P2 P3 * *
						///
						///Shift the integer value in register P2 to the right by the
						///number of bits specified by the integer in register P1.
						///Store the result in register P3.
						///If either input is NULL, the result is NULL.
						///
						///</summary>
						case OpCode.OP_BitAnd:
						///
						///<summary>
						///same as Sqlite3.TK_BITAND, in1, in2, ref3 
						///</summary>
						case OpCode.OP_BitOr:
						///
						///<summary>
						///same as Sqlite3.TK_BITOR, in1, in2, ref3 
						///</summary>
						case OpCode.OP_ShiftLeft:
						///
						///<summary>
						///same as Sqlite3.TK_LSHIFT, in1, in2, ref3 
						///</summary>
						case OpCode.OP_ShiftRight: {
							///
							///<summary>
							///same as Sqlite3.TK_RSHIFT, in1, in2, ref3 
							///</summary>
							i64 iA;
							u64 uA;
							i64 iB;
							OpCode op;
							pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							pOut=aMem[pOp.p3];
							if(((pIn1.flags|pIn2.flags)&MemFlags.MEM_Null)!=0) {
								pOut.sqlite3VdbeMemSetNull();
								break;
							}
							iA=pIn2.sqlite3VdbeIntValue();
							iB=pIn1.sqlite3VdbeIntValue();
							op=pOp.OpCode;
                            if (op == OpCode.OP_BitAnd)
                            {
								iA&=iB;
							}
							else
                                if (op == OpCode.OP_BitOr)
                                {
									iA|=iB;
								}
								else
									if(iB!=0) {
										Debug.Assert(op== OpCode.OP_ShiftRight||op== OpCode.OP_ShiftLeft);
										///
										///<summary>
										///If shifting by a negative amount, shift in the other direction 
										///</summary>
										if(iB<0) {
											Debug.Assert( OpCode.OP_ShiftRight== OpCode.OP_ShiftLeft+1);
											op=(OpCode)(2*(u8)OpCode.OP_ShiftLeft+1-op);
											iB=iB>(-64)?-iB:64;
										}
										if(iB>=64) {
											iA=(iA>=0||op== OpCode.OP_ShiftLeft)?0:-1;
										}
										else {
											//uA = (ulong)(iA << 0); // memcpy( &uA, &iA, sizeof( uA ) );
											if(op== OpCode.OP_ShiftLeft) {
												iA=iA<<(int)iB;
											}
											else {
												iA=iA>>(int)iB;
												///
												///<summary>
												///</summary>
												///<param name="Sign">extend on a right shift of a negative number </param>
												//if ( iA < 0 )
												//  uA |= ( ( (0xffffffff ) << (u8)32 ) | 0xffffffff ) << (u8)( 64 - iB );
											}
											//iA = (long)( uA << 0 ); //memcpy( &iA, &uA, sizeof( iA ) );
										}
									}
							pOut.u.i=iA;
							pOut.MemSetTypeFlag(MemFlags.MEM_Int);
							break;
						}
						///
						///<summary>
						///Opcode: AddImm  P1 P2 * * *
						///
						///Add the constant P2 to the value in register P1.
						///The result is always an integer.
						///
						///To force any register to be an integer, just add 0.
						///
						///</summary>
						case OpCode.OP_AddImm: {
							///
							///<summary>
							///in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							pIn1.sqlite3VdbeMemIntegerify();
							pIn1.u.i+=pOp.p2;
							break;
						}
						///
						///<summary>
						///Opcode: MustBeInt P1 P2 * * *
						///
						///Force the value in register P1 to be an integer.  If the value
						///in P1 is not an integer and cannot be converted into an integer
						///without data loss, then jump immediately to P2, or if P2==0
						///raise an SQLITE_MISMATCH exception.
						///
						///</summary>
						case OpCode.OP_MustBeInt: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
                            applyAffinity(pIn1, sqliteinth.SQLITE_AFF_NUMERIC, encoding);
							if((pIn1.flags&MemFlags.MEM_Int)==0) {
								if(pOp.p2==0) {
									rc=SQLITE_MISMATCH;
									goto abort_due_to_error;
								}
								else {
									opcodeIndex=pOp.p2-1;
								}
							}
							else {
								pIn1.MemSetTypeFlag(MemFlags.MEM_Int);
							}
							break;
						}
						#if !SQLITE_OMIT_FLOATING_POINT
						///
						///<summary>
						///Opcode: RealAffinity P1 * * * *
						///
						///If register P1 holds an integer convert it to a real value.
						///
						///This opcode is used when extracting information from a column that
						///has REAL affinity.  Such column values may still be stored as
						///integers, for space efficiency, but after extraction we want them
						///to have only a real value.
						///</summary>
						case OpCode.OP_RealAffinity: {
							///
							///<summary>
							///in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Int)!=0) {
                                pIn1.sqlite3VdbeMemRealify();
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_CAST
						///
						///<summary>
						///Opcode: ToText P1 * * * *
						///
						///Force the value in register P1 to be text.
						///If the value is numeric, convert it to a string using the
						///equivalent of printf().  Blob values are unchanged and
						///are afterwards simply interpreted as text.
						///
						///A NULL value is not changed by this routine.  It remains NULL.
						///</summary>
						case OpCode.OP_ToText: {
							///
							///<summary>
							///same as Sqlite3.TK_TO_TEXT, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							if((pIn1.flags&MemFlags.MEM_Null)!=0)
								break;
                            Debug.Assert(MemFlags.MEM_Str == (MemFlags)((int)MemFlags.MEM_Blob >> 3));
							pIn1.flags|=((pIn1.flags&(MemFlags)((int)MemFlags.MEM_Blob>>3)));
							applyAffinity(pIn1,sqliteinth.SQLITE_AFF_TEXT,encoding);
							rc=ExpandBlob(pIn1);
							Debug.Assert((pIn1.flags&MemFlags.MEM_Str)!=0///
							///<summary>
							///|| db.mallocFailed != 0 
							///</summary>
							);
							pIn1.flags=(pIn1.flags&~(MemFlags.MEM_Int|MemFlags.MEM_Real|MemFlags.MEM_Blob|MemFlags.MEM_Zero));
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: ToBlob P1 * * * *
						///
						///Force the value in register P1 to be a BLOB.
						///If the value is numeric, convert it to a string first.
						///Strings are simply reinterpreted as blobs with no change
						///to the underlying data.
						///
						///A NULL value is not changed by this routine.  It remains NULL.
						///
						///</summary>
						case OpCode.OP_ToBlob: {
							///
							///<summary>
							///same as Sqlite3.TK_TO_BLOB, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Null)!=0)
								break;
							if((pIn1.flags&MemFlags.MEM_Blob)==0) {
								applyAffinity(pIn1,sqliteinth.SQLITE_AFF_TEXT,encoding);
								Debug.Assert((pIn1.flags&MemFlags.MEM_Str)!=0///
								///<summary>
								///|| db.mallocFailed != 0 
								///</summary>
								);
								pIn1.MemSetTypeFlag(MemFlags.MEM_Blob);
							}
							else {
								pIn1.flags=(pIn1.flags&~(MemFlags.MEM_TypeMask&~MemFlags.MEM_Blob));
							}
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: ToNumeric P1 * * * *
						///
						///Force the value in register P1 to be numeric (either an
						///</summary>
						///<param name="integer or a floating">point number.)</param>
						///<param name="If the value is text or blob, try to convert it to an using the">If the value is text or blob, try to convert it to an using the</param>
						///<param name="equivalent of _Custom.atoi() or atof() and store 0 if no such conversion">equivalent of _Custom.atoi() or atof() and store 0 if no such conversion</param>
						///<param name="is possible.">is possible.</param>
						///<param name=""></param>
						///<param name="A NULL value is not changed by this routine.  It remains NULL.">A NULL value is not changed by this routine.  It remains NULL.</param>
						///<param name=""></param>
						case OpCode.OP_ToNumeric: {
							///
							///<summary>
							///same as Sqlite3.TK_TO_NUMERIC, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							pIn1.sqlite3VdbeMemNumerify();
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: ToInt P1 * * * *
						///
						///Force the value in register P1 to be an integer.  If
						///The value is currently a real number, drop its fractional part.
						///If the value is text or blob, try to convert it to an integer using the
						///equivalent of _Custom.atoi() and store 0 if no such conversion is possible.
						///
						///A NULL value is not changed by this routine.  It remains NULL.
						///</summary>
						case OpCode.OP_ToInt: {
							///
							///<summary>
							///same as Sqlite3.TK_TO_INT, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Null)==0) {
								pIn1.sqlite3VdbeMemIntegerify();
							}
							break;
						}
						#if !(SQLITE_OMIT_CAST) && !(SQLITE_OMIT_FLOATING_POINT)
						///
						///<summary>
						///Opcode: ToReal P1 * * * *
						///
						///Force the value in register P1 to be a floating point number.
						///If The value is currently an integer, convert it.
						///If the value is text or blob, try to convert it to an integer using the
						///equivalent of _Custom.atoi() and store 0.0 if no such conversion is possible.
						///
						///A NULL value is not changed by this routine.  It remains NULL.
						///</summary>
						case OpCode.OP_ToReal: {
							///
							///<summary>
							///same as Sqlite3.TK_TO_REAL, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							if((pIn1.flags&MemFlags.MEM_Null)==0) {
                                pIn1.sqlite3VdbeMemRealify();
							}
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: Lt P1 P2 P3 P4 P5
						///
						///Compare the values in register P1 and P3.  If reg(P3)<reg(P1) then
						///jump to address P2.
						///
						///If the sqliteinth.SQLITE_JUMPIFNULL bit of P5 is set and either reg(P1) or
						///reg(P3) is NULL then take the jump.  If the sqliteinth.SQLITE_JUMPIFNULL
						///bit is clear then fall through if either operand is NULL.
						///
						///</summary>
						///<param name="The SQLITE_AFF_MASK portion of P5 must be an affinity character "></param>
						///<param name="sqliteinth.SQLITE_AFF_TEXT, SQLITE_AFF_INTEGER, and so forth. An attempt is made">sqliteinth.SQLITE_AFF_TEXT, SQLITE_AFF_INTEGER, and so forth. An attempt is made</param>
						///<param name="to coerce both inputs according to this affinity before the">to coerce both inputs according to this affinity before the</param>
						///<param name="comparison is made. If the SQLITE_AFF_MASK is 0x00, then numeric">comparison is made. If the SQLITE_AFF_MASK is 0x00, then numeric</param>
						///<param name="affinity is used. Note that the affinity conversions are stored">affinity is used. Note that the affinity conversions are stored</param>
						///<param name="back into the input registers P1 and P3.  So this opcode can cause">back into the input registers P1 and P3.  So this opcode can cause</param>
						///<param name="persistent changes to registers P1 and P3.">persistent changes to registers P1 and P3.</param>
						///<param name=""></param>
						///<param name="Once any conversions have taken place, and neither value is NULL,">Once any conversions have taken place, and neither value is NULL,</param>
						///<param name="the values are compared. If both values are blobs then memcmp() is">the values are compared. If both values are blobs then memcmp() is</param>
						///<param name="used to determine the results of the comparison.  If both values">used to determine the results of the comparison.  If both values</param>
						///<param name="are text, then the appropriate collating function specified in">are text, then the appropriate collating function specified in</param>
						///<param name="P4 is  used to do the comparison.  If P4 is not specified then">P4 is  used to do the comparison.  If P4 is not specified then</param>
						///<param name="memcmp() is used to compare text string.  If both values are">memcmp() is used to compare text string.  If both values are</param>
						///<param name="numeric, then a numeric comparison is used. If the two values">numeric, then a numeric comparison is used. If the two values</param>
						///<param name="are of different types, then numbers are considered less than">are of different types, then numbers are considered less than</param>
						///<param name="strings and strings are considered less than blobs.">strings and strings are considered less than blobs.</param>
						///<param name=""></param>
						///<param name="If the SQLITE_STOREP2 bit of P5 is set, then do not jump.  Instead,">If the SQLITE_STOREP2 bit of P5 is set, then do not jump.  Instead,</param>
						///<param name="store a boolean result (either 0, or 1, or NULL) in register P2.">store a boolean result (either 0, or 1, or NULL) in register P2.</param>
						///
						///<summary>
						///Opcode: Ne P1 P2 P3 P4 P5
						///
						///This works just like the Lt opcode except that the jump is taken if
						///the operands in registers P1 and P3 are not equal.  See the Lt opcode for
						///additional information.
						///
						///If sqliteinth.SQLITE_NULLEQ is set in P5 then the result of comparison is always either
						///true or false and is never NULL.  If both operands are NULL then the result
						///of comparison is false.  If either operand is NULL then the result is true.
						///If neither operand is NULL the result is the same as it would be if
						///the sqliteinth.SQLITE_NULLEQ flag were omitted from P5.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Eq P1 P2 P3 P4 P5
						///
						///This works just like the Lt opcode except that the jump is taken if
						///the operands in registers P1 and P3 are equal.
						///See the Lt opcode for additional information.
						///
						///If sqliteinth.SQLITE_NULLEQ is set in P5 then the result of comparison is always either
						///true or false and is never NULL.  If both operands are NULL then the result
						///of comparison is true.  If either operand is NULL then the result is false.
						///If neither operand is NULL the result is the same as it would be if
						///the sqliteinth.SQLITE_NULLEQ flag were omitted from P5.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Le P1 P2 P3 P4 P5
						///
						///This works just like the Lt opcode except that the jump is taken if
						///the content of register P3 is less than or equal to the content of
						///register P1.  See the Lt opcode for additional information.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Gt P1 P2 P3 P4 P5
						///
						///This works just like the Lt opcode except that the jump is taken if
						///the content of register P3 is greater than the content of
						///register P1.  See the Lt opcode for additional information.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Ge P1 P2 P3 P4 P5
						///
						///This works just like the Lt opcode except that the jump is taken if
						///the content of register P3 is greater than or equal to the content of
						///register P1.  See the Lt opcode for additional information.
						///
						///</summary>
						case OpCode.OP_Eq:
						///
						///<summary>
						///same as Sqlite3.TK_EQ, jump, in1, in3 
						///</summary>
						case OpCode.OP_Ne:
						///
						///<summary>
						///same as Sqlite3.TK_NE, jump, in1, in3 
						///</summary>
						case OpCode.OP_Lt:
						///
						///<summary>
						///same as Sqlite3.TK_LT, jump, in1, in3 
						///</summary>
						case OpCode.OP_Le:
						///
						///<summary>
						///same as Sqlite3.TK_LE, jump, in1, in3 
						///</summary>
						case OpCode.OP_Gt:
						///
						///<summary>
						///same as Sqlite3.TK_GT, jump, in1, in3 
						///</summary>
						case OpCode.OP_Ge: {
							///
							///<summary>
							///same as Sqlite3.TK_GE, jump, in1, in3 
							///</summary>
							int res=0;
							///
							///<summary>
							///Result of the comparison of pIn1 against pIn3 
							///</summary>
							char affinity;
							///
							///<summary>
							///Affinity to use for comparison 
							///</summary>
							MemFlags flags1;
							///
							///<summary>
							///</summary>
							///<param name="Copy of initial value of pIn1">>flags </param>
							MemFlags flags3;
							///
							///<summary>
							///</summary>
							///<param name="Copy of initial value of pIn3">>flags </param>
							pIn1=aMem[pOp.p1];
							pIn3=aMem[pOp.p3];
							flags1=pIn1.flags;
							flags3=pIn3.flags;
							if(((pIn1.flags|pIn3.flags)&MemFlags.MEM_Null)!=0) {
								///
								///<summary>
								///One or both operands are NULL 
								///</summary>
								if((pOp.p5&sqliteinth.SQLITE_NULLEQ)!=0) {
									///
									///<summary>
									///If sqliteinth.SQLITE_NULLEQ is set (which will only happen if the operator is
									///OP_Eq or  OpCode.OP_Ne) then take the jump or not depending on whether
									///or not both operands are null.
									///
									///</summary>
									Debug.Assert(pOp.OpCode== OpCode.OP_Eq||pOp.OpCode== OpCode.OP_Ne);
									res=(pIn1.flags&pIn3.flags&MemFlags.MEM_Null)==0?1:0;
								}
								else {
									///
									///<summary>
									///sqliteinth.SQLITE_NULLEQ is clear and at least one operand is NULL,
									///then the result is always NULL.
									///The jump is taken if the sqliteinth.SQLITE_JUMPIFNULL bit is set.
									///
									///</summary>
                                    if ((pOp.p5 & sqliteinth.SQLITE_STOREP2) != 0)
                                    {
										pOut=aMem[pOp.p2];
										pOut.MemSetTypeFlag(MemFlags.MEM_Null);
										REGISTER_TRACE(this,pOp.p2,pOut);
									}
									else
										if((pOp.p5&sqliteinth.SQLITE_JUMPIFNULL)!=0) {
											opcodeIndex=pOp.p2-1;
										}
									break;
								}
							}
							else {
								///
								///<summary>
								///Neither operand is NULL.  Do a comparison. 
								///</summary>
                                affinity = (char)(pOp.p5 & sqliteinth.SQLITE_AFF_MASK);
								if(affinity!='\0') {
									applyAffinity(pIn1,affinity,encoding);
									applyAffinity(pIn3,affinity,encoding);
									//      if ( db.mallocFailed != 0 ) goto no_mem;
								}
								Debug.Assert(pOp.p4type== P4Usage.P4_COLLSEQ||pOp.p4.pColl==null);
								ExpandBlob(pIn1);
								ExpandBlob(pIn3);
								res=vdbemem_cs.sqlite3MemCompare(pIn3,pIn1,pOp.p4.pColl);
							}
							switch(pOp.OpCode) {
							case OpCode.OP_Eq:
							res=(res==0)?1:0;
							break;
							case OpCode.OP_Ne:
							res=(res!=0)?1:0;
							break;
							case OpCode.OP_Lt:
							res=(res<0)?1:0;
							break;
							case OpCode.OP_Le:
							res=(res<=0)?1:0;
							break;
							case OpCode.OP_Gt:
							res=(res>0)?1:0;
							break;
							default:
							res=(res>=0)?1:0;
							break;
							}
                            if ((pOp.p5 & sqliteinth.SQLITE_STOREP2) != 0)
                            {
								pOut=aMem[pOp.p2];
								memAboutToChange(this,pOut);
								pOut.MemSetTypeFlag(MemFlags.MEM_Int);
								pOut.u.i=res;
								REGISTER_TRACE(this,pOp.p2,pOut);
							}
							else
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
								}
							///
							///<summary>
							///Undo any changes made by applyAffinity() to the input registers. 
							///</summary>
							pIn1.flags=((pIn1.flags&~MemFlags.MEM_TypeMask)|(flags1&MemFlags.MEM_TypeMask));
							pIn3.flags=((pIn3.flags&~MemFlags.MEM_TypeMask)|(flags3&MemFlags.MEM_TypeMask));
							break;
						}
						///
						///<summary>
						///Opcode: Permutation * * * P4 *
						///
						///Set the permutation used by the  OpCode.OP_Compare operator to be the array
						///of integers in P4.
						///
						///The permutation is only valid until the next  OpCode.OP_Permutation,  OpCode.OP_Compare,
						///OP_Halt, or OpCode.OP_ResultRow.  Typically the  OpCode.OP_Permutation should occur
						///immediately prior to the  OpCode.OP_Compare.
						///
						///</summary>
						case OpCode.OP_Permutation: {
							Debug.Assert(pOp.p4type== P4Usage.P4_INTARRAY);
							Debug.Assert(pOp.p4.ai!=null);
							aPermute=pOp.p4.ai;
							break;
						}
						///
						///<summary>
						///Opcode: Compare P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="Compare two vectors of registers in reg(P1)..reg(P1+P3">1) (call this</param>
						///<param name="vector "A") and in reg(P2)..reg(P2+P3">1) ("B").  Save the result of</param>
						///<param name="the comparison for use by the next  OpCode.OP_Jump instruct.">the comparison for use by the next  OpCode.OP_Jump instruct.</param>
						///<param name=""></param>
						///<param name="P4 is a KeyInfo structure that defines collating sequences and sort">P4 is a KeyInfo structure that defines collating sequences and sort</param>
						///<param name="orders for the comparison.  The permutation applies to registers">orders for the comparison.  The permutation applies to registers</param>
						///<param name="only.  The KeyInfo elements are used sequentially.">only.  The KeyInfo elements are used sequentially.</param>
						///<param name=""></param>
						///<param name="The comparison is a sort comparison, so NULLs compare equal,">The comparison is a sort comparison, so NULLs compare equal,</param>
						///<param name="NULLs are less than numbers, numbers are less than strings,">NULLs are less than numbers, numbers are less than strings,</param>
						///<param name="and strings are less than blobs.">and strings are less than blobs.</param>
						///<param name=""></param>
						case OpCode.OP_Compare: {
                            OCode_Compare(pOp, aMem, ref iCompare, ref aPermute);
							break;
						}
						///
						///<summary>
						///Opcode: Jump P1 P2 P3 * *
						///
						///Jump to the instruction at address P1, P2, or P3 depending on whether
						///in the most recent  OpCode.OP_Compare instruction the P1 vector was less than
						///equal to, or greater than the P2 vector, respectively.
						///
						///</summary>
						case OpCode.OP_Jump: {
							///
							///<summary>
							///jump 
							///</summary>
							if(iCompare<0) {
								opcodeIndex=pOp.p1-1;
							}
							else
								if(iCompare==0) {
									opcodeIndex=pOp.p2-1;
								}
								else {
									opcodeIndex=pOp.p3-1;
								}
							break;
						}
						///
						///<summary>
						///Opcode: And P1 P2 P3 * *
						///
						///Take the logical AND of the values in registers P1 and P2 and
						///write the result into register P3.
						///
						///If either P1 or P2 is 0 (false) then the result is 0 even if
						///the other input is NULL.  A NULL and true or two NULLs give
						///a NULL output.
						///
						///</summary>
						///
						///<summary>
						///Opcode: Or P1 P2 P3 * *
						///
						///Take the logical OR of the values in register P1 and P2 and
						///store the answer in register P3.
						///
						///If either P1 or P2 is nonzero (true) then the result is 1 (true)
						///even if the other input is NULL.  A NULL and false or two NULLs
						///give a NULL output.
						///
						///</summary>
						case OpCode.OP_And:
						///
						///<summary>
						///same as Sqlite3.TK_AND, in1, in2, ref3 
						///</summary>
						case OpCode.OP_Or: {
                            OpCode_AndOr(pOp, aMem, ref pIn1, ref pIn2, ref pOut);
							break;
						}
						///
						///<summary>
						///Opcode: Not P1 P2 * * *
						///
						///Interpret the value in register P1 as a boolean value.  Store the
						///boolean complement in register P2.  If the value in register P1 is
						///NULL, then a NULL is stored in P2.
						///
						///</summary>
						case OpCode.OP_Not: {
							///
							///<summary>
							///same as Sqlite3.TK_NOT, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							if((pIn1.flags&MemFlags.MEM_Null)!=0) {
								pOut.sqlite3VdbeMemSetNull();
							}
							else {
                                pOut.sqlite3VdbeMemSetInt64(pIn1.sqlite3VdbeIntValue() == 0 ? 1 : 0);
							}
							break;
						}
						///
						///<summary>
						///Opcode: BitNot P1 P2 * * *
						///
						///Interpret the content of register P1 as an integer.  Store the
						///</summary>
						///<param name="ones">complement of the P1 value into register P2.  If P1 holds</param>
						///<param name="a NULL then store a NULL in P2.">a NULL then store a NULL in P2.</param>
						///<param name=""></param>
						case OpCode.OP_BitNot: {
							///
							///<summary>
							///same as Sqlite3.TK_BITNOT, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							if((pIn1.flags&MemFlags.MEM_Null)!=0) {
								pOut.sqlite3VdbeMemSetNull();
							}
							else {
                                pOut.sqlite3VdbeMemSetInt64(~pIn1.sqlite3VdbeIntValue());
							}
							break;
						}
						///
						///<summary>
						///Opcode: If P1 P2 P3 * *
						///
						///Jump to P2 if the value in register P1 is true.  The value
						///</summary>
						///<param name="is considered true if it is numeric and non">zero.  If the value</param>
						///<param name="in P1 is NULL then take the jump if P3 is true.">in P1 is NULL then take the jump if P3 is true.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: IfNot P1 P2 P3 * *
						///
						///Jump to P2 if the value in register P1 is False.  The value
						///is considered true if it has a numeric value of zero.  If the value
						///in P1 is NULL then take the jump if P3 is true.
						///
						///</summary>
						case OpCode.OP_If:
						///
						///<summary>
						///jump, in1 
						///</summary>
						case OpCode.OP_IfNot: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							int c;
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Null)!=0) {
								c=pOp.p3;
							}
							else {
								#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																																													c = pIn1.sqlite3VdbeIntValue()!=0;
#else
                                c = (pIn1.sqlite3VdbeRealValue() != 0.0) ? 1 : 0;
								#endif
								if(pOp.OpCode==OpCode.OP_IfNot)
									c=(c==0)?1:0;
							}
							if(c!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: IsNull P1 P2 * * *
						///
						///Jump to P2 if the value in register P1 is NULL.
						///
						///</summary>
						case OpCode.OP_IsNull: {
							///
							///<summary>
							///same as Sqlite3.TK_ISNULL, jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Null)!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: NotNull P1 P2 * * *
						///
						///Jump to P2 if the value in register P1 is not NULL.
						///
						///</summary>
						case OpCode.OP_NotNull: {
							///
							///<summary>
							///same as Sqlite3.TK_NOTNULL, jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MemFlags.MEM_Null)==0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Column P1 P2 P3 P4 *
						///
						///Interpret the data that cursor P1 points to as a structure built using
						///the MakeRecord instruction.  (See the MakeRecord opcode for additional
						///</summary>
						///<param name="information about the format of the data.)  Extract the P2">th column</param>
						///<param name="from this record.  If there are less that (P2+1)">from this record.  If there are less that (P2+1)</param>
						///<param name="values in the record, extract a NULL.">values in the record, extract a NULL.</param>
						///<param name=""></param>
						///<param name="The value extracted is stored in register P3.">The value extracted is stored in register P3.</param>
						///<param name=""></param>
						///<param name="If the column contains fewer than P2 fields, then extract a NULL.  Or,">If the column contains fewer than P2 fields, then extract a NULL.  Or,</param>
						///<param name="if the P4 argument is a  P4Usage.P4_MEM use the value of the P4 argument as">if the P4 argument is a  P4Usage.P4_MEM use the value of the P4 argument as</param>
						///<param name="the result.">the result.</param>
						///<param name=""></param>
						///<param name="If the OPFLAG_CLEARCACHE bit is set on P5 and P1 is a pseudo">table cursor,</param>
						///<param name="then the cache of the cursor is reset prior to extracting the column.">then the cache of the cursor is reset prior to extracting the column.</param>
						///<param name="The first  OpCode.OP_Column against a pseudo">table after the value of the content</param>
						///<param name="register has changed should have this bit set.">register has changed should have this bit set.</param>
						///<param name=""></param>
						case OpCode.OP_Column: {
                            rc = OpCode_Column(pOp, rc, db, encoding, aMem);
                            switch(rc){}
                            break;
						}
						///
						///<summary>
						///Opcode: Affinity P1 P2 * P4 *
						///
						///Apply affinities to a range of P2 registers starting with P1.
						///
						///P4 is a string that is P2 characters long. The nth character of the
						///string indicates the column affinity that should be used for the nth
						///memory cell in the range.
						///
						///</summary>
						case OpCode.OP_Affinity: {
							string zAffinity;
							///
							///<summary>
							///The affinity to be applied 
							///</summary>
							char cAff;
							///
							///<summary>
							///A single character of affinity 
							///</summary>
							zAffinity=pOp.p4.z;
							Debug.Assert(!String.IsNullOrEmpty(zAffinity));
							Debug.Assert(zAffinity.Length<=pOp.p2);
							//zAffinity[pOp.p2] == 0
							//pIn1 = aMem[pOp.p1];
							for(int zI=0;zI<zAffinity.Length;zI++)// while( (cAff = *(zAffinity++))!=0 ){
							 {
								cAff=zAffinity[zI];
								pIn1=aMem[pOp.p1+zI];
								//Debug.Assert( pIn1 <= p->aMem[p->nMem] );
								Debug.Assert(pIn1.memIsValid());
								ExpandBlob(pIn1);
								applyAffinity(pIn1,cAff,encoding);
								//pIn1++;
							}
							break;
						}
						///
						///<summary>
						///Opcode: MakeRecord P1 P2 P3 P4 *
						///
						///Convert P2 registers beginning with P1 into the [record format]
						///use as a data record in a database table or as a key
						///in an index.  The  OpCode.OP_Column opcode can decode the record later.
						///
						///P4 may be a string that is P2 characters long.  The nth character of the
						///string indicates the column affinity that should be used for the nth
						///field of the index key.
						///
						///The mapping from character to affinity is given by the SQLITE_AFF_
						///macros defined in sqliteInt.h.
						///
						///If P4 is NULL then all index fields have the affinity NONE.
						///
						///</summary>
						case OpCode.OP_MakeRecord: {
							///A buffer to hold the data for the new record 
							byte[] zNewRecord;
							Mem pRec;
							///
							///<summary>
							///The new record 
							///</summary>
							u64 nData;
							///
							///<summary>
							///Number of bytes of data space 
							///</summary>
							int nHdr;
							///
							///<summary>
							///Number of bytes of header space 
							///</summary>
							i64 nByte;
							///
							///<summary>
							///Data space required for this record 
							///</summary>
							int nZero;
							///
							///<summary>
							///Number of zero bytes at the end of the record 
							///</summary>
							int nVarint;
							///
							///<summary>
							///Number of bytes in a varint 
							///</summary>
							u32 serial_type;
							///
							///<summary>
							///Type field 
							///</summary>
							//Mem pData0;            /* First field to be combined into the record */
							//Mem pLast;             /* Last field of the record */
							int nField;
							///
							///<summary>
							///Number of fields in the record 
							///</summary>
							string zAffinity;
							///
							///<summary>
							///The affinity string for the record 
							///</summary>
							int file_format;
							///
							///<summary>
							///File format to use for encoding 
							///</summary>
							int i;
							///
							///<summary>
							///Space used in zNewRecord[] 
							///</summary>
							int len;
							///
							///<summary>
							///Length of a field 
							///</summary>
							///
							///<summary>
							///Assuming the record contains N fields, the record format looks
							///like this:
							///
							///</summary>
							///<param name=""></param>
							///<param name="| hdr">1 |</param>
							///<param name=""></param>
							///<param name=""></param>
							///<param name="Data(0) is taken from register P1.  Data(1) comes from register P1+1">Data(0) is taken from register P1.  Data(1) comes from register P1+1</param>
							///<param name="and so froth.">and so froth.</param>
							///<param name=""></param>
							///<param name="Each type field is a varint representing the serial type of the">Each type field is a varint representing the serial type of the</param>
							///<param name="corresponding data element (see sqlite3VdbeSerialType()). The">corresponding data element (see sqlite3VdbeSerialType()). The</param>
							///<param name="hdr">size field is also a varint which is the offset from the beginning</param>
							///<param name="of the record to data0.">of the record to data0.</param>
							nData=0;
							///
							///<summary>
							///Number of bytes of data space 
							///</summary>
							nHdr=0;
							///
							///<summary>
							///Number of bytes of header space 
							///</summary>
							nZero=0;
							///
							///<summary>
							///Number of zero bytes at the end of the record 
							///</summary>
							nField=pOp.p1;
							zAffinity=(pOp.p4.z==null||pOp.p4.z.Length==0)?"":pOp.p4.z;
							Debug.Assert(nField>0&&pOp.p2>0&&pOp.p2+nField<=this.nMem+1);
							//pData0 = aMem[nField];
							nField=pOp.p2;
							//pLast =  pData0[nField - 1];
							file_format=this.minWriteFileFormat;
							///
							///<summary>
							///Identify the output register 
							///</summary>
							Debug.Assert(pOp.p3<pOp.p1||pOp.p3>=pOp.p1+pOp.p2);
							pOut=aMem[pOp.p3];
							memAboutToChange(this,pOut);
							///
							///<summary>
							///Loop through the elements that will make up the record to figure
							///out how much space is required for the new record.
							///
							///</summary>
							//for (pRec = pData0; pRec <= pLast; pRec++)
							for(int pD0=0;pD0<nField;pD0++) {
								pRec=this.aMem[pOp.p1+pD0];
								Debug.Assert(pRec.memIsValid());
								if(pD0<zAffinity.Length&&zAffinity[pD0]!='\0') {
									applyAffinity(pRec,(char)zAffinity[pD0],encoding);
								}
								if((pRec.flags&MemFlags.MEM_Zero)!=0&&pRec.n>0) {
									pRec.sqlite3VdbeMemExpandBlob();
								}
                                serial_type = vdbeaux.sqlite3VdbeSerialType(pRec, file_format);
                                len = (int)vdbeaux.sqlite3VdbeSerialTypeLen(serial_type);
								nData+=(u64)len;
								nHdr+=utilc.sqlite3VarintLen(serial_type);
								if((pRec.flags&MemFlags.MEM_Zero)!=0) {
									///
									///<summary>
									///</summary>
									///<param name="Only pure zero">filled BLOBs can be input to this Opcode.</param>
									///<param name="We do not allow blobs with a prefix and a zero">filled tail. </param>
									nZero+=pRec.u.nZero;
								}
								else
									if(len!=0) {
										nZero=0;
									}
							}
							///
							///<summary>
							///Add the initial header varint and total the size 
							///</summary>
							nHdr+=nVarint=utilc.sqlite3VarintLen((u64)nHdr);
							if(nVarint<utilc.sqlite3VarintLen((u64)nHdr)) {
								nHdr++;
							}
							nByte=(i64)((u64)nHdr+nData-(u64)nZero);
							if(nByte>db.aLimit[SQLITE_LIMIT_LENGTH]) {
                                return (int)ColumnResult.too_big;
							}
							///
							///<summary>
							///Make sure the output register has a buffer large enough to store
							///the new record. The output register (pOp.p3) is not allowed to
							///be one of the input registers (because the following call to
							///sqlite3VdbeMemGrow() could clobber the value before it is used).
							///
							///</summary>
							//if ( sqlite3VdbeMemGrow( pOut, (int)nByte, 0 ) != 0 )
							//{
							//  goto no_mem;
							//}
							zNewRecord=malloc_cs.sqlite3Malloc((int)nByte);
							// (u8 )pOut.z;
							///
							///<summary>
							///Write the record 
							///</summary>
							i=utilc.putVarint32(zNewRecord,nHdr);
							for(int pD0=0;pD0<nField;pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
							 {
								pRec=this.aMem[pOp.p1+pD0];
                                serial_type = vdbeaux.sqlite3VdbeSerialType(pRec, file_format);
                                i += utilc.putVarint32(zNewRecord, i, (int)serial_type);
								///
								///<summary>
								///serial type 
								///</summary>
							}
							for(int pD0=0;pD0<nField;pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
							 {
								///
								///<summary>
								///serial data 
								///</summary>
								pRec=this.aMem[pOp.p1+pD0];
                                i += (int)vdbeaux.sqlite3VdbeSerialPut(zNewRecord, i, (int)nByte - i, pRec, file_format);
							}
							//TODO -- Remove this  for testing Debug.Assert( i == nByte );
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pOut.zBLOB=zNewRecord;
							pOut.z=null;
							pOut.n=(int)nByte;
							pOut.flags=MemFlags.MEM_Blob|MemFlags.MEM_Dyn;
							pOut.xDel=null;
							if(nZero!=0) {
								pOut.u.nZero=nZero;
								pOut.flags|=MemFlags.MEM_Zero;
							}
							pOut.enc=SqliteEncoding.UTF8;
							///
							///<summary>
							///In case the blob is ever converted to text 
							///</summary>
							REGISTER_TRACE(this,pOp.p3,pOut);
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Count P1 P2 * * *
						///
						///Store the number of entries (an integer value) in the table or index
						///opened by cursor P1 in register P2
						///
						///</summary>
						#if !SQLITE_OMIT_BTREECOUNT
						case OpCode.OP_Count: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							i64 nEntry=0;
							BtCursor pCrsr;
							pCrsr=this.apCsr[pOp.p1].pCursor;
							if(pCrsr!=null) {
								rc=pCrsr.sqlite3BtreeCount(ref nEntry);
							}
							else {
								nEntry=0;
							}
							pOut.u.i=nEntry;
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: Savepoint P1 * * P4 *
						///
						///Open, release or rollback the savepoint named by parameter P4, depending
						///on the value of P1. To open a new savepoint, P1==0. To release (commit) an
						///existing savepoint, P1==1, or to rollback an existing savepoint P1==2.
						///</summary>
						case OpCode.OP_Savepoint: {
							int p1;
							///
							///<summary>
							///Value of P1 operand 
							///</summary>
							string zName;
							///
							///<summary>
							///Name of savepoint 
							///</summary>
							int nName;
							Savepoint pNew;
							Savepoint pSavepoint;
							Savepoint pTmp;
							int iSavepoint;
							int ii;
							p1=pOp.p1;
							zName=pOp.p4.z;
							///
							///<summary>
							///Assert that the p1 parameter is valid. Also that if there is no open
							///transaction, then there cannot be any savepoints.
							///
							///</summary>
							Debug.Assert(db.pSavepoint==null||db.autoCommit==0);
                            Debug.Assert(p1 == sqliteinth.SAVEPOINT_BEGIN || p1 == sqliteinth.SAVEPOINT_RELEASE || p1 == sqliteinth.SAVEPOINT_ROLLBACK);
							Debug.Assert(db.pSavepoint!=null||db.isTransactionSavepoint==0);
							Debug.Assert(checkSavepointCount(db)!=0);
                            if (p1 == sqliteinth.SAVEPOINT_BEGIN)
                            {
								if(db.writeVdbeCnt>0) {
									///
									///<summary>
									///A new savepoint cannot be created if there are active write
									///statements (i.e. open read/write incremental blob handles).
									///
									///</summary>
									malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"cannot open savepoint - ","SQL statements in progress");
									rc=SQLITE_BUSY;
								}
								else {
									nName=StringExtensions.sqlite3Strlen30(zName);
									#if !SQLITE_OMIT_VIRTUALTABLE
									///
									///<summary>
									///This call is Ok even if this savepoint is actually a transaction
									///savepoint (and therefore should not prompt xSavepoint()) callbacks.
									///If this is a transaction savepoint being opened, it is guaranteed
									///</summary>
									///<param name="that the db">>aVTrans[] array is empty.  </param>
									Debug.Assert(db.autoCommit==0||db.nVTrans==0);
                                    rc = vtab.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_BEGIN, db.nStatement + db.nSavepoint);
									if(rc!=Sqlite3.SQLITE_OK)
										goto abort_due_to_error;
									#endif
									///
									///<summary>
									///Create a new savepoint structure. 
									///</summary>
									pNew=new Savepoint();
									// sqlite3DbMallocRaw( db, sizeof( Savepoint ) + nName + 1 );
									if(pNew!=null) {
										//pNew.zName = (char )&pNew[1];
										//memcpy(pNew.zName, zName, nName+1);
										pNew.zName=zName;
										///
										///<summary>
										///If there is no open transaction, then mark this as a special
										///"transaction savepoint". 
										///</summary>
										if(db.autoCommit!=0) {
											db.autoCommit=0;
											db.isTransactionSavepoint=1;
										}
										else {
											db.nSavepoint++;
										}
										///
										///<summary>
										///Link the new savepoint into the database handle's list. 
										///</summary>
										pNew.pNext=db.pSavepoint;
										db.pSavepoint=pNew;
										pNew.nDeferredCons=db.nDeferredCons;
									}
								}
							}
							else {
								iSavepoint=0;
								///
								///<summary>
								///Find the named savepoint. If there is no such savepoint, then an
								///an error is returned to the user.  
								///</summary>
								for(pSavepoint=db.pSavepoint;pSavepoint!=null&&!pSavepoint.zName.Equals(zName,StringComparison.InvariantCultureIgnoreCase);pSavepoint=pSavepoint.pNext) {
									iSavepoint++;
								}
								if(null==pSavepoint) {
									malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"no such savepoint: %s",zName);
									rc=Sqlite3.SQLITE_ERROR;
								}
								else
									if(db.writeVdbeCnt>0||(p1==sqliteinth.SAVEPOINT_ROLLBACK&&db.activeVdbeCnt>1)) {
										///
										///<summary>
										///It is not possible to release (commit) a savepoint if there are
										///active write statements. It is not possible to rollback a savepoint
										///if there are any active statements at all.
										///
										///</summary>
										malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"cannot %s savepoint - SQL statements in progress",(p1==sqliteinth.SAVEPOINT_ROLLBACK?"rollback":"release"));
										rc=SQLITE_BUSY;
									}
									else {
										///
										///<summary>
										///Determine whether or not this is a transaction savepoint. If so,
										///and this is a RELEASE command, then the current transaction
										///is committed.
										///
										///</summary>
										int isTransaction=(pSavepoint.pNext==null&&db.isTransactionSavepoint!=0)?1:0;
                                        if (isTransaction != 0 && p1 == sqliteinth.SAVEPOINT_RELEASE)
                                        {
											if((rc=this.sqlite3VdbeCheckFk(1))!=Sqlite3.SQLITE_OK) {
												goto vdbe_return;
											}
											db.autoCommit=1;
											if(this.sqlite3VdbeHalt()==SQLITE_BUSY) {
												this.currentOpCodeIndex=opcodeIndex;
												db.autoCommit=0;
												this.rc=rc=SQLITE_BUSY;
												goto vdbe_return;
											}
											db.isTransactionSavepoint=0;
											rc=this.rc;
										}
										else {
											iSavepoint=db.nSavepoint-iSavepoint-1;
											for(ii=0;ii<db.nDb;ii++) {
												rc=db.aDb[ii].pBt.sqlite3BtreeSavepoint(p1,iSavepoint);
												if(rc!=Sqlite3.SQLITE_OK) {
													goto abort_due_to_error;
												}
											}
                                            if (p1 == sqliteinth.SAVEPOINT_ROLLBACK && (db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
                                            {
                                                vdbeaux.sqlite3ExpirePreparedStatements(db);
												build.sqlite3ResetInternalSchema(db,-1);
                                                db.flags = (db.flags | SqliteFlags.SQLITE_InternChanges);
											}
										}
										///
										///<summary>
										///Regardless of whether this is a RELEASE or ROLLBACK, destroy all
										///savepoints nested inside of the savepoint being operated on. 
										///</summary>
										while(db.pSavepoint!=pSavepoint) {
											pTmp=db.pSavepoint;
											db.pSavepoint=pTmp.pNext;
											db.sqlite3DbFree(ref pTmp);
											db.nSavepoint--;
										}
										///
										///<summary>
										///If it is a RELEASE, then destroy the savepoint being operated on 
										///too. If it is a ROLLBACK TO, then set the number of deferred 
										///constraint violations present in the database to the value stored
										///when the savepoint was created.  
										///</summary>
                                        if (p1 == sqliteinth.SAVEPOINT_RELEASE)
                                        {
											Debug.Assert(pSavepoint==db.pSavepoint);
											db.pSavepoint=pSavepoint.pNext;
											db.sqlite3DbFree(ref pSavepoint);
											if(0==isTransaction) {
												db.nSavepoint--;
											}
										}
										else {
											db.nDeferredCons=pSavepoint.nDeferredCons;
										}
										if(0==isTransaction) {
                                            rc = vtab.sqlite3VtabSavepoint(db, p1, iSavepoint);
											if(rc!=Sqlite3.SQLITE_OK)
												goto abort_due_to_error;
										}
									}
							}
							break;
						}
						///
						///<summary>
						///Opcode: AutoCommit P1 P2 * * *
						///
						///</summary>
						///<param name="Set the database auto">commit flag to P1 (1 or 0). If P2 is true, roll</param>
						///<param name="back any currently active btree transactions. If there are any active">back any currently active btree transactions. If there are any active</param>
						///<param name="VMs (apart from this one), then the COMMIT or ROLLBACK statement fails.">VMs (apart from this one), then the COMMIT or ROLLBACK statement fails.</param>
						///<param name=""></param>
						///<param name="This instruction causes the VM to halt.">This instruction causes the VM to halt.</param>
						///<param name=""></param>
						case OpCode.OP_AutoCommit: {
							int desiredAutoCommit;
							int iRollback;
							int turnOnAC;
							desiredAutoCommit=(u8)pOp.p1;
							iRollback=pOp.p2;
							turnOnAC=(desiredAutoCommit!=0&&0==db.autoCommit)?1:0;
							Debug.Assert(desiredAutoCommit!=0||0==desiredAutoCommit);
							Debug.Assert(desiredAutoCommit!=0||0==iRollback);
							Debug.Assert(db.activeVdbeCnt>0);
							///
							///<summary>
							///At least this one VM is active 
							///</summary>
							if(turnOnAC!=0&&iRollback!=0&&db.activeVdbeCnt>1) {
								///
								///<summary>
								///If this instruction implements a ROLLBACK and other VMs are
								///still running, and a transaction is active, return an error indicating
								///that the other VMs must complete first.
								///
								///</summary>
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"cannot rollback transaction - "+"SQL statements in progress");
								rc=SQLITE_BUSY;
							}
							else
								if(turnOnAC!=0&&0==iRollback&&db.writeVdbeCnt>0) {
									///
									///<summary>
									///If this instruction implements a COMMIT and other VMs are writing
									///return an error indicating that the other VMs must complete first.
									///
									///</summary>
									malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"cannot commit transaction - "+"SQL statements in progress");
									rc=SQLITE_BUSY;
								}
								else
									if(desiredAutoCommit!=db.autoCommit) {
										if(iRollback!=0) {
											Debug.Assert(desiredAutoCommit!=0);
											sqlite3RollbackAll(db);
											db.autoCommit=1;
										}
										else
											if((rc=this.sqlite3VdbeCheckFk(1))!=Sqlite3.SQLITE_OK) {
												goto vdbe_return;
											}
											else {
												db.autoCommit=(u8)desiredAutoCommit;
												if(this.sqlite3VdbeHalt()==SQLITE_BUSY) {
													this.currentOpCodeIndex=opcodeIndex;
													db.autoCommit=(u8)(desiredAutoCommit==0?1:0);
													this.rc=rc=SQLITE_BUSY;
													goto vdbe_return;
												}
											}
										Debug.Assert(db.nStatement==0);
										sqlite3CloseSavepoints(db);
										if(this.rc==Sqlite3.SQLITE_OK) {
											rc=SQLITE_DONE;
										}
										else {
											rc=Sqlite3.SQLITE_ERROR;
										}
										goto vdbe_return;
									}
									else {
										malloc_cs.sqlite3SetString(ref this.zErrMsg,db,(0==desiredAutoCommit)?"cannot start a transaction within a transaction":((iRollback!=0)?"cannot rollback - no transaction is active":"cannot commit - no transaction is active"));
										rc=Sqlite3.SQLITE_ERROR;
									}
							break;
						}
						///
						///<summary>
						///Opcode: Transaction P1 P2 * * *
						///
						///Begin a transaction.  The transaction ends when a Commit or Rollback
						///opcode is encountered.  Depending on the ON CONFLICT setting, the
						///transaction might also be rolled back if an error is encountered.
						///
						///P1 is the index of the database file on which the transaction is
						///started.  Index 0 is the main database file and index 1 is the
						///file used for temporary tables.  Indices of 2 or more are used for
						///attached databases.
						///
						///</summary>
						///<param name="If P2 is non">transaction is started.  A RESERVED lock is</param>
						///<param name="obtained on the database file when a write">transaction is started.  No</param>
						///<param name="other process can start another write transaction while this transaction is">other process can start another write transaction while this transaction is</param>
						///<param name="underway.  Starting a write transaction also creates a rollback journal. A">underway.  Starting a write transaction also creates a rollback journal. A</param>
						///<param name="write transaction must be started before any changes can be made to the">write transaction must be started before any changes can be made to the</param>
						///<param name="database.  If P2 is 2 or greater then an EXCLUSIVE lock is also obtained">database.  If P2 is 2 or greater then an EXCLUSIVE lock is also obtained</param>
						///<param name="on the file.">on the file.</param>
						///<param name=""></param>
						///<param name="If a write">transaction is started and the Vdbe.usesStmtJournal flag is</param>
						///<param name="true (this flag is set if the Vdbe may modify more than one row and may">true (this flag is set if the Vdbe may modify more than one row and may</param>
						///<param name="throw an ABORT exception), a statement transaction may also be opened.">throw an ABORT exception), a statement transaction may also be opened.</param>
						///<param name="More specifically, a statement transaction is opened iff the database">More specifically, a statement transaction is opened iff the database</param>
						///<param name="connection is currently not in autocommit mode, or if there are other">connection is currently not in autocommit mode, or if there are other</param>
						///<param name="active statements. A statement transaction allows the affects of this">active statements. A statement transaction allows the affects of this</param>
						///<param name="VDBE to be rolled back after an error without having to roll back the">VDBE to be rolled back after an error without having to roll back the</param>
						///<param name="entire transaction. If no error is encountered, the statement transaction">entire transaction. If no error is encountered, the statement transaction</param>
						///<param name="will automatically commit when the VDBE halts.">will automatically commit when the VDBE halts.</param>
						///<param name=""></param>
						///<param name="If P2 is zero, then a read">lock is obtained on the database file.</param>
						///<param name=""></param>
						case OpCode.OP_Transaction: {
							Btree pBt;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pBt=db.aDb[pOp.p1].pBt;
							if(pBt!=null) {
								rc=pBt.sqlite3BtreeBeginTrans(pOp.p2);
								if(rc==SQLITE_BUSY) {
									this.currentOpCodeIndex=opcodeIndex;
									this.rc=rc=SQLITE_BUSY;
									goto vdbe_return;
								}
								if(rc!=Sqlite3.SQLITE_OK) {
									goto abort_due_to_error;
								}
								if(pOp.p2!=0&&this.usesStmtJournal&&(db.autoCommit==0||db.activeVdbeCnt>1)) {
									Debug.Assert(pBt.sqlite3BtreeIsInTrans());
									if(this.iStatement==0) {
										Debug.Assert(db.nStatement>=0&&db.nSavepoint>=0);
										db.nStatement++;
										this.iStatement=db.nSavepoint+db.nStatement;
									}
                                    rc = vtab.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_BEGIN, this.iStatement - 1);
									if(rc==Sqlite3.SQLITE_OK) {
										rc=pBt.sqlite3BtreeBeginStmt(this.iStatement);
									}
									///
									///<summary>
									///Store the current value of the database handles deferred constraint
									///counter. If the statement transaction needs to be rolled back,
									///the value of this counter needs to be restored too.  
									///</summary>
									this.nStmtDefCons=db.nDeferredCons;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: ReadCookie P1 P2 P3 * *
						///
						///Read cookie number P3 from database P1 and write it into register P2.
						///P3==1 is the schema version.  P3==2 is the database format.
						///P3==3 is the recommended pager cache size, and so forth.  P1==0 is
						///the main database file and P1==1 is the database file used to store
						///temporary tables.
						///
						///</summary>
						///<param name="There must be a read">lock on the database (either a transaction</param>
						///<param name="must be started or there must be an open cursor) before">must be started or there must be an open cursor) before</param>
						///<param name="executing this instruction.">executing this instruction.</param>
						///<param name=""></param>
						case OpCode.OP_ReadCookie: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							u32 iMeta;
							int iDb;
							int iCookie;
							iMeta=0;
							iDb=pOp.p1;
							iCookie=pOp.p3;
							Debug.Assert(pOp.p3<SQLITE_N_BTREE_META);
							Debug.Assert(iDb>=0&&iDb<db.nDb);
							Debug.Assert(db.aDb[iDb].pBt!=null);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<iDb))!=0);
							iMeta=db.aDb[iDb].pBt.sqlite3BtreeGetMeta(iCookie);
							pOut.u.i=(int)iMeta;
							break;
						}
						///
						///<summary>
						///Opcode: SetCookie P1 P2 P3 * *
						///
						///Write the content of register P3 (interpreted as an integer)
						///into cookie number P2 of database P1.  P2==1 is the schema version.
						///P2==2 is the database format. P2==3 is the recommended pager cache
						///size, and so forth.  P1==0 is the main database file and P1==1 is the
						///database file used to store temporary tables.
						///
						///A transaction must be started before executing this opcode.
						///
						///</summary>
						case OpCode.OP_SetCookie: {
							///
							///<summary>
							///in3 
							///</summary>
							Db pDb;
							Debug.Assert(pOp.p2<SQLITE_N_BTREE_META);
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pDb=db.aDb[pOp.p1];
							Debug.Assert(pDb.pBt!=null);
							Debug.Assert(sqlite3SchemaMutexHeld(db,pOp.p1,null));
							pIn3=aMem[pOp.p3];
							pIn3.sqlite3VdbeMemIntegerify();
							///
							///<summary>
							///See note about index shifting on  OpCode.OP_ReadCookie 
							///</summary>
							rc=pDb.pBt.sqlite3BtreeUpdateMeta(pOp.p2,(u32)pIn3.u.i);
							if(pOp.p2==(int)BTreeProp.SCHEMA_VERSION) {
								///
								///<summary>
								///When the schema cookie changes, record the new cookie internally 
								///</summary>
								pDb.pSchema.schema_cookie=(int)pIn3.u.i;
                                db.flags |= SqliteFlags.SQLITE_InternChanges;
							}
							else
								if(pOp.p2==(int)BTreeProp.FILE_FORMAT) {
									///
									///<summary>
									///Record changes in the file format 
									///</summary>
									pDb.pSchema.file_format=(u8)pIn3.u.i;
								}
							if(pOp.p1==1) {
								///
								///<summary>
								///Invalidate all prepared statements whenever the TEMP database
								///schema is changed.  Ticket #1644 
								///</summary>
                                vdbeaux.sqlite3ExpirePreparedStatements(db);
								this.expired=false;
							}
							break;
						}
						///
						///<summary>
						///Opcode: VerifyCookie P1 P2 P3 * *
						///
						///Check the value of global database parameter number 0 (the
						///schema version) and make sure it is equal to P2 and that the
						///generation counter on the local schema parse equals P3.
						///
						///P1 is the database number which is 0 for the main database file
						///and 1 for the file holding temporary tables and some higher number
						///for auxiliary databases.
						///
						///The cookie changes its value whenever the database schema changes.
						///This operation is used to detect when that the cookie has changed
						///and that the current process needs to reread the schema.
						///
						///Either a transaction needs to have been started or an  OpCode.OP_Open needs
						///to be executed (to establish a read lock) before this opcode is
						///invoked.
						///
						///</summary>
						case OpCode.OP_VerifyCookie: {
							u32 iMeta=0;
							u32 iGen;
							Btree pBt;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&((yDbMask)1<<pOp.p1))!=0);
							Debug.Assert(sqlite3SchemaMutexHeld(db,pOp.p1,null));
							pBt=db.aDb[pOp.p1].pBt;
							if(pBt!=null) {
								iMeta=pBt.sqlite3BtreeGetMeta(BTREE_SCHEMA_VERSION);
								iGen=db.aDb[pOp.p1].pSchema.iGeneration;
							}
							else {
								iGen=iMeta=0;
							}
							if(iMeta!=pOp.p2||iGen!=pOp.p3) {
								db.sqlite3DbFree(ref this.zErrMsg);
								this.zErrMsg="database schema has changed";
								// sqlite3DbStrDup(db, "database schema has changed");
								///
								///<summary>
								///</summary>
								///<param name="If the schema">cookie from the database file matches the cookie</param>
								///<param name="stored with the in">memory representation of the schema, do</param>
								///<param name="not reload the schema from the database file.">not reload the schema from the database file.</param>
								///<param name=""></param>
								///<param name="If virtual">tables are in use, this is not just an optimization.</param>
								///<param name="Often, v">tables store their data in other SQLite tables, which</param>
								///<param name="are queried from within xNext() and other v">table methods using</param>
								///<param name="prepared queries. If such a query is out">date, we do not want to</param>
								///<param name="discard the database schema, as the user code implementing the">discard the database schema, as the user code implementing the</param>
								///<param name="v">table would have to be ready for the sqlite3_vtab structure itself</param>
								///<param name="to be invalidated whenever sqlite3_step() is called from within">to be invalidated whenever sqlite3_step() is called from within</param>
								///<param name="a v">table method.</param>
								///<param name=""></param>
								if(db.aDb[pOp.p1].pSchema.schema_cookie!=iMeta) {
									build.sqlite3ResetInternalSchema(db,pOp.p1);
								}
								this.expired=true;
								rc=SQLITE_SCHEMA;
							}
							break;
						}
						///
						///<summary>
						///Opcode: OpenRead P1 P2 P3 P4 P5
						///
						///</summary>
						///<param name="Open a read">only cursor for the database table whose root page is</param>
						///<param name="P2 in a database file.  The database file is determined by P3.">P2 in a database file.  The database file is determined by P3.</param>
						///<param name="P3==0 means the main database, P3==1 means the database used for">P3==0 means the main database, P3==1 means the database used for</param>
						///<param name="temporary tables, and P3>1 means used the corresponding attached">temporary tables, and P3>1 means used the corresponding attached</param>
						///<param name="database.  Give the new cursor an identifier of P1.  The P1">database.  Give the new cursor an identifier of P1.  The P1</param>
						///<param name="values need not be contiguous but all P1 values should be small integers.">values need not be contiguous but all P1 values should be small integers.</param>
						///<param name="It is an error for P1 to be negative.">It is an error for P1 to be negative.</param>
						///<param name=""></param>
						///<param name="If P5!=0 then use the content of register P2 as the root page, not">If P5!=0 then use the content of register P2 as the root page, not</param>
						///<param name="the value of P2 itself.">the value of P2 itself.</param>
						///<param name=""></param>
						///<param name="There will be a read lock on the database whenever there is an">There will be a read lock on the database whenever there is an</param>
						///<param name="open cursor.  If the database was unlocked prior to this instruction">open cursor.  If the database was unlocked prior to this instruction</param>
						///<param name="then a read lock is acquired as part of this instruction.  A read">then a read lock is acquired as part of this instruction.  A read</param>
						///<param name="lock allows other processes to read the database but prohibits">lock allows other processes to read the database but prohibits</param>
						///<param name="any other process from modifying the database.  The read lock is">any other process from modifying the database.  The read lock is</param>
						///<param name="released when all cursors are closed.  If this instruction attempts">released when all cursors are closed.  If this instruction attempts</param>
						///<param name="to get a read lock but fails, the script terminates with an">to get a read lock but fails, the script terminates with an</param>
						///<param name="SQLITE_BUSY error code.">SQLITE_BUSY error code.</param>
						///<param name=""></param>
						///<param name="The P4 value may be either an integer ( P4Usage.P4_INT32) or a pointer to">The P4 value may be either an integer ( P4Usage.P4_INT32) or a pointer to</param>
						///<param name="a KeyInfo structure ( P4Usage.P4_KEYINFO). If it is a pointer to a KeyInfo">a KeyInfo structure ( P4Usage.P4_KEYINFO). If it is a pointer to a KeyInfo</param>
						///<param name="structure, then said structure defines the content and collating">structure, then said structure defines the content and collating</param>
						///<param name="sequence of the index being opened. Otherwise, if P4 is an integer">sequence of the index being opened. Otherwise, if P4 is an integer</param>
						///<param name="value, it is set to the number of columns in the table.">value, it is set to the number of columns in the table.</param>
						///<param name=""></param>
						///<param name="See also OpenWrite.">See also OpenWrite.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: OpenWrite P1 P2 P3 P4 P5
						///
						///Open a read/write cursor named P1 on the table or index whose root
						///page is P2.  Or if P5!=0 use the content of register P2 to find the
						///root page.
						///
						///The P4 value may be either an integer ( P4Usage.P4_INT32) or a pointer to
						///a KeyInfo structure ( P4Usage.P4_KEYINFO). If it is a pointer to a KeyInfo
						///structure, then said structure defines the content and collating
						///sequence of the index being opened. Otherwise, if P4 is an integer
						///value, it is set to the number of columns in the table, or to the
						///largest index of any column of the table that is actually used.
						///
						///This instruction works just like OpenRead except that it opens the cursor
						///</summary>
						///<param name="in read/write mode.  For a given table, there can be one or more read">only</param>
						///<param name="cursors or a single read/write cursor but not both.">cursors or a single read/write cursor but not both.</param>
						///<param name=""></param>
						///<param name="See also OpenRead.">See also OpenRead.</param>
						///<param name=""></param>
						case OpCode.OP_OpenRead:
						case OpCode.OP_OpenWrite: {
							int nField;
							KeyInfo pKeyInfo;
							int p2;
							int iDb;
							int wrFlag;
							Btree pX;
							VdbeCursor pCur;
							Db pDb;
							if(this.expired) {
								rc=SQLITE_ABORT;
								break;
							}
							nField=0;
							pKeyInfo=null;
							p2=pOp.p2;
							iDb=pOp.p3;
							Debug.Assert(iDb>=0&&iDb<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<iDb))!=0);
							pDb=db.aDb[iDb];
							pX=pDb.pBt;
							Debug.Assert(pX!=null);
							if(pOp.OpCode==OpCode.OP_OpenWrite) {
								wrFlag=1;
								Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
								if(pDb.pSchema.file_format<this.minWriteFileFormat) {
									this.minWriteFileFormat=pDb.pSchema.file_format;
								}
							}
							else {
								wrFlag=0;
							}
							if(pOp.p5!=0) {
								Debug.Assert(p2>0);
								Debug.Assert(p2<=this.nMem);
								pIn2=aMem[p2];
								Debug.Assert(pIn2.memIsValid());
								Debug.Assert((pIn2.flags&MemFlags.MEM_Int)!=0);
								pIn2.sqlite3VdbeMemIntegerify();
								p2=(int)pIn2.u.i;
								///
								///<summary>
								///The p2 value always comes from a prior  OpCode.OP_CreateTable opcode and
								///that opcode will always set the p2 value to 2 or more or else fail.
								///If there were a failure, the prepared statement would have halted
								///before reaching this instruction. 
								///</summary>
								if(NEVER(p2<2)) {
                                    rc = sqliteinth.SQLITE_CORRUPT_BKPT();
									goto abort_due_to_error;
								}
							}
							if(pOp.p4type== P4Usage.P4_KEYINFO) {
								pKeyInfo=pOp.p4.pKeyInfo;
								pKeyInfo.enc=sqliteinth.ENC(this.db);
								nField=pKeyInfo.nField+1;
							}
							else
								if(pOp.p4type== P4Usage.P4_INT32) {
									nField=pOp.p4.i;
								}
							Debug.Assert(pOp.p1>=0);
							pCur=allocateCursor(this,pOp.p1,nField,iDb,1);
							if(pCur==null)
								goto no_mem;
							pCur.nullRow=true;
							pCur.isOrdered=true;
							rc=pX.sqlite3BtreeCursor(p2,wrFlag,pKeyInfo,pCur.pCursor);
							pCur.pKeyInfo=pKeyInfo;
							///
							///<summary>
							///Since it performs no memory allocation or IO, the only values that
							///sqlite3BtreeCursor() may return are SQLITE_EMPTY and Sqlite3.SQLITE_OK. 
							///SQLITE_EMPTY is only returned when attempting to open the table
							///</summary>
							///<param name="rooted at page 1 of a zero">byte database.  </param>
							Debug.Assert(rc==SQLITE_EMPTY||rc==Sqlite3.SQLITE_OK);
							if(rc==SQLITE_EMPTY) {
								mempoolMethods.sqlite3MemFreeBtCursor(ref pCur.pCursor);
								rc=Sqlite3.SQLITE_OK;
							}
							///
							///<summary>
							///Set the VdbeCursor.isTable and isIndex variables. Previous versions of
							///</summary>
							///<param name="SQLite used to check if the root">page flags were sane at this point</param>
							///<param name="and report database corruption if they were not, but this check has">and report database corruption if they were not, but this check has</param>
							///<param name="since moved into the btree layer.  ">since moved into the btree layer.  </param>
							pCur.isTable=pOp.p4type!= P4Usage.P4_KEYINFO;
							pCur.isIndex=!pCur.isTable;
							break;
						}
						///
						///<summary>
						///Opcode: OpenEphemeral P1 P2 * P4 *
						///
						///Open a new cursor P1 to a transient table.
						///The cursor is always opened read/write even if 
						///the main database is read">only.  The ephemeral</param>
						///table is deleted automatically when the cursor is closed.">table is deleted automatically when the cursor is closed.</param>
						///"></param>
						///P2 is the number of columns in the ephemeral table.">P2 is the number of columns in the ephemeral table.</param>
						///The cursor points to a BTree table if P4==0 and to a BTree index">The cursor points to a BTree table if P4==0 and to a BTree index</param>
						///if P4 is not 0.  If P4 is not NULL, it points to a KeyInfo structure">if P4 is not 0.  If P4 is not NULL, it points to a KeyInfo structure</param>
						///that defines the format of keys in the index.">that defines the format of keys in the index.</param>
						///"></param>
						///This opcode was once called OpenTemp.  But that created">This opcode was once called OpenTemp.  But that created</param>
						///confusion because the term "temp table", might refer either">confusion because the term "temp table", might refer either</param>
						///to a TEMP table at the SQL level, or to a table opened by">to a TEMP table at the SQL level, or to a table opened by</param>
						///this opcode.  Then this opcode was call OpenVirtual.  But">this opcode.  Then this opcode was call OpenVirtual.  But</param>
						///that created confusion with the whole virtual">table idea.</param>
						///"></param>
						///
						///Opcode: OpenAutoindex P1 P2 * P4 *
						///
						///This opcode works the same as  OpCode.OP_OpenEphemeral.  It has a
						///different name to distinguish its use.  Tables created using
						///by this opcode will be used for automatically created transient
						///indices in joins.
						///
						///</summary>
						case OpCode.OP_OpenAutoindex:
						case OpCode.OP_OpenEphemeral: {
							VdbeCursor pCx;
							const int vfsFlags=SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE|SQLITE_OPEN_EXCLUSIVE|SQLITE_OPEN_DELETEONCLOSE|SQLITE_OPEN_TRANSIENT_DB;
							Debug.Assert(pOp.p1>=0);
							pCx=allocateCursor(this,pOp.p1,pOp.p2,-1,1);
							if(pCx==null)
								goto no_mem;
							pCx.nullRow=true;
							rc=Btree.Open(db.pVfs,null,db,ref pCx.pBt,BTREE_OMIT_JOURNAL|BTREE_SINGLE|pOp.p5,vfsFlags);
							if(rc==Sqlite3.SQLITE_OK) {
								rc=pCx.pBt.sqlite3BtreeBeginTrans(1);
							}
							if(rc==Sqlite3.SQLITE_OK) {
								///
								///<summary>
								///If a transient index is required, create it by calling
								///sqlite3BtreeCreateTable() with the BTREE_BLOBKEY flag before
								///opening it. If a transient table is required, just use the
								///</summary>
								///<param name="automatically created table with root">page 1 (an BLOB_INTKEY table).</param>
								///<param name=""></param>
								if(pOp.p4.pKeyInfo!=null) {
									int pgno=0;
									Debug.Assert(pOp.p4type== P4Usage.P4_KEYINFO);
                                    rc = pCx.pBt.sqlite3BtreeCreateTable(ref pgno, BTREE_BLOBKEY);
									if(rc==Sqlite3.SQLITE_OK) {
										Debug.Assert(pgno==sqliteinth.MASTER_ROOT+1);
										rc=pCx.pBt.sqlite3BtreeCursor(pgno,1,pOp.p4.pKeyInfo,pCx.pCursor);
										pCx.pKeyInfo=pOp.p4.pKeyInfo;
										pCx.pKeyInfo.enc=sqliteinth.ENC(this.db);
									}
									pCx.isTable=false;
								}
								else {
									rc=pCx.pBt.sqlite3BtreeCursor(sqliteinth.MASTER_ROOT,1,null,pCx.pCursor);
									pCx.isTable=true;
								}
							}
							pCx.isOrdered=(pOp.p5!=BTREE_UNORDERED);
							pCx.isIndex=!pCx.isTable;
							break;
						}
						///
						///<summary>
						///Opcode: OpenPseudo P1 P2 P3 * *
						///
						///Open a new cursor that points to a fake table that contains a single
						///row of data.  The content of that one row in the content of memory
						///register P2.  In other words, cursor P1 becomes an alias for the 
						///MEM.MEM_Blob content contained in register P2.
						///
						///</summary>
						///<param name="A pseudo">table created by this opcode is used to hold a single</param>
						///<param name="row output from the sorter so that the row can be decomposed into">row output from the sorter so that the row can be decomposed into</param>
						///<param name="individual columns using the  OpCode.OP_Column opcode.  The  OpCode.OP_Column opcode">individual columns using the  OpCode.OP_Column opcode.  The  OpCode.OP_Column opcode</param>
						///<param name="is the only cursor opcode that works with a pseudo">table.</param>
						///<param name=""></param>
						///<param name="P3 is the number of fields in the records that will be stored by">P3 is the number of fields in the records that will be stored by</param>
						///<param name="the pseudo">table.</param>
						///<param name=""></param>
						case OpCode.OP_OpenPseudo: {
							VdbeCursor pCx;
							Debug.Assert(pOp.p1>=0);
							pCx=allocateCursor(this,pOp.p1,pOp.p3,-1,0);
							if(pCx==null)
								goto no_mem;
							pCx.nullRow=true;
							pCx.pseudoTableReg=pOp.p2;
							pCx.isTable=true;
							pCx.isIndex=false;
							break;
						}
						///
						///<summary>
						///Opcode: Close P1 * * * *
						///
						///Close a cursor previously opened as P1.  If P1 is not
						///</summary>
						///<param name="currently open, this instruction is a no">op.</param>
						///<param name=""></param>
						case OpCode.OP_Close: {
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
                            vdbeaux.sqlite3VdbeFreeCursor(this, this.apCsr[pOp.p1]);
							this.apCsr[pOp.p1]=null;
							break;
						}
						///
						///<summary>
						///Opcode: SeekGe P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
						///<param name="use the value in register P3 as the key.  If cursor P1 refers">use the value in register P3 as the key.  If cursor P1 refers</param>
						///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
						///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
						///<param name=""></param>
						///<param name="Reposition cursor P1 so that  it points to the smallest entry that">Reposition cursor P1 so that  it points to the smallest entry that</param>
						///<param name="is greater than or equal to the key value. If there are no records">is greater than or equal to the key value. If there are no records</param>
						///<param name="greater than or equal to the key and P2 is not zero, then jump to P2.">greater than or equal to the key and P2 is not zero, then jump to P2.</param>
						///<param name=""></param>
						///<param name="See also: Found, NotFound, Distinct, SeekLt, SeekGt, SeekLe">See also: Found, NotFound, Distinct, SeekLt, SeekGt, SeekLe</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: SeekGt P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
						///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
						///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
						///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
						///<param name=""></param>
						///<param name="Reposition cursor P1 so that  it points to the smallest entry that">Reposition cursor P1 so that  it points to the smallest entry that</param>
						///<param name="is greater than the key value. If there are no records greater than">is greater than the key value. If there are no records greater than</param>
						///<param name="the key and P2 is not zero, then jump to P2.">the key and P2 is not zero, then jump to P2.</param>
						///<param name=""></param>
						///<param name="See also: Found, NotFound, Distinct, SeekLt, SeekGe, SeekLe">See also: Found, NotFound, Distinct, SeekLt, SeekGe, SeekLe</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: SeekLt P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
						///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
						///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
						///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
						///<param name=""></param>
						///<param name="Reposition cursor P1 so that  it points to the largest entry that">Reposition cursor P1 so that  it points to the largest entry that</param>
						///<param name="is less than the key value. If there are no records less than">is less than the key value. If there are no records less than</param>
						///<param name="the key and P2 is not zero, then jump to P2.">the key and P2 is not zero, then jump to P2.</param>
						///<param name=""></param>
						///<param name="See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLe">See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLe</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: SeekLe P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
						///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
						///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
						///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
						///<param name=""></param>
						///<param name="Reposition cursor P1 so that it points to the largest entry that">Reposition cursor P1 so that it points to the largest entry that</param>
						///<param name="is less than or equal to the key value. If there are no records">is less than or equal to the key value. If there are no records</param>
						///<param name="less than or equal to the key and P2 is not zero, then jump to P2.">less than or equal to the key and P2 is not zero, then jump to P2.</param>
						///<param name=""></param>
						///<param name="See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLt">See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLt</param>
						///<param name=""></param>
						case OpCode.OP_SeekLt:
						///
						///<summary>
						///jump, in3 
						///</summary>
						case OpCode.OP_SeekLe:
						///
						///<summary>
						///jump, in3 
						///</summary>
						case OpCode.OP_SeekGe:
						///
						///<summary>
						///jump, in3 
						///</summary>
						case OpCode.OP_SeekGt: {
							///
							///<summary>
							///jump, in3 
							///</summary>
							int res;
                            OpCode oc;
							VdbeCursor pC;
							UnpackedRecord r;
							int nField;
							i64 iKey;
							///
							///<summary>
							///The rowid we are to seek to 
							///</summary>
							res=0;
							r=new UnpackedRecord();
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pOp.p2!=0);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pseudoTableReg==0);
                            Debug.Assert(OpCode.OP_SeekLe == OpCode.OP_SeekLt + 1);
                            Debug.Assert(OpCode.OP_SeekGe == OpCode.OP_SeekLt + 2);
                            Debug.Assert(OpCode.OP_SeekGt == OpCode.OP_SeekLt + 3);
							Debug.Assert(pC.isOrdered);
							if(pC.pCursor!=null) {
								oc=pOp.OpCode;
								pC.nullRow=false;
								if(pC.isTable) {
									///
									///<summary>
									///The input value in P3 might be of any type: integer, real, string,
									///blob, or NULL.  But it needs to be an integer before we can do
									///the seek, so convert it. 
									///</summary>
									pIn3=aMem[pOp.p3];
									applyNumericAffinity(pIn3);
									iKey=pIn3.sqlite3VdbeIntValue();
									pC.rowidIsValid=false;
									///
									///<summary>
									///If the P3 value could not be converted into an integer without
									///loss of information, then special processing is required... 
									///</summary>
									if((pIn3.flags&MemFlags.MEM_Int)==0) {
										if((pIn3.flags&MemFlags.MEM_Real)==0) {
											///
											///<summary>
											///If the P3 value cannot be converted into any kind of a number,
											///then the seek is not possible, so jump to P2 
											///</summary>
											opcodeIndex=pOp.p2-1;
											break;
										}
										///
										///<summary>
										///If we reach this point, then the P3 value must be a floating
										///point number. 
										///</summary>
										Debug.Assert((pIn3.flags&MemFlags.MEM_Real)!=0);
										if(iKey==IntegerExtensions.SMALLEST_INT64&&(pIn3.r<(double)iKey||pIn3.r>0)) {
											///
											///<summary>
											///The P3 value is too large in magnitude to be expressed as an
											///integer. 
											///</summary>
											res=1;
											if(pIn3.r<0) {
                                                if (oc >= OpCode.OP_SeekGe)
                                                {
                                                    Debug.Assert(oc == OpCode.OP_SeekGe || oc == OpCode.OP_SeekGt);
													rc=pC.pCursor.sqlite3BtreeFirst(ref res);
													if(rc!=Sqlite3.SQLITE_OK)
														goto abort_due_to_error;
												}
											}
											else {
                                                if (oc <= OpCode.OP_SeekLe)
                                                {
                                                    Debug.Assert(oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekLe);
													rc=pC.pCursor.sqlite3BtreeLast(ref res);
													if(rc!=Sqlite3.SQLITE_OK)
														goto abort_due_to_error;
												}
											}
											if(res!=0) {
												opcodeIndex=pOp.p2-1;
											}
											break;
										}
										else
                                            if (oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekGe)
                                            {
												///
												///<summary>
												///Use the ceiling() function to convert real.int 
												///</summary>
												if(pIn3.r>(double)iKey)
													iKey++;
											}
											else {
												///
												///<summary>
												///Use the floor() function to convert real.int 
												///</summary>
                                                Debug.Assert(oc == OpCode.OP_SeekLe || oc == OpCode.OP_SeekGt);
												if(pIn3.r<(double)iKey)
													iKey--;
											}
									}
									rc=pC.pCursor.sqlite3BtreeMovetoUnpacked(null,iKey,0,ref res);
									if(rc!=Sqlite3.SQLITE_OK) {
										goto abort_due_to_error;
									}
									if(res==0) {
										pC.rowidIsValid=true;
										pC.lastRowid=iKey;
									}
								}
								else {
									nField=pOp.p4.i;
									Debug.Assert(pOp.p4type== P4Usage.P4_INT32);
									Debug.Assert(nField>0);
									r.pKeyInfo=pC.pKeyInfo;
									r.nField=(u16)nField;
									///
									///<summary>
									///The next line of code computes as follows, only faster:
									///if( oc== OpCode.OP_SeekGt || oc== OpCode.OP_SeekLe ){
									///r.flags = UnpackedRecordFlags.UNPACKED_INCRKEY;
									///}else{
									///r.flags = 0;
									///}
									///
									///</summary>
                                    r.flags = (UnpackedRecordFlags)((int)UnpackedRecordFlags.UNPACKED_INCRKEY * (1 & (oc - OpCode.OP_SeekLt)));
                                    Debug.Assert(oc != OpCode.OP_SeekGt || r.flags == UnpackedRecordFlags.UNPACKED_INCRKEY);
                                    Debug.Assert(oc != OpCode.OP_SeekLe || r.flags == UnpackedRecordFlags.UNPACKED_INCRKEY);
                                    Debug.Assert(oc != OpCode.OP_SeekGe || r.flags == 0);
                                    Debug.Assert(oc != OpCode.OP_SeekLt || r.flags == 0);
									r.aMem=new Mem[r.nField];
									for(int rI=0;rI<r.nField;rI++)
										r.aMem[rI]=aMem[pOp.p3+rI];
									// r.aMem = aMem[pOp.p3];
									#if SQLITE_DEBUG
																																																																																																																																																																																						                  {
                    int i;
                    for ( i = 0; i < r.nField; i++ )
                      Debug.Assert( memIsValid( r.aMem[i] ) );
                  }
#endif
									ExpandBlob(r.aMem[0]);
									rc=pC.pCursor.sqlite3BtreeMovetoUnpacked(r,0,0,ref res);
									if(rc!=Sqlite3.SQLITE_OK) {
										goto abort_due_to_error;
									}
									pC.rowidIsValid=false;
								}
								pC.deferredMoveto=false;
								pC.cacheStatus=CACHE_STALE;
								#if SQLITE_TEST
																#if !TCLSH
																																																																																																																																																													                sqlite3_search_count++;
#else
																																																																																																																																																													                sqlite3_search_count.iValue++;
#endif
																#endif
                                if (oc >= OpCode.OP_SeekGe)
                                {
                                    Debug.Assert(oc == OpCode.OP_SeekGe || oc == OpCode.OP_SeekGt);
                                    if (res < 0 || (res == 0 && oc == OpCode.OP_SeekGt))
                                    {
										rc=pC.pCursor.sqlite3BtreeNext(ref res);
										if(rc!=Sqlite3.SQLITE_OK)
											goto abort_due_to_error;
										pC.rowidIsValid=false;
									}
									else {
										res=0;
									}
								}
								else {
                                    Debug.Assert(oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekLe);
                                    if (res > 0 || (res == 0 && oc == OpCode.OP_SeekLt))
                                    {
										rc=pC.pCursor.sqlite3BtreePrevious(ref res);
										if(rc!=Sqlite3.SQLITE_OK)
											goto abort_due_to_error;
										pC.rowidIsValid=false;
									}
									else {
										///
										///<summary>
										///res might be negative because the table is empty.  Check to
										///see if this is the case.
										///
										///</summary>
										res=pC.pCursor.sqlite3BtreeEof()?1:0;
									}
								}
								Debug.Assert(pOp.p2>0);
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
								}
							}
							else {
								///
								///<summary>
								///This happens when attempting to open the sqlite3_master table
								///for read access returns SQLITE_EMPTY. In this case always
								///take the jump (since there are no records in the table).
								///
								///</summary>
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Seek P1 P2 * * *
						///
						///P1 is an open table cursor and P2 is a rowid integer.  Arrange
						///for P1 to move so that it points to the rowid given by P2.
						///
						///This is actually a deferred seek.  Nothing actually happens until
						///the cursor is used to read a record.  That way, if no reads
						///occur, no unnecessary I/O happens.
						///
						///</summary>
						case OpCode.OP_Seek: {
							///
							///<summary>
							///in2 
							///</summary>
							VdbeCursor pC;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(Sqlite3.ALWAYS(pC!=null));
							if(pC.pCursor!=null) {
								Debug.Assert(pC.isTable);
								pC.nullRow=false;
								pIn2=aMem[pOp.p2];
								pC.movetoTarget=pIn2.sqlite3VdbeIntValue();
								pC.rowidIsValid=false;
								pC.deferredMoveto=true;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Found P1 P2 P3 P4 *
						///
						///If P4==0 then register P3 holds a blob constructed by MakeRecord.  If
						///P4>0 then register P3 is the first of P4 registers that form an unpacked
						///record.
						///
						///Cursor P1 is on an index btree.  If the record identified by P3 and P4
						///is a prefix of any entry in P1 then a jump is made to P2 and
						///P1 is left pointing at the matching entry.
						///
						///</summary>
						///
						///<summary>
						///Opcode: NotFound P1 P2 P3 P4 *
						///
						///If P4==0 then register P3 holds a blob constructed by MakeRecord.  If
						///P4>0 then register P3 is the first of P4 registers that form an unpacked
						///record.
						///
						///Cursor P1 is on an index btree.  If the record identified by P3 and P4
						///is not the prefix of any entry in P1 then a jump is made to P2.  If P1 
						///does contain an entry whose prefix matches the P3/P4 record then control
						///falls through to the next instruction and P1 is left pointing at the
						///matching entry.
						///
						///See also: Found, NotExists, IsUnique
						///
						///</summary>
						case OpCode.OP_NotFound:
						///
						///<summary>
						///jump, in3 
						///</summary>
						case OpCode.OP_Found: {
							///
							///<summary>
							///jump, in3 
							///</summary>
							int alreadyExists;
							VdbeCursor pC;
							int res=0;
							UnpackedRecord pIdxKey;
							UnpackedRecord r=new UnpackedRecord();
							UnpackedRecord aTempRec=new UnpackedRecord();
							//char aTempRec[ROUND8(sizeof(UnpackedRecord)) + sizeof(Mem)*3 + 7];
							#if SQLITE_TEST
														#if !TCLSH
																																																																																																																																				              sqlite3_found_count++;
#else
																																																																																																																																				              sqlite3_found_count.iValue++;
#endif
														#endif
							alreadyExists=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pOp.p4type== P4Usage.P4_INT32);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pIn3=aMem[pOp.p3];
							if(Sqlite3.ALWAYS(pC.pCursor!=null)) {
								Debug.Assert(!pC.isTable);
								if(pOp.p4.i>0) {
									r.pKeyInfo=pC.pKeyInfo;
									r.nField=(u16)pOp.p4.i;
									r.aMem=new Mem[r.nField];
									for(int i=0;i<r.aMem.Length;i++) {
										r.aMem[i]=aMem[pOp.p3+i];
										#if SQLITE_DEBUG
																																																																																																																																																																																																															                    Debug.Assert( memIsValid( r.aMem[i] ) );
#endif
									}
									r.flags=UnpackedRecordFlags.UNPACKED_PREFIX_MATCH;
									pIdxKey=r;
								}
								else {
									Debug.Assert((pIn3.flags&MemFlags.MEM_Blob)!=0);
									Debug.Assert((pIn3.flags&MemFlags.MEM_Zero)==0);
									///
									///<summary>
									///zeroblobs already expanded 
									///</summary>
                                    pIdxKey = vdbeaux.sqlite3VdbeRecordUnpack(pC.pKeyInfo, pIn3.n, pIn3.zBLOB, aTempRec, 0);
									//sizeof( aTempRec ) );
									if(pIdxKey==null) {
										goto no_mem;
									}
									pIdxKey.flags|=UnpackedRecordFlags.UNPACKED_PREFIX_MATCH;
								}
								rc=pC.pCursor.sqlite3BtreeMovetoUnpacked(pIdxKey,0,0,ref res);
								if(pOp.p4.i==0) {
                                    vdbeaux.sqlite3VdbeDeleteUnpackedRecord(pIdxKey);
								}
								if(rc!=Sqlite3.SQLITE_OK) {
									break;
								}
								alreadyExists=(res==0)?1:0;
								pC.deferredMoveto=false;
								pC.cacheStatus=CACHE_STALE;
							}
							if(pOp.OpCode==OpCode.OP_Found) {
								if(alreadyExists!=0)
									opcodeIndex=pOp.p2-1;
							}
							else {
								if(0==alreadyExists)
									opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: IsUnique P1 P2 P3 P4 *
						///
						///</summary>
						///<param name="Cursor P1 is open on an index b"> that is to say, a btree which</param>
						///<param name="no data and where the key are records generated by  OpCode.OP_MakeRecord with">no data and where the key are records generated by  OpCode.OP_MakeRecord with</param>
						///<param name="the list field being the integer ROWID of the entry that the index">the list field being the integer ROWID of the entry that the index</param>
						///<param name="entry refers to.">entry refers to.</param>
						///<param name=""></param>
						///<param name="The P3 register contains an integer record number. Call this record">The P3 register contains an integer record number. Call this record</param>
						///<param name="number R. Register P4 is the first in a set of N contiguous registers">number R. Register P4 is the first in a set of N contiguous registers</param>
						///<param name="that make up an unpacked index key that can be used with cursor P1.">that make up an unpacked index key that can be used with cursor P1.</param>
						///<param name="The value of N can be inferred from the cursor. N includes the rowid">The value of N can be inferred from the cursor. N includes the rowid</param>
						///<param name="value appended to the end of the index record. This rowid value may">value appended to the end of the index record. This rowid value may</param>
						///<param name="or may not be the same as R.">or may not be the same as R.</param>
						///<param name=""></param>
						///<param name="If any of the N registers beginning with register P4 contains a NULL">If any of the N registers beginning with register P4 contains a NULL</param>
						///<param name="value, jump immediately to P2.">value, jump immediately to P2.</param>
						///<param name=""></param>
						///<param name="Otherwise, this instruction checks if cursor P1 contains an entry">Otherwise, this instruction checks if cursor P1 contains an entry</param>
						///<param name="where the first (N">1) fields match but the rowid value at the end</param>
						///<param name="of the index entry is not R. If there is no such entry, control jumps">of the index entry is not R. If there is no such entry, control jumps</param>
						///<param name="to instruction P2. Otherwise, the rowid of the conflicting index">to instruction P2. Otherwise, the rowid of the conflicting index</param>
						///<param name="entry is copied to register P3 and control falls through to the next">entry is copied to register P3 and control falls through to the next</param>
						///<param name="instruction.">instruction.</param>
						///<param name=""></param>
						///<param name="See also: NotFound, NotExists, Found">See also: NotFound, NotExists, Found</param>
						///<param name=""></param>
						case OpCode.OP_IsUnique: {
							///
							///<summary>
							///jump, in3 
							///</summary>
							u16 ii;
							VdbeCursor pCx=new VdbeCursor();
							BtCursor pCrsr;
							u16 nField;
							Mem[] aMx;
							UnpackedRecord r;
							///
							///<summary>
							///</summary>
							///<param name="B">Tree index search key </param>
							i64 R;
							///
							///<summary>
							///Rowid stored in register P3 
							///</summary>
							r=new UnpackedRecord();
							pIn3=aMem[pOp.p3];
							//aMx = aMem[pOp->p4.i];
							///
							///<summary>
							///Assert that the values of parameters P1 and P4 are in range. 
							///</summary>
							Debug.Assert(pOp.p4type== P4Usage.P4_INT32);
							Debug.Assert(pOp.p4.i>0&&pOp.p4.i<=this.nMem);
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							///
							///<summary>
							///Find the index cursor. 
							///</summary>
							pCx=this.apCsr[pOp.p1];
							Debug.Assert(!pCx.deferredMoveto);
							pCx.seekResult=0;
							pCx.cacheStatus=CACHE_STALE;
							pCrsr=pCx.pCursor;
							///
							///<summary>
							///If any of the values are NULL, take the jump. 
							///</summary>
							nField=pCx.pKeyInfo.nField;
							aMx=new Mem[nField+1];
							for(ii=0;ii<nField;ii++) {
								aMx[ii]=aMem[pOp.p4.i+ii];
								if((aMx[ii].flags&MemFlags.MEM_Null)!=0) {
									opcodeIndex=pOp.p2-1;
									pCrsr=null;
									break;
								}
							}
							aMx[nField]=new Mem();
							//Debug.Assert( ( aMx[nField].flags & MEM.MEM_Null ) == 0 );
							if(pCrsr!=null) {
								///
								///<summary>
								///Populate the index search key. 
								///</summary>
								r.pKeyInfo=pCx.pKeyInfo;
								r.nField=(ushort)(nField+1);
								r.flags=UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH;
								r.aMem=aMx;
								#if SQLITE_DEBUG
																																																																																																																																																													                {
                  int i;
                  for ( i = 0; i < r.nField; i++ )
                    Debug.Assert( memIsValid( r.aMem[i] ) );
                }
#endif
								///
								///<summary>
								///Extract the value of R from register P3. 
								///</summary>
                                
								pIn3.sqlite3VdbeMemIntegerify();
								R=pIn3.u.i;
								///
								///<summary>
								///</summary>
								///<param name="Search the B">Tree index. If no conflicting record is found, jump</param>
								///<param name="to P2. Otherwise, copy the rowid of the conflicting record to">to P2. Otherwise, copy the rowid of the conflicting record to</param>
								///<param name="register P3 and fall through to the next instruction.  ">register P3 and fall through to the next instruction.  </param>
								rc=pCrsr.sqlite3BtreeMovetoUnpacked(r,0,0,ref pCx.seekResult);
								if((r.flags&UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH)!=0||r.rowid==R) {
									opcodeIndex=pOp.p2-1;
								}
								else {
									pIn3.u.i=r.rowid;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: NotExists P1 P2 P3 * *
						///
						///Use the content of register P3 as an integer key.  If a record
						///with that key does not exist in table of P1, then jump to P2.
						///If the record does exist, then fall through.  The cursor is left
						///pointing to the record if it exists.
						///
						///The difference between this operation and NotFound is that this
						///operation assumes the key is an integer and that P1 is a table whereas
						///NotFound assumes key is a blob constructed from MakeRecord and
						///P1 is an index.
						///
						///See also: Found, NotFound, IsUnique
						///
						///</summary>
						case OpCode.OP_NotExists: {
							///
							///<summary>
							///jump, in3 
							///</summary>
							VdbeCursor pC;
							BtCursor pCrsr;
							int res;
							i64 iKey;
							pIn3=aMem[pOp.p3];
							Debug.Assert((pIn3.flags&MemFlags.MEM_Int)!=0);
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.isTable);
							Debug.Assert(pC.pseudoTableReg==0);
							pCrsr=pC.pCursor;
							if(pCrsr!=null) {
								res=0;
								iKey=pIn3.u.i;
								rc=pCrsr.sqlite3BtreeMovetoUnpacked(null,(long)iKey,0,ref res);
								pC.lastRowid=pIn3.u.i;
								pC.rowidIsValid=res==0?true:false;
								pC.nullRow=false;
								pC.cacheStatus=CACHE_STALE;
								pC.deferredMoveto=false;
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
									Debug.Assert(!pC.rowidIsValid);
								}
								pC.seekResult=res;
							}
							else {
								///
								///<summary>
								///This happens when an attempt to open a read cursor on the
								///sqlite_master table returns SQLITE_EMPTY.
								///
								///</summary>
								opcodeIndex=pOp.p2-1;
								Debug.Assert(!pC.rowidIsValid);
								pC.seekResult=0;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Sequence P1 P2 * * *
						///
						///Find the next available sequence number for cursor P1.
						///Write the sequence number into register P2.
						///The sequence number on the cursor is incremented after this
						///instruction.
						///
						///</summary>
						case OpCode.OP_Sequence: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(this.apCsr[pOp.p1]!=null);
							pOut.u.i=(long)this.apCsr[pOp.p1].seqCount++;
							break;
						}
						///
						///<summary>
						///Opcode: NewRowid P1 P2 P3 * *
						///
						///Get a new integer record number (a.k.a "rowid") used as the key to a table.
						///The record number is not previously used as a key in the database
						///table that cursor P1 points to.  The new record number is written
						///written to register P2.
						///
						///If P3>0 then P3 is a register in the root frame of this VDBE that holds 
						///the largest previously generated record number. No new record numbers are
						///allowed to be less than this value. When this value reaches its maximum, 
						///an SQLITE_FULL error is generated. The P3 register is updated with the '
						///generated record number. This P3 mechanism is used to help implement the
						///AUTOINCREMENT feature.
						///
						///</summary>
						case OpCode.OP_NewRowid:
						#region generate rowid
						 {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							i64 v;
							///
							///<summary>
							///The new rowid 
							///</summary>
							VdbeCursor pC;
							///
							///<summary>
							///Cursor of table to get the new rowid 
							///</summary>
							int res;
							///
							///<summary>
							///Result of an sqlite3BtreeLast() 
							///</summary>
							int cnt;
							///
							///<summary>
							///Counter to limit the number of searches 
							///</summary>
							Mem pMem;
							///
							///<summary>
							///Register holding largest rowid for AUTOINCREMENT 
							///</summary>
							VdbeFrame rootFrame;
							///
							///<summary>
							///Root frame of VDBE 
							///</summary>
							v=0;
							res=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							if(NEVER(pC.pCursor==null)) {
								///
								///<summary>
								///The zero initialization above is all that is needed 
								///</summary>
							}
							else {
								///
								///<summary>
								///The next rowid or record number (different terms for the same
								///</summary>
								///<param name="thing) is obtained in a two">step algorithm.</param>
								///<param name=""></param>
								///<param name="First we attempt to find the largest existing rowid and add one">First we attempt to find the largest existing rowid and add one</param>
								///<param name="to that.  But if the largest existing rowid is already the maximum">to that.  But if the largest existing rowid is already the maximum</param>
								///<param name="positive integer, we have to fall through to the second">positive integer, we have to fall through to the second</param>
								///<param name="probabilistic algorithm">probabilistic algorithm</param>
								///<param name=""></param>
								///<param name="The second algorithm is to select a rowid at random and see if">The second algorithm is to select a rowid at random and see if</param>
								///<param name="it already exists in the table.  If it does not exist, we have">it already exists in the table.  If it does not exist, we have</param>
								///<param name="succeeded.  If the random rowid does exist, we select a new one">succeeded.  If the random rowid does exist, we select a new one</param>
								///<param name="and try again, up to 100 times.">and try again, up to 100 times.</param>
								///<param name=""></param>
								Debug.Assert(pC.isTable);
								#if SQLITE_32BIT_ROWID
																																																																																																																																																													const int MAX_ROWID = i32.MaxValue;//   define MAX_ROWID 0x7fffffff
#else
								///
								///<summary>
								///Some compilers complain about constants of the form 0x7fffffffffffffff.
								///Others complain about 0x7ffffffffffffffffLL.  The following macro seems
								///to provide the constant while making all compilers happy.
								///</summary>
								const long MAX_ROWID=i64.MaxValue;
								// (i64)( (((u64)0x7fffffff)<<32) | (u64)0xffffffff )
								#endif
								if(!pC.useRandomRowid) {
									v=pC.pCursor.sqlite3BtreeGetCachedRowid();
									if(v==0) {
										rc=pC.pCursor.sqlite3BtreeLast(ref res);
										if(rc!=Sqlite3.SQLITE_OK) {
											goto abort_due_to_error;
										}
										if(res!=0) {
											v=1;
											///
											///<summary>
											///</summary>
											///<param name="IMP: R">48074 </param>
										}
										else {
											Debug.Assert(pC.pCursor.sqlite3BtreeCursorIsValid());
											rc=pC.pCursor.sqlite3BtreeKeySize(ref v);
											Debug.Assert(rc==Sqlite3.SQLITE_OK);
											///
											///<summary>
											///Cannot fail following BtreeLast() 
											///</summary>
											if(v==MAX_ROWID) {
												pC.useRandomRowid=true;
											}
											else {
												v++;
												///
												///<summary>
												///</summary>
												///<param name="IMP: R">34987 </param>
											}
										}
									}
									#if !SQLITE_OMIT_AUTOINCREMENT
									if(pOp.p3!=0) {
										///
										///<summary>
										///Assert that P3 is a valid memory cell. 
										///</summary>
										Debug.Assert(pOp.p3>0);
										if(this.pFrame!=null) {
											rootFrame=this.pFrame.GetRoot();
											///
											///<summary>
											///Assert that P3 is a valid memory cell. 
											///</summary>
											Debug.Assert(pOp.p3<=rootFrame.nMem);
											pMem=rootFrame.aMem[pOp.p3];
										}
										else {
											///
											///<summary>
											///Assert that P3 is a valid memory cell. 
											///</summary>
											Debug.Assert(pOp.p3<=this.nMem);
											pMem=aMem[pOp.p3];
											memAboutToChange(this,pMem);
										}
										Debug.Assert(pMem.memIsValid());
										REGISTER_TRACE(this,pOp.p3,pMem);
										pMem.sqlite3VdbeMemIntegerify();
										Debug.Assert((pMem.flags&MemFlags.MEM_Int)!=0);
										///
										///<summary>
										///mem(P3) holds an integer 
										///</summary>
										if(pMem.u.i==MAX_ROWID||pC.useRandomRowid) {
											rc=SQLITE_FULL;
											///
											///<summary>
											///</summary>
											///<param name="IMP: R">61338 </param>
											goto abort_due_to_error;
										}
										if(v<(pMem.u.i+1)) {
											v=(int)(pMem.u.i+1);
										}
										pMem.u.i=(long)v;
									}
									#endif
									pC.pCursor.sqlite3BtreeSetCachedRowid(v<MAX_ROWID?v+1:0);
								}
								if(pC.useRandomRowid) {
									///
									///<summary>
									///</summary>
									///<param name="IMPLEMENTATION">41881 If the largest ROWID is equal to the</param>
									///<param name="largest possible integer (9223372036854775807) then the database">largest possible integer (9223372036854775807) then the database</param>
									///<param name="engine starts picking positive candidate ROWIDs at random until">engine starts picking positive candidate ROWIDs at random until</param>
									///<param name="it finds one that is not previously used. ">it finds one that is not previously used. </param>
									Debug.Assert(pOp.p3==0);
									///
									///<summary>
									///We cannot be in random rowid mode if this is
									///an AUTOINCREMENT table. 
									///</summary>
									///
									///<summary>
									///on the first attempt, simply do one more than previous 
									///</summary>
									v=lastRowid;
									v&=(MAX_ROWID>>1);
									///
									///<summary>
									///ensure doesn't go negative 
									///</summary>
									v++;
									///
									///<summary>
									///</summary>
									///<param name="ensure non">zero </param>
									cnt=0;
									while(((rc=pC.pCursor.sqlite3BtreeMovetoUnpacked(null,v,0,ref res))==Sqlite3.SQLITE_OK)&&(res==0)&&(++cnt<100)) {
										///
										///<summary>
										///</summary>
										///<param name="collision "> try another random rowid </param>
										sqlite3_randomness(sizeof(i64),ref v);
										if(cnt<5) {
											///
											///<summary>
											///try "small" random rowids for the initial attempts 
											///</summary>
											v&=0xffffff;
										}
										else {
											v&=(MAX_ROWID>>1);
											///
											///<summary>
											///ensure doesn't go negative 
											///</summary>
										}
										v++;
										///
										///<summary>
										///</summary>
										///<param name="ensure non">zero </param>
									}
									if(rc==Sqlite3.SQLITE_OK&&res==0) {
										rc=SQLITE_FULL;
										///
										///<summary>
										///</summary>
										///<param name="IMP: R">53002 </param>
										goto abort_due_to_error;
									}
									Debug.Assert(v>0);
									///
									///<summary>
									///</summary>
									///<param name="EV: R">03570 </param>
								}
								pC.rowidIsValid=false;
								pC.deferredMoveto=false;
								pC.cacheStatus=CACHE_STALE;
							}
							pOut.u.i=(long)v;
							break;
						}
						#endregion
						///
						///<summary>
						///Opcode: Insert P1 P2 P3 P4 P5
						///
						///Write an entry into the table of cursor P1.  A new entry is
						///created if it doesn't already exist or the data for an existing
						///entry is overwritten.  The data is the value MEM.MEM_Blob stored in register
						///number P2. The key is stored in register P3. The key must
						///be a MEM.MEM_Int.
						///
						///If the OPFLAG_NCHANGE flag of P5 is set, then the row change count is
						///incremented (otherwise not).  If the OPFLAG_LASTROWID flag of P5 is set,
						///then rowid is stored for subsequent return by the
						///sqlite3_last_insert_rowid() function (otherwise it is unmodified).
						///
						///If the OPFLAG_USESEEKRESULT flag of P5 is set and if the result of
						///the last seek operation ( OpCode.OP_NotExists) was a success, then this
						///operation will not attempt to find the appropriate row before doing
						///the insert but will instead overwrite the row that the cursor is
						///currently pointing to.  Presumably, the prior  OpCode.OP_NotExists opcode
						///has already positioned the cursor correctly.  This is an optimization
						///that boosts performance by avoiding redundant seeks.
						///
						///If the OPFLAG_ISUPDATE flag is set, then this opcode is part of an
						///UPDATE operation.  Otherwise (if the flag is clear) then this opcode
						///is part of an INSERT operation.  The difference is only important to
						///the update hook.
						///
						///</summary>
						///<param name="Parameter P4 may point to a string containing the table">name, or</param>
						///<param name="may be NULL. If it is not NULL, then the update">hook </param>
						///<param name="(sqlite3.xUpdateCallback) is invoked following a successful insert.">(sqlite3.xUpdateCallback) is invoked following a successful insert.</param>
						///<param name=""></param>
						///<param name="(WARNING/TODO: If P1 is a pseudo">cursor and P2 is dynamically</param>
						///<param name="allocated, then ownership of P2 is transferred to the pseudo">cursor</param>
						///<param name="and register P2 becomes ephemeral.  If the cursor is changed, the">and register P2 becomes ephemeral.  If the cursor is changed, the</param>
						///<param name="value of register P2 will then change.  Make sure this does not">value of register P2 will then change.  Make sure this does not</param>
						///<param name="cause any problems.)">cause any problems.)</param>
						///<param name=""></param>
						///<param name="This instruction only works on tables.  The equivalent instruction">This instruction only works on tables.  The equivalent instruction</param>
						///<param name="for indices is  OpCode.OP_IdxInsert.">for indices is  OpCode.OP_IdxInsert.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: InsertInt P1 P2 P3 P4 P5
						///
						///This works exactly like  OpCode.OP_Insert except that the key is the
						///integer value P3, not the value of the integer stored in register P3.
						///
						///</summary>
						case OpCode.OP_Insert:
						case OpCode.OP_InsertInt: {
							Mem pData;
							///
							///<summary>
							///MEM cell holding data for the record to be inserted 
							///</summary>
							Mem pKey;
							///
							///<summary>
							///MEM cell holding key  for the record 
							///</summary>
							i64 iKey;
							///
							///<summary>
							///The integer ROWID or key for the record to be inserted 
							///</summary>
							VdbeCursor pC;
							///
							///<summary>
							///Cursor to table into which insert is written 
							///</summary>
							int nZero;
							///
							///<summary>
							///</summary>
							///<param name="Number of zero">bytes to append </param>
							int seekResult;
							///
							///<summary>
							///Result of prior seek or 0 if no USESEEKRESULT flag 
							///</summary>
							string zDb;
							///
							///<summary>
							///</summary>
							///<param name="database name "> used by the update hook </param>
							string zTbl;
							///
							///<summary>
							///</summary>
							///<param name="Table name "> used by the opdate hook </param>
							int op;
							///
							///<summary>
							///Opcode for update hook: SQLITE_UPDATE or SQLITE_INSERT 
							///</summary>
							pData=aMem[pOp.p2];
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pData.memIsValid());
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pCursor!=null);
							Debug.Assert(pC.pseudoTableReg==0);
							Debug.Assert(pC.isTable);
							REGISTER_TRACE(this,pOp.p2,pData);
							if(pOp.OpCode==OpCode.OP_Insert) {
								pKey=aMem[pOp.p3];
								Debug.Assert((pKey.flags&MemFlags.MEM_Int)!=0);
								Debug.Assert(pKey.memIsValid());
								REGISTER_TRACE(this,pOp.p3,pKey);
								iKey=pKey.u.i;
							}
							else {
								Debug.Assert(pOp.OpCode==OpCode.OP_InsertInt);
								iKey=pOp.p3;
							}
                            if (((OpFlag)pOp.p5 & OpFlag.OPFLAG_NCHANGE) != 0)
								this.nChange++;
                            if (((OpFlag)pOp.p5 & OpFlag.OPFLAG_LASTROWID) != 0)
								db.lastRowid=lastRowid=iKey;
							if((pData.flags&MemFlags.MEM_Null)!=0) {
								malloc_cs.sqlite3_free(ref pData.zBLOB);
								pData.z=null;
								pData.n=0;
							}
							else {
								Debug.Assert((pData.flags&(MemFlags.MEM_Blob|MemFlags.MEM_Str))!=0);
							}
							seekResult=(((OpFlag)pOp.p5&OpFlag.OPFLAG_USESEEKRESULT)!=0?pC.seekResult:0);
							if((pData.flags&MemFlags.MEM_Zero)!=0) {
								nZero=pData.u.nZero;
							}
							else {
								nZero=0;
							}
                            rc = pC.pCursor.sqlite3BtreeInsert(null, iKey, pData.zBLOB, pData.n, nZero, ((OpFlag)pOp.p5 & OpFlag.OPFLAG_APPEND) != 0 ? 1 : 0, seekResult);
							pC.rowidIsValid=false;
							pC.deferredMoveto=false;
							pC.cacheStatus=CACHE_STALE;
							///
							///<summary>
							///</summary>
							///<param name="Invoke the update">hook if required. </param>
							if(rc==Sqlite3.SQLITE_OK&&db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								zDb=db.aDb[pC.iDb].zName;
								zTbl=pOp.p4.z;
                                op = ((
                                    ((OpFlag)pOp.p5)
                                    .Has(OpFlag.OPFLAG_ISUPDATE) 
                                    ? SQLITE_UPDATE : SQLITE_INSERT
                                    ));
								Debug.Assert(pC.isTable);
								db.xUpdateCallback(db.pUpdateArg,op,zDb,zTbl,iKey);
								Debug.Assert(pC.iDb>=0);
							}
							break;
						}
						///
						///<summary>
						///Opcode: Delete P1 P2 * P4 *
						///
						///Delete the record at which the P1 cursor is currently pointing.
						///
						///The cursor will be left pointing at either the next or the previous
						///record in the table. If it is left pointing at the next record, then
						///</summary>
						///<param name="the next Next instruction will be a no">op.  Hence it is OK to delete</param>
						///<param name="a record from within an Next loop.">a record from within an Next loop.</param>
						///<param name=""></param>
						///<param name="If the OPFLAG_NCHANGE flag of P2 is set, then the row change count is">If the OPFLAG_NCHANGE flag of P2 is set, then the row change count is</param>
						///<param name="incremented (otherwise not).">incremented (otherwise not).</param>
						///<param name=""></param>
						///<param name="P1 must not be pseudo">table.  It has to be a real table with</param>
						///<param name="multiple rows.">multiple rows.</param>
						///<param name=""></param>
						///<param name="If P4 is not NULL, then it is the name of the table that P1 is">If P4 is not NULL, then it is the name of the table that P1 is</param>
						///<param name="pointing to.  The update hook will be invoked, if it exists.">pointing to.  The update hook will be invoked, if it exists.</param>
						///<param name="If P4 is not NULL then the P1 cursor must have been positioned">If P4 is not NULL then the P1 cursor must have been positioned</param>
						///<param name="using  OpCode.OP_NotFound prior to invoking this opcode.">using  OpCode.OP_NotFound prior to invoking this opcode.</param>
						///<param name=""></param>
						case OpCode.OP_Delete: {
							i64 iKey;
							VdbeCursor pC;
							iKey=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pCursor!=null);
							///
							///<summary>
							///Only valid for real tables, no pseudotables 
							///</summary>
							///
							///<summary>
							///</summary>
							///<param name="If the update">hook will be invoked, set iKey to the rowid of the</param>
							///<param name="row being deleted.">row being deleted.</param>
							if(db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								Debug.Assert(pC.isTable);
								Debug.Assert(pC.rowidIsValid);
								///
								///<summary>
								///lastRowid set by previous  OpCode.OP_NotFound 
								///</summary>
								iKey=pC.lastRowid;
							}
							///
							///<summary>
							///The  OpCode.OP_Delete opcode always follows an  OpCode.OP_NotExists or  OpCode.OP_Last or
							///OP_Column on the same table without any intervening operations that
							///might move or invalidate the cursor.  Hence cursor pC is always pointing
							///to the row to be deleted and the sqlite3VdbeCursorMoveto() operation
							///</summary>
							///<param name="below is always a no">op and cannot fail.  We will run it anyhow, though,</param>
							///<param name="to guard against future changes to the code generator.">to guard against future changes to the code generator.</param>
							///<param name=""></param>
							Debug.Assert(pC.deferredMoveto==false);
                            rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
							if(NEVER(rc!=Sqlite3.SQLITE_OK))
								goto abort_due_to_error;
							pC.pCursor.sqlite3BtreeSetCachedRowid(0);
							rc=pC.pCursor.sqlite3BtreeDelete();
							pC.cacheStatus=CACHE_STALE;
							///
							///<summary>
							///</summary>
							///<param name="Invoke the update">hook if required. </param>
							if(rc==Sqlite3.SQLITE_OK&&db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								string zDb=db.aDb[pC.iDb].zName;
								string zTbl=pOp.p4.z;
								db.xUpdateCallback(db.pUpdateArg,SQLITE_DELETE,zDb,zTbl,iKey);
								Debug.Assert(pC.iDb>=0);
							}
                            if ((pOp.p2 & (int)OpFlag.OPFLAG_NCHANGE) != 0)
								this.nChange++;
							break;
						}
						///
						///<summary>
						///Opcode: ResetCount P1 * *
						///
						///The value of the change counter is copied to the database handle
						///change counter (returned by subsequent calls to sqlite3_changes()).
						///Then the VMs internal change counter resets to 0.
						///This is used by trigger programs.
						///
						///</summary>
						case OpCode.OP_ResetCount: {
                            vdbeaux.sqlite3VdbeSetChanges(db, this.nChange);
							this.nChange=0;
							break;
						}
						///
						///<summary>
						///Opcode: RowData P1 P2 * * *
						///
						///Write into register P2 the complete row data for cursor P1.
						///There is no interpretation of the data.
						///It is just copied onto the P2 register exactly as
						///it is found in the database file.
						///
						///If the P1 cursor must be pointing to a valid row (not a NULL row)
						///</summary>
						///<param name="of a real table, not a pseudo">table.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: RowKey P1 P2 * * *
						///
						///Write into register P2 the complete row key for cursor P1.
						///There is no interpretation of the data.
						///The key is copied onto the P3 register exactly as
						///it is found in the database file.
						///
						///If the P1 cursor must be pointing to a valid row (not a NULL row)
						///</summary>
						///<param name="of a real table, not a pseudo">table.</param>
						///<param name=""></param>
						case OpCode.OP_RowKey:
						case OpCode.OP_RowData: {
							VdbeCursor pC;
							BtCursor pCrsr;
							u32 n;
							i64 n64;
							n=0;
							n64=0;
							pOut=aMem[pOp.p2];
							memAboutToChange(this,pOut);
							///
							///<summary>
							///Note that RowKey and RowData are really exactly the same instruction 
							///</summary>
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC.isTable||pOp.OpCode==OpCode.OP_RowKey);
							Debug.Assert(pC.isIndex||pOp.OpCode==OpCode.OP_RowData);
							Debug.Assert(pC!=null);
							Debug.Assert(pC.nullRow==false);
							Debug.Assert(pC.pseudoTableReg==0);
							Debug.Assert(pC.pCursor!=null);
							pCrsr=pC.pCursor;
							Debug.Assert(pCrsr.sqlite3BtreeCursorIsValid());
							///
							///<summary>
							///The  OpCode.OP_RowKey and  OpCode.OP_RowData opcodes always follow  OpCode.OP_NotExists or
							///OP_Rewind/Op_Next with no intervening instructions that might invalidate
							///the cursor.  Hence the following sqlite3VdbeCursorMoveto() call is always
							///</summary>
							///<param name="a no">op and can never fail.  But we leave it in place as a safety.</param>
							///<param name=""></param>
							Debug.Assert(pC.deferredMoveto==false);
                            rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
							if(NEVER(rc!=Sqlite3.SQLITE_OK))
								goto abort_due_to_error;
							if(pC.isIndex) {
								Debug.Assert(!pC.isTable);
								rc=pCrsr.sqlite3BtreeKeySize(ref n64);
								Debug.Assert(rc==Sqlite3.SQLITE_OK);
								///
								///<summary>
								///True because of CursorMoveto() call above 
								///</summary>
								if(n64>db.aLimit[SQLITE_LIMIT_LENGTH]) {
									goto too_big;
								}
								n=(u32)n64;
							}
							else {
								rc=pCrsr.sqlite3BtreeDataSize(ref n);
								Debug.Assert(rc==Sqlite3.SQLITE_OK);
								///
								///<summary>
								///DataSize() cannot fail 
								///</summary>
								if(n>(u32)db.aLimit[SQLITE_LIMIT_LENGTH]) {
									goto too_big;
								}
                                if (vdbemem_cs.sqlite3VdbeMemGrow(pOut, (int)n, 0) != 0)
                                {
									goto no_mem;
								}
							}
							pOut.n=(int)n;
							if(pC.isIndex) {
								pOut.zBLOB=malloc_cs.sqlite3Malloc((int)n);
								rc=pCrsr.sqlite3BtreeKey(0,n,pOut.zBLOB);
							}
							else {
								pOut.zBLOB=malloc_cs.sqlite3Malloc((int)pCrsr.info.nData);
								rc=pCrsr.sqlite3BtreeData(0,(u32)n,pOut.zBLOB);
							}
                            
							pOut.MemSetTypeFlag(MemFlags.MEM_Blob);
							pOut.enc=SqliteEncoding.UTF8;
							///
							///<summary>
							///In case the blob is ever cast to text 
							///</summary>
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						///
						///<summary>
						///Opcode: Rowid P1 P2 * * *
						///
						///Store in register P2 an integer which is the key of the table entry that
						///P1 is currently point to.
						///
						///P1 can be either an ordinary table or a virtual table.  There used to
						///be a separate  OpCode.OP_VRowid opcode for use with virtual tables, but this
						///one opcode now works for both table types.
						///
						///</summary>
						case OpCode.OP_Rowid: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							VdbeCursor pC;
							i64 v;
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							v=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pseudoTableReg==0);
							if(pC.nullRow) {
								pOut.flags=MemFlags.MEM_Null;
								break;
							}
							else
								if(pC.deferredMoveto) {
									v=pC.movetoTarget;
									#if !SQLITE_OMIT_VIRTUALTABLE
								}
								else
									if(pC.pVtabCursor!=null) {
										pVtab=pC.pVtabCursor.pVtab;
										pModule=pVtab.pModule;
										Debug.Assert(pModule.xRowid!=null);
										rc=pModule.xRowid(pC.pVtabCursor,out v);
										importVtabErrMsg(this,pVtab);
										#endif
									}
									else {
										Debug.Assert(pC.pCursor!=null);
                                        rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
										if(rc!=0)
											goto abort_due_to_error;
										if(pC.rowidIsValid) {
											v=pC.lastRowid;
										}
										else {
											rc=pC.pCursor.sqlite3BtreeKeySize(ref v);
											Debug.Assert(rc==Sqlite3.SQLITE_OK);
											///
											///<summary>
											///Always so because of CursorMoveto() above 
											///</summary>
										}
									}
							pOut.u.i=(long)v;
							break;
						}
						///
						///<summary>
						///Opcode: NullRow P1 * * * *
						///
						///Move the cursor P1 to a null row.  Any  OpCode.OP_Column operations
						///that occur while the cursor is on the null row will always
						///write a NULL.
						///
						///</summary>
						case OpCode.OP_NullRow: {
							VdbeCursor pC;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pC.nullRow=true;
							pC.rowidIsValid=false;
							if(pC.pCursor!=null) {
								pC.pCursor.sqlite3BtreeClearCursor();
							}
							break;
						}
						///
						///<summary>
						///Opcode: Last P1 P2 * * *
						///
						///The next use of the Rowid or Column or Next instruction for P1
						///will refer to the last entry in the database table or index.
						///If the table or index is empty and P2>0, then jump immediately to P2.
						///If P2 is 0 or if the table or index is not empty, fall through
						///to the following instruction.
						///
						///</summary>
						case OpCode.OP_Last: {
							///
							///<summary>
							///jump 
							///</summary>
							VdbeCursor pC;
							BtCursor pCrsr;
							int res=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pCrsr=pC.pCursor;
							if(pCrsr==null) {
								res=1;
							}
							else {
								rc=pCrsr.sqlite3BtreeLast(ref res);
							}
							pC.nullRow=res==1?true:false;
							pC.deferredMoveto=false;
							pC.rowidIsValid=false;
							pC.cacheStatus=CACHE_STALE;
							if(pOp.p2>0&&res!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Sort P1 P2 * * *
						///
						///This opcode does exactly the same thing as  OpCode.OP_Rewind except that
						///it increments an undocumented global variable used for testing.
						///
						///Sorting is accomplished by writing records into a sorting index,
						///then rewinding that index and playing it back from beginning to
						///end.  We use the  OpCode.OP_Sort opcode instead of  OpCode.OP_Rewind to do the
						///rewinding so that the global variable will be incremented and
						///regression tests can determine whether or not the optimizer is
						///correctly optimizing out sorts.
						///
						///</summary>
						case OpCode.OP_Sort: {
							///
							///<summary>
							///jump 
							///</summary>
							#if SQLITE_TEST
														#if !TCLSH
																																																																																																																																				              sqlite3_sort_count++;
              sqlite3_search_count--;
#else
																																																																																																																																				              sqlite3_sort_count.iValue++;
              sqlite3_search_count.iValue--;
#endif
														#endif
							this.aCounter[SQLITE_STMTSTATUS_SORT-1]++;
							///
							///<summary>
							///Fall through into  OpCode.OP_Rewind 
							///</summary>
							goto case OpCode.OP_Rewind;
						}
						///
						///<summary>
						///Opcode: Rewind P1 P2 * * *
						///
						///The next use of the Rowid or Column or Next instruction for P1
						///will refer to the first entry in the database table or index.
						///If the table or index is empty and P2>0, then jump immediately to P2.
						///If P2 is 0 or if the table or index is not empty, fall through
						///to the following instruction.
						///
						///</summary>
						case OpCode.OP_Rewind: {
							///
							///<summary>
							///jump 
							///</summary>
							VdbeCursor pC;
							BtCursor pCrsr;
							int res=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							res=1;
							if((pCrsr=pC.pCursor)!=null) {
								rc=pCrsr.sqlite3BtreeFirst(ref res);
								pC.atFirst=res==0?true:false;
								pC.deferredMoveto=false;
								pC.cacheStatus=CACHE_STALE;
								pC.rowidIsValid=false;
							}
							pC.nullRow=res==1?true:false;
							Debug.Assert(pOp.p2>0&&pOp.p2<this.nOp);
							if(res!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: Next P1 P2 * * P5
						///
						///Advance cursor P1 so that it points to the next key/data pair in its
						///table or index.  If there are no more key/value pairs then fall through
						///to the following instruction.  But if the cursor advance was successful,
						///jump immediately to P2.
						///
						///</summary>
						///<param name="The P1 cursor must be for a real table, not a pseudo">table.</param>
						///<param name=""></param>
						///<param name="See also: Prev">See also: Prev</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: Prev P1 P2 * * *
						///
						///Back up cursor P1 so that it points to the previous key/data pair in its
						///table or index.  If there is no previous key/value pairs then fall through
						///to the following instruction.  But if the cursor backup was successful,
						///jump immediately to P2.
						///
						///</summary>
						///<param name="The P1 cursor must be for a real table, not a pseudo">table.</param>
						///<param name=""></param>
						///<param name="If P5 is positive and the jump is taken, then event counter">If P5 is positive and the jump is taken, then event counter</param>
						///<param name="number P5">1 in the prepared statement is incremented.</param>
						///<param name=""></param>
						///<param name=""></param>
						case OpCode.OP_Prev:
						///
						///<summary>
						///jump 
						///</summary>
						case OpCode.OP_Next: {
							///
							///<summary>
							///jump 
							///</summary>
							VdbeCursor pC;
							BtCursor pCrsr;
							int res;
							if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pOp.p5<=Sqlite3.ArraySize(this.aCounter));
							pC=this.apCsr[pOp.p1];
							if(pC==null) {
								break;
								///
								///<summary>
								///See ticket #2273 
								///</summary>
							}
							pCrsr=pC.pCursor;
							if(pCrsr==null) {
								pC.nullRow=true;
								break;
							}
							res=1;
							Debug.Assert(!pC.deferredMoveto);
							rc=pOp.OpCode==OpCode.OP_Next?pCrsr.sqlite3BtreeNext(ref res):pCrsr.sqlite3BtreePrevious(ref res);
							pC.nullRow=res==1?true:false;
							pC.cacheStatus=CACHE_STALE;
							if(res==0) {
								opcodeIndex=pOp.p2-1;
								if(pOp.p5!=0)
									this.aCounter[pOp.p5-1]++;
								#if SQLITE_TEST
																#if !TCLSH
																																																																																																																																																													                sqlite3_search_count++;
#else
																																																																																																																																																													                sqlite3_search_count.iValue++;
#endif
																#endif
							}
							pC.rowidIsValid=false;
							break;
						}
						///
						///<summary>
						///Opcode: IdxInsert P1 P2 P3 * P5
						///
						///Register P2 holds an SQL index key made using the
						///MakeRecord instructions.  This opcode writes that key
						///into the index P1.  Data for the entry is nil.
						///
						///</summary>
						///<param name="P3 is a flag that provides a hint to the b">tree layer that this</param>
						///<param name="insert is likely to be an append.">insert is likely to be an append.</param>
						///<param name=""></param>
						///<param name="This instruction only works for indices.  The equivalent instruction">This instruction only works for indices.  The equivalent instruction</param>
						///<param name="for tables is  OpCode.OP_Insert.">for tables is  OpCode.OP_Insert.</param>
						///<param name=""></param>
						case OpCode.OP_IdxInsert: {
							///
							///<summary>
							///in2 
							///</summary>
							VdbeCursor pC;
							BtCursor pCrsr;
							int nKey;
							byte[] zKey;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pIn2=aMem[pOp.p2];
							Debug.Assert((pIn2.flags&MemFlags.MEM_Blob)!=0);
							pCrsr=pC.pCursor;
							if(Sqlite3.ALWAYS(pCrsr!=null)) {
								Debug.Assert(!pC.isTable);
								ExpandBlob(pIn2);
								if(rc==Sqlite3.SQLITE_OK) {
									nKey=pIn2.n;
									zKey=(pIn2.flags&MemFlags.MEM_Blob)!=0?pIn2.zBLOB:Encoding.UTF8.GetBytes(pIn2.z);
                                    rc = pCrsr.sqlite3BtreeInsert(zKey, nKey, null, 0, 0, (pOp.p3 != 0) ? 1 : 0, (((OpFlag)pOp.p5 & OpFlag.OPFLAG_USESEEKRESULT) != 0 ? pC.seekResult : 0));
									Debug.Assert(!pC.deferredMoveto);
									pC.cacheStatus=CACHE_STALE;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: IdxDelete P1 P2 P3 * *
						///
						///The content of P3 registers starting at register P2 form
						///an unpacked index key. This opcode removes that entry from the
						///index opened by cursor P1.
						///
						///</summary>
						case OpCode.OP_IdxDelete: {
							VdbeCursor pC;
							BtCursor pCrsr;
							int res;
							UnpackedRecord r;
							res=0;
							r=new UnpackedRecord();
							Debug.Assert(pOp.p3>0);
							Debug.Assert(pOp.p2>0&&pOp.p2+pOp.p3<=this.nMem+1);
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pCrsr=pC.pCursor;
							if(Sqlite3.ALWAYS(pCrsr!=null)) {
								r.pKeyInfo=pC.pKeyInfo;
								r.nField=(u16)pOp.p3;
								r.flags=0;
								r.aMem=new Mem[r.nField];
								for(int ra=0;ra<r.nField;ra++) {
									r.aMem[ra]=aMem[pOp.p2+ra];
									#if SQLITE_DEBUG
																																																																																																																																																																																						                  Debug.Assert( memIsValid( r.aMem[ra] ) );
#endif
								}
								rc=pCrsr.sqlite3BtreeMovetoUnpacked(r,0,0,ref res);
								if(rc==Sqlite3.SQLITE_OK&&res==0) {
									rc=pCrsr.sqlite3BtreeDelete();
								}
								Debug.Assert(!pC.deferredMoveto);
								pC.cacheStatus=CACHE_STALE;
							}
							break;
						}
						///
						///<summary>
						///Opcode: IdxRowid P1 P2 * * *
						///
						///Write into register P2 an integer which is the last entry in the record at
						///the end of the index key pointed to by cursor P1.  This integer should be
						///the rowid of the table entry to which this index entry points.
						///
						///See also: Rowid, MakeRecord.
						///
						///</summary>
						case OpCode.OP_IdxRowid: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							BtCursor pCrsr;
							VdbeCursor pC;
							i64 rowid;
							rowid=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pCrsr=pC.pCursor;
							pOut.flags=MemFlags.MEM_Null;
							if(Sqlite3.ALWAYS(pCrsr!=null)) {
                                rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
								if(NEVER(rc!=0))
									goto abort_due_to_error;
								Debug.Assert(!pC.deferredMoveto);
								Debug.Assert(!pC.isTable);
								if(!pC.nullRow) {
                                    rc = vdbeaux.sqlite3VdbeIdxRowid(db, pCrsr, ref rowid);
									if(rc!=Sqlite3.SQLITE_OK) {
										goto abort_due_to_error;
									}
									pOut.u.i=rowid;
									pOut.flags=MemFlags.MEM_Int;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: IdxGE P1 P2 P3 P4 P5
						///
						///The P4 register values beginning with P3 form an unpacked index
						///key that omits the ROWID.  Compare this key value against the index
						///that P1 is currently pointing to, ignoring the ROWID on the P1 index.
						///
						///If the P1 index entry is greater than or equal to the key value
						///then jump to P2.  Otherwise fall through to the next instruction.
						///
						///</summary>
						///<param name="If P5 is non">zero then the key value is increased by an epsilon</param>
						///<param name="prior to the comparison.  This make the opcode work like IdxGT except">prior to the comparison.  This make the opcode work like IdxGT except</param>
						///<param name="that if the key from register P3 is a prefix of the key in the cursor,">that if the key from register P3 is a prefix of the key in the cursor,</param>
						///<param name="the result is false whereas it would be true with IdxGT.">the result is false whereas it would be true with IdxGT.</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: IdxLT P1 P2 P3 P4 P5
						///
						///The P4 register values beginning with P3 form an unpacked index
						///key that omits the ROWID.  Compare this key value against the index
						///that P1 is currently pointing to, ignoring the ROWID on the P1 index.
						///
						///If the P1 index entry is less than the key value then jump to P2.
						///Otherwise fall through to the next instruction.
						///
						///</summary>
						///<param name="If P5 is non">zero then the key value is increased by an epsilon prior</param>
						///<param name="to the comparison.  This makes the opcode work like IdxLE.">to the comparison.  This makes the opcode work like IdxLE.</param>
						///<param name=""></param>
						case OpCode.OP_IdxLT:
						///
						///<summary>
						///jump 
						///</summary>
						case OpCode.OP_IdxGE: {
							///
							///<summary>
							///jump 
							///</summary>
							VdbeCursor pC;
							int res;
							UnpackedRecord r;
							res=0;
							r=new UnpackedRecord();
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.isOrdered);
							if(Sqlite3.ALWAYS(pC.pCursor!=null)) {
								Debug.Assert(pC.deferredMoveto==false);
								Debug.Assert(pOp.p5==0||pOp.p5==1);
								Debug.Assert(pOp.p4type== P4Usage.P4_INT32);
								r.pKeyInfo=pC.pKeyInfo;
								r.nField=(u16)pOp.p4.i;
								if(pOp.p5!=0) {
									r.flags=UnpackedRecordFlags.UNPACKED_INCRKEY|UnpackedRecordFlags.UNPACKED_IGNORE_ROWID;
								}
								else {
									r.flags=UnpackedRecordFlags.UNPACKED_IGNORE_ROWID;
								}
								r.aMem=new Mem[r.nField];
								for(int rI=0;rI<r.nField;rI++) {
									r.aMem[rI]=aMem[pOp.p3+rI];
									// r.aMem = aMem[pOp.p3];
									#if SQLITE_DEBUG
																																																																																																																																																																																						                  Debug.Assert( memIsValid( r.aMem[rI] ) );
#endif
								}
                                rc = vdbeaux.sqlite3VdbeIdxKeyCompare(pC, r, ref res);
								if(pOp.OpCode==OpCode.OP_IdxLT) {
									res=-res;
								}
								else {
									Debug.Assert(pOp.OpCode==OpCode.OP_IdxGE);
									res++;
								}
								if(res>0) {
									opcodeIndex=pOp.p2-1;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: Destroy P1 P2 P3 * *
						///
						///Delete an entire database table or index whose root page in the database
						///file is given by P1.
						///
						///The table being destroyed is in the main database file if P3==0.  If
						///P3==1 then the table to be clear is in the auxiliary database file
						///that is used to store tables create using CREATE TEMPORARY TABLE.
						///
						///If AUTOVACUUM is enabled then it is possible that another root page
						///might be moved into the newly deleted root page in order to keep all
						///root pages contiguous at the beginning of the database.  The former
						///</summary>
						///<param name="value of the root page that moved "></param>
						///<param name="is stored in register P2.  If no page">is stored in register P2.  If no page</param>
						///<param name="movement was required (because the table being dropped was already">movement was required (because the table being dropped was already</param>
						///<param name="the last one in the database) then a zero is stored in register P2.">the last one in the database) then a zero is stored in register P2.</param>
						///<param name="If AUTOVACUUM is disabled then a zero is stored in register P2.">If AUTOVACUUM is disabled then a zero is stored in register P2.</param>
						///<param name=""></param>
						///<param name="See also: Clear">See also: Clear</param>
						///<param name=""></param>
						case OpCode.OP_Destroy: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							int iMoved=0;
							int iCnt;
							Vdbe pVdbe;
							int iDb;
							#if !SQLITE_OMIT_VIRTUALTABLE
							iCnt=0;
							for(pVdbe=db.pVdbe;pVdbe!=null;pVdbe=pVdbe.pNext) {
								if(pVdbe.magic==VdbeMagic.VDBE_MAGIC_RUN&&pVdbe.inVtabMethod<2&&pVdbe.currentOpCodeIndex>=0) {
									iCnt++;
								}
							}
							#else
																																																																																																																																				              iCnt = db.activeVdbeCnt;
#endif
							pOut.flags=MemFlags.MEM_Null;
							if(iCnt>1) {
								rc=SQLITE_LOCKED;
								this.errorAction=OnConstraintError.OE_Abort;
							}
							else {
								iDb=pOp.p3;
								Debug.Assert(iCnt==1);
								Debug.Assert((this.btreeMask&(((yDbMask)1)<<iDb))!=0);
								rc=db.aDb[iDb].pBt.sqlite3BtreeDropTable(pOp.p1,ref iMoved);
								pOut.flags=MemFlags.MEM_Int;
								pOut.u.i=iMoved;
								#if !SQLITE_OMIT_AUTOVACUUM
								if(rc==Sqlite3.SQLITE_OK&&iMoved!=0) {
									build.sqlite3RootPageMoved(db,iDb,iMoved,pOp.p1);
									///
									///<summary>
									///All  OpCode.OP_Destroy operations occur on the same btree 
									///</summary>
									Debug.Assert(resetSchemaOnFault==0||resetSchemaOnFault==iDb+1);
									resetSchemaOnFault=(u8)(iDb+1);
								}
								#endif
							}
							break;
						}
						///
						///<summary>
						///Opcode: Clear P1 P2 P3
						///
						///Delete all contents of the database table or index whose root page
						///in the database file is given by P1.  But, unlike Destroy, do not
						///remove the table or index from the database file.
						///
						///The table being clear is in the main database file if P2==0.  If
						///P2==1 then the table to be clear is in the auxiliary database file
						///that is used to store tables create using CREATE TEMPORARY TABLE.
						///
						///</summary>
						///<param name="If the P3 value is non">zero, then the table referred to must be an</param>
						///<param name="intkey table (an SQL table, not an index). In this case the row change">intkey table (an SQL table, not an index). In this case the row change</param>
						///<param name="count is incremented by the number of rows in the table being cleared.">count is incremented by the number of rows in the table being cleared.</param>
						///<param name="If P3 is greater than zero, then the value stored in register P3 is">If P3 is greater than zero, then the value stored in register P3 is</param>
						///<param name="also incremented by the number of rows in the table being cleared.">also incremented by the number of rows in the table being cleared.</param>
						///<param name=""></param>
						///<param name="See also: Destroy">See also: Destroy</param>
						///<param name=""></param>
						case OpCode.OP_Clear: {
							int nChange;
							nChange=0;
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p2))!=0);
							int iDummy0=0;
							if(pOp.p3!=0)
								rc=db.aDb[pOp.p2].pBt.sqlite3BtreeClearTable(pOp.p1,ref nChange);
							else
								rc=db.aDb[pOp.p2].pBt.sqlite3BtreeClearTable(pOp.p1,ref iDummy0);
							if(pOp.p3!=0) {
								this.nChange+=nChange;
								if(pOp.p3>0) {
									Debug.Assert(aMem[pOp.p3].memIsValid());
									memAboutToChange(this,aMem[pOp.p3]);
									aMem[pOp.p3].u.i+=nChange;
								}
							}
							break;
						}
						///
						///<summary>
						///Opcode: CreateTable P1 P2 * * *
						///
						///Allocate a new table in the main database file if P1==0 or in the
						///auxiliary database file if P1==1 or in an attached database if
						///P1>1.  Write the root page number of the new table into
						///register P2
						///
						///The difference between a table and an index is this:  A table must
						///</summary>
						///<param name="have a 4">byte integer key and can have arbitrary data.  An index</param>
						///<param name="has an arbitrary key but no data.">has an arbitrary key but no data.</param>
						///<param name=""></param>
						///<param name="See also: CreateIndex">See also: CreateIndex</param>
						///<param name=""></param>
						///
						///<summary>
						///Opcode: CreateIndex P1 P2 * * *
						///
						///Allocate a new index in the main database file if P1==0 or in the
						///auxiliary database file if P1==1 or in an attached database if
						///P1>1.  Write the root page number of the new table into
						///register P2.
						///
						///See documentation on  OpCode.OP_CreateTable for additional information.
						///
						///</summary>
						case OpCode.OP_CreateIndex:
						///
						///<summary>
						///</summary>
						///<param name="out2">prerelease </param>
						case OpCode.OP_CreateTable: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							int pgno;
							int flags;
							Db pDb;
							pgno=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pDb=db.aDb[pOp.p1];
							Debug.Assert(pDb.pBt!=null);
							if(pOp.OpCode==OpCode.OP_CreateTable) {
								///
								///<summary>
								///flags = BTREE_INTKEY; 
								///</summary>
								flags=BTREE_INTKEY;
							}
							else {
								flags=BTREE_BLOBKEY;
							}
                            rc = pDb.pBt.sqlite3BtreeCreateTable(ref pgno, flags);
							pOut.u.i=pgno;
							break;
						}
						///
						///<summary>
						///Opcode: ParseSchema P1 * * P4 *
						///
						///Read and parse all entries from the SQLITE_MASTER table of database P1
						///that match the WHERE clause P4. 
						///
						///This opcode invokes the parser to create a new virtual machine,
						///</summary>
						///<param name="then runs the new virtual machine.  It is thus a re">entrant opcode.</param>
						///<param name=""></param>
						case OpCode.OP_ParseSchema: {
							int iDb;
							string zMaster;
							string zSql;
							InitData initData;
							///
							///<summary>
							///Any prepared statement that invokes this opcode will hold mutexes
							///on every btree.  This is a prerequisite for invoking
							///sqlite3InitCallback().
							///
							///</summary>
							#if SQLITE_DEBUG
																																																																																																																																				              for ( iDb = 0; iDb < db.nDb; iDb++ )
              {
                Debug.Assert( iDb == 1 || sqlite3BtreeHoldsMutex( db.aDb[iDb].pBt ) );
              }
#endif
							iDb=pOp.p1;
							Debug.Assert(iDb>=0&&iDb<db.nDb);
                            Debug.Assert(db.DbHasProperty(iDb, sqliteinth.DB_SchemaLoaded));
							///
							///<summary>
							///Used to be a conditional 
							///</summary>
							{
                                zMaster = sqliteinth.SCHEMA_TABLE(iDb);
								initData=new InitData();
								initData.db=db;
								initData.iDb=pOp.p1;
								initData.pzErrMsg=this.zErrMsg;
								zSql=io.sqlite3MPrintf(db,"SELECT name, rootpage, sql FROM '%q'.%s WHERE %s ORDER BY rowid",db.aDb[iDb].zName,zMaster,pOp.p4.z);
								if(String.IsNullOrEmpty(zSql)) {
									rc=SQLITE_NOMEM;
								}
								else {
									Debug.Assert(0==db.init.busy);
									db.init.busy=1;
									initData.rc=Sqlite3.SQLITE_OK;
									//Debug.Assert( 0 == db.mallocFailed );
                                    rc = legacy.sqlite3_exec(db, zSql, (dxCallback)sqlite3InitCallback, (object)initData, 0);
									if(rc==Sqlite3.SQLITE_OK)
										rc=initData.rc;
									db.sqlite3DbFree(ref zSql);
									db.init.busy=0;
								}
							}
							if(rc==SQLITE_NOMEM) {
								goto no_mem;
							}
							break;
						}
						#if !SQLITE_OMIT_ANALYZE
						///
						///<summary>
						///Opcode: LoadAnalysis P1 * * * *
						///
						///Read the sqlite_stat1 table for database P1 and load the content
						///of that table into the internal index hash table.  This will cause
						///the analysis to be used when preparing all subsequent queries.
						///</summary>
						case OpCode.OP_LoadAnalysis: {
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							rc=sqlite3AnalysisLoad(db,pOp.p1);
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: DropTable P1 * * P4 *
						///
						///</summary>
						///<param name="Remove the internal (in">memory) data structures that describe</param>
						///<param name="the table named P4 in database P1.  This is called after a table">the table named P4 in database P1.  This is called after a table</param>
						///<param name="is dropped in order to keep the internal representation of the">is dropped in order to keep the internal representation of the</param>
						///<param name="schema consistent with what is on disk.">schema consistent with what is on disk.</param>
						case OpCode.OP_DropTable: {
							build.sqlite3UnlinkAndDeleteTable(db,pOp.p1,pOp.p4.z);
							break;
						}
						///
						///<summary>
						///Opcode: DropIndex P1 * * P4 *
						///
						///</summary>
						///<param name="Remove the internal (in">memory) data structures that describe</param>
						///<param name="the index named P4 in database P1.  This is called after an index">the index named P4 in database P1.  This is called after an index</param>
						///<param name="is dropped in order to keep the internal representation of the">is dropped in order to keep the internal representation of the</param>
						///<param name="schema consistent with what is on disk.">schema consistent with what is on disk.</param>
						///<param name=""></param>
						case OpCode.OP_DropIndex: {
							build.sqlite3UnlinkAndDeleteIndex(db,pOp.p1,pOp.p4.z);
							break;
						}
						///
						///<summary>
						///Opcode: DropTrigger P1 * * P4 *
						///
						///</summary>
						///<param name="Remove the internal (in">memory) data structures that describe</param>
						///<param name="the trigger named P4 in database P1.  This is called after a trigger">the trigger named P4 in database P1.  This is called after a trigger</param>
						///<param name="is dropped in order to keep the internal representation of the">is dropped in order to keep the internal representation of the</param>
						///<param name="schema consistent with what is on disk.">schema consistent with what is on disk.</param>
						///<param name=""></param>
						case OpCode.OP_DropTrigger: {
							sqlite3UnlinkAndDeleteTrigger(db,pOp.p1,pOp.p4.z);
							break;
						}
						#if !SQLITE_OMIT_INTEGRITY_CHECK
						///
						///<summary>
						///Opcode: IntegrityCk P1 P2 P3 * P5
						///
						///Do an analysis of the currently open database.  Store in
						///register P1 the text of an error message describing any problems.
						///If no problems are found, store a NULL in register P1.
						///
						///The register P3 contains the maximum number of allowed errors.
						///At most reg(P3) errors will be reported.
						///In other words, the analysis stops as soon as reg(P1) errors are
						///seen.  Reg(P1) is updated with the number of errors remaining.
						///
						///The root page numbers of all tables in the database are integer
						///stored in reg(P1), reg(P1+1), reg(P1+2), ....  There are P2 tables
						///total.
						///
						///If P5 is not zero, the check is done on the auxiliary database
						///file, not the main database file.
						///
						///This opcode is used to implement the integrity_check pragma.
						///</summary>
						case OpCode.OP_IntegrityCk: {
							int nRoot;
							///
							///<summary>
							///Number of tables to check.  (Number of root pages.) 
							///</summary>
							int[] aRoot=null;
							///
							///<summary>
							///Array of rootpage numbers for tables to be checked 
							///</summary>
							int j;
							///
							///<summary>
							///Loop counter 
							///</summary>
							int nErr=0;
							///
							///<summary>
							///Number of errors reported 
							///</summary>
							string z;
							///
							///<summary>
							///Text of the error report 
							///</summary>
							Mem pnErr;
							///
							///<summary>
							///Register keeping track of errors remaining 
							///</summary>
							nRoot=pOp.p2;
							Debug.Assert(nRoot>0);
							aRoot=malloc_cs.sqlite3Malloc(aRoot,(nRoot+1));
							// sqlite3DbMallocRaw(db, sizeof(int) * (nRoot + 1));
							if(aRoot==null)
								goto no_mem;
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pnErr=aMem[pOp.p3];
							Debug.Assert((pnErr.flags&MemFlags.MEM_Int)!=0);
							Debug.Assert((pnErr.flags&(MemFlags.MEM_Str|MemFlags.MEM_Blob))==0);
							pIn1=aMem[pOp.p1];
							for(j=0;j<nRoot;j++) {
								aRoot[j]=(int)this.aMem[pOp.p1+j].sqlite3VdbeIntValue();
								// pIn1[j]);
							}
							aRoot[j]=0;
							Debug.Assert(pOp.p5<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p5))!=0);
							z=db.aDb[pOp.p5].pBt.sqlite3BtreeIntegrityCheck(aRoot,nRoot,(int)pnErr.u.i,ref nErr);
							db.sqlite3DbFree(ref aRoot);
							pnErr.u.i-=nErr;
							pIn1.sqlite3VdbeMemSetNull();
							if(nErr==0) {
								Debug.Assert(z=="");
							}
							else
								if(String.IsNullOrEmpty(z)) {
									goto no_mem;
								}
								else {
                                    pIn1.sqlite3VdbeMemSetStr(z, -1, SqliteEncoding.UTF8, null);
									//malloc_cs.sqlite3_free );
								}
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
                            vdbemem_cs.sqlite3VdbeChangeEncoding(pIn1, encoding);
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: RowSetAdd P1 P2 * * *
						///
						///Insert the integer value held by register P2 into a boolean index
						///held in register P1.
						///
						///An assertion fails if P2 is not an integer.
						///</summary>
						case OpCode.OP_RowSetAdd: {
							///
							///<summary>
							///in1, in2 
							///</summary>
							pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							Debug.Assert((pIn2.flags&MemFlags.MEM_Int)!=0);
							if((pIn1.flags&MemFlags.MEM_RowSet)==0) {
                                pIn1.sqlite3VdbeMemSetRowSet();
								if((pIn1.flags&MemFlags.MEM_RowSet)==0)
									goto no_mem;
							}
                            pIn1.u.pRowSet.sqlite3RowSetInsert(pIn2.u.i);
							break;
						}
						///
						///<summary>
						///Opcode: RowSetRead P1 P2 P3 * *
						///
						///Extract the smallest value from boolean index P1 and put that value into
						///register P3.  Or, if boolean index P1 is initially empty, leave P3
						///unchanged and jump to instruction P2.
						///
						///</summary>
						case OpCode.OP_RowSetRead: {
							///
							///<summary>
							///jump, in1, ref3 
							///</summary>
							i64 val=0;
							if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							pIn1=aMem[pOp.p1];
                            if ((pIn1.flags & MemFlags.MEM_RowSet) == 0 || pIn1.u.pRowSet.sqlite3RowSetNext(ref val) == 0)
                            {
								///
								///<summary>
								///The boolean index is empty 
								///</summary>
                                pIn1.sqlite3VdbeMemSetNull();
								opcodeIndex=pOp.p2-1;
							}
							else {
								///
								///<summary>
								///A value was pulled from the index 
								///</summary>
								aMem[pOp.p3].sqlite3VdbeMemSetInt64(val);
							}
							break;
						}
						///
						///<summary>
						///Opcode: RowSetTest P1 P2 P3 P4
						///
						///</summary>
						///<param name="Register P3 is assumed to hold a 64">bit integer value. If register P1</param>
						///<param name="contains a RowSet object and that RowSet object contains">contains a RowSet object and that RowSet object contains</param>
						///<param name="the value held in P3, jump to register P2. Otherwise, insert the">the value held in P3, jump to register P2. Otherwise, insert the</param>
						///<param name="integer in P3 into the RowSet and continue on to the">integer in P3 into the RowSet and continue on to the</param>
						///<param name="next opcode.">next opcode.</param>
						///<param name=""></param>
						///<param name="The RowSet object is optimized for the case where successive sets">The RowSet object is optimized for the case where successive sets</param>
						///<param name="of integers, where each set contains no duplicates. Each set">of integers, where each set contains no duplicates. Each set</param>
						///<param name="of values is identified by a unique P4 value. The first set">of values is identified by a unique P4 value. The first set</param>
						///<param name="must have P4==0, the final set P4=">1 or</param>
						///<param name="non">negative values of P4 only the lower 4</param>
						///<param name="bits are significant.">bits are significant.</param>
						///<param name=""></param>
						///<param name="This allows optimizations: (a) when P4==0 there is no need to test">This allows optimizations: (a) when P4==0 there is no need to test</param>
						///<param name="the rowset object for P3, as it is guaranteed not to contain it,">the rowset object for P3, as it is guaranteed not to contain it,</param>
						///<param name="(b) when P4==">1 there is no need to insert the value, as it will</param>
						///<param name="never be tested for, and (c) when a value that is part of set X is">never be tested for, and (c) when a value that is part of set X is</param>
						///<param name="inserted, there is no need to search to see if the same value was">inserted, there is no need to search to see if the same value was</param>
						///<param name="previously inserted as part of set X (only if it was previously">previously inserted as part of set X (only if it was previously</param>
						///<param name="inserted as part of some other set).">inserted as part of some other set).</param>
						///<param name=""></param>
						case OpCode.OP_RowSetTest: {
							///
							///<summary>
							///jump, in1, in3 
							///</summary>
							int iSet;
							int exists;
							pIn1=aMem[pOp.p1];
							pIn3=aMem[pOp.p3];
							iSet=pOp.p4.i;
							Debug.Assert((pIn3.flags&MemFlags.MEM_Int)!=0);
							///
							///<summary>
							///If there is anything other than a rowset object in memory cell P1,
							///delete it now and initialize P1 with an empty rowset
							///
							///</summary>
							if((pIn1.flags&MemFlags.MEM_RowSet)==0) {
                                pIn1.sqlite3VdbeMemSetRowSet();
								if((pIn1.flags&MemFlags.MEM_RowSet)==0)
									goto no_mem;
							}
							Debug.Assert(pOp.p4type== P4Usage.P4_INT32);
							Debug.Assert(iSet==-1||iSet>=0);
							if(iSet!=0) {
                                exists = pIn1.u.pRowSet.sqlite3RowSetTest((u8)(iSet >= 0 ? iSet & 0xf : 0xff), pIn3.u.i);
								if(exists!=0) {
									opcodeIndex=pOp.p2-1;
									break;
								}
							}
							if(iSet>=0) {
                                pIn1.u.pRowSet.sqlite3RowSetInsert(pIn3.u.i);
							}
							break;
						}
						#if !SQLITE_OMIT_TRIGGER
						///
						///<summary>
						///Opcode: Program P1 P2 P3 P4 *
						///
						///Execute the trigger program passed as P4 (type  P4Usage.P4_SUBPROGRAM). 
						///
						///P1 contains the address of the memory cell that contains the first memory 
						///</summary>
						///<param name="cell in an array of values used as arguments to the sub">program. P2 </param>
						///<param name="contains the address to jump to if the sub">program throws an IGNORE </param>
						///<param name="exception using the RAISE() function. Register P3 contains the address ">exception using the RAISE() function. Register P3 contains the address </param>
						///<param name="of a memory cell in this (the parent) VM that is used to allocate the ">of a memory cell in this (the parent) VM that is used to allocate the </param>
						///<param name="memory required by the sub">vdbe at runtime.</param>
						///<param name=""></param>
						///<param name="P4 is a pointer to the VM containing the trigger program.">P4 is a pointer to the VM containing the trigger program.</param>
						case OpCode.OP_Program: {
							///
							///<summary>
							///jump 
							///</summary>
							int nMem;
							///
							///<summary>
							///</summary>
							///<param name="Number of memory registers for sub">program </param>
							int nByte;
							///
							///<summary>
							///</summary>
							///<param name="Bytes of runtime space required for sub">program </param>
							Mem pRt;
							///
							///<summary>
							///Register to allocate runtime space 
							///</summary>
							Mem pMem=null;
							///
							///<summary>
							///Used to iterate through memory cells 
							///</summary>
							//Mem pEnd;            /* Last memory cell in new array */
							VdbeFrame pFrame;
							///
							///<summary>
							///New vdbe frame to execute in 
							///</summary>
							SubProgram pProgram;
							///
							///<summary>
							///</summary>
							///<param name="Sub">program to execute </param>
							int t;
							///
							///<summary>
							///Token identifying trigger 
							///</summary>
							pProgram=pOp.p4.pProgram;
							pRt=aMem[pOp.p3];
							Debug.Assert(pRt.memIsValid());
							Debug.Assert(pProgram.nOp>0);
							///
							///<summary>
							///If the p5 flag is clear, then recursive invocation of triggers is 
							///</summary>
							///<param name="disabled for backwards compatibility (p5 is set if this sub">program</param>
							///<param name="is really a trigger, not a foreign key action, and the flag set">is really a trigger, not a foreign key action, and the flag set</param>
							///<param name="and cleared by the "PRAGMA recursive_triggers" command is clear).">and cleared by the "PRAGMA recursive_triggers" command is clear).</param>
							///<param name=""></param>
							///<param name="It is recursive invocation of triggers, at the SQL level, that is ">It is recursive invocation of triggers, at the SQL level, that is </param>
							///<param name="disabled. In some cases a single trigger may generate more than one ">disabled. In some cases a single trigger may generate more than one </param>
							///<param name="SubProgram (if the trigger may be executed with more than one different ">SubProgram (if the trigger may be executed with more than one different </param>
							///<param name="ON CONFLICT algorithm). SubProgram structures associated with a">ON CONFLICT algorithm). SubProgram structures associated with a</param>
							///<param name="single trigger all have the same value for the SubProgram.token ">single trigger all have the same value for the SubProgram.token </param>
							///<param name="variable.  ">variable.  </param>
							if(pOp.p5!=0) {
								t=pProgram.token;
								for(pFrame=this.pFrame;pFrame!=null&&pFrame.token!=t;pFrame=pFrame.pParent)
									;
								if(pFrame!=null)
									break;
							}
							if(this.nFrame>=db.aLimit[SQLITE_LIMIT_TRIGGER_DEPTH]) {
								rc=Sqlite3.SQLITE_ERROR;
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"too many levels of trigger recursion");
								break;
							}
							///
							///<summary>
							///Register pRt is used to store the memory required to save the state
							///of the current program, and the memory required at runtime to execute
							///the trigger program. If this trigger has been fired before, then pRt 
							///is already allocated. Otherwise, it must be initialized.  
							///</summary>
							if((pRt.flags&MemFlags.MEM_Frame)==0) {
								///
								///<summary>
								///SubProgram.nMem is set to the number of memory cells used by the 
								///program stored in SubProgram.aOp. As well as these, one memory
								///cell is required for each cursor used by the program. Set local
								///variable nMem (and later, VdbeFrame.nChildMem) to this value.
								///
								///</summary>
								nMem=pProgram.nMem+pProgram.nCsr;
								//nByte = ROUND8( sizeof( VdbeFrame ) )
								//+ nMem * sizeof( Mem )
								//+ pProgram.nCsr * sizeof( VdbeCursor* );
								pFrame=new VdbeFrame();
								// sqlite3DbMallocZero( db, nByte );
								//if ( !pFrame )
								//{
								//  goto no_mem;
								//}
								pRt.sqlite3VdbeMemRelease();
								pRt.flags=MemFlags.MEM_Frame;
								pRt.u.pFrame=pFrame;
								pFrame.v=this;
								pFrame.nChildMem=nMem;
								pFrame.nChildCsr=pProgram.nCsr;
								pFrame.currentOpCodeIndex=opcodeIndex;
								pFrame.aMem=this.aMem;
								pFrame.nMem=this.nMem;
								pFrame.apCsr=this.apCsr;
								pFrame.nCursor=this.nCursor;
								pFrame.aOp=this.aOp;
								pFrame.nOp=this.nOp;
								pFrame.token=pProgram.token;
								// &VdbeFrameMem( pFrame )[pFrame.nChildMem];
								// aMem is 1 based, so allocate 1 extra cell under C#
								pFrame.aChildMem=new Mem[pFrame.nChildMem+1];
								for(int i=0;i<pFrame.aChildMem.Length;i++)//pMem = VdbeFrameMem( pFrame ) ; pMem != pEnd ; pMem++ )
								 {
									//pFrame.aMem[i] = pFrame.aMem[pFrame.nMem+i];
									pMem=malloc_cs.sqlite3Malloc(pMem);
									pMem.flags=MemFlags.MEM_Null;
									pMem.db=db;
									pFrame.aChildMem[i]=pMem;
								}
								pFrame.aChildCsr=new VdbeCursor[pFrame.nChildCsr];
								for(int i=0;i<pFrame.nChildCsr;i++)
									pFrame.aChildCsr[i]=new VdbeCursor();
							}
							else {
								pFrame=pRt.u.pFrame;
								Debug.Assert(pProgram.nMem+pProgram.nCsr==pFrame.nChildMem);
								Debug.Assert(pProgram.nCsr==pFrame.nChildCsr);
								Debug.Assert(opcodeIndex==pFrame.currentOpCodeIndex);
							}
							this.nFrame++;
							pFrame.pParent=this.pFrame;
							pFrame.lastRowid=lastRowid;
							pFrame.nChange=this.nChange;
							this.nChange=0;
							this.pFrame=pFrame;
							this.aMem=aMem=pFrame.aChildMem;
							// &VdbeFrameMem( pFrame )[-1];
							this.nMem=pFrame.nChildMem;
							this.nCursor=(u16)pFrame.nChildCsr;
							this.apCsr=pFrame.aChildCsr;
							// (VdbeCursor *)&aMem[p->nMem+1];
							this.lOp=lOp=new List<Op>(pProgram.aOp);
							this.nOp=pProgram.nOp;
							opcodeIndex=-1;
							break;
						}
						///
						///<summary>
						///Opcode: Param P1 P2 * * *
						///
						///</summary>
						///<param name="This opcode is only ever present in sub">programs called via the </param>
						///<param name="OP_Program instruction. Copy a value currently stored in a memory ">OP_Program instruction. Copy a value currently stored in a memory </param>
						///<param name="cell of the calling (parent) frame to cell P2 in the current frames ">cell of the calling (parent) frame to cell P2 in the current frames </param>
						///<param name="address space. This is used by trigger programs to access the new.* ">address space. This is used by trigger programs to access the new.* </param>
						///<param name="and old.* values.">and old.* values.</param>
						///<param name=""></param>
						///<param name="The address of the cell in the parent frame is determined by adding">The address of the cell in the parent frame is determined by adding</param>
						///<param name="the value of the P1 argument to the value of the P1 argument to the">the value of the P1 argument to the value of the P1 argument to the</param>
						///<param name="calling  OpCode.OP_Program instruction.">calling  OpCode.OP_Program instruction.</param>
						///<param name=""></param>
						case OpCode.OP_Param: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							VdbeFrame pFrame;
							Mem pIn;
							pFrame=this.pFrame;
							pIn=pFrame.aMem[pOp.p1+pFrame.aOp[pFrame.currentOpCodeIndex].p1];
							vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut,pIn,MemFlags.MEM_Ephem);
							break;
						}
						#endif
						#if !SQLITE_OMIT_FOREIGN_KEY
						///
						///<summary>
						///Opcode: FkCounter P1 P2 * * *
						///
						///Increment a "constraint counter" by P2 (P2 may be negative or positive).
						///</summary>
						///<param name="If P1 is non">zero, the database constraint counter is incremented </param>
						///<param name="(deferred foreign key constraints). Otherwise, if P1 is zero, the ">(deferred foreign key constraints). Otherwise, if P1 is zero, the </param>
						///<param name="statement counter is incremented (immediate foreign key constraints).">statement counter is incremented (immediate foreign key constraints).</param>
						case OpCode.OP_FkCounter: {
							if(pOp.p1!=0) {
								db.nDeferredCons+=pOp.p2;
							}
							else {
								this.nFkConstraint+=pOp.p2;
							}
							break;
						}
						///
						///<summary>
						///Opcode: FkIfZero P1 P2 * * *
						///
						///</summary>
						///<param name="This opcode tests if a foreign key constraint">counter is currently zero.</param>
						///<param name="If so, jump to instruction P2. Otherwise, fall through to the next ">If so, jump to instruction P2. Otherwise, fall through to the next </param>
						///<param name="instruction.">instruction.</param>
						///<param name=""></param>
						///<param name="If P1 is non">counter</param>
						///<param name="is zero (the one that counts deferred constraint violations). If P1 is">is zero (the one that counts deferred constraint violations). If P1 is</param>
						///<param name="zero, the jump is taken if the statement constraint">counter is zero</param>
						///<param name="(immediate foreign key constraint violations).">(immediate foreign key constraint violations).</param>
						///<param name=""></param>
						case OpCode.OP_FkIfZero: {
							///
							///<summary>
							///jump 
							///</summary>
							if(pOp.p1!=0) {
								if(db.nDeferredCons==0)
									opcodeIndex=pOp.p2-1;
							}
							else {
								if(this.nFkConstraint==0)
									opcodeIndex=pOp.p2-1;
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_AUTOINCREMENT
						///
						///<summary>
						///Opcode: MemMax P1 P2 * * *
						///
						///P1 is a register in the root frame of this VM (the root frame is
						///different from the current frame if this instruction is being executed
						///</summary>
						///<param name="within a sub">program). Set the value of register P1 to the maximum of </param>
						///<param name="its current value and the value in register P2.">its current value and the value in register P2.</param>
						///<param name=""></param>
						///<param name="This instruction throws an error if the memory cell is not initially">This instruction throws an error if the memory cell is not initially</param>
						///<param name="an integer.">an integer.</param>
						case OpCode.OP_MemMax: {
							///
							///<summary>
							///in2 
							///</summary>
							Mem _pIn1;
							VdbeFrame pFrame;
							if(this.pFrame!=null) {
								for(pFrame=this.pFrame;pFrame.pParent!=null;pFrame=pFrame.pParent)
									;
								_pIn1=pFrame.aMem[pOp.p1];
							}
							else {
								_pIn1=aMem[pOp.p1];
							}
							Debug.Assert(_pIn1.memIsValid());
							_pIn1.sqlite3VdbeMemIntegerify();
							pIn2=aMem[pOp.p2];
							pIn2.sqlite3VdbeMemIntegerify();
							if(_pIn1.u.i<pIn2.u.i) {
								_pIn1.u.i=pIn2.u.i;
							}
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: IfPos P1 P2 * * *
						///
						///If the value of register P1 is 1 or greater, jump to P2.
						///
						///It is illegal to use this instruction on a register that does
						///not contain an integer.  An Debug.Assertion fault will result if you try.
						///</summary>
						case OpCode.OP_IfPos: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MemFlags.MEM_Int)!=0);
							if(pIn1.u.i>0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: IfNeg P1 P2 * * *
						///
						///If the value of register P1 is less than zero, jump to P2.
						///
						///It is illegal to use this instruction on a register that does
						///not contain an integer.  An Debug.Assertion fault will result if you try.
						///
						///</summary>
						case OpCode.OP_IfNeg: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MemFlags.MEM_Int)!=0);
							if(pIn1.u.i<0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: IfZero P1 P2 P3 * *
						///
						///The register P1 must contain an integer.  Add literal P3 to the
						///value in register P1.  If the result is exactly 0, jump to P2. 
						///
						///It is illegal to use this instruction on a register that does
						///not contain an integer.  An assertion fault will result if you try.
						///
						///</summary>
						case OpCode.OP_IfZero: {
							///
							///<summary>
							///jump, in1 
							///</summary>
							pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MemFlags.MEM_Int)!=0);
							pIn1.u.i+=pOp.p3;
							if(pIn1.u.i==0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						///
						///<summary>
						///Opcode: AggStep * P2 P3 P4 P5
						///
						///Execute the step function for an aggregate.  The
						///function has P5 arguments.   P4 is a pointer to the FuncDef
						///structure that specifies the function.  Use register
						///P3 as the accumulator.
						///
						///The P5 arguments are taken from register P2 and its
						///successors.
						///
						///</summary>
						case OpCode.OP_AggStep: {
							int n;
							int i;
							Mem pMem;
							Mem pRec;
							sqlite3_context ctx=new sqlite3_context();
							sqlite3_value[] apVal;
							n=pOp.p5;
							Debug.Assert(n>=0);
							//pRec = aMem[pOp.p2];
							apVal=this.apArg;
							Debug.Assert(apVal!=null||n==0);
							for(i=0;i<n;i++)//, pRec++)
							 {
								pRec=aMem[pOp.p2+i];
								Debug.Assert(pRec.memIsValid());
								apVal[i]=pRec;
								memAboutToChange(this,pRec);
								sqlite3VdbeMemStoreType(pRec);
							}
							ctx.pFunc=pOp.p4.pFunc;
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							ctx.pMem=pMem=aMem[pOp.p3];
							pMem.n++;
							ctx.s.flags=MemFlags.MEM_Null;
							ctx.s.z=null;
							//ctx.s.zMalloc = null;
							ctx.s.xDel=null;
							ctx.s.db=db;
							ctx.isError=0;
							ctx.pColl=null;
							if((ctx.pFunc.flags&FuncFlags.SQLITE_FUNC_NEEDCOLL)!=0) {
								Debug.Assert(opcodeIndex>0);
								//pOp > p.aOp );
								Debug.Assert(this.lOp[opcodeIndex-1].p4type== P4Usage.P4_COLLSEQ);
								//pOp[-1].p4type ==  P4Usage.P4_COLLSEQ );
								Debug.Assert(this.lOp[opcodeIndex-1].OpCode==OpCode.OP_CollSeq);
								// pOp[-1].opcode ==  OpCode.OP_CollSeq );
								ctx.pColl=this.lOp[opcodeIndex-1].p4.pColl;
								;
								// pOp[-1].p4.pColl;
							}
							ctx.pFunc.xStep(ctx,n,apVal);
							///
							///<summary>
							///</summary>
							///<param name="IMP: R">23230 </param>
							if(ctx.isError!=0) {
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,vdbeapi.sqlite3_value_text(ctx.s));
								rc=ctx.isError;
							}
							ctx.s.sqlite3VdbeMemRelease();
							break;
						}
						///
						///<summary>
						///Opcode: AggFinal P1 P2 * P4 *
						///
						///Execute the finalizer function for an aggregate.  P1 is
						///the memory location that is the accumulator for the aggregate.
						///
						///P2 is the number of arguments that the step function takes and
						///P4 is a pointer to the FuncDef for this function.  The P2
						///argument is not used by this opcode.  It is only there to disambiguate
						///functions that can take varying numbers of arguments.  The
						///P4 argument is only needed for the degenerate case where
						///the step function was not previously called.
						///
						///</summary>
						case OpCode.OP_AggFinal: {
							Mem pMem;
							Debug.Assert(pOp.p1>0&&pOp.p1<=this.nMem);
							pMem=aMem[pOp.p1];
							Debug.Assert((pMem.flags&~(MemFlags.MEM_Null|MemFlags.MEM_Agg))==0);
                            rc = vdbemem_cs.sqlite3VdbeMemFinalize(pMem, pOp.p4.pFunc);
							this.aMem[pOp.p1]=pMem;
							if(rc!=0) {
								malloc_cs.sqlite3SetString(ref this.zErrMsg,db,vdbeapi.sqlite3_value_text(pMem));
							}
                            vdbemem_cs.sqlite3VdbeChangeEncoding(pMem, encoding);
							#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pMem );
#endif
                            if (pMem.sqlite3VdbeMemTooBig())
                            {
								goto too_big;
							}
							break;
						}
						#if !SQLITE_OMIT_WAL
																																																																																																											/* Opcode: Checkpoint P1 P2 P3 * *
**
** Checkpoint database P1. This is a no-op if P1 is not currently in
** WAL mode. Parameter P2 is one of SQLITE_CHECKPOINT_PASSIVE, FULL
** or RESTART.  Write 1 or 0 into mem[P3] if the checkpoint returns
** SQLITE_BUSY or not, respectively.  Write the number of pages in the
** WAL after the checkpoint into mem[P3+1] and the number of pages
** in the WAL that have been checkpointed after the checkpoint
** completes into mem[P3+2].  However on an error, mem[P3+1] and
** mem[P3+2] are initialized to -1.
*/
cDebug.Ase  OpCode.OP_Checkpoint: {
  aRes[0] = 0;
  aRes[1] = aRes[2] = -1;
  Debug.Assert( pOp.p2==SQLITE_CHECKPOINT_PDebug.AsSIVE
       || pOp.p2==SQLITE_CHECKPOINT_FULL
       || pOp.p2==SQLITE_CHECKPOINT_RESTART
  );
  rc = sqlite3Checkpoint(db, pOp.p1, pOp.p2, ref aRes[1], ref aRes[2]);
  if( rc==SQLITE_BUSY ){
    rc = Sqlite3.SQLITE_OK;
    aRes[0] = 1;
  }
  for(i=0, pMem = aMem[pOp.p3]; i<3; i++, pMem++){
    sqlite3VdbeMemSetInt64(pMem, (i64)aRes[i]);
  }
  break;
};  
#endif
						#if !SQLITE_OMIT_PRAGMA
						///
						///<summary>
						///Opcode: JournalMode P1 P2 P3 * P5
						///
						///Change the journal mode of database P1 to P3. P3 must be one of the
						///PAGER_JOURNALMODE_XXX values. If changing between the various rollback
						///modes (delete, truncate, persist, off and memory), this is a simple
						///operation. No IO is required.
						///
						///If changing into or out of WAL mode the procedure is more complicated.
						///
						///</summary>
						///<param name="Write a string containing the final journal">mode to register P2.</param>
						case OpCode.OP_JournalMode:
						{
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							Btree pBt;
							///
							///<summary>
							///Btree to change journal mode of 
							///</summary>
							Pager pPager;
							///
							///<summary>
							///Pager associated with pBt 
							///</summary>
							int eNew;
							///
							///<summary>
							///New journal mode 
							///</summary>
							int eOld;
							///
							///<summary>
							///The old journal mode 
							///</summary>
							string zFilename;
							///
							///<summary>
							///Name of database file for pPager 
							///</summary>
							eNew=pOp.p3;
							Debug.Assert(eNew==PAGER_JOURNALMODE_DELETE||eNew==PAGER_JOURNALMODE_TRUNCATE||eNew==PAGER_JOURNALMODE_PERSIST||eNew==PAGER_JOURNALMODE_OFF||eNew==PAGER_JOURNALMODE_MEMORY||eNew==PAGER_JOURNALMODE_WAL||eNew==PAGER_JOURNALMODE_QUERY);
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							pBt=db.aDb[pOp.p1].pBt;
							pPager=pBt.sqlite3BtreePager();
							eOld=pPager.sqlite3PagerGetJournalMode();
							if(eNew==PAGER_JOURNALMODE_QUERY)
								eNew=eOld;
							if(0==pPager.sqlite3PagerOkToChangeJournalMode())
								eNew=eOld;
							#if !SQLITE_OMIT_WAL
																																																																																																																																					zFilename = sqlite3PagerFilename(pPager);

/* Do not allow a transition to journal_mode=WAL for a database
** in temporary storage or if the VFS does not support shared memory 
*/
if( eNew==PAGER_JOURNALMODE_WAL
&& (zFilename[0]==0                         /* Temp file */
|| !sqlite3PagerWalSupported(pPager))   /* No shared-memory support */
){
eNew = eOld;
}

if( (eNew!=eOld)
&& (eOld==PAGER_JOURNALMODE_WAL || eNew==PAGER_JOURNALMODE_WAL)
){
if( null==db.autoCommit || db.activeVdbeCnt>1 ){
rc = Sqlite3.SQLITE_ERROR;
malloc_cs.sqlite3SetString(&p.zErrMsg, db, 
"cannot change %s wal mode from within a transaction",
(eNew==PAGER_JOURNALMODE_WAL ? "into" : "out of")
);
break;
}else{

if( eOld==PAGER_JOURNALMODE_WAL ){
/* If leaving WAL mode, close the log file. If successful, the call
** to PagerCloseWal() checkpoints and deletes the write-ahead-log 
** file. An EXCLUSIVE lock may still be held on the database file 
** after a successful return. 
*/
rc = sqlite3PagerCloseWal(pPager);
if( rc==Sqlite3.SQLITE_OK ){
sqlite3PagerSetJournalMode(pPager, eNew);
}
}else if( eOld==PAGER_JOURNALMODE_MEMORY ){
/* Cannot transition directly from MEMORY to WAL.  Use mode OFF
** as an intermediate */
sqlite3PagerSetJournalMode(pPager, PAGER_JOURNALMODE_OFF);
}

/* Open a transaction on the database file. Regardless of the journal
** mode, this transaction always uses a rollback journal.
*/
Debug.Assert( sqlite3BtreeIsInTrans(pBt)==0 );
if( rc==Sqlite3.SQLITE_OK ){
rc = sqlite3BtreeSetVersion(pBt, (eNew==PAGER_JOURNALMODE_WAL ? 2 : 1));
}
}
}
#endif
							if(rc!=0) {
								eNew=eOld;
							}
							eNew=pPager.sqlite3PagerSetJournalMode(eNew);
							pOut=aMem[pOp.p2];
							pOut.flags=MemFlags.MEM_Str|MemFlags.MEM_Static|MemFlags.MEM_Term;
							pOut.z=sqlite3JournalModename(eNew);
							pOut.n=StringExtensions.sqlite3Strlen30(pOut.z);
							pOut.enc=SqliteEncoding.UTF8;
                            vdbemem_cs.sqlite3VdbeChangeEncoding(pOut, encoding);
							break;
						}
						;
						#endif
						#if !SQLITE_OMIT_VACUUM && !SQLITE_OMIT_ATTACH
						///
						///<summary>
						///Opcode: Vacuum * * * * *
						///
						///Vacuum the entire database.  This opcode will cause other virtual
						///machines to be created and run.  It may not be called from within
						///a transaction.
						///</summary>
						case OpCode.OP_Vacuum: {
							rc=sqlite3RunVacuum(ref this.zErrMsg,db);
							break;
						}
						#endif
						#if !SQLITE_OMIT_AUTOVACUUM
						///
						///<summary>
						///Opcode: IncrVacuum P1 P2 * * *
						///
						///Perform a single step of the incremental vacuum procedure on
						///the P1 database. If the vacuum has finished, jump to instruction
						///P2. Otherwise, fall through to the next instruction.
						///</summary>
						case OpCode.OP_IncrVacuum: {
							///
							///<summary>
							///jump 
							///</summary>
							Btree pBt;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pBt=db.aDb[pOp.p1].pBt;
							rc=pBt.sqlite3BtreeIncrVacuum();
							if(rc==SQLITE_DONE) {
								opcodeIndex=pOp.p2-1;
								rc=Sqlite3.SQLITE_OK;
							}
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: Expire P1 * * * *
						///
						///Cause precompiled statements to become expired. An expired statement
						///fails with an error code of SQLITE_SCHEMA if it is ever executed
						///(via sqlite3_step()).
						///
						///</summary>
						///<param name="If P1 is 0, then all SQL statements become expired. If P1 is non">zero,</param>
						///<param name="then only the currently executing statement is affected.">then only the currently executing statement is affected.</param>
						case OpCode.OP_Expire: {
							if(pOp.p1==0) {
                                vdbeaux.sqlite3ExpirePreparedStatements(db);
							}
							else {
								this.expired=true;
							}
							break;
						}
						#if !SQLITE_OMIT_SHARED_CACHE
																																																																																																											/* Opcode: TableLock P1 P2 P3 P4 *
**
** Obtain a lock on a particular table. This instruction is only used when
** the shared-cache feature is enabled.
**
** P1 is the index of the database in sqlite3.aDb[] of the database
** on which the lock is acquired.  A readlock is obtained if P3==0 or
** a write lock if P3==1.
**
** P2 contains the root-page of the table to lock.
**
** P4 contains a pointer to the name of the table being locked. This is only
** used to generate an error message if the lock cannot be obtained.
*/
case  OpCode.OP_TableLock:
{
u8 isWriteLock = (u8)pOp.p3;
if( isWriteLock || 0==(db.flags&SQLITE_ReadUncommitted) ){
int p1 = pOp.p1; 
Debug.Assert( p1 >= 0 && p1 < db.nDb );
Debug.Assert( ( p.btreeMask & ( ((yDbMask)1) << p1 ) ) != 0 );
Debug.Assert( isWriteLock == 0 || isWriteLock == 1 );
rc = sqlite3BtreeLockTable( db.aDb[p1].pBt, pOp.p2, isWriteLock );
if ( ( rc & 0xFF ) == SQLITE_LOCKED )
{
string z = pOp.p4.z;
malloc_cs.sqlite3SetString( ref p.zErrMsg, db, "database table is locked: ", z );
}
}
break;
}
#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VBegin * * * P4 *
						///
						///P4 may be a pointer to an sqlite3_vtab structure. If so, call the
						///xBegin method for that table.
						///
						///Also, whether or not P4 is set, check that this is not being called from
						///within a callback to a virtual table xSync() method. If it is, the error
						///code will be set to SQLITE_LOCKED.
						///</summary>
						case OpCode.OP_VBegin: {
							VTable pVTab;
							pVTab=pOp.p4.pVtab;
                            rc = vtab.sqlite3VtabBegin(db, pVTab);
							if(pVTab!=null)
								importVtabErrMsg(this,pVTab.pVtab);
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VCreate P1 * * P4 *
						///
						///P4 is the name of a virtual table in database P1. Call the xCreate method
						///for that table.
						///</summary>
						case OpCode.OP_VCreate: {
							rc=vtab.sqlite3VtabCallCreate(db,pOp.p1,pOp.p4.z,ref this.zErrMsg);
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VDestroy P1 * * P4 *
						///
						///P4 is the name of a virtual table in database P1.  Call the xDestroy method
						///of that table.
						///</summary>
						case OpCode.OP_VDestroy: {
							this.inVtabMethod=2;
							rc=vtab.sqlite3VtabCallDestroy(db,pOp.p1,pOp.p4.z);
							this.inVtabMethod=0;
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VOpen P1 * * P4 *
						///
						///P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
						///P1 is a cursor number.  This opcode opens a cursor to the virtual
						///table and stores that cursor in P1.
						///</summary>
						case OpCode.OP_VOpen: {
							VdbeCursor pCur;
							sqlite3_vtab_cursor pVtabCursor;
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							pCur=null;
							pVtab=pOp.p4.pVtab.pVtab;
							pModule=(sqlite3_module)pVtab.pModule;
							Debug.Assert(pVtab!=null&&pModule!=null);
							rc=pModule.xOpen(pVtab,out pVtabCursor);
							importVtabErrMsg(this,pVtab);
							if(Sqlite3.SQLITE_OK==rc) {
								///
								///<summary>
								///Initialize sqlite3_vtab_cursor base class 
								///</summary>
								pVtabCursor.pVtab=pVtab;
								///
								///<summary>
								///Initialise vdbe cursor object 
								///</summary>
								pCur=allocateCursor(this,pOp.p1,0,-1,0);
								if(pCur!=null) {
									pCur.pVtabCursor=pVtabCursor;
									pCur.pModule=pVtabCursor.pVtab.pModule;
								}
								else {
									//db.mallocFailed = 1;
									pModule.xClose(ref pVtabCursor);
								}
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VFilter P1 P2 P3 P4 *
						///
						///P1 is a cursor opened using VOpen.  P2 is an address to jump to if
						///the filtered result set is empty.
						///
						///P4 is either NULL or a string that was generated by the xBestIndex
						///method of the module.  The interpretation of the P4 string is left
						///to the module implementation.
						///
						///This opcode invokes the xFilter method on the virtual table specified
						///by P1.  The integer query plan parameter to xFilter is stored in register
						///P3. Register P3+1 stores the argc parameter to be passed to the
						///xFilter method. Registers P3+2..P3+1+argc are the argc
						///additional parameters which are passed to
						///xFilter as argv. Register P3+2 becomes argv[0] when passed to xFilter.
						///
						///A jump is made to P2 if the result set after filtering would be empty.
						///</summary>
						case OpCode.OP_VFilter: {
							///
							///<summary>
							///jump 
							///</summary>
							int nArg;
							int iQuery;
							sqlite3_module pModule;
							Mem pQuery;
							Mem pArgc=null;
							sqlite3_vtab_cursor pVtabCursor;
							sqlite3_vtab pVtab;
							VdbeCursor pCur;
							int res;
							int i;
							Mem[] apArg;
							pQuery=aMem[pOp.p3];
							pArgc=aMem[pOp.p3+1];
							// pQuery[1];
							pCur=this.apCsr[pOp.p1];
							Debug.Assert(pQuery.memIsValid());
							REGISTER_TRACE(this,pOp.p3,pQuery);
							Debug.Assert(pCur.pVtabCursor!=null);
							pVtabCursor=pCur.pVtabCursor;
							pVtab=pVtabCursor.pVtab;
							pModule=pVtab.pModule;
							///
							///<summary>
							///Grab the index number and argc parameters 
							///</summary>
							Debug.Assert((pQuery.flags&MemFlags.MEM_Int)!=0&&pArgc.flags==MemFlags.MEM_Int);
							nArg=(int)pArgc.u.i;
							iQuery=(int)pQuery.u.i;
							///
							///<summary>
							///Invoke the xFilter method 
							///</summary>
							{
								res=0;
								apArg=this.apArg;
								for(i=0;i<nArg;i++) {
									apArg[i]=aMem[(pOp.p3+1)+i+1];
									//apArg[i] = pArgc[i + 1];
									sqlite3VdbeMemStoreType(apArg[i]);
								}
								this.inVtabMethod=1;
								rc=pModule.xFilter(pVtabCursor,iQuery,pOp.p4.z,nArg,apArg);
								this.inVtabMethod=0;
								importVtabErrMsg(this,pVtab);
								if(rc==Sqlite3.SQLITE_OK) {
									res=pModule.xEof(pVtabCursor);
								}
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
								}
							}
							pCur.nullRow=false;
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VColumn P1 P2 P3 * *
						///
						///</summary>
						///<param name="Store the value of the P2">th column of</param>
						///<param name="the row of the virtual">table that the</param>
						///<param name="P1 cursor is pointing to into register P3.">P1 cursor is pointing to into register P3.</param>
						case OpCode.OP_VColumn: {
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							Mem pDest;
							sqlite3_context sContext;
							VdbeCursor pCur=this.apCsr[pOp.p1];
							Debug.Assert(pCur.pVtabCursor!=null);
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pDest=aMem[pOp.p3];
							memAboutToChange(this,pDest);
							if(pCur.nullRow) {
                                pDest.sqlite3VdbeMemSetNull();
								break;
							}
							pVtab=pCur.pVtabCursor.pVtab;
							pModule=pVtab.pModule;
							Debug.Assert(pModule.xColumn!=null);
							sContext=new sqlite3_context();
							//memset( &sContext, 0, sizeof( sContext ) );
							///
							///<summary>
							///The output cell may already have a buffer allocated. Move
							///</summary>
							///<param name="the current contents to sContext.s so in case the user">function</param>
							///<param name="can use the already allocated buffer instead of allocating a">can use the already allocated buffer instead of allocating a</param>
							///<param name="new one.">new one.</param>
							///<param name=""></param>
							vdbemem_cs.sqlite3VdbeMemMove(sContext.s,pDest);
							sContext.s.MemSetTypeFlag(MemFlags.MEM_Null);
							rc=pModule.xColumn(pCur.pVtabCursor,sContext,pOp.p2);
							importVtabErrMsg(this,pVtab);
							if(sContext.isError!=0) {
								rc=sContext.isError;
							}
							///
							///<summary>
							///Copy the result of the function to the P3 register. We
							///do this regardless of whether or not an error occurred to ensure any
							///dynamic allocation in sContext.s (a Mem struct) is  released.
							///
							///</summary>
                            vdbemem_cs.sqlite3VdbeChangeEncoding(sContext.s, encoding);
                            vdbemem_cs.sqlite3VdbeMemMove(pDest, sContext.s);
							REGISTER_TRACE(this,pOp.p3,pDest);
							UPDATE_MAX_BLOBSIZE(pDest);
                            if (pDest.sqlite3VdbeMemTooBig())
                            {
								goto too_big;
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VNext P1 P2 * * *
						///
						///Advance virtual table P1 to the next row in its result set and
						///jump to instruction P2.  Or, if the virtual table has reached
						///the end of its result set, then fall through to the next instruction.
						///</summary>
						case OpCode.OP_VNext: {
							///
							///<summary>
							///jump 
							///</summary>
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							int res;
							VdbeCursor pCur;
							res=0;
							pCur=this.apCsr[pOp.p1];
							Debug.Assert(pCur.pVtabCursor!=null);
							if(pCur.nullRow) {
								break;
							}
							pVtab=pCur.pVtabCursor.pVtab;
							pModule=pVtab.pModule;
							Debug.Assert(pModule.xNext!=null);
							///
							///<summary>
							///Invoke the xNext() method of the module. There is no way for the
							///underlying implementation to return an error if one occurs during
							///xNext(). Instead, if an error occurs, true is returned (indicating that
							///data is available) and the error code returned when xColumn or
							///some other method is next invoked on the save virtual table cursor.
							///
							///</summary>
							this.inVtabMethod=1;
							rc=pModule.xNext(pCur.pVtabCursor);
							this.inVtabMethod=0;
							importVtabErrMsg(this,pVtab);
							if(rc==Sqlite3.SQLITE_OK) {
								res=pModule.xEof(pCur.pVtabCursor);
							}
							if(0==res) {
								///
								///<summary>
								///If there is data, jump to P2 
								///</summary>
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VRename P1 * * P4 *
						///
						///P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
						///This opcode invokes the corresponding xRename method. The value
						///in register P1 is passed as the zName argument to the xRename method.
						///</summary>
						case OpCode.OP_VRename: {
							sqlite3_vtab pVtab;
							Mem pName;
							pVtab=pOp.p4.pVtab.pVtab;
							pName=aMem[pOp.p1];
							Debug.Assert(pVtab.pModule.xRename!=null);
							Debug.Assert(pName.memIsValid());
							REGISTER_TRACE(this,pOp.p1,pName);
							Debug.Assert((pName.flags&MemFlags.MEM_Str)!=0);
							rc=pVtab.pModule.xRename(pVtab,pName.z);
							importVtabErrMsg(this,pVtab);
							this.expired=false;
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						///
						///<summary>
						///Opcode: VUpdate P1 P2 P3 P4 *
						///
						///P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
						///This opcode invokes the corresponding xUpdate method. P2 values
						///are contiguous memory cells starting at P3 to pass to the xUpdate
						///</summary>
						///<param name="invocation. The value in register (P3+P2">1) corresponds to the</param>
						///<param name="p2th element of the argv array passed to xUpdate.">p2th element of the argv array passed to xUpdate.</param>
						///<param name=""></param>
						///<param name="The xUpdate method will do a DELETE or an INSERT or both.">The xUpdate method will do a DELETE or an INSERT or both.</param>
						///<param name="The argv[0] element (which corresponds to memory cell P3)">The argv[0] element (which corresponds to memory cell P3)</param>
						///<param name="is the rowid of a row to delete.  If argv[0] is NULL then no">is the rowid of a row to delete.  If argv[0] is NULL then no</param>
						///<param name="deletion occurs.  The argv[1] element is the rowid of the new">deletion occurs.  The argv[1] element is the rowid of the new</param>
						///<param name="row.  This can be NULL to have the virtual table select the new">row.  This can be NULL to have the virtual table select the new</param>
						///<param name="rowid for itself.  The subsequent elements in the array are">rowid for itself.  The subsequent elements in the array are</param>
						///<param name="the values of columns in the new row.">the values of columns in the new row.</param>
						///<param name=""></param>
						///<param name="If P2==1 then no insert is performed.  argv[0] is the rowid of">If P2==1 then no insert is performed.  argv[0] is the rowid of</param>
						///<param name="a row to delete.">a row to delete.</param>
						///<param name=""></param>
						///<param name="P1 is a boolean flag. If it is set to true and the xUpdate call">P1 is a boolean flag. If it is set to true and the xUpdate call</param>
						///<param name="is successful, then the value returned by sqlite3_last_insert_rowid()">is successful, then the value returned by sqlite3_last_insert_rowid()</param>
						///<param name="is set to the value of the rowid for the row just inserted.">is set to the value of the rowid for the row just inserted.</param>
						case OpCode.OP_VUpdate: {
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							int nArg;
							int i;
							sqlite_int64 rowid=0;
							Mem[] apArg;
							Mem pX;
							Debug.Assert(pOp.p2==1||
                                ((OnConstraintError)pOp.p5)
                                .In(OnConstraintError.OE_Fail
                                    ,OnConstraintError.OE_Rollback
                                    ,OnConstraintError.OE_Abort
                                    ,OnConstraintError.OE_Ignore
                                    ,OnConstraintError.OE_Replace));

							pVtab=pOp.p4.pVtab.pVtab;
							pModule=(sqlite3_module)pVtab.pModule;
							nArg=pOp.p2;
							Debug.Assert(pOp.p4type== P4Usage.P4_VTAB);
							if(Sqlite3.ALWAYS(pModule.xUpdate)) {
								u8 vtabOnConflict=db.vtabOnConflict;
								apArg=this.apArg;
								//pX = aMem[pOp.p3];
								for(i=0;i<nArg;i++) {
									pX=aMem[pOp.p3+i];
									Debug.Assert(pX.memIsValid());
									memAboutToChange(this,pX);
									sqlite3VdbeMemStoreType(pX);
									apArg[i]=pX;
									//pX++;
								}
								db.vtabOnConflict=pOp.p5;
								rc=pModule.xUpdate(pVtab,nArg,apArg,out rowid);
								db.vtabOnConflict=vtabOnConflict;
								importVtabErrMsg(this,pVtab);
								if(rc==Sqlite3.SQLITE_OK&&pOp.p1!=0) {
									Debug.Assert(nArg>1&&apArg[0]!=null&&(apArg[0].flags&MemFlags.MEM_Null)!=0);
									db.lastRowid=lastRowid=rowid;
								}
								if(rc==SQLITE_CONSTRAINT&&pOp.p4.pVtab.bConstraint!=0) {
                                    if ((OnConstraintError)pOp.p5 == OnConstraintError.OE_Ignore)
                                    {
										rc=Sqlite3.SQLITE_OK;
									}
									else {
                                        this.errorAction = ((OnConstraintError)pOp.p5).Filter(OnConstraintError.OE_Replace, OnConstraintError.OE_Abort);
									}
								}
								else {
									this.nChange++;
								}
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_PAGER_PRAGMAS
						///
						///<summary>
						///Opcode: Pagecount P1 P2 * * *
						///
						///Write the current number of pages in database P1 to memory cell P2.
						///</summary>
						case OpCode.OP_Pagecount: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							pOut.u.i=db.aDb[pOp.p1].pBt.sqlite3BtreeLastPage();
							break;
						}
						#endif
						#if !SQLITE_OMIT_PAGER_PRAGMAS
						///
						///<summary>
						///Opcode: MaxPgcnt P1 P2 P3 * *
						///
						///Try to set the maximum page count for database P1 to the value in P3.
						///Do not let the maximum page count fall below the current page count and
						///do not change the maximum page count value if P3==0.
						///
						///Store the maximum page count after the change in register P2.
						///</summary>
						case OpCode.OP_MaxPgcnt: {
							///
							///<summary>
							///</summary>
							///<param name="out2">prerelease </param>
							i64 newMax;
							Btree pBt;
							pBt=db.aDb[pOp.p1].pBt;
							newMax=0;
							if(pOp.p3!=0) {
								newMax=pBt.sqlite3BtreeLastPage();
								if(newMax<pOp.p3)
									newMax=pOp.p3;
							}
							pOut.u.i=(i64)pBt.GetMaxPageCount((int)newMax);
							break;
						}
						#endif
						#if !SQLITE_OMIT_TRACE
						///
						///<summary>
						///Opcode: Trace * * * P4 *
						///
						///If tracing is enabled (by the sqlite3_trace()) interface, then
						///</summary>
						///<param name="the UTF">8 string contained in P4 is emitted on the trace callback.</param>
						case OpCode.OP_Trace: {
							string zTrace;
							string z;
							if(db.xTrace!=null&&!String.IsNullOrEmpty(zTrace=(pOp.p4.z!=null?pOp.p4.z:this.zSql))) {
								z=sqlite3VdbeExpandSql(this,zTrace);
								db.xTrace(db.pTraceArg,z);
								//sqlite3DbFree( db, ref z );
							}
							#if SQLITE_DEBUG
																																																																																																																																				              if ( ( db.flags & SQLITE_SqlTrace ) != 0
                && ( zTrace = ( pOp.p4.z != null ? pOp.p4.z : p.zSql ) ) != "" )
              {
                sqlite3DebugPrintf( "SQL-trace: %s\n", zTrace );
              }
#endif
							break;
						}
						#endif
						///
						///<summary>
						///Opcode: Noop * * * * *
						///
						///Do nothing.  This instruction is often useful as a jump
						///destination.
						///</summary>
						///
						///<summary>
						///The magic Explain opcode are only inserted when explain==2 (which
						///is to say when the EXPLAIN QUERY PLAN syntax is used.)
						///This opcode records information from the optimizer.  It is the
						///</summary>
						///<param name="the same as a no">op.  This opcodesnever appears in a real VM program.</param>
						///<param name=""></param>
						default: {
							///
							///<summary>
							///This is really OpCode.OP_Noop and  OpCode.OP_Explain 
							///</summary>
							Debug.Assert(pOp.OpCode==OpCode.OP_Noop||pOp.OpCode==OpCode.OP_Explain);
							break;
						}
						///
						///<summary>
						///
						///The cases of the switch statement above this line should all be indented
						///</summary>
						///<param name="by 6 spaces.  But the left">most 6 spaces have been removed to improve the</param>
						///<param name="readability.  From this point on down, the normal indentation rules are">readability.  From this point on down, the normal indentation rules are</param>
						///<param name="restored.">restored.</param>
						///<param name=""></param>
						}
						#if VDBE_PROFILE
																																																																																																											{
u64 elapsed = sqlite3Hwtime() - start;
pOp.cycles += elapsed;
pOp.cnt++;
#if FALSE
																																																																																																											fprintf(stdout, "%10llu ", elapsed);
sqlite3VdbePrintOp(stdout, origPc, aOp[origPc]);
#endif
																																																																																																											}
#endif
						///
						///<summary>
						///The following code adds nothing to the actual functionality
						///of the program.  It is only here for testing and debugging.
						///On the other hand, it does burn CPU cycles every time through
						///the evaluator loop.  So we can leave it out when NDEBUG is defined.
						///</summary>
						#if !NDEBUG
																																																																																																											        Debug.Assert( pc >= -1 && pc < p.nOp );

#if SQLITE_DEBUG
																																																																																																											        if ( p.trace != null )
        {
          if ( rc != 0 )
            fprintf( p.trace, "rc=%d\n", rc );
          if ( ( pOp.opflags & ( OPFLG_OUT2_PRERELEASE | OPFLG_OUT2 ) ) != 0 )
          {
            registerTrace( p.trace, pOp.p2, aMem[pOp.p2] );
          }
          if ( ( pOp.opflags & OPFLG_OUT3 ) != 0 )
          {
            registerTrace( p.trace, pOp.p3, aMem[pOp.p3] );
          }
        }
#endif
												#endif
					}
					#endregion
					int xyz=6;
					///
					///<summary>
					///The end of the for(;;) loop the loops through opcodes 
					///</summary>
					///
					///<summary>
					///If we reach this point, it means that execution is finished with
					///an error of some kind.
					///
					///</summary>
					vdbe_error_halt:
					Debug.Assert(rc!=0);
					this.rc=rc;
					sqliteinth.testcase(Sqlite3.sqliteinth.sqlite3GlobalConfig.xLog!=null);
					io.sqlite3_log(rc,"statement aborts at %d: [%s] %s",opcodeIndex,this.zSql,this.zErrMsg);
					this.sqlite3VdbeHalt();
					//if ( rc == SQLITE_IOERR_NOMEM ) db.mallocFailed = 1;
					rc=Sqlite3.SQLITE_ERROR;
					if(resetSchemaOnFault>0) {
						build.sqlite3ResetInternalSchema(db,resetSchemaOnFault-1);
					}
					///
					///<summary>
					///This is the only way out of this procedure.  We have to
					///release the mutexes on btrees that were acquired at the
					///top. 
					///</summary>
					vdbe_return:
					db.lastRowid=lastRowid;
					this.sqlite3VdbeLeave();
					return rc;
					///
					///<summary>
					///Jump to here if a string or blob larger than db.aLimit[SQLITE_LIMIT_LENGTH]
					///is encountered.
					///
					///</summary>
					too_big:
					malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"string or blob too big");
					rc=SQLITE_TOOBIG;
					goto vdbe_error_halt;
					///
					///<summary>
					///Jump to here if a malloc() fails.
					///
					///</summary>
					no_mem:
					//db.mallocFailed = 1;
					malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"out of memory");
					rc=SQLITE_NOMEM;
					goto vdbe_error_halt;
					///
					///<summary>
					///Jump to here for any other kind of fatal error.  The "rc" variable
					///should hold the error number.
					///
					///</summary>
					abort_due_to_error:
					//Debug.Assert( p.zErrMsg); /// Not needed in C#
					//if ( db.mallocFailed != 0 ) rc = SQLITE_NOMEM;
					if(rc!=SQLITE_IOERR_NOMEM) {
						malloc_cs.sqlite3SetString(ref this.zErrMsg,db,"%s",sqlite3ErrStr(rc));
					}
					goto vdbe_error_halt;
					///
					///<summary>
					///Jump to here if the sqlite3_interrupt() API sets the interrupt
					///flag.
					///
					///</summary>
					abort_due_to_interrupt:
					Debug.Assert(db.u1.isInterrupted);
					rc=SQLITE_INTERRUPT;
					this.rc=rc;
					malloc_cs.sqlite3SetString(ref this.zErrMsg,db,sqlite3ErrStr(rc));
					goto vdbe_error_halt;
				}
				finally {
					Log.Unindent();
				}
			}

            enum ColumnResult
            {
                abort_due_to_error,
                op_column_out,
                too_big

            }
            private int OpCode_Column(Op pOp, int rc, sqlite3 db, SqliteEncoding encoding, Mem[] aMem)
            {
                
                

                ///The length of the serialized data for the column 
                ///
                int len;

                ///Number of bytes in the record 
                u32 payloadSize = 0;
                ///Pointer to complete record
                byte[] zRecord = null;
                ///Number of bytes in the record : INDEX 
                i64 payloadSize64 = 0;

                ///P1 value of the opcode 
                ///
                int p1 = pOp.p1;
                Debug.Assert(p1 < this.nCursor);

                ///column number to retrieve 
                int clumnNumber_p2=pOp.p2;
                ///The VDBE cursor 
                VdbeCursor vdbeCursor = this.apCsr[p1];
                ///This block sets the variable payloadSize to be the total number of
                ///bytes in the record.
                ///
                ///zRec is set to be the complete text of the record if it is available.
                ///The complete record text is always available for pseudo">tables</param>
                ///If the record is stored in a cursor, the complete record text
                ///might be available in the  pC.aRow cache.  Or it might not be.
                ///If the data is unavailable,  zRec is set to NULL.">If the data is unavailable,  zRec is set to NULL.</param>
                ///We also compute the number of columns in the record.  For cursors,
                ///the number of columns is stored in the VdbeCursor.nField element.

                ///number of fields in the record 
                int nField = vdbeCursor.nField;
                
                ///The BTree cursor 
                BtCursor btCursor = vdbeCursor.pCursor;
                ///aType[i] holds the numeric type of the i"th column 
                u32[] columnTypes = vdbeCursor.aType;

                //aOffset[i] is offset to start of data for i">th column
                u32[] clumnOffsets;
                
                ///Loop counter 
                int i;
                ///Part of the record being decoded 
                byte[] zData = null;
                ///Where to write the extracted value 
                Debug.Assert(pOp.p3 > 0 && pOp.p3 <= this.nMem);
                Mem pDest = aMem[pOp.p3];
                ///For storing the record being decoded 
                Mem sMem = null;
                sMem = malloc_cs.sqlite3Malloc(sMem);


                ///Index into header 
                int idxHeader;
                ///Pointer to first byte after the header 
                int idxEndHeader;
                ///Offset into the data 
                u32 offsetIntoData = 0;
                ///Number of bytes in the content of a field 
                u32 szField = 0;
                
                ///Number of bytes of available data 
                int avail;
                ///PseudoTable input register 
                Mem pReg;

                
                //  memset(&sMem, 0, sizeof(sMem));
                
                memAboutToChange(this, pDest);
                pDest.MemSetTypeFlag(MemFlags.MEM_Null);

                Debug.Assert(vdbeCursor != null);
#if !SQLITE_OMIT_VIRTUALTABLE
                Debug.Assert(vdbeCursor.pVtabCursor == null);
#endif
             
                #region payload size
                if (btCursor != null)
                {
                    ///The record is stored in a B">Tree
                    rc = (int)vdbeaux.sqlite3VdbeCursorMoveto(vdbeCursor);
                    if (rc != 0)
                        return (int)ColumnResult.abort_due_to_error;
                        //goto  abort_due_to_error;
                    if (vdbeCursor.nullRow)
                    {
                        payloadSize = 0;
                    }
                    else{
                        if ((vdbeCursor.cacheStatus == this.cacheCtr) && (vdbeCursor.aRow != -1))
                        {
                            payloadSize = vdbeCursor.payloadSize;
                            zRecord = malloc_cs.sqlite3Malloc((int)payloadSize);
                            Buffer.BlockCopy(btCursor.info.pCell, vdbeCursor.aRow, zRecord, 0, (int)payloadSize);
                        }
                        else{
                            if (vdbeCursor.isIndex)
                            {
                                Debug.Assert(btCursor.sqlite3BtreeCursorIsValid());
                                rc = btCursor.sqlite3BtreeKeySize(ref payloadSize64);
                                Debug.Assert(rc == Sqlite3.SQLITE_OK);
                                ///True because of CursorMoveto() call above 
                                ///sqlite3BtreeParseCellPtr() uses utilc.getVarint32() to extract the
                                ///payload size, so it is impossible for payloadSize64 to be
                                ///larger than 32 bits. 
                                Debug.Assert(((u64)payloadSize64 & sqliteinth.SQLITE_MAX_U32) == (u64)payloadSize64);
                                payloadSize = (u32)payloadSize64;
                            }
                            else
                            {
                                Debug.Assert(btCursor.sqlite3BtreeCursorIsValid());
                                rc = btCursor.sqlite3BtreeDataSize(ref payloadSize);
                                Debug.Assert(rc == Sqlite3.SQLITE_OK);
                                ///
                                ///<summary>
                                ///DataSize() cannot fail 
                                ///</summary>
                            }
                        }
                    }
                }
                else{
                    if (vdbeCursor.pseudoTableReg > 0)
                    {
                        ///The record is the sole entry of a pseudo">table
                        pReg = aMem[vdbeCursor.pseudoTableReg];
                        Debug.Assert((pReg.flags & MemFlags.MEM_Blob) != 0);
                        Debug.Assert(pReg.memIsValid());
                        payloadSize = (u32)pReg.n;
                        zRecord = pReg.zBLOB;
                        vdbeCursor.cacheStatus = ((OpFlag)pOp.p5 & OpFlag.OPFLAG_CLEARCACHE) != 0 ? CACHE_STALE : this.cacheCtr;
                        Debug.Assert(payloadSize == 0 || zRecord != null);
                    }
                    else
                    {
                        ///Consider the row to be NULL 
                        payloadSize = 0;
                    }
                }

#endregion

                ///If payloadSize is 0, then just store a NULL 
                if (payloadSize == 0)
                {
                    Debug.Assert((pDest.flags & MemFlags.MEM_Null) != 0);
                    return (int)ColumnResult.op_column_out;
                }
                Debug.Assert(db.aLimit[SQLITE_LIMIT_LENGTH] >= 0);
                if (payloadSize > (u32)db.aLimit[SQLITE_LIMIT_LENGTH])
                {
                    return (int)ColumnResult.too_big;
                }


                Debug.Assert(clumnNumber_p2 < nField);
                ///Read and parse the table header.  Store the results of the parse
                ///into the record header cache fields of the cursor.
                
                if (vdbeCursor.cacheStatus == this.cacheCtr)
                {
                    clumnOffsets = vdbeCursor.aOffset;
                }
                else
                {
                    Debug.Assert(columnTypes != null);
                    avail = 0;
                    //pC.aOffset = aOffset = aType[nField];
                    clumnOffsets = new u32[nField];
                    vdbeCursor.aOffset = clumnOffsets;
                    vdbeCursor.payloadSize = payloadSize;
                    vdbeCursor.cacheStatus = this.cacheCtr;
                    ///Figure out how many bytes are in the header 
                    if (zRecord != null)
                    {
                        zData = zRecord;
                    }
                    else
                    {
                        if (vdbeCursor.isIndex)
                        {
                            zData = btCursor.KeyFetch(ref avail, ref vdbeCursor.aRow);
                        }
                        else
                        {
                            zData = btCursor.DataFetch(ref avail, ref vdbeCursor.aRow);
                        }
                        ///If KeyFetch()/DataFetch() managed to get the entire payload,
                        ///save the payload in the pC.aRow cache.  That will save us from
                        ///having to make additional calls to fetch the content portion of
                        ///the record.
                        Debug.Assert(avail >= 0);
                        if (payloadSize <= (u32)avail)
                        {
                            zRecord = zData;
                            //vdbeCursor.aRow = zData;
                        }
                        else
                        {
                            vdbeCursor.aRow = -1;
                            //pC.aRow = null;
                        }
                    }
                    ///The following Debug.Assert is true in all cases accept when
                    ///the database file has been corrupted externally.
                    ///Debug.Assert( zRec!=0 || avail>=payloadSize || avail>=9 );
                    ///
                    ///Size of the header size field at start of record 
                    int szHdr;
                    szHdr = utilc.getVarint32(zData, out offsetIntoData);
                    ///Make sure a corrupt database has not given us an oversize header.
                    ///Do this now to avoid an oversize memory allocation.
                    ///
                    ///Type entries can be between 1 and 5 bytes each.  But 4 and 5 byte
                    ///types use so much data space that there can only be 4096 and 32 of
                    ///them, respectively.  So the maximum header length results from a
                    ///3">byte type for each of the maximum of 32768 columns plus three</param>
                    ///extra bytes for the header length itself.  32768*3 + 3 = 98307.">extra bytes for the header length itself.  32768*3 + 3 = 98307.</param>
                    ///"></param>
                    if (offsetIntoData > 98307)
                    {
                        rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                        return (int)ColumnResult.op_column_out;
                    }
                    ///Compute in len the number of bytes of data we need to read in order
                    ///to get nField type values.  offset is an upper bound on this.  But
                    ///nField might be significantly less than the true number of columns
                    ///in the table, and in that case, 5*nField+3 might be smaller than offset.
                    ///We want to minimize len in order to limit the size of the memory
                    ///allocation, especially if a corrupt database file has caused offset
                    ///to be oversized. Offset is limited to 98307 above.  But 98307 might
                    ///still exceed Robson memory allocation limits on some configurations.
                    ///On systems that cannot tolerate large memory allocations, nField*5+3
                    ///will likely be much smaller since nField will likely be less than
                    ///20 or so.  This insures that Robson memory allocation limits are
                    ///not exceeded even for corrupt database files.
                    len = nField * 5 + 3;
                    if (len > (int)offsetIntoData)
                        len = (int)offsetIntoData;
                    ///The KeyFetch() or DataFetch() above are fast and will get the entire
                    ///record header in most cases.  But they will fail to get the complete
                    ///record header if the record header does not fit on a single page
                    ///in the B"Tree.  When that happens, use sqlite3VdbeMemFromBtree() to
                    ///acquire the complete header text.
                    if (zRecord == null && avail < len)
                    {
                        sMem.db = null;
                        sMem.flags = 0;
                        rc = vdbemem_cs.sqlite3VdbeMemFromBtree(btCursor, 0, len, vdbeCursor.isIndex, sMem);
                        if (rc != Sqlite3.SQLITE_OK)
                        {
                            return (int)ColumnResult.op_column_out;
                        }
                        zData = sMem.zBLOB;
                    }
                    idxEndHeader = len;
                    // zData[len];
                    idxHeader = szHdr;
                    // zData[szHdr];
                    ///
                    ///Scan the header and use it to fill in the aType[] and aOffset[]
                    ///arrays.  aType[i] will contain the type integer for the i'th
                    ///column and aOffset[i] will contain the offset from the beginning
                    ///of the record to the start of the data for the i"th column
                    ///
                    ///Tural: zData 
                    ///[0]          : number of columns
                    ///[1-num]      : type of the column
                    ///[num,2*num]  : data of the column
                    for (i = 0; i < nField; i++)
                    {
                        if (idxHeader < idxEndHeader)
                        {
                            clumnOffsets[i] = offsetIntoData;
                            idxHeader += utilc.getVarint32(zData, idxHeader, out columnTypes[i]);
                            //utilc.getVarint32(zIdx, aType[i]);
                            szField = vdbeaux.sqlite3VdbeSerialTypeLen(columnTypes[i]);
                            offsetIntoData += szField;
                            if (offsetIntoData < szField)
                            {
                                ///True if offset overflows 
                                idxHeader = int.MaxValue;
                                ///Forces SQLITE_CORRUPT return below 
                                break;
                            }
                        }
                        else
                        {
                            ///If i is less that nField, then there are less fields in this
                            ///record than SetNumColumns indicated there are columns in the
                            ///table. Set the offset for any extra columns not present in
                            ///the record to 0. This tells code below to store a NULL
                            ///instead of deserializing a value from the record.
                            clumnOffsets[i] = 0;
                        }
                    }
                    sMem.sqlite3VdbeMemRelease();
                    sMem.flags = MemFlags.MEM_Null;
                    ///If we have read more header data than was contained in the header,
                    ///or if the end of the last field appears to be past the end of the
                    ///record, or if the end of the last field appears to be before the end
                    ///of the record (when all fields present), then we must be dealing
                    ///with a corrupt database.
                    if ((idxHeader > idxEndHeader) || (offsetIntoData > payloadSize) || (idxHeader == idxEndHeader && offsetIntoData != payloadSize))
                    {
                        rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                        return (int)ColumnResult.op_column_out;
                    }
                }
                ///Get the column information. If aOffset[p2] is non"zero, then
                ///deserialize the value from the record. If aOffset[p2] is zero,
                ///then there are not enough fields in the record to satisfy the">then there are not enough fields in the record to satisfy the</param>
                ///request.  In this case, set the value NULL or to P4 if P4 is">request.  In this case, set the value NULL or to P4 if P4 is</param>
                ///a pointer to a Mem object.
                if (clumnOffsets[clumnNumber_p2] != 0)
                {
                    Debug.Assert(rc == Sqlite3.SQLITE_OK);
                    if (zRecord != null)
                    {
                        pDest.sqlite3VdbeMemReleaseExternal();
                        vdbeaux.sqlite3VdbeSerialGet(zRecord, (int)clumnOffsets[clumnNumber_p2], columnTypes[clumnNumber_p2], pDest);
                    }
                    else
                    {
                        len = (int)vdbeaux.sqlite3VdbeSerialTypeLen(columnTypes[clumnNumber_p2]);
                        vdbemem_cs.sqlite3VdbeMemMove(sMem, pDest);
                        rc = vdbemem_cs.sqlite3VdbeMemFromBtree(btCursor, (int)clumnOffsets[clumnNumber_p2], len, vdbeCursor.isIndex, sMem);
                        if (rc != Sqlite3.SQLITE_OK)
                        {
                            return (int)ColumnResult.op_column_out;
                        }
                        zData = sMem.zBLOB;
                        sMem.zBLOB = null;
                        vdbeaux.sqlite3VdbeSerialGet(zData, columnTypes[clumnNumber_p2], pDest);
                    }
                    pDest.enc = encoding;
                }
                else
                {
                    if (pOp.p4type ==  P4Usage.P4_MEM)
                    {
                        vdbemem_cs.sqlite3VdbeMemShallowCopy(pDest, pOp.p4.pMem, MemFlags.MEM_Static);
                    }
                    else
                    {
                        Debug.Assert((pDest.flags & MemFlags.MEM_Null) != 0);
                    }
                }
                ///If we dynamically allocated space to hold the data (in the
                ///sqlite3VdbeMemFromBtree() call above) then transfer control of that
                ///dynamically allocated space over to the pDest structure.
                ///This prevents a memory copy.
                //if ( sMem.zMalloc != null )
                //{
                //  Debug.Assert( sMem.z == sMem.zMalloc);
                //  Debug.Assert( sMem.xDel == null );
                //  Debug.Assert( ( pDest.flags & MEM.MEM_Dyn ) == 0 );
                //  Debug.Assert( ( pDest.flags & ( MEM.MEM_Blob | MEM.MEM_Str ) ) == 0 || pDest.z == sMem.z );
                //  pDest.flags &= ~( MEM.MEM_Ephem | MEM.MEM_Static );
                //  pDest.flags |= MEM.MEM_Term;
                //  pDest.z = sMem.z;
                //  pDest.zMalloc = sMem.zMalloc;
                //}
                rc = pDest.sqlite3VdbeMemMakeWriteable();
            op_column_out:
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pDest );
#endif
                REGISTER_TRACE(this, pOp.p3, pDest);
                if (zData != null && zData != zRecord)
                    malloc_cs.sqlite3_free(ref zData);
                //malloc_cs.sqlite3_free( ref zRec );
                malloc_cs.sqlite3_free(ref sMem);
                return rc;
            }

            private yDbMask OpCode_ResultRow(int opcodeIndex, int dataOffset, int columnCount, int rc, Mem[] memoryBuffer)
            {
                //Mem[] pMem;
                int i;
               
                ///
                ///<summary>
                ///If this statement has violated immediate foreign key constraints, do
                ///not return the number of rows modified. And do not RELEASE the statement
                ///transaction. It needs to be rolled back.  
                ///</summary>
                if (Sqlite3.SQLITE_OK != (rc = this.sqlite3VdbeCheckFk(0)))
                {
                    Debug.Assert((db.flags & SqliteFlags.SQLITE_CountRows) != 0);
                    Debug.Assert(this.usesStmtJournal);
                    return rc;
                }
                ///
                ///<summary>
                ///If the SQLITE_CountRows flag is set in sqlite3.flags mask, then
                ///DML statements invoke this opcode to return the number of rows
                ///modified to the user. This is the only way that a VM that
                ///opens a statement transaction may invoke this opcode.
                ///
                ///In case this is such a statement, close any statement transaction
                ///opened by this VM before returning control to the user. This is to
                ///</summary>
                ///<param name="ensure that statement">transactions are always nested, not overlapping.</param>
                ///<param name="If the open statement">transaction is not closed here, then the user</param>
                ///<param name="may step another VM that opens its own statement transaction. This">may step another VM that opens its own statement transaction. This</param>
                ///<param name="may lead to overlapping statement transactions.">may lead to overlapping statement transactions.</param>
                ///<param name=""></param>
                ///<param name="The statement transaction is never a top">level transaction.  Hence</param>
                ///<param name="the RELEASE call below can never fail.">the RELEASE call below can never fail.</param>
                ///<param name=""></param>
                Debug.Assert(this.iStatement == 0 || (db.flags & SqliteFlags.SQLITE_CountRows) != 0);
                rc = this.sqlite3VdbeCloseStatement(sqliteinth.SAVEPOINT_RELEASE);
                if (NEVER(rc != Sqlite3.SQLITE_OK))
                {
                    return rc;
                }
                ///
                ///<summary>
                ///Invalidate all ephemeral cursor row caches 
                ///</summary>
                this.cacheCtr = (this.cacheCtr + 2) | 1;
                ///
                ///<summary>
                ///Make sure the results of the current row are \000 terminated
                ///</summary>
                ///<param name="and have an assigned type.  The results are de">ephemeralized as</param>
                ///<param name="as side effect.">as side effect.</param>
                ///<param name=""></param>
                //pMem = p.pResultSet = aMem[pOp.p1];
                this.pResultSet = new Mem[columnCount];
                for (i = 0; i < columnCount; i++)
                {
                    this.pResultSet[i] = memoryBuffer[dataOffset + i];
                    Debug.Assert(this.pResultSet[i].memIsValid());
                    //Deephemeralize( p.pResultSet[i] );
                    //Debug.Assert( ( p.pResultSet[i].flags & MEM.MEM_Ephem ) == 0
                    //        || ( p.pResultSet[i].flags & ( MEM.MEM_Str | MEM.MEM_Blob ) ) == 0 );
                    vdbemem_cs.sqlite3VdbeMemNulTerminate(this.pResultSet[i]);
                    //sqlite3VdbeMemNulTerminate(pMem[i]);
                    sqlite3VdbeMemStoreType(this.pResultSet[i]);
                    REGISTER_TRACE(this, dataOffset + i, this.pResultSet[i]);
                }
                //      if ( db.mallocFailed != 0 ) goto no_mem;
                ///
                ///<summary>
                ///Return SQLITE_ROW
                ///
                ///</summary>
                this.currentOpCodeIndex = opcodeIndex + 1;
                rc = SQLITE_ROW;
                return rc;
            }

            private static void OpCode_AndOr(Op pOp, Mem[] aMem, ref Mem pIn1, ref Mem pIn2, ref Mem pOut)
            {
                ///
                ///<summary>
                ///same as Sqlite3.TK_OR, in1, in2, ref3 
                ///</summary>
                int v1;
                ///
                ///<summary>
                ///Left operand:  0==FALSE, 1==TRUE, 2==UNKNOWN or NULL 
                ///</summary>
                int v2;
                ///
                ///<summary>
                ///Right operand: 0==FALSE, 1==TRUE, 2==UNKNOWN or NULL 
                ///</summary>
                pIn1 = aMem[pOp.p1];
                if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                {
                    v1 = 2;
                }
                else
                {
                    v1 = (pIn1.sqlite3VdbeIntValue() != 0) ? 1 : 0;
                }
                pIn2 = aMem[pOp.p2];
                if ((pIn2.flags & MemFlags.MEM_Null) != 0)
                {
                    v2 = 2;
                }
                else
                {
                    v2 = (pIn2.sqlite3VdbeIntValue() != 0) ? 1 : 0;
                }
                if (pOp.OpCode == OpCode.OP_And)
                {
                    byte[] and_logic = new byte[] {
									0,
									0,
									0,
									0,
									1,
									2,
									0,
									2,
									2
								};
                    v1 = and_logic[v1 * 3 + v2];
                }
                else
                {
                    byte[] or_logic = new byte[] {
									0,
									1,
									2,
									1,
									1,
									1,
									2,
									1,
									2
								};
                    v1 = or_logic[v1 * 3 + v2];
                }
                pOut = aMem[pOp.p3];
                if (v1 == 2)
                {
                    pOut.MemSetTypeFlag(MemFlags.MEM_Null);
                }
                else
                {
                    pOut.u.i = v1;
                    pOut.MemSetTypeFlag(MemFlags.MEM_Int);
                }
            }

            private void OCode_Compare(Op pOp, Mem[] aMem, ref int iCompare, ref int[] aPermute)
            {
                int n;
                int i;
                int p1;
                int p2;
                KeyInfo pKeyInfo;
                int idx;
                CollSeq pColl;
                ///
                ///<summary>
                ///Collating sequence to use on this term 
                ///</summary>
                SortOrder bRev;
                ///
                ///<summary>
                ///True for DESCENDING sort order 
                ///</summary>
                n = pOp.p3;
                pKeyInfo = pOp.p4.pKeyInfo;
                Debug.Assert(n > 0);
                Debug.Assert(pKeyInfo != null);
                p1 = pOp.p1;
                p2 = pOp.p2;
#if SQLITE_DEBUG
																																																																																																																																				              if ( aPermute != null )
              {
                int k, mx = 0;
                for ( k = 0; k < n; k++ )
                  if ( aPermute[k] > mx )
                    mx = aPermute[k];
                Debug.Assert( p1 > 0 && p1 + mx <= p.nMem + 1 );
                Debug.Assert( p2 > 0 && p2 + mx <= p.nMem + 1 );
              }
              else
              {
                Debug.Assert( p1 > 0 && p1 + n <= p.nMem + 1 );
                Debug.Assert( p2 > 0 && p2 + n <= p.nMem + 1 );
              }
#endif
                for (i = 0; i < n; i++)
                {
                    idx = aPermute != null ? aPermute[i] : i;
                    Debug.Assert(aMem[p1 + idx].memIsValid());
                    Debug.Assert(aMem[p2 + idx].memIsValid());
                    REGISTER_TRACE(this, p1 + idx, aMem[p1 + idx]);
                    REGISTER_TRACE(this, p2 + idx, aMem[p2 + idx]);
                    Debug.Assert(i < pKeyInfo.nField);
                    pColl = pKeyInfo.aColl[i];
                    bRev = pKeyInfo.aSortOrder[i];
                    iCompare = vdbemem_cs.sqlite3MemCompare(aMem[p1 + idx], aMem[p2 + idx], pColl);
                    if (iCompare != 0)
                    {
                        if (bRev != 0)
                            iCompare = -iCompare;
                        break;
                    }
                }
                aPermute = null;
            }

            private void OpCode_SCopy(Op pOp, Mem[] aMem, ref Mem pIn1, ref Mem pOut)
            {
                ///
                ///<summary>
                ///in1, ref2 
                ///</summary>
                pIn1 = aMem[pOp.p1];
                pOut = aMem[pOp.p2];
                Debug.Assert(pOut != pIn1);
                vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pIn1, MemFlags.MEM_Ephem);
#if SQLITE_DEBUG
																																																																																																																																				              if ( pOut.pScopyFrom == null )
                pOut.pScopyFrom = pIn1;
#endif
                REGISTER_TRACE(this, pOp.p2, pOut);
            }

            private void OpCode_Move(Op pOp, Mem[] aMem, ref Mem pIn1, ref Mem pOut)
            {
                //char* zMalloc;   /* Holding variable for allocated memory */
                int n;
                ///
                ///<summary>
                ///Number of registers left to copy 
                ///</summary>
                int p1;
                ///
                ///<summary>
                ///Register to copy from 
                ///</summary>
                int p2;
                ///
                ///<summary>
                ///Register to copy to 
                ///</summary>
                n = pOp.p3;
                p1 = pOp.p1;
                p2 = pOp.p2;
                Debug.Assert(n > 0 && p1 > 0 && p2 > 0);
                Debug.Assert(p1 + n <= p2 || p2 + n <= p1);
                //pIn1 = aMem[p1];
                //pOut = aMem[p2];
                while (n-- != 0)
                {
                    pIn1 = aMem[p1 + pOp.p3 - n - 1];
                    pOut = aMem[p2];
                    //Debug.Assert( pOut<=&aMem[p.nMem] );
                    //Debug.Assert( pIn1<=&aMem[p.nMem] );
                    Debug.Assert(pIn1.memIsValid());
                    memAboutToChange(this, pOut);
                    //zMalloc = pOut.zMalloc;
                    //pOut.zMalloc = null;
                    vdbemem_cs.sqlite3VdbeMemMove(pOut, pIn1);
                    //pIn1.zMalloc = zMalloc;
                    REGISTER_TRACE(this, p2++, pOut);
                    //pIn1++;
                    //pOut++;
                }
            }

            private static void OpCode_Integer(Op pOp, Mem pOut)
            {
                ///
                ///<summary>
                ///</summary>
                ///<param name="out2">prerelease </param>
                pOut.u.i = pOp.p1;
            }

            private void OpCode_Yield(ref int opcodeIndex, Op pOp, Mem[] aMem, ref Mem pIn1)
            {
                ///
                ///<summary>
                ///in1 
                ///</summary>
                int pcDest;
                pIn1 = aMem[pOp.p1];
                Debug.Assert((pIn1.flags & MemFlags.MEM_Dyn) == 0);
                pIn1.flags = MemFlags.MEM_Int;
                pcDest = (int)pIn1.u.i;
                pIn1.u.i = opcodeIndex;
                REGISTER_TRACE(this, pOp.p1, pIn1);
                opcodeIndex = pcDest;
            }

            private static void OpCode_Return(ref int opcodeIndex, Op pOp, Mem[] aMem, ref Mem pIn1)
            {
                ///
                ///<summary>
                ///in1 
                ///</summary>
                pIn1 = aMem[pOp.p1];
                Debug.Assert((pIn1.flags & MemFlags.MEM_Int) != 0);
                opcodeIndex = (int)pIn1.u.i;
            }


            ///<summary>
            /// Create a new virtual database engine.
            ///</summary>
            public static Vdbe Create(sqlite3 db)
            {
                Vdbe p;
                p = new Vdbe();
                // sqlite3DbMallocZero(db, Vdbe).Length;
                if (p == null)
                    return null;
                p.db = db;
                if (db.pVdbe != null)
                {
                    db.pVdbe.pPrev = p;
                }
                p.pNext = db.pVdbe;
                p.pPrev = null;
                db.pVdbe = p;
                p.magic = VdbeMagic.VDBE_MAGIC_INIT;
                return p;
            }

#if !NDEBUG
																																						    //void sqlite3VdbeComment(Vdbe*, const char*, ...);
    static void VdbeComment( Vdbe v, string zFormat, params object[] ap )
    {
      sqlite3VdbeComment( v, zFormat, ap );
    }// define VdbeComment(X)  sqlite3VdbeComment X
    //void sqlite3VdbeNoopComment(Vdbe*, const char*, ...);
    static void VdbeNoopComment( Vdbe v, string zFormat, params object[] ap )
    {
      sqlite3VdbeNoopComment( v, zFormat, ap );
    }// define VdbeNoopComment(X)  sqlite3VdbeNoopComment X
#else
            //# define VdbeComment(X)
            public void VdbeComment( string zFormat, params object[] ap)
            {
            }

            //# define VdbeNoopComment(X)
            public void VdbeNoopComment( string zFormat, params object[] ap)
            {
            }
#endif

            


        }
	

#endif
	}
}
