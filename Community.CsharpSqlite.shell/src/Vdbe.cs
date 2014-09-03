using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
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
using yDbMask = System.Int32;
#endif
namespace Community.CsharpSqlite
{
    using Op = VdbeOp;
    using System.Text;
    using sqlite3_value = Sqlite3.Mem;
    using System.Collections.Generic;
    public partial class Sqlite3
    {
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
    */
        public class Vdbe
        {
            public Vdbe()
            {
            }
            public sqlite3 db;
            /* The database connection that owns this statement */
            /** Space to hold the virtual machine's program */
            public Op[] aOp;
            public List<Op> lOp = new List<Op>();
            /* The memory locations */
            public Mem[] aMem;
            public Mem[] apArg;
            /* Arguments to currently executing user function */
            public Mem[] aColName;
            /* Column names to return */
            public Mem[] pResultSet;
            /* Pointer to an array of results */
            public int nMem;
            /* Number of memory locations currently allocated */
            public int nOp;
            /* Number of instructions in the program */
            public int nOpAlloc;
            /* Number of slots allocated for aOp[] */
            public int nLabel;
            /* Number of labels used */
            public int nLabelAlloc;
            /* Number of slots allocated in aLabel[] */
            public int[] aLabel;
            /* Space to hold the labels */
            public u16 nResColumn;
            /* Number of columns in one row of the result set */
            public u16 nCursor;
            /* Number of slots in apCsr[] */
            public u32 magic;
            /* Magic number for sanity checking */
            public string zErrMsg;
            /* Error message written here */
            public Vdbe pPrev;
            /* Linked list of VDBEs with the same Vdbe.db */
            public Vdbe pNext;
            /* Linked list of VDBEs with the same Vdbe.db */
            public VdbeCursor[] apCsr;
            /* One element of this array for each open cursor */
            public Mem[] aVar;
            /* Values for the OP_Variable opcode. */
            public string[] azVar;
            /* Name of variables */
            public ynVar nVar;
            /* Number of entries in aVar[] */
            public ynVar nzVar;
            /* Number of entries in azVar[] */
            public u32 cacheCtr;
            /* VdbeCursor row cache generation counter */
            /// <summary>
            /// The program counter 
            /// </summary>
            public int currentOpCodeIndex;
            /// <summary>
            /// Value to return
            /// </summary>
            public int rc;
            public u8 errorAction;
            /* Recovery action to do in case of an error */
            public int explain;
            /* True if EXPLAIN present on SQL command */
            public bool changeCntOn;
            /* True to update the change-counter */
            public bool expired;
            /* True if the VM needs to be recompiled */
            public u8 runOnlyOnce;
            /* Automatically expire on reset */
            public int minWriteFileFormat;
            /* Minimum file format for writable database files */
            public int inVtabMethod;
            /* See comments above */
            public bool usesStmtJournal;
            /* True if uses a statement journal */
            public bool readOnly;
            /* True for read-only statements */
            public int nChange;
            /* Number of db changes made since last reset */
            public bool isPrepareV2;
            /* True if prepared with prepare_v2() */
            public yDbMask btreeMask;
            /* Bitmask of db.aDb[] entries referenced */
            public yDbMask lockMask;
            /* Subset of btreeMask that requires a lock */
            public int iStatement;
            /* Statement number (or 0 if has not opened stmt) */
            public int[] aCounter = new int[3];
            /* Counters used by sqlite3_stmt_status() */
#if !SQLITE_OMIT_TRACE
            public i64 startTime;
            /* Time when query started - used for profiling */
#endif
            public i64 nFkConstraint;
            /* Number of imm. FK constraints this VM */
            public i64 nStmtDefCons;
            /* Number of def. constraints when stmt started */
            public string zSql = "";
            /* Text of the SQL statement that generated this */
            public object pFree;
            /* Free this when deleting the vdbe */
#if SQLITE_DEBUG
																																																																					      public FILE trace;             /* Write an execution trace here, if not NULL */
#endif
            public VdbeFrame pFrame;
            /* Parent frame */
            public VdbeFrame pDelFrame;
            /* List of frame objects to free on VM reset */
            public int nFrame;
            /* Number of frames in pFrame list */
            public u32 expmask;
            ///<summary>
            ///Binding to these vars invalidates VM
            ///</summary>
            public SubProgram pProgram;
            ///<summary>
            ///Linked list of all sub-programs used by VM
            ///</summary>
            public Vdbe Copy()
            {
                Vdbe cp = (Vdbe)MemberwiseClone();
                return cp;
            }
            public void CopyTo(Vdbe ct)
            {
                ct.db = db;
                ct.pPrev = pPrev;
                ct.pNext = pNext;
                ct.nOp = nOp;
                ct.nOpAlloc = nOpAlloc;
                ct.lOp = lOp;
                ct.nLabel = nLabel;
                ct.nLabelAlloc = nLabelAlloc;
                ct.aLabel = aLabel;
                ct.apArg = apArg;
                ct.aColName = aColName;
                ct.nCursor = nCursor;
                ct.apCsr = apCsr;
                ct.aVar = aVar;
                ct.azVar = azVar;
                ct.nVar = nVar;
                ct.nzVar = nzVar;
                ct.magic = magic;
                ct.nMem = nMem;
                ct.aMem = aMem;
                ct.cacheCtr = cacheCtr;
                ct.currentOpCodeIndex = currentOpCodeIndex;
                ct.rc = rc;
                ct.errorAction = errorAction;
                ct.nResColumn = nResColumn;
                ct.zErrMsg = zErrMsg;
                ct.pResultSet = pResultSet;
                ct.explain = explain;
                ct.changeCntOn = changeCntOn;
                ct.expired = expired;
                ct.minWriteFileFormat = minWriteFileFormat;
                ct.inVtabMethod = inVtabMethod;
                ct.usesStmtJournal = usesStmtJournal;
                ct.readOnly = readOnly;
                ct.nChange = nChange;
                ct.isPrepareV2 = isPrepareV2;
#if !SQLITE_OMIT_TRACE
                ct.startTime = startTime;
#endif
                ct.btreeMask = btreeMask;
                ct.lockMask = lockMask;
                aCounter.CopyTo(ct.aCounter, 0);
                ct.zSql = zSql;
                ct.pFree = pFree;
#if SQLITE_DEBUG
																																																																																												        ct.trace = trace;
#endif
                ct.nFkConstraint = nFkConstraint;
                ct.nStmtDefCons = nStmtDefCons;
                ct.iStatement = iStatement;
                ct.pFrame = pFrame;
                ct.nFrame = nFrame;
                ct.expmask = expmask;
                ct.pProgram = pProgram;
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
            void sqlite3VdbeEnter()
            {
            }
            public void sqlite3VdbeLeave()
            {
            }
            public int sqlite3VdbeAddOp3(int op, int p1, int p2, int p3)
            {
                return sqlite3VdbeAddOp3((OpCode)op, p1, p2, p3);
            }
            public int sqlite3VdbeAddOp3(OpCode op, int p1, int p2, int p3)
            {
                int i;
                VdbeOp pOp;
                i = this.nOp;
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                //Debug.Assert(op>0&&op<0xff);
                if (this.nOpAlloc <= i)
                {
                    if (this.growOpArray() != 0)
                    {
                        return 1;
                    }
                }
                this.nOp++;
                pOp = new VdbeOp();
                pOp.opcode = (u8)op;
                pOp.p5 = 0;
                pOp.p1 = p1;
                pOp.p2 = p2;
                pOp.p3 = p3;
                pOp.p4.p = null;
                pOp.p4type = P4_NOTUSED;
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
            public int sqlite3VdbeAddOp0(int op)
            {
                return this.sqlite3VdbeAddOp3(op, 0, 0, 0);
            }
            public int sqlite3VdbeAddOp1(int op, int p1)
            {
                return this.sqlite3VdbeAddOp3(op, p1, 0, 0);
            }
            public int sqlite3VdbeAddOp2(int op, int p1, bool b2)
            {
                return this.sqlite3VdbeAddOp2(op, p1, (int)(b2 ? 1 : 0));
            }
            public int sqlite3VdbeAddOp2(int op, int p1, int p2)
            {
                return this.sqlite3VdbeAddOp3(op, p1, p2, 0);
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, i32 pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, char pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.z = pP4.ToString();
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, StringBuilder pP4, int p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4.ToString();
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, string pP4, int p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, byte[] pP4, int p4type)
            {
                Debug.Assert(op == OP_Null || pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.z = Encoding.UTF8.GetString(pP4, 0, pP4.Length);
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, int[] pP4, int p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.ai = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, i64 pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pI64 = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, double pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pReal = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, FuncDef pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pFunc = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, CollSeq pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, KeyInfo pP4, int p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, VTable pP4, int p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.pVtab = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4Int(/* Add the opcode to this VM */int op,/* The new opcode */int p1,/* The P1 operand */int p2,/* The P2 operand */int p3,/* The P3 operand */int p4/* The P4 operand as an integer */)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = p4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, P4_INT32);
                return addr;
            }
            public int sqlite3VdbeMakeLabel()
            {
                int i;
                i = this.nLabel++;
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                if (i >= this.nLabelAlloc)
                {
                    int n = this.nLabelAlloc == 0 ? 15 : this.nLabelAlloc * 2 + 5;
                    if (this.aLabel == null)
                        this.aLabel = sqlite3Malloc(this.aLabel, n);
                    else
                        Array.Resize(ref this.aLabel, n);
                    //p.aLabel = sqlite3DbReallocOrFree(p.db, p.aLabel,
                    //                                       n*sizeof(p.aLabel[0]));
                    this.nLabelAlloc = this.aLabel.Length;
                    //sqlite3DbMallocSize(p.db, p.aLabel)/sizeof(p.aLabel[0]);
                }
                if (this.aLabel != null)
                {
                    this.aLabel[i] = -1;
                }
                return -1 - i;
            }
            public void sqlite3VdbeResolveLabel(int x)
            {
                int j = -1 - x;
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                Debug.Assert(j >= 0 && j < this.nLabel);
                if (this.aLabel != null)
                {
                    this.aLabel[j] = this.nOp;
                }
            }
            public void sqlite3VdbeRunOnlyOnce()
            {
                this.runOnlyOnce = 1;
            }
            public void resolveP2Values(ref int pMaxFuncArgs)
            {
                int i;
                int nMaxArgs = pMaxFuncArgs;
                Op pOp;
                int[] aLabel = this.aLabel;
                this.readOnly = true;
                for (i = 0; i < this.nOp; i++)//  for(pOp=p->aOp, i=p->nOp-1; i>=0; i--, pOp++)
                {
                    pOp = this.lOp[i];
                    u8 opcode = pOp.opcode;
                    pOp.opflags = (u8)sqlite3OpcodeProperty[opcode];
                    if (opcode == OP_Function || opcode == OP_AggStep)
                    {
                        if (pOp.p5 > nMaxArgs)
                            nMaxArgs = pOp.p5;
                    }
                    else
                        if ((opcode == OP_Transaction && pOp.p2 != 0) || opcode == OP_Vacuum)
                        {
                            this.readOnly = false;
#if !SQLITE_OMIT_VIRTUALTABLE
                        }
                        else
                            if (opcode == OP_VUpdate)
                            {
                                if (pOp.p2 > nMaxArgs)
                                    nMaxArgs = pOp.p2;
                            }
                            else
                                if (opcode == OP_VFilter)
                                {
                                    int n;
                                    Debug.Assert(this.nOp - i >= 3);
                                    Debug.Assert(this.lOp[i - 1].opcode == OP_Integer);
                                    //pOp[-1].opcode==OP_Integer );
                                    n = this.lOp[i - 1].p1;
                                    //pOp[-1].p1;
                                    if (n > nMaxArgs)
                                        nMaxArgs = n;
#endif
                                }
                    if ((pOp.opflags & OPFLG_JUMP) != 0 && pOp.p2 < 0)
                    {
                        Debug.Assert(-1 - pOp.p2 < this.nLabel);
                        pOp.p2 = aLabel[-1 - pOp.p2];
                    }
                }
                this.db.sqlite3DbFree(ref this.aLabel);
                pMaxFuncArgs = nMaxArgs;
            }
            public int sqlite3VdbeCurrentAddr()
            {
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                return this.nOp;
            }
            public VdbeOp[] sqlite3VdbeTakeOpArray(ref int pnOp, ref int pnMaxArg)
            {
                List<VdbeOp> lOp = this.lOp;
                Debug.Assert(aOp != null);
                // && 0==p.db.mallocFailed );
                /* Check that sqlite3VdbeUsesBtree() was not called on this VM */
                Debug.Assert(this.btreeMask == 0);
                this.resolveP2Values(ref pnMaxArg);
                pnOp = this.nOp;
                this.lOp = null;
                return lOp.ToArray();
            }
            public int sqlite3VdbeAddOpList(int nOp, VdbeOpList[] aOp)
            {
                int addr;
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                if (this.nOp + nOp > this.nOpAlloc && this.growOpArray() != 0)
                {
                    return 0;
                }
                addr = this.nOp;
                if (ALWAYS(nOp > 0))
                {
                    int i;
                    VdbeOpList pIn;
                    for (i = 0; i < nOp; i++)
                    {
                        pIn = aOp[i];
                        int p2 = pIn.p2;
                        if (this.lOp[i + addr] == null)
                            this.lOp[i + addr] = new VdbeOp();
                        VdbeOp pOut = this.lOp[i + addr];
                        pOut.opcode = pIn.opcode;
                        pOut.p1 = pIn.p1;
                        if (p2 < 0 && (sqlite3OpcodeProperty[pOut.opcode] & OPFLG_JUMP) != 0)
                        {
                            pOut.p2 = addr + (-1 - p2);
                            // ADDR(p2);
                        }
                        else
                        {
                            pOut.p2 = p2;
                        }
                        pOut.p3 = pIn.p3;
                        pOut.p4type = P4_NOTUSED;
                        pOut.p4.p = null;
                        pOut.p5 = 0;
#if SQLITE_DEBUG
																																																																																																																													          pOut.zComment = null;
          if ( sqlite3VdbeAddopTrace )
          {
            sqlite3VdbePrintOp( null, i + addr, p.aOp[i + addr] );
          }
#endif
                    }
                    this.nOp += nOp;
                }
                return addr;
            }
            public void sqlite3VdbeChangeP1(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.nOp > addr)
                {
                    this.lOp[addr].p1 = val;
                }
            }
            public void sqlite3VdbeChangeP2(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.nOp > addr)
                {
                    this.lOp[addr].p2 = val;
                }
            }
            public void sqlite3VdbeChangeP3(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.nOp > addr)
                {
                    this.lOp[addr].p3 = val;
                }
            }
            public void sqlite3VdbeChangeP5(u8 val)
            {
                Debug.Assert(this != null);
                if (this.lOp != null)
                {
                    Debug.Assert(this.nOp > 0);
                    this.lOp[this.nOp - 1].p5 = val;
                }
            }
            public void sqlite3VdbeJumpHere(int addr)
            {
                Debug.Assert(addr >= 0);
                this.sqlite3VdbeChangeP2(addr, this.nOp);
            }
            public void sqlite3VdbeChangeP4(int addr, CollSeq pColl, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pColl;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, FuncDef pFunc, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pFunc = pFunc;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, KeyInfo pKeyInfo, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pKeyInfo;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, int i32n, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = i32n;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, char c, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.z = c.ToString();
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, Mem m, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pMem = m;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, dxDel P4_Type)
            {
                union_p4 _p4 = new union_p4();
                _p4.z = z;
                this.sqlite3VdbeChangeP4(addr, _p4, P4_DYNAMIC);
            }
            public void sqlite3VdbeChangeP4(int addr, SubProgram pProgram, int n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pProgram = pProgram;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, int n)
            {
                union_p4 _p4 = new union_p4();
                if (n > 0 && n <= z.Length)
                    _p4.z = z.Substring(0, n);
                else
                    _p4.z = z;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, union_p4 _p4, int n)
            {
                Op pOp;
                sqlite3 db;
                Debug.Assert(this != null);
                db = this.db;
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                if (this.lOp == null/*|| db.mallocFailed != 0 */)
                {
                    if (n != P4_KEYINFO && n != P4_VTAB)
                    {
                        freeP4(db, n, _p4);
                    }
                    return;
                }
                Debug.Assert(this.nOp > 0);
                Debug.Assert(addr < this.nOp);
                if (addr < 0)
                {
                    addr = this.nOp - 1;
                }
                pOp = this.lOp[addr];
                freeP4(db, pOp.p4type, pOp.p4.p);
                pOp.p4.p = null;
                if (n == P4_INT32)
                {
                    /* Note: this cast is safe, because the origin data point was an int
        ** that was cast to a (string ). */
                    pOp.p4.i = _p4.i;
                    // SQLITE_PTR_TO_INT(zP4);
                    pOp.p4type = P4_INT32;
                }
                else
                    if (n == P4_INT64)
                    {
                        pOp.p4.pI64 = _p4.pI64;
                        pOp.p4type = n;
                    }
                    else
                        if (n == P4_REAL)
                        {
                            pOp.p4.pReal = _p4.pReal;
                            pOp.p4type = n;
                        }
                        else
                            if (_p4 == null)
                            {
                                pOp.p4.p = null;
                                pOp.p4type = P4_NOTUSED;
                            }
                            else
                                if (n == P4_KEYINFO)
                                {
                                    KeyInfo pKeyInfo;
                                    int nField, nByte;
                                    nField = _p4.pKeyInfo.nField;
                                    //nByte = sizeof(*pKeyInfo) + (nField-1)*sizeof(pKeyInfo.aColl[0]) + nField;
                                    pKeyInfo = new KeyInfo();
                                    //sqlite3DbMallocRaw(0, nByte);
                                    pOp.p4.pKeyInfo = pKeyInfo;
                                    if (pKeyInfo != null)
                                    {
                                        //u8 *aSortOrder;
                                        // memcpy((char)pKeyInfo, zP4, nByte - nField);
                                        //aSortOrder = pKeyInfo.aSortOrder;
                                        //if( aSortOrder ){
                                        //  pKeyInfo.aSortOrder = (unsigned char)&pKeyInfo.aColl[nField];
                                        //  memcpy(pKeyInfo.aSortOrder, aSortOrder, nField);
                                        //}
                                        pKeyInfo = _p4.pKeyInfo.Copy();
                                        pOp.p4type = P4_KEYINFO;
                                    }
                                    else
                                    {
                                        //p.db.mallocFailed = 1;
                                        pOp.p4type = P4_NOTUSED;
                                    }
                                    pOp.p4.pKeyInfo = _p4.pKeyInfo;
                                    pOp.p4type = P4_KEYINFO;
                                }
                                else
                                    if (n == P4_KEYINFO_HANDOFF || n == P4_KEYINFO_STATIC)
                                    {
                                        pOp.p4.pKeyInfo = _p4.pKeyInfo;
                                        pOp.p4type = P4_KEYINFO;
                                    }
                                    else
                                        if (n == P4_FUNCDEF)
                                        {
                                            pOp.p4.pFunc = _p4.pFunc;
                                            pOp.p4type = P4_FUNCDEF;
                                        }
                                        else
                                            if (n == P4_COLLSEQ)
                                            {
                                                pOp.p4.pColl = _p4.pColl;
                                                pOp.p4type = P4_COLLSEQ;
                                            }
                                            else
                                                if (n == P4_DYNAMIC || n == P4_STATIC || n == P4_MPRINTF)
                                                {
                                                    pOp.p4.z = _p4.z;
                                                    pOp.p4type = P4_DYNAMIC;
                                                }
                                                else
                                                    if (n == P4_MEM)
                                                    {
                                                        pOp.p4.pMem = _p4.pMem;
                                                        pOp.p4type = P4_MEM;
                                                    }
                                                    else
                                                        if (n == P4_INTARRAY)
                                                        {
                                                            pOp.p4.ai = _p4.ai;
                                                            pOp.p4type = P4_INTARRAY;
                                                        }
                                                        else
                                                            if (n == P4_SUBPROGRAM)
                                                            {
                                                                pOp.p4.pProgram = _p4.pProgram;
                                                                pOp.p4type = P4_SUBPROGRAM;
                                                            }
                                                            else
                                                                if (n == P4_VTAB)
                                                                {
                                                                    pOp.p4.pVtab = _p4.pVtab;
                                                                    pOp.p4type = P4_VTAB;
                                                                    sqlite3VtabLock(_p4.pVtab);
                                                                    Debug.Assert((_p4.pVtab).db == this.db);
                                                                }
                                                                else
                                                                    if (n < 0)
                                                                    {
                                                                        pOp.p4.p = _p4.p;
                                                                        pOp.p4type = n;
                                                                    }
                                                                    else
                                                                    {
                                                                        //if (n == 0) n =  n = StringExtensions.sqlite3Strlen30(zP4);
                                                                        pOp.p4.z = _p4.z;
                                                                        // sqlite3DbStrNDup(p.db, zP4, n);
                                                                        pOp.p4type = P4_DYNAMIC;
                                                                    }
            }
            public VdbeOp sqlite3VdbeGetOp(int addr)
            {
                /* C89 specifies that the constant "dummy" will be initialized to all
      ** zeros, which is correct.  MSVC generates a warning, nevertheless. */
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                if (addr < 0)
                {
#if SQLITE_OMIT_TRACE
																																																																																																					if( p.nOp==0 ) return dummy;
#endif
                    addr = this.nOp - 1;
                }
                Debug.Assert((addr >= 0 && addr < this.nOp)/* || p.db.mallocFailed != 0 */);
                //if ( p.db.mallocFailed != 0 )
                //{
                //  return dummy;
                //}
                //else
                {
                    return this.lOp[addr];
                }
            }
            public string sqlite3IndexAffinityStr(Index pIdx)
            {
                if (pIdx.zColAff == null || pIdx.zColAff[0] == '\0')
                {
                    /* The first time a column affinity string for a particular index is
        ** required, it is allocated and populated here. It is then stored as
        ** a member of the Index structure for subsequent use.
        **
        ** The column affinity string will eventually be deleted by
        ** sqliteDeleteIndex() when the Index structure itself is cleaned
        ** up.
        */
                    int n;
                    Table pTab = pIdx.pTable;
                    sqlite3 db = this.sqlite3VdbeDb();
                    StringBuilder pIdx_zColAff = new StringBuilder(pIdx.nColumn + 2);
                    // (char )sqlite3DbMallocRaw(0, pIdx->nColumn+2);
                    //      if ( pIdx_zColAff == null )
                    //      {
                    //        db.mallocFailed = 1;
                    //        return null;
                    //      }
                    for (n = 0; n < pIdx.nColumn; n++)
                    {
                        pIdx_zColAff.Append(pTab.aCol[pIdx.aiColumn[n]].affinity);
                    }
                    pIdx_zColAff.Append(SQLITE_AFF_NONE);
                    pIdx_zColAff.Append('\0');
                    pIdx.zColAff = pIdx_zColAff.ToString();
                }
                return pIdx.zColAff;
            }
            public void sqlite3TableAffinityStr(Table pTab)
            {
                /* The first time a column affinity string for a particular table
      ** is required, it is allocated and populated here. It is then
      ** stored as a member of the Table structure for subsequent use.
      **
      ** The column affinity string will eventually be deleted by
      ** sqlite3DeleteTable() when the Table structure itself is cleaned up.
      */
                if (pTab.zColAff == null)
                {
                    StringBuilder zColAff;
                    int i;
                    sqlite3 db = this.sqlite3VdbeDb();
                    zColAff = new StringBuilder(pTab.nCol + 1);
                    // (char)sqlite3DbMallocRaw(0, pTab->nCol+1);
                    if (zColAff == null)
                    {
                        ////        db.mallocFailed = 1;
                        return;
                    }
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        zColAff.Append(pTab.aCol[i].affinity);
                    }
                    //zColAff.Append( '\0' );
                    pTab.zColAff = zColAff.ToString();
                }
                this.sqlite3VdbeChangeP4(-1, pTab.zColAff, P4_TRANSIENT);
            }
            public void sqlite3ColumnDefault(Table pTab, int i, int iReg)
            {
                Debug.Assert(pTab != null);
                if (null == pTab.pSelect)
                {
                    sqlite3_value pValue = new sqlite3_value();
                    SqliteEncoding enc = ENC(this.sqlite3VdbeDb());
                    Column pCol = pTab.aCol[i];
#if SQLITE_DEBUG
																																																																																																        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
                    Debug.Assert(i < pTab.nCol);
                    sqlite3ValueFromExpr(this.sqlite3VdbeDb(), pCol.pDflt, enc, pCol.affinity, ref pValue);
                    if (pValue != null)
                    {
                        this.sqlite3VdbeChangeP4(-1, pValue, P4_MEM);
                    }
#if !SQLITE_OMIT_FLOATING_POINT
                    if (iReg >= 0 && pTab.aCol[i].affinity == SQLITE_AFF_REAL)
                    {
                        this.sqlite3VdbeAddOp1(OP_RealAffinity, iReg);
                    }
#endif
                }
            }
            public void sqlite3ExprCodeGetColumnOfTable(/* The VDBE under construction */Table pTab,/* The table containing the value */int iTabCur,/* The cursor for this table */int iCol,/* Index of the column to extract */int regOut/* Extract the value into this register */)
            {
                if (iCol < 0 || iCol == pTab.iPKey)
                {
                    this.sqlite3VdbeAddOp2(OP_Rowid, iTabCur, regOut);
                }
                else
                {
                    int op = IsVirtual(pTab) ? OP_VColumn : OP_Column;
                    this.sqlite3VdbeAddOp3(op, iTabCur, iCol, regOut);
                }
                if (iCol >= 0)
                {
                    this.sqlite3ColumnDefault(pTab, iCol, regOut);
                }
            }
            public bool vdbeSafety()
            {
                if (this.db == null)
                {
                    sqlite3_log(SQLITE_MISUSE, "API called with finalized prepared statement");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public bool vdbeSafetyNotNull()
            {
                if (this == null)
                {
                    sqlite3_log(SQLITE_MISUSE, "API called with NULL prepared statement");
                    return true;
                }
                else
                {
                    return this.vdbeSafety();
                }
            }
            public int growOpArray()
            {
                //VdbeOp pNew;
                int nNew = (this.nOpAlloc != 0 ? this.nOpAlloc * 2 : 1024 / 4);
                //(int)(1024/sizeof(Op)));
                // pNew = sqlite3DbRealloc( p.db, p.aOp, nNew * sizeof( Op ) );
                //if (pNew != null)
                //{
                //      p.nOpAlloc = sqlite3DbMallocSize(p.db, pNew)/sizeof(Op);
                //  p.aOp = pNew;
                //}
                this.nOpAlloc = nNew;
                if (this.aOp == null)
                    this.aOp = new VdbeOp[nNew];
                else
                    Array.Resize(ref this.aOp, nNew);
                return (this.aOp != null ? SQLITE_OK : SQLITE_NOMEM);
                //  return (pNew ? SQLITE_OK : SQLITE_NOMEM);
            }
            public void sqlite3VdbeAddParseSchemaOp(int iDb, string zWhere)
            {
                int j;
                int addr = this.sqlite3VdbeAddOp3(OP_ParseSchema, iDb, 0, 0);
                this.sqlite3VdbeChangeP4(addr, zWhere, P4_DYNAMIC);
                for (j = 0; j < this.db.nDb; j++)
                    sqlite3VdbeUsesBtree(this, j);
            }
            public void sqlite3VdbeLinkSubProgram(SubProgram p)
            {
                p.pNext = this.pProgram;
                this.pProgram = p;
            }
            public void sqlite3VdbeRewind()
            {
#if (SQLITE_DEBUG) || (VDBE_PROFILE)
																																																																			      int i;
#endif
                Debug.Assert(this != null);
                Debug.Assert(this.magic == VDBE_MAGIC_INIT);
                /* There should be at least one opcode.
      */
                Debug.Assert(this.nOp > 0);
                /* Set the magic to VDBE_MAGIC_RUN sooner rather than later. */
                this.magic = VDBE_MAGIC_RUN;
#if SQLITE_DEBUG
																																																																			      for(i=1; i<p.nMem; i++){
        Debug.Assert( p.aMem[i].db==p.db );
      }
#endif
                this.currentOpCodeIndex = -1;
                this.rc = SQLITE_OK;
                this.errorAction = OE_Abort;
                this.magic = VDBE_MAGIC_RUN;
                this.nChange = 0;
                this.cacheCtr = 1;
                this.minWriteFileFormat = 255;
                this.iStatement = 0;
                this.nFkConstraint = 0;
#if VDBE_PROFILE
																																																																			      for(i=0; i<p.nOp; i++){
        p.aOp[i].cnt = 0;
        p.aOp[i].cycles = 0;
      }
#endif
            }
            public void sqlite3VdbeSetNumCols(int nResColumn)
            {
                Mem pColName;
                int n;
                sqlite3 db = this.db;
                releaseMemArray(this.aColName, this.nResColumn * COLNAME_N);
                db.sqlite3DbFree(ref this.aColName);
                n = nResColumn * COLNAME_N;
                this.nResColumn = (u16)nResColumn;
                this.aColName = new Mem[n];
                // (Mem)sqlite3DbMallocZero(db, Mem.Length * n);
                //if (p.aColName == 0) return;
                while (n-- > 0)
                {
                    this.aColName[n] = sqlite3Malloc(this.aColName[n]);
                    pColName = this.aColName[n];
                    pColName.flags = MEM_Null;
                    pColName.db = this.db;
                }
            }
            public int sqlite3VdbeSetColName(/* Vdbe being configured */int idx,/* Index of column zName applies to */int var,/* One of the COLNAME_* constants */string zName,/* Pointer to buffer containing name */dxDel xDel/* Memory management strategy for zName */)
            {
                int rc;
                Mem pColName;
                Debug.Assert(idx < this.nResColumn);
                Debug.Assert(var < COLNAME_N);
                //if ( p.db.mallocFailed != 0 )
                //{
                //  Debug.Assert( null == zName || xDel != SQLITE_DYNAMIC );
                //  return SQLITE_NOMEM;
                //}
                Debug.Assert(this.aColName != null);
                pColName = this.aColName[idx + var * this.nResColumn];
                rc = sqlite3VdbeMemSetStr(pColName, zName, -1, SqliteEncoding.UTF8, xDel);
                Debug.Assert(rc != 0 || null == zName || (pColName.flags & MEM_Term) != 0);
                return rc;
            }
            public int sqlite3VdbeCloseStatement(int eOp)
            {
                sqlite3 db = this.db;
                int rc = SQLITE_OK;
                /* If p->iStatement is greater than zero, then this Vdbe opened a 
      ** statement transaction that should be closed here. The only exception
      ** is that an IO error may have occured, causing an emergency rollback.
      ** In this case (db->nStatement==0), and there is nothing to do.
      */
                if (db.nStatement != 0 && this.iStatement != 0)
                {
                    int i;
                    int iSavepoint = this.iStatement - 1;
                    Debug.Assert(eOp == SAVEPOINT_ROLLBACK || eOp == SAVEPOINT_RELEASE);
                    Debug.Assert(db.nStatement > 0);
                    Debug.Assert(this.iStatement == (db.nStatement + db.nSavepoint));
                    for (i = 0; i < db.nDb; i++)
                    {
                        int rc2 = SQLITE_OK;
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt != null)
                        {
                            if (eOp == SAVEPOINT_ROLLBACK)
                            {
                                rc2 = sqlite3BtreeSavepoint(pBt, SAVEPOINT_ROLLBACK, iSavepoint);
                            }
                            if (rc2 == SQLITE_OK)
                            {
                                rc2 = sqlite3BtreeSavepoint(pBt, SAVEPOINT_RELEASE, iSavepoint);
                            }
                            if (rc == SQLITE_OK)
                            {
                                rc = rc2;
                            }
                        }
                    }
                    db.nStatement--;
                    this.iStatement = 0;
                    if (rc == SQLITE_OK)
                    {
                        if (eOp == SAVEPOINT_ROLLBACK)
                        {
                            rc = sqlite3VtabSavepoint(db, SAVEPOINT_ROLLBACK, iSavepoint);
                        }
                        if (rc == SQLITE_OK)
                        {
                            rc = sqlite3VtabSavepoint(db, SAVEPOINT_RELEASE, iSavepoint);
                        }
                    }
                    /* If the statement transaction is being rolled back, also restore the 
        ** database handles deferred constraint counter to the value it had when 
        ** the statement transaction was opened.  */
                    if (eOp == SAVEPOINT_ROLLBACK)
                    {
                        db.nDeferredCons = this.nStmtDefCons;
                    }
                }
                return rc;
            }
            public int sqlite3VdbeCheckFk(int deferred)
            {
                sqlite3 db = this.db;
                if ((deferred != 0 && db.nDeferredCons > 0) || (0 == deferred && this.nFkConstraint > 0))
                {
                    this.rc = SQLITE_CONSTRAINT;
                    this.errorAction = OE_Abort;
                    sqlite3SetString(ref this.zErrMsg, db, "foreign key constraint failed");
                    return SQLITE_ERROR;
                }
                return SQLITE_OK;
            }
            public int sqlite3VdbeHalt()
            {
                int rc;
                /* Used to store transient return codes */
                sqlite3 db = this.db;
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
      */
                //if ( p.db.mallocFailed != 0 )
                //{
                //  p.rc = SQLITE_NOMEM;
                //}
                closeAllCursors(this);
                if (this.magic != VDBE_MAGIC_RUN)
                {
                    return SQLITE_OK;
                }
                checkActiveVdbeCnt(db);
                /* No commit or rollback needed if the program never started */
                if (this.currentOpCodeIndex >= 0)
                {
                    int mrc;
                    /* Primary error code from p.rc */
                    int eStatementOp = 0;
                    bool isSpecialError = false;
                    /* Set to true if a 'special' error */
                    /* Lock all btrees used by the statement */
                    this.sqlite3VdbeEnter();
                    /* Check for one of the special errors */
                    mrc = this.rc & 0xff;
                    Debug.Assert(this.rc != SQLITE_IOERR_BLOCKED);
                    /* This error no longer exists */
                    isSpecialError = mrc == SQLITE_NOMEM || mrc == SQLITE_IOERR || mrc == SQLITE_INTERRUPT || mrc == SQLITE_FULL;
                    if (isSpecialError)
                    {
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
          */
                        if (!this.readOnly || mrc != SQLITE_INTERRUPT)
                        {
                            if ((mrc == SQLITE_NOMEM || mrc == SQLITE_FULL) && this.usesStmtJournal)
                            {
                                eStatementOp = SAVEPOINT_ROLLBACK;
                            }
                            else
                            {
                                /* We are forced to roll back the active transaction. Before doing
              ** so, abort any other statements this handle currently has active.
              */
                                invalidateCursorsOnModifiedBtrees(db);
                                sqlite3RollbackAll(db);
                                sqlite3CloseSavepoints(db);
                                db.autoCommit = 1;
                            }
                        }
                    }
                    /* Check for immediate foreign key violations. */
                    if (this.rc == SQLITE_OK)
                    {
                        this.sqlite3VdbeCheckFk(0);
                    }
                    /* If the auto-commit flag is set and this is the only active writer
        ** VM, then we do either a commit or rollback of the current transaction.
        **
        ** Note: This block also runs if one of the special errors handled
        ** above has occurred.
        */
                    if (!sqlite3VtabInSync(db) && db.autoCommit != 0 && db.writeVdbeCnt == ((this.readOnly == false) ? 1 : 0))
                    {
                        if (this.rc == SQLITE_OK || (this.errorAction == OE_Fail && !isSpecialError))
                        {
                            rc = this.sqlite3VdbeCheckFk(1);
                            if (rc != SQLITE_OK)
                            {
                                if (NEVER(this.readOnly))
                                {
                                    this.sqlite3VdbeLeave();
                                    return SQLITE_ERROR;
                                }
                                rc = SQLITE_CONSTRAINT;
                            }
                            else
                            {
                                /* The auto-commit flag is true, the vdbe program was successful 
              ** or hit an 'OR FAIL' constraint and there are no deferred foreign
              ** key constraints to hold up the transaction. This means a commit 
              ** is required. */
                                rc = vdbeCommit(db, this);
                            }
                            if (rc == SQLITE_BUSY && this.readOnly)
                            {
                                this.sqlite3VdbeLeave();
                                return SQLITE_BUSY;
                            }
                            else
                                if (rc != SQLITE_OK)
                                {
                                    this.rc = rc;
                                    sqlite3RollbackAll(db);
                                }
                                else
                                {
                                    db.nDeferredCons = 0;
                                    sqlite3CommitInternalChanges(db);
                                }
                        }
                        else
                        {
                            sqlite3RollbackAll(db);
                        }
                        db.nStatement = 0;
                    }
                    else
                        if (eStatementOp == 0)
                        {
                            if (this.rc == SQLITE_OK || this.errorAction == OE_Fail)
                            {
                                eStatementOp = SAVEPOINT_RELEASE;
                            }
                            else
                                if (this.errorAction == OE_Abort)
                                {
                                    eStatementOp = SAVEPOINT_ROLLBACK;
                                }
                                else
                                {
                                    invalidateCursorsOnModifiedBtrees(db);
                                    sqlite3RollbackAll(db);
                                    sqlite3CloseSavepoints(db);
                                    db.autoCommit = 1;
                                }
                        }
                    /* If eStatementOp is non-zero, then a statement transaction needs to
        ** be committed or rolled back. Call sqlite3VdbeCloseStatement() to
        ** do so. If this operation returns an error, and the current statement
        ** error code is SQLITE_OK or SQLITE_CONSTRAINT, then promote the
        ** current statement error code.
        */
                    if (eStatementOp != 0)
                    {
                        rc = this.sqlite3VdbeCloseStatement(eStatementOp);
                        if (rc != 0)
                        {
                            if (this.rc == SQLITE_OK || this.rc == SQLITE_CONSTRAINT)
                            {
                                this.rc = rc;
                                db.sqlite3DbFree(ref this.zErrMsg);
                                this.zErrMsg = null;
                            }
                            invalidateCursorsOnModifiedBtrees(db);
                            sqlite3RollbackAll(db);
                            sqlite3CloseSavepoints(db);
                            db.autoCommit = 1;
                        }
                    }
                    /* If this was an INSERT, UPDATE or DELETE and no statement transaction
        ** has been rolled back, update the database connection change-counter.
        */
                    if (this.changeCntOn)
                    {
                        if (eStatementOp != SAVEPOINT_ROLLBACK)
                        {
                            sqlite3VdbeSetChanges(db, this.nChange);
                        }
                        else
                        {
                            sqlite3VdbeSetChanges(db, 0);
                        }
                        this.nChange = 0;
                    }
                    /* Rollback or commit any schema changes that occurred. */
                    if (this.rc != SQLITE_OK && (db.flags & SQLITE_InternChanges) != 0)
                    {
                        sqlite3ResetInternalSchema(db, -1);
                        db.flags = (db.flags | SQLITE_InternChanges);
                    }
                    /* Release the locks */
                    this.sqlite3VdbeLeave();
                }
                /* We have successfully halted and closed the VM.  Record this fact. */
                if (this.currentOpCodeIndex >= 0)
                {
                    db.activeVdbeCnt--;
                    if (!this.readOnly)
                    {
                        db.writeVdbeCnt--;
                    }
                    Debug.Assert(db.activeVdbeCnt >= db.writeVdbeCnt);
                }
                this.magic = VDBE_MAGIC_HALT;
                checkActiveVdbeCnt(db);
                //if ( p.db.mallocFailed != 0 )
                //{
                //  p.rc = SQLITE_NOMEM;
                //}
                /* If the auto-commit flag is set to true, then any locks that were held
      ** by connection db have now been released. Call sqlite3ConnectionUnlocked()
      ** to invoke any required unlock-notify callbacks.
      */
                if (db.autoCommit != 0)
                {
                    sqlite3ConnectionUnlocked(db);
                }
                Debug.Assert(db.activeVdbeCnt > 0 || db.autoCommit == 0 || db.nStatement == 0);
                return (this.rc == SQLITE_BUSY ? SQLITE_BUSY : SQLITE_OK);
            }
            public void sqlite3VdbeResetStepResult()
            {
                this.rc = SQLITE_OK;
            }
            public int sqlite3VdbeReset()
            {
                sqlite3 db;
                db = this.db;
                /* If the VM did not run to completion or if it encountered an
      ** error, then it might not have been halted properly.  So halt
      ** it now.
      */
                this.sqlite3VdbeHalt();
                /* If the VDBE has be run even partially, then transfer the error code
      ** and error message from the VDBE into the main database structure.  But
      ** if the VDBE has just been set to run but has not actually executed any
      ** instructions yet, leave the main database error information unchanged.
      */
                if (this.currentOpCodeIndex >= 0)
                {
                    //if ( p.zErrMsg != 0 ) // Always exists under C#
                    {
                        sqlite3BeginBenignMalloc();
                        sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg == null ? "" : this.zErrMsg, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
                        sqlite3EndBenignMalloc();
                        db.errCode = this.rc;
                        db.sqlite3DbFree(ref this.zErrMsg);
                        this.zErrMsg = "";
                        //else if ( p.rc != 0 )
                        //{
                        //  sqlite3Error( db, p.rc, 0 );
                        //}
                        //else
                        //{
                        //  sqlite3Error( db, SQLITE_OK, 0 );
                        //}
                    }
                    if (this.runOnlyOnce != 0)
                        this.expired = true;
                }
                else
                    if (this.rc != 0 && this.expired)
                    {
                        /* The expired flag was set on the VDBE before the first call
        ** to sqlite3_step(). For consistency (since sqlite3_step() was
        ** called), set the database error in this case as well.
        */
                        sqlite3Error(db, this.rc, 0);
                        sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
                        db.sqlite3DbFree(ref this.zErrMsg);
                        this.zErrMsg = "";
                    }
                /* Reclaim all memory used by the VDBE
      */
                Cleanup(this);
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
                this.magic = VDBE_MAGIC_INIT;
                return this.rc & db.errMask;
            }
            public sqlite3 sqlite3VdbeDb()
            {
                return this.db;
            }
            public sqlite3_value sqlite3VdbeGetValue(int iVar, u8 aff)
            {
                Debug.Assert(iVar > 0);
                if (this != null)
                {
                    Mem pMem = this.aVar[iVar - 1];
                    if (0 == (pMem.flags & MEM_Null))
                    {
                        sqlite3_value pRet = sqlite3ValueNew(this.db);
                        if (pRet != null)
                        {
                            sqlite3VdbeMemCopy((Mem)pRet, pMem);
                            sqlite3ValueApplyAffinity(pRet, (char)aff, SqliteEncoding.UTF8);
                            sqlite3VdbeMemStoreType((Mem)pRet);
                        }
                        return pRet;
                    }
                }
                return null;
            }
            public void sqlite3VdbeSetVarmask(int iVar)
            {
                Debug.Assert(iVar > 0);
                if (iVar > 32)
                {
                    this.expmask = 0xffffffff;
                }
                else
                {
                    this.expmask |= ((u32)1 << (iVar - 1));
                }
            }
            public void sqlite3VdbeCountChanges()
            {
                this.changeCntOn = true;
            }
        }

#endif


    }
}
