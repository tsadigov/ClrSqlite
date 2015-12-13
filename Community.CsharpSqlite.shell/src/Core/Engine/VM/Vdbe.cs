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
namespace Community.CsharpSqlite
{
    
    using System.Text;
    using sqlite3_value = Engine.Mem;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Vdbe=Engine.Vdbe;
    


    

    namespace Engine
    {

        using Operation = VdbeOp;
        using Community.CsharpSqlite.tree;
        using Community.CsharpSqlite.Paging;
        using Community.CsharpSqlite.Utils;



        public enum RuntimeException
        {
            too_big, no_mem, abort_due_to_error, abort_due_to_interrupt, vdbe_error_halt,
            vdbe_return, OK,
            noop
        }

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
        public class Vdbe : CPU, ILinkedListNode<Vdbe>
        {
            #region hehehe
#if SQLITE_ENABLE_COLUMN_METADATA
																																						const int COLNAME_N = 5;     /* Number of COLNAME_xxx symbols */
#else
#if SQLITE_OMIT_DECLTYPE
																																						const int COLNAME_N = 1;     /* Number of COLNAME_xxx symbols */
#else
            public const int COLNAME_N = 2;
#endif
#endif
            public Vdbe()
            {
            }
            ///
            ///<summary>
            ///The database connection that owns this statement 
            ///</summary>
            public Connection db;
            /** Space to hold the virtual machine's program */
            public Operation[] aOp;
            ///
            ///<summary>
            ///The memory locations 
            ///</summary>
            public List<Operation> lOp = new List<Operation>();
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
            public Vdbe pNext { get; set; }
            ///
            ///<summary>
            ///Linked list of VDBEs with the same Vdbe.db 
            ///</summary>


            ///<summary>
            ///One element of this array for each open cursor 
            ///</summary>
            public VdbeCursor[] OpenCursors
            {
                get;
                set;
            }


            ///
            ///<summary>
            ///Values for the  OpCode.OP_Variable opcode. 
            ///</summary>
            public Mem[] aVar;
            ///<summary>
            ///Name of variables 
            ///</summary>
            public string[] azVar;
            ///
            ///<summary>
            ///Number of entries in aVar[] 
            ///</summary>
            public ynVar nVar;
            ///
            public ynVar nzVar;
            ///
            ///<summary>
            ///Number of entries in azVar[] 
            ///</summary>
            public u32 cacheCtr;
            
