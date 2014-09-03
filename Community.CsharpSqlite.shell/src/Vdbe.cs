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
/*
** The yDbMask datatype for the bitmask of all attached databases.
*/
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
	using sqlite3_value=Sqlite3.Mem;
	using System.Collections.Generic;
	public partial class Sqlite3 {
		/*
    ** An instance of the virtual machine.  This structure contains the complete
    ** state of the virtual machine.
    **
    ** The "sqlite3_stmt" structure pointer that is returned by sqlite3_prepare()
    ** is really a pointer to an instance of this structure.
    **
    ** The Vdbe.inVtabMethod variable is set to non-zero for the duration of
    ** any virtual table method invocations made by the vdbe program. It is
    ** set to 2 for xDestroy method calls and 1 for all other methods. This
    ** variable is used for two purposes: to allow xDestroy methods to execute
    ** "DROP TABLE" statements and to prevent some nasty side effects of
    ** malloc failure when SQLite is invoked recursively by a virtual table
    ** method function.
    */public class Vdbe {
			public Vdbe() {
			}
			/* The database connection that owns this statement */public sqlite3 db;
			/** Space to hold the virtual machine's program */public Op[] aOp;
			/* The memory locations */public List<Op> lOp=new List<Op>();
			public Mem[] aMem;
			public Mem[] apArg;
			/* Arguments to currently executing user function */public Mem[] aColName;
			/* Column names to return */public Mem[] pResultSet;
			/* Pointer to an array of results */public int nMem;
			/* Number of memory locations currently allocated */public int nOp;
			/* Number of instructions in the program */public int nOpAlloc;
			/* Number of slots allocated for aOp[] */public int nLabel;
			/* Number of labels used */public int nLabelAlloc;
			/* Number of slots allocated in aLabel[] */public int[] aLabel;
			/* Space to hold the labels */public u16 nResColumn;
			/* Number of columns in one row of the result set */public u16 nCursor;
			/* Number of slots in apCsr[] */public u32 magic;
			/* Magic number for sanity checking */public string zErrMsg;
			/* Error message written here */public Vdbe pPrev;
			/* Linked list of VDBEs with the same Vdbe.db */public Vdbe pNext;
			/* Linked list of VDBEs with the same Vdbe.db */public VdbeCursor[] apCsr;
			/* One element of this array for each open cursor */public Mem[] aVar;
			/* Values for the OP_Variable opcode. */public string[] azVar;
			/* Name of variables */public ynVar nVar;
			/* Number of entries in aVar[] */public ynVar nzVar;
			/* Number of entries in azVar[] */public u32 cacheCtr;
			/* VdbeCursor row cache generation counter *//// <summary>
			/// The program counter 
			/// </summary>
			public int currentOpCodeIndex;
			/// <summary>
			/// Value to return
			/// </summary>
			public int rc;
			public u8 errorAction;
			/* Recovery action to do in case of an error */public int explain;
			/* True if EXPLAIN present on SQL command */public bool changeCntOn;
			/* True to update the change-counter */public bool expired;
			/* True if the VM needs to be recompiled */public u8 runOnlyOnce;
			/* Automatically expire on reset */public int minWriteFileFormat;
			/* Minimum file format for writable database files */public int inVtabMethod;
			/* See comments above */public bool usesStmtJournal;
			/* True if uses a statement journal */public bool readOnly;
			/* True for read-only statements */public int nChange;
			/* Number of db changes made since last reset */public bool isPrepareV2;
			/* True if prepared with prepare_v2() */public yDbMask btreeMask;
			/* Bitmask of db.aDb[] entries referenced */public yDbMask lockMask;
			/* Subset of btreeMask that requires a lock */public int iStatement;
			/* Statement number (or 0 if has not opened stmt) */public int[] aCounter=new int[3];
			/* Counters used by sqlite3_stmt_status() */
			#if !SQLITE_OMIT_TRACE
			public i64 startTime;
			/* Time when query started - used for profiling */
			#endif
			public i64 nFkConstraint;
			/* Number of imm. FK constraints this VM */public i64 nStmtDefCons;
			/* Number of def. constraints when stmt started */public string zSql="";
			/* Text of the SQL statement that generated this */public object pFree;
			/* Free this when deleting the vdbe */
			#if SQLITE_DEBUG
																																																																								      public FILE trace;             /* Write an execution trace here, if not NULL */
#endif
			public VdbeFrame pFrame;
			/* Parent frame */public VdbeFrame pDelFrame;
			/* List of frame objects to free on VM reset */public int nFrame;
			/* Number of frames in pFrame list */public u32 expmask;
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
			//int sqlite3VdbeMemStringify(Mem*, int);
			//i64 sqlite3VdbeIntValue(Mem);
			//int sqlite3VdbeMemIntegerify(Mem);
			//double sqlite3VdbeRealValue(Mem);
			//void sqlite3VdbeIntegerAffinity(Mem);
			//int sqlite3VdbeMemRealify(Mem);
			//int sqlite3VdbeMemNumerify(Mem);
			//int sqlite3VdbeMemFromBtree(BtCursor*,int,int,int,Mem);
			//void sqlite3VdbeMemRelease(Mem p);
			//void sqlite3VdbeMemReleaseExternal(Mem p);
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
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				//Debug.Assert(op>0&&op<0xff);
				if(this.nOpAlloc<=i) {
					if(this.growOpArray()!=0) {
						return 1;
					}
				}
				this.nOp++;
				pOp=new VdbeOp();
				pOp.opcode=(u8)op;
				pOp.p5=0;
				pOp.p1=p1;
				pOp.p2=p2;
				pOp.p3=p3;
				pOp.p4.p=null;
				pOp.p4type=P4_NOTUSED;
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
			public int sqlite3VdbeAddOp0(int op) {
				return this.sqlite3VdbeAddOp3(op,0,0,0);
			}
			public int sqlite3VdbeAddOp1(int op,int p1) {
				return this.sqlite3VdbeAddOp3(op,p1,0,0);
			}
			public int sqlite3VdbeAddOp2(int op,int p1,bool b2) {
				return this.sqlite3VdbeAddOp2(op,p1,(int)(b2?1:0));
			}
			public int sqlite3VdbeAddOp2(int op,int p1,int p2) {
				return this.sqlite3VdbeAddOp3(op,p1,p2,0);
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i32 pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.i=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,char pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,StringBuilder pP4,int p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,string pP4,int p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,byte[] pP4,int p4type) {
				Debug.Assert(op==OP_Null||pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.z=Encoding.UTF8.GetString(pP4,0,pP4.Length);
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,int[] pP4,int p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.ai=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i64 pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pI64=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,double pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pReal=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,FuncDef pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,CollSeq pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,KeyInfo pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,VTable pP4,int p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.pVtab=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4Int(/* Add the opcode to this VM */int op,/* The new opcode */int p1,/* The P1 operand */int p2,/* The P2 operand */int p3,/* The P3 operand */int p4/* The P4 operand as an integer */) {
				union_p4 _p4=new union_p4();
				_p4.i=p4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,P4_INT32);
				return addr;
			}
			public int sqlite3VdbeMakeLabel() {
				int i;
				i=this.nLabel++;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(i>=this.nLabelAlloc) {
					int n=this.nLabelAlloc==0?15:this.nLabelAlloc*2+5;
					if(this.aLabel==null)
						this.aLabel=sqlite3Malloc(this.aLabel,n);
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
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
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
					u8 opcode=pOp.opcode;
					pOp.opflags=(u8)sqlite3OpcodeProperty[opcode];
					if(opcode==OP_Function||opcode==OP_AggStep) {
						if(pOp.p5>nMaxArgs)
							nMaxArgs=pOp.p5;
					}
					else
						if((opcode==OP_Transaction&&pOp.p2!=0)||opcode==OP_Vacuum) {
							this.readOnly=false;
							#if !SQLITE_OMIT_VIRTUALTABLE
						}
						else
							if(opcode==OP_VUpdate) {
								if(pOp.p2>nMaxArgs)
									nMaxArgs=pOp.p2;
							}
							else
								if(opcode==OP_VFilter) {
									int n;
									Debug.Assert(this.nOp-i>=3);
									Debug.Assert(this.lOp[i-1].opcode==OP_Integer);
									//pOp[-1].opcode==OP_Integer );
									n=this.lOp[i-1].p1;
									//pOp[-1].p1;
									if(n>nMaxArgs)
										nMaxArgs=n;
									#endif
								}
					if((pOp.opflags&OPFLG_JUMP)!=0&&pOp.p2<0) {
						Debug.Assert(-1-pOp.p2<this.nLabel);
						pOp.p2=aLabel[-1-pOp.p2];
					}
				}
				this.db.sqlite3DbFree(ref this.aLabel);
				pMaxFuncArgs=nMaxArgs;
			}
			public int sqlite3VdbeCurrentAddr() {
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				return this.nOp;
			}
			public VdbeOp[] sqlite3VdbeTakeOpArray(ref int pnOp,ref int pnMaxArg) {
				List<VdbeOp> lOp=this.lOp;
				Debug.Assert(aOp!=null);
				// && 0==p.db.mallocFailed );
				/* Check that sqlite3VdbeUsesBtree() was not called on this VM */Debug.Assert(this.btreeMask==0);
				this.resolveP2Values(ref pnMaxArg);
				pnOp=this.nOp;
				this.lOp=null;
				return lOp.ToArray();
			}
			public int sqlite3VdbeAddOpList(int nOp,VdbeOpList[] aOp) {
				int addr;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(this.nOp+nOp>this.nOpAlloc&&this.growOpArray()!=0) {
					return 0;
				}
				addr=this.nOp;
				if(ALWAYS(nOp>0)) {
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
						if(p2<0&&(sqlite3OpcodeProperty[pOut.opcode]&OPFLG_JUMP)!=0) {
							pOut.p2=addr+(-1-p2);
							// ADDR(p2);
						}
						else {
							pOut.p2=p2;
						}
						pOut.p3=pIn.p3;
						pOut.p4type=P4_NOTUSED;
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
			public void sqlite3VdbeJumpHere(int addr) {
				Debug.Assert(addr>=0);
				this.sqlite3VdbeChangeP2(addr,this.nOp);
			}
			public void sqlite3VdbeChangeP4(int addr,CollSeq pColl,int n) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pColl;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,FuncDef pFunc,int n) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pFunc;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,KeyInfo pKeyInfo,int n) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pKeyInfo;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,int i32n,int n) {
				union_p4 _p4=new union_p4();
				_p4.i=i32n;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,char c,int n) {
				union_p4 _p4=new union_p4();
				_p4.z=c.ToString();
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,Mem m,int n) {
				union_p4 _p4=new union_p4();
				_p4.pMem=m;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,string z,dxDel P4_Type) {
				union_p4 _p4=new union_p4();
				_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4,P4_DYNAMIC);
			}
			public void sqlite3VdbeChangeP4(int addr,SubProgram pProgram,int n) {
				union_p4 _p4=new union_p4();
				_p4.pProgram=pProgram;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,string z,int n) {
				union_p4 _p4=new union_p4();
				if(n>0&&n<=z.Length)
					_p4.z=z.Substring(0,n);
				else
					_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,union_p4 _p4,int n) {
				Op pOp;
				sqlite3 db;
				Debug.Assert(this!=null);
				db=this.db;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(this.lOp==null/*|| db.mallocFailed != 0 */) {
					if(n!=P4_KEYINFO&&n!=P4_VTAB) {
						freeP4(db,n,_p4);
					}
					return;
				}
				Debug.Assert(this.nOp>0);
				Debug.Assert(addr<this.nOp);
				if(addr<0) {
					addr=this.nOp-1;
				}
				pOp=this.lOp[addr];
				freeP4(db,pOp.p4type,pOp.p4.p);
				pOp.p4.p=null;
				if(n==P4_INT32) {
					/* Note: this cast is safe, because the origin data point was an int
        ** that was cast to a (string ). */pOp.p4.i=_p4.i;
					// SQLITE_PTR_TO_INT(zP4);
					pOp.p4type=P4_INT32;
				}
				else
					if(n==P4_INT64) {
						pOp.p4.pI64=_p4.pI64;
						pOp.p4type=n;
					}
					else
						if(n==P4_REAL) {
							pOp.p4.pReal=_p4.pReal;
							pOp.p4type=n;
						}
						else
							if(_p4==null) {
								pOp.p4.p=null;
								pOp.p4type=P4_NOTUSED;
							}
							else
								if(n==P4_KEYINFO) {
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
										pOp.p4type=P4_KEYINFO;
									}
									else {
										//p.db.mallocFailed = 1;
										pOp.p4type=P4_NOTUSED;
									}
									pOp.p4.pKeyInfo=_p4.pKeyInfo;
									pOp.p4type=P4_KEYINFO;
								}
								else
									if(n==P4_KEYINFO_HANDOFF||n==P4_KEYINFO_STATIC) {
										pOp.p4.pKeyInfo=_p4.pKeyInfo;
										pOp.p4type=P4_KEYINFO;
									}
									else
										if(n==P4_FUNCDEF) {
											pOp.p4.pFunc=_p4.pFunc;
											pOp.p4type=P4_FUNCDEF;
										}
										else
											if(n==P4_COLLSEQ) {
												pOp.p4.pColl=_p4.pColl;
												pOp.p4type=P4_COLLSEQ;
											}
											else
												if(n==P4_DYNAMIC||n==P4_STATIC||n==P4_MPRINTF) {
													pOp.p4.z=_p4.z;
													pOp.p4type=P4_DYNAMIC;
												}
												else
													if(n==P4_MEM) {
														pOp.p4.pMem=_p4.pMem;
														pOp.p4type=P4_MEM;
													}
													else
														if(n==P4_INTARRAY) {
															pOp.p4.ai=_p4.ai;
															pOp.p4type=P4_INTARRAY;
														}
														else
															if(n==P4_SUBPROGRAM) {
																pOp.p4.pProgram=_p4.pProgram;
																pOp.p4type=P4_SUBPROGRAM;
															}
															else
																if(n==P4_VTAB) {
																	pOp.p4.pVtab=_p4.pVtab;
																	pOp.p4type=P4_VTAB;
																	sqlite3VtabLock(_p4.pVtab);
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
																		pOp.p4type=P4_DYNAMIC;
																	}
			}
			public VdbeOp sqlite3VdbeGetOp(int addr) {
				/* C89 specifies that the constant "dummy" will be initialized to all
      ** zeros, which is correct.  MSVC generates a warning, nevertheless. */Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(addr<0) {
					#if SQLITE_OMIT_TRACE
																																																																																																										if( p.nOp==0 ) return dummy;
#endif
					addr=this.nOp-1;
				}
				Debug.Assert((addr>=0&&addr<this.nOp)/* || p.db.mallocFailed != 0 */);
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
					/* The first time a column affinity string for a particular index is
        ** required, it is allocated and populated here. It is then stored as
        ** a member of the Index structure for subsequent use.
        **
        ** The column affinity string will eventually be deleted by
        ** sqliteDeleteIndex() when the Index structure itself is cleaned
        ** up.
        */int n;
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
					pIdx_zColAff.Append(SQLITE_AFF_NONE);
					pIdx_zColAff.Append('\0');
					pIdx.zColAff=pIdx_zColAff.ToString();
				}
				return pIdx.zColAff;
			}
			public void sqlite3TableAffinityStr(Table pTab) {
				/* The first time a column affinity string for a particular table
      ** is required, it is allocated and populated here. It is then
      ** stored as a member of the Table structure for subsequent use.
      **
      ** The column affinity string will eventually be deleted by
      ** sqlite3DeleteTable() when the Table structure itself is cleaned up.
      */if(pTab.zColAff==null) {
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
				this.sqlite3VdbeChangeP4(-1,pTab.zColAff,P4_TRANSIENT);
			}
			public void sqlite3ColumnDefault(Table pTab,int i,int iReg) {
				Debug.Assert(pTab!=null);
				if(null==pTab.pSelect) {
					sqlite3_value pValue=new sqlite3_value();
					SqliteEncoding enc=ENC(this.sqlite3VdbeDb());
					Column pCol=pTab.aCol[i];
					#if SQLITE_DEBUG
																																																																																																					        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
					Debug.Assert(i<pTab.nCol);
					sqlite3ValueFromExpr(this.sqlite3VdbeDb(),pCol.pDflt,enc,pCol.affinity,ref pValue);
					if(pValue!=null) {
						this.sqlite3VdbeChangeP4(-1,pValue,P4_MEM);
					}
					#if !SQLITE_OMIT_FLOATING_POINT
					if(iReg>=0&&pTab.aCol[i].affinity==SQLITE_AFF_REAL) {
						this.sqlite3VdbeAddOp1(OP_RealAffinity,iReg);
					}
					#endif
				}
			}
			public void sqlite3ExprCodeGetColumnOfTable(/* The VDBE under construction */Table pTab,/* The table containing the value */int iTabCur,/* The cursor for this table */int iCol,/* Index of the column to extract */int regOut/* Extract the value into this register */) {
				if(iCol<0||iCol==pTab.iPKey) {
					this.sqlite3VdbeAddOp2(OP_Rowid,iTabCur,regOut);
				}
				else {
					int op=IsVirtual(pTab)?OP_VColumn:OP_Column;
					this.sqlite3VdbeAddOp3(op,iTabCur,iCol,regOut);
				}
				if(iCol>=0) {
					this.sqlite3ColumnDefault(pTab,iCol,regOut);
				}
			}
			public bool vdbeSafety() {
				if(this.db==null) {
					sqlite3_log(SQLITE_MISUSE,"API called with finalized prepared statement");
					return true;
				}
				else {
					return false;
				}
			}
			public bool vdbeSafetyNotNull() {
				if(this==null) {
					sqlite3_log(SQLITE_MISUSE,"API called with NULL prepared statement");
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
				return (this.aOp!=null?SQLITE_OK:SQLITE_NOMEM);
				//  return (pNew ? SQLITE_OK : SQLITE_NOMEM);
			}
			public void sqlite3VdbeAddParseSchemaOp(int iDb,string zWhere) {
				int j;
				int addr=this.sqlite3VdbeAddOp3(OP_ParseSchema,iDb,0,0);
				this.sqlite3VdbeChangeP4(addr,zWhere,P4_DYNAMIC);
				for(j=0;j<this.db.nDb;j++)
					sqlite3VdbeUsesBtree(this,j);
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
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				/* There should be at least one opcode.
      */Debug.Assert(this.nOp>0);
				/* Set the magic to VDBE_MAGIC_RUN sooner rather than later. */this.magic=VDBE_MAGIC_RUN;
				#if SQLITE_DEBUG
																																																																							      for(i=1; i<p.nMem; i++){
        Debug.Assert( p.aMem[i].db==p.db );
      }
#endif
				this.currentOpCodeIndex=-1;
				this.rc=SQLITE_OK;
				this.errorAction=OE_Abort;
				this.magic=VDBE_MAGIC_RUN;
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
				releaseMemArray(this.aColName,this.nResColumn*COLNAME_N);
				db.sqlite3DbFree(ref this.aColName);
				n=nResColumn*COLNAME_N;
				this.nResColumn=(u16)nResColumn;
				this.aColName=new Mem[n];
				// (Mem)sqlite3DbMallocZero(db, Mem.Length * n);
				//if (p.aColName == 0) return;
				while(n-->0) {
					this.aColName[n]=sqlite3Malloc(this.aColName[n]);
					pColName=this.aColName[n];
					pColName.flags=MEM_Null;
					pColName.db=this.db;
				}
			}
			public int sqlite3VdbeSetColName(/* Vdbe being configured */int idx,/* Index of column zName applies to */int var,/* One of the COLNAME_* constants */string zName,/* Pointer to buffer containing name */dxDel xDel/* Memory management strategy for zName */) {
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
				rc=sqlite3VdbeMemSetStr(pColName,zName,-1,SqliteEncoding.UTF8,xDel);
				Debug.Assert(rc!=0||null==zName||(pColName.flags&MEM_Term)!=0);
				return rc;
			}
			public int sqlite3VdbeCloseStatement(int eOp) {
				sqlite3 db=this.db;
				int rc=SQLITE_OK;
				/* If p->iStatement is greater than zero, then this Vdbe opened a 
      ** statement transaction that should be closed here. The only exception
      ** is that an IO error may have occured, causing an emergency rollback.
      ** In this case (db->nStatement==0), and there is nothing to do.
      */if(db.nStatement!=0&&this.iStatement!=0) {
					int i;
					int iSavepoint=this.iStatement-1;
					Debug.Assert(eOp==SAVEPOINT_ROLLBACK||eOp==SAVEPOINT_RELEASE);
					Debug.Assert(db.nStatement>0);
					Debug.Assert(this.iStatement==(db.nStatement+db.nSavepoint));
					for(i=0;i<db.nDb;i++) {
						int rc2=SQLITE_OK;
						Btree pBt=db.aDb[i].pBt;
						if(pBt!=null) {
							if(eOp==SAVEPOINT_ROLLBACK) {
								rc2=sqlite3BtreeSavepoint(pBt,SAVEPOINT_ROLLBACK,iSavepoint);
							}
							if(rc2==SQLITE_OK) {
								rc2=sqlite3BtreeSavepoint(pBt,SAVEPOINT_RELEASE,iSavepoint);
							}
							if(rc==SQLITE_OK) {
								rc=rc2;
							}
						}
					}
					db.nStatement--;
					this.iStatement=0;
					if(rc==SQLITE_OK) {
						if(eOp==SAVEPOINT_ROLLBACK) {
							rc=sqlite3VtabSavepoint(db,SAVEPOINT_ROLLBACK,iSavepoint);
						}
						if(rc==SQLITE_OK) {
							rc=sqlite3VtabSavepoint(db,SAVEPOINT_RELEASE,iSavepoint);
						}
					}
					/* If the statement transaction is being rolled back, also restore the 
        ** database handles deferred constraint counter to the value it had when 
        ** the statement transaction was opened.  */if(eOp==SAVEPOINT_ROLLBACK) {
						db.nDeferredCons=this.nStmtDefCons;
					}
				}
				return rc;
			}
			public int sqlite3VdbeCheckFk(int deferred) {
				sqlite3 db=this.db;
				if((deferred!=0&&db.nDeferredCons>0)||(0==deferred&&this.nFkConstraint>0)) {
					this.rc=SQLITE_CONSTRAINT;
					this.errorAction=OE_Abort;
					sqlite3SetString(ref this.zErrMsg,db,"foreign key constraint failed");
					return SQLITE_ERROR;
				}
				return SQLITE_OK;
			}
			public int sqlite3VdbeHalt() {
				int rc;
				/* Used to store transient return codes */sqlite3 db=this.db;
				/* This function contains the logic that determines if a statement or
      ** transaction will be committed or rolled back as a result of the
      ** execution of this virtual machine.
      **
      ** If any of the following errors occur:
      **
      **     SQLITE_NOMEM
      **     SQLITE_IOERR
      **     SQLITE_FULL
      **     SQLITE_INTERRUPT
      **
      ** Then the internal cache might have been left in an inconsistent
      ** state.  We need to rollback the statement transaction, if there is
      ** one, or the complete transaction if there is no statement transaction.
      *///if ( p.db.mallocFailed != 0 )
				//{
				//  p.rc = SQLITE_NOMEM;
				//}
				closeAllCursors(this);
				if(this.magic!=VDBE_MAGIC_RUN) {
					return SQLITE_OK;
				}
				checkActiveVdbeCnt(db);
				/* No commit or rollback needed if the program never started */if(this.currentOpCodeIndex>=0) {
					int mrc;
					/* Primary error code from p.rc */int eStatementOp=0;
					bool isSpecialError=false;
					/* Set to true if a 'special' error *//* Lock all btrees used by the statement */this.sqlite3VdbeEnter();
					/* Check for one of the special errors */mrc=this.rc&0xff;
					Debug.Assert(this.rc!=SQLITE_IOERR_BLOCKED);
					/* This error no longer exists */isSpecialError=mrc==SQLITE_NOMEM||mrc==SQLITE_IOERR||mrc==SQLITE_INTERRUPT||mrc==SQLITE_FULL;
					if(isSpecialError) {
						/* If the query was read-only and the error code is SQLITE_INTERRUPT, 
          ** no rollback is necessary. Otherwise, at least a savepoint 
          ** transaction must be rolled back to restore the database to a 
          ** consistent state.
          **
          ** Even if the statement is read-only, it is important to perform
          ** a statement or transaction rollback operation. If the error 
          ** occured while writing to the journal, sub-journal or database
          ** file as part of an effort to free up cache space (see function
          ** pagerStress() in pager.c), the rollback is required to restore 
          ** the pager to a consistent state.
          */if(!this.readOnly||mrc!=SQLITE_INTERRUPT) {
							if((mrc==SQLITE_NOMEM||mrc==SQLITE_FULL)&&this.usesStmtJournal) {
								eStatementOp=SAVEPOINT_ROLLBACK;
							}
							else {
								/* We are forced to roll back the active transaction. Before doing
              ** so, abort any other statements this handle currently has active.
              */invalidateCursorsOnModifiedBtrees(db);
								sqlite3RollbackAll(db);
								sqlite3CloseSavepoints(db);
								db.autoCommit=1;
							}
						}
					}
					/* Check for immediate foreign key violations. */if(this.rc==SQLITE_OK) {
						this.sqlite3VdbeCheckFk(0);
					}
					/* If the auto-commit flag is set and this is the only active writer
        ** VM, then we do either a commit or rollback of the current transaction.
        **
        ** Note: This block also runs if one of the special errors handled
        ** above has occurred.
        */if(!sqlite3VtabInSync(db)&&db.autoCommit!=0&&db.writeVdbeCnt==((this.readOnly==false)?1:0)) {
						if(this.rc==SQLITE_OK||(this.errorAction==OE_Fail&&!isSpecialError)) {
							rc=this.sqlite3VdbeCheckFk(1);
							if(rc!=SQLITE_OK) {
								if(NEVER(this.readOnly)) {
									this.sqlite3VdbeLeave();
									return SQLITE_ERROR;
								}
								rc=SQLITE_CONSTRAINT;
							}
							else {
								/* The auto-commit flag is true, the vdbe program was successful 
              ** or hit an 'OR FAIL' constraint and there are no deferred foreign
              ** key constraints to hold up the transaction. This means a commit 
              ** is required. */rc=vdbeCommit(db,this);
							}
							if(rc==SQLITE_BUSY&&this.readOnly) {
								this.sqlite3VdbeLeave();
								return SQLITE_BUSY;
							}
							else
								if(rc!=SQLITE_OK) {
									this.rc=rc;
									sqlite3RollbackAll(db);
								}
								else {
									db.nDeferredCons=0;
									sqlite3CommitInternalChanges(db);
								}
						}
						else {
							sqlite3RollbackAll(db);
						}
						db.nStatement=0;
					}
					else
						if(eStatementOp==0) {
							if(this.rc==SQLITE_OK||this.errorAction==OE_Fail) {
								eStatementOp=SAVEPOINT_RELEASE;
							}
							else
								if(this.errorAction==OE_Abort) {
									eStatementOp=SAVEPOINT_ROLLBACK;
								}
								else {
									invalidateCursorsOnModifiedBtrees(db);
									sqlite3RollbackAll(db);
									sqlite3CloseSavepoints(db);
									db.autoCommit=1;
								}
						}
					/* If eStatementOp is non-zero, then a statement transaction needs to
        ** be committed or rolled back. Call sqlite3VdbeCloseStatement() to
        ** do so. If this operation returns an error, and the current statement
        ** error code is SQLITE_OK or SQLITE_CONSTRAINT, then promote the
        ** current statement error code.
        */if(eStatementOp!=0) {
						rc=this.sqlite3VdbeCloseStatement(eStatementOp);
						if(rc!=0) {
							if(this.rc==SQLITE_OK||this.rc==SQLITE_CONSTRAINT) {
								this.rc=rc;
								db.sqlite3DbFree(ref this.zErrMsg);
								this.zErrMsg=null;
							}
							invalidateCursorsOnModifiedBtrees(db);
							sqlite3RollbackAll(db);
							sqlite3CloseSavepoints(db);
							db.autoCommit=1;
						}
					}
					/* If this was an INSERT, UPDATE or DELETE and no statement transaction
        ** has been rolled back, update the database connection change-counter.
        */if(this.changeCntOn) {
						if(eStatementOp!=SAVEPOINT_ROLLBACK) {
							sqlite3VdbeSetChanges(db,this.nChange);
						}
						else {
							sqlite3VdbeSetChanges(db,0);
						}
						this.nChange=0;
					}
					/* Rollback or commit any schema changes that occurred. */if(this.rc!=SQLITE_OK&&(db.flags&SQLITE_InternChanges)!=0) {
						sqlite3ResetInternalSchema(db,-1);
						db.flags=(db.flags|SQLITE_InternChanges);
					}
					/* Release the locks */this.sqlite3VdbeLeave();
				}
				/* We have successfully halted and closed the VM.  Record this fact. */if(this.currentOpCodeIndex>=0) {
					db.activeVdbeCnt--;
					if(!this.readOnly) {
						db.writeVdbeCnt--;
					}
					Debug.Assert(db.activeVdbeCnt>=db.writeVdbeCnt);
				}
				this.magic=VDBE_MAGIC_HALT;
				checkActiveVdbeCnt(db);
				//if ( p.db.mallocFailed != 0 )
				//{
				//  p.rc = SQLITE_NOMEM;
				//}
				/* If the auto-commit flag is set to true, then any locks that were held
      ** by connection db have now been released. Call sqlite3ConnectionUnlocked()
      ** to invoke any required unlock-notify callbacks.
      */if(db.autoCommit!=0) {
					sqlite3ConnectionUnlocked(db);
				}
				Debug.Assert(db.activeVdbeCnt>0||db.autoCommit==0||db.nStatement==0);
				return (this.rc==SQLITE_BUSY?SQLITE_BUSY:SQLITE_OK);
			}
			public void sqlite3VdbeResetStepResult() {
				this.rc=SQLITE_OK;
			}
			public int sqlite3VdbeReset() {
				sqlite3 db;
				db=this.db;
				/* If the VM did not run to completion or if it encountered an
      ** error, then it might not have been halted properly.  So halt
      ** it now.
      */this.sqlite3VdbeHalt();
				/* If the VDBE has be run even partially, then transfer the error code
      ** and error message from the VDBE into the main database structure.  But
      ** if the VDBE has just been set to run but has not actually executed any
      ** instructions yet, leave the main database error information unchanged.
      */if(this.currentOpCodeIndex>=0) {
					//if ( p.zErrMsg != 0 ) // Always exists under C#
					{
						sqlite3BeginBenignMalloc();
						sqlite3ValueSetStr(db.pErr,-1,this.zErrMsg==null?"":this.zErrMsg,SqliteEncoding.UTF8,SQLITE_TRANSIENT);
						sqlite3EndBenignMalloc();
						db.errCode=this.rc;
						db.sqlite3DbFree(ref this.zErrMsg);
						this.zErrMsg="";
						//else if ( p.rc != 0 )
						//{
						//  sqlite3Error( db, p.rc, 0 );
						//}
						//else
						//{
						//  sqlite3Error( db, SQLITE_OK, 0 );
						//}
					}
					if(this.runOnlyOnce!=0)
						this.expired=true;
				}
				else
					if(this.rc!=0&&this.expired) {
						/* The expired flag was set on the VDBE before the first call
        ** to sqlite3_step(). For consistency (since sqlite3_step() was
        ** called), set the database error in this case as well.
        */sqlite3Error(db,this.rc,0);
						sqlite3ValueSetStr(db.pErr,-1,this.zErrMsg,SqliteEncoding.UTF8,SQLITE_TRANSIENT);
						db.sqlite3DbFree(ref this.zErrMsg);
						this.zErrMsg="";
					}
				/* Reclaim all memory used by the VDBE
      */Cleanup(this);
				/* Save profiling information from this VDBE run.
      */
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
				this.magic=VDBE_MAGIC_INIT;
				return this.rc&db.errMask;
			}
			public sqlite3 sqlite3VdbeDb() {
				return this.db;
			}
			public sqlite3_value sqlite3VdbeGetValue(int iVar,u8 aff) {
				Debug.Assert(iVar>0);
				if(this!=null) {
					Mem pMem=this.aVar[iVar-1];
					if(0==(pMem.flags&MEM_Null)) {
						sqlite3_value pRet=sqlite3ValueNew(this.db);
						if(pRet!=null) {
							sqlite3VdbeMemCopy((Mem)pRet,pMem);
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
			public int sqlite3VdbeExec(/* The VDBE */) {
				int opcodeIndex=0;
				/* The program counter */Op[] aOp=this.aOp;
				var lOp=this.lOp;
				Log.WriteHeader("Plan VdbeExec");
				Log.Indent();
				foreach(var item in lOp) {
					Log.WriteLine(item.ToString(this));
				}
				Log.WriteHeader("---");
				try {
					/* Copy of p.aOp */Op pOp;
					/* Current operation */int rc=SQLITE_OK;
					/* Value to return */sqlite3 db=this.db;
					/* The database */u8 resetSchemaOnFault=0;
					/* Reset schema after an error if positive */SqliteEncoding encoding=ENC(db);
					/* The database encoding */
					#if !SQLITE_OMIT_PROGRESS_CALLBACK
					bool checkProgress;
					/* True if progress callbacks are enabled */int nProgressOps=0;
					/* Opcodes executed since progress callback. */
					#endif
					Mem[] aMem=this.aMem;
					/* Copy of p.aMem */Mem pIn1=null;
					/* 1st input operand */Mem pIn2=null;
					/* 2nd input operand */Mem pIn3=null;
					/* 3rd input operand */Mem pOut=null;
					/* Output operand */int iCompare=0;
					/* Result of last OP_Compare operation */int[] aPermute=null;
					/* Permutation of columns for OP_Compare */i64 lastRowid=db.lastRowid;
					/* Saved value of the last insert ROWID */
					#if VDBE_PROFILE
																																																																								u64 start;                   /* CPU clock count at start of opcode */
int origPc;                  /* Program counter at start of opcode */
#endif
					/*** INSERT STACK UNION HERE ***/Debug.Assert(this.magic==VDBE_MAGIC_RUN);
					/* sqlite3_step() verifies this */this.sqlite3VdbeEnter();
					if(this.rc==SQLITE_NOMEM) {
						/* This happens if a malloc() inside a call to sqlite3_column_text() or
            ** sqlite3_column_text16() failed.  */goto no_mem;
					}
					Debug.Assert(this.rc==SQLITE_OK||this.rc==SQLITE_BUSY);
					this.rc=SQLITE_OK;
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
					for(opcodeIndex=this.currentOpCodeIndex;rc==SQLITE_OK;opcodeIndex++) {
						Debug.Assert(opcodeIndex>=0&&opcodeIndex<this.nOp);
						//      if ( db.mallocFailed != 0 ) goto no_mem;
						#if VDBE_PROFILE
																																																																																															origPc = pc;
start = sqlite3Hwtime();
#endif
						pOp=lOp[opcodeIndex];
						/* Only allow tracing if SQLITE_DEBUG is defined.
            */
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
						/* Check to see if we need to simulate an interrupt.  This only happens
** if we have a special test build.
*/
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
						/* Call the progress callback if it is configured and the required number
** of VDBE ops have been executed (either since this invocation of
** sqlite3VdbeExec() or since last time the progress callback was called).
** If the progress callback returns non-zero, exit the virtual machine with
** a return code SQLITE_ABORT.
*/if(checkProgress) {
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
						/* On any opcode with the "out2-prerelase" tag, free any
** external allocations out of mem[p2] and set mem[p2] to be
** an undefined integer.  Opcodes will either fill in the integer
** value or convert mem[p2] to a different type.
*/Debug.Assert(pOp.opflags==sqlite3OpcodeProperty[pOp.opcode]);
						if((pOp.opflags&OPFLG_OUT2_PRERELEASE)!=0) {
							Debug.Assert(pOp.p2>0);
							Debug.Assert(pOp.p2<=this.nMem);
							pOut=aMem[pOp.p2];
							memAboutToChange(this,pOut);
							sqlite3VdbeMemReleaseExternal(pOut);
							pOut.Flags=MemFlags.MEM_Int;
						}
						/* Sanity checking on other operands *//* Sanity checking on other operands */
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
						/*****************************************************************************
                  ** What follows is a massive switch statement where each case implements a
                  ** separate instruction in the virtual machine.  If we follow the usual
                  ** indentation conventions, each case should be indented by 6 spaces.  But
                  ** that is a lot of wasted space on the left margin.  So the code within
                  ** the switch statement will break with convention and be flush-left. Another
                  ** big comment (similar to this one) will mark the point in the code where
                  ** we transition back to normal indentation.
                  **
                  ** The formatting of each case is important.  The makefile for SQLite
                  ** generates two C files "opcodes.h" and "opcodes.c" by scanning this
                  ** file looking for lines that begin with "case OP_".  The opcodes.h files
                  ** will be filled with #defines that give unique integer values to each
                  ** opcode and the opcodes.c file is filled with an array of strings where
                  ** each string is the symbolic name for the corresponding opcode.  If the
                  ** case statement is followed by a comment of the form "/# same as ... #/"
                  ** that comment is used to determine the particular value of the opcode.
                  **
                  ** Other keywords in the comment that follows each case are used to
                  ** construct the OPFLG_INITIALIZER value that initializes opcodeProperty[].
                  ** Keywords include: in1, in2, in3, ref2_prerelease, ref2, ref3.  See
                  ** the mkopcodeh.awk script for additional information.
                  **
                  ** Documentation about VDBE opcodes is generated by scanning this file
                  ** for lines of that contain "Opcode:".  That line and all subsequent
                  ** comment lines are used in the generation of the opcode.html documentation
                  ** file.
                  **
                  ** SUMMARY:
                  **
                  **     Formatting is important to scripts that scan this file.
                  **     Do not deviate from the formatting style currently in use.
                  **
                  *****************************************************************************//* Opcode:  Goto * P2 * * *
**
** An unconditional jump to address P2.
** The next instruction executed will be
** the one at index P2 from the beginning of
** the program.
*/case OpCode.OP_Goto: {
							/* jump */if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							opcodeIndex=pOp.p2-1;
							break;
						}
						/* Opcode:  Gosub P1 P2 * * *
                  **
                  ** Write the current address onto register P1
                  ** and then jump to address P2.
                  */case OpCode.OP_Gosub: {
							/* jump, in1 */pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Dyn)==0);
							memAboutToChange(this,pIn1);
							pIn1.flags=MEM_Int;
							pIn1.u.i=opcodeIndex;
							REGISTER_TRACE(this,pOp.p1,pIn1);
							opcodeIndex=pOp.p2-1;
							break;
						}
						/* Opcode:  Return P1 * * * *
                  **
                  ** Jump to the next instruction after the address in register P1.
                  */case OpCode.OP_Return: {
							/* in1 */pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Int)!=0);
							opcodeIndex=(int)pIn1.u.i;
							break;
						}
						/* Opcode:  Yield P1 * * * *
                  **
                  ** Swap the program counter with the value in register P1.
                  */case OpCode.OP_Yield: {
							/* in1 */int pcDest;
							pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Dyn)==0);
							pIn1.flags=MEM_Int;
							pcDest=(int)pIn1.u.i;
							pIn1.u.i=opcodeIndex;
							REGISTER_TRACE(this,pOp.p1,pIn1);
							opcodeIndex=pcDest;
							break;
						}
						/* Opcode:  HaltIfNull  P1 P2 P3 P4 *
                  **
                  ** Check the value in register P3.  If it is NULL then Halt using
                  ** parameter P1, P2, and P4 as if this were a Halt instruction.  If the
                  ** value in register P3 is not NULL, then this routine is a no-op.
                  */case OpCode.OP_HaltIfNull: {
							/* in3 */pIn3=aMem[pOp.p3];
							if((pIn3.flags&MEM_Null)==0)
								break;
							/* Fall through into OP_Halt */goto case OpCode.OP_Halt;
						}
						/* Opcode:  Halt P1 P2 * P4 *
                  **
                  ** Exit immediately.  All open cursors, etc are closed
                  ** automatically.
                  **
                  ** P1 is the result code returned by sqlite3_exec(), sqlite3_reset(),
                  ** or sqlite3_finalize().  For a normal halt, this should be SQLITE_OK (0).
                  ** For errors, it can be some other value.  If P1!=0 then P2 will determine
                  ** whether or not to rollback the current transaction.  Do not rollback
                  ** if P2==OE_Fail. Do the rollback if P2==OE_Rollback.  If P2==OE_Abort,
                  ** then back out all changes that have occurred during this execution of the
                  ** VDBE, but do not rollback the transaction.
                  **
                  ** If P4 is not null then it is an error message string.
                  **
                  ** There is an implied "Halt 0 0 0" instruction inserted at the very end of
                  ** every program.  So a jump past the last instruction of the program
                  ** is the same as executing Halt.
                  */case OpCode.OP_Halt: {
							pIn3=aMem[pOp.p3];
							if(pOp.p1==SQLITE_OK&&this.pFrame!=null) {
								/* Halt the sub-program. Return control to the parent frame. */VdbeFrame pFrame=this.pFrame;
								this.pFrame=pFrame.pParent;
								this.nFrame--;
								sqlite3VdbeSetChanges(db,this.nChange);
								opcodeIndex=pFrame.sqlite3VdbeFrameRestore();
								lastRowid=db.lastRowid;
								if(pOp.p2==OE_Ignore) {
									/* Instruction pc is the OP_Program that invoked the sub-program 
                              ** currently being halted. If the p2 instruction of this OP_Halt
                              ** instruction is set to OE_Ignore, then the sub-program is throwing
                              ** an IGNORE exception. In this case jump to the address specified
                              ** as the p2 of the calling OP_Program.  */opcodeIndex=this.lOp[opcodeIndex].p2-1;
								}
								lOp=this.lOp;
								aMem=this.aMem;
								break;
							}
							this.rc=pOp.p1;
							this.errorAction=(u8)pOp.p2;
							this.currentOpCodeIndex=opcodeIndex;
							if(pOp.p4.z!=null) {
								Debug.Assert(this.rc!=SQLITE_OK);
								sqlite3SetString(ref this.zErrMsg,db,"%s",pOp.p4.z);
								testcase(sqlite3GlobalConfig.xLog!=null);
								sqlite3_log(pOp.p1,"abort at %d in [%s]: %s",opcodeIndex,this.zSql,pOp.p4.z);
							}
							else
								if(this.rc!=0) {
									testcase(sqlite3GlobalConfig.xLog!=null);
									sqlite3_log(pOp.p1,"constraint failed at %d in [%s]",opcodeIndex,this.zSql);
								}
							rc=this.sqlite3VdbeHalt();
							Debug.Assert(rc==SQLITE_BUSY||rc==SQLITE_OK||rc==SQLITE_ERROR);
							if(rc==SQLITE_BUSY) {
								this.rc=rc=SQLITE_BUSY;
							}
							else {
								Debug.Assert(rc==SQLITE_OK||this.rc==SQLITE_CONSTRAINT);
								Debug.Assert(rc==SQLITE_OK||db.nDeferredCons>0);
								rc=this.rc!=0?SQLITE_ERROR:SQLITE_DONE;
							}
							goto vdbe_return;
						}
						/* Opcode: Integer P1 P2 * * *
                  **
                  ** The 32-bit integer value P1 is written into register P2.
                  */case OpCode.OP_Integer: {
							/* out2-prerelease */pOut.u.i=pOp.p1;
							break;
						}
						/* Opcode: Int64 * P2 * P4 *
                  **
                  ** P4 is a pointer to a 64-bit integer value.
                  ** Write that value into register P2.
                  */case OpCode.OP_Int64: {
							/* out2-prerelease */// Integer pointer always exists Debug.Assert( pOp.p4.pI64 != 0 );
							pOut.u.i=pOp.p4.pI64;
							break;
						}
						#if !SQLITE_OMIT_FLOATING_POINT
						/* Opcode: Real * P2 * P4 *
**
** P4 is a pointer to a 64-bit floating point value.
** Write that value into register P2.
*/case OpCode.OP_Real: {
							/* same as TK_FLOAT, ref2-prerelease */pOut.flags=MEM_Real;
							Debug.Assert(!MathExtensions.sqlite3IsNaN(pOp.p4.pReal));
							pOut.r=pOp.p4.pReal;
							break;
						}
						#endif
						/* Opcode: String8 * P2 * P4 *
**
** P4 points to a nul terminated UTF-8 string. This opcode is transformed
** into an OP_String before it is executed for the first time.
*/case OpCode.OP_String8: {
							/* same as TK_STRING, ref2-prerelease */Debug.Assert(pOp.p4.z!=null);
							pOp.OpCode=OpCode.OP_String;
							pOp.p1=StringExtensions.sqlite3Strlen30(pOp.p4.z);
							#if !SQLITE_OMIT_UTF16
																																																																																																																						if( encoding!=SqliteEncoding.UTF8 ){
rc = sqlite3VdbeMemSetStr(pOut, pOp.p4.z, -1, SqliteEncoding.UTF8, SQLITE_STATIC);
if( rc==SQLITE_TOOBIG ) goto too_big;
if( SQLITE_OK!=sqlite3VdbeChangeEncoding(pOut, encoding) ) goto no_mem;
Debug.Assert( pOut.zMalloc==pOut.z );
Debug.Assert( pOut.flags & MEM_Dyn );
pOut.zMalloc = 0;
pOut.flags |= MEM_Static;
pOut.flags &= ~MEM_Dyn;
if( pOp.p4type==P4_DYNAMIC ){
sqlite3DbFree(db, ref pOp.p4.z);
}
pOp.p4type = P4_DYNAMIC;
pOp.p4.z = pOut.z;
pOp.p1 = pOut.n;
}
#endif
							if(pOp.p1>db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							/* Fall through to the next case, OP_String */goto case OpCode.OP_String;
						}
						/* Opcode: String P1 P2 * P4 *
                  **
                  ** The string value P4 of length P1 (bytes) is stored in register P2.
                  */case OpCode.OP_String: {
							/* out2-prerelease */Debug.Assert(pOp.p4.z!=null);
							pOut.flags=MEM_Str|MEM_Static|MEM_Term;
							sqlite3_free(ref pOut.zBLOB);
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
						/* Opcode: Null * P2 * * *
                  **
                  ** Write a NULL into register P2.
                  */case OpCode.OP_Null: {
							/* out2-prerelease */pOut.flags=MEM_Null;
							break;
						}
						/* Opcode: Blob P1 P2 * P4
                  **
                  ** P4 points to a blob of data P1 bytes long.  Store this
                  **  blob in register P2.
                  */case OpCode.OP_Blob: {
							/* out2-prerelease */Debug.Assert(pOp.p1<=db.aLimit[SQLITE_LIMIT_LENGTH]);
							sqlite3VdbeMemSetStr(pOut,pOp.p4.z,pOp.p1,0,null);
							pOut.enc=encoding;
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						/* Opcode: Variable P1 P2 * P4 *
                  **
                  ** Transfer the values of bound parameter P1 into register P2
                  **
                  ** If the parameter is named, then its name appears in P4 and P3==1.
                  ** The P4 value is used by sqlite3_bind_parameter_name().
                  */case OpCode.OP_Variable: {
							/* out2-prerelease */Mem pVar;
							/* Value being transferred */Debug.Assert(pOp.p1>=0&&pOp.p1<=this.nVar);
							Debug.Assert(pOp.p4.z==null||pOp.p4.z==this.azVar[pOp.p1-1]);
							pVar=this.aVar[pOp.p1-1];
							if(sqlite3VdbeMemTooBig(pVar)) {
								goto too_big;
							}
							sqlite3VdbeMemShallowCopy(pOut,pVar,MEM_Static);
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						/* Opcode: Move P1 P2 P3 * *
                  **
                  ** Move the values in register P1..P1+P3-1 over into
                  ** registers P2..P2+P3-1.  Registers P1..P1+P1-1 are
                  ** left holding a NULL.  It is an error for register ranges
                  ** P1..P1+P3-1 and P2..P2+P3-1 to overlap.
                  */case OpCode.OP_Move: {
							//char* zMalloc;   /* Holding variable for allocated memory */
							int n;
							/* Number of registers left to copy */int p1;
							/* Register to copy from */int p2;
							/* Register to copy to */n=pOp.p3;
							p1=pOp.p1;
							p2=pOp.p2;
							Debug.Assert(n>0&&p1>0&&p2>0);
							Debug.Assert(p1+n<=p2||p2+n<=p1);
							//pIn1 = aMem[p1];
							//pOut = aMem[p2];
							while(n--!=0) {
								pIn1=aMem[p1+pOp.p3-n-1];
								pOut=aMem[p2];
								//Debug.Assert( pOut<=&aMem[p.nMem] );
								//Debug.Assert( pIn1<=&aMem[p.nMem] );
								Debug.Assert(pIn1.memIsValid());
								memAboutToChange(this,pOut);
								//zMalloc = pOut.zMalloc;
								//pOut.zMalloc = null;
								sqlite3VdbeMemMove(pOut,pIn1);
								//pIn1.zMalloc = zMalloc;
								REGISTER_TRACE(this,p2++,pOut);
								//pIn1++;
								//pOut++;
							}
							break;
						}
						/* Opcode: Copy P1 P2 * * *
                  **
                  ** Make a copy of register P1 into register P2.
                  **
                  ** This instruction makes a deep copy of the value.  A duplicate
                  ** is made of any string or blob constant.  See also OP_SCopy.
                  */case OpCode.OP_Copy: {
							/* in1, ref2 */pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							Debug.Assert(pOut!=pIn1);
							sqlite3VdbeMemShallowCopy(pOut,pIn1,MEM_Ephem);
							if((pOut.flags&MEM_Ephem)!=0&&sqlite3VdbeMemMakeWriteable(pOut)!=0) {
								goto no_mem;
							}
							//Deephemeralize( pOut );
							REGISTER_TRACE(this,pOp.p2,pOut);
							break;
						}
						/* Opcode: SCopy P1 P2 * * *
                  **
                  ** Make a shallow copy of register P1 into register P2.
                  **
                  ** This instruction makes a shallow copy of the value.  If the value
                  ** is a string or blob, then the copy is only a pointer to the
                  ** original and hence if the original changes so will the copy.
                  ** Worse, if the original is deallocated, the copy becomes invalid.
                  ** Thus the program must guarantee that the original will not change
                  ** during the lifetime of the copy.  Use OP_Copy to make a complete
                  ** copy.
                  */case OpCode.OP_SCopy: {
							/* in1, ref2 */pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							Debug.Assert(pOut!=pIn1);
							sqlite3VdbeMemShallowCopy(pOut,pIn1,MEM_Ephem);
							#if SQLITE_DEBUG
																																																																																																																						              if ( pOut.pScopyFrom == null )
                pOut.pScopyFrom = pIn1;
#endif
							REGISTER_TRACE(this,pOp.p2,pOut);
							break;
						}
						/* Opcode: ResultRow P1 P2 * * *
                  **
                  ** The registers P1 through P1+P2-1 contain a single row of
                  ** results. This opcode causes the sqlite3_step() call to terminate
                  ** with an SQLITE_ROW return code and it sets up the sqlite3_stmt
                  ** structure to provide access to the top P1 values as the result
                  ** row.
                  */case OpCode.OP_ResultRow: {
							//Mem[] pMem;
							int i;
							Debug.Assert(this.nResColumn==pOp.p2);
							Debug.Assert(pOp.p1>0);
							Debug.Assert(pOp.p1+pOp.p2<=this.nMem+1);
							/* If this statement has violated immediate foreign key constraints, do
                          ** not return the number of rows modified. And do not RELEASE the statement
                          ** transaction. It needs to be rolled back.  */if(SQLITE_OK!=(rc=this.sqlite3VdbeCheckFk(0))) {
								Debug.Assert((db.flags&SQLITE_CountRows)!=0);
								Debug.Assert(this.usesStmtJournal);
								break;
							}
							/* If the SQLITE_CountRows flag is set in sqlite3.flags mask, then
                          ** DML statements invoke this opcode to return the number of rows
                          ** modified to the user. This is the only way that a VM that
                          ** opens a statement transaction may invoke this opcode.
                          **
                          ** In case this is such a statement, close any statement transaction
                          ** opened by this VM before returning control to the user. This is to
                          ** ensure that statement-transactions are always nested, not overlapping.
                          ** If the open statement-transaction is not closed here, then the user
                          ** may step another VM that opens its own statement transaction. This
                          ** may lead to overlapping statement transactions.
                          **
                          ** The statement transaction is never a top-level transaction.  Hence
                          ** the RELEASE call below can never fail.
                          */Debug.Assert(this.iStatement==0||(db.flags&SQLITE_CountRows)!=0);
							rc=this.sqlite3VdbeCloseStatement(SAVEPOINT_RELEASE);
							if(NEVER(rc!=SQLITE_OK)) {
								break;
							}
							/* Invalidate all ephemeral cursor row caches */this.cacheCtr=(this.cacheCtr+2)|1;
							/* Make sure the results of the current row are \000 terminated
                          ** and have an assigned type.  The results are de-ephemeralized as
                          ** as side effect.
                          *///pMem = p.pResultSet = aMem[pOp.p1];
							this.pResultSet=new Mem[pOp.p2];
							for(i=0;i<pOp.p2;i++) {
								this.pResultSet[i]=aMem[pOp.p1+i];
								Debug.Assert(this.pResultSet[i].memIsValid());
								//Deephemeralize( p.pResultSet[i] );
								//Debug.Assert( ( p.pResultSet[i].flags & MEM_Ephem ) == 0
								//        || ( p.pResultSet[i].flags & ( MEM_Str | MEM_Blob ) ) == 0 );
								sqlite3VdbeMemNulTerminate(this.pResultSet[i]);
								//sqlite3VdbeMemNulTerminate(pMem[i]);
								sqlite3VdbeMemStoreType(this.pResultSet[i]);
								REGISTER_TRACE(this,pOp.p1+i,this.pResultSet[i]);
							}
							//      if ( db.mallocFailed != 0 ) goto no_mem;
							/* Return SQLITE_ROW
                          */this.currentOpCodeIndex=opcodeIndex+1;
							rc=SQLITE_ROW;
							goto vdbe_return;
						}
						/* Opcode: Concat P1 P2 P3 * *
                  **
                  ** Add the text in register P1 onto the end of the text in
                  ** register P2 and store the result in register P3.
                  ** If either the P1 or P2 text are NULL then store NULL in P3.
                  **
                  **   P3 = P2 || P1
                  **
                  ** It is illegal for P1 and P3 to be the same register. Sometimes,
                  ** if P3 is the same register as P2, the implementation is able
                  ** to avoid a memcpy().
                  */case OpCode.OP_Concat: {
							/* same as TK_CONCAT, in1, in2, ref3 */i64 nByte;
							pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							pOut=aMem[pOp.p3];
							Debug.Assert(pIn1!=pOut);
							if(((pIn1.flags|pIn2.flags)&MEM_Null)!=0) {
								sqlite3VdbeMemSetNull(pOut);
								break;
							}
							if(ExpandBlob(pIn1)!=0||ExpandBlob(pIn2)!=0)
								goto no_mem;
							if(((pIn1.flags&(MEM_Str|MEM_Blob))==0)&&sqlite3VdbeMemStringify(pIn1,encoding)!=0) {
								goto no_mem;
							}
							// Stringify(pIn1, encoding);
							if(((pIn2.flags&(MEM_Str|MEM_Blob))==0)&&sqlite3VdbeMemStringify(pIn2,encoding)!=0) {
								goto no_mem;
							}
							// Stringify(pIn2, encoding);
							nByte=pIn1.n+pIn2.n;
							if(nByte>db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							pOut.MemSetTypeFlag(MEM_Str);
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
									if((pIn1.flags&MEM_Blob)==0)//String as Blob
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
								pOut.zBLOB=sqlite3Malloc(pIn1.n+pIn2.n);
								Buffer.BlockCopy(pIn2.zBLOB,0,pOut.zBLOB,0,pIn2.n);
								if(pIn1.zBLOB!=null)
									Buffer.BlockCopy(pIn1.zBLOB,0,pOut.zBLOB,pIn2.n,pIn1.n);
								else
									for(int i=0;i<pIn1.n;i++)
										pOut.zBLOB[pIn2.n+i]=(byte)pIn1.z[i];
							}
							//pOut.z[nByte] = 0;
							//pOut.z[nByte + 1] = 0;
							pOut.flags|=MEM_Term;
							pOut.n=(int)nByte;
							pOut.enc=encoding;
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						/* Opcode: Add P1 P2 P3 * *
                  **
                  ** Add the value in register P1 to the value in register P2
                  ** and store the result in register P3.
                  ** If either input is NULL, the result is NULL.
                  *//* Opcode: Multiply P1 P2 P3 * *
                      **
                      **
                      ** Multiply the value in register P1 by the value in register P2
                      ** and store the result in register P3.
                      ** If either input is NULL, the result is NULL.
                      *//* Opcode: Subtract P1 P2 P3 * *
                      **
                      ** Subtract the value in register P1 from the value in register P2
                      ** and store the result in register P3.
                      ** If either input is NULL, the result is NULL.
                      *//* Opcode: Divide P1 P2 P3 * *
                      **
                      ** Divide the value in register P1 by the value in register P2
                      ** and store the result in register P3 (P3=P2/P1). If the value in 
                      ** register P1 is zero, then the result is NULL. If either input is 
                      ** NULL, the result is NULL.
                      *//* Opcode: Remainder P1 P2 P3 * *
                      **
                      ** Compute the remainder after integer division of the value in
                      ** register P1 by the value in register P2 and store the result in P3.
                      ** If the value in register P2 is zero the result is NULL.
                      ** If either operand is NULL, the result is NULL.
                      */case OpCode.OP_Add:
						/* same as TK_PLUS, in1, in2, ref3 */case OpCode.OP_Subtract:
						/* same as TK_MINUS, in1, in2, ref3 */case OpCode.OP_Multiply:
						/* same as TK_STAR, in1, in2, ref3 */case OpCode.OP_Divide:
						/* same as TK_SLASH, in1, in2, ref3 */case OpCode.OP_Remainder: {
							/* same as TK_REM, in1, in2, ref3 */int flags;
							/* Combined MEM_* flags from both inputs */i64 iA;
							/* Integer value of left operand */i64 iB=0;
							/* Integer value of right operand */double rA;
							/* Real value of left operand */double rB;
							/* Real value of right operand */pIn1=aMem[pOp.p1];
							applyNumericAffinity(pIn1);
							pIn2=aMem[pOp.p2];
							applyNumericAffinity(pIn2);
							pOut=aMem[pOp.p3];
							flags=pIn1.flags|pIn2.flags;
							if((flags&MEM_Null)!=0)
								goto arithmetic_result_is_null;
							bool fp_math;
							if(!(fp_math=!((pIn1.Flags&pIn2.Flags&MemFlags.MEM_Int)==MemFlags.MEM_Int))) {
								iA=pIn1.u.i;
								iB=pIn2.u.i;
								switch(pOp.OpCode) {
								case OpCode.OP_Add: {
									if(sqlite3AddInt64(ref iB,iA)!=0)
										fp_math=true;
									// goto fp_math
									break;
								}
								case OpCode.OP_Subtract: {
									if(sqlite3SubInt64(ref iB,iA)!=0)
										fp_math=true;
									// goto fp_math
									break;
								}
								case OpCode.OP_Multiply: {
									if(sqlite3MulInt64(ref iB,iA)!=0)
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
								pOut.MemSetTypeFlag(MEM_Int);
							}
							else {
								//fp_math:
								rA=sqlite3VdbeRealValue(pIn1);
								rB=sqlite3VdbeRealValue(pIn2);
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
									/* (double)0 In case of SQLITE_OMIT_FLOATING_POINT... */if(rA==(double)0)
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
MemSetTypeFlag(pOut, MEM_Int);
#else
								if(MathExtensions.sqlite3IsNaN(rB)) {
									goto arithmetic_result_is_null;
								}
								pOut.r=rB;
								pOut.MemSetTypeFlag(MEM_Real);
								if((flags&MEM_Real)==0) {
									sqlite3VdbeIntegerAffinity(pOut);
								}
								#endif
							}
							break;
							arithmetic_result_is_null:
							sqlite3VdbeMemSetNull(pOut);
							break;
						}
						/* Opcode: CollSeq * * P4
                  **
                  ** P4 is a pointer to a CollSeq struct. If the next call to a user function
                  ** or aggregate calls sqlite3GetFuncCollSeq(), this collation sequence will
                  ** be returned. This is used by the built-in min(), max() and nullif()
                  ** functions.
                  **
                  ** The interface used by the implementation of the aforementioned functions
                  ** to retrieve the collation sequence set by this opcode is not available
                  ** publicly, only to user functions defined in func.c.
                  */case OpCode.OP_CollSeq: {
							Debug.Assert(pOp.p4type==P4_COLLSEQ);
							break;
						}
						/* Opcode: Function P1 P2 P3 P4 P5
                  **
                  ** Invoke a user function (P4 is a pointer to a Function structure that
                  ** defines the function) with P5 arguments taken from register P2 and
                  ** successors.  The result of the function is stored in register P3.
                  ** Register P3 must not be one of the function inputs.
                  **
                  ** P1 is a 32-bit bitmask indicating whether or not each argument to the
                  ** function was determined to be constant at compile time. If the first
                  ** argument was constant then bit 0 of P1 is set. This is used to determine
                  ** whether meta data associated with a user function argument using the
                  ** sqlite3_set_auxdata() API may be safely retained until the next
                  ** invocation of this opcode.
                  **
                  ** See also: AggStep and AggFinal
                  */case OpCode.OP_Function: {
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
							Debug.Assert(pOp.p4type==P4_FUNCDEF||pOp.p4type==P4_VDBEFUNC);
							if(pOp.p4type==P4_FUNCDEF) {
								ctx.pFunc=pOp.p4.pFunc;
								ctx.pVdbeFunc=null;
							}
							else {
								ctx.pVdbeFunc=(VdbeFunc)pOp.p4.pVdbeFunc;
								ctx.pFunc=ctx.pVdbeFunc.pFunc;
							}
							ctx.s.flags=MEM_Null;
							ctx.s.db=db;
							ctx.s.xDel=null;
							//ctx.s.zMalloc = null;
							/* The output cell may already have a buffer allocated. Move
                          ** the pointer to ctx.s so in case the user-function can use
                          ** the already allocated buffer instead of allocating a new one.
                          */sqlite3VdbeMemMove(ctx.s,pOut);
							ctx.s.MemSetTypeFlag(MEM_Null);
							ctx.isError=0;
							if((ctx.pFunc.flags&SQLITE_FUNC_NEEDCOLL)!=0) {
								Debug.Assert(opcodeIndex>1);
								//Debug.Assert(pOp > aOp);
								Debug.Assert(this.lOp[opcodeIndex-1].p4type==P4_COLLSEQ);
								//Debug.Assert(pOp[-1].p4type == P4_COLLSEQ);
								Debug.Assert(this.lOp[opcodeIndex-1].opcode==OP_CollSeq);
								//Debug.Assert(pOp[-1].opcode == OP_CollSeq);
								ctx.pColl=this.lOp[opcodeIndex-1].p4.pColl;
								//ctx.pColl = pOp[-1].p4.pColl;
							}
							db.lastRowid=lastRowid;
							ctx.pFunc.xFunc(ctx,n,apVal);
							///* IMP: R-24505-23230 */
							lastRowid=db.lastRowid;
							/* If any auxillary data functions have been called by this user function,
                          ** immediately call the destructor for any non-static values.
                          */if(ctx.pVdbeFunc!=null) {
								sqlite3VdbeDeleteAuxData(ctx.pVdbeFunc,pOp.p1);
								pOp.p4.pVdbeFunc=ctx.pVdbeFunc;
								pOp.p4type=P4_VDBEFUNC;
							}
							//if ( db->mallocFailed )
							//{
							//  /* Even though a malloc() has failed, the implementation of the
							//  ** user function may have called an sqlite3_result_XXX() function
							//  ** to return a value. The following call releases any resources
							//  ** associated with such a value.
							//  */
							//  sqlite3VdbeMemRelease( &u.ag.ctx.s );
							//  goto no_mem;
							//}
							/* If the function returned an error, throw an exception */if(ctx.isError!=0) {
								sqlite3SetString(ref this.zErrMsg,db,sqlite3_value_text(ctx.s));
								rc=ctx.isError;
							}
							/* Copy the result of the function into register P3 */sqlite3VdbeChangeEncoding(ctx.s,encoding);
							sqlite3VdbeMemMove(pOut,ctx.s);
							if(sqlite3VdbeMemTooBig(pOut)) {
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
						/* Opcode: BitAnd P1 P2 P3 * *
                  **
                  ** Take the bit-wise AND of the values in register P1 and P2 and
                  ** store the result in register P3.
                  ** If either input is NULL, the result is NULL.
                  *//* Opcode: BitOr P1 P2 P3 * *
                      **
                      ** Take the bit-wise OR of the values in register P1 and P2 and
                      ** store the result in register P3.
                      ** If either input is NULL, the result is NULL.
                      *//* Opcode: ShiftLeft P1 P2 P3 * *
                      **
                      ** Shift the integer value in register P2 to the left by the
                      ** number of bits specified by the integer in register P1.
                      ** Store the result in register P3.
                      ** If either input is NULL, the result is NULL.
                      *//* Opcode: ShiftRight P1 P2 P3 * *
                      **
                      ** Shift the integer value in register P2 to the right by the
                      ** number of bits specified by the integer in register P1.
                      ** Store the result in register P3.
                      ** If either input is NULL, the result is NULL.
                      */case OpCode.OP_BitAnd:
						/* same as TK_BITAND, in1, in2, ref3 */case OpCode.OP_BitOr:
						/* same as TK_BITOR, in1, in2, ref3 */case OpCode.OP_ShiftLeft:
						/* same as TK_LSHIFT, in1, in2, ref3 */case OpCode.OP_ShiftRight: {
							/* same as TK_RSHIFT, in1, in2, ref3 */i64 iA;
							u64 uA;
							i64 iB;
							u8 op;
							pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							pOut=aMem[pOp.p3];
							if(((pIn1.flags|pIn2.flags)&MEM_Null)!=0) {
								sqlite3VdbeMemSetNull(pOut);
								break;
							}
							iA=sqlite3VdbeIntValue(pIn2);
							iB=sqlite3VdbeIntValue(pIn1);
							op=pOp.opcode;
							if(op==OP_BitAnd) {
								iA&=iB;
							}
							else
								if(op==OP_BitOr) {
									iA|=iB;
								}
								else
									if(iB!=0) {
										Debug.Assert(op==OP_ShiftRight||op==OP_ShiftLeft);
										/* If shifting by a negative amount, shift in the other direction */if(iB<0) {
											Debug.Assert(OP_ShiftRight==OP_ShiftLeft+1);
											op=(u8)(2*OP_ShiftLeft+1-op);
											iB=iB>(-64)?-iB:64;
										}
										if(iB>=64) {
											iA=(iA>=0||op==OP_ShiftLeft)?0:-1;
										}
										else {
											//uA = (ulong)(iA << 0); // memcpy( &uA, &iA, sizeof( uA ) );
											if(op==OP_ShiftLeft) {
												iA=iA<<(int)iB;
											}
											else {
												iA=iA>>(int)iB;
												/* Sign-extend on a right shift of a negative number *///if ( iA < 0 )
												//  uA |= ( ( (0xffffffff ) << (u8)32 ) | 0xffffffff ) << (u8)( 64 - iB );
											}
											//iA = (long)( uA << 0 ); //memcpy( &iA, &uA, sizeof( iA ) );
										}
									}
							pOut.u.i=iA;
							pOut.MemSetTypeFlag(MEM_Int);
							break;
						}
						/* Opcode: AddImm  P1 P2 * * *
                  **
                  ** Add the constant P2 to the value in register P1.
                  ** The result is always an integer.
                  **
                  ** To force any register to be an integer, just add 0.
                  */case OpCode.OP_AddImm: {
							/* in1 */pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							sqlite3VdbeMemIntegerify(pIn1);
							pIn1.u.i+=pOp.p2;
							break;
						}
						/* Opcode: MustBeInt P1 P2 * * *
                  **
                  ** Force the value in register P1 to be an integer.  If the value
                  ** in P1 is not an integer and cannot be converted into an integer
                  ** without data loss, then jump immediately to P2, or if P2==0
                  ** raise an SQLITE_MISMATCH exception.
                  */case OpCode.OP_MustBeInt: {
							/* jump, in1 */pIn1=aMem[pOp.p1];
							applyAffinity(pIn1,SQLITE_AFF_NUMERIC,encoding);
							if((pIn1.flags&MEM_Int)==0) {
								if(pOp.p2==0) {
									rc=SQLITE_MISMATCH;
									goto abort_due_to_error;
								}
								else {
									opcodeIndex=pOp.p2-1;
								}
							}
							else {
								pIn1.MemSetTypeFlag(MEM_Int);
							}
							break;
						}
						#if !SQLITE_OMIT_FLOATING_POINT
						/* Opcode: RealAffinity P1 * * * *
**
** If register P1 holds an integer convert it to a real value.
**
** This opcode is used when extracting information from a column that
** has REAL affinity.  Such column values may still be stored as
** integers, for space efficiency, but after extraction we want them
** to have only a real value.
*/case OpCode.OP_RealAffinity: {
							/* in1 */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Int)!=0) {
								sqlite3VdbeMemRealify(pIn1);
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_CAST
						/* Opcode: ToText P1 * * * *
**
** Force the value in register P1 to be text.
** If the value is numeric, convert it to a string using the
** equivalent of printf().  Blob values are unchanged and
** are afterwards simply interpreted as text.
**
** A NULL value is not changed by this routine.  It remains NULL.
*/case OpCode.OP_ToText: {
							/* same as TK_TO_TEXT, in1 */pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							if((pIn1.flags&MEM_Null)!=0)
								break;
							Debug.Assert(MEM_Str==(MEM_Blob>>3));
							pIn1.flags|=(u16)((pIn1.flags&MEM_Blob)>>3);
							applyAffinity(pIn1,SQLITE_AFF_TEXT,encoding);
							rc=ExpandBlob(pIn1);
							Debug.Assert((pIn1.flags&MEM_Str)!=0/*|| db.mallocFailed != 0 */);
							pIn1.flags=(u16)(pIn1.flags&~(MEM_Int|MEM_Real|MEM_Blob|MEM_Zero));
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
							break;
						}
						/* Opcode: ToBlob P1 * * * *
                  **
                  ** Force the value in register P1 to be a BLOB.
                  ** If the value is numeric, convert it to a string first.
                  ** Strings are simply reinterpreted as blobs with no change
                  ** to the underlying data.
                  **
                  ** A NULL value is not changed by this routine.  It remains NULL.
                  */case OpCode.OP_ToBlob: {
							/* same as TK_TO_BLOB, in1 */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)!=0)
								break;
							if((pIn1.flags&MEM_Blob)==0) {
								applyAffinity(pIn1,SQLITE_AFF_TEXT,encoding);
								Debug.Assert((pIn1.flags&MEM_Str)!=0/*|| db.mallocFailed != 0 */);
								pIn1.MemSetTypeFlag(MEM_Blob);
							}
							else {
								pIn1.flags=(ushort)(pIn1.flags&~(MEM_TypeMask&~MEM_Blob));
							}
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
							break;
						}
						/* Opcode: ToNumeric P1 * * * *
                  **
                  ** Force the value in register P1 to be numeric (either an
                  ** integer or a floating-point number.)
                  ** If the value is text or blob, try to convert it to an using the
                  ** equivalent of atoi() or atof() and store 0 if no such conversion
                  ** is possible.
                  **
                  ** A NULL value is not changed by this routine.  It remains NULL.
                  */case OpCode.OP_ToNumeric: {
							/* same as TK_TO_NUMERIC, in1 */pIn1=aMem[pOp.p1];
							sqlite3VdbeMemNumerify(pIn1);
							break;
						}
						#endif
						/* Opcode: ToInt P1 * * * *
**
** Force the value in register P1 to be an integer.  If
** The value is currently a real number, drop its fractional part.
** If the value is text or blob, try to convert it to an integer using the
** equivalent of atoi() and store 0 if no such conversion is possible.
**
** A NULL value is not changed by this routine.  It remains NULL.
*/case OpCode.OP_ToInt: {
							/* same as TK_TO_INT, in1 */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)==0) {
								sqlite3VdbeMemIntegerify(pIn1);
							}
							break;
						}
						#if !(SQLITE_OMIT_CAST) && !(SQLITE_OMIT_FLOATING_POINT)
						/* Opcode: ToReal P1 * * * *
**
** Force the value in register P1 to be a floating point number.
** If The value is currently an integer, convert it.
** If the value is text or blob, try to convert it to an integer using the
** equivalent of atoi() and store 0.0 if no such conversion is possible.
**
** A NULL value is not changed by this routine.  It remains NULL.
*/case OpCode.OP_ToReal: {
							/* same as TK_TO_REAL, in1 */pIn1=aMem[pOp.p1];
							memAboutToChange(this,pIn1);
							if((pIn1.flags&MEM_Null)==0) {
								sqlite3VdbeMemRealify(pIn1);
							}
							break;
						}
						#endif
						/* Opcode: Lt P1 P2 P3 P4 P5
**
** Compare the values in register P1 and P3.  If reg(P3)<reg(P1) then
** jump to address P2.
**
** If the SQLITE_JUMPIFNULL bit of P5 is set and either reg(P1) or
** reg(P3) is NULL then take the jump.  If the SQLITE_JUMPIFNULL
** bit is clear then fall through if either operand is NULL.
**
** The SQLITE_AFF_MASK portion of P5 must be an affinity character -
** SQLITE_AFF_TEXT, SQLITE_AFF_INTEGER, and so forth. An attempt is made
** to coerce both inputs according to this affinity before the
** comparison is made. If the SQLITE_AFF_MASK is 0x00, then numeric
** affinity is used. Note that the affinity conversions are stored
** back into the input registers P1 and P3.  So this opcode can cause
** persistent changes to registers P1 and P3.
**
** Once any conversions have taken place, and neither value is NULL,
** the values are compared. If both values are blobs then memcmp() is
** used to determine the results of the comparison.  If both values
** are text, then the appropriate collating function specified in
** P4 is  used to do the comparison.  If P4 is not specified then
** memcmp() is used to compare text string.  If both values are
** numeric, then a numeric comparison is used. If the two values
** are of different types, then numbers are considered less than
** strings and strings are considered less than blobs.
**
** If the SQLITE_STOREP2 bit of P5 is set, then do not jump.  Instead,
** store a boolean result (either 0, or 1, or NULL) in register P2.
*//* Opcode: Ne P1 P2 P3 P4 P5
                                **
                                ** This works just like the Lt opcode except that the jump is taken if
                                ** the operands in registers P1 and P3 are not equal.  See the Lt opcode for
                                ** additional information.
                                **
                                ** If SQLITE_NULLEQ is set in P5 then the result of comparison is always either
                                ** true or false and is never NULL.  If both operands are NULL then the result
                                ** of comparison is false.  If either operand is NULL then the result is true.
                                ** If neither operand is NULL the result is the same as it would be if
                                ** the SQLITE_NULLEQ flag were omitted from P5.
                                *//* Opcode: Eq P1 P2 P3 P4 P5
                      **
                      ** This works just like the Lt opcode except that the jump is taken if
                      ** the operands in registers P1 and P3 are equal.
                      ** See the Lt opcode for additional information.
                      **
                      ** If SQLITE_NULLEQ is set in P5 then the result of comparison is always either
                      ** true or false and is never NULL.  If both operands are NULL then the result
                      ** of comparison is true.  If either operand is NULL then the result is false.
                      ** If neither operand is NULL the result is the same as it would be if
                      ** the SQLITE_NULLEQ flag were omitted from P5.
                      *//* Opcode: Le P1 P2 P3 P4 P5
                      **
                      ** This works just like the Lt opcode except that the jump is taken if
                      ** the content of register P3 is less than or equal to the content of
                      ** register P1.  See the Lt opcode for additional information.
                      *//* Opcode: Gt P1 P2 P3 P4 P5
                      **
                      ** This works just like the Lt opcode except that the jump is taken if
                      ** the content of register P3 is greater than the content of
                      ** register P1.  See the Lt opcode for additional information.
                      *//* Opcode: Ge P1 P2 P3 P4 P5
                      **
                      ** This works just like the Lt opcode except that the jump is taken if
                      ** the content of register P3 is greater than or equal to the content of
                      ** register P1.  See the Lt opcode for additional information.
                      */case OpCode.OP_Eq:
						/* same as TK_EQ, jump, in1, in3 */case OpCode.OP_Ne:
						/* same as TK_NE, jump, in1, in3 */case OpCode.OP_Lt:
						/* same as TK_LT, jump, in1, in3 */case OpCode.OP_Le:
						/* same as TK_LE, jump, in1, in3 */case OpCode.OP_Gt:
						/* same as TK_GT, jump, in1, in3 */case OpCode.OP_Ge: {
							/* same as TK_GE, jump, in1, in3 */int res=0;
							/* Result of the comparison of pIn1 against pIn3 */char affinity;
							/* Affinity to use for comparison */u16 flags1;
							/* Copy of initial value of pIn1->flags */u16 flags3;
							/* Copy of initial value of pIn3->flags */pIn1=aMem[pOp.p1];
							pIn3=aMem[pOp.p3];
							flags1=pIn1.flags;
							flags3=pIn3.flags;
							if(((pIn1.flags|pIn3.flags)&MEM_Null)!=0) {
								/* One or both operands are NULL */if((pOp.p5&SQLITE_NULLEQ)!=0) {
									/* If SQLITE_NULLEQ is set (which will only happen if the operator is
                              ** OP_Eq or OP_Ne) then take the jump or not depending on whether
                              ** or not both operands are null.
                              */Debug.Assert(pOp.opcode==OP_Eq||pOp.opcode==OP_Ne);
									res=(pIn1.flags&pIn3.flags&MEM_Null)==0?1:0;
								}
								else {
									/* SQLITE_NULLEQ is clear and at least one operand is NULL,
                              ** then the result is always NULL.
                              ** The jump is taken if the SQLITE_JUMPIFNULL bit is set.
                              */if((pOp.p5&SQLITE_STOREP2)!=0) {
										pOut=aMem[pOp.p2];
										pOut.MemSetTypeFlag(MEM_Null);
										REGISTER_TRACE(this,pOp.p2,pOut);
									}
									else
										if((pOp.p5&SQLITE_JUMPIFNULL)!=0) {
											opcodeIndex=pOp.p2-1;
										}
									break;
								}
							}
							else {
								/* Neither operand is NULL.  Do a comparison. */affinity=(char)(pOp.p5&SQLITE_AFF_MASK);
								if(affinity!='\0') {
									applyAffinity(pIn1,affinity,encoding);
									applyAffinity(pIn3,affinity,encoding);
									//      if ( db.mallocFailed != 0 ) goto no_mem;
								}
								Debug.Assert(pOp.p4type==P4_COLLSEQ||pOp.p4.pColl==null);
								ExpandBlob(pIn1);
								ExpandBlob(pIn3);
								res=sqlite3MemCompare(pIn3,pIn1,pOp.p4.pColl);
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
							if((pOp.p5&SQLITE_STOREP2)!=0) {
								pOut=aMem[pOp.p2];
								memAboutToChange(this,pOut);
								pOut.MemSetTypeFlag(MEM_Int);
								pOut.u.i=res;
								REGISTER_TRACE(this,pOp.p2,pOut);
							}
							else
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
								}
							/* Undo any changes made by applyAffinity() to the input registers. */pIn1.flags=(u16)((pIn1.flags&~MEM_TypeMask)|(flags1&MEM_TypeMask));
							pIn3.flags=(u16)((pIn3.flags&~MEM_TypeMask)|(flags3&MEM_TypeMask));
							break;
						}
						/* Opcode: Permutation * * * P4 *
                  **
                  ** Set the permutation used by the OP_Compare operator to be the array
                  ** of integers in P4.
                  **
                  ** The permutation is only valid until the next OP_Permutation, OP_Compare,
                  ** OP_Halt, or OP_ResultRow.  Typically the OP_Permutation should occur
                  ** immediately prior to the OP_Compare.
                  */case OpCode.OP_Permutation: {
							Debug.Assert(pOp.p4type==P4_INTARRAY);
							Debug.Assert(pOp.p4.ai!=null);
							aPermute=pOp.p4.ai;
							break;
						}
						/* Opcode: Compare P1 P2 P3 P4 *
                  **
                  ** Compare two vectors of registers in reg(P1)..reg(P1+P3-1) (call this
                  ** vector "A") and in reg(P2)..reg(P2+P3-1) ("B").  Save the result of
                  ** the comparison for use by the next OP_Jump instruct.
                  **
                  ** P4 is a KeyInfo structure that defines collating sequences and sort
                  ** orders for the comparison.  The permutation applies to registers
                  ** only.  The KeyInfo elements are used sequentially.
                  **
                  ** The comparison is a sort comparison, so NULLs compare equal,
                  ** NULLs are less than numbers, numbers are less than strings,
                  ** and strings are less than blobs.
                  */case OpCode.OP_Compare: {
							int n;
							int i;
							int p1;
							int p2;
							KeyInfo pKeyInfo;
							int idx;
							CollSeq pColl;
							/* Collating sequence to use on this term */int bRev;
							/* True for DESCENDING sort order */n=pOp.p3;
							pKeyInfo=pOp.p4.pKeyInfo;
							Debug.Assert(n>0);
							Debug.Assert(pKeyInfo!=null);
							p1=pOp.p1;
							p2=pOp.p2;
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
							for(i=0;i<n;i++) {
								idx=aPermute!=null?aPermute[i]:i;
								Debug.Assert(aMem[p1+idx].memIsValid());
								Debug.Assert(aMem[p2+idx].memIsValid());
								REGISTER_TRACE(this,p1+idx,aMem[p1+idx]);
								REGISTER_TRACE(this,p2+idx,aMem[p2+idx]);
								Debug.Assert(i<pKeyInfo.nField);
								pColl=pKeyInfo.aColl[i];
								bRev=pKeyInfo.aSortOrder[i];
								iCompare=sqlite3MemCompare(aMem[p1+idx],aMem[p2+idx],pColl);
								if(iCompare!=0) {
									if(bRev!=0)
										iCompare=-iCompare;
									break;
								}
							}
							aPermute=null;
							break;
						}
						/* Opcode: Jump P1 P2 P3 * *
                  **
                  ** Jump to the instruction at address P1, P2, or P3 depending on whether
                  ** in the most recent OP_Compare instruction the P1 vector was less than
                  ** equal to, or greater than the P2 vector, respectively.
                  */case OpCode.OP_Jump: {
							/* jump */if(iCompare<0) {
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
						/* Opcode: And P1 P2 P3 * *
                  **
                  ** Take the logical AND of the values in registers P1 and P2 and
                  ** write the result into register P3.
                  **
                  ** If either P1 or P2 is 0 (false) then the result is 0 even if
                  ** the other input is NULL.  A NULL and true or two NULLs give
                  ** a NULL output.
                  *//* Opcode: Or P1 P2 P3 * *
                      **
                      ** Take the logical OR of the values in register P1 and P2 and
                      ** store the answer in register P3.
                      **
                      ** If either P1 or P2 is nonzero (true) then the result is 1 (true)
                      ** even if the other input is NULL.  A NULL and false or two NULLs
                      ** give a NULL output.
                      */case OpCode.OP_And:
						/* same as TK_AND, in1, in2, ref3 */case OpCode.OP_Or: {
							/* same as TK_OR, in1, in2, ref3 */int v1;
							/* Left operand:  0==FALSE, 1==TRUE, 2==UNKNOWN or NULL */int v2;
							/* Right operand: 0==FALSE, 1==TRUE, 2==UNKNOWN or NULL */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)!=0) {
								v1=2;
							}
							else {
								v1=(sqlite3VdbeIntValue(pIn1)!=0)?1:0;
							}
							pIn2=aMem[pOp.p2];
							if((pIn2.flags&MEM_Null)!=0) {
								v2=2;
							}
							else {
								v2=(sqlite3VdbeIntValue(pIn2)!=0)?1:0;
							}
							if(pOp.OpCode==OpCode.OP_And) {
								byte[] and_logic=new byte[] {
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
								v1=and_logic[v1*3+v2];
							}
							else {
								byte[] or_logic=new byte[] {
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
								v1=or_logic[v1*3+v2];
							}
							pOut=aMem[pOp.p3];
							if(v1==2) {
								pOut.MemSetTypeFlag(MEM_Null);
							}
							else {
								pOut.u.i=v1;
								pOut.MemSetTypeFlag(MEM_Int);
							}
							break;
						}
						/* Opcode: Not P1 P2 * * *
                  **
                  ** Interpret the value in register P1 as a boolean value.  Store the
                  ** boolean complement in register P2.  If the value in register P1 is
                  ** NULL, then a NULL is stored in P2.
                  */case OpCode.OP_Not: {
							/* same as TK_NOT, in1 */pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							if((pIn1.flags&MEM_Null)!=0) {
								sqlite3VdbeMemSetNull(pOut);
							}
							else {
								sqlite3VdbeMemSetInt64(pOut,sqlite3VdbeIntValue(pIn1)==0?1:0);
							}
							break;
						}
						/* Opcode: BitNot P1 P2 * * *
                  **
                  ** Interpret the content of register P1 as an integer.  Store the
                  ** ones-complement of the P1 value into register P2.  If P1 holds
                  ** a NULL then store a NULL in P2.
                  */case OpCode.OP_BitNot: {
							/* same as TK_BITNOT, in1 */pIn1=aMem[pOp.p1];
							pOut=aMem[pOp.p2];
							if((pIn1.flags&MEM_Null)!=0) {
								sqlite3VdbeMemSetNull(pOut);
							}
							else {
								sqlite3VdbeMemSetInt64(pOut,~sqlite3VdbeIntValue(pIn1));
							}
							break;
						}
						/* Opcode: If P1 P2 P3 * *
                  **
                  ** Jump to P2 if the value in register P1 is true.  The value
                  ** is considered true if it is numeric and non-zero.  If the value
                  ** in P1 is NULL then take the jump if P3 is true.
                  *//* Opcode: IfNot P1 P2 P3 * *
                      **
                      ** Jump to P2 if the value in register P1 is False.  The value
                      ** is considered true if it has a numeric value of zero.  If the value
                      ** in P1 is NULL then take the jump if P3 is true.
                      */case OpCode.OP_If:
						/* jump, in1 */case OpCode.OP_IfNot: {
							/* jump, in1 */int c;
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)!=0) {
								c=pOp.p3;
							}
							else {
								#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																													c = sqlite3VdbeIntValue(pIn1)!=0;
#else
								c=(sqlite3VdbeRealValue(pIn1)!=0.0)?1:0;
								#endif
								if(pOp.OpCode==OpCode.OP_IfNot)
									c=(c==0)?1:0;
							}
							if(c!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: IsNull P1 P2 * * *
                  **
                  ** Jump to P2 if the value in register P1 is NULL.
                  */case OpCode.OP_IsNull: {
							/* same as TK_ISNULL, jump, in1 */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)!=0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: NotNull P1 P2 * * *
                  **
                  ** Jump to P2 if the value in register P1 is not NULL.
                  */case OpCode.OP_NotNull: {
							/* same as TK_NOTNULL, jump, in1 */pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_Null)==0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: Column P1 P2 P3 P4 *
                  **
                  ** Interpret the data that cursor P1 points to as a structure built using
                  ** the MakeRecord instruction.  (See the MakeRecord opcode for additional
                  ** information about the format of the data.)  Extract the P2-th column
                  ** from this record.  If there are less that (P2+1)
                  ** values in the record, extract a NULL.
                  **
                  ** The value extracted is stored in register P3.
                  **
                  ** If the column contains fewer than P2 fields, then extract a NULL.  Or,
                  ** if the P4 argument is a P4_MEM use the value of the P4 argument as
                  ** the result.
                  **
                  ** If the OPFLAG_CLEARCACHE bit is set on P5 and P1 is a pseudo-table cursor,
                  ** then the cache of the cursor is reset prior to extracting the column.
                  ** The first OP_Column against a pseudo-table after the value of the content
                  ** register has changed should have this bit set.
                  */case OpCode.OP_Column: {
							u32 payloadSize;
							/* Number of bytes in the record */i64 payloadSize64;
							/* Number of bytes in the record */int p1;
							/* P1 value of the opcode */int p2;
							/* column number to retrieve */VdbeCursor pC;
							/* The VDBE cursor */byte[] zRec;
							/* Pointer to complete record-data */BtCursor pCrsr;
							/* The BTree cursor */u32[] aType;
							/* aType[i] holds the numeric type of the i-th column */u32[] aOffset;
							/* aOffset[i] is offset to start of data for i-th column */int nField;
							/* number of fields in the record */int len;
							/* The length of the serialized data for the column */int i;
							/* Loop counter */byte[] zData=null;
							/* Part of the record being decoded */Mem pDest;
							/* Where to write the extracted value */Mem sMem=null;
							/* For storing the record being decoded */int zIdx;
							/* Index into header */int zEndHdr;
							/* Pointer to first byte after the header */u32 offset;
							/* Offset into the data */u32 szField=0;
							/* Number of bytes in the content of a field */int szHdr;
							/* Size of the header size field at start of record */int avail;
							/* Number of bytes of available data */Mem pReg;
							/* PseudoTable input register */p1=pOp.p1;
							p2=pOp.p2;
							pC=null;
							payloadSize=0;
							payloadSize64=0;
							offset=0;
							sMem=sqlite3Malloc(sMem);
							//  memset(&sMem, 0, sizeof(sMem));
							Debug.Assert(p1<this.nCursor);
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pDest=aMem[pOp.p3];
							memAboutToChange(this,pDest);
							pDest.MemSetTypeFlag(MEM_Null);
							zRec=null;
							/* This block sets the variable payloadSize to be the total number of
                          ** bytes in the record.
                          **
                          ** zRec is set to be the complete text of the record if it is available.
                          ** The complete record text is always available for pseudo-tables
                          ** If the record is stored in a cursor, the complete record text
                          ** might be available in the  pC.aRow cache.  Or it might not be.
                          ** If the data is unavailable,  zRec is set to NULL.
                          **
                          ** We also compute the number of columns in the record.  For cursors,
                          ** the number of columns is stored in the VdbeCursor.nField element.
                          */pC=this.apCsr[p1];
							Debug.Assert(pC!=null);
							#if !SQLITE_OMIT_VIRTUALTABLE
							Debug.Assert(pC.pVtabCursor==null);
							#endif
							pCrsr=pC.pCursor;
							if(pCrsr!=null) {
								/* The record is stored in a B-Tree */rc=sqlite3VdbeCursorMoveto(pC);
								if(rc!=0)
									goto abort_due_to_error;
								if(pC.nullRow) {
									payloadSize=0;
								}
								else
									if((pC.cacheStatus==this.cacheCtr)&&(pC.aRow!=-1)) {
										payloadSize=pC.payloadSize;
										zRec=sqlite3Malloc((int)payloadSize);
										Buffer.BlockCopy(pCrsr.info.pCell,pC.aRow,zRec,0,(int)payloadSize);
									}
									else
										if(pC.isIndex) {
											Debug.Assert(sqlite3BtreeCursorIsValid(pCrsr));
											rc=sqlite3BtreeKeySize(pCrsr,ref payloadSize64);
											Debug.Assert(rc==SQLITE_OK);
											/* True because of CursorMoveto() call above *//* sqlite3BtreeParseCellPtr() uses getVarint32() to extract the
** payload size, so it is impossible for payloadSize64 to be
** larger than 32 bits. */Debug.Assert(((u64)payloadSize64&SQLITE_MAX_U32)==(u64)payloadSize64);
											payloadSize=(u32)payloadSize64;
										}
										else {
											Debug.Assert(sqlite3BtreeCursorIsValid(pCrsr));
											rc=sqlite3BtreeDataSize(pCrsr,ref payloadSize);
											Debug.Assert(rc==SQLITE_OK);
											/* DataSize() cannot fail */}
							}
							else
								if(pC.pseudoTableReg>0) {
									/* The record is the sole entry of a pseudo-table */pReg=aMem[pC.pseudoTableReg];
									Debug.Assert((pReg.flags&MEM_Blob)!=0);
									Debug.Assert(pReg.memIsValid());
									payloadSize=(u32)pReg.n;
									zRec=pReg.zBLOB;
									pC.cacheStatus=(pOp.p5&OPFLAG_CLEARCACHE)!=0?CACHE_STALE:this.cacheCtr;
									Debug.Assert(payloadSize==0||zRec!=null);
								}
								else {
									/* Consider the row to be NULL */payloadSize=0;
								}
							/* If payloadSize is 0, then just store a NULL */if(payloadSize==0) {
								Debug.Assert((pDest.flags&MEM_Null)!=0);
								goto op_column_out;
							}
							Debug.Assert(db.aLimit[SQLITE_LIMIT_LENGTH]>=0);
							if(payloadSize>(u32)db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							nField=pC.nField;
							Debug.Assert(p2<nField);
							/* Read and parse the table header.  Store the results of the parse
                          ** into the record header cache fields of the cursor.
                          */aType=pC.aType;
							if(pC.cacheStatus==this.cacheCtr) {
								aOffset=pC.aOffset;
							}
							else {
								Debug.Assert(aType!=null);
								avail=0;
								//pC.aOffset = aOffset = aType[nField];
								aOffset=new u32[nField];
								pC.aOffset=aOffset;
								pC.payloadSize=payloadSize;
								pC.cacheStatus=this.cacheCtr;
								/* Figure out how many bytes are in the header */if(zRec!=null) {
									zData=zRec;
								}
								else {
									if(pC.isIndex) {
										zData=sqlite3BtreeKeyFetch(pCrsr,ref avail,ref pC.aRow);
									}
									else {
										zData=sqlite3BtreeDataFetch(pCrsr,ref avail,ref pC.aRow);
									}
									/* If KeyFetch()/DataFetch() managed to get the entire payload,
                              ** save the payload in the pC.aRow cache.  That will save us from
                              ** having to make additional calls to fetch the content portion of
                              ** the record.
                              */Debug.Assert(avail>=0);
									if(payloadSize<=(u32)avail) {
										zRec=zData;
										//pC.aRow = zData;
									}
									else {
										pC.aRow=-1;
										//pC.aRow = null;
									}
								}
								/* The following Debug.Assert is true in all cases accept when
                            ** the database file has been corrupted externally.
                            **    Debug.Assert( zRec!=0 || avail>=payloadSize || avail>=9 ); */szHdr=getVarint32(zData,out offset);
								/* Make sure a corrupt database has not given us an oversize header.
                            ** Do this now to avoid an oversize memory allocation.
                            **
                            ** Type entries can be between 1 and 5 bytes each.  But 4 and 5 byte
                            ** types use so much data space that there can only be 4096 and 32 of
                            ** them, respectively.  So the maximum header length results from a
                            ** 3-byte type for each of the maximum of 32768 columns plus three
                            ** extra bytes for the header length itself.  32768*3 + 3 = 98307.
                            */if(offset>98307) {
									rc=SQLITE_CORRUPT_BKPT();
									goto op_column_out;
								}
								/* Compute in len the number of bytes of data we need to read in order
                            ** to get nField type values.  offset is an upper bound on this.  But
                            ** nField might be significantly less than the true number of columns
                            ** in the table, and in that case, 5*nField+3 might be smaller than offset.
                            ** We want to minimize len in order to limit the size of the memory
                            ** allocation, especially if a corrupt database file has caused offset
                            ** to be oversized. Offset is limited to 98307 above.  But 98307 might
                            ** still exceed Robson memory allocation limits on some configurations.
                            ** On systems that cannot tolerate large memory allocations, nField*5+3
                            ** will likely be much smaller since nField will likely be less than
                            ** 20 or so.  This insures that Robson memory allocation limits are
                            ** not exceeded even for corrupt database files.
                            */len=nField*5+3;
								if(len>(int)offset)
									len=(int)offset;
								/* The KeyFetch() or DataFetch() above are fast and will get the entire
                            ** record header in most cases.  But they will fail to get the complete
                            ** record header if the record header does not fit on a single page
                            ** in the B-Tree.  When that happens, use sqlite3VdbeMemFromBtree() to
                            ** acquire the complete header text.
                            */if(zRec==null&&avail<len) {
									sMem.db=null;
									sMem.flags=0;
									rc=sqlite3VdbeMemFromBtree(pCrsr,0,len,pC.isIndex,sMem);
									if(rc!=SQLITE_OK) {
										goto op_column_out;
									}
									zData=sMem.zBLOB;
								}
								zEndHdr=len;
								// zData[len];
								zIdx=szHdr;
								// zData[szHdr];
								/* Scan the header and use it to fill in the aType[] and aOffset[]
                            ** arrays.  aType[i] will contain the type integer for the i-th
                            ** column and aOffset[i] will contain the offset from the beginning
                            ** of the record to the start of the data for the i-th column
                            */for(i=0;i<nField;i++) {
									if(zIdx<zEndHdr) {
										aOffset[i]=offset;
										zIdx+=getVarint32(zData,zIdx,out aType[i]);
										//getVarint32(zIdx, aType[i]);
										szField=sqlite3VdbeSerialTypeLen(aType[i]);
										offset+=szField;
										if(offset<szField) {
											/* True if offset overflows */zIdx=int.MaxValue;
											/* Forces SQLITE_CORRUPT return below */break;
										}
									}
									else {
										/* If i is less that nField, then there are less fields in this
                                ** record than SetNumColumns indicated there are columns in the
                                ** table. Set the offset for any extra columns not present in
                                ** the record to 0. This tells code below to store a NULL
                                ** instead of deserializing a value from the record.
                                */aOffset[i]=0;
									}
								}
								sqlite3VdbeMemRelease(sMem);
								sMem.flags=MEM_Null;
								/* If we have read more header data than was contained in the header,
                            ** or if the end of the last field appears to be past the end of the
                            ** record, or if the end of the last field appears to be before the end
                            ** of the record (when all fields present), then we must be dealing
                            ** with a corrupt database.
                            */if((zIdx>zEndHdr)||(offset>payloadSize)||(zIdx==zEndHdr&&offset!=payloadSize)) {
									rc=SQLITE_CORRUPT_BKPT();
									goto op_column_out;
								}
							}
							/* Get the column information. If aOffset[p2] is non-zero, then
                          ** deserialize the value from the record. If aOffset[p2] is zero,
                          ** then there are not enough fields in the record to satisfy the
                          ** request.  In this case, set the value NULL or to P4 if P4 is
                          ** a pointer to a Mem object.
                          */if(aOffset[p2]!=0) {
								Debug.Assert(rc==SQLITE_OK);
								if(zRec!=null) {
									sqlite3VdbeMemReleaseExternal(pDest);
									sqlite3VdbeSerialGet(zRec,(int)aOffset[p2],aType[p2],pDest);
								}
								else {
									len=(int)sqlite3VdbeSerialTypeLen(aType[p2]);
									sqlite3VdbeMemMove(sMem,pDest);
									rc=sqlite3VdbeMemFromBtree(pCrsr,(int)aOffset[p2],len,pC.isIndex,sMem);
									if(rc!=SQLITE_OK) {
										goto op_column_out;
									}
									zData=sMem.zBLOB;
									sMem.zBLOB=null;
									sqlite3VdbeSerialGet(zData,aType[p2],pDest);
								}
								pDest.enc=encoding;
							}
							else {
								if(pOp.p4type==P4_MEM) {
									sqlite3VdbeMemShallowCopy(pDest,pOp.p4.pMem,MEM_Static);
								}
								else {
									Debug.Assert((pDest.flags&MEM_Null)!=0);
								}
							}
							/* If we dynamically allocated space to hold the data (in the
                          ** sqlite3VdbeMemFromBtree() call above) then transfer control of that
                          ** dynamically allocated space over to the pDest structure.
                          ** This prevents a memory copy.
                          *///if ( sMem.zMalloc != null )
							//{
							//  Debug.Assert( sMem.z == sMem.zMalloc);
							//  Debug.Assert( sMem.xDel == null );
							//  Debug.Assert( ( pDest.flags & MEM_Dyn ) == 0 );
							//  Debug.Assert( ( pDest.flags & ( MEM_Blob | MEM_Str ) ) == 0 || pDest.z == sMem.z );
							//  pDest.flags &= ~( MEM_Ephem | MEM_Static );
							//  pDest.flags |= MEM_Term;
							//  pDest.z = sMem.z;
							//  pDest.zMalloc = sMem.zMalloc;
							//}
							rc=sqlite3VdbeMemMakeWriteable(pDest);
							op_column_out:
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pDest );
#endif
							REGISTER_TRACE(this,pOp.p3,pDest);
							if(zData!=null&&zData!=zRec)
								sqlite3_free(ref zData);
							//sqlite3_free( ref zRec );
							sqlite3_free(ref sMem);
							break;
						}
						/* Opcode: Affinity P1 P2 * P4 *
                  **
                  ** Apply affinities to a range of P2 registers starting with P1.
                  **
                  ** P4 is a string that is P2 characters long. The nth character of the
                  ** string indicates the column affinity that should be used for the nth
                  ** memory cell in the range.
                  */case OpCode.OP_Affinity: {
							string zAffinity;
							/* The affinity to be applied */char cAff;
							/* A single character of affinity */zAffinity=pOp.p4.z;
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
						/* Opcode: MakeRecord P1 P2 P3 P4 *
                  **
                  ** Convert P2 registers beginning with P1 into the [record format]
                  ** use as a data record in a database table or as a key
                  ** in an index.  The OP_Column opcode can decode the record later.
                  **
                  ** P4 may be a string that is P2 characters long.  The nth character of the
                  ** string indicates the column affinity that should be used for the nth
                  ** field of the index key.
                  **
                  ** The mapping from character to affinity is given by the SQLITE_AFF_
                  ** macros defined in sqliteInt.h.
                  **
                  ** If P4 is NULL then all index fields have the affinity NONE.
                  */case OpCode.OP_MakeRecord: {
							byte[] zNewRecord;
							/* A buffer to hold the data for the new record */Mem pRec;
							/* The new record */u64 nData;
							/* Number of bytes of data space */int nHdr;
							/* Number of bytes of header space */i64 nByte;
							/* Data space required for this record */int nZero;
							/* Number of zero bytes at the end of the record */int nVarint;
							/* Number of bytes in a varint */u32 serial_type;
							/* Type field *///Mem pData0;            /* First field to be combined into the record */
							//Mem pLast;             /* Last field of the record */
							int nField;
							/* Number of fields in the record */string zAffinity;
							/* The affinity string for the record */int file_format;
							/* File format to use for encoding */int i;
							/* Space used in zNewRecord[] */int len;
							/* Length of a field *//* Assuming the record contains N fields, the record format looks
** like this:
**
** ------------------------------------------------------------------------
** | hdr-size | type 0 | type 1 | ... | type N-1 | data0 | ... | data N-1 |
** ------------------------------------------------------------------------
**
** Data(0) is taken from register P1.  Data(1) comes from register P1+1
** and so froth.
**
** Each type field is a varint representing the serial type of the
** corresponding data element (see sqlite3VdbeSerialType()). The
** hdr-size field is also a varint which is the offset from the beginning
** of the record to data0.
*/nData=0;
							/* Number of bytes of data space */nHdr=0;
							/* Number of bytes of header space */nZero=0;
							/* Number of zero bytes at the end of the record */nField=pOp.p1;
							zAffinity=(pOp.p4.z==null||pOp.p4.z.Length==0)?"":pOp.p4.z;
							Debug.Assert(nField>0&&pOp.p2>0&&pOp.p2+nField<=this.nMem+1);
							//pData0 = aMem[nField];
							nField=pOp.p2;
							//pLast =  pData0[nField - 1];
							file_format=this.minWriteFileFormat;
							/* Identify the output register */Debug.Assert(pOp.p3<pOp.p1||pOp.p3>=pOp.p1+pOp.p2);
							pOut=aMem[pOp.p3];
							memAboutToChange(this,pOut);
							/* Loop through the elements that will make up the record to figure
                          ** out how much space is required for the new record.
                          *///for (pRec = pData0; pRec <= pLast; pRec++)
							for(int pD0=0;pD0<nField;pD0++) {
								pRec=this.aMem[pOp.p1+pD0];
								Debug.Assert(pRec.memIsValid());
								if(pD0<zAffinity.Length&&zAffinity[pD0]!='\0') {
									applyAffinity(pRec,(char)zAffinity[pD0],encoding);
								}
								if((pRec.flags&MEM_Zero)!=0&&pRec.n>0) {
									pRec.sqlite3VdbeMemExpandBlob();
								}
								serial_type=sqlite3VdbeSerialType(pRec,file_format);
								len=(int)sqlite3VdbeSerialTypeLen(serial_type);
								nData+=(u64)len;
								nHdr+=sqlite3VarintLen(serial_type);
								if((pRec.flags&MEM_Zero)!=0) {
									/* Only pure zero-filled BLOBs can be input to this Opcode.
                              ** We do not allow blobs with a prefix and a zero-filled tail. */nZero+=pRec.u.nZero;
								}
								else
									if(len!=0) {
										nZero=0;
									}
							}
							/* Add the initial header varint and total the size */nHdr+=nVarint=sqlite3VarintLen((u64)nHdr);
							if(nVarint<sqlite3VarintLen((u64)nHdr)) {
								nHdr++;
							}
							nByte=(i64)((u64)nHdr+nData-(u64)nZero);
							if(nByte>db.aLimit[SQLITE_LIMIT_LENGTH]) {
								goto too_big;
							}
							/* Make sure the output register has a buffer large enough to store
                          ** the new record. The output register (pOp.p3) is not allowed to
                          ** be one of the input registers (because the following call to
                          ** sqlite3VdbeMemGrow() could clobber the value before it is used).
                          *///if ( sqlite3VdbeMemGrow( pOut, (int)nByte, 0 ) != 0 )
							//{
							//  goto no_mem;
							//}
							zNewRecord=sqlite3Malloc((int)nByte);
							// (u8 )pOut.z;
							/* Write the record */i=putVarint32(zNewRecord,nHdr);
							for(int pD0=0;pD0<nField;pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
							 {
								pRec=this.aMem[pOp.p1+pD0];
								serial_type=sqlite3VdbeSerialType(pRec,file_format);
								i+=putVarint32(zNewRecord,i,(int)serial_type);
								/* serial type */}
							for(int pD0=0;pD0<nField;pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
							 {
								/* serial data */pRec=this.aMem[pOp.p1+pD0];
								i+=(int)sqlite3VdbeSerialPut(zNewRecord,i,(int)nByte-i,pRec,file_format);
							}
							//TODO -- Remove this  for testing Debug.Assert( i == nByte );
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pOut.zBLOB=zNewRecord;
							pOut.z=null;
							pOut.n=(int)nByte;
							pOut.flags=MEM_Blob|MEM_Dyn;
							pOut.xDel=null;
							if(nZero!=0) {
								pOut.u.nZero=nZero;
								pOut.flags|=MEM_Zero;
							}
							pOut.enc=SqliteEncoding.UTF8;
							/* In case the blob is ever converted to text */REGISTER_TRACE(this,pOp.p3,pOut);
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						/* Opcode: Count P1 P2 * * *
                  **
                  ** Store the number of entries (an integer value) in the table or index
                  ** opened by cursor P1 in register P2
                  */
						#if !SQLITE_OMIT_BTREECOUNT
						case OpCode.OP_Count: {
							/* out2-prerelease */i64 nEntry=0;
							BtCursor pCrsr;
							pCrsr=this.apCsr[pOp.p1].pCursor;
							if(pCrsr!=null) {
								rc=sqlite3BtreeCount(pCrsr,ref nEntry);
							}
							else {
								nEntry=0;
							}
							pOut.u.i=nEntry;
							break;
						}
						#endif
						/* Opcode: Savepoint P1 * * P4 *
**
** Open, release or rollback the savepoint named by parameter P4, depending
** on the value of P1. To open a new savepoint, P1==0. To release (commit) an
** existing savepoint, P1==1, or to rollback an existing savepoint P1==2.
*/case OpCode.OP_Savepoint: {
							int p1;
							/* Value of P1 operand */string zName;
							/* Name of savepoint */int nName;
							Savepoint pNew;
							Savepoint pSavepoint;
							Savepoint pTmp;
							int iSavepoint;
							int ii;
							p1=pOp.p1;
							zName=pOp.p4.z;
							/* Assert that the p1 parameter is valid. Also that if there is no open
                          ** transaction, then there cannot be any savepoints.
                          */Debug.Assert(db.pSavepoint==null||db.autoCommit==0);
							Debug.Assert(p1==SAVEPOINT_BEGIN||p1==SAVEPOINT_RELEASE||p1==SAVEPOINT_ROLLBACK);
							Debug.Assert(db.pSavepoint!=null||db.isTransactionSavepoint==0);
							Debug.Assert(checkSavepointCount(db)!=0);
							if(p1==SAVEPOINT_BEGIN) {
								if(db.writeVdbeCnt>0) {
									/* A new savepoint cannot be created if there are active write
                              ** statements (i.e. open read/write incremental blob handles).
                              */sqlite3SetString(ref this.zErrMsg,db,"cannot open savepoint - ","SQL statements in progress");
									rc=SQLITE_BUSY;
								}
								else {
									nName=StringExtensions.sqlite3Strlen30(zName);
									#if !SQLITE_OMIT_VIRTUALTABLE
									/* This call is Ok even if this savepoint is actually a transaction
      ** savepoint (and therefore should not prompt xSavepoint()) callbacks.
      ** If this is a transaction savepoint being opened, it is guaranteed
      ** that the db->aVTrans[] array is empty.  */Debug.Assert(db.autoCommit==0||db.nVTrans==0);
									rc=sqlite3VtabSavepoint(db,SAVEPOINT_BEGIN,db.nStatement+db.nSavepoint);
									if(rc!=SQLITE_OK)
										goto abort_due_to_error;
									#endif
									/* Create a new savepoint structure. */pNew=new Savepoint();
									// sqlite3DbMallocRaw( db, sizeof( Savepoint ) + nName + 1 );
									if(pNew!=null) {
										//pNew.zName = (char )&pNew[1];
										//memcpy(pNew.zName, zName, nName+1);
										pNew.zName=zName;
										/* If there is no open transaction, then mark this as a special
                                ** "transaction savepoint". */if(db.autoCommit!=0) {
											db.autoCommit=0;
											db.isTransactionSavepoint=1;
										}
										else {
											db.nSavepoint++;
										}
										/* Link the new savepoint into the database handle's list. */pNew.pNext=db.pSavepoint;
										db.pSavepoint=pNew;
										pNew.nDeferredCons=db.nDeferredCons;
									}
								}
							}
							else {
								iSavepoint=0;
								/* Find the named savepoint. If there is no such savepoint, then an
                            ** an error is returned to the user.  */for(pSavepoint=db.pSavepoint;pSavepoint!=null&&!pSavepoint.zName.Equals(zName,StringComparison.InvariantCultureIgnoreCase);pSavepoint=pSavepoint.pNext) {
									iSavepoint++;
								}
								if(null==pSavepoint) {
									sqlite3SetString(ref this.zErrMsg,db,"no such savepoint: %s",zName);
									rc=SQLITE_ERROR;
								}
								else
									if(db.writeVdbeCnt>0||(p1==SAVEPOINT_ROLLBACK&&db.activeVdbeCnt>1)) {
										/* It is not possible to release (commit) a savepoint if there are
                              ** active write statements. It is not possible to rollback a savepoint
                              ** if there are any active statements at all.
                              */sqlite3SetString(ref this.zErrMsg,db,"cannot %s savepoint - SQL statements in progress",(p1==SAVEPOINT_ROLLBACK?"rollback":"release"));
										rc=SQLITE_BUSY;
									}
									else {
										/* Determine whether or not this is a transaction savepoint. If so,
                              ** and this is a RELEASE command, then the current transaction
                              ** is committed.
                              */int isTransaction=(pSavepoint.pNext==null&&db.isTransactionSavepoint!=0)?1:0;
										if(isTransaction!=0&&p1==SAVEPOINT_RELEASE) {
											if((rc=this.sqlite3VdbeCheckFk(1))!=SQLITE_OK) {
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
												rc=sqlite3BtreeSavepoint(db.aDb[ii].pBt,p1,iSavepoint);
												if(rc!=SQLITE_OK) {
													goto abort_due_to_error;
												}
											}
											if(p1==SAVEPOINT_ROLLBACK&&(db.flags&SQLITE_InternChanges)!=0) {
												sqlite3ExpirePreparedStatements(db);
												sqlite3ResetInternalSchema(db,-1);
												db.flags=(db.flags|SQLITE_InternChanges);
											}
										}
										/* Regardless of whether this is a RELEASE or ROLLBACK, destroy all
                              ** savepoints nested inside of the savepoint being operated on. */while(db.pSavepoint!=pSavepoint) {
											pTmp=db.pSavepoint;
											db.pSavepoint=pTmp.pNext;
											db.sqlite3DbFree(ref pTmp);
											db.nSavepoint--;
										}
										/* If it is a RELEASE, then destroy the savepoint being operated on 
                              ** too. If it is a ROLLBACK TO, then set the number of deferred 
                              ** constraint violations present in the database to the value stored
                              ** when the savepoint was created.  */if(p1==SAVEPOINT_RELEASE) {
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
											rc=sqlite3VtabSavepoint(db,p1,iSavepoint);
											if(rc!=SQLITE_OK)
												goto abort_due_to_error;
										}
									}
							}
							break;
						}
						/* Opcode: AutoCommit P1 P2 * * *
                  **
                  ** Set the database auto-commit flag to P1 (1 or 0). If P2 is true, roll
                  ** back any currently active btree transactions. If there are any active
                  ** VMs (apart from this one), then the COMMIT or ROLLBACK statement fails.
                  **
                  ** This instruction causes the VM to halt.
                  */case OpCode.OP_AutoCommit: {
							int desiredAutoCommit;
							int iRollback;
							int turnOnAC;
							desiredAutoCommit=(u8)pOp.p1;
							iRollback=pOp.p2;
							turnOnAC=(desiredAutoCommit!=0&&0==db.autoCommit)?1:0;
							Debug.Assert(desiredAutoCommit!=0||0==desiredAutoCommit);
							Debug.Assert(desiredAutoCommit!=0||0==iRollback);
							Debug.Assert(db.activeVdbeCnt>0);
							/* At least this one VM is active */if(turnOnAC!=0&&iRollback!=0&&db.activeVdbeCnt>1) {
								/* If this instruction implements a ROLLBACK and other VMs are
                            ** still running, and a transaction is active, return an error indicating
                            ** that the other VMs must complete first.
                            */sqlite3SetString(ref this.zErrMsg,db,"cannot rollback transaction - "+"SQL statements in progress");
								rc=SQLITE_BUSY;
							}
							else
								if(turnOnAC!=0&&0==iRollback&&db.writeVdbeCnt>0) {
									/* If this instruction implements a COMMIT and other VMs are writing
                            ** return an error indicating that the other VMs must complete first.
                            */sqlite3SetString(ref this.zErrMsg,db,"cannot commit transaction - "+"SQL statements in progress");
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
											if((rc=this.sqlite3VdbeCheckFk(1))!=SQLITE_OK) {
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
										if(this.rc==SQLITE_OK) {
											rc=SQLITE_DONE;
										}
										else {
											rc=SQLITE_ERROR;
										}
										goto vdbe_return;
									}
									else {
										sqlite3SetString(ref this.zErrMsg,db,(0==desiredAutoCommit)?"cannot start a transaction within a transaction":((iRollback!=0)?"cannot rollback - no transaction is active":"cannot commit - no transaction is active"));
										rc=SQLITE_ERROR;
									}
							break;
						}
						/* Opcode: Transaction P1 P2 * * *
                  **
                  ** Begin a transaction.  The transaction ends when a Commit or Rollback
                  ** opcode is encountered.  Depending on the ON CONFLICT setting, the
                  ** transaction might also be rolled back if an error is encountered.
                  **
                  ** P1 is the index of the database file on which the transaction is
                  ** started.  Index 0 is the main database file and index 1 is the
                  ** file used for temporary tables.  Indices of 2 or more are used for
                  ** attached databases.
                  **
                  ** If P2 is non-zero, then a write-transaction is started.  A RESERVED lock is
                  ** obtained on the database file when a write-transaction is started.  No
                  ** other process can start another write transaction while this transaction is
                  ** underway.  Starting a write transaction also creates a rollback journal. A
                  ** write transaction must be started before any changes can be made to the
                  ** database.  If P2 is 2 or greater then an EXCLUSIVE lock is also obtained
                  ** on the file.
                  **
                  ** If a write-transaction is started and the Vdbe.usesStmtJournal flag is
                  ** true (this flag is set if the Vdbe may modify more than one row and may
                  ** throw an ABORT exception), a statement transaction may also be opened.
                  ** More specifically, a statement transaction is opened iff the database
                  ** connection is currently not in autocommit mode, or if there are other
                  ** active statements. A statement transaction allows the affects of this
                  ** VDBE to be rolled back after an error without having to roll back the
                  ** entire transaction. If no error is encountered, the statement transaction
                  ** will automatically commit when the VDBE halts.
                  **
                  ** If P2 is zero, then a read-lock is obtained on the database file.
                  */case OpCode.OP_Transaction: {
							Btree pBt;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pBt=db.aDb[pOp.p1].pBt;
							if(pBt!=null) {
								rc=sqlite3BtreeBeginTrans(pBt,pOp.p2);
								if(rc==SQLITE_BUSY) {
									this.currentOpCodeIndex=opcodeIndex;
									this.rc=rc=SQLITE_BUSY;
									goto vdbe_return;
								}
								if(rc!=SQLITE_OK) {
									goto abort_due_to_error;
								}
								if(pOp.p2!=0&&this.usesStmtJournal&&(db.autoCommit==0||db.activeVdbeCnt>1)) {
									Debug.Assert(pBt.sqlite3BtreeIsInTrans());
									if(this.iStatement==0) {
										Debug.Assert(db.nStatement>=0&&db.nSavepoint>=0);
										db.nStatement++;
										this.iStatement=db.nSavepoint+db.nStatement;
									}
									rc=sqlite3VtabSavepoint(db,SAVEPOINT_BEGIN,this.iStatement-1);
									if(rc==SQLITE_OK) {
										rc=sqlite3BtreeBeginStmt(pBt,this.iStatement);
									}
									/* Store the current value of the database handles deferred constraint
                              ** counter. If the statement transaction needs to be rolled back,
                              ** the value of this counter needs to be restored too.  */this.nStmtDefCons=db.nDeferredCons;
								}
							}
							break;
						}
						/* Opcode: ReadCookie P1 P2 P3 * *
                  **
                  ** Read cookie number P3 from database P1 and write it into register P2.
                  ** P3==1 is the schema version.  P3==2 is the database format.
                  ** P3==3 is the recommended pager cache size, and so forth.  P1==0 is
                  ** the main database file and P1==1 is the database file used to store
                  ** temporary tables.
                  **
                  ** There must be a read-lock on the database (either a transaction
                  ** must be started or there must be an open cursor) before
                  ** executing this instruction.
                  */case OpCode.OP_ReadCookie: {
							/* out2-prerelease */u32 iMeta;
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
						/* Opcode: SetCookie P1 P2 P3 * *
                  **
                  ** Write the content of register P3 (interpreted as an integer)
                  ** into cookie number P2 of database P1.  P2==1 is the schema version.
                  ** P2==2 is the database format. P2==3 is the recommended pager cache
                  ** size, and so forth.  P1==0 is the main database file and P1==1 is the
                  ** database file used to store temporary tables.
                  **
                  ** A transaction must be started before executing this opcode.
                  */case OpCode.OP_SetCookie: {
							/* in3 */Db pDb;
							Debug.Assert(pOp.p2<SQLITE_N_BTREE_META);
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pDb=db.aDb[pOp.p1];
							Debug.Assert(pDb.pBt!=null);
							Debug.Assert(sqlite3SchemaMutexHeld(db,pOp.p1,null));
							pIn3=aMem[pOp.p3];
							sqlite3VdbeMemIntegerify(pIn3);
							/* See note about index shifting on OP_ReadCookie */rc=pDb.pBt.sqlite3BtreeUpdateMeta(pOp.p2,(u32)pIn3.u.i);
							if(pOp.p2==BTREE_SCHEMA_VERSION) {
								/* When the schema cookie changes, record the new cookie internally */pDb.pSchema.schema_cookie=(int)pIn3.u.i;
								db.flags|=SQLITE_InternChanges;
							}
							else
								if(pOp.p2==BTREE_FILE_FORMAT) {
									/* Record changes in the file format */pDb.pSchema.file_format=(u8)pIn3.u.i;
								}
							if(pOp.p1==1) {
								/* Invalidate all prepared statements whenever the TEMP database
                            ** schema is changed.  Ticket #1644 */sqlite3ExpirePreparedStatements(db);
								this.expired=false;
							}
							break;
						}
						/* Opcode: VerifyCookie P1 P2 P3 * *
                  **
                  ** Check the value of global database parameter number 0 (the
                  ** schema version) and make sure it is equal to P2 and that the
                  ** generation counter on the local schema parse equals P3.
                  **
                  ** P1 is the database number which is 0 for the main database file
                  ** and 1 for the file holding temporary tables and some higher number
                  ** for auxiliary databases.
                  **
                  ** The cookie changes its value whenever the database schema changes.
                  ** This operation is used to detect when that the cookie has changed
                  ** and that the current process needs to reread the schema.
                  **
                  ** Either a transaction needs to have been started or an OP_Open needs
                  ** to be executed (to establish a read lock) before this opcode is
                  ** invoked.
                  */case OpCode.OP_VerifyCookie: {
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
								/* If the schema-cookie from the database file matches the cookie
                            ** stored with the in-memory representation of the schema, do
                            ** not reload the schema from the database file.
                            **
                            ** If virtual-tables are in use, this is not just an optimization.
                            ** Often, v-tables store their data in other SQLite tables, which
                            ** are queried from within xNext() and other v-table methods using
                            ** prepared queries. If such a query is out-of-date, we do not want to
                            ** discard the database schema, as the user code implementing the
                            ** v-table would have to be ready for the sqlite3_vtab structure itself
                            ** to be invalidated whenever sqlite3_step() is called from within
                            ** a v-table method.
                            */if(db.aDb[pOp.p1].pSchema.schema_cookie!=iMeta) {
									sqlite3ResetInternalSchema(db,pOp.p1);
								}
								this.expired=true;
								rc=SQLITE_SCHEMA;
							}
							break;
						}
						/* Opcode: OpenRead P1 P2 P3 P4 P5
                  **
                  ** Open a read-only cursor for the database table whose root page is
                  ** P2 in a database file.  The database file is determined by P3.
                  ** P3==0 means the main database, P3==1 means the database used for
                  ** temporary tables, and P3>1 means used the corresponding attached
                  ** database.  Give the new cursor an identifier of P1.  The P1
                  ** values need not be contiguous but all P1 values should be small integers.
                  ** It is an error for P1 to be negative.
                  **
                  ** If P5!=0 then use the content of register P2 as the root page, not
                  ** the value of P2 itself.
                  **
                  ** There will be a read lock on the database whenever there is an
                  ** open cursor.  If the database was unlocked prior to this instruction
                  ** then a read lock is acquired as part of this instruction.  A read
                  ** lock allows other processes to read the database but prohibits
                  ** any other process from modifying the database.  The read lock is
                  ** released when all cursors are closed.  If this instruction attempts
                  ** to get a read lock but fails, the script terminates with an
                  ** SQLITE_BUSY error code.
                  **
                  ** The P4 value may be either an integer (P4_INT32) or a pointer to
                  ** a KeyInfo structure (P4_KEYINFO). If it is a pointer to a KeyInfo
                  ** structure, then said structure defines the content and collating
                  ** sequence of the index being opened. Otherwise, if P4 is an integer
                  ** value, it is set to the number of columns in the table.
                  **
                  ** See also OpenWrite.
                  *//* Opcode: OpenWrite P1 P2 P3 P4 P5
                      **
                      ** Open a read/write cursor named P1 on the table or index whose root
                      ** page is P2.  Or if P5!=0 use the content of register P2 to find the
                      ** root page.
                      **
                      ** The P4 value may be either an integer (P4_INT32) or a pointer to
                      ** a KeyInfo structure (P4_KEYINFO). If it is a pointer to a KeyInfo
                      ** structure, then said structure defines the content and collating
                      ** sequence of the index being opened. Otherwise, if P4 is an integer
                      ** value, it is set to the number of columns in the table, or to the
                      ** largest index of any column of the table that is actually used.
                      **
                      ** This instruction works just like OpenRead except that it opens the cursor
                      ** in read/write mode.  For a given table, there can be one or more read-only
                      ** cursors or a single read/write cursor but not both.
                      **
                      ** See also OpenRead.
                      */case OpCode.OP_OpenRead:
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
								Debug.Assert((pIn2.flags&MEM_Int)!=0);
								sqlite3VdbeMemIntegerify(pIn2);
								p2=(int)pIn2.u.i;
								/* The p2 value always comes from a prior OP_CreateTable opcode and
                            ** that opcode will always set the p2 value to 2 or more or else fail.
                            ** If there were a failure, the prepared statement would have halted
                            ** before reaching this instruction. */if(NEVER(p2<2)) {
									rc=SQLITE_CORRUPT_BKPT();
									goto abort_due_to_error;
								}
							}
							if(pOp.p4type==P4_KEYINFO) {
								pKeyInfo=pOp.p4.pKeyInfo;
								pKeyInfo.enc=ENC(this.db);
								nField=pKeyInfo.nField+1;
							}
							else
								if(pOp.p4type==P4_INT32) {
									nField=pOp.p4.i;
								}
							Debug.Assert(pOp.p1>=0);
							pCur=allocateCursor(this,pOp.p1,nField,iDb,1);
							if(pCur==null)
								goto no_mem;
							pCur.nullRow=true;
							pCur.isOrdered=true;
							rc=sqlite3BtreeCursor(pX,p2,wrFlag,pKeyInfo,pCur.pCursor);
							pCur.pKeyInfo=pKeyInfo;
							/* Since it performs no memory allocation or IO, the only values that
                          ** sqlite3BtreeCursor() may return are SQLITE_EMPTY and SQLITE_OK. 
                          ** SQLITE_EMPTY is only returned when attempting to open the table
                          ** rooted at page 1 of a zero-byte database.  */Debug.Assert(rc==SQLITE_EMPTY||rc==SQLITE_OK);
							if(rc==SQLITE_EMPTY) {
								sqlite3MemFreeBtCursor(ref pCur.pCursor);
								rc=SQLITE_OK;
							}
							/* Set the VdbeCursor.isTable and isIndex variables. Previous versions of
                          ** SQLite used to check if the root-page flags were sane at this point
                          ** and report database corruption if they were not, but this check has
                          ** since moved into the btree layer.  */pCur.isTable=pOp.p4type!=P4_KEYINFO;
							pCur.isIndex=!pCur.isTable;
							break;
						}
						/* Opcode: OpenEphemeral P1 P2 * P4 *
                  **
                  ** Open a new cursor P1 to a transient table.
                  ** The cursor is always opened read/write even if 
                  ** the main database is read-only.  The ephemeral
                  ** table is deleted automatically when the cursor is closed.
                  **
                  ** P2 is the number of columns in the ephemeral table.
                  ** The cursor points to a BTree table if P4==0 and to a BTree index
                  ** if P4 is not 0.  If P4 is not NULL, it points to a KeyInfo structure
                  ** that defines the format of keys in the index.
                  **
                  ** This opcode was once called OpenTemp.  But that created
                  ** confusion because the term "temp table", might refer either
                  ** to a TEMP table at the SQL level, or to a table opened by
                  ** this opcode.  Then this opcode was call OpenVirtual.  But
                  ** that created confusion with the whole virtual-table idea.
                  *//* Opcode: OpenAutoindex P1 P2 * P4 *
                      **
                      ** This opcode works the same as OP_OpenEphemeral.  It has a
                      ** different name to distinguish its use.  Tables created using
                      ** by this opcode will be used for automatically created transient
                      ** indices in joins.
                      */case OpCode.OP_OpenAutoindex:
						case OpCode.OP_OpenEphemeral: {
							VdbeCursor pCx;
							const int vfsFlags=SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE|SQLITE_OPEN_EXCLUSIVE|SQLITE_OPEN_DELETEONCLOSE|SQLITE_OPEN_TRANSIENT_DB;
							Debug.Assert(pOp.p1>=0);
							pCx=allocateCursor(this,pOp.p1,pOp.p2,-1,1);
							if(pCx==null)
								goto no_mem;
							pCx.nullRow=true;
							rc=sqlite3BtreeOpen(db.pVfs,null,db,ref pCx.pBt,BTREE_OMIT_JOURNAL|BTREE_SINGLE|pOp.p5,vfsFlags);
							if(rc==SQLITE_OK) {
								rc=sqlite3BtreeBeginTrans(pCx.pBt,1);
							}
							if(rc==SQLITE_OK) {
								/* If a transient index is required, create it by calling
                            ** sqlite3BtreeCreateTable() with the BTREE_BLOBKEY flag before
                            ** opening it. If a transient table is required, just use the
                            ** automatically created table with root-page 1 (an BLOB_INTKEY table).
                            */if(pOp.p4.pKeyInfo!=null) {
									int pgno=0;
									Debug.Assert(pOp.p4type==P4_KEYINFO);
									rc=sqlite3BtreeCreateTable(pCx.pBt,ref pgno,BTREE_BLOBKEY);
									if(rc==SQLITE_OK) {
										Debug.Assert(pgno==MASTER_ROOT+1);
										rc=sqlite3BtreeCursor(pCx.pBt,pgno,1,pOp.p4.pKeyInfo,pCx.pCursor);
										pCx.pKeyInfo=pOp.p4.pKeyInfo;
										pCx.pKeyInfo.enc=ENC(this.db);
									}
									pCx.isTable=false;
								}
								else {
									rc=sqlite3BtreeCursor(pCx.pBt,MASTER_ROOT,1,null,pCx.pCursor);
									pCx.isTable=true;
								}
							}
							pCx.isOrdered=(pOp.p5!=BTREE_UNORDERED);
							pCx.isIndex=!pCx.isTable;
							break;
						}
						/* Opcode: OpenPseudo P1 P2 P3 * *
                  **
                  ** Open a new cursor that points to a fake table that contains a single
                  ** row of data.  The content of that one row in the content of memory
                  ** register P2.  In other words, cursor P1 becomes an alias for the 
                  ** MEM_Blob content contained in register P2.
                  **
                  ** A pseudo-table created by this opcode is used to hold a single
                  ** row output from the sorter so that the row can be decomposed into
                  ** individual columns using the OP_Column opcode.  The OP_Column opcode
                  ** is the only cursor opcode that works with a pseudo-table.
                  **
                  ** P3 is the number of fields in the records that will be stored by
                  ** the pseudo-table.
                  */case OpCode.OP_OpenPseudo: {
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
						/* Opcode: Close P1 * * * *
                  **
                  ** Close a cursor previously opened as P1.  If P1 is not
                  ** currently open, this instruction is a no-op.
                  */case OpCode.OP_Close: {
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							sqlite3VdbeFreeCursor(this,this.apCsr[pOp.p1]);
							this.apCsr[pOp.p1]=null;
							break;
						}
						/* Opcode: SeekGe P1 P2 P3 P4 *
                  **
                  ** If cursor P1 refers to an SQL table (B-Tree that uses integer keys),
                  ** use the value in register P3 as the key.  If cursor P1 refers
                  ** to an SQL index, then P3 is the first in an array of P4 registers
                  ** that are used as an unpacked index key.
                  **
                  ** Reposition cursor P1 so that  it points to the smallest entry that
                  ** is greater than or equal to the key value. If there are no records
                  ** greater than or equal to the key and P2 is not zero, then jump to P2.
                  **
                  ** See also: Found, NotFound, Distinct, SeekLt, SeekGt, SeekLe
                  *//* Opcode: SeekGt P1 P2 P3 P4 *
                      **
                      ** If cursor P1 refers to an SQL table (B-Tree that uses integer keys),
                      ** use the value in register P3 as a key. If cursor P1 refers
                      ** to an SQL index, then P3 is the first in an array of P4 registers
                      ** that are used as an unpacked index key.
                      **
                      ** Reposition cursor P1 so that  it points to the smallest entry that
                      ** is greater than the key value. If there are no records greater than
                      ** the key and P2 is not zero, then jump to P2.
                      **
                      ** See also: Found, NotFound, Distinct, SeekLt, SeekGe, SeekLe
                      *//* Opcode: SeekLt P1 P2 P3 P4 *
                      **
                      ** If cursor P1 refers to an SQL table (B-Tree that uses integer keys),
                      ** use the value in register P3 as a key. If cursor P1 refers
                      ** to an SQL index, then P3 is the first in an array of P4 registers
                      ** that are used as an unpacked index key.
                      **
                      ** Reposition cursor P1 so that  it points to the largest entry that
                      ** is less than the key value. If there are no records less than
                      ** the key and P2 is not zero, then jump to P2.
                      **
                      ** See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLe
                      *//* Opcode: SeekLe P1 P2 P3 P4 *
                      **
                      ** If cursor P1 refers to an SQL table (B-Tree that uses integer keys),
                      ** use the value in register P3 as a key. If cursor P1 refers
                      ** to an SQL index, then P3 is the first in an array of P4 registers
                      ** that are used as an unpacked index key.
                      **
                      ** Reposition cursor P1 so that it points to the largest entry that
                      ** is less than or equal to the key value. If there are no records
                      ** less than or equal to the key and P2 is not zero, then jump to P2.
                      **
                      ** See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLt
                      */case OpCode.OP_SeekLt:
						/* jump, in3 */case OpCode.OP_SeekLe:
						/* jump, in3 */case OpCode.OP_SeekGe:
						/* jump, in3 */case OpCode.OP_SeekGt: {
							/* jump, in3 */int res;
							int oc;
							VdbeCursor pC;
							UnpackedRecord r;
							int nField;
							i64 iKey;
							/* The rowid we are to seek to */res=0;
							r=new UnpackedRecord();
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pOp.p2!=0);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pseudoTableReg==0);
							Debug.Assert(OP_SeekLe==OP_SeekLt+1);
							Debug.Assert(OP_SeekGe==OP_SeekLt+2);
							Debug.Assert(OP_SeekGt==OP_SeekLt+3);
							Debug.Assert(pC.isOrdered);
							if(pC.pCursor!=null) {
								oc=pOp.opcode;
								pC.nullRow=false;
								if(pC.isTable) {
									/* The input value in P3 might be of any type: integer, real, string,
                              ** blob, or NULL.  But it needs to be an integer before we can do
                              ** the seek, so convert it. */pIn3=aMem[pOp.p3];
									applyNumericAffinity(pIn3);
									iKey=sqlite3VdbeIntValue(pIn3);
									pC.rowidIsValid=false;
									/* If the P3 value could not be converted into an integer without
                              ** loss of information, then special processing is required... */if((pIn3.flags&MEM_Int)==0) {
										if((pIn3.flags&MEM_Real)==0) {
											/* If the P3 value cannot be converted into any kind of a number,
                                  ** then the seek is not possible, so jump to P2 */opcodeIndex=pOp.p2-1;
											break;
										}
										/* If we reach this point, then the P3 value must be a floating
                                ** point number. */Debug.Assert((pIn3.flags&MEM_Real)!=0);
										if(iKey==IntegerExtensions.SMALLEST_INT64&&(pIn3.r<(double)iKey||pIn3.r>0)) {
											/* The P3 value is too large in magnitude to be expressed as an
                                  ** integer. */res=1;
											if(pIn3.r<0) {
												if(oc>=OP_SeekGe) {
													Debug.Assert(oc==OP_SeekGe||oc==OP_SeekGt);
													rc=sqlite3BtreeFirst(pC.pCursor,ref res);
													if(rc!=SQLITE_OK)
														goto abort_due_to_error;
												}
											}
											else {
												if(oc<=OP_SeekLe) {
													Debug.Assert(oc==OP_SeekLt||oc==OP_SeekLe);
													rc=sqlite3BtreeLast(pC.pCursor,ref res);
													if(rc!=SQLITE_OK)
														goto abort_due_to_error;
												}
											}
											if(res!=0) {
												opcodeIndex=pOp.p2-1;
											}
											break;
										}
										else
											if(oc==OP_SeekLt||oc==OP_SeekGe) {
												/* Use the ceiling() function to convert real.int */if(pIn3.r>(double)iKey)
													iKey++;
											}
											else {
												/* Use the floor() function to convert real.int */Debug.Assert(oc==OP_SeekLe||oc==OP_SeekGt);
												if(pIn3.r<(double)iKey)
													iKey--;
											}
									}
									rc=sqlite3BtreeMovetoUnpacked(pC.pCursor,null,iKey,0,ref res);
									if(rc!=SQLITE_OK) {
										goto abort_due_to_error;
									}
									if(res==0) {
										pC.rowidIsValid=true;
										pC.lastRowid=iKey;
									}
								}
								else {
									nField=pOp.p4.i;
									Debug.Assert(pOp.p4type==P4_INT32);
									Debug.Assert(nField>0);
									r.pKeyInfo=pC.pKeyInfo;
									r.nField=(u16)nField;
									/* The next line of code computes as follows, only faster:
                              **   if( oc==OP_SeekGt || oc==OP_SeekLe ){
                              **     r.flags = UNPACKED_INCRKEY;
                              **   }else{
                              **     r.flags = 0;
                              **   }
                              */r.flags=(u16)(UNPACKED_INCRKEY*(1&(oc-OP_SeekLt)));
									Debug.Assert(oc!=OP_SeekGt||r.flags==UNPACKED_INCRKEY);
									Debug.Assert(oc!=OP_SeekLe||r.flags==UNPACKED_INCRKEY);
									Debug.Assert(oc!=OP_SeekGe||r.flags==0);
									Debug.Assert(oc!=OP_SeekLt||r.flags==0);
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
									rc=sqlite3BtreeMovetoUnpacked(pC.pCursor,r,0,0,ref res);
									if(rc!=SQLITE_OK) {
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
								if(oc>=OP_SeekGe) {
									Debug.Assert(oc==OP_SeekGe||oc==OP_SeekGt);
									if(res<0||(res==0&&oc==OP_SeekGt)) {
										rc=sqlite3BtreeNext(pC.pCursor,ref res);
										if(rc!=SQLITE_OK)
											goto abort_due_to_error;
										pC.rowidIsValid=false;
									}
									else {
										res=0;
									}
								}
								else {
									Debug.Assert(oc==OP_SeekLt||oc==OP_SeekLe);
									if(res>0||(res==0&&oc==OP_SeekLt)) {
										rc=sqlite3BtreePrevious(pC.pCursor,ref res);
										if(rc!=SQLITE_OK)
											goto abort_due_to_error;
										pC.rowidIsValid=false;
									}
									else {
										/* res might be negative because the table is empty.  Check to
                                ** see if this is the case.
                                */res=sqlite3BtreeEof(pC.pCursor)?1:0;
									}
								}
								Debug.Assert(pOp.p2>0);
								if(res!=0) {
									opcodeIndex=pOp.p2-1;
								}
							}
							else {
								/* This happens when attempting to open the sqlite3_master table
                            ** for read access returns SQLITE_EMPTY. In this case always
                            ** take the jump (since there are no records in the table).
                            */opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: Seek P1 P2 * * *
                  **
                  ** P1 is an open table cursor and P2 is a rowid integer.  Arrange
                  ** for P1 to move so that it points to the rowid given by P2.
                  **
                  ** This is actually a deferred seek.  Nothing actually happens until
                  ** the cursor is used to read a record.  That way, if no reads
                  ** occur, no unnecessary I/O happens.
                  */case OpCode.OP_Seek: {
							/* in2 */VdbeCursor pC;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(ALWAYS(pC!=null));
							if(pC.pCursor!=null) {
								Debug.Assert(pC.isTable);
								pC.nullRow=false;
								pIn2=aMem[pOp.p2];
								pC.movetoTarget=sqlite3VdbeIntValue(pIn2);
								pC.rowidIsValid=false;
								pC.deferredMoveto=true;
							}
							break;
						}
						/* Opcode: Found P1 P2 P3 P4 *
                  **
                  ** If P4==0 then register P3 holds a blob constructed by MakeRecord.  If
                  ** P4>0 then register P3 is the first of P4 registers that form an unpacked
                  ** record.
                  **
                  ** Cursor P1 is on an index btree.  If the record identified by P3 and P4
                  ** is a prefix of any entry in P1 then a jump is made to P2 and
                  ** P1 is left pointing at the matching entry.
                  *//* Opcode: NotFound P1 P2 P3 P4 *
                      **
                      ** If P4==0 then register P3 holds a blob constructed by MakeRecord.  If
                      ** P4>0 then register P3 is the first of P4 registers that form an unpacked
                      ** record.
                      ** 
                      ** Cursor P1 is on an index btree.  If the record identified by P3 and P4
                      ** is not the prefix of any entry in P1 then a jump is made to P2.  If P1 
                      ** does contain an entry whose prefix matches the P3/P4 record then control
                      ** falls through to the next instruction and P1 is left pointing at the
                      ** matching entry.
                      **
                      ** See also: Found, NotExists, IsUnique
                      */case OpCode.OP_NotFound:
						/* jump, in3 */case OpCode.OP_Found: {
							/* jump, in3 */int alreadyExists;
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
							Debug.Assert(pOp.p4type==P4_INT32);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pIn3=aMem[pOp.p3];
							if(ALWAYS(pC.pCursor!=null)) {
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
									r.flags=UNPACKED_PREFIX_MATCH;
									pIdxKey=r;
								}
								else {
									Debug.Assert((pIn3.flags&MEM_Blob)!=0);
									Debug.Assert((pIn3.flags&MEM_Zero)==0);
									/* zeroblobs already expanded */pIdxKey=sqlite3VdbeRecordUnpack(pC.pKeyInfo,pIn3.n,pIn3.zBLOB,aTempRec,0);
									//sizeof( aTempRec ) );
									if(pIdxKey==null) {
										goto no_mem;
									}
									pIdxKey.flags|=UNPACKED_PREFIX_MATCH;
								}
								rc=sqlite3BtreeMovetoUnpacked(pC.pCursor,pIdxKey,0,0,ref res);
								if(pOp.p4.i==0) {
									sqlite3VdbeDeleteUnpackedRecord(pIdxKey);
								}
								if(rc!=SQLITE_OK) {
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
						/* Opcode: IsUnique P1 P2 P3 P4 *
                  **
                  ** Cursor P1 is open on an index b-tree - that is to say, a btree which
                  ** no data and where the key are records generated by OP_MakeRecord with
                  ** the list field being the integer ROWID of the entry that the index
                  ** entry refers to.
                  **
                  ** The P3 register contains an integer record number. Call this record
                  ** number R. Register P4 is the first in a set of N contiguous registers
                  ** that make up an unpacked index key that can be used with cursor P1.
                  ** The value of N can be inferred from the cursor. N includes the rowid
                  ** value appended to the end of the index record. This rowid value may
                  ** or may not be the same as R.
                  **
                  ** If any of the N registers beginning with register P4 contains a NULL
                  ** value, jump immediately to P2.
                  **
                  ** Otherwise, this instruction checks if cursor P1 contains an entry
                  ** where the first (N-1) fields match but the rowid value at the end
                  ** of the index entry is not R. If there is no such entry, control jumps
                  ** to instruction P2. Otherwise, the rowid of the conflicting index
                  ** entry is copied to register P3 and control falls through to the next
                  ** instruction.
                  **
                  ** See also: NotFound, NotExists, Found
                  */case OpCode.OP_IsUnique: {
							/* jump, in3 */u16 ii;
							VdbeCursor pCx=new VdbeCursor();
							BtCursor pCrsr;
							u16 nField;
							Mem[] aMx;
							UnpackedRecord r;
							/* B-Tree index search key */i64 R;
							/* Rowid stored in register P3 */r=new UnpackedRecord();
							pIn3=aMem[pOp.p3];
							//aMx = aMem[pOp->p4.i];
							/* Assert that the values of parameters P1 and P4 are in range. */Debug.Assert(pOp.p4type==P4_INT32);
							Debug.Assert(pOp.p4.i>0&&pOp.p4.i<=this.nMem);
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							/* Find the index cursor. */pCx=this.apCsr[pOp.p1];
							Debug.Assert(!pCx.deferredMoveto);
							pCx.seekResult=0;
							pCx.cacheStatus=CACHE_STALE;
							pCrsr=pCx.pCursor;
							/* If any of the values are NULL, take the jump. */nField=pCx.pKeyInfo.nField;
							aMx=new Mem[nField+1];
							for(ii=0;ii<nField;ii++) {
								aMx[ii]=aMem[pOp.p4.i+ii];
								if((aMx[ii].flags&MEM_Null)!=0) {
									opcodeIndex=pOp.p2-1;
									pCrsr=null;
									break;
								}
							}
							aMx[nField]=new Mem();
							//Debug.Assert( ( aMx[nField].flags & MEM_Null ) == 0 );
							if(pCrsr!=null) {
								/* Populate the index search key. */r.pKeyInfo=pCx.pKeyInfo;
								r.nField=(ushort)(nField+1);
								r.flags=UNPACKED_PREFIX_SEARCH;
								r.aMem=aMx;
								#if SQLITE_DEBUG
																																																																																																																																													                {
                  int i;
                  for ( i = 0; i < r.nField; i++ )
                    Debug.Assert( memIsValid( r.aMem[i] ) );
                }
#endif
								/* Extract the value of R from register P3. */sqlite3VdbeMemIntegerify(pIn3);
								R=pIn3.u.i;
								/* Search the B-Tree index. If no conflicting record is found, jump
                            ** to P2. Otherwise, copy the rowid of the conflicting record to
                            ** register P3 and fall through to the next instruction.  */rc=sqlite3BtreeMovetoUnpacked(pCrsr,r,0,0,ref pCx.seekResult);
								if((r.flags&UNPACKED_PREFIX_SEARCH)!=0||r.rowid==R) {
									opcodeIndex=pOp.p2-1;
								}
								else {
									pIn3.u.i=r.rowid;
								}
							}
							break;
						}
						/* Opcode: NotExists P1 P2 P3 * *
                  **
                  ** Use the content of register P3 as an integer key.  If a record
                  ** with that key does not exist in table of P1, then jump to P2.
                  ** If the record does exist, then fall through.  The cursor is left
                  ** pointing to the record if it exists.
                  **
                  ** The difference between this operation and NotFound is that this
                  ** operation assumes the key is an integer and that P1 is a table whereas
                  ** NotFound assumes key is a blob constructed from MakeRecord and
                  ** P1 is an index.
                  **
                  ** See also: Found, NotFound, IsUnique
                  */case OpCode.OP_NotExists: {
							/* jump, in3 */VdbeCursor pC;
							BtCursor pCrsr;
							int res;
							i64 iKey;
							pIn3=aMem[pOp.p3];
							Debug.Assert((pIn3.flags&MEM_Int)!=0);
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.isTable);
							Debug.Assert(pC.pseudoTableReg==0);
							pCrsr=pC.pCursor;
							if(pCrsr!=null) {
								res=0;
								iKey=pIn3.u.i;
								rc=sqlite3BtreeMovetoUnpacked(pCrsr,null,(long)iKey,0,ref res);
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
								/* This happens when an attempt to open a read cursor on the
                            ** sqlite_master table returns SQLITE_EMPTY.
                            */opcodeIndex=pOp.p2-1;
								Debug.Assert(!pC.rowidIsValid);
								pC.seekResult=0;
							}
							break;
						}
						/* Opcode: Sequence P1 P2 * * *
                  **
                  ** Find the next available sequence number for cursor P1.
                  ** Write the sequence number into register P2.
                  ** The sequence number on the cursor is incremented after this
                  ** instruction.
                  */case OpCode.OP_Sequence: {
							/* out2-prerelease */Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(this.apCsr[pOp.p1]!=null);
							pOut.u.i=(long)this.apCsr[pOp.p1].seqCount++;
							break;
						}
						/* Opcode: NewRowid P1 P2 P3 * *
                  **
                  ** Get a new integer record number (a.k.a "rowid") used as the key to a table.
                  ** The record number is not previously used as a key in the database
                  ** table that cursor P1 points to.  The new record number is written
                  ** written to register P2.
                  **
                  ** If P3>0 then P3 is a register in the root frame of this VDBE that holds 
                  ** the largest previously generated record number. No new record numbers are
                  ** allowed to be less than this value. When this value reaches its maximum, 
                  ** an SQLITE_FULL error is generated. The P3 register is updated with the '
                  ** generated record number. This P3 mechanism is used to help implement the
                  ** AUTOINCREMENT feature.
                  */case OpCode.OP_NewRowid: {
							/* out2-prerelease */i64 v;
							/* The new rowid */VdbeCursor pC;
							/* Cursor of table to get the new rowid */int res;
							/* Result of an sqlite3BtreeLast() */int cnt;
							/* Counter to limit the number of searches */Mem pMem;
							/* Register holding largest rowid for AUTOINCREMENT */VdbeFrame pFrame;
							/* Root frame of VDBE */v=0;
							res=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							if(NEVER(pC.pCursor==null)) {
								/* The zero initialization above is all that is needed */}
							else {
								/* The next rowid or record number (different terms for the same
                            ** thing) is obtained in a two-step algorithm.
                            **
                            ** First we attempt to find the largest existing rowid and add one
                            ** to that.  But if the largest existing rowid is already the maximum
                            ** positive integer, we have to fall through to the second
                            ** probabilistic algorithm
                            **
                            ** The second algorithm is to select a rowid at random and see if
                            ** it already exists in the table.  If it does not exist, we have
                            ** succeeded.  If the random rowid does exist, we select a new one
                            ** and try again, up to 100 times.
                            */Debug.Assert(pC.isTable);
								#if SQLITE_32BIT_ROWID
																																																																																																																																													const int MAX_ROWID = i32.MaxValue;//   define MAX_ROWID 0x7fffffff
#else
								/* Some compilers complain about constants of the form 0x7fffffffffffffff.
** Others complain about 0x7ffffffffffffffffLL.  The following macro seems
** to provide the constant while making all compilers happy.
*/const long MAX_ROWID=i64.MaxValue;
								// (i64)( (((u64)0x7fffffff)<<32) | (u64)0xffffffff )
								#endif
								if(!pC.useRandomRowid) {
									v=pC.pCursor.sqlite3BtreeGetCachedRowid();
									if(v==0) {
										rc=sqlite3BtreeLast(pC.pCursor,ref res);
										if(rc!=SQLITE_OK) {
											goto abort_due_to_error;
										}
										if(res!=0) {
											v=1;
											/* IMP: R-61914-48074 */}
										else {
											Debug.Assert(sqlite3BtreeCursorIsValid(pC.pCursor));
											rc=sqlite3BtreeKeySize(pC.pCursor,ref v);
											Debug.Assert(rc==SQLITE_OK);
											/* Cannot fail following BtreeLast() */if(v==MAX_ROWID) {
												pC.useRandomRowid=true;
											}
											else {
												v++;
												/* IMP: R-29538-34987 */}
										}
									}
									#if !SQLITE_OMIT_AUTOINCREMENT
									if(pOp.p3!=0) {
										/* Assert that P3 is a valid memory cell. */Debug.Assert(pOp.p3>0);
										if(this.pFrame!=null) {
											for(pFrame=this.pFrame;pFrame.pParent!=null;pFrame=pFrame.pParent)
												;
											/* Assert that P3 is a valid memory cell. */Debug.Assert(pOp.p3<=pFrame.nMem);
											pMem=pFrame.aMem[pOp.p3];
										}
										else {
											/* Assert that P3 is a valid memory cell. */Debug.Assert(pOp.p3<=this.nMem);
											pMem=aMem[pOp.p3];
											memAboutToChange(this,pMem);
										}
										Debug.Assert(pMem.memIsValid());
										REGISTER_TRACE(this,pOp.p3,pMem);
										sqlite3VdbeMemIntegerify(pMem);
										Debug.Assert((pMem.flags&MEM_Int)!=0);
										/* mem(P3) holds an integer */if(pMem.u.i==MAX_ROWID||pC.useRandomRowid) {
											rc=SQLITE_FULL;
											/* IMP: R-12275-61338 */goto abort_due_to_error;
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
									/* IMPLEMENTATION-OF: R-07677-41881 If the largest ROWID is equal to the
                              ** largest possible integer (9223372036854775807) then the database
                              ** engine starts picking positive candidate ROWIDs at random until
                              ** it finds one that is not previously used. */Debug.Assert(pOp.p3==0);
									/* We cannot be in random rowid mode if this is
            ** an AUTOINCREMENT table. *//* on the first attempt, simply do one more than previous */v=lastRowid;
									v&=(MAX_ROWID>>1);
									/* ensure doesn't go negative */v++;
									/* ensure non-zero */cnt=0;
									while(((rc=sqlite3BtreeMovetoUnpacked(pC.pCursor,null,v,0,ref res))==SQLITE_OK)&&(res==0)&&(++cnt<100)) {
										/* collision - try another random rowid */sqlite3_randomness(sizeof(i64),ref v);
										if(cnt<5) {
											/* try "small" random rowids for the initial attempts */v&=0xffffff;
										}
										else {
											v&=(MAX_ROWID>>1);
											/* ensure doesn't go negative */}
										v++;
										/* ensure non-zero */}
									if(rc==SQLITE_OK&&res==0) {
										rc=SQLITE_FULL;
										/* IMP: R-38219-53002 */goto abort_due_to_error;
									}
									Debug.Assert(v>0);
									/* EV: R-40812-03570 */}
								pC.rowidIsValid=false;
								pC.deferredMoveto=false;
								pC.cacheStatus=CACHE_STALE;
							}
							pOut.u.i=(long)v;
							break;
						}
						/* Opcode: Insert P1 P2 P3 P4 P5
                  **
                  ** Write an entry into the table of cursor P1.  A new entry is
                  ** created if it doesn't already exist or the data for an existing
                  ** entry is overwritten.  The data is the value MEM_Blob stored in register
                  ** number P2. The key is stored in register P3. The key must
                  ** be a MEM_Int.
                  **
                  ** If the OPFLAG_NCHANGE flag of P5 is set, then the row change count is
                  ** incremented (otherwise not).  If the OPFLAG_LASTROWID flag of P5 is set,
                  ** then rowid is stored for subsequent return by the
                  ** sqlite3_last_insert_rowid() function (otherwise it is unmodified).
                  **
                  ** If the OPFLAG_USESEEKRESULT flag of P5 is set and if the result of
                  ** the last seek operation (OP_NotExists) was a success, then this
                  ** operation will not attempt to find the appropriate row before doing
                  ** the insert but will instead overwrite the row that the cursor is
                  ** currently pointing to.  Presumably, the prior OP_NotExists opcode
                  ** has already positioned the cursor correctly.  This is an optimization
                  ** that boosts performance by avoiding redundant seeks.
                  **
                  ** If the OPFLAG_ISUPDATE flag is set, then this opcode is part of an
                  ** UPDATE operation.  Otherwise (if the flag is clear) then this opcode
                  ** is part of an INSERT operation.  The difference is only important to
                  ** the update hook.
                  **
                  ** Parameter P4 may point to a string containing the table-name, or
                  ** may be NULL. If it is not NULL, then the update-hook 
                  ** (sqlite3.xUpdateCallback) is invoked following a successful insert.
                  **
                  ** (WARNING/TODO: If P1 is a pseudo-cursor and P2 is dynamically
                  ** allocated, then ownership of P2 is transferred to the pseudo-cursor
                  ** and register P2 becomes ephemeral.  If the cursor is changed, the
                  ** value of register P2 will then change.  Make sure this does not
                  ** cause any problems.)
                  **
                  ** This instruction only works on tables.  The equivalent instruction
                  ** for indices is OP_IdxInsert.
                  *//* Opcode: InsertInt P1 P2 P3 P4 P5
                      **
                      ** This works exactly like OP_Insert except that the key is the
                      ** integer value P3, not the value of the integer stored in register P3.
                      */case OpCode.OP_Insert:
						case OpCode.OP_InsertInt: {
							Mem pData;
							/* MEM cell holding data for the record to be inserted */Mem pKey;
							/* MEM cell holding key  for the record */i64 iKey;
							/* The integer ROWID or key for the record to be inserted */VdbeCursor pC;
							/* Cursor to table into which insert is written */int nZero;
							/* Number of zero-bytes to append */int seekResult;
							/* Result of prior seek or 0 if no USESEEKRESULT flag */string zDb;
							/* database name - used by the update hook */string zTbl;
							/* Table name - used by the opdate hook */int op;
							/* Opcode for update hook: SQLITE_UPDATE or SQLITE_INSERT */pData=aMem[pOp.p2];
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
								Debug.Assert((pKey.flags&MEM_Int)!=0);
								Debug.Assert(pKey.memIsValid());
								REGISTER_TRACE(this,pOp.p3,pKey);
								iKey=pKey.u.i;
							}
							else {
								Debug.Assert(pOp.OpCode==OpCode.OP_InsertInt);
								iKey=pOp.p3;
							}
							if((pOp.p5&OPFLAG_NCHANGE)!=0)
								this.nChange++;
							if((pOp.p5&OPFLAG_LASTROWID)!=0)
								db.lastRowid=lastRowid=iKey;
							if((pData.flags&MEM_Null)!=0) {
								sqlite3_free(ref pData.zBLOB);
								pData.z=null;
								pData.n=0;
							}
							else {
								Debug.Assert((pData.flags&(MEM_Blob|MEM_Str))!=0);
							}
							seekResult=((pOp.p5&OPFLAG_USESEEKRESULT)!=0?pC.seekResult:0);
							if((pData.flags&MEM_Zero)!=0) {
								nZero=pData.u.nZero;
							}
							else {
								nZero=0;
							}
							rc=sqlite3BtreeInsert(pC.pCursor,null,iKey,pData.zBLOB,pData.n,nZero,(pOp.p5&OPFLAG_APPEND)!=0?1:0,seekResult);
							pC.rowidIsValid=false;
							pC.deferredMoveto=false;
							pC.cacheStatus=CACHE_STALE;
							/* Invoke the update-hook if required. */if(rc==SQLITE_OK&&db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								zDb=db.aDb[pC.iDb].zName;
								zTbl=pOp.p4.z;
								op=((pOp.p5&OPFLAG_ISUPDATE)!=0?SQLITE_UPDATE:SQLITE_INSERT);
								Debug.Assert(pC.isTable);
								db.xUpdateCallback(db.pUpdateArg,op,zDb,zTbl,iKey);
								Debug.Assert(pC.iDb>=0);
							}
							break;
						}
						/* Opcode: Delete P1 P2 * P4 *
                  **
                  ** Delete the record at which the P1 cursor is currently pointing.
                  **
                  ** The cursor will be left pointing at either the next or the previous
                  ** record in the table. If it is left pointing at the next record, then
                  ** the next Next instruction will be a no-op.  Hence it is OK to delete
                  ** a record from within an Next loop.
                  **
                  ** If the OPFLAG_NCHANGE flag of P2 is set, then the row change count is
                  ** incremented (otherwise not).
                  **
                  ** P1 must not be pseudo-table.  It has to be a real table with
                  ** multiple rows.
                  **
                  ** If P4 is not NULL, then it is the name of the table that P1 is
                  ** pointing to.  The update hook will be invoked, if it exists.
                  ** If P4 is not NULL then the P1 cursor must have been positioned
                  ** using OP_NotFound prior to invoking this opcode.
                  */case OpCode.OP_Delete: {
							i64 iKey;
							VdbeCursor pC;
							iKey=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pCursor!=null);
							/* Only valid for real tables, no pseudotables *//* If the update-hook will be invoked, set iKey to the rowid of the
** row being deleted.
*/if(db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								Debug.Assert(pC.isTable);
								Debug.Assert(pC.rowidIsValid);
								/* lastRowid set by previous OP_NotFound */iKey=pC.lastRowid;
							}
							/* The OP_Delete opcode always follows an OP_NotExists or OP_Last or
                          ** OP_Column on the same table without any intervening operations that
                          ** might move or invalidate the cursor.  Hence cursor pC is always pointing
                          ** to the row to be deleted and the sqlite3VdbeCursorMoveto() operation
                          ** below is always a no-op and cannot fail.  We will run it anyhow, though,
                          ** to guard against future changes to the code generator.
                          **/Debug.Assert(pC.deferredMoveto==false);
							rc=sqlite3VdbeCursorMoveto(pC);
							if(NEVER(rc!=SQLITE_OK))
								goto abort_due_to_error;
							pC.pCursor.sqlite3BtreeSetCachedRowid(0);
							rc=sqlite3BtreeDelete(pC.pCursor);
							pC.cacheStatus=CACHE_STALE;
							/* Invoke the update-hook if required. */if(rc==SQLITE_OK&&db.xUpdateCallback!=null&&pOp.p4.z!=null) {
								string zDb=db.aDb[pC.iDb].zName;
								string zTbl=pOp.p4.z;
								db.xUpdateCallback(db.pUpdateArg,SQLITE_DELETE,zDb,zTbl,iKey);
								Debug.Assert(pC.iDb>=0);
							}
							if((pOp.p2&OPFLAG_NCHANGE)!=0)
								this.nChange++;
							break;
						}
						/* Opcode: ResetCount P1 * *
                  **
                  ** The value of the change counter is copied to the database handle
                  ** change counter (returned by subsequent calls to sqlite3_changes()).
                  ** Then the VMs internal change counter resets to 0.
                  ** This is used by trigger programs.
                  */case OpCode.OP_ResetCount: {
							sqlite3VdbeSetChanges(db,this.nChange);
							this.nChange=0;
							break;
						}
						/* Opcode: RowData P1 P2 * * *
                  **
                  ** Write into register P2 the complete row data for cursor P1.
                  ** There is no interpretation of the data.
                  ** It is just copied onto the P2 register exactly as
                  ** it is found in the database file.
                  **
                  ** If the P1 cursor must be pointing to a valid row (not a NULL row)
                  ** of a real table, not a pseudo-table.
                  *//* Opcode: RowKey P1 P2 * * *
                      **
                      ** Write into register P2 the complete row key for cursor P1.
                      ** There is no interpretation of the data.
                      ** The key is copied onto the P3 register exactly as
                      ** it is found in the database file.
                      **
                      ** If the P1 cursor must be pointing to a valid row (not a NULL row)
                      ** of a real table, not a pseudo-table.
                      */case OpCode.OP_RowKey:
						case OpCode.OP_RowData: {
							VdbeCursor pC;
							BtCursor pCrsr;
							u32 n;
							i64 n64;
							n=0;
							n64=0;
							pOut=aMem[pOp.p2];
							memAboutToChange(this,pOut);
							/* Note that RowKey and RowData are really exactly the same instruction */Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC.isTable||pOp.OpCode==OpCode.OP_RowKey);
							Debug.Assert(pC.isIndex||pOp.OpCode==OpCode.OP_RowData);
							Debug.Assert(pC!=null);
							Debug.Assert(pC.nullRow==false);
							Debug.Assert(pC.pseudoTableReg==0);
							Debug.Assert(pC.pCursor!=null);
							pCrsr=pC.pCursor;
							Debug.Assert(sqlite3BtreeCursorIsValid(pCrsr));
							/* The OP_RowKey and OP_RowData opcodes always follow OP_NotExists or
                          ** OP_Rewind/Op_Next with no intervening instructions that might invalidate
                          ** the cursor.  Hence the following sqlite3VdbeCursorMoveto() call is always
                          ** a no-op and can never fail.  But we leave it in place as a safety.
                          */Debug.Assert(pC.deferredMoveto==false);
							rc=sqlite3VdbeCursorMoveto(pC);
							if(NEVER(rc!=SQLITE_OK))
								goto abort_due_to_error;
							if(pC.isIndex) {
								Debug.Assert(!pC.isTable);
								rc=sqlite3BtreeKeySize(pCrsr,ref n64);
								Debug.Assert(rc==SQLITE_OK);
								/* True because of CursorMoveto() call above */if(n64>db.aLimit[SQLITE_LIMIT_LENGTH]) {
									goto too_big;
								}
								n=(u32)n64;
							}
							else {
								rc=sqlite3BtreeDataSize(pCrsr,ref n);
								Debug.Assert(rc==SQLITE_OK);
								/* DataSize() cannot fail */if(n>(u32)db.aLimit[SQLITE_LIMIT_LENGTH]) {
									goto too_big;
								}
								if(sqlite3VdbeMemGrow(pOut,(int)n,0)!=0) {
									goto no_mem;
								}
							}
							pOut.n=(int)n;
							if(pC.isIndex) {
								pOut.zBLOB=sqlite3Malloc((int)n);
								rc=sqlite3BtreeKey(pCrsr,0,n,pOut.zBLOB);
							}
							else {
								pOut.zBLOB=sqlite3Malloc((int)pCrsr.info.nData);
								rc=sqlite3BtreeData(pCrsr,0,(u32)n,pOut.zBLOB);
							}
							pOut.MemSetTypeFlag(MEM_Blob);
							pOut.enc=SqliteEncoding.UTF8;
							/* In case the blob is ever cast to text */
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pOut );
#endif
							break;
						}
						/* Opcode: Rowid P1 P2 * * *
                  **
                  ** Store in register P2 an integer which is the key of the table entry that
                  ** P1 is currently point to.
                  **
                  ** P1 can be either an ordinary table or a virtual table.  There used to
                  ** be a separate OP_VRowid opcode for use with virtual tables, but this
                  ** one opcode now works for both table types.
                  */case OpCode.OP_Rowid: {
							/* out2-prerelease */VdbeCursor pC;
							i64 v;
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							v=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.pseudoTableReg==0);
							if(pC.nullRow) {
								pOut.flags=MEM_Null;
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
										rc=sqlite3VdbeCursorMoveto(pC);
										if(rc!=0)
											goto abort_due_to_error;
										if(pC.rowidIsValid) {
											v=pC.lastRowid;
										}
										else {
											rc=sqlite3BtreeKeySize(pC.pCursor,ref v);
											Debug.Assert(rc==SQLITE_OK);
											/* Always so because of CursorMoveto() above */}
									}
							pOut.u.i=(long)v;
							break;
						}
						/* Opcode: NullRow P1 * * * *
                  **
                  ** Move the cursor P1 to a null row.  Any OP_Column operations
                  ** that occur while the cursor is on the null row will always
                  ** write a NULL.
                  */case OpCode.OP_NullRow: {
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
						/* Opcode: Last P1 P2 * * *
                  **
                  ** The next use of the Rowid or Column or Next instruction for P1
                  ** will refer to the last entry in the database table or index.
                  ** If the table or index is empty and P2>0, then jump immediately to P2.
                  ** If P2 is 0 or if the table or index is not empty, fall through
                  ** to the following instruction.
                  */case OpCode.OP_Last: {
							/* jump */VdbeCursor pC;
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
								rc=sqlite3BtreeLast(pCrsr,ref res);
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
						/* Opcode: Sort P1 P2 * * *
                  **
                  ** This opcode does exactly the same thing as OP_Rewind except that
                  ** it increments an undocumented global variable used for testing.
                  **
                  ** Sorting is accomplished by writing records into a sorting index,
                  ** then rewinding that index and playing it back from beginning to
                  ** end.  We use the OP_Sort opcode instead of OP_Rewind to do the
                  ** rewinding so that the global variable will be incremented and
                  ** regression tests can determine whether or not the optimizer is
                  ** correctly optimizing out sorts.
                  */case OpCode.OP_Sort: {
							/* jump */
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
							/* Fall through into OP_Rewind */goto case OpCode.OP_Rewind;
						}
						/* Opcode: Rewind P1 P2 * * *
                  **
                  ** The next use of the Rowid or Column or Next instruction for P1
                  ** will refer to the first entry in the database table or index.
                  ** If the table or index is empty and P2>0, then jump immediately to P2.
                  ** If P2 is 0 or if the table or index is not empty, fall through
                  ** to the following instruction.
                  */case OpCode.OP_Rewind: {
							/* jump */VdbeCursor pC;
							BtCursor pCrsr;
							int res=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							res=1;
							if((pCrsr=pC.pCursor)!=null) {
								rc=sqlite3BtreeFirst(pCrsr,ref res);
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
						/* Opcode: Next P1 P2 * * P5
                  **
                  ** Advance cursor P1 so that it points to the next key/data pair in its
                  ** table or index.  If there are no more key/value pairs then fall through
                  ** to the following instruction.  But if the cursor advance was successful,
                  ** jump immediately to P2.
                  **
                  ** The P1 cursor must be for a real table, not a pseudo-table.
                  **
                  ** See also: Prev
                  *//* Opcode: Prev P1 P2 * * *
                      **
                      ** Back up cursor P1 so that it points to the previous key/data pair in its
                      ** table or index.  If there is no previous key/value pairs then fall through
                      ** to the following instruction.  But if the cursor backup was successful,
                      ** jump immediately to P2.
                      **
                      ** The P1 cursor must be for a real table, not a pseudo-table.
                      **
                      ** If P5 is positive and the jump is taken, then event counter
                      ** number P5-1 in the prepared statement is incremented.
                      **
                      */case OpCode.OP_Prev:
						/* jump */case OpCode.OP_Next: {
							/* jump */VdbeCursor pC;
							BtCursor pCrsr;
							int res;
							if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							Debug.Assert(pOp.p5<=ArraySize(this.aCounter));
							pC=this.apCsr[pOp.p1];
							if(pC==null) {
								break;
								/* See ticket #2273 */}
							pCrsr=pC.pCursor;
							if(pCrsr==null) {
								pC.nullRow=true;
								break;
							}
							res=1;
							Debug.Assert(!pC.deferredMoveto);
							rc=pOp.OpCode==OpCode.OP_Next?sqlite3BtreeNext(pCrsr,ref res):sqlite3BtreePrevious(pCrsr,ref res);
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
						/* Opcode: IdxInsert P1 P2 P3 * P5
                  **
                  ** Register P2 holds an SQL index key made using the
                  ** MakeRecord instructions.  This opcode writes that key
                  ** into the index P1.  Data for the entry is nil.
                  **
                  ** P3 is a flag that provides a hint to the b-tree layer that this
                  ** insert is likely to be an append.
                  **
                  ** This instruction only works for indices.  The equivalent instruction
                  ** for tables is OP_Insert.
                  */case OpCode.OP_IdxInsert: {
							/* in2 */VdbeCursor pC;
							BtCursor pCrsr;
							int nKey;
							byte[] zKey;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pIn2=aMem[pOp.p2];
							Debug.Assert((pIn2.flags&MEM_Blob)!=0);
							pCrsr=pC.pCursor;
							if(ALWAYS(pCrsr!=null)) {
								Debug.Assert(!pC.isTable);
								ExpandBlob(pIn2);
								if(rc==SQLITE_OK) {
									nKey=pIn2.n;
									zKey=(pIn2.flags&MEM_Blob)!=0?pIn2.zBLOB:Encoding.UTF8.GetBytes(pIn2.z);
									rc=sqlite3BtreeInsert(pCrsr,zKey,nKey,null,0,0,(pOp.p3!=0)?1:0,((pOp.p5&OPFLAG_USESEEKRESULT)!=0?pC.seekResult:0));
									Debug.Assert(!pC.deferredMoveto);
									pC.cacheStatus=CACHE_STALE;
								}
							}
							break;
						}
						/* Opcode: IdxDelete P1 P2 P3 * *
                  **
                  ** The content of P3 registers starting at register P2 form
                  ** an unpacked index key. This opcode removes that entry from the
                  ** index opened by cursor P1.
                  */case OpCode.OP_IdxDelete: {
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
							if(ALWAYS(pCrsr!=null)) {
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
								rc=sqlite3BtreeMovetoUnpacked(pCrsr,r,0,0,ref res);
								if(rc==SQLITE_OK&&res==0) {
									rc=sqlite3BtreeDelete(pCrsr);
								}
								Debug.Assert(!pC.deferredMoveto);
								pC.cacheStatus=CACHE_STALE;
							}
							break;
						}
						/* Opcode: IdxRowid P1 P2 * * *
                  **
                  ** Write into register P2 an integer which is the last entry in the record at
                  ** the end of the index key pointed to by cursor P1.  This integer should be
                  ** the rowid of the table entry to which this index entry points.
                  **
                  ** See also: Rowid, MakeRecord.
                  */case OpCode.OP_IdxRowid: {
							/* out2-prerelease */BtCursor pCrsr;
							VdbeCursor pC;
							i64 rowid;
							rowid=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							pCrsr=pC.pCursor;
							pOut.flags=MEM_Null;
							if(ALWAYS(pCrsr!=null)) {
								rc=sqlite3VdbeCursorMoveto(pC);
								if(NEVER(rc!=0))
									goto abort_due_to_error;
								Debug.Assert(!pC.deferredMoveto);
								Debug.Assert(!pC.isTable);
								if(!pC.nullRow) {
									rc=sqlite3VdbeIdxRowid(db,pCrsr,ref rowid);
									if(rc!=SQLITE_OK) {
										goto abort_due_to_error;
									}
									pOut.u.i=rowid;
									pOut.flags=MEM_Int;
								}
							}
							break;
						}
						/* Opcode: IdxGE P1 P2 P3 P4 P5
                  **
                  ** The P4 register values beginning with P3 form an unpacked index
                  ** key that omits the ROWID.  Compare this key value against the index
                  ** that P1 is currently pointing to, ignoring the ROWID on the P1 index.
                  **
                  ** If the P1 index entry is greater than or equal to the key value
                  ** then jump to P2.  Otherwise fall through to the next instruction.
                  **
                  ** If P5 is non-zero then the key value is increased by an epsilon
                  ** prior to the comparison.  This make the opcode work like IdxGT except
                  ** that if the key from register P3 is a prefix of the key in the cursor,
                  ** the result is false whereas it would be true with IdxGT.
                  *//* Opcode: IdxLT P1 P2 P3 P4 P5
                      **
                      ** The P4 register values beginning with P3 form an unpacked index
                      ** key that omits the ROWID.  Compare this key value against the index
                      ** that P1 is currently pointing to, ignoring the ROWID on the P1 index.
                      **
                      ** If the P1 index entry is less than the key value then jump to P2.
                      ** Otherwise fall through to the next instruction.
                      **
                      ** If P5 is non-zero then the key value is increased by an epsilon prior
                      ** to the comparison.  This makes the opcode work like IdxLE.
                      */case OpCode.OP_IdxLT:
						/* jump */case OpCode.OP_IdxGE: {
							/* jump */VdbeCursor pC;
							int res;
							UnpackedRecord r;
							res=0;
							r=new UnpackedRecord();
							Debug.Assert(pOp.p1>=0&&pOp.p1<this.nCursor);
							pC=this.apCsr[pOp.p1];
							Debug.Assert(pC!=null);
							Debug.Assert(pC.isOrdered);
							if(ALWAYS(pC.pCursor!=null)) {
								Debug.Assert(pC.deferredMoveto==false);
								Debug.Assert(pOp.p5==0||pOp.p5==1);
								Debug.Assert(pOp.p4type==P4_INT32);
								r.pKeyInfo=pC.pKeyInfo;
								r.nField=(u16)pOp.p4.i;
								if(pOp.p5!=0) {
									r.flags=UNPACKED_INCRKEY|UNPACKED_IGNORE_ROWID;
								}
								else {
									r.flags=UNPACKED_IGNORE_ROWID;
								}
								r.aMem=new Mem[r.nField];
								for(int rI=0;rI<r.nField;rI++) {
									r.aMem[rI]=aMem[pOp.p3+rI];
									// r.aMem = aMem[pOp.p3];
									#if SQLITE_DEBUG
																																																																																																																																																																				                  Debug.Assert( memIsValid( r.aMem[rI] ) );
#endif
								}
								rc=sqlite3VdbeIdxKeyCompare(pC,r,ref res);
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
						/* Opcode: Destroy P1 P2 P3 * *
                  **
                  ** Delete an entire database table or index whose root page in the database
                  ** file is given by P1.
                  **
                  ** The table being destroyed is in the main database file if P3==0.  If
                  ** P3==1 then the table to be clear is in the auxiliary database file
                  ** that is used to store tables create using CREATE TEMPORARY TABLE.
                  **
                  ** If AUTOVACUUM is enabled then it is possible that another root page
                  ** might be moved into the newly deleted root page in order to keep all
                  ** root pages contiguous at the beginning of the database.  The former
                  ** value of the root page that moved - its value before the move occurred -
                  ** is stored in register P2.  If no page
                  ** movement was required (because the table being dropped was already
                  ** the last one in the database) then a zero is stored in register P2.
                  ** If AUTOVACUUM is disabled then a zero is stored in register P2.
                  **
                  ** See also: Clear
                  */case OpCode.OP_Destroy: {
							/* out2-prerelease */int iMoved=0;
							int iCnt;
							Vdbe pVdbe;
							int iDb;
							#if !SQLITE_OMIT_VIRTUALTABLE
							iCnt=0;
							for(pVdbe=db.pVdbe;pVdbe!=null;pVdbe=pVdbe.pNext) {
								if(pVdbe.magic==VDBE_MAGIC_RUN&&pVdbe.inVtabMethod<2&&pVdbe.currentOpCodeIndex>=0) {
									iCnt++;
								}
							}
							#else
																																																																																																																						              iCnt = db.activeVdbeCnt;
#endif
							pOut.flags=MEM_Null;
							if(iCnt>1) {
								rc=SQLITE_LOCKED;
								this.errorAction=OE_Abort;
							}
							else {
								iDb=pOp.p3;
								Debug.Assert(iCnt==1);
								Debug.Assert((this.btreeMask&(((yDbMask)1)<<iDb))!=0);
								rc=db.aDb[iDb].pBt.sqlite3BtreeDropTable(pOp.p1,ref iMoved);
								pOut.flags=MEM_Int;
								pOut.u.i=iMoved;
								#if !SQLITE_OMIT_AUTOVACUUM
								if(rc==SQLITE_OK&&iMoved!=0) {
									sqlite3RootPageMoved(db,iDb,iMoved,pOp.p1);
									/* All OP_Destroy operations occur on the same btree */Debug.Assert(resetSchemaOnFault==0||resetSchemaOnFault==iDb+1);
									resetSchemaOnFault=(u8)(iDb+1);
								}
								#endif
							}
							break;
						}
						/* Opcode: Clear P1 P2 P3
                  **
                  ** Delete all contents of the database table or index whose root page
                  ** in the database file is given by P1.  But, unlike Destroy, do not
                  ** remove the table or index from the database file.
                  **
                  ** The table being clear is in the main database file if P2==0.  If
                  ** P2==1 then the table to be clear is in the auxiliary database file
                  ** that is used to store tables create using CREATE TEMPORARY TABLE.
                  **
                  ** If the P3 value is non-zero, then the table referred to must be an
                  ** intkey table (an SQL table, not an index). In this case the row change
                  ** count is incremented by the number of rows in the table being cleared.
                  ** If P3 is greater than zero, then the value stored in register P3 is
                  ** also incremented by the number of rows in the table being cleared.
                  **
                  ** See also: Destroy
                  */case OpCode.OP_Clear: {
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
						/* Opcode: CreateTable P1 P2 * * *
                  **
                  ** Allocate a new table in the main database file if P1==0 or in the
                  ** auxiliary database file if P1==1 or in an attached database if
                  ** P1>1.  Write the root page number of the new table into
                  ** register P2
                  **
                  ** The difference between a table and an index is this:  A table must
                  ** have a 4-byte integer key and can have arbitrary data.  An index
                  ** has an arbitrary key but no data.
                  **
                  ** See also: CreateIndex
                  *//* Opcode: CreateIndex P1 P2 * * *
                      **
                      ** Allocate a new index in the main database file if P1==0 or in the
                      ** auxiliary database file if P1==1 or in an attached database if
                      ** P1>1.  Write the root page number of the new table into
                      ** register P2.
                      **
                      ** See documentation on OP_CreateTable for additional information.
                      */case OpCode.OP_CreateIndex:
						/* out2-prerelease */case OpCode.OP_CreateTable: {
							/* out2-prerelease */int pgno;
							int flags;
							Db pDb;
							pgno=0;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pDb=db.aDb[pOp.p1];
							Debug.Assert(pDb.pBt!=null);
							if(pOp.OpCode==OpCode.OP_CreateTable) {
								/* flags = BTREE_INTKEY; */flags=BTREE_INTKEY;
							}
							else {
								flags=BTREE_BLOBKEY;
							}
							rc=sqlite3BtreeCreateTable(pDb.pBt,ref pgno,flags);
							pOut.u.i=pgno;
							break;
						}
						/* Opcode: ParseSchema P1 * * P4 *
                  **
                  ** Read and parse all entries from the SQLITE_MASTER table of database P1
                  ** that match the WHERE clause P4. 
                  **
                  ** This opcode invokes the parser to create a new virtual machine,
                  ** then runs the new virtual machine.  It is thus a re-entrant opcode.
                  */case OpCode.OP_ParseSchema: {
							int iDb;
							string zMaster;
							string zSql;
							InitData initData;
							/* Any prepared statement that invokes this opcode will hold mutexes
                          ** on every btree.  This is a prerequisite for invoking
                          ** sqlite3InitCallback().
                          */
							#if SQLITE_DEBUG
																																																																																																																						              for ( iDb = 0; iDb < db.nDb; iDb++ )
              {
                Debug.Assert( iDb == 1 || sqlite3BtreeHoldsMutex( db.aDb[iDb].pBt ) );
              }
#endif
							iDb=pOp.p1;
							Debug.Assert(iDb>=0&&iDb<db.nDb);
							Debug.Assert(DbHasProperty(db,iDb,DB_SchemaLoaded));
							/* Used to be a conditional */{
								zMaster=SCHEMA_TABLE(iDb);
								initData=new InitData();
								initData.db=db;
								initData.iDb=pOp.p1;
								initData.pzErrMsg=this.zErrMsg;
								zSql=sqlite3MPrintf(db,"SELECT name, rootpage, sql FROM '%q'.%s WHERE %s ORDER BY rowid",db.aDb[iDb].zName,zMaster,pOp.p4.z);
								if(String.IsNullOrEmpty(zSql)) {
									rc=SQLITE_NOMEM;
								}
								else {
									Debug.Assert(0==db.init.busy);
									db.init.busy=1;
									initData.rc=SQLITE_OK;
									//Debug.Assert( 0 == db.mallocFailed );
									rc=sqlite3_exec(db,zSql,(dxCallback)sqlite3InitCallback,(object)initData,0);
									if(rc==SQLITE_OK)
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
						/* Opcode: LoadAnalysis P1 * * * *
**
** Read the sqlite_stat1 table for database P1 and load the content
** of that table into the internal index hash table.  This will cause
** the analysis to be used when preparing all subsequent queries.
*/case OpCode.OP_LoadAnalysis: {
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							rc=sqlite3AnalysisLoad(db,pOp.p1);
							break;
						}
						#endif
						/* Opcode: DropTable P1 * * P4 *
**
** Remove the internal (in-memory) data structures that describe
** the table named P4 in database P1.  This is called after a table
** is dropped in order to keep the internal representation of the
** schema consistent with what is on disk.
*/case OpCode.OP_DropTable: {
							sqlite3UnlinkAndDeleteTable(db,pOp.p1,pOp.p4.z);
							break;
						}
						/* Opcode: DropIndex P1 * * P4 *
                  **
                  ** Remove the internal (in-memory) data structures that describe
                  ** the index named P4 in database P1.  This is called after an index
                  ** is dropped in order to keep the internal representation of the
                  ** schema consistent with what is on disk.
                  */case OpCode.OP_DropIndex: {
							sqlite3UnlinkAndDeleteIndex(db,pOp.p1,pOp.p4.z);
							break;
						}
						/* Opcode: DropTrigger P1 * * P4 *
                  **
                  ** Remove the internal (in-memory) data structures that describe
                  ** the trigger named P4 in database P1.  This is called after a trigger
                  ** is dropped in order to keep the internal representation of the
                  ** schema consistent with what is on disk.
                  */case OpCode.OP_DropTrigger: {
							sqlite3UnlinkAndDeleteTrigger(db,pOp.p1,pOp.p4.z);
							break;
						}
						#if !SQLITE_OMIT_INTEGRITY_CHECK
						/* Opcode: IntegrityCk P1 P2 P3 * P5
**
** Do an analysis of the currently open database.  Store in
** register P1 the text of an error message describing any problems.
** If no problems are found, store a NULL in register P1.
**
** The register P3 contains the maximum number of allowed errors.
** At most reg(P3) errors will be reported.
** In other words, the analysis stops as soon as reg(P1) errors are
** seen.  Reg(P1) is updated with the number of errors remaining.
**
** The root page numbers of all tables in the database are integer
** stored in reg(P1), reg(P1+1), reg(P1+2), ....  There are P2 tables
** total.
**
** If P5 is not zero, the check is done on the auxiliary database
** file, not the main database file.
**
** This opcode is used to implement the integrity_check pragma.
*/case OpCode.OP_IntegrityCk: {
							int nRoot;
							/* Number of tables to check.  (Number of root pages.) */int[] aRoot=null;
							/* Array of rootpage numbers for tables to be checked */int j;
							/* Loop counter */int nErr=0;
							/* Number of errors reported */string z;
							/* Text of the error report */Mem pnErr;
							/* Register keeping track of errors remaining */nRoot=pOp.p2;
							Debug.Assert(nRoot>0);
							aRoot=sqlite3Malloc(aRoot,(nRoot+1));
							// sqlite3DbMallocRaw(db, sizeof(int) * (nRoot + 1));
							if(aRoot==null)
								goto no_mem;
							Debug.Assert(pOp.p3>0&&pOp.p3<=this.nMem);
							pnErr=aMem[pOp.p3];
							Debug.Assert((pnErr.flags&MEM_Int)!=0);
							Debug.Assert((pnErr.flags&(MEM_Str|MEM_Blob))==0);
							pIn1=aMem[pOp.p1];
							for(j=0;j<nRoot;j++) {
								aRoot[j]=(int)sqlite3VdbeIntValue(this.aMem[pOp.p1+j]);
								// pIn1[j]);
							}
							aRoot[j]=0;
							Debug.Assert(pOp.p5<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p5))!=0);
							z=db.aDb[pOp.p5].pBt.sqlite3BtreeIntegrityCheck(aRoot,nRoot,(int)pnErr.u.i,ref nErr);
							db.sqlite3DbFree(ref aRoot);
							pnErr.u.i-=nErr;
							sqlite3VdbeMemSetNull(pIn1);
							if(nErr==0) {
								Debug.Assert(z=="");
							}
							else
								if(String.IsNullOrEmpty(z)) {
									goto no_mem;
								}
								else {
									sqlite3VdbeMemSetStr(pIn1,z,-1,SqliteEncoding.UTF8,null);
									//sqlite3_free );
								}
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
							sqlite3VdbeChangeEncoding(pIn1,encoding);
							break;
						}
						#endif
						/* Opcode: RowSetAdd P1 P2 * * *
**
** Insert the integer value held by register P2 into a boolean index
** held in register P1.
**
** An assertion fails if P2 is not an integer.
*/case OpCode.OP_RowSetAdd: {
							/* in1, in2 */pIn1=aMem[pOp.p1];
							pIn2=aMem[pOp.p2];
							Debug.Assert((pIn2.flags&MEM_Int)!=0);
							if((pIn1.flags&MEM_RowSet)==0) {
								sqlite3VdbeMemSetRowSet(pIn1);
								if((pIn1.flags&MEM_RowSet)==0)
									goto no_mem;
							}
							sqlite3RowSetInsert(pIn1.u.pRowSet,pIn2.u.i);
							break;
						}
						/* Opcode: RowSetRead P1 P2 P3 * *
                  **
                  ** Extract the smallest value from boolean index P1 and put that value into
                  ** register P3.  Or, if boolean index P1 is initially empty, leave P3
                  ** unchanged and jump to instruction P2.
                  */case OpCode.OP_RowSetRead: {
							/* jump, in1, ref3 */i64 val=0;
							if(db.u1.isInterrupted)
								goto abort_due_to_interrupt;
							//CHECK_FOR_INTERRUPT;
							pIn1=aMem[pOp.p1];
							if((pIn1.flags&MEM_RowSet)==0||sqlite3RowSetNext(pIn1.u.pRowSet,ref val)==0) {
								/* The boolean index is empty */sqlite3VdbeMemSetNull(pIn1);
								opcodeIndex=pOp.p2-1;
							}
							else {
								/* A value was pulled from the index */sqlite3VdbeMemSetInt64(aMem[pOp.p3],val);
							}
							break;
						}
						/* Opcode: RowSetTest P1 P2 P3 P4
                  **
                  ** Register P3 is assumed to hold a 64-bit integer value. If register P1
                  ** contains a RowSet object and that RowSet object contains
                  ** the value held in P3, jump to register P2. Otherwise, insert the
                  ** integer in P3 into the RowSet and continue on to the
                  ** next opcode.
                  **
                  ** The RowSet object is optimized for the case where successive sets
                  ** of integers, where each set contains no duplicates. Each set
                  ** of values is identified by a unique P4 value. The first set
                  ** must have P4==0, the final set P4=-1.  P4 must be either -1 or
                  ** non-negative.  For non-negative values of P4 only the lower 4
                  ** bits are significant.
                  **
                  ** This allows optimizations: (a) when P4==0 there is no need to test
                  ** the rowset object for P3, as it is guaranteed not to contain it,
                  ** (b) when P4==-1 there is no need to insert the value, as it will
                  ** never be tested for, and (c) when a value that is part of set X is
                  ** inserted, there is no need to search to see if the same value was
                  ** previously inserted as part of set X (only if it was previously
                  ** inserted as part of some other set).
                  */case OpCode.OP_RowSetTest: {
							/* jump, in1, in3 */int iSet;
							int exists;
							pIn1=aMem[pOp.p1];
							pIn3=aMem[pOp.p3];
							iSet=pOp.p4.i;
							Debug.Assert((pIn3.flags&MEM_Int)!=0);
							/* If there is anything other than a rowset object in memory cell P1,
                          ** delete it now and initialize P1 with an empty rowset
                          */if((pIn1.flags&MEM_RowSet)==0) {
								sqlite3VdbeMemSetRowSet(pIn1);
								if((pIn1.flags&MEM_RowSet)==0)
									goto no_mem;
							}
							Debug.Assert(pOp.p4type==P4_INT32);
							Debug.Assert(iSet==-1||iSet>=0);
							if(iSet!=0) {
								exists=sqlite3RowSetTest(pIn1.u.pRowSet,(u8)(iSet>=0?iSet&0xf:0xff),pIn3.u.i);
								if(exists!=0) {
									opcodeIndex=pOp.p2-1;
									break;
								}
							}
							if(iSet>=0) {
								sqlite3RowSetInsert(pIn1.u.pRowSet,pIn3.u.i);
							}
							break;
						}
						#if !SQLITE_OMIT_TRIGGER
						/* Opcode: Program P1 P2 P3 P4 *
**
** Execute the trigger program passed as P4 (type P4_SUBPROGRAM). 
**
** P1 contains the address of the memory cell that contains the first memory 
** cell in an array of values used as arguments to the sub-program. P2 
** contains the address to jump to if the sub-program throws an IGNORE 
** exception using the RAISE() function. Register P3 contains the address 
** of a memory cell in this (the parent) VM that is used to allocate the 
** memory required by the sub-vdbe at runtime.
**
** P4 is a pointer to the VM containing the trigger program.
*/case OpCode.OP_Program: {
							/* jump */int nMem;
							/* Number of memory registers for sub-program */int nByte;
							/* Bytes of runtime space required for sub-program */Mem pRt;
							/* Register to allocate runtime space */Mem pMem=null;
							/* Used to iterate through memory cells *///Mem pEnd;            /* Last memory cell in new array */
							VdbeFrame pFrame;
							/* New vdbe frame to execute in */SubProgram pProgram;
							/* Sub-program to execute */int t;
							/* Token identifying trigger */pProgram=pOp.p4.pProgram;
							pRt=aMem[pOp.p3];
							Debug.Assert(pRt.memIsValid());
							Debug.Assert(pProgram.nOp>0);
							/* If the p5 flag is clear, then recursive invocation of triggers is 
                          ** disabled for backwards compatibility (p5 is set if this sub-program
                          ** is really a trigger, not a foreign key action, and the flag set
                          ** and cleared by the "PRAGMA recursive_triggers" command is clear).
                          ** 
                          ** It is recursive invocation of triggers, at the SQL level, that is 
                          ** disabled. In some cases a single trigger may generate more than one 
                          ** SubProgram (if the trigger may be executed with more than one different 
                          ** ON CONFLICT algorithm). SubProgram structures associated with a
                          ** single trigger all have the same value for the SubProgram.token 
                          ** variable.  */if(pOp.p5!=0) {
								t=pProgram.token;
								for(pFrame=this.pFrame;pFrame!=null&&pFrame.token!=t;pFrame=pFrame.pParent)
									;
								if(pFrame!=null)
									break;
							}
							if(this.nFrame>=db.aLimit[SQLITE_LIMIT_TRIGGER_DEPTH]) {
								rc=SQLITE_ERROR;
								sqlite3SetString(ref this.zErrMsg,db,"too many levels of trigger recursion");
								break;
							}
							/* Register pRt is used to store the memory required to save the state
                          ** of the current program, and the memory required at runtime to execute
                          ** the trigger program. If this trigger has been fired before, then pRt 
                          ** is already allocated. Otherwise, it must be initialized.  */if((pRt.flags&MEM_Frame)==0) {
								/* SubProgram.nMem is set to the number of memory cells used by the 
                            ** program stored in SubProgram.aOp. As well as these, one memory
                            ** cell is required for each cursor used by the program. Set local
                            ** variable nMem (and later, VdbeFrame.nChildMem) to this value.
                            */nMem=pProgram.nMem+pProgram.nCsr;
								//nByte = ROUND8( sizeof( VdbeFrame ) )
								//+ nMem * sizeof( Mem )
								//+ pProgram.nCsr * sizeof( VdbeCursor* );
								pFrame=new VdbeFrame();
								// sqlite3DbMallocZero( db, nByte );
								//if ( !pFrame )
								//{
								//  goto no_mem;
								//}
								sqlite3VdbeMemRelease(pRt);
								pRt.flags=MEM_Frame;
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
									pMem=sqlite3Malloc(pMem);
									pMem.flags=MEM_Null;
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
						/* Opcode: Param P1 P2 * * *
                  **
                  ** This opcode is only ever present in sub-programs called via the 
                  ** OP_Program instruction. Copy a value currently stored in a memory 
                  ** cell of the calling (parent) frame to cell P2 in the current frames 
                  ** address space. This is used by trigger programs to access the new.* 
                  ** and old.* values.
                  **
                  ** The address of the cell in the parent frame is determined by adding
                  ** the value of the P1 argument to the value of the P1 argument to the
                  ** calling OP_Program instruction.
                  */case OpCode.OP_Param: {
							/* out2-prerelease */VdbeFrame pFrame;
							Mem pIn;
							pFrame=this.pFrame;
							pIn=pFrame.aMem[pOp.p1+pFrame.aOp[pFrame.currentOpCodeIndex].p1];
							sqlite3VdbeMemShallowCopy(pOut,pIn,MEM_Ephem);
							break;
						}
						#endif
						#if !SQLITE_OMIT_FOREIGN_KEY
						/* Opcode: FkCounter P1 P2 * * *
**
** Increment a "constraint counter" by P2 (P2 may be negative or positive).
** If P1 is non-zero, the database constraint counter is incremented 
** (deferred foreign key constraints). Otherwise, if P1 is zero, the 
** statement counter is incremented (immediate foreign key constraints).
*/case OpCode.OP_FkCounter: {
							if(pOp.p1!=0) {
								db.nDeferredCons+=pOp.p2;
							}
							else {
								this.nFkConstraint+=pOp.p2;
							}
							break;
						}
						/* Opcode: FkIfZero P1 P2 * * *
                  **
                  ** This opcode tests if a foreign key constraint-counter is currently zero.
                  ** If so, jump to instruction P2. Otherwise, fall through to the next 
                  ** instruction.
                  **
                  ** If P1 is non-zero, then the jump is taken if the database constraint-counter
                  ** is zero (the one that counts deferred constraint violations). If P1 is
                  ** zero, the jump is taken if the statement constraint-counter is zero
                  ** (immediate foreign key constraint violations).
                  */case OpCode.OP_FkIfZero: {
							/* jump */if(pOp.p1!=0) {
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
						/* Opcode: MemMax P1 P2 * * *
**
** P1 is a register in the root frame of this VM (the root frame is
** different from the current frame if this instruction is being executed
** within a sub-program). Set the value of register P1 to the maximum of 
** its current value and the value in register P2.
**
** This instruction throws an error if the memory cell is not initially
** an integer.
*/case OpCode.OP_MemMax: {
							/* in2 */Mem _pIn1;
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
							sqlite3VdbeMemIntegerify(_pIn1);
							pIn2=aMem[pOp.p2];
							sqlite3VdbeMemIntegerify(pIn2);
							if(_pIn1.u.i<pIn2.u.i) {
								_pIn1.u.i=pIn2.u.i;
							}
							break;
						}
						#endif
						/* Opcode: IfPos P1 P2 * * *
**
** If the value of register P1 is 1 or greater, jump to P2.
**
** It is illegal to use this instruction on a register that does
** not contain an integer.  An Debug.Assertion fault will result if you try.
*/case OpCode.OP_IfPos: {
							/* jump, in1 */pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Int)!=0);
							if(pIn1.u.i>0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: IfNeg P1 P2 * * *
                  **
                  ** If the value of register P1 is less than zero, jump to P2.
                  **
                  ** It is illegal to use this instruction on a register that does
                  ** not contain an integer.  An Debug.Assertion fault will result if you try.
                  */case OpCode.OP_IfNeg: {
							/* jump, in1 */pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Int)!=0);
							if(pIn1.u.i<0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: IfZero P1 P2 P3 * *
                  **
                  ** The register P1 must contain an integer.  Add literal P3 to the
                  ** value in register P1.  If the result is exactly 0, jump to P2. 
                  **
                  ** It is illegal to use this instruction on a register that does
                  ** not contain an integer.  An assertion fault will result if you try.
                  */case OpCode.OP_IfZero: {
							/* jump, in1 */pIn1=aMem[pOp.p1];
							Debug.Assert((pIn1.flags&MEM_Int)!=0);
							pIn1.u.i+=pOp.p3;
							if(pIn1.u.i==0) {
								opcodeIndex=pOp.p2-1;
							}
							break;
						}
						/* Opcode: AggStep * P2 P3 P4 P5
                  **
                  ** Execute the step function for an aggregate.  The
                  ** function has P5 arguments.   P4 is a pointer to the FuncDef
                  ** structure that specifies the function.  Use register
                  ** P3 as the accumulator.
                  **
                  ** The P5 arguments are taken from register P2 and its
                  ** successors.
                  */case OpCode.OP_AggStep: {
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
							ctx.s.flags=MEM_Null;
							ctx.s.z=null;
							//ctx.s.zMalloc = null;
							ctx.s.xDel=null;
							ctx.s.db=db;
							ctx.isError=0;
							ctx.pColl=null;
							if((ctx.pFunc.flags&SQLITE_FUNC_NEEDCOLL)!=0) {
								Debug.Assert(opcodeIndex>0);
								//pOp > p.aOp );
								Debug.Assert(this.lOp[opcodeIndex-1].p4type==P4_COLLSEQ);
								//pOp[-1].p4type == P4_COLLSEQ );
								Debug.Assert(this.lOp[opcodeIndex-1].OpCode==OpCode.OP_CollSeq);
								// pOp[-1].opcode == OP_CollSeq );
								ctx.pColl=this.lOp[opcodeIndex-1].p4.pColl;
								;
								// pOp[-1].p4.pColl;
							}
							ctx.pFunc.xStep(ctx,n,apVal);
							/* IMP: R-24505-23230 */if(ctx.isError!=0) {
								sqlite3SetString(ref this.zErrMsg,db,sqlite3_value_text(ctx.s));
								rc=ctx.isError;
							}
							sqlite3VdbeMemRelease(ctx.s);
							break;
						}
						/* Opcode: AggFinal P1 P2 * P4 *
                  **
                  ** Execute the finalizer function for an aggregate.  P1 is
                  ** the memory location that is the accumulator for the aggregate.
                  **
                  ** P2 is the number of arguments that the step function takes and
                  ** P4 is a pointer to the FuncDef for this function.  The P2
                  ** argument is not used by this opcode.  It is only there to disambiguate
                  ** functions that can take varying numbers of arguments.  The
                  ** P4 argument is only needed for the degenerate case where
                  ** the step function was not previously called.
                  */case OpCode.OP_AggFinal: {
							Mem pMem;
							Debug.Assert(pOp.p1>0&&pOp.p1<=this.nMem);
							pMem=aMem[pOp.p1];
							Debug.Assert((pMem.flags&~(MEM_Null|MEM_Agg))==0);
							rc=sqlite3VdbeMemFinalize(pMem,pOp.p4.pFunc);
							this.aMem[pOp.p1]=pMem;
							if(rc!=0) {
								sqlite3SetString(ref this.zErrMsg,db,sqlite3_value_text(pMem));
							}
							sqlite3VdbeChangeEncoding(pMem,encoding);
							#if SQLITE_TEST
																																																																																																																						              UPDATE_MAX_BLOBSIZE( pMem );
#endif
							if(sqlite3VdbeMemTooBig(pMem)) {
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
cDebug.Ase OP_Checkpoint: {
  aRes[0] = 0;
  aRes[1] = aRes[2] = -1;
  Debug.Assert( pOp.p2==SQLITE_CHECKPOINT_PDebug.AsSIVE
       || pOp.p2==SQLITE_CHECKPOINT_FULL
       || pOp.p2==SQLITE_CHECKPOINT_RESTART
  );
  rc = sqlite3Checkpoint(db, pOp.p1, pOp.p2, ref aRes[1], ref aRes[2]);
  if( rc==SQLITE_BUSY ){
    rc = SQLITE_OK;
    aRes[0] = 1;
  }
  for(i=0, pMem = aMem[pOp.p3]; i<3; i++, pMem++){
    sqlite3VdbeMemSetInt64(pMem, (i64)aRes[i]);
  }
  break;
};  
#endif
						#if !SQLITE_OMIT_PRAGMA
						/* Opcode: JournalMode P1 P2 P3 * P5
**
** Change the journal mode of database P1 to P3. P3 must be one of the
** PAGER_JOURNALMODE_XXX values. If changing between the various rollback
** modes (delete, truncate, persist, off and memory), this is a simple
** operation. No IO is required.
**
** If changing into or out of WAL mode the procedure is more complicated.
**
** Write a string containing the final journal-mode to register P2.
*/case OpCode.OP_JournalMode:
						{
							/* out2-prerelease */Btree pBt;
							/* Btree to change journal mode of */Pager pPager;
							/* Pager associated with pBt */int eNew;
							/* New journal mode */int eOld;
							/* The old journal mode */string zFilename;
							/* Name of database file for pPager */eNew=pOp.p3;
							Debug.Assert(eNew==PAGER_JOURNALMODE_DELETE||eNew==PAGER_JOURNALMODE_TRUNCATE||eNew==PAGER_JOURNALMODE_PERSIST||eNew==PAGER_JOURNALMODE_OFF||eNew==PAGER_JOURNALMODE_MEMORY||eNew==PAGER_JOURNALMODE_WAL||eNew==PAGER_JOURNALMODE_QUERY);
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							pBt=db.aDb[pOp.p1].pBt;
							pPager=sqlite3BtreePager(pBt);
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
rc = SQLITE_ERROR;
sqlite3SetString(&p.zErrMsg, db, 
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
if( rc==SQLITE_OK ){
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
if( rc==SQLITE_OK ){
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
							pOut.flags=MEM_Str|MEM_Static|MEM_Term;
							pOut.z=sqlite3JournalModename(eNew);
							pOut.n=StringExtensions.sqlite3Strlen30(pOut.z);
							pOut.enc=SqliteEncoding.UTF8;
							sqlite3VdbeChangeEncoding(pOut,encoding);
							break;
						}
						;
						#endif
						#if !SQLITE_OMIT_VACUUM && !SQLITE_OMIT_ATTACH
						/* Opcode: Vacuum * * * * *
**
** Vacuum the entire database.  This opcode will cause other virtual
** machines to be created and run.  It may not be called from within
** a transaction.
*/case OpCode.OP_Vacuum: {
							rc=sqlite3RunVacuum(ref this.zErrMsg,db);
							break;
						}
						#endif
						#if !SQLITE_OMIT_AUTOVACUUM
						/* Opcode: IncrVacuum P1 P2 * * *
**
** Perform a single step of the incremental vacuum procedure on
** the P1 database. If the vacuum has finished, jump to instruction
** P2. Otherwise, fall through to the next instruction.
*/case OpCode.OP_IncrVacuum: {
							/* jump */Btree pBt;
							Debug.Assert(pOp.p1>=0&&pOp.p1<db.nDb);
							Debug.Assert((this.btreeMask&(((yDbMask)1)<<pOp.p1))!=0);
							pBt=db.aDb[pOp.p1].pBt;
							rc=sqlite3BtreeIncrVacuum(pBt);
							if(rc==SQLITE_DONE) {
								opcodeIndex=pOp.p2-1;
								rc=SQLITE_OK;
							}
							break;
						}
						#endif
						/* Opcode: Expire P1 * * * *
**
** Cause precompiled statements to become expired. An expired statement
** fails with an error code of SQLITE_SCHEMA if it is ever executed
** (via sqlite3_step()).
**
** If P1 is 0, then all SQL statements become expired. If P1 is non-zero,
** then only the currently executing statement is affected.
*/case OpCode.OP_Expire: {
							if(pOp.p1==0) {
								sqlite3ExpirePreparedStatements(db);
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
case OP_TableLock:
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
sqlite3SetString( ref p.zErrMsg, db, "database table is locked: ", z );
}
}
break;
}
#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VBegin * * * P4 *
**
** P4 may be a pointer to an sqlite3_vtab structure. If so, call the
** xBegin method for that table.
**
** Also, whether or not P4 is set, check that this is not being called from
** within a callback to a virtual table xSync() method. If it is, the error
** code will be set to SQLITE_LOCKED.
*/case OpCode.OP_VBegin: {
							VTable pVTab;
							pVTab=pOp.p4.pVtab;
							rc=sqlite3VtabBegin(db,pVTab);
							if(pVTab!=null)
								importVtabErrMsg(this,pVTab.pVtab);
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VCreate P1 * * P4 *
**
** P4 is the name of a virtual table in database P1. Call the xCreate method
** for that table.
*/case OpCode.OP_VCreate: {
							rc=sqlite3VtabCallCreate(db,pOp.p1,pOp.p4.z,ref this.zErrMsg);
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VDestroy P1 * * P4 *
**
** P4 is the name of a virtual table in database P1.  Call the xDestroy method
** of that table.
*/case OpCode.OP_VDestroy: {
							this.inVtabMethod=2;
							rc=sqlite3VtabCallDestroy(db,pOp.p1,pOp.p4.z);
							this.inVtabMethod=0;
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VOpen P1 * * P4 *
**
** P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
** P1 is a cursor number.  This opcode opens a cursor to the virtual
** table and stores that cursor in P1.
*/case OpCode.OP_VOpen: {
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
							if(SQLITE_OK==rc) {
								/* Initialize sqlite3_vtab_cursor base class */pVtabCursor.pVtab=pVtab;
								/* Initialise vdbe cursor object */pCur=allocateCursor(this,pOp.p1,0,-1,0);
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
						/* Opcode: VFilter P1 P2 P3 P4 *
**
** P1 is a cursor opened using VOpen.  P2 is an address to jump to if
** the filtered result set is empty.
**
** P4 is either NULL or a string that was generated by the xBestIndex
** method of the module.  The interpretation of the P4 string is left
** to the module implementation.
**
** This opcode invokes the xFilter method on the virtual table specified
** by P1.  The integer query plan parameter to xFilter is stored in register
** P3. Register P3+1 stores the argc parameter to be passed to the
** xFilter method. Registers P3+2..P3+1+argc are the argc
** additional parameters which are passed to
** xFilter as argv. Register P3+2 becomes argv[0] when passed to xFilter.
**
** A jump is made to P2 if the result set after filtering would be empty.
*/case OpCode.OP_VFilter: {
							/* jump */int nArg;
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
							/* Grab the index number and argc parameters */Debug.Assert((pQuery.flags&MEM_Int)!=0&&pArgc.flags==MEM_Int);
							nArg=(int)pArgc.u.i;
							iQuery=(int)pQuery.u.i;
							/* Invoke the xFilter method */{
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
								if(rc==SQLITE_OK) {
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
						/* Opcode: VColumn P1 P2 P3 * *
**
** Store the value of the P2-th column of
** the row of the virtual-table that the
** P1 cursor is pointing to into register P3.
*/case OpCode.OP_VColumn: {
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
								sqlite3VdbeMemSetNull(pDest);
								break;
							}
							pVtab=pCur.pVtabCursor.pVtab;
							pModule=pVtab.pModule;
							Debug.Assert(pModule.xColumn!=null);
							sContext=new sqlite3_context();
							//memset( &sContext, 0, sizeof( sContext ) );
							/* The output cell may already have a buffer allocated. Move
                          ** the current contents to sContext.s so in case the user-function
                          ** can use the already allocated buffer instead of allocating a
                          ** new one.
                          */sqlite3VdbeMemMove(sContext.s,pDest);
							sContext.s.MemSetTypeFlag(MEM_Null);
							rc=pModule.xColumn(pCur.pVtabCursor,sContext,pOp.p2);
							importVtabErrMsg(this,pVtab);
							if(sContext.isError!=0) {
								rc=sContext.isError;
							}
							/* Copy the result of the function to the P3 register. We
                          ** do this regardless of whether or not an error occurred to ensure any
                          ** dynamic allocation in sContext.s (a Mem struct) is  released.
                          */sqlite3VdbeChangeEncoding(sContext.s,encoding);
							sqlite3VdbeMemMove(pDest,sContext.s);
							REGISTER_TRACE(this,pOp.p3,pDest);
							UPDATE_MAX_BLOBSIZE(pDest);
							if(sqlite3VdbeMemTooBig(pDest)) {
								goto too_big;
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VNext P1 P2 * * *
**
** Advance virtual table P1 to the next row in its result set and
** jump to instruction P2.  Or, if the virtual table has reached
** the end of its result set, then fall through to the next instruction.
*/case OpCode.OP_VNext: {
							/* jump */sqlite3_vtab pVtab;
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
							/* Invoke the xNext() method of the module. There is no way for the
                          ** underlying implementation to return an error if one occurs during
                          ** xNext(). Instead, if an error occurs, true is returned (indicating that
                          ** data is available) and the error code returned when xColumn or
                          ** some other method is next invoked on the save virtual table cursor.
                          */this.inVtabMethod=1;
							rc=pModule.xNext(pCur.pVtabCursor);
							this.inVtabMethod=0;
							importVtabErrMsg(this,pVtab);
							if(rc==SQLITE_OK) {
								res=pModule.xEof(pCur.pVtabCursor);
							}
							if(0==res) {
								/* If there is data, jump to P2 */opcodeIndex=pOp.p2-1;
							}
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VRename P1 * * P4 *
**
** P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
** This opcode invokes the corresponding xRename method. The value
** in register P1 is passed as the zName argument to the xRename method.
*/case OpCode.OP_VRename: {
							sqlite3_vtab pVtab;
							Mem pName;
							pVtab=pOp.p4.pVtab.pVtab;
							pName=aMem[pOp.p1];
							Debug.Assert(pVtab.pModule.xRename!=null);
							Debug.Assert(pName.memIsValid());
							REGISTER_TRACE(this,pOp.p1,pName);
							Debug.Assert((pName.flags&MEM_Str)!=0);
							rc=pVtab.pModule.xRename(pVtab,pName.z);
							importVtabErrMsg(this,pVtab);
							this.expired=false;
							break;
						}
						#endif
						#if !SQLITE_OMIT_VIRTUALTABLE
						/* Opcode: VUpdate P1 P2 P3 P4 *
**
** P4 is a pointer to a virtual table object, an sqlite3_vtab structure.
** This opcode invokes the corresponding xUpdate method. P2 values
** are contiguous memory cells starting at P3 to pass to the xUpdate
** invocation. The value in register (P3+P2-1) corresponds to the
** p2th element of the argv array passed to xUpdate.
**
** The xUpdate method will do a DELETE or an INSERT or both.
** The argv[0] element (which corresponds to memory cell P3)
** is the rowid of a row to delete.  If argv[0] is NULL then no
** deletion occurs.  The argv[1] element is the rowid of the new
** row.  This can be NULL to have the virtual table select the new
** rowid for itself.  The subsequent elements in the array are
** the values of columns in the new row.
**
** If P2==1 then no insert is performed.  argv[0] is the rowid of
** a row to delete.
**
** P1 is a boolean flag. If it is set to true and the xUpdate call
** is successful, then the value returned by sqlite3_last_insert_rowid()
** is set to the value of the rowid for the row just inserted.
*/case OpCode.OP_VUpdate: {
							sqlite3_vtab pVtab;
							sqlite3_module pModule;
							int nArg;
							int i;
							sqlite_int64 rowid=0;
							Mem[] apArg;
							Mem pX;
							Debug.Assert(pOp.p2==1||pOp.p5==OE_Fail||pOp.p5==OE_Rollback||pOp.p5==OE_Abort||pOp.p5==OE_Ignore||pOp.p5==OE_Replace);
							pVtab=pOp.p4.pVtab.pVtab;
							pModule=(sqlite3_module)pVtab.pModule;
							nArg=pOp.p2;
							Debug.Assert(pOp.p4type==P4_VTAB);
							if(ALWAYS(pModule.xUpdate)) {
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
								if(rc==SQLITE_OK&&pOp.p1!=0) {
									Debug.Assert(nArg>1&&apArg[0]!=null&&(apArg[0].flags&MEM_Null)!=0);
									db.lastRowid=lastRowid=rowid;
								}
								if(rc==SQLITE_CONSTRAINT&&pOp.p4.pVtab.bConstraint!=0) {
									if(pOp.p5==OE_Ignore) {
										rc=SQLITE_OK;
									}
									else {
										this.errorAction=(byte)((pOp.p5==OE_Replace)?OE_Abort:pOp.p5);
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
						/* Opcode: Pagecount P1 P2 * * *
**
** Write the current number of pages in database P1 to memory cell P2.
*/case OpCode.OP_Pagecount: {
							/* out2-prerelease */pOut.u.i=sqlite3BtreeLastPage(db.aDb[pOp.p1].pBt);
							break;
						}
						#endif
						#if !SQLITE_OMIT_PAGER_PRAGMAS
						/* Opcode: MaxPgcnt P1 P2 P3 * *
**
** Try to set the maximum page count for database P1 to the value in P3.
** Do not let the maximum page count fall below the current page count and
** do not change the maximum page count value if P3==0.
**
** Store the maximum page count after the change in register P2.
*/case OpCode.OP_MaxPgcnt: {
							/* out2-prerelease */i64 newMax;
							Btree pBt;
							pBt=db.aDb[pOp.p1].pBt;
							newMax=0;
							if(pOp.p3!=0) {
								newMax=sqlite3BtreeLastPage(pBt);
								if(newMax<pOp.p3)
									newMax=pOp.p3;
							}
							pOut.u.i=(i64)sqlite3BtreeMaxPageCount(pBt,(int)newMax);
							break;
						}
						#endif
						#if !SQLITE_OMIT_TRACE
						/* Opcode: Trace * * * P4 *
**
** If tracing is enabled (by the sqlite3_trace()) interface, then
** the UTF-8 string contained in P4 is emitted on the trace callback.
*/case OpCode.OP_Trace: {
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
						/* Opcode: Noop * * * * *
**
** Do nothing.  This instruction is often useful as a jump
** destination.
*//*
                                ** The magic Explain opcode are only inserted when explain==2 (which
                                ** is to say when the EXPLAIN QUERY PLAN syntax is used.)
                                ** This opcode records information from the optimizer.  It is the
                                ** the same as a no-op.  This opcodesnever appears in a real VM program.
                                */default: {
							/* This is really OP_Noop and OP_Explain */Debug.Assert(pOp.OpCode==OpCode.OP_Noop||pOp.OpCode==OpCode.OP_Explain);
							break;
						}
						/*****************************************************************************
                  ** The cases of the switch statement above this line should all be indented
                  ** by 6 spaces.  But the left-most 6 spaces have been removed to improve the
                  ** readability.  From this point on down, the normal indentation rules are
                  ** restored.
                  *****************************************************************************/}
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
						/* The following code adds nothing to the actual functionality
** of the program.  It is only here for testing and debugging.
** On the other hand, it does burn CPU cycles every time through
** the evaluator loop.  So we can leave it out when NDEBUG is defined.
*/
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
					/* The end of the for(;;) loop the loops through opcodes *//* If we reach this point, it means that execution is finished with
    ** an error of some kind.
    */vdbe_error_halt:
					Debug.Assert(rc!=0);
					this.rc=rc;
					testcase(sqlite3GlobalConfig.xLog!=null);
					sqlite3_log(rc,"statement aborts at %d: [%s] %s",opcodeIndex,this.zSql,this.zErrMsg);
					this.sqlite3VdbeHalt();
					//if ( rc == SQLITE_IOERR_NOMEM ) db.mallocFailed = 1;
					rc=SQLITE_ERROR;
					if(resetSchemaOnFault>0) {
						sqlite3ResetInternalSchema(db,resetSchemaOnFault-1);
					}
					/* This is the only way out of this procedure.  We have to
** release the mutexes on btrees that were acquired at the
** top. */vdbe_return:
					db.lastRowid=lastRowid;
					this.sqlite3VdbeLeave();
					return rc;
					/* Jump to here if a string or blob larger than db.aLimit[SQLITE_LIMIT_LENGTH]
        ** is encountered.
        */too_big:
					sqlite3SetString(ref this.zErrMsg,db,"string or blob too big");
					rc=SQLITE_TOOBIG;
					goto vdbe_error_halt;
					/* Jump to here if a malloc() fails.
        */no_mem:
					//db.mallocFailed = 1;
					sqlite3SetString(ref this.zErrMsg,db,"out of memory");
					rc=SQLITE_NOMEM;
					goto vdbe_error_halt;
					/* Jump to here for any other kind of fatal error.  The "rc" variable
        ** should hold the error number.
        */abort_due_to_error:
					//Debug.Assert( p.zErrMsg); /// Not needed in C#
					//if ( db.mallocFailed != 0 ) rc = SQLITE_NOMEM;
					if(rc!=SQLITE_IOERR_NOMEM) {
						sqlite3SetString(ref this.zErrMsg,db,"%s",sqlite3ErrStr(rc));
					}
					goto vdbe_error_halt;
					/* Jump to here if the sqlite3_interrupt() API sets the interrupt
        ** flag.
        */abort_due_to_interrupt:
					Debug.Assert(db.u1.isInterrupted);
					rc=SQLITE_INTERRUPT;
					this.rc=rc;
					sqlite3SetString(ref this.zErrMsg,db,sqlite3ErrStr(rc));
					goto vdbe_error_halt;
				}
				finally {
					Log.Unindent();
				}
			}
		}
	#endif
	}
}
