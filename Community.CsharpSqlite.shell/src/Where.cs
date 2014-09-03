using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {




        ///<summary>
        /// A WherePlan object holds information that describes a lookup
        /// strategy.
        ///
        /// This object is intended to be opaque outside of the where.c module.
        /// It is included here only so that that compiler will know how big it
        /// is.  None of the fields in this object should be used outside of
        /// the where.c module.
        ///
        /// Within the union, pIdx is only used when wsFlags&WHERE_INDEXED is true.
        /// pTerm is only used when wsFlags&WHERE_MULTI_OR is true.  And pVtabIdx
        /// is only used when wsFlags&WHERE_VIRTUALTABLE is true.  It is never the
        /// case that more than one of these conditions is true.
        ///
        ///</summary>
        public class WherePlan
        {
            public u32 wsFlags;

            ///
            ///<summary>
            ///WHERE_* flags that describe the strategy 
            ///</summary>

            public u32 nEq;

            ///<summary>
            ///Number of == constraints
            ///</summary>
            public double nRow;

            ///<summary>
            ///Estimated number of rows (for EQP)
            ///</summary>
            public class _u
            {
                public Index pIdx;

                ///
                ///<summary>
                ///Index when WHERE_INDEXED is true 
                ///</summary>

                public WhereTerm pTerm;

                ///
                ///<summary>
                ///</summary>
                ///<param name="WHERE clause term for OR">search </param>

                public sqlite3_index_info pVtabIdx;
                ///
                ///<summary>
                ///Virtual table index to use 
                ///</summary>

            }

            public _u u = new _u();

            public void Clear()
            {
                wsFlags = 0;
                nEq = 0;
                nRow = 0;
                u.pIdx = null;
                u.pTerm = null;
                u.pVtabIdx = null;
            }
        };


        public class WhereLevel
        {
            public WherePlan plan = new WherePlan();

            /** query plan for this element of the FROM clause */
            public int iLeftJoin;

            /** Memory cell used to implement LEFT OUTER JOIN */
            public int iTabCur;

            /** The VDBE cursor used to access the table */
            public int iIdxCur;

            /** The VDBE cursor used to access pIdx */
            public int addrBrk;

            /** Jump here to break out of the loop */
            public int addrNxt;

            /** Jump here to start the next IN combination */
            public int addrCont;

            /** Jump here to continue with the next loop cycle */
            public int addrFirst;

            /** First instruction of interior of the loop */
            public u8 iFrom;

            /** Which entry in the FROM clause */
            public u8 op, p5;

            ///<summary>
            ///Opcode and P5 of the opcode that ends the loop
            ///</summary>
            public int p1, p2;

            ///
            ///<summary>
            ///Operands of the opcode used to ends the loop 
            ///</summary>

            public class _u
            {
                public class __in
                ///
                ///<summary>
                ///Information that depends on plan.wsFlags 
                ///</summary>
                {
                    public int nIn;

                    ///
                    ///<summary>
                    ///Number of entries in aInLoop[] 
                    ///</summary>

                    public InLoop[] aInLoop;
                    ///
                    ///<summary>
                    ///Information about each nested IN operator 
                    ///</summary>

                }

                public __in _in = new __in();
                ///
                ///<summary>
                ///Used when plan.wsFlags&WHERE_IN_ABLE 
                ///</summary>

            }

            public _u u = new _u();

            ///
            ///<summary>
            ///The following field is really not part of the current level.  But
            ///we need a place to cache virtual table index information for each
            ///virtual table in the FROM clause and the WhereLevel structure is
            ///a convenient place since there is one WhereLevel for each FROM clause
            ///element.
            ///
            ///</summary>

            public sqlite3_index_info pIdxInfo;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Index info for n">th source table </param>

            public void disableTerm(WhereTerm pTerm)
            {
                if (pTerm != null && (pTerm.wtFlags & TERM_CODED) == 0 && (this.iLeftJoin == 0 || ExprHasProperty(pTerm.pExpr, EP_FromJoin)))
                {
                    pTerm.wtFlags |= TERM_CODED;
                    if (pTerm.iParent >= 0)
                    {
                        WhereTerm pOther = pTerm.pWC.a[pTerm.iParent];
                        if ((--pOther.nChild) == 0)
                        {
                            this.disableTerm(pOther);
                        }
                    }
                }
            }
        }

        ///<summary>
        /// Flags appropriate for the wctrlFlags parameter of sqlite3WhereBegin()
        /// and the WhereInfo.wctrlFlags member.
        ///
        ///</summary>
        //#define WHERE_ORDERBY_NORMAL   0x0000 /* No-op */
        //#define WHERE_ORDERBY_MIN      0x0001 /* ORDER BY processing for min() func */
        //#define WHERE_ORDERBY_MAX      0x0002 /* ORDER BY processing for max() func */
        //#define WHERE_ONEPASS_DESIRED  0x0004 /* Want to do one-pass UPDATE/DELETE */
        //#define WHERE_DUPLICATES_OK    0x0008 /* Ok to return a row more than once */
        //#define WHERE_OMIT_OPEN        0x0010  /* Table cursors are already open */
        //#define WHERE_OMIT_CLOSE       0x0020  /* Omit close of table & index cursors */
        //#define WHERE_FORCE_TABLE      0x0040 /* Do not use an index-only search */
        //#define WHERE_ONETABLE_ONLY    0x0080 /* Only code the 1st table in pTabList */
        private const int WHERE_ORDERBY_NORMAL = 0x0000;

        private const int WHERE_ORDERBY_MIN = 0x0001;

        private const int WHERE_ORDERBY_MAX = 0x0002;

        private const int WHERE_ONEPASS_DESIRED = 0x0004;

        private const int WHERE_DUPLICATES_OK = 0x0008;

        private const int WHERE_OMIT_OPEN = 0x0010;

        private const int WHERE_OMIT_CLOSE = 0x0020;

        private const int WHERE_FORCE_TABLE = 0x0040;

        private const int WHERE_ONETABLE_ONLY = 0x0080;

        ///<summary>
        /// The WHERE clause processing routine has two halves.  The
        /// first part does the start of the WHERE loop and the second
        /// half does the tail of the WHERE loop.  An instance of
        /// this structure is returned by the first half and passed
        /// into the second half to give some continuity.
        ///
        ///</summary>
        public class WhereInfo
        {
            public Parse pParse;

            ///
            ///<summary>
            ///Parsing and code generating context 
            ///</summary>

            public u16 wctrlFlags;

            ///
            ///<summary>
            ///Flags originally passed to sqlite3WhereBegin() 
            ///</summary>

            public u8 okOnePass;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Ok to use one">pass algorithm for UPDATE or DELETE </param>

            public u8 untestedTerms;

            ///
            ///<summary>
            ///Not all WHERE terms resolved by outer loop 
            ///</summary>

            public SrcList pTabList;

            ///
            ///<summary>
            ///List of tables in the join 
            ///</summary>

            public int iTop;

            ///
            ///<summary>
            ///The very beginning of the WHERE loop 
            ///</summary>

            public int iContinue;

            ///
            ///<summary>
            ///Jump here to continue with next record 
            ///</summary>

            public int iBreak;

            ///
            ///<summary>
            ///Jump here to break out of the loop 
            ///</summary>

            public int nLevel;

            ///
            ///<summary>
            ///Number of nested loop 
            ///</summary>

            public WhereClause pWC;

            ///
            ///<summary>
            ///Decomposition of the WHERE clause 
            ///</summary>

            public double savedNQueryLoop;

            ///
            ///<summary>
            ///</summary>
            ///<param name="pParse">>nQueryLoop outside the WHERE loop </param>

            public double nRowOut;

            ///
            ///<summary>
            ///Estimated number of output rows 
            ///</summary>

            public WhereLevel[] a = new WhereLevel[] {
				new WhereLevel ()
			};

            ///
            ///<summary>
            ///Information about each nest loop in the WHERE 
            ///</summary>

            public void sqlite3WhereEnd()
            {
                Parse pParse = this.pParse;
                Vdbe v = pParse.pVdbe;
                int i;
                WhereLevel pLevel;
                SrcList pTabList = this.pTabList;
                sqlite3 db = pParse.db;
                ///
                ///<summary>
                ///Generate loop termination code.
                ///
                ///</summary>

                pParse.sqlite3ExprCacheClear();
                for (i = this.nLevel - 1; i >= 0; i--)
                {
                    pLevel = this.a[i];
                    v.sqlite3VdbeResolveLabel(pLevel.addrCont);
                    if (pLevel.op != OP_Noop)
                    {
                        v.sqlite3VdbeAddOp2(pLevel.op, pLevel.p1, pLevel.p2);
                        v.sqlite3VdbeChangeP5(pLevel.p5);
                    }
                    if ((pLevel.plan.wsFlags & WHERE_IN_ABLE) != 0 && pLevel.u._in.nIn > 0)
                    {
                        InLoop pIn;
                        int j;
                        v.sqlite3VdbeResolveLabel(pLevel.addrNxt);
                        for (j = pLevel.u._in.nIn; j > 0; j--)//, pIn--)
                        {
                            pIn = pLevel.u._in.aInLoop[j - 1];
                            v.sqlite3VdbeJumpHere(pIn.addrInTop + 1);
                            v.sqlite3VdbeAddOp2(OP_Next, pIn.iCur, pIn.addrInTop);
                            v.sqlite3VdbeJumpHere(pIn.addrInTop - 1);
                        }
                        db.sqlite3DbFree(ref pLevel.u._in.aInLoop);
                    }
                    v.sqlite3VdbeResolveLabel(pLevel.addrBrk);
                    if (pLevel.iLeftJoin != 0)
                    {
                        int addr;
                        addr = v.sqlite3VdbeAddOp1(OpCode.OP_IfPos, pLevel.iLeftJoin);
                        Debug.Assert((pLevel.plan.wsFlags & WHERE_IDX_ONLY) == 0 || (pLevel.plan.wsFlags & WHERE_INDEXED) != 0);
                        if ((pLevel.plan.wsFlags & WHERE_IDX_ONLY) == 0)
                        {
                            v.sqlite3VdbeAddOp1(OpCode.OP_NullRow, pTabList.a[i].iCursor);
                        }
                        if (pLevel.iIdxCur >= 0)
                        {
                            v.sqlite3VdbeAddOp1(OpCode.OP_NullRow, pLevel.iIdxCur);
                        }
                        if (pLevel.op == OP_Return)
                        {
                            v.sqlite3VdbeAddOp2(OP_Gosub, pLevel.p1, pLevel.addrFirst);
                        }
                        else
                        {
                            v.sqlite3VdbeAddOp2(OP_Goto, 0, pLevel.addrFirst);
                        }
                        v.sqlite3VdbeJumpHere(addr);
                    }
                }
                ///
                ///<summary>
                ///The "break" point is here, just past the end of the outer loop.
                ///Set it.
                ///
                ///</summary>

                v.sqlite3VdbeResolveLabel(this.iBreak);
                ///
                ///<summary>
                ///Close all of the cursors that were opened by sqlite3WhereBegin.
                ///
                ///</summary>

                Debug.Assert(this.nLevel == 1 || this.nLevel == pTabList.nSrc);
                for (i = 0; i < this.nLevel; i++)//  for(i=0, pLevel=pWInfo.a; i<pWInfo.nLevel; i++, pLevel++){
                {
                    pLevel = this.a[i];
                    SrcList_item pTabItem = pTabList.a[pLevel.iFrom];
                    Table pTab = pTabItem.pTab;
                    Debug.Assert(pTab != null);
                    if ((pTab.tabFlags & TF_Ephemeral) == 0 && pTab.pSelect == null && (this.wctrlFlags & WHERE_OMIT_CLOSE) == 0)
                    {
                        u32 ws = pLevel.plan.wsFlags;
                        if (0 == this.okOnePass && (ws & WHERE_IDX_ONLY) == 0)
                        {
                            v.sqlite3VdbeAddOp1(OpCode.OP_Close, pTabItem.iCursor);
                        }
                        if ((ws & WHERE_INDEXED) != 0 && (ws & WHERE_TEMP_INDEX) == 0)
                        {
                            v.sqlite3VdbeAddOp1(OpCode.OP_Close, pLevel.iIdxCur);
                        }
                    }
                    ///
                    ///<summary>
                    ///If this scan uses an index, make code substitutions to read data
                    ///from the index in preference to the table. Sometimes, this means
                    ///the table need never be read from. This is a performance boost,
                    ///as the vdbe level waits until the table is read before actually
                    ///seeking the table cursor to the record corresponding to the current
                    ///position in the index.
                    ///
                    ///Calls to the code generator in between sqlite3WhereBegin and
                    ///sqlite3WhereEnd will have created code that references the table
                    ///directly.  This loop scans all that code looking for opcodes
                    ///that reference the table and converts them into opcodes that
                    ///reference the index.
                    ///
                    ///</summary>

                    if ((pLevel.plan.wsFlags & WHERE_INDEXED) != 0)///* && 0 == db.mallocFailed */ )
                    {
                        int k, j, last;
                        VdbeOp pOp;
                        Index pIdx = pLevel.plan.u.pIdx;
                        Debug.Assert(pIdx != null);
                        //pOp = sqlite3VdbeGetOp( v, pWInfo.iTop );
                        last = v.sqlite3VdbeCurrentAddr();
                        for (k = this.iTop; k < last; k++)//, pOp++ )
                        {
                            pOp = v.sqlite3VdbeGetOp(k);
                            if (pOp.p1 != pLevel.iTabCur)
                                continue;
                            if (pOp.OpCode == OpCode.OP_Column)
                            {
                                for (j = 0; j < pIdx.nColumn; j++)
                                {
                                    if (pOp.p2 == pIdx.aiColumn[j])
                                    {
                                        pOp.p2 = j;
                                        pOp.p1 = pLevel.iIdxCur;
                                        break;
                                    }
                                }
                                Debug.Assert((pLevel.plan.wsFlags & WHERE_IDX_ONLY) == 0 || j < pIdx.nColumn);
                            }
                            else
                                if (pOp.OpCode == OpCode.OP_Rowid)
                                {
                                    pOp.p1 = pLevel.iIdxCur;
                                    pOp.OpCode = OpCode.OP_IdxRowid;
                                }
                        }
                    }
                }
                ///
                ///<summary>
                ///Final cleanup
                ///
                ///</summary>

                pParse.nQueryLoop = this.savedNQueryLoop;
                db.whereInfoFree(this);
                return;
            }

            public Bitmask codeOneLoopStart(///
                ///<summary>
                ///Complete information about the WHERE clause 
                ///</summary>

            int iLevel, ///
                ///<summary>
                ///Which level of pWInfo.a[] should be coded 
                ///</summary>

            u16 wctrlFlags, ///
                ///<summary>
                ///One of the WHERE_* flags defined in sqliteInt.h 
                ///</summary>

            Bitmask notReady///
                ///<summary>
                ///Which tables are currently available 
                ///</summary>

            )
            {
                int j, k;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>

                int iCur;
                ///
                ///<summary>
                ///The VDBE cursor for the table 
                ///</summary>

                int addrNxt;
                ///
                ///<summary>
                ///Where to jump to continue with the next IN case 
                ///</summary>

                int omitTable;
                ///
                ///<summary>
                ///True if we use the index only 
                ///</summary>

                int bRev;
                ///
                ///<summary>
                ///True if we need to scan in reverse order 
                ///</summary>

                WhereLevel pLevel;
                ///
                ///<summary>
                ///The where level to be coded 
                ///</summary>

                WhereClause pWC;
                ///
                ///<summary>
                ///Decomposition of the entire WHERE clause 
                ///</summary>

                WhereTerm pTerm;
                ///
                ///<summary>
                ///A WHERE clause term 
                ///</summary>

                Parse pParse;
                ///
                ///<summary>
                ///Parsing context 
                ///</summary>

                Vdbe v;
                ///
                ///<summary>
                ///The prepared stmt under constructions 
                ///</summary>

                SrcList_item pTabItem;
                ///
                ///<summary>
                ///FROM clause term being coded 
                ///</summary>

                int addrBrk;
                ///
                ///<summary>
                ///Jump here to break out of the loop 
                ///</summary>

                int addrCont;
                ///
                ///<summary>
                ///Jump here to continue with next cycle 
                ///</summary>

                int iRowidReg = 0;
                ///
                ///<summary>
                ///Rowid is stored in this register, if not zero 
                ///</summary>

                int iReleaseReg = 0;
                ///
                ///<summary>
                ///Temp register to free before returning 
                ///</summary>

                pParse = this.pParse;
                v = pParse.pVdbe;
                pWC = this.pWC;
                pLevel = this.a[iLevel];
                pTabItem = this.pTabList.a[pLevel.iFrom];
                iCur = pTabItem.iCursor;
                bRev = (pLevel.plan.wsFlags & WHERE_REVERSE) != 0 ? 1 : 0;
                omitTable = ((pLevel.plan.wsFlags & WHERE_IDX_ONLY) != 0 && (wctrlFlags & WHERE_FORCE_TABLE) == 0) ? 1 : 0;
                ///
                ///<summary>
                ///Create labels for the "break" and "continue" instructions
                ///for the current loop.  Jump to addrBrk to break out of a loop.
                ///Jump to cont to go immediately to the next iteration of the
                ///loop.
                ///
                ///When there is an IN operator, we also have a "addrNxt" label that
                ///means to continue with the next IN value combination.  When
                ///there are no IN operators in the constraints, the "addrNxt" label
                ///is the same as "addrBrk".
                ///
                ///</summary>

                addrBrk = pLevel.addrBrk = pLevel.addrNxt = v.sqlite3VdbeMakeLabel();
                addrCont = pLevel.addrCont = v.sqlite3VdbeMakeLabel();
                ///
                ///<summary>
                ///If this is the right table of a LEFT OUTER JOIN, allocate and
                ///initialize a memory cell that records if this table matches any
                ///row of the left table of the join.
                ///
                ///</summary>

                if (pLevel.iFrom > 0 && (pTabItem.jointype & JT_LEFT) != 0)// Check value of pTabItem[0].jointype
                {
                    pLevel.iLeftJoin = ++pParse.nMem;
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, pLevel.iLeftJoin);
#if SQLITE_DEBUG
																																																																																																																			        VdbeComment( v, "init LEFT JOIN no-match flag" );
#endif
                }
#if !SQLITE_OMIT_VIRTUALTABLE
                if ((pLevel.plan.wsFlags & WHERE_VIRTUALTABLE) != 0)
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Case 0:  The table is a virtual">table.  Use the VFilter and VNext</param>
                    ///<param name="to access the data.">to access the data.</param>
                    ///<param name=""></param>

                    int iReg;
                    ///
                    ///<summary>
                    ///P3 Value for OP_VFilter 
                    ///</summary>

                    sqlite3_index_info pVtabIdx = pLevel.plan.u.pVtabIdx;
                    int nConstraint = pVtabIdx.nConstraint;
                    sqlite3_index_constraint_usage[] aUsage = pVtabIdx.aConstraintUsage;
                    sqlite3_index_constraint[] aConstraint = pVtabIdx.aConstraint;
                    pParse.sqlite3ExprCachePush();
                    iReg = pParse.sqlite3GetTempRange(nConstraint + 2);
                    for (j = 1; j <= nConstraint; j++)
                    {
                        for (k = 0; k < nConstraint; k++)
                        {
                            if (aUsage[k].argvIndex == j)
                            {
                                int iTerm = aConstraint[k].iTermOffset;
                                pParse.sqlite3ExprCode(pWC.a[iTerm].pExpr.pRight, iReg + j + 1);
                                break;
                            }
                        }
                        if (k == nConstraint)
                            break;
                    }
                    v.sqlite3VdbeAddOp2(OP_Integer, pVtabIdx.idxNum, iReg);
                    v.sqlite3VdbeAddOp2(OP_Integer, j - 1, iReg + 1);
                    v.sqlite3VdbeAddOp4(OP_VFilter, iCur, addrBrk, iReg, pVtabIdx.idxStr, pVtabIdx.needToFreeIdxStr != 0 ? P4_MPRINTF : P4_STATIC);
                    pVtabIdx.needToFreeIdxStr = 0;
                    for (j = 0; j < nConstraint; j++)
                    {
                        if (aUsage[j].omit != false)
                        {
                            int iTerm = aConstraint[j].iTermOffset;
                            pLevel.disableTerm(pWC.a[iTerm]);
                        }
                    }
                    pLevel.op = OP_VNext;
                    pLevel.p1 = iCur;
                    pLevel.p2 = v.sqlite3VdbeCurrentAddr();
                    pParse.sqlite3ReleaseTempRange(iReg, nConstraint + 2);
                    pParse.sqlite3ExprCachePop(1);
                }
                else
#endif
                    if ((pLevel.plan.wsFlags & WHERE_ROWID_EQ) != 0)
                    {
                        ///
                        ///<summary>
                        ///Case 1:  We can directly reference a single row using an
                        ///equality comparison against the ROWID field.  Or
                        ///we reference multiple rows using a "rowid IN (...)"
                        ///construct.
                        ///
                        ///</summary>

                        iReleaseReg = pParse.sqlite3GetTempReg();
                        pTerm = pWC.findTerm(iCur, -1, notReady, WO_EQ | WO_IN, null);
                        Debug.Assert(pTerm != null);
                        Debug.Assert(pTerm.pExpr != null);
                        Debug.Assert(pTerm.leftCursor == iCur);
                        Debug.Assert(omitTable == 0);
                        testcase(pTerm.wtFlags & TERM_VIRTUAL);
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="EV: R">11662 </param>

                        iRowidReg = pParse.codeEqualityTerm(pTerm, pLevel, iReleaseReg);
                        addrNxt = pLevel.addrNxt;
                        v.sqlite3VdbeAddOp2(OP_MustBeInt, iRowidReg, addrNxt);
                        v.sqlite3VdbeAddOp3(OP_NotExists, iCur, addrNxt, iRowidReg);
                        pParse.sqlite3ExprCacheStore(iCur, -1, iRowidReg);
#if SQLITE_DEBUG
																																																																																																																																														          VdbeComment( v, "pk" );
#endif
                        pLevel.op = OP_Noop;
                    }
                    else
                        if ((pLevel.plan.wsFlags & WHERE_ROWID_RANGE) != 0)
                        {
                            ///
                            ///<summary>
                            ///Case 2:  We have an inequality comparison against the ROWID field.
                            ///
                            ///</summary>

                            int testOp = OP_Noop;
                            int start;
                            int memEndValue = 0;
                            WhereTerm pStart, pEnd;
                            Debug.Assert(omitTable == 0);
                            pStart = pWC.findTerm(iCur, -1, notReady, WO_GT | WO_GE, null);
                            pEnd = pWC.findTerm(iCur, -1, notReady, WO_LT | WO_LE, null);
                            if (bRev != 0)
                            {
                                pTerm = pStart;
                                pStart = pEnd;
                                pEnd = pTerm;
                            }
                            if (pStart != null)
                            {
                                Expr pX;
                                ///
                                ///<summary>
                                ///The expression that defines the start bound 
                                ///</summary>

                                int r1, rTemp = 0;
                                ///
                                ///<summary>
                                ///Registers for holding the start boundary 
                                ///</summary>

                                ///
                                ///<summary>
                                ///The following constant maps TK_xx codes into corresponding
                                ///seek opcodes.  It depends on a particular ordering of TK_xx
                                ///
                                ///</summary>

                                u8[] aMoveOp = new u8[] {
									///
///<summary>
///TK_GT 
///</summary>

									OP_SeekGt,
									///
///<summary>
///TK_LE 
///</summary>

									OP_SeekLe,
									///
///<summary>
///TK_LT 
///</summary>

									OP_SeekLt,
									///
///<summary>
///TK_GE 
///</summary>

									OP_SeekGe
								};
                                Debug.Assert(TK_LE == TK_GT + 1);
                                ///
                                ///<summary>
                                ///Make sure the ordering.. 
                                ///</summary>

                                Debug.Assert(TK_LT == TK_GT + 2);
                                ///
                                ///<summary>
                                ///... of the TK_xx values... 
                                ///</summary>

                                Debug.Assert(TK_GE == TK_GT + 3);
                                ///
                                ///<summary>
                                ///... is correcct. 
                                ///</summary>

                                testcase(pStart.wtFlags & TERM_VIRTUAL);
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="EV: R">11662 </param>

                                pX = pStart.pExpr;
                                Debug.Assert(pX != null);
                                Debug.Assert(pStart.leftCursor == iCur);
                                r1 = pParse.sqlite3ExprCodeTemp(pX.pRight, ref rTemp);
                                v.sqlite3VdbeAddOp3(aMoveOp[pX.op - TK_GT], iCur, addrBrk, r1);
#if SQLITE_DEBUG
																																																																																																																																																																																																				            VdbeComment( v, "pk" );
#endif
                                pParse.sqlite3ExprCacheAffinityChange(r1, 1);
                                pParse.sqlite3ReleaseTempReg(rTemp);
                                pLevel.disableTerm(pStart);
                            }
                            else
                            {
                                v.sqlite3VdbeAddOp2(bRev != 0 ? OP_Last : OP_Rewind, iCur, addrBrk);
                            }
                            if (pEnd != null)
                            {
                                Expr pX;
                                pX = pEnd.pExpr;
                                Debug.Assert(pX != null);
                                Debug.Assert(pEnd.leftCursor == iCur);
                                testcase(pEnd.wtFlags & TERM_VIRTUAL);
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="EV: R">11662 </param>

                                memEndValue = ++pParse.nMem;
                                pParse.sqlite3ExprCode(pX.pRight, memEndValue);
                                if (pX.op == TK_LT || pX.op == TK_GT)
                                {
                                    testOp = bRev != 0 ? OP_Le : OP_Ge;
                                }
                                else
                                {
                                    testOp = bRev != 0 ? OP_Lt : OP_Gt;
                                }
                                pLevel.disableTerm(pEnd);
                            }
                            start = v.sqlite3VdbeCurrentAddr();
                            pLevel.op = (u8)(bRev != 0 ? OP_Prev : OP_Next);
                            pLevel.p1 = iCur;
                            pLevel.p2 = start;
                            if (pStart == null && pEnd == null)
                            {
                                pLevel.p5 = SQLITE_STMTSTATUS_FULLSCAN_STEP;
                            }
                            else
                            {
                                Debug.Assert(pLevel.p5 == 0);
                            }
                            if (testOp != OP_Noop)
                            {
                                iRowidReg = iReleaseReg = pParse.sqlite3GetTempReg();
                                v.sqlite3VdbeAddOp2(OP_Rowid, iCur, iRowidReg);
                                pParse.sqlite3ExprCacheStore(iCur, -1, iRowidReg);
                                v.sqlite3VdbeAddOp3(testOp, memEndValue, addrBrk, iRowidReg);
                                v.sqlite3VdbeChangeP5(SQLITE_AFF_NUMERIC | SQLITE_JUMPIFNULL);
                            }
                        }
                        else
                            if ((pLevel.plan.wsFlags & (WHERE_COLUMN_RANGE | WHERE_COLUMN_EQ)) != 0)
                            {
                                ///
                                ///<summary>
                                ///Case 3: A scan using an index.
                                ///
                                ///The WHERE clause may contain zero or more equality
                                ///terms ("==" or "IN" operators) that refer to the N
                                ///</summary>
                                ///<param name="left">most columns of the index. It may also contain</param>
                                ///<param name="inequality constraints (>, <, >= or <=) on the indexed">inequality constraints (>, <, >= or <=) on the indexed</param>
                                ///<param name="column that immediately follows the N equalities. Only">column that immediately follows the N equalities. Only</param>
                                ///<param name="the right"> the rest must</param>
                                ///<param name="use the "==" and "IN" operators. For example, if the">use the "==" and "IN" operators. For example, if the</param>
                                ///<param name="index is on (x,y,z), then the following clauses are all">index is on (x,y,z), then the following clauses are all</param>
                                ///<param name="optimized:">optimized:</param>
                                ///<param name=""></param>
                                ///<param name="x=5">x=5</param>
                                ///<param name="x=5 AND y=10">x=5 AND y=10</param>
                                ///<param name="x=5 AND y<10">x=5 AND y<10</param>
                                ///<param name="x=5 AND y>5 AND y<10">x=5 AND y>5 AND y<10</param>
                                ///<param name="x=5 AND y=5 AND z<=10">x=5 AND y=5 AND z<=10</param>
                                ///<param name=""></param>
                                ///<param name="The z<10 term of the following cannot be used, only">The z<10 term of the following cannot be used, only</param>
                                ///<param name="the x=5 term:">the x=5 term:</param>
                                ///<param name=""></param>
                                ///<param name="x=5 AND z<10">x=5 AND z<10</param>
                                ///<param name=""></param>
                                ///<param name="N may be zero if there are inequality constraints.">N may be zero if there are inequality constraints.</param>
                                ///<param name="If there are no inequality constraints, then N is at">If there are no inequality constraints, then N is at</param>
                                ///<param name="least one.">least one.</param>
                                ///<param name=""></param>
                                ///<param name="This case is also used when there are no WHERE clause">This case is also used when there are no WHERE clause</param>
                                ///<param name="constraints but an index is selected anyway, in order">constraints but an index is selected anyway, in order</param>
                                ///<param name="to force the output order to conform to an ORDER BY.">to force the output order to conform to an ORDER BY.</param>
                                ///<param name=""></param>

                                u8[] aStartOp = new u8[] {
									0,
									0,
									OP_Rewind,
									///
///<summary>
///2: (!start_constraints && startEq &&  !bRev) 
///</summary>

									OP_Last,
									///
///<summary>
///3: (!start_constraints && startEq &&   bRev) 
///</summary>

									OP_SeekGt,
									///
///<summary>
///4: (start_constraints  && !startEq && !bRev) 
///</summary>

									OP_SeekLt,
									///
///<summary>
///5: (start_constraints  && !startEq &&  bRev) 
///</summary>

									OP_SeekGe,
									///
///<summary>
///6: (start_constraints  &&  startEq && !bRev) 
///</summary>

									OP_SeekLe
								///
///<summary>
///7: (start_constraints  &&  startEq &&  bRev) 
///</summary>

								};
                                u8[] aEndOp = new u8[] {
									OP_Noop,
									///
///<summary>
///0: (!end_constraints) 
///</summary>

									OP_IdxGE,
									///
///<summary>
///1: (end_constraints && !bRev) 
///</summary>

									OP_IdxLT
								///
///<summary>
///2: (end_constraints && bRev) 
///</summary>

								};
                                int nEq = (int)pLevel.plan.nEq;
                                ///
                                ///<summary>
                                ///Number of == or IN terms 
                                ///</summary>

                                int isMinQuery = 0;
                                ///
                                ///<summary>
                                ///If this is an optimized SELECT min(x).. 
                                ///</summary>

                                int regBase;
                                ///
                                ///<summary>
                                ///Base register holding constraint values 
                                ///</summary>

                                int r1;
                                ///
                                ///<summary>
                                ///Temp register 
                                ///</summary>

                                WhereTerm pRangeStart = null;
                                ///
                                ///<summary>
                                ///Inequality constraint at range start 
                                ///</summary>

                                WhereTerm pRangeEnd = null;
                                ///
                                ///<summary>
                                ///Inequality constraint at range end 
                                ///</summary>

                                int startEq;
                                ///
                                ///<summary>
                                ///True if range start uses ==, >= or <= 
                                ///</summary>

                                int endEq;
                                ///
                                ///<summary>
                                ///True if range end uses ==, >= or <= 
                                ///</summary>

                                int start_constraints;
                                ///
                                ///<summary>
                                ///Start of range is constrained 
                                ///</summary>

                                int nConstraint;
                                ///
                                ///<summary>
                                ///Number of constraint terms 
                                ///</summary>

                                Index pIdx;
                                ///
                                ///<summary>
                                ///The index we will be using 
                                ///</summary>

                                int iIdxCur;
                                ///
                                ///<summary>
                                ///The VDBE cursor for the index 
                                ///</summary>

                                int nExtraReg = 0;
                                ///
                                ///<summary>
                                ///Number of extra registers needed 
                                ///</summary>

                                int op;
                                ///
                                ///<summary>
                                ///Instruction opcode 
                                ///</summary>

                                StringBuilder zStartAff = new StringBuilder("");
                                ;
                                ///
                                ///<summary>
                                ///Affinity for start of range constraint 
                                ///</summary>

                                StringBuilder zEndAff;
                                ///
                                ///<summary>
                                ///Affinity for end of range constraint 
                                ///</summary>

                                pIdx = pLevel.plan.u.pIdx;
                                iIdxCur = pLevel.iIdxCur;
                                k = pIdx.aiColumn[nEq];
                                ///
                                ///<summary>
                                ///Column for inequality constraints 
                                ///</summary>

                                ///
                                ///<summary>
                                ///If this loop satisfies a sort order (pOrderBy) request that
                                ///was pDebug.Assed to this function to implement a "SELECT min(x) ..."
                                ///query, then the caller will only allow the loop to run for
                                ///a single iteration. This means that the first row returned
                                ///should not have a NULL value stored in 'x'. If column 'x' is
                                ///the first one after the nEq equality constraints in the index,
                                ///this requires some special handling.
                                ///
                                ///</summary>

                                if ((wctrlFlags & WHERE_ORDERBY_MIN) != 0 && ((pLevel.plan.wsFlags & WHERE_ORDERBY) != 0) && (pIdx.nColumn > nEq))
                                {
                                    ///
                                    ///<summary>
                                    ///Debug.Assert( pOrderBy.nExpr==1 ); 
                                    ///</summary>

                                    ///
                                    ///<summary>
                                    ///Debug.Assert( pOrderBy.a[0].pExpr.iColumn==pIdx.aiColumn[nEq] ); 
                                    ///</summary>

                                    isMinQuery = 1;
                                    nExtraReg = 1;
                                }
                                ///
                                ///<summary>
                                ///Find any inequality constraint terms for the start and end
                                ///of the range.
                                ///
                                ///</summary>

                                if ((pLevel.plan.wsFlags & WHERE_TOP_LIMIT) != 0)
                                {
                                    pRangeEnd = pWC.findTerm(iCur, k, notReady, (WO_LT | WO_LE), pIdx);
                                    nExtraReg = 1;
                                }
                                if ((pLevel.plan.wsFlags & WHERE_BTM_LIMIT) != 0)
                                {
                                    pRangeStart = pWC.findTerm(iCur, k, notReady, (WO_GT | WO_GE), pIdx);
                                    nExtraReg = 1;
                                }
                                ///
                                ///<summary>
                                ///Generate code to evaluate all constraint terms using == or IN
                                ///and store the values of those terms in an array of registers
                                ///starting at regBase.
                                ///
                                ///</summary>

                                regBase = pParse.codeAllEqualityTerms(pLevel, pWC, notReady, nExtraReg, out zStartAff);
                                zEndAff = new StringBuilder(zStartAff.ToString());
                                //sqlite3DbStrDup(pParse.db, zStartAff);
                                addrNxt = pLevel.addrNxt;
                                ///
                                ///<summary>
                                ///If we are doing a reverse order scan on an ascending index, or
                                ///a forward order scan on a descending index, interchange the
                                ///start and end terms (pRangeStart and pRangeEnd).
                                ///
                                ///</summary>

                                if (nEq < pIdx.nColumn && bRev == (pIdx.aSortOrder[nEq] == SQLITE_SO_ASC ? 1 : 0))
                                {
                                    SWAP(ref pRangeEnd, ref pRangeStart);
                                }
                                testcase(pRangeStart != null && (pRangeStart.eOperator & WO_LE) != 0);
                                testcase(pRangeStart != null && (pRangeStart.eOperator & WO_GE) != 0);
                                testcase(pRangeEnd != null && (pRangeEnd.eOperator & WO_LE) != 0);
                                testcase(pRangeEnd != null && (pRangeEnd.eOperator & WO_GE) != 0);
                                startEq = (null == pRangeStart || (pRangeStart.eOperator & (WO_LE | WO_GE)) != 0) ? 1 : 0;
                                endEq = (null == pRangeEnd || (pRangeEnd.eOperator & (WO_LE | WO_GE)) != 0) ? 1 : 0;
                                start_constraints = (pRangeStart != null || nEq > 0) ? 1 : 0;
                                ///
                                ///<summary>
                                ///Seek the index cursor to the start of the range. 
                                ///</summary>

                                nConstraint = nEq;
                                if (pRangeStart != null)
                                {
                                    Expr pRight = pRangeStart.pExpr.pRight;
                                    pParse.sqlite3ExprCode(pRight, regBase + nEq);
                                    if ((pRangeStart.wtFlags & TERM_VNULL) == 0)
                                    {
                                        sqlite3ExprCodeIsNullJump(v, pRight, regBase + nEq, addrNxt);
                                    }
                                    if (zStartAff.Length != 0)
                                    {
                                        if (pRight.sqlite3CompareAffinity(zStartAff[nEq]) == SQLITE_AFF_NONE)
                                        {
                                            ///
                                            ///<summary>
                                            ///Since the comparison is to be performed with no conversions
                                            ///applied to the operands, set the affinity to apply to pRight to 
                                            ///SQLITE_AFF_NONE.  
                                            ///</summary>

                                            zStartAff[nEq] = SQLITE_AFF_NONE;
                                        }
                                        if ((sqlite3ExprNeedsNoAffinityChange(pRight, zStartAff[nEq])) != 0)
                                        {
                                            zStartAff[nEq] = SQLITE_AFF_NONE;
                                        }
                                    }
                                    nConstraint++;
                                    testcase(pRangeStart.wtFlags & TERM_VIRTUAL);
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="EV: R">11662 </param>

                                }
                                else
                                    if (isMinQuery != 0)
                                    {
                                        v.sqlite3VdbeAddOp2(OP_Null, 0, regBase + nEq);
                                        nConstraint++;
                                        startEq = 0;
                                        start_constraints = 1;
                                    }
                                pParse.codeApplyAffinity(regBase, nConstraint, zStartAff.ToString());
                                op = aStartOp[(start_constraints << 2) + (startEq << 1) + bRev];
                                Debug.Assert(op != 0);
                                testcase(op == OP_Rewind);
                                testcase(op == OP_Last);
                                testcase(op == OP_SeekGt);
                                testcase(op == OP_SeekGe);
                                testcase(op == OP_SeekLe);
                                testcase(op == OP_SeekLt);
                                v.sqlite3VdbeAddOp4Int(op, iIdxCur, addrNxt, regBase, nConstraint);
                                ///
                                ///<summary>
                                ///Load the value for the inequality constraint at the end of the
                                ///range (if any).
                                ///
                                ///</summary>

                                nConstraint = nEq;
                                if (pRangeEnd != null)
                                {
                                    Expr pRight = pRangeEnd.pExpr.pRight;
                                    pParse.sqlite3ExprCacheRemove(regBase + nEq, 1);
                                    pParse.sqlite3ExprCode(pRight, regBase + nEq);
                                    if ((pRangeEnd.wtFlags & TERM_VNULL) == 0)
                                    {
                                        sqlite3ExprCodeIsNullJump(v, pRight, regBase + nEq, addrNxt);
                                    }
                                    if (zEndAff.Length > 0)
                                    {
                                        if (pRight.sqlite3CompareAffinity(zEndAff[nEq]) == SQLITE_AFF_NONE)
                                        {
                                            ///
                                            ///<summary>
                                            ///Since the comparison is to be performed with no conversions
                                            ///applied to the operands, set the affinity to apply to pRight to 
                                            ///SQLITE_AFF_NONE.  
                                            ///</summary>

                                            zEndAff[nEq] = SQLITE_AFF_NONE;
                                        }
                                        if ((sqlite3ExprNeedsNoAffinityChange(pRight, zEndAff[nEq])) != 0)
                                        {
                                            zEndAff[nEq] = SQLITE_AFF_NONE;
                                        }
                                    }
                                    pParse.codeApplyAffinity(regBase, nEq + 1, zEndAff.ToString());
                                    nConstraint++;
                                    testcase(pRangeEnd.wtFlags & TERM_VIRTUAL);
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="EV: R">11662 </param>

                                }
                                pParse.db.sqlite3DbFree(ref zStartAff);
                                pParse.db.sqlite3DbFree(ref zEndAff);
                                ///
                                ///<summary>
                                ///Top of the loop body 
                                ///</summary>

                                pLevel.p2 = v.sqlite3VdbeCurrentAddr();
                                ///
                                ///<summary>
                                ///Check if the index cursor is past the end of the range. 
                                ///</summary>

                                op = aEndOp[((pRangeEnd != null || nEq != 0) ? 1 : 0) * (1 + bRev)];
                                testcase(op == OP_Noop);
                                testcase(op == OP_IdxGE);
                                testcase(op == OP_IdxLT);
                                if (op != OP_Noop)
                                {
                                    v.sqlite3VdbeAddOp4Int(op, iIdxCur, addrNxt, regBase, nConstraint);
                                    v.sqlite3VdbeChangeP5((u8)(endEq != bRev ? 1 : 0));
                                }
                                ///
                                ///<summary>
                                ///If there are inequality constraints, check that the value
                                ///of the table column that the inequality contrains is not NULL.
                                ///If it is, jump to the next iteration of the loop.
                                ///
                                ///</summary>

                                r1 = pParse.sqlite3GetTempReg();
                                testcase(pLevel.plan.wsFlags & WHERE_BTM_LIMIT);
                                testcase(pLevel.plan.wsFlags & WHERE_TOP_LIMIT);
                                if ((pLevel.plan.wsFlags & (WHERE_BTM_LIMIT | WHERE_TOP_LIMIT)) != 0)
                                {
                                    v.sqlite3VdbeAddOp3(OP_Column, iIdxCur, nEq, r1);
                                    v.sqlite3VdbeAddOp2(OP_IsNull, r1, addrCont);
                                }
                                pParse.sqlite3ReleaseTempReg(r1);
                                ///
                                ///<summary>
                                ///Seek the table cursor, if required 
                                ///</summary>

                                pLevel.disableTerm(pRangeStart);
                                pLevel.disableTerm(pRangeEnd);
                                if (0 == omitTable)
                                {
                                    iRowidReg = iReleaseReg = pParse.sqlite3GetTempReg();
                                    v.sqlite3VdbeAddOp2(OP_IdxRowid, iIdxCur, iRowidReg);
                                    pParse.sqlite3ExprCacheStore(iCur, -1, iRowidReg);
                                    v.sqlite3VdbeAddOp2(OP_Seek, iCur, iRowidReg);
                                    ///
                                    ///<summary>
                                    ///Deferred seek 
                                    ///</summary>

                                }
                                ///
                                ///<summary>
                                ///Record the instruction used to terminate the loop. Disable
                                ///WHERE clause terms made redundant by the index range scan.
                                ///
                                ///</summary>

                                if ((pLevel.plan.wsFlags & WHERE_UNIQUE) != 0)
                                {
                                    pLevel.op = OP_Noop;
                                }
                                else
                                    if (bRev != 0)
                                    {
                                        pLevel.op = OP_Prev;
                                    }
                                    else
                                    {
                                        pLevel.op = OP_Next;
                                    }
                                pLevel.p1 = iIdxCur;
                            }
                            else
#if !SQLITE_OMIT_OR_OPTIMIZATION
                                if ((pLevel.plan.wsFlags & WHERE_MULTI_OR) != 0)
                                {
                                    ///
                                    ///<summary>
                                    ///Case 4:  Two or more separately indexed terms connected by OR
                                    ///
                                    ///Example:
                                    ///
                                    ///CREATE TABLE t1(a,b,c,d);
                                    ///CREATE INDEX i1 ON t1(a);
                                    ///CREATE INDEX i2 ON t1(b);
                                    ///CREATE INDEX i3 ON t1(c);
                                    ///
                                    ///SELECT * FROM t1 WHERE a=5 OR b=7 OR (c=11 AND d=13)
                                    ///
                                    ///In the example, there are three indexed terms connected by OR.
                                    ///The top of the loop looks like this:
                                    ///
                                    ///Null       1                # Zero the rowset in reg 1
                                    ///
                                    ///Then, for each indexed term, the following. The arguments to
                                    ///RowSetTest are such that the rowid of the current row is inserted
                                    ///into the RowSet. If it is already present, control skips the
                                    ///Gosub opcode and jumps straight to the code generated by WhereEnd().
                                    ///
                                    ///sqlite3WhereBegin(<term>)
                                    ///RowSetTest                  # Insert rowid into rowset
                                    ///Gosub      2 A
                                    ///sqlite3WhereEnd()
                                    ///
                                    ///Following the above, code to terminate the loop. Label A, the target
                                    ///of the Gosub above, jumps to the instruction right after the Goto.
                                    ///
                                    ///Null       1                # Zero the rowset in reg 1
                                    ///Goto       B                # The loop is finished.
                                    ///
                                    ///A: <loop body>                 # Return data, whatever.
                                    ///
                                    ///Return     2                # Jump back to the Gosub
                                    ///
                                    ///B: <after the loop>
                                    ///
                                    ///
                                    ///</summary>

                                    WhereClause pOrWc;
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="The OR">clause broken out into subterms </param>

                                    SrcList pOrTab;
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="Shortened table list or OR">clause generation </param>

                                    int regReturn = ++pParse.nMem;
                                    ///
                                    ///<summary>
                                    ///Register used with OP_Gosub 
                                    ///</summary>

                                    int regRowset = 0;
                                    ///
                                    ///<summary>
                                    ///Register for RowSet object 
                                    ///</summary>

                                    int regRowid = 0;
                                    ///
                                    ///<summary>
                                    ///Register holding rowid 
                                    ///</summary>

                                    int iLoopBody = v.sqlite3VdbeMakeLabel();
                                    ///
                                    ///<summary>
                                    ///Start of loop body 
                                    ///</summary>

                                    int iRetInit;
                                    ///
                                    ///<summary>
                                    ///Address of regReturn init 
                                    ///</summary>

                                    int untestedTerms = 0;
                                    ///
                                    ///<summary>
                                    ///Some terms not completely tested 
                                    ///</summary>

                                    int ii;
                                    pTerm = pLevel.plan.u.pTerm;
                                    Debug.Assert(pTerm != null);
                                    Debug.Assert(pTerm.eOperator == WO_OR);
                                    Debug.Assert((pTerm.wtFlags & TERM_ORINFO) != 0);
                                    pOrWc = pTerm.u.pOrInfo.wc;
                                    pLevel.op = OP_Return;
                                    pLevel.p1 = regReturn;
                                    ///
                                    ///<summary>
                                    ///Set up a new SrcList in pOrTab containing the table being scanned
                                    ///by this loop in the a[0] slot and all notReady tables in a[1..] slots.
                                    ///This becomes the SrcList in the recursive call to sqlite3WhereBegin().
                                    ///
                                    ///</summary>

                                    if (this.nLevel > 1)
                                    {
                                        int nNotReady;
                                        ///
                                        ///<summary>
                                        ///The number of notReady tables 
                                        ///</summary>

                                        SrcList_item[] origSrc;
                                        ///
                                        ///<summary>
                                        ///Original list of tables 
                                        ///</summary>

                                        nNotReady = this.nLevel - iLevel - 1;
                                        //sqlite3StackAllocRaw(pParse.db,
                                        //sizeof(*pOrTab)+ nNotReady*sizeof(pOrTab.a[0]));
                                        pOrTab = new SrcList();
                                        pOrTab.a = new SrcList_item[nNotReady + 1];
                                        //if( pOrTab==0 ) return notReady;
                                        pOrTab.nAlloc = (i16)(nNotReady + 1);
                                        pOrTab.nSrc = pOrTab.nAlloc;
                                        pOrTab.a[0] = pTabItem;
                                        //memcpy(pOrTab.a, pTabItem, sizeof(*pTabItem));
                                        origSrc = this.pTabList.a;
                                        for (k = 1; k <= nNotReady; k++)
                                        {
                                            pOrTab.a[k] = origSrc[this.a[iLevel + k].iFrom];
                                            // memcpy(&pOrTab.a[k], &origSrc[pLevel[k].iFrom], sizeof(pOrTab.a[k]));
                                        }
                                    }
                                    else
                                    {
                                        pOrTab = this.pTabList;
                                    }
                                    ///
                                    ///<summary>
                                    ///Initialize the rowset register to contain NULL. An SQL NULL is
                                    ///equivalent to an empty rowset.
                                    ///
                                    ///Also initialize regReturn to contain the address of the instruction
                                    ///immediately following the OP_Return at the bottom of the loop. This
                                    ///is required in a few obscure LEFT JOIN cases where control jumps
                                    ///over the top of the loop into the body of it. In this case the
                                    ///</summary>
                                    ///<param name="correct response for the end">loop code (the OP_Return) is to</param>
                                    ///<param name="fall through to the next instruction, just as an OP_Next does if">fall through to the next instruction, just as an OP_Next does if</param>
                                    ///<param name="called on an uninitialized cursor.">called on an uninitialized cursor.</param>
                                    ///<param name=""></param>

                                    if ((wctrlFlags & WHERE_DUPLICATES_OK) == 0)
                                    {
                                        regRowset = ++pParse.nMem;
                                        regRowid = ++pParse.nMem;
                                        v.sqlite3VdbeAddOp2(OP_Null, 0, regRowset);
                                    }
                                    iRetInit = v.sqlite3VdbeAddOp2(OP_Integer, 0, regReturn);
                                    for (ii = 0; ii < pOrWc.nTerm; ii++)
                                    {
                                        WhereTerm pOrTerm = pOrWc.a[ii];
                                        if (pOrTerm.leftCursor == iCur || pOrTerm.eOperator == WO_AND)
                                        {
                                            WhereInfo pSubWInfo;
                                            ///
                                            ///<summary>
                                            ///</summary>
                                            ///<param name="Info for single OR">term scan </param>

                                            ///
                                            ///<summary>
                                            ///Loop through table entries that match term pOrTerm. 
                                            ///</summary>

                                            ExprList elDummy = null;
                                            pSubWInfo = pParse.sqlite3WhereBegin(pOrTab, pOrTerm.pExpr, ref elDummy, WHERE_OMIT_OPEN | WHERE_OMIT_CLOSE | WHERE_FORCE_TABLE | WHERE_ONETABLE_ONLY);
                                            if (pSubWInfo != null)
                                            {
                                                pParse.explainOneScan(pOrTab, pSubWInfo.a[0], iLevel, pLevel.iFrom, 0);
                                                if ((wctrlFlags & WHERE_DUPLICATES_OK) == 0)
                                                {
                                                    int iSet = ((ii == pOrWc.nTerm - 1) ? -1 : ii);
                                                    int r;
                                                    r = pParse.sqlite3ExprCodeGetColumn(pTabItem.pTab, -1, iCur, regRowid);
                                                    v.sqlite3VdbeAddOp4Int(OP_RowSetTest, regRowset, v.sqlite3VdbeCurrentAddr() + 2, r, iSet);
                                                }
                                                v.sqlite3VdbeAddOp2(OP_Gosub, regReturn, iLoopBody);
                                                ///
                                                ///<summary>
                                                ///The pSubWInfo.untestedTerms flag means that this OR term
                                                ///contained one or more AND term from a notReady table.  The
                                                ///terms from the notReady table could not be tested and will
                                                ///need to be tested later.
                                                ///
                                                ///</summary>

                                                if (pSubWInfo.untestedTerms != 0)
                                                    untestedTerms = 1;
                                                ///
                                                ///<summary>
                                                ///Finish the loop through table entries that match term pOrTerm. 
                                                ///</summary>

                                                pSubWInfo.sqlite3WhereEnd();
                                            }
                                        }
                                    }
                                    v.sqlite3VdbeChangeP1(iRetInit, v.sqlite3VdbeCurrentAddr());
                                    v.sqlite3VdbeAddOp2(OP_Goto, 0, pLevel.addrBrk);
                                    v.sqlite3VdbeResolveLabel(iLoopBody);
                                    if (this.nLevel > 1)
                                        pParse.db.sqlite3DbFree(ref pOrTab);
                                    //sqlite3DbFree(pParse.db, pOrTab)
                                    if (0 == untestedTerms)
                                        pLevel.disableTerm(pTerm);
                                }
                                else
#endif
                                {
                                    ///
                                    ///<summary>
                                    ///Case 5:  There is no usable index.  We must do a complete
                                    ///scan of the entire table.
                                    ///
                                    ///</summary>

                                    u8[] aStep = new u8[] {
										OP_Next,
										OP_Prev
									};
                                    u8[] aStart = new u8[] {
										OP_Rewind,
										OP_Last
									};
                                    Debug.Assert(bRev == 0 || bRev == 1);
                                    Debug.Assert(omitTable == 0);
                                    pLevel.op = aStep[bRev];
                                    pLevel.p1 = iCur;
                                    pLevel.p2 = 1 + v.sqlite3VdbeAddOp2(aStart[bRev], iCur, addrBrk);
                                    pLevel.p5 = SQLITE_STMTSTATUS_FULLSCAN_STEP;
                                }
                notReady &= ~pWC.pMaskSet.getMask(iCur);
                ///
                ///<summary>
                ///Insert code to test every subexpression that can be completely
                ///computed using the current set of tables.
                ///
                ///</summary>
                ///<param name="IMPLEMENTATION">50935 Terms that cannot be satisfied through</param>
                ///<param name="the use of indices become tests that are evaluated against each row of">the use of indices become tests that are evaluated against each row of</param>
                ///<param name="the relevant input tables.">the relevant input tables.</param>
                ///<param name=""></param>

                for (j = pWC.nTerm; j > 0; j--)//, pTerm++)
                {
                    pTerm = pWC.a[pWC.nTerm - j];
                    Expr pE;
                    testcase(pTerm.wtFlags & TERM_VIRTUAL);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="IMP: R">11662 </param>

                    testcase(pTerm.wtFlags & TERM_CODED);
                    if ((pTerm.wtFlags & (TERM_VIRTUAL | TERM_CODED)) != 0)
                        continue;
                    if ((pTerm.prereqAll & notReady) != 0)
                    {
                        testcase(this.untestedTerms == 0 && (this.wctrlFlags & WHERE_ONETABLE_ONLY) != 0);
                        this.untestedTerms = 1;
                        continue;
                    }
                    pE = pTerm.pExpr;
                    Debug.Assert(pE != null);
                    if (pLevel.iLeftJoin != 0 && !((pE.flags & EP_FromJoin) == EP_FromJoin))// !ExprHasProperty(pE, EP_FromJoin) ){
                    {
                        continue;
                    }
                    pParse.sqlite3ExprIfFalse(pE, addrCont, SQLITE_JUMPIFNULL);
                    pTerm.wtFlags |= TERM_CODED;
                }
                ///
                ///<summary>
                ///For a LEFT OUTER JOIN, generate code that will record the fact that
                ///at least one row of the right table has matched the left table.
                ///
                ///</summary>

                if (pLevel.iLeftJoin != 0)
                {
                    pLevel.addrFirst = v.sqlite3VdbeCurrentAddr();
                    v.sqlite3VdbeAddOp2(OP_Integer, 1, pLevel.iLeftJoin);
#if SQLITE_DEBUG
																																																																																																																			        VdbeComment( v, "record LEFT JOIN hit" );
#endif
                    pParse.sqlite3ExprCacheClear();
                    for (j = 0; j < pWC.nTerm; j++)//, pTerm++)
                    {
                        pTerm = pWC.a[j];
                        testcase(pTerm.wtFlags & TERM_VIRTUAL);
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="IMP: R">11662 </param>

                        testcase(pTerm.wtFlags & TERM_CODED);
                        if ((pTerm.wtFlags & (TERM_VIRTUAL | TERM_CODED)) != 0)
                            continue;
                        if ((pTerm.prereqAll & notReady) != 0)
                        {
                            Debug.Assert(this.untestedTerms != 0);
                            continue;
                        }
                        Debug.Assert(pTerm.pExpr != null);
                        pParse.sqlite3ExprIfFalse(pTerm.pExpr, addrCont, SQLITE_JUMPIFNULL);
                        pTerm.wtFlags |= TERM_CODED;
                    }
                }
                pParse.sqlite3ReleaseTempReg(iReleaseReg);
                return notReady;
            }
        }




    }
}