            /// <summary>
            /// Value to return
            /// </summary>
            public SqlResult rc;
            public SqlResult result
            {
                get
                {
                    return (SqlResult)rc;
                }
                set
                {
                    rc = value;
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
            public int[] aCounter = new int[3];
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
            public string zSql = "";
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
                ct.OpenCursors = OpenCursors;
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
                //void sqlite3VdbePrintOp(FILE*, int, Operation);
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

            #endregion


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
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                //Debug.Assert(op>0&&op<0xff);
                if (this.nOpAlloc <= i)
                {
                    if (this.growOpArray() != 0)
                    {
                        return 1;
                    }
                }
                this.nOp++;
                pOp = new Operation();
                pOp.OpCode = op;
                pOp.p5 = 0;
                pOp.p1 = p1;
                pOp.p2 = p2;
                pOp.p3 = p3;
                pOp.p4.p = null;
                pOp.p4type = P4Usage.P4_NOTUSED;
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
            public int sqlite3VdbeAddOp0(int op)
            {//---------------------
                return this.sqlite3VdbeAddOp3(op, 0, 0, 0);
            }
            public int sqlite3VdbeAddOp1(OpCode op, int p1)
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
            public int sqlite3VdbeAddOp2(OpCode op, int p1, int p2)
            {
                return this.sqlite3VdbeAddOp3(op, p1, p2, 0);
            }
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, i32 pP4, P4Usage p4type)
            {
                return sqlite3VdbeAddOp4((int)op, p1, p2, p3, pP4, p4type);
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, i32 pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }


            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, StringBuilder pP4, P4Usage p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4.ToString();
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
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



            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, string pP4, P4Usage p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
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

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, int[] pP4, P4Usage p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.ai = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
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
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, double pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pReal = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, FuncDef pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pFunc = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(OpCode opCode, yDbMask p1, yDbMask regAgg, yDbMask p2, FuncDef funcDef, P4Usage p4type)
            {
                return sqlite3VdbeAddOp4((int)opCode, p1, regAgg, p2, funcDef, p4type);
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, CollSeq pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(int op, int p1, int p2, int p3, KeyInfo pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
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
                this.sqlite3VdbeChangeP4(addr, _p4, P4Usage.P4_INT32);
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
                this.sqlite3VdbeChangeP4(addr, _p4, P4Usage.P4_INT32);
                return addr;
            }

            public int sqlite3VdbeMakeLabel()
            {
                int i;
                i = this.nLabel++;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (i >= this.nLabelAlloc)
                {
                    int n = this.nLabelAlloc == 0 ? 15 : this.nLabelAlloc * 2 + 5;
                    if (this.aLabel == null)
                        this.aLabel = malloc_cs.sqlite3Malloc(this.aLabel, n);
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
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
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
                Operation pOp;
                int[] aLabel = this.aLabel;
                this.readOnly = true;
                for (i = 0; i < this.nOp; i++)//  for(pOp=p->aOp, i=p->nOp-1; i>=0; i--, pOp++)
                {
                    pOp = this.lOp[i];
                    OpCode opcode = pOp.OpCode;
                    pOp.opflags = Sqlite3.sqlite3OpcodeProperty[(u8)opcode];
                    if (opcode == OpCode.OP_Function || opcode == OpCode.OP_AggStep)
                    {
                        if (pOp.p5 > nMaxArgs)
                            nMaxArgs = pOp.p5;
                    }
                    else
                        if ((opcode == OpCode.OP_Transaction && pOp.p2 != 0) || opcode == OpCode.OP_Vacuum)
                        {
                            this.readOnly = false;
#if !SQLITE_OMIT_VIRTUALTABLE
                        }
                        else
                            if (opcode == OpCode.OP_VUpdate)
                            {
                                if (pOp.p2 > nMaxArgs)
                                    nMaxArgs = pOp.p2;
                            }
                            else
                                if (opcode == OpCode.OP_VFilter)
                                {
                                    int n;
                                    Debug.Assert(this.nOp - i >= 3);
                                    Debug.Assert(this.lOp[i - 1].OpCode == OpCode.OP_Integer);
                                    //pOp[-1].opcode==OpCode.OP_Integer );
                                    n = this.lOp[i - 1].p1;
                                    //pOp[-1].p1;
                                    if (n > nMaxArgs)
                                        nMaxArgs = n;
#endif
                                }
                    if (((int)pOp.opflags & Sqlite3.OPFLG_JUMP) != 0 && pOp.p2 < 0)
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
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                return this.nOp;
            }
            public VdbeOp[] sqlite3VdbeTakeOpArray(ref int pnOp, ref int pnMaxArg)
            {
                List<VdbeOp> lOp = this.lOp;
                Debug.Assert(aOp != null);
                // && 0==p.db.mallocFailed );
                ///
                ///<summary>
                ///Check that sqlite3VdbeUsesBtree() was not called on this VM 
                ///</summary>
                Debug.Assert(this.btreeMask == 0);
                this.resolveP2Values(ref pnMaxArg);
                pnOp = this.nOp;
                this.lOp = null;
                return lOp.ToArray();
            }
            public int sqlite3VdbeAddOpList(int nOp, VdbeOpList[] aOp)
            {
                int addr;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (this.nOp + nOp > this.nOpAlloc && this.growOpArray() != 0)
                {
                    return 0;
                }
                addr = this.nOp;
                if (Sqlite3.ALWAYS(nOp > 0))
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
                        if (p2 < 0 && (Sqlite3.sqlite3OpcodeProperty[pOut.opcode] & (OpFlag)Sqlite3.OPFLG_JUMP) != 0)
                        {
                            pOut.p2 = addr + (-1 - p2);
                            // ADDR(p2);
                        }
                        else
                        {
                            pOut.p2 = p2;
                        }
                        pOut.p3 = pIn.p3;
                        pOut.p4type = P4Usage.P4_NOTUSED;
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
            public void sqlite3VdbeJumpHere(int addr)
            {
                Debug.Assert(addr >= 0);
                this.sqlite3VdbeChangeP2(addr, this.nOp);
            }
            public void sqlite3VdbeChangeP4(int addr, CollSeq pColl, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pColl;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, FuncDef pFunc, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pFunc = pFunc;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, KeyInfo pKeyInfo, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pKeyInfo;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, int i32n, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = i32n;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, char c, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.z = c.ToString();
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, Mem m, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pMem = m;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, dxDel P4_Type)
            {
                union_p4 _p4 = new union_p4();
                _p4.z = z;
                this.sqlite3VdbeChangeP4(addr, _p4, P4Usage.P4_DYNAMIC);
            }
            public void sqlite3VdbeChangeP4(int addr, SubProgram pProgram, P4Usage n)
            {
                union_p4 _p4 = new union_p4();
                _p4.pProgram = pProgram;
                this.sqlite3VdbeChangeP4(addr, _p4, n);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, P4Usage p4type)
            {
                sqlite3VdbeChangeP4(addr, z, (int)p4type);
            }
            public void sqlite3VdbeChangeP4(int addr, string z, int n)
            {
                union_p4 _p4 = new union_p4();
                if (n > 0 && n <= z.Length)
                    _p4.z = z.Substring(0, n);
                else
                    _p4.z = z;
                this.sqlite3VdbeChangeP4(addr, _p4, (P4Usage)n);
            }
            public void sqlite3VdbeChangeP4(int addr, union_p4 _p4, P4Usage n)
            {
                Operation pOp;
                Connection db;
                Debug.Assert(this != null);
                db = this.db;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (this.lOp == null///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                )
                {
                    if (n != P4Usage.P4_KEYINFO && n != P4Usage.P4_VTAB)
                    {
                        vdbeaux.freeP4(db, n, _p4);
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
                vdbeaux.freeP4(db, pOp.p4type, pOp.p4.p);
                pOp.p4.p = null;
                if (n == P4Usage.P4_INT32)
                {
                    ///
                    ///<summary>
                    ///Note: this cast is safe, because the origin data point was an int
                    ///that was cast to a (string ). 
                    ///</summary>
                    pOp.p4.i = _p4.i;
                    // SQLITE_PTR_TO_INT(zP4);
                    pOp.p4type = P4Usage.P4_INT32;
                }
                else
                    if (n == P4Usage.P4_INT64)
                    {
                        pOp.p4.pI64 = _p4.pI64;
                        pOp.p4type = n;
                    }
                    else
                        if (n == P4Usage.P4_REAL)
                        {
                            pOp.p4.pReal = _p4.pReal;
                            pOp.p4type = n;
                        }
                        else
                            if (_p4 == null)
                            {
                                pOp.p4.p = null;
                                pOp.p4type = P4Usage.P4_NOTUSED;
                            }
                            else
                                if (n == P4Usage.P4_KEYINFO)
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
                                        pOp.p4type = P4Usage.P4_KEYINFO;
                                    }
                                    else
                                    {
                                        //p.db.mallocFailed = 1;
                                        pOp.p4type = P4Usage.P4_NOTUSED;
                                    }
                                    pOp.p4.pKeyInfo = _p4.pKeyInfo;
                                    pOp.p4type = P4Usage.P4_KEYINFO;
                                }
                                else
                                    if (n == P4Usage.P4_KEYINFO_HANDOFF || n == P4Usage.P4_KEYINFO_STATIC)
                                    {
                                        pOp.p4.pKeyInfo = _p4.pKeyInfo;
                                        pOp.p4type = P4Usage.P4_KEYINFO;
                                    }
                                    else
                                        if (n == P4Usage.P4_FUNCDEF)
                                        {
                                            pOp.p4.pFunc = _p4.pFunc;
                                            pOp.p4type = P4Usage.P4_FUNCDEF;
                                        }
                                        else
                                            if (n == P4Usage.P4_COLLSEQ)
                                            {
                                                pOp.p4.pColl = _p4.pColl;
                                                pOp.p4type = P4Usage.P4_COLLSEQ;
                                            }
                                            else
                                                if (n == P4Usage.P4_DYNAMIC || n == P4Usage.P4_STATIC || n == P4Usage.P4_MPRINTF)
                                                {
                                                    pOp.p4.z = _p4.z;
                                                    pOp.p4type = P4Usage.P4_DYNAMIC;
                                                }
                                                else
                                                    if (n == P4Usage.P4_MEM)
                                                    {
                                                        pOp.p4.pMem = _p4.pMem;
                                                        pOp.p4type = P4Usage.P4_MEM;
                                                    }
                                                    else
                                                        if (n == P4Usage.P4_INTARRAY)
                                                        {
                                                            pOp.p4.ai = _p4.ai;
                                                            pOp.p4type = P4Usage.P4_INTARRAY;
                                                        }
                                                        else
                                                            if (n == P4Usage.P4_SUBPROGRAM)
                                                            {
                                                                pOp.p4.pProgram = _p4.pProgram;
                                                                pOp.p4type = P4Usage.P4_SUBPROGRAM;
                                                            }
                                                            else
                                                                if (n == P4Usage.P4_VTAB)
                                                                {
                                                                    pOp.p4.pVtab = _p4.pVtab;
                                                                    pOp.p4type = P4Usage.P4_VTAB;
                                                                    VTableMethodsExtensions.sqlite3VtabLock(_p4.pVtab);
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
                                                                        pOp.p4type = P4Usage.P4_DYNAMIC;
                                                                    }
            }
            public VdbeOp sqlite3VdbeGetOp(int addr)
            {
                ///
                ///<summary>
                ///C89 specifies that the constant "dummy" will be initialized to all
                ///zeros, which is correct.  MSVC generates a warning, nevertheless. 
                ///</summary>
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (addr < 0)
                {
#if SQLITE_OMIT_TRACE
																																																																																																																				if( p.nOp==0 ) return dummy;
#endif
                    addr = this.nOp - 1;
                }
                Debug.Assert((addr >= 0 && addr < this.nOp)///
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
            public string sqlite3IndexAffinityStr(Index pIdx)
            {
                if (pIdx.zColAff == null || pIdx.zColAff[0] == '\0')
                {
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
                    Table pTab = pIdx.pTable;
                    Connection db = this.sqlite3VdbeDb();
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
                    pIdx_zColAff.Append(sqliteinth.SQLITE_AFF_NONE);
                    pIdx_zColAff.Append('\0');
                    pIdx.zColAff = pIdx_zColAff.ToString();
                }
                return pIdx.zColAff;
            }
            public void sqlite3TableAffinityStr(Table pTab)
            {
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
                if (pTab.zColAff == null)
                {
                    StringBuilder zColAff;
                    int i;
                    Connection db = this.sqlite3VdbeDb();
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
                this.sqlite3VdbeChangeP4(-1, pTab.zColAff, P4Usage.P4_TRANSIENT);
            }
            public void sqlite3ColumnDefault(Table pTab, int i, int iReg)
            {
                Debug.Assert(pTab != null);
                if (null == pTab.pSelect)
                {
                    sqlite3_value pValue = new sqlite3_value();
                    SqliteEncoding enc = sqliteinth.ENC(this.sqlite3VdbeDb());
                    Column pCol = pTab.aCol[i];
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
                    Debug.Assert(i < pTab.nCol);
                    vdbemem_cs.sqlite3ValueFromExpr(this.sqlite3VdbeDb(), pCol.pDflt, enc, pCol.affinity, ref pValue);
                    if (pValue != null)
                    {
                        this.sqlite3VdbeChangeP4(-1, pValue, P4Usage.P4_MEM);
                    }
#if !SQLITE_OMIT_FLOATING_POINT
                    if (iReg >= 0 && pTab.aCol[i].affinity == sqliteinth.SQLITE_AFF_REAL)
                    {
                        this.sqlite3VdbeAddOp1(OpCode.OP_RealAffinity, iReg);
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
            )
            {
                if (iCol < 0 || iCol == pTab.iPKey)
                {
                    this.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iTabCur, regOut);
                }
                else
                {
                    OpCode op = pTab.IsVirtual() ? OpCode.OP_VColumn : OpCode.OP_Column;
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
                    io.sqlite3_log(SqlResult.SQLITE_MISUSE, "API called with finalized prepared statement");
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
                    io.sqlite3_log(SqlResult.SQLITE_MISUSE, "API called with NULL prepared statement");
                    return true;
                }
                else
                {
                    return this.vdbeSafety();
                }
            }
            public SqlResult growOpArray()
            {
                //VdbeOp pNew;
                int nNew = (this.nOpAlloc != 0 ? this.nOpAlloc * 2 : 1024 / 4);
                //(int)(1024/sizeof(Operation)));
                // pNew = sqlite3DbRealloc( p.db, p.aOp, nNew * sizeof( Operation ) );
                //if (pNew != null)
                //{
                //      p.nOpAlloc = sqlite3DbMallocSize(p.db, pNew)/sizeof(Operation);
                //  p.aOp = pNew;
                //}
                this.nOpAlloc = nNew;
                if (this.aOp == null)
                    this.aOp = new VdbeOp[nNew];
                else
                    Array.Resize(ref this.aOp, nNew);
                return (this.aOp != null ? SqlResult.SQLITE_OK : SqlResult.SQLITE_NOMEM);
                //  return (pNew ? SqlResult.SQLITE_OK : SQLITE_NOMEM);
            }
            public void sqlite3VdbeAddParseSchemaOp(int iDb, string zWhere)
            {
                int j;
                int addr = this.sqlite3VdbeAddOp3(OpCode.OP_ParseSchema, iDb, 0, 0);
                this.sqlite3VdbeChangeP4(addr, zWhere, P4Usage.P4_DYNAMIC);
                for (j = 0; j < this.db.BackendCount; j++)
                    vdbeaux.sqlite3VdbeUsesBtree(this, j);
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
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                ///
                ///<summary>
                ///There should be at least one opcode.
                ///
                ///</summary>
                Debug.Assert(this.nOp > 0);
                ///
                ///<summary>
                ///Set the magic to VdbeMagic.VDBE_MAGIC_RUN sooner rather than later. 
                ///</summary>
                this.magic = VdbeMagic.VDBE_MAGIC_RUN;
#if SQLITE_DEBUG
																																																																															      for(i=1; i<p.nMem; i++){
        Debug.Assert( p.aMem[i].db==p.db );
      }
#endif
                this.currentOpCodeIndex = -1;
                this.rc = SqlResult.SQLITE_OK;
                this.errorAction = OnConstraintError.OE_Abort;
                this.magic = VdbeMagic.VDBE_MAGIC_RUN;
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
                Connection db = this.db;
                vdbeaux.releaseMemArray(this.aColName, this.nResColumn * COLNAME_N);
                db.sqlite3DbFree(ref this.aColName);
                n = nResColumn * COLNAME_N;
                this.nResColumn = (u16)nResColumn;
                this.aColName = new Mem[n];
                // (Mem)sqlite3DbMallocZero(db, Mem.Length * n);
                //if (p.aColName == 0) return;
                while (n-- > 0)
                {
                    this.aColName[n] = malloc_cs.sqlite3Malloc(this.aColName[n]);
                    pColName = this.aColName[n];
                    pColName.flags = MemFlags.MEM_Null;
                    pColName.db = this.db;
                }
            }
            public SqlResult sqlite3VdbeSetColName(
            int idx,///Index of column zName applies to 
            ColName var,///One of the COLNAME_* constants 
            string zName,///Pointer to buffer containing name 
            dxDel xDel///Memory management strategy for zName 
            )
            {
                SqlResult rc;
                Mem pColName;
                Debug.Assert(idx < this.nResColumn);
                Debug.Assert((int)var < Vdbe.COLNAME_N);
                //if ( p.db.mallocFailed != 0 )
                //{
                //  Debug.Assert( null == zName || xDel != SQLITE_DYNAMIC );
                //  return SQLITE_NOMEM;
                //}
                Debug.Assert(this.aColName != null);
                pColName = this.aColName[idx + (int)var * this.nResColumn];
                rc = pColName.sqlite3VdbeMemSetStr(zName, -1, SqliteEncoding.UTF8, xDel);
                Debug.Assert(rc != 0 || null == zName || (pColName.flags & MemFlags.MEM_Term) != 0);
                return rc;
            }
            public SqlResult sqlite3VdbeCloseStatement(int eOp)
            {
                Connection db = this.db;
                SqlResult rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///</summary>
                ///<param name="If p">>iStatement is greater than zero, then this Vdbe opened a </param>
                ///<param name="statement transaction that should be closed here. The only exception">statement transaction that should be closed here. The only exception</param>
                ///<param name="is that an IO error may have occured, causing an emergency rollback.">is that an IO error may have occured, causing an emergency rollback.</param>
                ///<param name="In this case (db">>nStatement==0), and there is nothing to do.</param>
                ///<param name=""></param>
                if (db.nStatement != 0 && this.iStatement != 0)
                {
                    int i;
                    int iSavepoint = this.iStatement - 1;
                    Debug.Assert(eOp == sqliteinth.SAVEPOINT_ROLLBACK || eOp == sqliteinth.SAVEPOINT_RELEASE);
                    Debug.Assert(db.nStatement > 0);
                    Debug.Assert(this.iStatement == (db.nStatement + db.nSavepoint));
                    for (i = 0; i < db.BackendCount; i++)
                    {
                        SqlResult rc2 = SqlResult.SQLITE_OK;
                        Btree pBt = db.Backends[i].BTree;
                        if (pBt != null)
                        {
                            if (eOp == sqliteinth.SAVEPOINT_ROLLBACK)
                            {
                                rc2 = pBt.sqlite3BtreeSavepoint(sqliteinth.SAVEPOINT_ROLLBACK, iSavepoint);
                            }
                            if (rc2 == SqlResult.SQLITE_OK)
                            {
                                rc2 = pBt.sqlite3BtreeSavepoint(sqliteinth.SAVEPOINT_RELEASE, iSavepoint);
                            }
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                rc = rc2;
                            }
                        }
                    }
                    db.nStatement--;
                    this.iStatement = 0;
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        if (eOp == sqliteinth.SAVEPOINT_ROLLBACK)
                        {
                            rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_ROLLBACK, iSavepoint);
                        }
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_RELEASE, iSavepoint);
                        }
                    }
                    ///
                    ///<summary>
                    ///If the statement transaction is being rolled back, also restore the 
                    ///database handles deferred constraint counter to the value it had when 
                    ///the statement transaction was opened.  
                    ///</summary>
                    if (eOp == sqliteinth.SAVEPOINT_ROLLBACK)
                    {
                        db.nDeferredCons = this.nStmtDefCons;
                    }
                }
                return rc;
            }
            public SqlResult sqlite3VdbeCheckFk(int deferred)
            {
                Connection db = this.db;
                if ((deferred != 0 && db.nDeferredCons > 0) || (0 == deferred && this.nFkConstraint > 0))
                {
                    this.rc = SqlResult.SQLITE_CONSTRAINT;
                    this.errorAction = OnConstraintError.OE_Abort;
                    malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "foreign key constraint failed");
                    return SqlResult.SQLITE_ERROR;
                }
                return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3VdbeHalt()
            {
                SqlResult rc;
                ///
                ///<summary>
                ///Used to store transient return codes 
                ///</summary>
                Connection db = this.db;
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
                if (this.magic != VdbeMagic.VDBE_MAGIC_RUN)
                {
                    return SqlResult.SQLITE_OK;
                }
                vdbeaux.checkActiveVdbeCnt(db);
                ///
                ///<summary>
                ///No commit or rollback needed if the program never started 
                ///</summary>
                if (this.currentOpCodeIndex >= 0)
                {
                    SqlResult mrc;
                    ///
                    ///<summary>
                    ///Primary error code from p.rc 
                    ///</summary>
                    int eStatementOp = 0;
                    bool isSpecialError = false;
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
                    mrc = this.rc & (SqlResult)0xff;
                    Debug.Assert(this.rc != SqlResult.SQLITE_IOERR_BLOCKED);
                    ///
                    ///<summary>
                    ///This error no longer exists 
                    ///</summary>
                    isSpecialError = mrc == SqlResult.SQLITE_NOMEM || mrc == SqlResult.SQLITE_IOERR || mrc == SqlResult.SQLITE_INTERRUPT || mrc == SqlResult.SQLITE_FULL;
                    if (isSpecialError)
                    {
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
                        if (!this.readOnly || mrc != SqlResult.SQLITE_INTERRUPT)
                        {
                            if ((mrc == SqlResult.SQLITE_NOMEM || mrc == SqlResult.SQLITE_FULL) && this.usesStmtJournal)
                            {
                                eStatementOp = sqliteinth.SAVEPOINT_ROLLBACK;
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///We are forced to roll back the active transaction. Before doing
                                ///so, abort any other statements this handle currently has active.
                                ///
                                ///</summary>
                                vdbeaux.invalidateCursorsOnModifiedBtrees(db);
                                Sqlite3.sqlite3RollbackAll(db);
                                Sqlite3.sqlite3CloseSavepoints(db);
                                db.autoCommit = 1;
                            }
                        }
                    }
                    ///
                    ///<summary>
                    ///Check for immediate foreign key violations. 
                    ///</summary>
                    if (this.rc == SqlResult.SQLITE_OK)
                    {
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

                        if (this.rc == SqlResult.SQLITE_OK || (this.errorAction == OnConstraintError.OE_Fail && !isSpecialError))
                        {
                            rc = this.sqlite3VdbeCheckFk(1);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                if (Sqlite3.NEVER(this.readOnly))
                                {
                                    this.sqlite3VdbeLeave();
                                    return SqlResult.SQLITE_ERROR;
                                }
                                rc = SqlResult.SQLITE_CONSTRAINT;
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="The auto">commit flag is true, the vdbe program was successful </param>
                                ///<param name="or hit an 'OR FAIL' constraint and there are no deferred foreign">or hit an 'OR FAIL' constraint and there are no deferred foreign</param>
                                ///<param name="key constraints to hold up the transaction. This means a commit ">key constraints to hold up the transaction. This means a commit </param>
                                ///<param name="is required. ">is required. </param>
                                rc = vdbeaux.vdbeCommit(db, this);
                            }
                            if (rc == SqlResult.SQLITE_BUSY && this.readOnly)
                            {
                                this.sqlite3VdbeLeave();
                                return SqlResult.SQLITE_BUSY;
                            }
                            else
                                if (rc != SqlResult.SQLITE_OK)
                                {
                                    this.rc = rc;
                                    Sqlite3.sqlite3RollbackAll(db);
                                }
                                else
                                {
                                    db.nDeferredCons = 0;
                                    build.sqlite3CommitInternalChanges(db);
                                }
                        }
                        else
                        {
                            Sqlite3.sqlite3RollbackAll(db);
                        }
                        db.nStatement = 0;
                    }
                    else
                        if (eStatementOp == 0)
                        {
                            if (this.rc == SqlResult.SQLITE_OK || this.errorAction == OnConstraintError.OE_Fail)
                            {
                                eStatementOp = sqliteinth.SAVEPOINT_RELEASE;
                            }
                            else
                                if (this.errorAction == OnConstraintError.OE_Abort)
                                {
                                    eStatementOp = sqliteinth.SAVEPOINT_ROLLBACK;
                                }
                                else
                                {
                                    vdbeaux.invalidateCursorsOnModifiedBtrees(db);
                                    Sqlite3.sqlite3RollbackAll(db);
                                    Sqlite3.sqlite3CloseSavepoints(db);
                                    db.autoCommit = 1;
                                }
                        }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If eStatementOp is non">zero, then a statement transaction needs to</param>
                    ///<param name="be committed or rolled back. Call sqlite3VdbeCloseStatement() to">be committed or rolled back. Call sqlite3VdbeCloseStatement() to</param>
                    ///<param name="do so. If this operation returns an error, and the current statement">do so. If this operation returns an error, and the current statement</param>
                    ///<param name="error code is SqlResult.SQLITE_OK or SQLITE_CONSTRAINT, then promote the">error code is SqlResult.SQLITE_OK or SQLITE_CONSTRAINT, then promote the</param>
                    ///<param name="current statement error code.">current statement error code.</param>
                    ///<param name=""></param>
                    if (eStatementOp != 0)
                    {
                        rc = this.sqlite3VdbeCloseStatement(eStatementOp);
                        if (rc != 0)
                        {
                            if (this.rc == SqlResult.SQLITE_OK || this.rc == SqlResult.SQLITE_CONSTRAINT)
                            {
                                this.rc = rc;
                                db.sqlite3DbFree(ref this.zErrMsg);
                                this.zErrMsg = null;
                            }
                            vdbeaux.invalidateCursorsOnModifiedBtrees(db);
                            Sqlite3.sqlite3RollbackAll(db);
                            Sqlite3.sqlite3CloseSavepoints(db);
                            db.autoCommit = 1;
                        }
                    }
                    ///
                    ///<summary>
                    ///If this was an INSERT, UPDATE or DELETE and no statement transaction
                    ///</summary>
                    ///<param name="has been rolled back, update the database connection change">counter.</param>
                    ///<param name=""></param>
                    if (this.changeCntOn)
                    {
                        if (eStatementOp != sqliteinth.SAVEPOINT_ROLLBACK)
                        {
                            vdbeaux.sqlite3VdbeSetChanges(db, this.nChange);
                        }
                        else
                        {
                            vdbeaux.sqlite3VdbeSetChanges(db, 0);
                        }
                        this.nChange = 0;
                    }
                    ///
                    ///<summary>
                    ///Rollback or commit any schema changes that occurred. 
                    ///</summary>
                    if (this.rc != SqlResult.SQLITE_OK && (db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
                    {
                        build.sqlite3ResetInternalSchema(db, -1);
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
                if (this.currentOpCodeIndex >= 0)
                {
                    db.activeVdbeCnt--;
                    if (!this.readOnly)
                    {
                        db.writeVdbeCnt--;
                    }
                    Debug.Assert(db.activeVdbeCnt >= db.writeVdbeCnt);
                }
                this.magic = VdbeMagic.VDBE_MAGIC_HALT;
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
                if (db.autoCommit != 0)
                {
                    sqliteinth.sqlite3ConnectionUnlocked(db);
                }
                Debug.Assert(db.activeVdbeCnt > 0 || db.autoCommit == 0 || db.nStatement == 0);
                return (this.rc == SqlResult.SQLITE_BUSY ? SqlResult.SQLITE_BUSY : SqlResult.SQLITE_OK);
            }
            public void sqlite3VdbeResetStepResult()
            {
                this.rc = SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3VdbeReset()
            {
                Connection db;
                db = this.db;
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
                if (this.currentOpCodeIndex >= 0)
                {
                    //if ( p.zErrMsg != 0 ) // Always exists under C#
                    {
                        Sqlite3.sqlite3BeginBenignMalloc();
                        vdbemem_cs.sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg == null ? "" : this.zErrMsg, SqliteEncoding.UTF8, Sqlite3.SQLITE_TRANSIENT);
                        Sqlite3.sqlite3EndBenignMalloc();
                        db.errCode = this.rc;
                        db.sqlite3DbFree(ref this.zErrMsg);
                        this.zErrMsg = "";
                        //else if ( p.rc != 0 )
                        //{
                        //  utilc.sqlite3Error( db, p.rc, 0 );
                        //}
                        //else
                        //{
                        //  utilc.sqlite3Error( db, SqlResult.SQLITE_OK, 0 );
                        //}
                    }
                    if (this.runOnlyOnce != 0)
                        this.expired = true;
                }
                else
                    if (this.rc != 0 && this.expired)
                    {
                        ///
                        ///<summary>
                        ///The expired flag was set on the VDBE before the first call
                        ///to sqlite3_step(). For consistency (since sqlite3_step() was
                        ///called), set the database error in this case as well.
                        ///
                        ///</summary>
                        utilc.sqlite3Error(db, this.rc, 0);
                        vdbemem_cs.sqlite3ValueSetStr(db.pErr, -1, this.zErrMsg, SqliteEncoding.UTF8, Sqlite3.SQLITE_TRANSIENT);
                        db.sqlite3DbFree(ref this.zErrMsg);
                        this.zErrMsg = "";
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
                this.magic = VdbeMagic.VDBE_MAGIC_INIT;
                return this.rc & db.errMask;
            }
            public Connection sqlite3VdbeDb()
            {
                return this.db;
            }
            public sqlite3_value sqlite3VdbeGetValue(int iVar, u8 aff)
            {
                Debug.Assert(iVar > 0);
                if (this != null)
                {
                    Mem pMem = this.aVar[iVar - 1];
                    if (0 == (pMem.flags & MemFlags.MEM_Null))
                    {
                        sqlite3_value pRet = vdbemem_cs.sqlite3ValueNew(this.db);
                        if (pRet != null)
                        {
                            vdbemem_cs.sqlite3VdbeMemCopy((Mem)pRet, pMem);
                            Sqlite3.sqlite3ValueApplyAffinity(pRet, (char)aff, SqliteEncoding.UTF8);
                            Sqlite3.sqlite3VdbeMemStoreType((Mem)pRet);
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
            public SqlResult Exec()
            {
                
                ///The program counter 
                Operation[] aOp = this.aOp;
                var lOp = this.lOp;
                /*
                Log.WriteHeader("Plan VdbeExec");
                Log.Indent();
                foreach(var item in lOp) {
                    Log.WriteLine(item.ToString(this));
                }
                Log.WriteHeader("---");
				
                 */
                try
                {
                    ///Copy of p.aOp 
                    Operation pOp;
                    ///Current operation 
                    ///Value to return 
                    rc = SqlResult.SQLITE_OK;

                    ///The database 
                    resetSchemaOnFault = 0;
                    ///Reset schema after an error if positive 
                    encoding = sqliteinth.ENC(db);
                    ///The database encoding 
#if !SQLITE_OMIT_PROGRESS_CALLBACK
                    bool checkProgress;
                    ///True if progress callbacks are enabled 
                    int nProgressOps = 0;
                    ///Opcodes executed since progress callback. 
#endif
                    Mem[] aMem = this.aMem;
                    ///Copy of p.aMem 
                    
                    ///3rd input operand 
                    pOut = null;
                    ///Output operand 
                    ///Result of last  OpCode.OP_Compare operation 
                    
                    lastRowid = db.lastRowid;
                    ///Saved value of the last insert ROWID 
#if VDBE_PROFILE
																																																																																		u64 start;                   /* CPU clock count at start of opcode */
int origPc;                  /* Program counter at start of opcode */
#endif
                    ///<summary>
                    ///INSERT STACK UNION HERE **
                    ///</summary>
                    Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_RUN);
                    ///
                    ///<summary>
                    ///sqlite3_step() verifies this 
                    ///</summary>
                    this.sqlite3VdbeEnter();
                    if (this.rc == SqlResult.SQLITE_NOMEM)
                    {
                        ///
                        ///<summary>
                        ///This happens if a malloc() inside a call to sqlite3_column_text() or
                        ///sqlite3_column_text16() failed.  
                        ///</summary>
                        goto no_mem;
                    }
                    Debug.Assert(this.rc == SqlResult.SQLITE_OK || this.rc == SqlResult.SQLITE_BUSY);
                    this.rc = SqlResult.SQLITE_OK;
                    Debug.Assert(this.explain == 0);
                    this.pResultSet = null;
                    db.busyHandler.nBusy = 0;
                    if (db.u1.isInterrupted)
                        goto abort_due_to_interrupt;
                    //CHECK_FOR_INTERRUPT;
#if TRACE
																																																																																		sqlite3VdbeIOTraceSql( p );
#endif
#if !SQLITE_OMIT_PROGRESS_CALLBACK
                    checkProgress = db.xProgress != null;
#endif
#if SQLITE_DEBUG
																																																																																		      Sqlite3.sqlite3BeginBenignMalloc();
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

                    RuntimeException exp = RuntimeException.noop;
                    
                    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------					///

                    #region MAIN CPU LOOP
                    for (opcodeIndex = this.currentOpCodeIndex; rc == SqlResult.SQLITE_OK; opcodeIndex++)
                    {
                        Debug.Assert(opcodeIndex >= 0 && opcodeIndex < this.nOp);
                        //      if ( db.mallocFailed != 0 ) goto no_mem;
#if VDBE_PROFILE
																																																																																																											origPc = pc;
start = sqlite3Hwtime();
#endif
                        pOp = lOp[opcodeIndex];
                        ///Only allow tracing if SQLITE_DEBUG is defined.
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
                        ///Check to see if we need to simulate an interrupt.  This only happens
                        ///if we have a special test build.
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
                        ///Call the progress callback if it is configured and the required number
                        ///of VDBE ops have been executed (either since this invocation of
                        ///sqlite3VdbeExec() or since last time the progress callback was called).
                        ///<param name="If the progress callback returns non">zero, exit the virtual machine with</param>
                        ///<param name="a return code SQLITE_ABORT.">a return code SQLITE_ABORT.</param>
                        if (checkProgress)
                        {
                            if (db.nProgressOps == nProgressOps)
                            {
                                int prc;
                                prc = db.xProgress(db.pProgressArg);
                                if (prc != 0)
                                {
                                    rc = SqlResult.SQLITE_INTERRUPT;
                                    goto vdbe_error_halt;
                                }
                                nProgressOps = 0;
                            }
                            nProgressOps++;
                        }
#endif
                        #endregion
                        ///<param name="On any opcode with the "out2">prerelase" tag, free any</param>
                        ///<param name="external allocations out of mem[p2] and set mem[p2] to be">external allocations out of mem[p2] and set mem[p2] to be</param>
                        ///<param name="an undefined integer.  Opcodes will either fill in the integer">an undefined integer.  Opcodes will either fill in the integer</param>
                        ///<param name="value or convert mem[p2] to a different type.">value or convert mem[p2] to a different type.</param>
                        Debug.Assert(pOp.opflags == Sqlite3.sqlite3OpcodeProperty[pOp.opcode]);
                        if (((int)pOp.opflags & Sqlite3.OPFLG_OUT2_PRERELEASE) != 0)
                        {
                            Debug.Assert(pOp.p2 > 0);
                            Debug.Assert(pOp.p2 <= this.nMem);
                            pOut = aMem[pOp.p2];
                            this.memAboutToChange(pOut);
                            pOut.sqlite3VdbeMemReleaseExternal();

                            pOut.Flags = MemFlags.MEM_Int;
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

                        CPU cpu = this;
                        cpu.vdbe = this;
                        cpu.errorAction = errorAction;
                        cpu.opcodeIndex = opcodeIndex;
                        cpu.aMem = aMem;
                        cpu.db = db;
                        //before try the plugged handlers
                        exp = RuntimeException.noop;

                        foreach (var handler in handlers)
                        {
                            exp = handler(cpu, pOp.OpCode, pOp);
                            if (RuntimeException.OK == exp)
                            {
                                var clr = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("handled :" + handler.Method.DeclaringType.Name +" / "+ pOp.OpCode);
                                Console.ForegroundColor = clr;

                                
                                aMem = cpu.aMem;
                                errorAction = cpu.errorAction;
                                opcodeIndex = cpu.opcodeIndex;
                            }
                            if (RuntimeException.noop != exp)
                            {
                                break;
                            }
                        }

                        if (RuntimeException.noop == exp)

                            //Log.WriteLine(opcodeIndex.ToString().PadLeft(2)+":\t"+pOp.ToString(this));
                            switch (pOp.OpCode)
                            { 

                                ///Opcode: Noop * * * * *
                                ///
                                ///Do nothing.  This instruction is often useful as a jump
                                ///destination.
                                ///The magic Explain opcode are only inserted when explain==2 (which
                                ///is to say when the EXPLAIN QUERY PLAN syntax is used.)
                                ///This opcode records information from the optimizer.  It is the
                                ///<param name="the same as a no">op.  This opcodesnever appears in a real VM program.</param>
                                ///<param name=""></param>
                                default:
                                    {
                                        ///This is really OpCode.OP_Noop and  OpCode.OP_Explain 
                                        Debug.Assert(pOp.OpCode == OpCode.OP_Noop || pOp.OpCode == OpCode.OP_Explain);
                                        break;
                                    }
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

                    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------					///

                    goto n;
                no_mem: exp = RuntimeException.no_mem;
                    goto n;
                abort_due_to_error: exp = RuntimeException.abort_due_to_error;
                    goto n;
                too_big: exp = RuntimeException.too_big;
                    goto n;
                vdbe_error_halt: exp = RuntimeException.vdbe_error_halt;
                    goto n;
                abort_due_to_interrupt: exp = RuntimeException.abort_due_to_interrupt;
                    goto n;
                vdbe_return: exp = RuntimeException.vdbe_return;

                n:
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
                    switch (exp)
                    {
                        case RuntimeException.too_big:
                            ///Jump to here if a string or blob larger than db.aLimit[SQLITE_LIMIT_LENGTH]
                            ///is encountered.
                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "string or blob too big");
                            rc = SqlResult.SQLITE_TOOBIG;
                            goto case RuntimeException.vdbe_error_halt;
                            break;
                        case RuntimeException.no_mem:
                            ///Jump to here if a malloc() fails.
                            //db.mallocFailed = 1;
                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "out of memory");
                            rc = SqlResult.SQLITE_NOMEM;
                            goto case RuntimeException.vdbe_error_halt;
                            break;
                        case RuntimeException.abort_due_to_error:
                            ///Jump to here for any other kind of fatal error.  The "rc" variable
                            ///should hold the error number.
                            ///
                            ///</summary>
                            //Debug.Assert( p.zErrMsg); /// Not needed in C#
                            //if ( db.mallocFailed != 0 ) rc = SQLITE_NOMEM;
                            if (rc != SqlResult.SQLITE_IOERR_NOMEM)
                            {
                                malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "%s", Sqlite3.sqlite3ErrStr(rc));
                            }
                            goto case RuntimeException.vdbe_error_halt;
                            break;
                        case RuntimeException.abort_due_to_interrupt:
                            ///Jump to here if the sqlite3_interrupt() API sets the interrupt
                            ///flag.
                            Debug.Assert(db.u1.isInterrupted);
                            rc = SqlResult.SQLITE_INTERRUPT;
                            this.rc = rc;
                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, Sqlite3.sqlite3ErrStr(rc));
                            goto case RuntimeException.vdbe_error_halt;
                            break;
                        case RuntimeException.vdbe_error_halt:
                            Debug.Assert(rc != 0);
                            this.rc = rc;
                            sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog != null);
                            io.sqlite3_log(rc, "statement aborts at %d: [%s] %s", opcodeIndex, this.zSql, this.zErrMsg);
                            this.sqlite3VdbeHalt();
                            //if ( rc == SQLITE_IOERR_NOMEM ) db.mallocFailed = 1;
                            rc = SqlResult.SQLITE_ERROR;
                            if (resetSchemaOnFault > 0)
                            {
                                build.sqlite3ResetInternalSchema(db, resetSchemaOnFault - 1);
                            }
                            goto case RuntimeException.vdbe_return;
                        case RuntimeException.vdbe_return:
                            db.lastRowid = lastRowid;
                            this.sqlite3VdbeLeave();
                            return rc;
                        ///<summary>
                        ///This is the only way out of this procedure.  We have to
                        ///release the mutexes on btrees that were acquired at the
                        ///top. 
                        ///</summary>

                    }

                    return rc;
                }
                finally
                {
                    //Log.Unindent();
                }
            }



            ///<summary>
            /// Create a new virtual database engine.
            ///</summary>
            public static Vdbe Create(Connection db)
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
            public void VdbeComment(string zFormat, params object[] ap)
            {
            }

            //# define VdbeNoopComment(X)
            public void VdbeNoopComment(string zFormat, params object[] ap)
            {
            }
#endif

#endif


            List<Func<CPU, OpCode, VdbeOp, RuntimeException>> handlers = new List<Func<CPU, OpCode, VdbeOp, RuntimeException>>() {
                            _OpCode.Exec,
                            Engine.Op.BTree.Exec,
                            Engine.Op.ControlFlow.Exec,
                            Engine.Op.Math.Exec,
                            Engine.Op.Schema.Exec,
                            Engine.Op.Cast.Exec,
                            Engine.Op.Cursor.Exec,
                            Engine.Op.Idx.Exec,
                            Engine.Op.Others.Exec,
                            Engine.Op.VirtualTable.Exec,
                            Engine.Op.Crud.Exec,
                            Engine.Op.AutoVacuum.Exec,
                            Engine.Op.Transaction.Exec,
                            Engine.Op.TheRest.Exec
                    };


        }
        

    }

}
