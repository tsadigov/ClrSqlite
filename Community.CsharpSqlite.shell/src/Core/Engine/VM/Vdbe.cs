using System;
using System.Linq;
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
using sqlite_int64 = System.Int64;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
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
using yDbMask = System.Int32;
#endif
namespace Community.CsharpSqlite
{

    using System.Text;
    using sqlite3_value = Engine.Mem;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Metadata;
    using Vdbe = Engine.Vdbe;



    public enum RuntimeException
    {
        too_big, no_mem, abort_due_to_error, abort_due_to_interrupt, vdbe_error_halt,
        vdbe_return, OK,
        noop
    }

    namespace Engine
    {
        using Operation = VdbeOp;
        using Community.CsharpSqlite.tree;
        using Community.CsharpSqlite.Paging;
        using Community.CsharpSqlite.Utils;
        using Ast;
        using Core.Runtime;
        using CsharpSqlite.Core.Runtime;///
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
        public class Vdbe : CPU, ILinkedListNode<Vdbe>, IBackwardLinkedListNode<Vdbe>
        {
            bool dbg = false;
            public override void ShowDebugInfo()
            {
                if (dbg) return;
                dbg = true;

                //Console.Clear();
                vdbeaux.sqlite3VdbePrintSql(this);
                //vdbeaux.sqlite3VdbeList(this);
                for (int i = 0; i < aOp.Count(); i++)
                {
                    var clr = Console.ForegroundColor;
                    var bgclr = Console.BackgroundColor;
                    Console.ForegroundColor = i == this.opcodeIndex ? ConsoleColor.Red : ConsoleColor.White;
                    Console.BackgroundColor = i == this.currentOpCodeIndex ? ConsoleColor.DarkBlue : ConsoleColor.Black;
                    Console.WriteLine(i + ": " + lOp[i].ToString(this));
                    Console.ForegroundColor = clr;
                    Console.BackgroundColor = bgclr;

                }
                Console.WriteLine();
                Console.WriteLine();

                aMem.ForEach(
                    m => Console.WriteLine(m)
                    );

                Console.WriteLine();
                Console.WriteLine("Frame");
                tabcount = 0;
                pFrame.path(f => f.pParent).ForEach<VdbeFrame>(PrintFrame);

                //Console.ReadKey();
                dbg = false;
            }
            int tabcount = 0;
            string[] tab = new string[] { "\t", "\t\t", "\t\t\t", "\t\t\t\t", "\t\t\t\t\t" };
            public void PrintFrame(VdbeFrame frame)
            {
                Console.WriteLine();
                tabcount++;
                frame.aOp.ForEach(
                    op => {
                        var clr = Console.ForegroundColor;
                        var bgclr = Console.BackgroundColor;
                        //Console.ForegroundColor = i == frame.opcodeIndex ? ConsoleColor.Red : ConsoleColor.White;
                        //Console.BackgroundColor = i == frame.currentOpCodeIndex ? ConsoleColor.DarkMagenta : ConsoleColor.Black;
                        Console.WriteLine(tab[tabcount] + op.OpCode);
                        Console.ForegroundColor = clr;
                        Console.BackgroundColor = bgclr;
                    }
                );
            }

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
            public static List<Vdbe> s_instances = new List<Vdbe>();
            public Vdbe()
            {
                s_instances.Add(this);
            }
            ///
            ///<summary>
            ///The database connection that owns this statement 
            ///</summary>
            public Connection db;
            
            ///
            ///<summary>
            ///The memory locations 
            ///</summary>
            public List<Operation> lOp = new List<Operation>();
            
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
            ///
            ///<summary>
            ///Number of memory locations currently allocated 
            ///</summary>
            //public int nMem;

            ///
            ///<summary>
            ///Number of instructions in the program 
            ///</summary>
            //public int nOp;
            ///
            ///<summary>
            ///Number of slots allocated for aOp[] 
            ///</summary>
            //public int nOpAlloc;
            //** Space to hold the virtual machine's program */
            public List<Operation> aOp { get { return lOp; } set { lOp = value; } }


            #region refactored
            ///
            ///<summary>
            ///Number of labels used 
            ///</summary>
            //public int nLabelAlloc;
            //public int nLabel;
            ///
            ///<summary>
            ///Number of slots allocated in aLabel[] 
            ///</summary>
            #endregion

            ///<summary>
            ///Space to hold the labels 
            ///</summary>
            public List<int> aLabel=new List<i32>();
            
            
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
            public Vdbe pPrev { get; set; }
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
            public u8 runOnlyOnce { get; set; }
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
                //ct.nOp = nOp;
                //ct.nOpAlloc = nOpAlloc;
                ct.lOp = lOp;
                //ct.nLabel = nLabel;
                //ct.nLabelAlloc = nLabelAlloc;
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
                //ct.nMem = nMem;
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

            public int sqlite3VdbeAddOp3(OpCode op, int p1, BTreeProp p2, int p3)
            {
                return AddOpp3(op, p1, (int)p2, p3);
            }
            public int sqlite3VdbeAddOp3(OpCode op, int p1, int p2, BTreeProp p3) {
                return AddOpp3(op,p1,p2,(int)p3);
            }
            public int sqlite3VdbeAddOp3(TokenType op, int p1, int p2, int p3)
            {
                return AddOpp3((OpCode)op, p1, p2, p3);
            }
            public int AddOpp3(OpCode op, int p1, int p2, int p3)
            {   
                //var i = this.nOp;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                //Debug.Assert(op>0&&op<0xff);
                /*if (this.nOpAlloc <= i)
                {
                    if (this.growOpArray() != 0)
                    {
                        return 1;
                    }
                }
                this.nOp++;
                */
                var pOp = new Operation()
                {
                    OpCode = op,
                    p5 = 0,
                    p1 = p1,
                    p2 = p2,
                    p3 = p3,
                    p4type = P4Usage.P4_NOTUSED
                };
                pOp.p4.p = null;                
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
                return aOp.Count()-1;
            }

            public int sqlite3VdbeAddOp0(OpCode op)
            {
                return this.AddOpp3(op, 0, 0, 0);
            }
            public int sqlite3VdbeAddOp0(TokenType op)
            {//---------------------
                return this.sqlite3VdbeAddOp3(op, 0, 0, 0);
            }
            public int AddOpp1(OpCode op, int p1)
            {
                return this.AddOpp3(op, p1, 0, 0);
            }
            public int sqlite3VdbeAddOp2(OpCode op, int p1, bool b2)
            {
                return this.sqlite3VdbeAddOp2(op, p1, (int)(b2 ? 1 : 0));
            }
            public int sqlite3VdbeAddOp2(TokenType op, int p1, int p2)
            {
                return this.sqlite3VdbeAddOp3(op, p1, p2, 0);
            }
            public int sqlite3VdbeAddOp2(OpCode op, int p1, int p2)
            {
                return this.AddOpp3(op, p1, p2, 0);
            }
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, i32 pP4, P4Usage p4type)
            {
                return sqlite3VdbeAddOp4(op, p1, p2, p3, pP4, p4type);
            }
            public int sqlite3VdbeAddOp4(TokenType op, int p1, int p2, int p3, i32 pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.i = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }


            public int sqlite3VdbeAddOp4(TokenType op, int p1, int p2, int p3, StringBuilder pP4, P4Usage p4type)
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
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }



            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, string pP4, P4Usage p4type)
            {
                //      Debug.Assert( pP4 != null );
                union_p4 _p4 = new union_p4();
                _p4.z = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, byte[] pP4, P4Usage p4type)
            {
                Debug.Assert(op == OpCode.OP_Null || pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.z = Encoding.UTF8.GetString(pP4, 0, pP4.Length);
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, int[] pP4, P4Usage p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.ai = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, i64 pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pI64 = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, double pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pReal = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(TokenType op, int p1, int p2, int p3, FuncDef pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pFunc = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(OpCode opCode, yDbMask p1, yDbMask regAgg, yDbMask p2, FuncDef funcDef, P4Usage p4type)
            {
                return sqlite3VdbeAddOp4(opCode, p1, regAgg, p2, funcDef, p4type);
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, CollSeq pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pColl = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(TokenType op, int p1, int p2, int p3, KeyInfo pP4, P4Usage p4type)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pP4;
                int addr = this.sqlite3VdbeAddOp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, KeyInfo pP4, P4Usage p4type=P4Usage.P4_KEYINFO_HANDOFF)
            {
                union_p4 _p4 = new union_p4();
                _p4.pKeyInfo = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }

            public int sqlite3VdbeAddOp4(OpCode op, int p1, int p2, int p3, VTable pP4, P4Usage p4type)
            {
                Debug.Assert(pP4 != null);
                union_p4 _p4 = new union_p4();
                _p4.pVtab = pP4;
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, p4type);
                return addr;
            }
            public int sqlite3VdbeAddOp4Int(
                TokenType op,///The new opcode 
                int p1,///The P1 operand 
                int p2,///The P2 operand 
                int p3,///The P3 operand 
                int p4///The P4 operand as an integer 
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
                int addr = this.AddOpp3(op, p1, p2, p3);
                this.sqlite3VdbeChangeP4(addr, _p4, P4Usage.P4_INT32);
                return addr;
            }

            public int sqlite3VdbeMakeLabel()
            {   
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                aLabel.Add(-1);
                return  - aLabel.Count();
            }
            public void sqlite3VdbeResolveLabel(int x)
            {
                int j = -1 - x;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                Debug.Assert(j >= 0 && j < this.aLabel.Count());
                if (this.aLabel != null)
                {
                    this.aLabel[j] = this.aOp.Count();
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
                var aLabel = this.aLabel;
                this.readOnly = true;
                for (i = 0; i < this.aOp.Count(); i++)//  for(pOp=p->aOp, i=p->nOp-1; i>=0; i--, pOp++)
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
                        Debug.Assert(this.aOp.Count() - i >= 3);
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
                        Debug.Assert(-1 - pOp.p2 < this.aLabel.Count());
                        pOp.p2 = aLabel[-1 - pOp.p2];
                    }
                }
                this.db.DbFree(ref this.aLabel);
                pMaxFuncArgs = nMaxArgs;
            }
            public int sqlite3VdbeCurrentAddr()
            {
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                return this.aOp.Count();
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
                pnOp = this.aOp.Count();
                this.lOp = null;
                return lOp.ToArray();
            }
            public int sqlite3VdbeAddOpList(int nOp, VdbeOpList[] aOp)
            {
                int addr;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (this.aOp.Count() + nOp > this.aOp.Capacity && this.growOpArray() != 0)
                {
                    return 0;
                }
                addr = this.aOp.Count();
                if (Sqlite3.ALWAYS(nOp > 0))
                {
                    int i;
                    VdbeOpList pIn;
                    for (i = 0; i < nOp; i++)
                    {
                        pIn = aOp[i];
                        int p2 = pIn.p2;
                        //if (this.lOp[i + addr] == null)
                        var pOut = this.lOp[i + addr] = new VdbeOp()
                        {
                            opcode = pIn.opcode,
                            p1 = pIn.p1,
                            p2 = (p2 < 0 && (Sqlite3.sqlite3OpcodeProperty[pIn.opcode] & (OpFlag)Sqlite3.OPFLG_JUMP) != 0) ? addr + (-1 - p2) : p2,
                            p3 = pIn.p3,
                            p4type = P4Usage.P4_NOTUSED,
                            p5 = 0
                        };                    
                        pOut.p4.p = null;
#if SQLITE_DEBUG
																																																																																																																																															          pOut.zComment = null;
          if ( sqlite3VdbeAddopTrace )
          {
            sqlite3VdbePrintOp( null, i + addr, p.aOp[i + addr] );
          }
#endif
                    }
                    
                }
                return addr;
            }
            public void sqlite3VdbeChangeP1(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.aOp.Count() > addr)
                {
                    this.lOp[addr].p1 = val;
                }
            }
            public void sqlite3VdbeChangeP2(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.aOp.Count() > addr)
                {
                    this.lOp[addr].p2 = val;
                }
            }
            public void sqlite3VdbeChangeP3(int addr, int val)
            {
                Debug.Assert(this != null);
                Debug.Assert(addr >= 0);
                if (this.aOp.Count() > addr)
                {
                    this.lOp[addr].p3 = val;
                }
            }
            public void ChangeP5(u8 val)
            {
                Debug.Assert(this != null);
                if (this.lOp != null)
                {
                    Debug.Assert(this.aOp.Count() > 0);
                    this.lOp[this.aOp.Count() - 1].p5 = val;
                }
            }
            public void sqlite3VdbeChangeP5(OpFlag val)
            {
                ChangeP5((u8)val);
            }
            public void sqlite3VdbeChangeP5(OpCode val)
            {
                Debug.Assert(this != null);
                if (this.lOp != null)
                {
                    Debug.Assert(this.aOp.Count() > 0);
                    this.lOp[this.aOp.Count() - 1].p5 = (u8)val;
                }
            }
            public void sqlite3VdbeJumpHere(int addr)
            {
                Debug.Assert(addr >= 0);
                this.sqlite3VdbeChangeP2(addr, this.aOp.Count());
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
                Debug.Assert(this != null);
                var db = this.db;
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (this.lOp == null)
                {
                    if (n != P4Usage.P4_KEYINFO && n != P4Usage.P4_VTAB)
                    {
                        vdbeaux.freeP4(db, n, _p4);
                    }
                    return;
                }
                Debug.Assert(this.aOp.Count() > 0);
                Debug.Assert(addr < this.aOp.Count());
                if (addr < 0)
                {
                    addr = this.aOp.Count() - 1;
                }
                var pOp = this.lOp[addr];
                vdbeaux.freeP4(db, pOp.p4type, pOp.p4.p);
                pOp.p4.p = null;
                if (n == P4Usage.P4_INT32)
                {
                    ///Note: this cast is safe, because the origin data point was an int
                    ///that was cast to a (string ). 
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
                    int nField, nByte;
                    nField = _p4.pKeyInfo.nField;
                    //nByte = sizeof(*pKeyInfo) + (nField-1)*sizeof(pKeyInfo.aColl[0]) + nField;
                    var pKeyInfo = pOp.p4.pKeyInfo = new KeyInfo();                    
                    
                    if (pKeyInfo != null)
                    {                        
                        pKeyInfo = _p4.pKeyInfo.Copy();
                        pOp.p4type = P4Usage.P4_KEYINFO;
                    }
                    else
                        pOp.p4type = P4Usage.P4_NOTUSED;

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
                ///C89 specifies that the constant "dummy" will be initialized to all
                ///zeros, which is correct.  MSVC generates a warning, nevertheless. 
                Debug.Assert(this.magic == VdbeMagic.VDBE_MAGIC_INIT);
                if (addr < 0)
                {
#if SQLITE_OMIT_TRACE
																																																																																																																				if( p.nOp==0 ) return dummy;
#endif
                    addr = this.aOp.Count() - 1;
                }
                Debug.Assert((addr >= 0 && addr < this.aOp.Count())///
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
                    ///The first time a column affinity string for a particular index is
                    ///required, it is allocated and populated here. It is then stored as
                    ///a member of the Index structure for subsequent use.
                    ///
                    ///The column affinity string will eventually be deleted by
                    ///sqliteDeleteIndex() when the Index structure itself is cleaned
                    ///up.
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
                        pIdx_zColAff.Append(pTab.aCol[pIdx.ColumnIdx[n]].affinity);
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
            
            public void codegenExprCodeGetColumnOfTable(
                Table pTab,///The table containing the value 
                int iTabCur,///The cursor for this table 
                int iCol,///Index of the column to extract 
                int regOut///Extract the value into this register 
            )
            {
                var facade = new VdbeFacade(this);
                if (iCol < 0 || iCol == pTab.iPKey)
                {
                    this.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iTabCur, regOut);
                }
                else
                {
                    OpCode op = pTab.IsVirtual() ? OpCode.OP_VColumn : OpCode.OP_Column;
                    this.AddOpp3(op, iTabCur, iCol, regOut);
                }
                if (iCol >= 0)
                {
                    facade.codegenColumnDefault(pTab, iCol, regOut);
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
            {/*
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
                    Array.Resize(ref this.aOp, nNew);*/
                return (this.aOp != null ? SqlResult.SQLITE_OK : SqlResult.SQLITE_NOMEM);
                //  return (pNew ? SqlResult.SQLITE_OK : SQLITE_NOMEM);
            }
            public void codegenAddParseSchemaOp(int iDb, string zWhere)
            {
                int j;
                int addr = this.AddOpp3(OpCode.OP_ParseSchema, iDb, 0, 0);
                this.sqlite3VdbeChangeP4(addr, zWhere, P4Usage.P4_DYNAMIC);
                for (j = 0; j < this.db.BackendCount; j++)
                    vdbeaux.markUsed(this, j);
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
                ///There should be at least one opcode.
                Debug.Assert(this.aOp.Count() > 0);
                ///Set the magic to VdbeMagic.VDBE_MAGIC_RUN sooner rather than later. 
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
                Debug.Assert(idx < this.nResColumn);
                Debug.Assert((int)var < Vdbe.COLNAME_N);
                
                Debug.Assert(this.aColName != null);
                var colName = this.aColName[idx + (int)var * this.nResColumn];
                var rc = colName.Set(zName, -1, SqliteEncoding.UTF8, xDel);
                Debug.Assert(rc != 0 || null == zName || (colName.flags & MemFlags.MEM_Term) != 0);
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
                                db.DbFree(ref this.zErrMsg);
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
                        db.DbFree(ref this.zErrMsg);
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
                    db.DbFree(ref this.zErrMsg);
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
            public sqlite3_value GetValue(int iVar, u8 aff)
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

            //int opcodeIndex = 0;
            public SqlResult sqlite3VdbeExec()
            {

                ///The program counter 
                var aOp = this.aOp;
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
                    //var aMem = this.aMem;
                    ///Copy of p.aMem 
                    Mem pIn1 = null;
                    ///1st input operand 
                    Mem pIn2 = null;
                    ///2nd input operand 
                    Mem pIn3 = null;
                    ///3rd input operand 
                    pOut = null;
                    ///Output operand 
                    ///Result of last  OpCode.OP_Compare operation 
                    int[] aPermute = null;
                    ///Permutation of columns for  OpCode.OP_Compare 
                    i64 lastRowid = db.lastRowid;
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
                    for (this.opcodeIndex = this.currentOpCodeIndex; rc == SqlResult.SQLITE_OK; this.opcodeIndex++)
                    {
                        Debug.Assert(opcodeIndex >= 0 && opcodeIndex < this.aOp.Count());
                        //      if ( db.mallocFailed != 0 ) goto no_mem;
#if VDBE_PROFILE
																																																																																																											origPc = pc;
start = sqlite3Hwtime();
#endif
                        ///Copy of p.aOp 
                        var pOp = lOp[opcodeIndex];
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
                            Debug.Assert(pOp.p2 <= this.aMem.Count());
                            pOut = aMem[pOp.p2];
                            this.memAboutToChange(pOut);
                            pOut.sqlite3VdbeMemReleaseExternal();

                            pOut.Flags = MemFlags.MEM_Int;
                        }
                        ///Sanity checking on other operands 
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
                                Console.WriteLine("handled :" + pOp.OpCode);
                                aMem = cpu.aMem;
                                errorAction = cpu.errorAction;

                            }
                            if (RuntimeException.noop != exp)
                            {
                                break;
                            }
                        }

                        if (RuntimeException.OK == exp)
                            continue;
                        else if (RuntimeException.noop != exp)
                            goto n;
                        else

                            //Log.WriteLine(opcodeIndex.ToString().PadLeft(2)+":\t"+pOp.ToString(this));
                            switch (pOp.OpCode)
                            {
                                ///<summary>
                                ///
                                ///What follows is a massive switch statement where each case implements a
                                ///separate instruction in the virtual machine.  If we follow the usual
                                ///indentation conventions, each case should be indented by 6 spaces.  But
                                ///that is a lot of wasted space on the left margin.  So the code within
                                ///</summary>
                                ///the switch statement will break with convention and be flush">left. Another</param>
                                ///big comment (similar to this one) will mark the point in the code where</param>
                                ///we transition back to normal indentation.</param>
                                ///
                                ///The formatting of each case is important.  The makefile for SQLite</param>
                                ///generates two C files "opcodes.h" and "opcodes.c" by scanning this</param>
                                ///file looking for lines that begin with "case  OpCode.OP_".  The opcodes.h files</param>
                                ///will be filled with #defines that give unique integer values to each</param>
                                ///opcode and the opcodes.c file is filled with an array of strings where</param>
                                ///each string is the symbolic name for the corresponding opcode.  If the</param>
                                ///case statement is followed by a comment of the form "/# same as ... #/"</param>
                                ///that comment is used to determine the particular value of the opcode.</param>
                                ///
                                ///Other keywords in the comment that follows each case are used to</param>
                                ///construct the OPFLG_INITIALIZER value that initializes opcodeProperty[].</param>
                                ///Keywords include: in1, in2, in3, ref2_prerelease, ref2, ref3.  See</param>
                                ///mkopcodeh.awk script for additional information.</param>
                                ///
                                ///Documentation about VDBE opcodes is generated by scanning this file">Documentation about VDBE opcodes is generated by scanning this file</param>
                                ///for lines of that contain "Opcode:".  That line and all subsequent</param>
                                ///comment lines are used in the generation of the opcode.html documentation</param>
                                ///file.</param>
                                ///
                                ///SUMMARY:</param>
                                ///
                                ///Formatting is important to scripts that scan this file.</param>
                                ///Do not deviate from the formatting style currently in use.</param>
                                ///
                                /// 

                                ///
                                ///<summary>
                                ///Opcode:  HaltIfNull  P1 P2 P3 P4 *
                                ///
                                ///Check the value in register P3.  If it is NULL then Halt using
                                ///parameter P1, P2, and P4 as if this were a Halt instruction.  If the
                                ///</summary>
                                ///<param name="value in register P3 is not NULL, then this routine is a no">op.</param>
                                ///<param name=""></param>
                                case OpCode.OP_HaltIfNull:
                                    {
                                        ///in3 
                                        pIn3 = aMem[pOp.p3];
                                        if ((pIn3.flags & MemFlags.MEM_Null) == 0)
                                            break;
                                        ///Fall through into  OpCode.OP_Halt 
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
                                ///or sqlite3_finalize().  For a normal halt, this should be SqlResult.SQLITE_OK (0).
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
                                case OpCode.OP_Halt:
                                    {
                                        pIn3 = aMem[pOp.p3];
                                        if (pOp.p1 == (int)SqlResult.SQLITE_OK && this.pFrame != null)
                                        {
                                            ///<param name="Halt the sub">program. Return control to the parent frame. </param>
                                            VdbeFrame pFrame = this.pFrame;
                                            this.pFrame = pFrame.pParent;
                                            this.nFrame--;
                                            vdbeaux.sqlite3VdbeSetChanges(db, this.nChange);
                                            opcodeIndex = pFrame.sqlite3VdbeFrameRestore();
                                            lastRowid = db.lastRowid;
                                            if (pOp.p2 == (int)OnConstraintError.OE_Ignore)
                                            {
                                                ///<param name="Instruction pc is the  OpCode.OP_Program that invoked the sub">program </param>
                                                ///<param name="currently being halted. If the p2 instruction of this  OpCode.OP_Halt">currently being halted. If the p2 instruction of this  OpCode.OP_Halt</param>
                                                ///<param name="instruction is set to OnConstraintError.OE_Ignore, then the sub">program is throwing</param>
                                                ///<param name="an IGNORE exception. In this case jump to the address specified">an IGNORE exception. In this case jump to the address specified</param>
                                                ///<param name="as the p2 of the calling  OpCode.OP_Program.  ">as the p2 of the calling  OpCode.OP_Program.  </param>
                                                opcodeIndex = this.lOp[opcodeIndex].p2 - 1;
                                            }
                                            lOp = this.lOp;
                                            aMem = this.aMem;
                                            break;
                                        }
                                        this.rc = (SqlResult)pOp.p1;
                                        this.errorAction = (OnConstraintError)pOp.p2;
                                        this.currentOpCodeIndex = opcodeIndex;
                                        if (pOp.p4.z != null)
                                        {
                                            Debug.Assert(this.rc != SqlResult.SQLITE_OK);
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "%s", pOp.p4.z);
                                            sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog != null);
                                            io.sqlite3_log(pOp.p1, "abort at %d in [%s]: %s", opcodeIndex, this.zSql, pOp.p4.z);
                                        }
                                        else
                                            if (this.rc != 0)
                                        {
                                            sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog != null);
                                            io.sqlite3_log(pOp.p1, "constraint failed at %d in [%s]", opcodeIndex, this.zSql);
                                        }
                                        rc = this.sqlite3VdbeHalt();
                                        Debug.Assert(rc == SqlResult.SQLITE_BUSY || rc == SqlResult.SQLITE_OK || rc == SqlResult.SQLITE_ERROR);
                                        if (rc == SqlResult.SQLITE_BUSY)
                                        {
                                            this.rc = rc = SqlResult.SQLITE_BUSY;
                                        }
                                        else
                                        {
                                            Debug.Assert(rc == SqlResult.SQLITE_OK || this.rc == SqlResult.SQLITE_CONSTRAINT);
                                            Debug.Assert(rc == SqlResult.SQLITE_OK || db.nDeferredCons > 0);
                                            rc = this.rc != 0 ? SqlResult.SQLITE_ERROR : SqlResult.SQLITE_DONE;
                                        }
                                        goto vdbe_return;
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
                                case OpCode.OP_Permutation:
                                    {
                                        Debug.Assert(pOp.p4type == P4Usage.P4_INTARRAY);
                                        Debug.Assert(pOp.p4.ai != null);
                                        aPermute = pOp.p4.ai;
                                        break;
                                    }
                               
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
                                case OpCode.OP_MakeRecord:
                                    {
                                        ///A buffer to hold the data for the new record 
                                        byte[] zNewRecord;
                                        Mem pRec;
                                        ///The new record 
                                        u64 nData;
                                        ///Number of bytes of data space 
                                        int nHdr;
                                        ///Number of bytes of header space 
                                        i64 nByte;
                                        ///Data space required for this record 
                                        int nZero;
                                        ///Number of zero bytes at the end of the record 
                                        int nVarint;
                                        ///Number of bytes in a varint 
                                        u32 serial_type;
                                        ///Type field 
                                        //Mem pData0;            /* First field to be combined into the record */
                                        //Mem pLast;             /* Last field of the record */
                                        int nField;
                                        ///Number of fields in the record 
                                        string zAffinity;
                                        ///The affinity string for the record 
                                        int file_format;
                                        ///File format to use for encoding 
                                        int i;
                                        ///Space used in zNewRecord[] 
                                        int len;
                                        ///Length of a field 
                                        ///Assuming the record contains N fields, the record format looks
                                        ///like this:
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
                                        nData = 0;
                                        ///Number of bytes of data space 
                                        nHdr = 0;
                                        ///Number of bytes of header space 
                                        nZero = 0;
                                        ///Number of zero bytes at the end of the record 
                                        nField = pOp.p1;
                                        zAffinity = (pOp.p4.z == null || pOp.p4.z.Length == 0) ? "" : pOp.p4.z;
                                        Debug.Assert(nField > 0 && pOp.p2 > 0 && pOp.p2 + nField <= this.aMem.Count() + 1);
                                        //pData0 = aMem[nField];
                                        nField = pOp.p2;
                                        //pLast =  pData0[nField - 1];
                                        file_format = this.minWriteFileFormat;
                                        ///Identify the output register 
                                        Debug.Assert(pOp.p3 < pOp.p1 || pOp.p3 >= pOp.p1 + pOp.p2);
                                        pOut = aMem[pOp.p3];
                                        this.memAboutToChange(pOut);
                                        ///Loop through the elements that will make up the record to figure
                                        ///out how much space is required for the new record.
                                        //for (pRec = pData0; pRec <= pLast; pRec++)
                                        for (int pD0 = 0; pD0 < nField; pD0++)
                                        {
                                            pRec = this.aMem[pOp.p1 + pD0];
                                            Debug.Assert(pRec.memIsValid());
                                            if (pD0 < zAffinity.Length && zAffinity[pD0] != '\0')
                                            {
                                                pRec.applyAffinity((char)zAffinity[pD0], encoding);
                                            }
                                            if ((pRec.flags & MemFlags.MEM_Zero) != 0 && pRec.CharacterCount > 0)
                                            {
                                                pRec.sqlite3VdbeMemExpandBlob();
                                            }
                                            serial_type = vdbeaux.sqlite3VdbeSerialType(pRec, file_format);
                                            len = (int)vdbeaux.sqlite3VdbeSerialTypeLen(serial_type);
                                            nData += (u64)len;
                                            nHdr += utilc.sqlite3VarintLen(serial_type);
                                            if ((pRec.flags & MemFlags.MEM_Zero) != 0)
                                            {
                                                ///
                                                ///<summary>
                                                ///</summary>
                                                ///<param name="Only pure zero">filled BLOBs can be input to this Opcode.</param>
                                                ///<param name="We do not allow blobs with a prefix and a zero">filled tail. </param>
                                                nZero += pRec.u.nZero;
                                            }
                                            else
                                                if (len != 0)
                                            {
                                                nZero = 0;
                                            }
                                        }
                                        ///
                                        ///<summary>
                                        ///Add the initial header varint and total the size 
                                        ///</summary>
                                        nHdr += nVarint = utilc.sqlite3VarintLen((u64)nHdr);
                                        if (nVarint < utilc.sqlite3VarintLen((u64)nHdr))
                                        {
                                            nHdr++;
                                        }
                                        nByte = (i64)((u64)nHdr + nData - (u64)nZero);
                                        if (nByte > db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
                                        {
                                            return (SqlResult)ColumnResult.too_big;
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
                                        zNewRecord = malloc_cs.sqlite3Malloc((int)nByte);
                                        // (u8 )pOut.z;
                                        ///
                                        ///<summary>
                                        ///Write the record 
                                        ///</summary>
                                        i = utilc.putVarint32(zNewRecord, nHdr);
                                        for (int pD0 = 0; pD0 < nField; pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
                                        {
                                            pRec = this.aMem[pOp.p1 + pD0];
                                            serial_type = vdbeaux.sqlite3VdbeSerialType(pRec, file_format);
                                            i += utilc.putVarint32(zNewRecord, i, (int)serial_type);
                                            ///
                                            ///<summary>
                                            ///serial type 
                                            ///</summary>
                                        }
                                        for (int pD0 = 0; pD0 < nField; pD0++)//for (pRec = pData0; pRec <= pLast; pRec++)
                                        {
                                            ///
                                            ///<summary>
                                            ///serial data 
                                            ///</summary>
                                            pRec = this.aMem[pOp.p1 + pD0];
                                            i += (int)vdbeaux.sqlite3VdbeSerialPut(zNewRecord, i, (int)nByte - i, pRec, file_format);
                                        }
                                        //TODO -- Remove this  for testing Debug.Assert( i == nByte );
                                        Debug.Assert(pOp.p3 > 0 && pOp.p3 <= this.aMem.Count());
                                        pOut.zBLOB = zNewRecord;
                                        pOut.AsString = null;
                                        pOut.CharacterCount = (int)nByte;
                                        pOut.flags = MemFlags.MEM_Blob | MemFlags.MEM_Dyn;
                                        pOut.xDel = null;
                                        if (nZero != 0)
                                        {
                                            pOut.u.nZero = nZero;
                                            pOut.flags |= MemFlags.MEM_Zero;
                                        }
                                        pOut.enc = SqliteEncoding.UTF8;
                                        ///
                                        ///<summary>
                                        ///In case the blob is ever converted to text 
                                        ///</summary>
                                        Sqlite3.REGISTER_TRACE(this, pOp.p3, pOut);
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
                                        break;
                                    }

                                ///
                                ///<summary>
                                ///Opcode: Savepoint P1 * * P4 *
                                ///
                                ///Open, release or rollback the savepoint named by parameter P4, depending
                                ///on the value of P1. To open a new savepoint, P1==0. To release (commit) an
                                ///existing savepoint, P1==1, or to rollback an existing savepoint P1==2.
                                ///</summary>
                                case OpCode.OP_Savepoint:
                                    {                                        
                                        ///Name of savepoint 
                                        int nName;
                                        Savepoint pNew;
                                        Savepoint pSavepoint;
                                        Savepoint pTmp;
                                        int iSavepoint;
                                        int ii;
                                        var p1 = pOp.p1;
                                        var zName = pOp.p4.z;
                                        ///Assert that the p1 parameter is valid. Also that if there is no open
                                        ///transaction, then there cannot be any savepoints.
                                        Debug.Assert(db.pSavepoint == null || db.autoCommit == 0);
                                        Debug.Assert(p1 == sqliteinth.SAVEPOINT_BEGIN || p1 == sqliteinth.SAVEPOINT_RELEASE || p1 == sqliteinth.SAVEPOINT_ROLLBACK);
                                        Debug.Assert(db.pSavepoint != null || db.isTransactionSavepoint == 0);
                                        Debug.Assert(Sqlite3.checkSavepointCount(db) != 0);
                                        if (p1 == sqliteinth.SAVEPOINT_BEGIN)
                                        {
                                            if (db.writeVdbeCnt > 0)
                                            {
                                                ///A new savepoint cannot be created if there are active write
                                                ///statements (i.e. open read/write incremental blob handles).
                                                malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "cannot open savepoint - ", "SQL statements in progress");
                                                rc = SqlResult.SQLITE_BUSY;
                                            }
                                            else
                                            {
                                                nName = StringExtensions.Strlen30(zName);
#if !SQLITE_OMIT_VIRTUALTABLE
                                                ///This call is Ok even if this savepoint is actually a transaction
                                                ///savepoint (and therefore should not prompt xSavepoint()) callbacks.
                                                ///If this is a transaction savepoint being opened, it is guaranteed
                                                ///<param name="that the db">>aVTrans[] array is empty.  </param>
                                                Debug.Assert(db.autoCommit == 0 || db.nVTrans == 0);
                                                rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_BEGIN, db.nStatement + db.nSavepoint);
                                                if (rc != SqlResult.SQLITE_OK)
                                                    goto abort_due_to_error;
#endif
                                                ///Create a new savepoint structure. 
                                                pNew = new Savepoint();
                                                // sqlite3DbMallocRaw( db, sizeof( Savepoint ) + nName + 1 );
                                                if (pNew != null)
                                                {
                                                    //pNew.zName = (char )&pNew[1];
                                                    //memcpy(pNew.zName, zName, nName+1);
                                                    pNew.zName = zName;
                                                    ///If there is no open transaction, then mark this as a special
                                                    ///"transaction savepoint". 
                                                    if (db.autoCommit != 0)
                                                    {
                                                        db.autoCommit = 0;
                                                        db.isTransactionSavepoint = 1;
                                                    }
                                                    else
                                                    {
                                                        db.nSavepoint++;
                                                    }
                                                    ///Link the new savepoint into the database handle's list. 
                                                    pNew.pNext = db.pSavepoint;
                                                    db.pSavepoint = pNew;
                                                    pNew.nDeferredCons = db.nDeferredCons;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            iSavepoint = 0;
                                            ///Find the named savepoint. If there is no such savepoint, then an
                                            ///an error is returned to the user.  
                                            for (pSavepoint = db.pSavepoint; pSavepoint != null && !pSavepoint.zName.Equals(zName, StringComparison.InvariantCultureIgnoreCase); pSavepoint = pSavepoint.pNext)
                                            {
                                                iSavepoint++;
                                            }
                                            if (null == pSavepoint)
                                            {
                                                malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "no such savepoint: %s", zName);
                                                rc = SqlResult.SQLITE_ERROR;
                                            }
                                            else
                                                if (db.writeVdbeCnt > 0 || (p1 == sqliteinth.SAVEPOINT_ROLLBACK && db.activeVdbeCnt > 1))
                                            {
                                                ///It is not possible to release (commit) a savepoint if there are
                                                ///active write statements. It is not possible to rollback a savepoint
                                                ///if there are any active statements at all.
                                                malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "cannot %s savepoint - SQL statements in progress", (p1 == sqliteinth.SAVEPOINT_ROLLBACK ? "rollback" : "release"));
                                                rc = SqlResult.SQLITE_BUSY;
                                            }
                                            else
                                            {
                                                ///Determine whether or not this is a transaction savepoint. If so,
                                                ///and this is a RELEASE command, then the current transaction
                                                ///is committed.
                                                int isTransaction = (pSavepoint.pNext == null && db.isTransactionSavepoint != 0) ? 1 : 0;
                                                if (isTransaction != 0 && p1 == sqliteinth.SAVEPOINT_RELEASE)
                                                {
                                                    if ((rc = this.sqlite3VdbeCheckFk(1)) != SqlResult.SQLITE_OK)
                                                    {
                                                        goto vdbe_return;
                                                    }
                                                    db.autoCommit = 1;
                                                    if (this.sqlite3VdbeHalt() == SqlResult.SQLITE_BUSY)
                                                    {
                                                        this.currentOpCodeIndex = opcodeIndex;
                                                        db.autoCommit = 0;
                                                        this.rc = rc = SqlResult.SQLITE_BUSY;
                                                        goto vdbe_return;
                                                    }
                                                    db.isTransactionSavepoint = 0;
                                                    rc = this.rc;
                                                }
                                                else
                                                {
                                                    iSavepoint = db.nSavepoint - iSavepoint - 1;
                                                    for (ii = 0; ii < db.BackendCount; ii++)
                                                    {
                                                        rc = db.Backends[ii].BTree.sqlite3BtreeSavepoint(p1, iSavepoint);
                                                        if (rc != SqlResult.SQLITE_OK)
                                                        {
                                                            goto abort_due_to_error;
                                                        }
                                                    }
                                                    if (p1 == sqliteinth.SAVEPOINT_ROLLBACK && (db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
                                                    {
                                                        vdbeaux.sqlite3ExpirePreparedStatements(db);
                                                        build.sqlite3ResetInternalSchema(db, -1);
                                                        db.flags = (db.flags | SqliteFlags.SQLITE_InternChanges);
                                                    }
                                                }
                                                ///Regardless of whether this is a RELEASE or ROLLBACK, destroy all
                                                ///savepoints nested inside of the savepoint being operated on. 
                                                while (db.pSavepoint != pSavepoint)
                                                {
                                                    pTmp = db.pSavepoint;
                                                    db.pSavepoint = pTmp.pNext;
                                                    db.DbFree(ref pTmp);
                                                    db.nSavepoint--;
                                                }
                                                ///If it is a RELEASE, then destroy the savepoint being operated on 
                                                ///too. If it is a ROLLBACK TO, then set the number of deferred 
                                                ///constraint violations present in the database to the value stored
                                                ///when the savepoint was created.  
                                                if (p1 == sqliteinth.SAVEPOINT_RELEASE)
                                                {
                                                    Debug.Assert(pSavepoint == db.pSavepoint);
                                                    db.pSavepoint = pSavepoint.pNext;
                                                    db.DbFree(ref pSavepoint);
                                                    if (0 == isTransaction)
                                                    {
                                                        db.nSavepoint--;
                                                    }
                                                }
                                                else
                                                {
                                                    db.nDeferredCons = pSavepoint.nDeferredCons;
                                                }
                                                if (0 == isTransaction)
                                                {
                                                    rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, p1, iSavepoint);
                                                    if (rc != SqlResult.SQLITE_OK)
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
                                case OpCode.OP_AutoCommit:
                                    {
                                        int desiredAutoCommit;
                                        int iRollback;
                                        int turnOnAC;
                                        desiredAutoCommit = (u8)pOp.p1;
                                        iRollback = pOp.p2;
                                        turnOnAC = (desiredAutoCommit != 0 && 0 == db.autoCommit) ? 1 : 0;
                                        Debug.Assert(desiredAutoCommit != 0 || 0 == desiredAutoCommit);
                                        Debug.Assert(desiredAutoCommit != 0 || 0 == iRollback);
                                        Debug.Assert(db.activeVdbeCnt > 0);
                                        ///At least this one VM is active 
                                        if (turnOnAC != 0 && iRollback != 0 && db.activeVdbeCnt > 1)
                                        {
                                            ///If this instruction implements a ROLLBACK and other VMs are
                                            ///still running, and a transaction is active, return an error indicating
                                            ///that the other VMs must complete first.
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "cannot rollback transaction - " + "SQL statements in progress");
                                            rc = SqlResult.SQLITE_BUSY;
                                        }
                                        else
                                            if (turnOnAC != 0 && 0 == iRollback && db.writeVdbeCnt > 0)
                                        {
                                            ///If this instruction implements a COMMIT and other VMs are writing
                                            ///return an error indicating that the other VMs must complete first.
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "cannot commit transaction - " + "SQL statements in progress");
                                            rc = SqlResult.SQLITE_BUSY;
                                        }
                                        else
                                                if (desiredAutoCommit != db.autoCommit)
                                        {
                                            if (iRollback != 0)
                                            {
                                                Debug.Assert(desiredAutoCommit != 0);
                                                Sqlite3.sqlite3RollbackAll(db);
                                                db.autoCommit = 1;
                                            }
                                            else
                                                if ((rc = this.sqlite3VdbeCheckFk(1)) != SqlResult.SQLITE_OK)
                                            {
                                                goto vdbe_return;
                                            }
                                            else
                                            {
                                                db.autoCommit = (u8)desiredAutoCommit;
                                                if (this.sqlite3VdbeHalt() == SqlResult.SQLITE_BUSY)
                                                {
                                                    this.currentOpCodeIndex = opcodeIndex;
                                                    db.autoCommit = (u8)(desiredAutoCommit == 0 ? 1 : 0);
                                                    this.rc = rc = SqlResult.SQLITE_BUSY;
                                                    goto vdbe_return;
                                                }
                                            }
                                            Debug.Assert(db.nStatement == 0);
                                            Sqlite3.sqlite3CloseSavepoints(db);
                                            if (this.rc == SqlResult.SQLITE_OK)
                                            {
                                                rc = SqlResult.SQLITE_DONE;
                                            }
                                            else
                                            {
                                                rc = SqlResult.SQLITE_ERROR;
                                            }
                                            goto vdbe_return;
                                        }
                                        else
                                        {
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, (0 == desiredAutoCommit) ? "cannot start a transaction within a transaction" : ((iRollback != 0) ? "cannot rollback - no transaction is active" : "cannot commit - no transaction is active"));
                                            rc = SqlResult.SQLITE_ERROR;
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
                                case OpCode.OP_Found:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump, in3 
                                        ///</summary>
                                        int alreadyExists;
                                        VdbeCursor pC;
                                        var res = ThreeState.Neutral;
                                        UnpackedRecord pIdxKey;
                                        UnpackedRecord r = new UnpackedRecord();
                                        UnpackedRecord aTempRec = new UnpackedRecord();
                                        //char aTempRec[ROUND8(sizeof(UnpackedRecord)) + sizeof(Mem)*3 + 7];
                                        alreadyExists = 0;
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                                        Debug.Assert(pOp.p4type == P4Usage.P4_INT32);
                                        pC = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pC != null);
                                        pIn3 = aMem[pOp.p3];
                                        if (Sqlite3.ALWAYS(pC.pCursor != null))
                                        {
                                            Debug.Assert(!pC.isTable);
                                            if (pOp.p4.i > 0)
                                            {
                                                r.pKeyInfo = pC.pKeyInfo;
                                                r.nField = (u16)pOp.p4.i;
                                                r.aMem = new Mem[r.nField];
                                                for (int i = 0; i < r.aMem.Length; i++)
                                                {
                                                    r.aMem[i] = aMem[pOp.p3 + i];
#if SQLITE_DEBUG
																																																																																																																																																																																																															                    Debug.Assert( memIsValid( r.aMem[i] ) );
#endif
                                                }
                                                r.flags = UnpackedRecordFlags.UNPACKED_PREFIX_MATCH;
                                                pIdxKey = r;
                                            }
                                            else
                                            {
                                                Debug.Assert((pIn3.flags & MemFlags.MEM_Blob) != 0);
                                                Debug.Assert((pIn3.flags & MemFlags.MEM_Zero) == 0);
                                                ///
                                                ///<summary>
                                                ///zeroblobs already expanded 
                                                ///</summary>
                                                pIdxKey = vdbeaux.sqlite3VdbeRecordUnpack(pC.pKeyInfo, pIn3.CharacterCount, pIn3.zBLOB, aTempRec, 0);
                                                //sizeof( aTempRec ) );
                                                if (pIdxKey == null)
                                                {
                                                    goto no_mem;
                                                }
                                                pIdxKey.flags |= UnpackedRecordFlags.UNPACKED_PREFIX_MATCH;
                                            }
                                            rc = pC.pCursor.sqlite3BtreeMovetoUnpacked(pIdxKey, 0, 0, ref res);
                                            if (pOp.p4.i == 0)
                                            {
                                                vdbeaux.sqlite3VdbeDeleteUnpackedRecord(pIdxKey);
                                            }
                                            if (rc != SqlResult.SQLITE_OK)
                                            {
                                                break;
                                            }
                                            alreadyExists = (res == 0) ? 1 : 0;
                                            pC.deferredMoveto = false;
                                            pC.cacheStatus = Sqlite3.CACHE_STALE;
                                        }
                                        if (pOp.OpCode == OpCode.OP_Found)
                                        {
                                            if (alreadyExists != 0)
                                                opcodeIndex = pOp.p2 - 1;
                                        }
                                        else
                                        {
                                            if (0 == alreadyExists)
                                                opcodeIndex = pOp.p2 - 1;
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
                                case OpCode.OP_IsUnique:
                                    {
                                        ///jump, in3 
                                        u16 ii;
                                        VdbeCursor pCx = new VdbeCursor();
                                        BtCursor pCrsr;
                                        u16 nField;
                                        Mem[] aMx;
                                        i64 R;
                                        ///Rowid stored in register P3 
                                        var r = new UnpackedRecord();///<param name="B">Tree index search key </param>
                                        pIn3 = aMem[pOp.p3];
                                        //aMx = aMem[pOp->p4.i];
                                        ///Assert that the values of parameters P1 and P4 are in range. 
                                        Debug.Assert(pOp.p4type == P4Usage.P4_INT32);
                                        Debug.Assert(pOp.p4.i > 0 && pOp.p4.i <= this.aMem.Count());
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                                        ///Find the index cursor. 
                                        pCx = this.OpenCursors[pOp.p1];
                                        Debug.Assert(!pCx.deferredMoveto);
                                        pCx.seekResult = 0;
                                        pCx.cacheStatus = Sqlite3.CACHE_STALE;
                                        pCrsr = pCx.pCursor;
                                        ///If any of the values are NULL, take the jump. 
                                        nField = pCx.pKeyInfo.nField;
                                        aMx = new Mem[nField + 1];
                                        for (ii = 0; ii < nField; ii++)
                                        {
                                            aMx[ii] = aMem[pOp.p4.i + ii];
                                            if ((aMx[ii].flags & MemFlags.MEM_Null) != 0)
                                            {
                                                opcodeIndex = pOp.p2 - 1;
                                                pCrsr = null;
                                                break;
                                            }
                                        }
                                        aMx[nField] = new Mem();
                                        //Debug.Assert( ( aMx[nField].flags & MEM.MEM_Null ) == 0 );
                                        if (pCrsr != null)
                                        {
                                            ///Populate the index search key. 
                                            r.pKeyInfo = pCx.pKeyInfo;
                                            r.nField = (ushort)(nField + 1);
                                            r.flags = UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH;
                                            r.aMem = aMx;
#if SQLITE_DEBUG
																																																																																																																																																													                {
                  int i;
                  for ( i = 0; i < r.nField; i++ )
                    Debug.Assert( memIsValid( r.aMem[i] ) );
                }
#endif
                                            ///Extract the value of R from register P3. 
                                            pIn3.Integerify();
                                            R = pIn3.u.AsInteger;
                                            ///<param name="Search the B">Tree index. If no conflicting record is found, jump</param>
                                            ///<param name="to P2. Otherwise, copy the rowid of the conflicting record to">to P2. Otherwise, copy the rowid of the conflicting record to</param>
                                            ///<param name="register P3 and fall through to the next instruction.  ">register P3 and fall through to the next instruction.  </param>
                                            rc = pCrsr.sqlite3BtreeMovetoUnpacked(r, 0, 0, ref pCx.seekResult);
                                            if ((r.flags & UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH) != 0 || r.rowid == R)
                                            {
                                                opcodeIndex = pOp.p2 - 1;
                                            }
                                            else
                                            {
                                                pIn3.u.AsInteger = r.rowid;
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
                                case OpCode.OP_NotExists:
                                    {
                                        ///jump, in3   
                                        pIn3 = aMem[pOp.p3];
                                        Debug.Assert((pIn3.flags & MemFlags.MEM_Int) != 0);
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                                        var pC = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pC != null);
                                        Debug.Assert(pC.isTable);
                                        Debug.Assert(pC.pseudoTableReg == 0);
                                        var pCrsr = pC.pCursor;
                                        if (pCrsr != null)
                                        {
                                            var res = ThreeState.Neutral;
                                            var iKey = pIn3.u.AsInteger;
                                            rc = pCrsr.sqlite3BtreeMovetoUnpacked(null, (long)iKey, 0, ref res);
                                            pC.lastRowid = pIn3.u.AsInteger;
                                            pC.rowidIsValid = res == 0 ? true : false;
                                            pC.nullRow = false;
                                            pC.cacheStatus = Sqlite3.CACHE_STALE;
                                            pC.deferredMoveto = false;
                                            if (res != 0)
                                            {
                                                opcodeIndex = pOp.p2 - 1;
                                                Debug.Assert(!pC.rowidIsValid);
                                            }
                                            pC.seekResult = res;
                                        }
                                        else
                                        {
                                            ///This happens when an attempt to open a read cursor on the
                                            ///sqlite_master table returns SQLITE_EMPTY.
                                            opcodeIndex = pOp.p2 - 1;
                                            Debug.Assert(!pC.rowidIsValid);
                                            pC.seekResult = 0;
                                        }
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
                                case OpCode.OP_RowData:
                                    {
                                        VdbeCursor pC;
                                        BtCursor pCrsr;
                                        u32 n;
                                        i64 n64;
                                        n = 0;
                                        n64 = 0;
                                        pOut = aMem[pOp.p2];
                                        this.memAboutToChange(pOut);
                                        ///
                                        ///<summary>
                                        ///Note that RowKey and RowData are really exactly the same instruction 
                                        ///</summary>
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                                        pC = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pC.isTable || pOp.OpCode == OpCode.OP_RowKey);
                                        Debug.Assert(pC.isIndex || pOp.OpCode == OpCode.OP_RowData);
                                        Debug.Assert(pC != null);
                                        Debug.Assert(pC.nullRow == false);
                                        Debug.Assert(pC.pseudoTableReg == 0);
                                        Debug.Assert(pC.pCursor != null);
                                        pCrsr = pC.pCursor;
                                        Debug.Assert(pCrsr.sqlite3BtreeCursorIsValid());
                                        ///
                                        ///<summary>
                                        ///The  OpCode.OP_RowKey and  OpCode.OP_RowData opcodes always follow  OpCode.OP_NotExists or
                                        ///OP_Rewind/Op_Next with no intervening instructions that might invalidate
                                        ///the cursor.  Hence the following sqlite3VdbeCursorMoveto() call is always
                                        ///</summary>
                                        ///<param name="a no">op and can never fail.  But we leave it in place as a safety.</param>
                                        ///<param name=""></param>
                                        Debug.Assert(pC.deferredMoveto == false);
                                        rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
                                        if (Sqlite3.NEVER(rc != SqlResult.SQLITE_OK))
                                            goto abort_due_to_error;
                                        if (pC.isIndex)
                                        {
                                            Debug.Assert(!pC.isTable);
                                            rc = pCrsr.sqlite3BtreeKeySize(ref n64);
                                            Debug.Assert(rc == SqlResult.SQLITE_OK);
                                            ///
                                            ///<summary>
                                            ///True because of CursorMoveto() call above 
                                            ///</summary>
                                            if (n64 > db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
                                            {
                                                goto too_big;
                                            }
                                            n = (u32)n64;
                                        }
                                        else
                                        {
                                            rc = pCrsr.sqlite3BtreeDataSize(ref n);
                                            Debug.Assert(rc == SqlResult.SQLITE_OK);
                                            ///
                                            ///<summary>
                                            ///DataSize() cannot fail 
                                            ///</summary>
                                            if (n > (u32)db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
                                            {
                                                goto too_big;
                                            }
                                            if (pOut.Grow( (int)n, 0) != 0)
                                            {
                                                goto no_mem;
                                            }
                                        }
                                        pOut.CharacterCount = (int)n;
                                        if (pC.isIndex)
                                        {
                                            pOut.zBLOB = malloc_cs.sqlite3Malloc((int)n);
                                            rc = pCrsr.sqlite3BtreeKey(0, n, pOut.zBLOB);
                                        }
                                        else
                                        {
                                            pOut.zBLOB = malloc_cs.sqlite3Malloc((int)pCrsr.info.nData);
                                            rc = pCrsr.sqlite3BtreeData(0, (u32)n, pOut.zBLOB);
                                        }

                                        pOut.MemSetTypeFlag(MemFlags.MEM_Blob);
                                        pOut.enc = SqliteEncoding.UTF8;
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
                                case OpCode.OP_Rowid:
                                    {
                                        ///
                                        ///<summary>
                                        ///</summary>
                                        ///<param name="out2">prerelease </param>
                                        VdbeCursor pC;
                                        i64 v;
                                        sqlite3_vtab pVtab;
                                        sqlite3_module pModule;
                                        v = 0;
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                                        pC = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pC != null);
                                        Debug.Assert(pC.pseudoTableReg == 0);
                                        if (pC.nullRow)
                                        {
                                            pOut.flags = MemFlags.MEM_Null;
                                            break;
                                        }
                                        else
                                            if (pC.deferredMoveto)
                                        {
                                            v = pC.movetoTarget;
#if !SQLITE_OMIT_VIRTUALTABLE
                                        }
                                        else
                                                if (pC.pVtabCursor != null)
                                        {
                                            pVtab = pC.pVtabCursor.pVtab;
                                            pModule = pVtab.pModule;
                                            Debug.Assert(pModule.xRowid != null);
                                            rc = pModule.xRowid(pC.pVtabCursor, out v);
                                            Sqlite3.importVtabErrMsg(this, pVtab);
#endif
                                        }
                                        else
                                        {
                                            Debug.Assert(pC.pCursor != null);
                                            rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
                                            if (rc != 0)
                                                goto abort_due_to_error;
                                            if (pC.rowidIsValid)
                                            {
                                                v = pC.lastRowid;
                                            }
                                            else
                                            {
                                                rc = pC.pCursor.sqlite3BtreeKeySize(ref v);
                                                Debug.Assert(rc == SqlResult.SQLITE_OK);
                                                ///
                                                ///<summary>
                                                ///Always so because of CursorMoveto() above 
                                                ///</summary>
                                            }
                                        }
                                        pOut.u.AsInteger = (long)v;
                                        break;
                                    }




                                ///

                                //-------------------------------------------------------
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
                                case OpCode.OP_RowSetTest:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump, in1, in3 
                                        ///</summary>
                                        int iSet;
                                        int exists;
                                        pIn1 = aMem[pOp.p1];
                                        pIn3 = aMem[pOp.p3];
                                        iSet = pOp.p4.i;
                                        Debug.Assert((pIn3.flags & MemFlags.MEM_Int) != 0);
                                        ///
                                        ///<summary>
                                        ///If there is anything other than a rowset object in memory cell P1,
                                        ///delete it now and initialize P1 with an empty rowset
                                        ///
                                        ///</summary>
                                        if ((pIn1.flags & MemFlags.MEM_RowSet) == 0)
                                        {
                                            pIn1.sqlite3VdbeMemSetRowSet();
                                            if ((pIn1.flags & MemFlags.MEM_RowSet) == 0)
                                                goto no_mem;
                                        }
                                        Debug.Assert(pOp.p4type == P4Usage.P4_INT32);
                                        Debug.Assert(iSet == -1 || iSet >= 0);
                                        if (iSet != 0)
                                        {
                                            exists = pIn1.u.pRowSet.sqlite3RowSetTest((u8)(iSet >= 0 ? iSet & 0xf : 0xff), pIn3.u.AsInteger);
                                            if (exists != 0)
                                            {
                                                opcodeIndex = pOp.p2 - 1;
                                                break;
                                            }
                                        }
                                        if (iSet >= 0)
                                        {
                                            pIn1.u.pRowSet.sqlite3RowSetInsert(pIn3.u.AsInteger);
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
                                case OpCode.OP_Program:
                                    {
                                        ///jump 
                                        int nMem;
                                        ///<param name="Number of memory registers for sub">program </param>
                                        int nByte;
                                        ///<param name="Bytes of runtime space required for sub">program </param>
                                        Mem pRt;
                                        ///Register to allocate runtime space 
                                        Mem pMem = null;
                                        ///Used to iterate through memory cells 
                                        //Mem pEnd;            /* Last memory cell in new array */
                                        VdbeFrame pFrame;
                                        ///New vdbe frame to execute in 
                                        SubProgram pProgram;
                                        ///<param name="Sub">program to execute </param>
                                        int t;
                                        ///Token identifying trigger 
                                        pProgram = pOp.p4.pProgram;
                                        pRt = aMem[pOp.p3];
                                        Debug.Assert(pRt.memIsValid());
                                        Debug.Assert(pProgram.nOp > 0);
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
                                        if (pOp.p5 != 0)
                                        {
                                            t = pProgram.token;
                                            for (pFrame = this.pFrame; pFrame != null && pFrame.token != t; pFrame = pFrame.pParent)
                                                ;
                                            if (pFrame != null)
                                                break;
                                        }
                                        if (this.nFrame >= db.aLimit[Globals.SQLITE_LIMIT_TRIGGER_DEPTH])
                                        {
                                            rc = SqlResult.SQLITE_ERROR;
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "too many levels of trigger recursion");
                                            break;
                                        }
                                        ///
                                        ///<summary>
                                        ///Register pRt is used to store the memory required to save the state
                                        ///of the current program, and the memory required at runtime to execute
                                        ///the trigger program. If this trigger has been fired before, then pRt 
                                        ///is already allocated. Otherwise, it must be initialized.  
                                        ///</summary>
                                        if ((pRt.flags & MemFlags.MEM_Frame) == 0)
                                        {
                                            ///
                                            ///<summary>
                                            ///SubProgram.nMem is set to the number of memory cells used by the 
                                            ///program stored in SubProgram.aOp. As well as these, one memory
                                            ///cell is required for each cursor used by the program. Set local
                                            ///variable nMem (and later, VdbeFrame.nChildMem) to this value.
                                            ///
                                            ///</summary>
                                            nMem = pProgram.nMem + pProgram.nCsr;
                                            //nByte = ROUND8( sizeof( VdbeFrame ) )
                                            //+ nMem * sizeof( Mem )
                                            //+ pProgram.nCsr * sizeof( VdbeCursor* );
                                            pFrame = new VdbeFrame();
                                            // sqlite3DbMallocZero( db, nByte );
                                            //if ( !pFrame )
                                            //{
                                            //  goto no_mem;
                                            //}
                                            pRt.Release();
                                            pRt.flags = MemFlags.MEM_Frame;
                                            pRt.u.pFrame = pFrame;
                                            pFrame.v = this;
                                            pFrame.nChildMem = nMem;
                                            pFrame.nChildCsr = pProgram.nCsr;
                                            pFrame.currentOpCodeIndex = opcodeIndex;
                                            pFrame.aMem = this.aMem;
                                            //pFrame.nMem = this.nMem;
                                            pFrame.apCsr = this.OpenCursors;
                                            pFrame.nCursor = this.nCursor;
                                            pFrame.aOp = this.aOp;
                                            //pFrame.nOp = this.nOp;
                                            pFrame.token = pProgram.token;
                                            // &VdbeFrameMem( pFrame )[pFrame.nChildMem];
                                            // aMem is 1 based, so allocate 1 extra cell under C#
                                            pFrame.aChildMem = new List<Mem>(pFrame.nChildMem + 1);
                                            for (int i = 0; i < pFrame.aChildMem.Count(); i++)//pMem = VdbeFrameMem( pFrame ) ; pMem != pEnd ; pMem++ )
                                            {
                                                //pFrame.aMem[i] = pFrame.aMem[pFrame.nMem+i];
                                                pMem = malloc_cs.sqlite3Malloc(pMem);
                                                pMem.flags = MemFlags.MEM_Null;
                                                pMem.db = db;
                                                pFrame.aChildMem[i] = pMem;
                                            }
                                            pFrame.aChildCsr = new VdbeCursor[pFrame.nChildCsr];
                                            for (int i = 0; i < pFrame.nChildCsr; i++)
                                                pFrame.aChildCsr[i] = new VdbeCursor();
                                        }
                                        else
                                        {
                                            pFrame = pRt.u.pFrame;
                                            Debug.Assert(pProgram.nMem + pProgram.nCsr == pFrame.nChildMem);
                                            Debug.Assert(pProgram.nCsr == pFrame.nChildCsr);
                                            Debug.Assert(opcodeIndex == pFrame.currentOpCodeIndex);
                                        }
                                        this.nFrame++;
                                        pFrame.pParent = this.pFrame;
                                        pFrame.lastRowid = lastRowid;
                                        pFrame.nChange = this.nChange;
                                        this.nChange = 0;
                                        this.pFrame = pFrame;
                                        this.aMem = aMem = pFrame.aChildMem;
                                        // &VdbeFrameMem( pFrame )[-1];
                                        //this.nMem = pFrame.nChildMem;
                                        this.nCursor = (u16)pFrame.nChildCsr;
                                        this.OpenCursors = pFrame.aChildCsr;
                                        // (VdbeCursor *)&aMem[p->nMem+1];
                                        this.lOp = lOp = new List<Operation>(pProgram.aOp);
                                        //this.nOp = pProgram.nOp;
                                        opcodeIndex = -1;
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
                                case OpCode.OP_Param:
                                    {
                                        ///
                                        ///<summary>
                                        ///</summary>
                                        ///<param name="out2">prerelease </param>
                                        VdbeFrame pFrame;
                                        Mem pIn;
                                        pFrame = this.pFrame;
                                        pIn = pFrame.aMem[pOp.p1 + pFrame.aOp[pFrame.currentOpCodeIndex].p1];
                                        vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pIn, MemFlags.MEM_Ephem);
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
                                case OpCode.OP_FkCounter:
                                    {
                                        if (pOp.p1 != 0)
                                        {
                                            db.nDeferredCons += pOp.p2;
                                        }
                                        else
                                        {
                                            this.nFkConstraint += pOp.p2;
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
                                case OpCode.OP_FkIfZero:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump 
                                        ///</summary>
                                        if (pOp.p1 != 0)
                                        {
                                            if (db.nDeferredCons == 0)
                                                opcodeIndex = pOp.p2 - 1;
                                        }
                                        else
                                        {
                                            if (this.nFkConstraint == 0)
                                                opcodeIndex = pOp.p2 - 1;
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
                                case OpCode.OP_MemMax:
                                    {
                                        ///
                                        ///<summary>
                                        ///in2 
                                        ///</summary>
                                        Mem _pIn1;
                                        VdbeFrame pFrame;
                                        if (this.pFrame != null)
                                        {
                                            for (pFrame = this.pFrame; pFrame.pParent != null; pFrame = pFrame.pParent)
                                                ;
                                            _pIn1 = pFrame.aMem[pOp.p1];
                                        }
                                        else
                                        {
                                            _pIn1 = aMem[pOp.p1];
                                        }
                                        Debug.Assert(_pIn1.memIsValid());
                                        _pIn1.Integerify();
                                        pIn2 = aMem[pOp.p2];
                                        pIn2.Integerify();
                                        if (_pIn1.u.AsInteger < pIn2.u.AsInteger)
                                        {
                                            _pIn1.u.AsInteger = pIn2.u.AsInteger;
                                        }
                                        break;
                                    }
#endif

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
                                case OpCode.OP_AggStep:
                                    {
                                        int n;
                                        int i;
                                        Mem pMem;
                                        Mem pRec;
                                        sqlite3_context ctx = new sqlite3_context();
                                        sqlite3_value[] apVal;
                                        n = pOp.p5;
                                        Debug.Assert(n >= 0);
                                        //pRec = aMem[pOp.p2];
                                        apVal = this.apArg;
                                        Debug.Assert(apVal != null || n == 0);
                                        for (i = 0; i < n; i++)//, pRec++)
                                        {
                                            pRec = aMem[pOp.p2 + i];
                                            Debug.Assert(pRec.memIsValid());
                                            apVal[i] = pRec;
                                            this.memAboutToChange(pRec);
                                            Sqlite3.sqlite3VdbeMemStoreType(pRec);
                                        }
                                        ctx.pFunc = pOp.p4.pFunc;
                                        Debug.Assert(pOp.p3 > 0 && pOp.p3 <= this.aMem.Count());
                                        ctx.pMem = pMem = aMem[pOp.p3];
                                        pMem.CharacterCount++;
                                        ctx.s.flags = MemFlags.MEM_Null;
                                        ctx.s.AsString = null;
                                        //ctx.s.zMalloc = null;
                                        ctx.s.xDel = null;
                                        ctx.s.db = db;
                                        ctx.isError = 0;
                                        ctx.pColl = null;
                                        if ((ctx.pFunc.flags & FuncFlags.SQLITE_FUNC_NEEDCOLL) != 0)
                                        {
                                            Debug.Assert(opcodeIndex > 0);
                                            //pOp > p.aOp );
                                            Debug.Assert(this.lOp[opcodeIndex - 1].p4type == P4Usage.P4_COLLSEQ);
                                            //pOp[-1].p4type ==  P4Usage.P4_COLLSEQ );
                                            Debug.Assert(this.lOp[opcodeIndex - 1].OpCode == OpCode.OP_CollSeq);
                                            // pOp[-1].opcode ==  OpCode.OP_CollSeq );
                                            ctx.pColl = this.lOp[opcodeIndex - 1].p4.pColl;
                                            ;
                                            // pOp[-1].p4.pColl;
                                        }
                                        ctx.pFunc.xStep(ctx, n, apVal);
                                        ///
                                        ///<summary>
                                        ///</summary>
                                        ///<param name="IMP: R">23230 </param>
                                        if (ctx.isError != 0)
                                        {
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, vdbeapi.sqlite3_value_text(ctx.s));
                                            rc = ctx.isError;
                                        }
                                        ctx.s.Release();
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
                                case OpCode.OP_AggFinal:
                                    {
                                        Mem pMem;
                                        Debug.Assert(pOp.p1 > 0 && pOp.p1 <= this.aMem.Count());
                                        pMem = aMem[pOp.p1];
                                        Debug.Assert((pMem.flags & ~(MemFlags.MEM_Null | MemFlags.MEM_Agg)) == 0);
                                        rc = vdbemem_cs.sqlite3VdbeMemFinalize(pMem, pOp.p4.pFunc);
                                        this.aMem[pOp.p1] = pMem;
                                        if (rc != 0)
                                        {
                                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, vdbeapi.sqlite3_value_text(pMem));
                                        }
                                        vdbemem_cs.sqlite3VdbeChangeEncoding(pMem, encoding);
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pMem );
#endif
                                        if (pMem.IsTooBig())
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
    rc = SqlResult.SQLITE_OK;
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
                                        ///<param name="out2">prerelease </param>
                                        Btree pBt;
                                        ///Btree to change journal mode of 
                                        Pager pPager;
                                        ///Pager associated with pBt 
                                        JournalMode eNew;
                                        ///New journal mode 
                                        JournalMode eOld;
                                        ///The old journal mode 
                                        string zFilename;
                                        ///Name of database file for pPager 
                                        eNew = (JournalMode)pOp.p3;
                                        Debug.Assert(eNew == JournalMode.PAGER_JOURNALMODE_DELETE || eNew == JournalMode.PAGER_JOURNALMODE_TRUNCATE || eNew == JournalMode.PAGER_JOURNALMODE_PERSIST || eNew == JournalMode.PAGER_JOURNALMODE_OFF || eNew == JournalMode.PAGER_JOURNALMODE_MEMORY || eNew == JournalMode.PAGER_JOURNALMODE_WAL || eNew == JournalMode.PAGER_JOURNALMODE_QUERY);
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.BackendCount);
                                        pBt = db.Backends[pOp.p1].BTree;
                                        pPager = pBt.sqlite3BtreePager();
                                        eOld = pPager.sqlite3PagerGetJournalMode();
                                        if (eNew == JournalMode.PAGER_JOURNALMODE_QUERY)
                                            eNew = eOld;
                                        if (0 == pPager.sqlite3PagerOkToChangeJournalMode())
                                            eNew = eOld;
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
rc = SqlResult.SQLITE_ERROR;
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
if( rc==SqlResult.SQLITE_OK ){
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
if( rc==SqlResult.SQLITE_OK ){
rc = sqlite3BtreeSetVersion(pBt, (eNew==PAGER_JOURNALMODE_WAL ? 2 : 1));
}
}
}
#endif
                                        if (rc != 0)
                                        {
                                            eNew = eOld;
                                        }
                                        eNew = pPager.sqlite3PagerSetJournalMode(eNew);
                                        pOut = aMem[pOp.p2];
                                        pOut.flags = MemFlags.MEM_Str | MemFlags.MEM_Static | MemFlags.MEM_Term;
                                        pOut.AsString = Sqlite3.sqlite3JournalModename(eNew);
                                        pOut.CharacterCount = StringExtensions.Strlen30(pOut.AsString);
                                        pOut.enc = SqliteEncoding.UTF8;
                                        vdbemem_cs.sqlite3VdbeChangeEncoding(pOut, encoding);
                                        break;
                                    }
                                    ;
#endif
#if !SQLITE_OMIT_VACUUM && !SQLITE_OMIT_ATTACH
                                ///Opcode: Vacuum * * * * *
                                ///
                                ///Vacuum the entire database.  This opcode will cause other virtual
                                ///machines to be created and run.  It may not be called from within
                                ///a transaction.
                                case OpCode.OP_Vacuum:
                                    {
                                        rc = Sqlite3.sqlite3RunVacuum(ref this.zErrMsg, db);
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
                                case OpCode.OP_IncrVacuum:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump 
                                        ///</summary>
                                        Btree pBt;
                                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.BackendCount);
                                        Debug.Assert((this.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);
                                        pBt = db.Backends[pOp.p1].BTree;
                                        rc = pBt.sqlite3BtreeIncrVacuum();
                                        if (rc == SqlResult.SQLITE_DONE)
                                        {
                                            opcodeIndex = pOp.p2 - 1;
                                            rc = SqlResult.SQLITE_OK;
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
                                case OpCode.OP_Expire:
                                    {
                                        if (pOp.p1 == 0)
                                        {
                                            vdbeaux.sqlite3ExpirePreparedStatements(db);
                                        }
                                        else
                                        {
                                            this.expired = true;
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
                                malloc_cs.sqlite3SetString(ref this.zErrMsg, db, "%s", rc.sqlite3ErrStr());
                            }
                            goto case RuntimeException.vdbe_error_halt;
                            break;
                        case RuntimeException.abort_due_to_interrupt:
                            ///Jump to here if the sqlite3_interrupt() API sets the interrupt
                            ///flag.
                            Debug.Assert(db.u1.isInterrupted);
                            rc = SqlResult.SQLITE_INTERRUPT;
                            this.rc = rc;
                            malloc_cs.sqlite3SetString(ref this.zErrMsg, db, rc.sqlite3ErrStr());
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
                p = new Vdbe()
                {
                    db=db,
                    pNext = db.pVdbe,
                    pPrev = null,
                    magic = VdbeMagic.VDBE_MAGIC_INIT
                };
                
                MyLinqExtensions.push(ref db.pVdbe,p);
                                
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
                            Engine.Op.Output.Exec

                            //Engine.Op.TheRest.Exec
                    };
        }



    }
}