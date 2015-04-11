using System.Diagnostics;
using System.IO;
using System.Text;
using FILE=System.IO.TextWriter;
using i16=System.Int16;
using i32=System.Int32;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using Pgno=System.UInt32;
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
    using Op = VdbeOp;
    using sqlite3_stmt = Sqlite3.Vdbe;
    using sqlite3_value = Mem;
    using Vdbe = Sqlite3.Vdbe;
    using System;
    using BTreeMethods=Sqlite3.BTreeMethods;     
    using System.Collections.Generic;
    using Parse = Sqlite3.Parse;
    using Btree = Sqlite3.Btree;
    using os = Sqlite3.os;
    using BtCursor = Sqlite3.BtCursor;
    //public partial class Sqlite3
    //{

        public static class  vdbeaux
        {
            ///
            ///<summary>
            ///2003 September 6
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///This file contains code used for creating, destroying, and populating
            ///a VDBE (or an "sqlite3_stmt" as it is known to the outside world.)  Prior
            ///to version 2.8.7, all this code was combined into the vdbe.c source file.
            ///But that file was getting too big so this subroutines were split out.
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>
            //#include "sqliteInt.h"
            //#include "vdbeInt.h"
            ///<summary>
            /// When debugging the code generator in a symbolic debugger, one can
            /// set the sqlite3VdbeAddopTrace to 1 and all opcodes will be printed
            /// as they are added to the instruction stream.
            ///
            ///</summary>
#if SQLITE_DEBUG
																																																    static bool sqlite3VdbeAddopTrace = false;
#endif

            ///<summary>
            /// Remember the SQL string for a prepared statement.
            ///
            ///</summary>
            public static void sqlite3VdbeSetSql(Vdbe p, string z, int n, int isPrepareV2)
            {
                Debug.Assert(isPrepareV2 == 1 || isPrepareV2 == 0);
                if (p == null)
                    return;
#if SQLITE_OMIT_TRACE
																																																																								if( 0==isPrepareV2 ) return;
#endif
                Debug.Assert(p.zSql == "");
                p.zSql = z.Substring(0, n);
                // sqlite3DbStrNDup(p.db, z, n);
                p.isPrepareV2 = isPrepareV2 != 0;
            }
            ///<summary>
            /// Return the SQL associated with a prepared statement
            ///
            ///</summary>
            public static string sqlite3_sql(sqlite3_stmt pStmt)
            {
                Vdbe p = (Vdbe)pStmt;
                return (p != null && p.isPrepareV2 ? p.zSql : "");
            }
            ///<summary>
            /// Swap all content between two VDBE structures.
            ///
            ///</summary>
            public static void sqlite3VdbeSwap(Vdbe pA, Vdbe pB)
            {
                Vdbe tmp = new Vdbe();
                Vdbe pTmp = new Vdbe();
                string zTmp;
                pA.CopyTo(tmp);
                pB.CopyTo(pA);
                tmp.CopyTo(pB);
                pTmp = pA.pNext;
                pA.pNext = pB.pNext;
                pB.pNext = pTmp;
                pTmp = pA.pPrev;
                pA.pPrev = pB.pPrev;
                pB.pPrev = pTmp;
                zTmp = pA.zSql;
                pA.zSql = pB.zSql;
                pB.zSql = zTmp;
                pB.isPrepareV2 = pA.isPrepareV2;
            }
#if SQLITE_DEBUG
																																																    /*
** Turn tracing on or off
*/
    static void sqlite3VdbeTrace( Vdbe p, FILE trace )
    {
      p.trace = trace;
    }
#endif
            ///<summary>
            /// Resize the Vdbe.aOp array so that it is at least one op larger than
            /// it was.
            ///
            /// If an out-of-memory error occurs while resizing the array, return
            /// SQLITE_NOMEM. In this case Vdbe.aOp and Vdbe.nOpAlloc remain
            /// unchanged (this is so that any opcodes already allocated can be
            /// correctly deallocated along with the rest of the Vdbe).
            ///</summary>
            ///<summary>
            /// Add a new instruction to the list of instructions current in the
            /// VDBE.  Return the address of the new instruction.
            ///
            /// Parameters:
            ///
            ///    p               Pointer to the VDBE
            ///
            ///    op              The opcode for this instruction
            ///
            ///    p1, p2, p3      Operands
            ///
            /// Use the sqlite3VdbeResolveLabel() function to fix an address and
            /// the sqlite3VdbeChangeP4() function to change the value of the P4
            /// operand.
            ///
            ///</summary>
            ///<summary>
            /// Add an opcode that includes the p4 value as a pointer.
            ///
            ///</summary>
            //P4_INT32
            //char
            //StringBuilder
            //String
            //P4_INTARRAY
            //P4_INT64
            //DOUBLE (REAL)
            //FUNCDEF
            //CollSeq
            //KeyInfo
#if !SQLITE_OMIT_VIRTUALTABLE
            //VTable
#endif
            //  static int sqlite3VdbeAddOp4(
            //  Vdbe p,               /* Add the opcode to this VM */
            //  int op,               /* The new opcode */
            //  int p1,               /* The P1 operand */
            //  int p2,               /* The P2 operand */
            //  int p3,               /* The P3 operand */
            //  union_p4 _p4,         /* The P4 operand */
            //  int p4type            /* P4 operand type */
            //)
            //  {
            //    int addr = sqlite3VdbeAddOp3(p, op, p1, p2, p3);
            //    sqlite3VdbeChangeP4(p, addr, _p4, p4type);
            //    return addr;
            //  }
            ///<summary>
            /// Add an OP_ParseSchema opcode.  This routine is broken out from
            /// sqlite3VdbeAddOp4() since it needs to also local all btrees.
            ///
            /// The zWhere string must have been obtained from sqlite3_malloc().
            /// This routine will take ownership of the allocated memory.
            ///
            ///</summary>
            ///<summary>
            /// Add an opcode that includes the p4 value as an integer.
            ///
            ///</summary>
            ///<summary>
            /// Create a new symbolic label for an instruction that has yet to be
            /// coded.  The symbolic label is really just a negative number.  The
            /// label can be used as the P2 value of an operation.  Later, when
            /// the label is resolved to a specific address, the VDBE will scan
            /// through its operation list and change all values of P2 which match
            /// the label into the resolved address.
            ///
            /// The VDBE knows that a P2 value is a label because labels are
            /// always negative and P2 values are suppose to be non-negative.
            /// Hence, a negative P2 value is a label that has yet to be resolved.
            ///
            /// Zero is returned if a malloc() fails.
            ///
            ///</summary>
            ///<summary>
            /// Resolve label "x" to be the address of the next instruction to
            /// be inserted.  The parameter "x" must have been obtained from
            /// a prior call to sqlite3VdbeMakeLabel().
            ///
            ///</summary>
            ///<summary>
            /// Mark the VDBE as one that can only be run one time.
            ///
            ///</summary>
#if SQLITE_DEBUG
																																																
    /*
** The following type and function are used to iterate through all opcodes
** in a Vdbe main program and each of the sub-programs (triggers) it may 
** invoke directly or indirectly. It should be used as follows:
**
**   Op *pOp;
**   VdbeOpIter sIter;
**
**   memset(&sIter, 0, sizeof(sIter));
**   sIter.v = v;                            // v is of type Vdbe* 
**   while( (pOp = opIterNext(&sIter)) ){
**     // Do something with pOp
**   }
**   sqlite3DbFree(v->db, sIter.apSub);
** 
*/
    //typedef struct VdbeOpIter VdbeOpIter;
    public class VdbeOpIter
    {
      public Vdbe v;                    /* Vdbe to iterate through the opcodes of */
      public SubProgram[] apSub;        /* Array of subprograms */
      public int nSub;                  /* Number of entries in apSub */
      public int iAddr;                 /* Address of next instruction to return */
      public int iSub;                  /* 0 = main program, 1 = first sub-program etc. */
    };

    static Op opIterNext( VdbeOpIter p )
    {
      Vdbe v = p.v;
      Op pRet = null;
      Op[] aOp;
      int nOp;

      if ( p.iSub <= p.nSub )
      {

        if ( p.iSub == 0 )
        {
          aOp = v.aOp;
          nOp = v.nOp;
        }
        else
        {
          aOp = p.apSub[p.iSub - 1].aOp;
          nOp = p.apSub[p.iSub - 1].nOp;
        }
        Debug.Assert( p.iAddr < nOp );

        pRet = aOp[p.iAddr];
        p.iAddr++;
        if ( p.iAddr == nOp )
        {
          p.iSub++;
          p.iAddr = 0;
        }

        if ( pRet.p4type ==  P4Usage.P4_SUBPROGRAM )
        {
          //int nByte =  p.nSub + 1 ) * sizeof( SubProgram* );
          int j;
          for ( j = 0; j < p.nSub; j++ )
          {
            if ( p.apSub[j] == pRet.p4.pProgram )
              break;
          }
          if ( j == p.nSub )
          {
            Array.Resize( ref p.apSub, p.nSub + 1 );/// sqlite3DbReallocOrFree( v.db, p.apSub, nByte );
            //if( null==p.apSub ){
            //  pRet = null;
            //}else{
            p.apSub[p.nSub++] = pRet.p4.pProgram;
            //}
          }
        }
      }

      return pRet;
    }

    /*
    ** Check if the program stored in the VM associated with pParse may
    ** throw an ABORT exception (causing the statement, but not entire transaction
    ** to be rolled back). This condition is true if the main program or any
    ** sub-programs contains any of the following:
    **
    **   *  OP_Halt with P1=SQLITE_CONSTRAINT and P2=OnConstraintError.OE_Abort.
    **   *  OP_HaltIfNull with P1=SQLITE_CONSTRAINT and P2=OnConstraintError.OE_Abort.
    **   *  OP_Destroy
    **   *  OP_VUpdate
    **   *  OP_VRename
    **   *  OP_FkCounter with P2==0 (immediate foreign key constraint)
    **
    ** Then check that the value of Parse.mayAbort is true if an
    ** ABORT may be thrown, or false otherwise. Return true if it does
    ** match, or false otherwise. This function is intended to be used as
    ** part of an assert statement in the compiler. Similar to:
    **
    **   Debug.Assert( sqlite3VdbeAssertMayAbort(pParse->pVdbe, pParse->mayAbort) );
    */
    static int sqlite3VdbeAssertMayAbort( Vdbe v, int mayAbort )
    {
      int hasAbort = 0;
      Op pOp;
      VdbeOpIter sIter;
      sIter = new VdbeOpIter();// memset( &sIter, 0, sizeof( sIter ) );
      sIter.v = v;

      while ( ( pOp = opIterNext( sIter ) ) != null )
      {
        int opcode = pOp.opcode;
        if ( opcode == OP_Destroy || opcode == OP_VUpdate || opcode == OP_VRename
#if !SQLITE_OMIT_FOREIGN_KEY
																																																 || ( opcode == OP_FkCounter && pOp.p1 == 0 && pOp.p2 == 1 )
#endif
																																																 || ( ( opcode == OP_Halt || opcode == OP_HaltIfNull )
        && ( pOp.p1 == SQLITE_CONSTRAINT && pOp.p2 == OnConstraintError.OE_Abort ) )
        )
        {
          hasAbort = 1;
          break;
        }
      }
      sIter.apSub = null;// sqlite3DbFree( v.db, sIter.apSub );

      /* Return true if hasAbort==mayAbort. Or if a malloc failure occured.
      ** If malloc failed, then the while() loop above may not have iterated
      ** through all opcodes and hasAbort may be set incorrectly. Return
      ** true for this case to prevent the Debug.Assert() in the callers frame
      ** from failing.  */
      return ( hasAbort == mayAbort ) ? 1 : 0;//v.db.mallocFailed !=0|| hasAbort==mayAbort );
    }
#endif
            ///<summary>
            /// Loop through the program looking for P2 values that are negative
            /// on jump instructions.  Each such value is a label.  Resolve the
            /// label by setting the P2 value to its correct non-zero value.
            ///
            /// This routine is called once after all opcodes have been inserted.
            ///
            /// Variable *pMaxFuncArgs is set to the maximum value of any P2 argument
            /// to an OP_Function, OP_AggStep or OP_VFilter opcode. This is used by
            /// sqlite3VdbeMakeReady() to size the Vdbe.apArg[] array.
            ///
            /// The Op.opflags field is set on all opcodes.
            ///</summary>
            ///<summary>
            /// Return the address of the next instruction to be inserted.
            ///
            ///</summary>
            ///<summary>
            /// This function returns a pointer to the array of opcodes associated with
            /// the Vdbe passed as the first argument. It is the callers responsibility
            /// to arrange for the returned array to be eventually freed using the
            /// vdbeFreeOpArray() function.
            ///
            /// Before returning, *pnOp is set to the number of entries in the returned
            /// array. Also, *pnMaxArg is set to the larger of its current value and
            /// the number of entries in the Vdbe.apArg[] array required to execute the
            /// returned program.
            ///
            ///</summary>
            ///<summary>
            /// Add a whole list of operations to the operation stack.  Return the
            /// address of the first operation added.
            ///
            ///</summary>
            ///<summary>
            /// Change the value of the P1 operand for a specific instruction.
            /// This routine is useful when a large program is loaded from a
            /// static array using sqlite3VdbeAddOpList but we want to make a
            /// few minor changes to the program.
            ///
            ///</summary>
            ///<summary>
            /// Change the value of the P2 operand for a specific instruction.
            /// This routine is useful for setting a jump destination.
            ///
            ///</summary>
            ///<summary>
            /// Change the value of the P3 operand for a specific instruction.
            ///
            ///</summary>
            ///<summary>
            /// Change the value of the P5 operand for the most recently
            /// added operation.
            ///
            ///</summary>
            ///<summary>
            /// Change the P2 operand of instruction addr so that it points to
            /// the address of the next instruction to be coded.
            ///
            ///</summary>
            ///<summary>
            /// If the input FuncDef structure is ephemeral, then free it.  If
            /// the FuncDef is not ephermal, then do nothing.
            ///
            ///</summary>
            static void freeEphemeralFunction(sqlite3 db, FuncDef pDef)
            {
                if (Sqlite3.ALWAYS(pDef) && (pDef.flags & FuncFlags.SQLITE_FUNC_EPHEM) != 0)
                {
                    pDef = null;
                    db.sqlite3DbFree(ref pDef);
                }
            }
            //static void vdbeFreeOpArray(sqlite3 *, Op *, int);
            ///<summary>
            /// Delete a P4 value if necessary.
            ///
            ///</summary>
            public static void freeP4(sqlite3 db, P4Usage p4type, object p4)
            {
                if (p4 != null)
                {
                    switch (p4type)
                    {
                        case P4Usage.P4_REAL:
                        case P4Usage.P4_INT64:
                        case P4Usage.P4_DYNAMIC:
                        case P4Usage.P4_KEYINFO:
                        case P4Usage.P4_INTARRAY:
                        case P4Usage.P4_KEYINFO_HANDOFF:
                            {
                                db.sqlite3DbFree(ref p4);
                                break;
                            }
                        case P4Usage.P4_MPRINTF:
                            {
                                if (db.pnBytesFreed == 0)
                                    p4 = null;
                                // malloc_cs.sqlite3_free( ref p4 );
                                break;
                            }
                        case P4Usage.P4_VDBEFUNC:
                            {
                                VdbeFunc pVdbeFunc = (VdbeFunc)p4;
                                freeEphemeralFunction(db, pVdbeFunc.pFunc);
                                if (db.pnBytesFreed == 0)
                                    sqlite3VdbeDeleteAuxData(pVdbeFunc, 0);
                                db.sqlite3DbFree(ref pVdbeFunc);
                                break;
                            }
                        case P4Usage.P4_FUNCDEF:
                            {
                                freeEphemeralFunction(db, (FuncDef)p4);
                                break;
                            }
                        case P4Usage.P4_MEM:
                            {
                                if (db.pnBytesFreed == 0)
                                {
                                    p4 = null;
                                    // sqlite3ValueFree(ref (sqlite3_value)p4);
                                }
                                else
                                {
                                    Community.CsharpSqlite.Mem p = (Mem)p4;
                                    //sqlite3DbFree( db, ref p.zMalloc );
                                    db.sqlite3DbFree(ref p);
                                }
                                break;
                            }
                        case P4Usage.P4_VTAB:
                            {
                                if (db.pnBytesFreed == 0)
                                    vtab.sqlite3VtabUnlock((VTable)p4);
                                break;
                            }
                    }
                }
            }
            ///<summary>
            /// Free the space allocated for aOp and any p4 values allocated for the
            /// opcodes contained within. If aOp is not NULL it is assumed to contain
            /// nOp entries.
            ///
            ///</summary>
            static void vdbeFreeOpArray(sqlite3 db, ref Op[] aOp, int nOp)
            {
                if (aOp != null)
                {
                    //Op pOp;
                    //    for(pOp=aOp; pOp<&aOp[nOp]; pOp++){
                    //      freeP4(db, pOp.p4type, pOp.p4.p);
                    //#if SQLITE_DEBUG
                    //      sqlite3DbFree(db, ref pOp.zComment);
                    //#endif     
                    //    }
                    //  }
                    //  sqlite3DbFree(db, aOp);
                    aOp = null;
                }
            }
            static void vdbeFreeOpArray(sqlite3 db, List<Op> lOp)
            {
                if (lOp != null)
                {
                    lOp.Clear();
                }
            }
            ///<summary>
            /// Link the SubProgram object passed as the second argument into the linked
            /// list at Vdbe.pSubProgram. This list is used to delete all sub-program
            /// objects when the VM is no longer required.
            ///
            ///</summary>
            ///
            ///<summary>
            ///</summary>
            ///<param name="Change N opcodes starting at addr to No">ops.</param>
            ///<param name=""></param>
            public static void sqlite3VdbeChangeToNoop(Vdbe p, int addr, int N)
            {
                if (p.lOp != null)
                {
                    sqlite3 db = p.db;
                    while (N-- > 0)
                    {
                        VdbeOp pOp = p.lOp[addr + N];
                        freeP4(db, pOp.p4type, pOp.p4.p);
                        pOp = p.lOp[addr + N] = new VdbeOp();
                        //memset(pOp, 0, sizeof(pOp[0]));
                        pOp.OpCode= OpCode.OP_Noop;
                        //pOp++;
                    }
                }
            }
            ///<summary>
            /// Change the value of the P4 operand for a specific instruction.
            /// This routine is useful when a large program is loaded from a
            /// static array using sqlite3VdbeAddOpList but we want to make a
            /// few minor changes to the program.
            ///
            /// If n>=0 then the P4 operand is dynamic, meaning that a copy of
            /// the string is made into memory obtained from malloc_cs.sqlite3Malloc().
            /// A value of n==0 means copy bytes of zP4 up to and including the
            /// first null byte.  If n>0 then copy n+1 bytes of zP4.
            ///
            /// If n== P4Usage.P4_KEYINFO it means that zP4 is a pointer to a KeyInfo structure.
            /// A copy is made of the KeyInfo structure into memory obtained from
            /// malloc_cs.sqlite3Malloc, to be freed when the Vdbe is finalized.
            /// n== P4Usage.P4_KEYINFO_HANDOFF indicates that zP4 points to a KeyInfo structure
            /// stored in memory that the caller has obtained from malloc_cs.sqlite3Malloc. The
            /// caller should not free the allocation, it will be freed when the Vdbe is
            /// finalized.
            ///
            /// Other values of n ( P4Usage.P4_STATIC,  P4Usage.P4_COLLSEQ etc.) indicate that zP4 points
            /// to a string or structure that is guaranteed to exist for the lifetime of
            /// the Vdbe. In these cases we can just copy the pointer.
            ///
            /// If addr<0 then change P4 on the most recently inserted instruction.
            ///
            ///</summary>
            //P4_COLLSEQ
            //P4_FUNCDEF
            //P4_INT32
            //P4_KEYINFO
            //CHAR
            //MEM
            //STRING
            //STRING + Type
            //SUBPROGRAM
#if !NDEBUG
																																																    ///<summary>
/// Change the comment on the the most recently coded instruction.  Or
/// insert a No-op and add the comment to that new instruction.  This
/// makes the code easier to read during debugging.  None of this happens
/// in a production build.
///</summary>
    static void sqlite3VdbeComment( Vdbe p, string zFormat, params object[] ap )
    {
      if ( null == p )
        return;
      //      va_list ap;
      lock ( _Custom.lock_va_list )
      {
        Debug.Assert( p.nOp > 0 || p.aOp == null );
        Debug.Assert( p.aOp == null || p.aOp[p.nOp - 1].zComment == null /* || p.db.mallocFailed != 0 */);
        if ( p.nOp != 0 )
        {
          string pz;// = p.aOp[p.nOp-1].zComment;
          _Custom.va_start( ap, zFormat );
          //sqlite3DbFree(db, ref pz);
          pz = sqlite3VMPrintf( p.db, zFormat, ap );
          p.aOp[p.nOp - 1].zComment = pz;
          _Custom.va_end( ref ap );
        }
      }
    }
    static void sqlite3VdbeNoopComment( Vdbe p, string zFormat, params object[] ap )
    {
      if ( null == p )
        return;
      //va_list ap;
      lock ( _Custom.lock_va_list )
      {
        sqlite3VdbeAddOp0( p, OpCode.OP_Noop );
        Debug.Assert( p.nOp > 0 || p.aOp == null );
        Debug.Assert( p.aOp == null || p.aOp[p.nOp - 1].zComment == null /* || p.db.mallocFailed != 0 */);
        if ( p.nOp != 0 )
        {
          string pz; // = p.aOp[p.nOp - 1].zComment;
          _Custom.va_start( ap, zFormat );
          //sqlite3DbFree(db,ref pz);
          pz = sqlite3VMPrintf( p.db, zFormat, ap );
          p.aOp[p.nOp - 1].zComment = pz;
          _Custom.va_end( ref ap );
        }
      }
    }
#else
#endif
            ///<summary>
            /// Return the opcode for a given address.  If the address is -1, then
            /// return the most recently inserted opcode.
            ///
            /// If a memory allocation error has occurred prior to the calling of this
            /// routine, then a pointer to a dummy VdbeOp will be returned.  That opcode
            /// is readable but not writable, though it is cast to a writable value.
            /// The return of a dummy opcode allows the call to continue functioning
            /// after a OOM fault without having to check to see if the return from
            /// this routine is a valid pointer.  But because the dummy.opcode is 0,
            /// dummy will never be written to.  This is verified by code inspection and
            /// by running with Valgrind.
            ///
            /// About the #if SQLITE_OMIT_TRACE:  Normally, this routine is never called
            /// unless p->nOp>0.  This is because in the absense of SQLITE_OMIT_TRACE,
            /// an OP_Trace instruction is always inserted by sqlite3VdbeGet() as soon as
            /// a new VDBE is created.  So we are free to set addr to p->nOp-1 without
            /// having to double-check to make sure that the result is non-negative. But
            /// if SQLITE_OMIT_TRACE is defined, the OP_Trace is omitted and we do need to
            /// check the value of p->nOp-1 before continuing.
            ///</summary>
            const VdbeOp dummy = null;
            ///
            ///<summary>
            ///Ignore the MSVC warning about no initializer 
            ///</summary>
#if !SQLITE_OMIT_EXPLAIN || !NDEBUG || VDBE_PROFILE || SQLITE_DEBUG
            ///<summary>
            /// Compute a string that describes the P4 parameter for an opcode.
            /// Use zTemp for any required temporary buffer space.
            ///</summary>
            static StringBuilder zTemp = new StringBuilder(100);
            public static string displayP4(Op pOp, string notUsedParam, int nTemp)
            {
                zTemp.Length = 0;
                Debug.Assert(nTemp >= 20);
                switch (pOp.p4type)
                {
                    case  P4Usage.P4_KEYINFO_STATIC:
                    case  P4Usage.P4_KEYINFO:
                        {
                            int i, j;
                            KeyInfo pKeyInfo = pOp.p4.pKeyInfo;
                            io.sqlite3_snprintf(nTemp, zTemp, "keyinfo(%d", pKeyInfo.nField);
                            i = StringExtensions.sqlite3Strlen30(zTemp);
                            for (j = 0; j < pKeyInfo.nField; j++)
                            {
                                CollSeq pColl = pKeyInfo.aColl[j];
                                if (pColl != null)
                                {
                                    int n = StringExtensions.sqlite3Strlen30(pColl.zName);
                                    if (i + n > nTemp)
                                    {
                                        zTemp.Append(",...");
                                        // memcpy( &zTemp[i], ",...", 4 );
                                        break;
                                    }
                                    zTemp.Append(",");
                                    // zTemp[i++] = ',';
                                    if (pKeyInfo.aSortOrder != null && pKeyInfo.aSortOrder[j] != 0)
                                    {
                                        zTemp.Append("-");
                                        // zTemp[i++] = '-';
                                    }
                                    zTemp.Append(pColl.zName);
                                    // memcpy( &zTemp[i], pColl.zName, n + 1 );
                                    i += n;
                                }
                                else
                                    if (i + 4 < nTemp)
                                    {
                                        zTemp.Append(",nil");
                                        // memcpy( &zTemp[i], ",nil", 4 );
                                        i += 4;
                                    }
                            }
                            zTemp.Append(")");
                            // zTemp[i++] = ')';
                            //zTemp[i] = 0;
                            Debug.Assert(i < nTemp);
                            break;
                        }
                    case  P4Usage.P4_COLLSEQ:
                        {
                            CollSeq pColl = pOp.p4.pColl;
                            io.sqlite3_snprintf(nTemp, zTemp, "collseq(%.20s)", (pColl != null ? pColl.zName : "null"));
                            break;
                        }
                    case  P4Usage.P4_FUNCDEF:
                        {
                            FuncDef pDef = pOp.p4.pFunc;
                            io.sqlite3_snprintf(nTemp, zTemp, "%s(%d)", pDef.zName, pDef.nArg);
                            break;
                        }
                    case  P4Usage.P4_INT64:
                        {
                            io.sqlite3_snprintf(nTemp, zTemp, "%lld", pOp.p4.pI64);
                            break;
                        }
                    case  P4Usage.P4_INT32:
                        {
                            io.sqlite3_snprintf(nTemp, zTemp, "%d", pOp.p4.i);
                            break;
                        }
                    case  P4Usage.P4_REAL:
                        {
                            io.sqlite3_snprintf(nTemp, zTemp, "%.16g", pOp.p4.pReal);
                            break;
                        }
                    case  P4Usage.P4_MEM:
                        {
                            Mem pMem = pOp.p4.pMem;
                            Debug.Assert((pMem.flags & MemFlags.MEM_Null) == 0);
                            if ((pMem.flags & MemFlags.MEM_Str) != 0)
                            {
                                zTemp.Append(pMem.z);
                            }
                            else
                                if ((pMem.flags & MemFlags.MEM_Int) != 0)
                                {
                                    io.sqlite3_snprintf(nTemp, zTemp, "%lld", pMem.u.i);
                                }
                                else
                                    if ((pMem.flags & MemFlags.MEM_Real) != 0)
                                    {
                                        io.sqlite3_snprintf(nTemp, zTemp, "%.16g", pMem.r);
                                    }
                                    else
                                    {
                                        Debug.Assert((pMem.flags & MemFlags.MEM_Blob) != 0);
                                        zTemp = new StringBuilder("(blob)");
                                    }
                            break;
                        }
#if !SQLITE_OMIT_VIRTUALTABLE
                    case  P4Usage.P4_VTAB:
                        {
                            sqlite3_vtab pVtab = pOp.p4.pVtab.pVtab;
                            io.sqlite3_snprintf(nTemp, zTemp, "vtab:%p:%p", pVtab, pVtab.pModule);
                            break;
                        }
#endif
                    case  P4Usage.P4_INTARRAY:
                        {
                            io.sqlite3_snprintf(nTemp, zTemp, "intarray");
                            break;
                        }
                    case  P4Usage.P4_SUBPROGRAM:
                        {
                            io.sqlite3_snprintf(nTemp, zTemp, "program");
                            break;
                        }
                    default:
                        {
                            if (pOp.p4.z != null)
                                zTemp.Append(pOp.p4.z);
                            //if ( zTemp == null )
                            //{
                            //  zTemp = "";
                            //}
                            break;
                        }
                }
                Debug.Assert(zTemp != null);
                return zTemp.ToString();
            }
#endif
            ///<summary>
            /// Declare to the Vdbe that the BTree object at db->aDb[i] is used.
            ///
            /// The prepared statements need to know in advance the complete set of
            /// attached databases that they will be using.  A mask of these databases
            /// is maintained in p->btreeMask and is used for locking and other purposes.
            ///
            ///</summary>
            public static void sqlite3VdbeUsesBtree(Vdbe p, int i)
            {
                Debug.Assert(i >= 0 && i < p.db.nDb && i < (int)sizeof(yDbMask) * 8);
                Debug.Assert(i < (int)sizeof(yDbMask) * 8);
                p.btreeMask |= ((yDbMask)1) << i;
                if (i != 1 && Sqlite3.sqlite3BtreeSharable(p.db.aDb[i].pBt))
                {
                    p.lockMask |= ((yDbMask)1) << i;
                }
            }
#if !(SQLITE_OMIT_SHARED_CACHE) && SQLITE_THREADSAFE
																																																/*
** If SQLite is compiled to support shared-cache mode and to be threadsafe,
** this routine obtains the mutex Debug.Associated with each BtShared structure
** that may be accessed by the VM pDebug.Assed as an argument. In doing so it also
** sets the BtShared.db member of each of the BtShared structures, ensuring
** that the correct busy-handler callback is invoked if required.
**
** If SQLite is not threadsafe but does support shared-cache mode, then
** sqlite3BtreeEnter() is invoked to set the BtShared.db variables
** of all of BtShared structures accessible via the database handle 
** Debug.Associated with the VM.
**
** If SQLite is not threadsafe and does not support shared-cache mode, this
** function is a no-op.
**
** The p.btreeMask field is a bitmask of all btrees that the prepared 
** statement p will ever use.  Let N be the number of bits in p.btreeMask
** corresponding to btrees that use shared cache.  Then the runtime of
** this routine is N*N.  But as N is rarely more than 1, this should not
** be a problem.
*/
void sqlite3VdbeEnter(Vdbe *p){
  int i;
  yDbMask mask;
  sqlite3 db;
  Db *aDb;
  int nDb;
  if( p.lockMask==0 ) return;  /* The common case */
  db = p.db;
  aDb = db.aDb;
  nDb = db.nDb;
  for(i=0, mask=1; i<nDb; i++, mask += mask){
    if( i!=1 && (mask & p.lockMask)!=0 && Sqlite3.ALWAYS(aDb[i].pBt!=0) ){
      sqlite3BtreeEnter(aDb[i].pBt);
    }
  }
}
#endif
#if !(SQLITE_OMIT_SHARED_CACHE) && SQLITE_THREADSAFE
																																																/*
** Unlock all of the btrees previously locked by a call to sqlite3VdbeEnter().
*/
void sqlite3VdbeLeave(Vdbe *p){
  int i;
  yDbMask mask;
  sqlite3 db;
  Db *aDb;
  int nDb;
  if( p.lockMask==0 ) return;  /* The common case */
  db = p.db;
  aDb = db.aDb;
  nDb = db.nDb;
  for(i=0, mask=1; i<nDb; i++, mask += mask){
    if( i!=1 && (mask & p.lockMask)!=0 && Sqlite3.ALWAYS(aDb[i].pBt!=0) ){
      sqlite3BtreeLeave(aDb[i].pBt);
    }
  }
}
#endif
#if VDBE_PROFILE || SQLITE_DEBUG
																																																    /*
** Print a single opcode.  This routine is used for debugging only.
*/
    static void sqlite3VdbePrintOp( FILE pOut, int pc, Op pOp )
    {
      string zP4;
      string zPtr = null;
      string zFormat1 = "%4d %-13s %4d %4d %4d %-4s %.2X %s\n";
      if ( pOut == null )
        pOut = System.Console.Out;
      zP4 = displayP4( pOp, zPtr, 50 );
      StringBuilder zOut = new StringBuilder( 10 );
      io.sqlite3_snprintf( 999, zOut, zFormat1, pc,
      sqlite3OpcodeName( pOp.opcode ), pOp.p1, pOp.p2, pOp.p3, zP4, pOp.p5,
#if SQLITE_DEBUG
																																																 pOp.zComment != null ? pOp.zComment : ""
#else
																																																""
#endif
																																																 );
      pOut.Write( zOut );
      //fflush(pOut);
    }
#endif
            ///<summary>
            /// Release an array of N Mem elements
            ///</summary>
            public static void releaseMemArray(Mem[] p, int N)
            {
                releaseMemArray(p, 0, N);
            }
            static void releaseMemArray(Mem[] p, int starting, int N)
            {
                if (p != null && p.Length > starting && p[starting] != null && N != 0)
                {
                    Mem pEnd;
                    sqlite3 db = p[starting].db;
                    //u8 malloc_failed =  db.mallocFailed;
                    //if ( db != null ) //&&  db.pnBytesFreed != 0 )
                    //{
                    //  for ( int i = starting; i < N; i++ )//pEnd =  p[N] ; p < pEnd ; p++ )
                    //  {
                    //    sqlite3DbFree( db, ref p[i].zMalloc );
                    //  }
                    //  return;
                    //}
                    for (int i = starting; i < N; i++)//pEnd =  p[N] ; p < pEnd ; p++ )
                    {
                        pEnd = p[i];
                        Debug.Assert(//( p[1] ) == pEnd ||
                        N == 1 || i == p.Length - 1 || p[starting].db == p[starting + 1].db);
                        ///
                        ///<summary>
                        ///This block is really an inlined version of sqlite3VdbeMemRelease()
                        ///that takes advantage of the fact that the memory cell value is
                        ///being set to NULL after releasing any dynamic resources.
                        ///
                        ///The justification for duplicating code is that according to
                        ///callgrind, this causes a certain test case to hit the CPU 4.7
                        ///</summary>
                        ///<param name="percent less (x86 linux, gcc version 4.1.2, ">O6) than if</param>
                        ///<param name="sqlite3MemRelease() were called from here. With ">O2, this jumps</param>
                        ///<param name="to 6.6 percent. The test case is inserting 1000 rows into a table">to 6.6 percent. The test case is inserting 1000 rows into a table</param>
                        ///<param name="with no indexes using a single prepared INSERT statement, bind()">with no indexes using a single prepared INSERT statement, bind()</param>
                        ///<param name="and reset(). Inserts are grouped into a transaction.">and reset(). Inserts are grouped into a transaction.</param>
                        ///<param name=""></param>
                        if (pEnd != null)
                        {
                            if ((pEnd.flags & (MemFlags.MEM_Agg | MemFlags.MEM_Dyn | MemFlags.MEM_Frame | MemFlags.MEM_RowSet)) != 0)
                            {
                                pEnd.sqlite3VdbeMemRelease();
                            }
                            //else if ( pEnd.zMalloc != null )
                            //{
                            //  sqlite3DbFree( db, ref pEnd.zMalloc );
                            //  pEnd.zMalloc = 0;
                            //}
                            pEnd.z = null;
                            pEnd.n = 0;
                            pEnd.flags = MemFlags.MEM_Null;
                            malloc_cs.sqlite3_free(ref pEnd._Mem);
                            malloc_cs.sqlite3_free(ref pEnd.zBLOB);
                        }
                    }
                    //        db.mallocFailed = malloc_failed;
                }
            }
            ///<summary>
            /// Delete a VdbeFrame object and its contents. VdbeFrame objects are
            /// allocated by the OP_Program opcode in sqlite3VdbeExec().
            ///
            ///</summary>
#if !SQLITE_OMIT_EXPLAIN
            ///
            ///<summary>
            ///Give a listing of the program in the virtual machine.
            ///
            ///The interface is the same as sqlite3VdbeExec().  But instead of
            ///running the code, it invokes the callback once for each instruction.
            ///This feature is used to implement "EXPLAIN".
            ///
            ///When p.explain==1, each instruction is listed.  When
            ///p.explain==2, only OP_Explain instructions are listed and these
            ///are shown in a different format.  p.explain==2 is used to implement
            ///EXPLAIN QUERY PLAN.
            ///
            ///</summary>
            ///<param name="When p">>explain==1, first the main program is listed, then each of</param>
            ///<param name="the trigger subprograms are listed one by one.">the trigger subprograms are listed one by one.</param>
            public static SqlResult sqlite3VdbeList(Vdbe p///
                ///<summary>
                ///The VDBE 
                ///</summary>
            )
            {
                int nRow;
                ///
                ///<summary>
                ///Stop when row count reaches this 
                ///</summary>
                int nSub = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Number of sub">vdbes seen so far </param>
                SubProgram[] apSub = null;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Array of sub">vdbes </param>
                Mem pSub = null;
                ///
                ///<summary>
                ///Memory cell hold array of subprogs 
                ///</summary>
                sqlite3 db = p.db;
                ///
                ///<summary>
                ///The database connection 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                var rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///Return code 
                ///</summary>
                if (p.pResultSet == null)
                    p.pResultSet = new Mem[0];
                //Mem* pMem = p.pResultSet = p.aMem[1];   /* First Mem of result set */
                Community.CsharpSqlite.Mem pMem;
                Debug.Assert(p.explain != 0);
                Debug.Assert(p.magic == VdbeMagic.VDBE_MAGIC_RUN);
                Debug.Assert(p.rc == SqlResult.SQLITE_OK || p.rc == SqlResult.SQLITE_BUSY || p.rc == SqlResult.SQLITE_NOMEM);
                ///
                ///<summary>
                ///Even though this opcode does not use dynamic strings for
                ///the result, result columns may become dynamic if the user calls
                ///</summary>
                ///<param name="sqlite3_column_text16(), causing a translation to UTF">16 encoding.</param>
                ///<param name=""></param>
                releaseMemArray(p.pResultSet, 8);
                //if ( p.rc == SQLITE_NOMEM )
                //{
                //  /* This happens if a malloc() inside a call to sqlite3_column_text() or
                //  ** sqlite3_column_text16() failed.  */
                //  db.mallocFailed = 1;
                //  return SqlResult.SQLITE_ERROR;
                //}
                ///
                ///<summary>
                ///When the number of output rows reaches nRow, that means the
                ///listing has finished and sqlite3_step() should return SQLITE_DONE.
                ///nRow is the sum of the number of rows in the main program, plus
                ///the sum of the number of rows in all trigger subprograms encountered
                ///so far.  The nRow value will increase as new trigger subprograms are
                ///</summary>
                ///<param name="encountered, but p">>pc will eventually catch up to nRow.</param>
                ///<param name=""></param>
                nRow = p.nOp;
                int i_pMem;
                if (p.explain == 1)
                {
                    ///
                    ///<summary>
                    ///The first 8 memory cells are used for the result set.  So we will
                    ///commandeer the 9th cell to use as storage for an array of pointers
                    ///to trigger subprograms.  The VDBE is guaranteed to have at least 9
                    ///cells.  
                    ///</summary>
                    Debug.Assert(p.nMem > 9);
                    pSub = p.aMem[9];
                    if ((pSub.flags & MemFlags.MEM_Blob) != 0)
                    {
                        ///
                        ///<summary>
                        ///On the first call to sqlite3_step(), pSub will hold a NULL.  It is
                        ///initialized to a BLOB by the  P4Usage.P4_SUBPROGRAM processing logic below 
                        ///</summary>
                        apSub = p.aMem[9]._SubProgram;
                        //    apSub = (SubProgram*)pSub->z;
                        nSub = apSub.Length;
                        // pSub->n / sizeof( Vdbe* );
                    }
                    for (i = 0; i < nSub; i++)
                    {
                        nRow += apSub[i].nOp;
                    }
                }
                i_pMem = 0;
                if (i_pMem >= p.pResultSet.Length)
                    Array.Resize(ref p.pResultSet, 8 + p.pResultSet.Length);
                {
                    p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                }
                pMem = p.pResultSet[i_pMem++];
                do
                {
                    i = p.currentOpCodeIndex++;
                }
                while (i < nRow && p.explain == 2 && p.lOp[i].OpCode != OpCode.OP_Explain);
                if (i >= nRow)
                {
                    p.rc = SqlResult.SQLITE_OK;
                    rc = SqlResult.SQLITE_DONE;
                }
                else
                    if (db.u1.isInterrupted)
                    {
                        p.rc = SqlResult.SQLITE_INTERRUPT;
                        rc = SqlResult.SQLITE_ERROR;
                        malloc_cs.sqlite3SetString(ref p.zErrMsg, db, Sqlite3.sqlite3ErrStr(p.rc));
                    }
                    else
                    {
                        string z;
                        Op pOp;
                        if (i < p.nOp)
                        {
                            ///
                            ///<summary>
                            ///The output line number is small enough that we are still in the
                            ///main program. 
                            ///</summary>
                            pOp = p.lOp[i];
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///We are currently listing subprograms.  Figure out which one and
                            ///pick up the appropriate opcode. 
                            ///</summary>
                            int j;
                            i -= p.nOp;
                            for (j = 0; i >= apSub[j].nOp; j++)
                            {
                                i -= apSub[j].nOp;
                            }
                            pOp = apSub[j].aOp[i];
                        }
                        if (p.explain == 1)
                        {
                            pMem.flags = MemFlags.MEM_Int;
                            pMem.ValType = FoundationalType.SQLITE_INTEGER;
                            pMem.u.i = i;
                            ///
                            ///<summary>
                            ///Program counter 
                            ///</summary>
                            if (p.pResultSet[i_pMem] == null)
                            {
                                p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                            }
                            pMem = p.pResultSet[i_pMem++];
                            //pMem++;
                            ///
                            ///<summary>
                            ///When an OP_Program opcode is encounter (the only opcode that has
                            ///a  P4Usage.P4_SUBPROGRAM argument), expand the size of the array of subprograms
                            ///</summary>
                            ///<param name="kept in p"> assuming this subprogram</param>
                            ///<param name="has not already been seen.">has not already been seen.</param>
                            ///<param name=""></param>
                            pMem.flags = MemFlags.MEM_Static | MemFlags.MEM_Str | MemFlags.MEM_Term;
                            pMem.z = Sqlite3.sqlite3OpcodeName(pOp.OpCode);
                            ///
                            ///<summary>
                            ///Opcode 
                            ///</summary>
                            Debug.Assert(pMem.z != null);
                            pMem.n = StringExtensions.sqlite3Strlen30(pMem.z);
                            pMem.ValType = FoundationalType.SQLITE_TEXT;
                            pMem.enc = SqliteEncoding.UTF8;
                            if (p.pResultSet[i_pMem] == null)
                            {
                                p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                            }
                            pMem = p.pResultSet[i_pMem++];
                            //pMem++;
                            if (pOp.p4type ==  P4Usage.P4_SUBPROGRAM)
                            {
                                //Debugger.Break(); // TODO
                                //int nByte = 0;//(nSub+1)*sizeof(SubProgram);
                                int j;
                                for (j = 0; j < nSub; j++)
                                {
                                    if (apSub[j] == pOp.p4.pProgram)
                                        break;
                                }
                                if (j == nSub)
                                {
                                    // && SqlResult.SQLITE_OK==sqlite3VdbeMemGrow(pSub, nByte, 1) ){
                                    Array.Resize(ref apSub, nSub + 1);
                                    pSub._SubProgram = apSub;
                                    // (SubProgram)pSub.z;
                                    apSub[nSub++] = pOp.p4.pProgram;
                                    pSub.flags |= MemFlags.MEM_Blob;
                                    pSub.n = 0;
                                    //nSub*sizeof(SubProgram);
                                }
                            }
                        }
                        pMem.flags = MemFlags.MEM_Int;
                        pMem.u.i = pOp.p1;
                        ///
                        ///<summary>
                        ///P1 
                        ///</summary>
                        pMem.ValType = FoundationalType.SQLITE_INTEGER;
                        if (p.pResultSet[i_pMem] == null)
                        {
                            p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                        }
                        //--------------------------
                        pMem = p.pResultSet[i_pMem++];
                        //pMem++;
                        pMem.flags = MemFlags.MEM_Int;
                        pMem.u.i = pOp.p2;
                        ///
                        ///<summary>
                        ///P2 
                        ///</summary>
                        pMem.ValType = FoundationalType.SQLITE_INTEGER;
                        if (p.pResultSet[i_pMem] == null)
                        {
                            p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                        }
                        //----------------------------
                        pMem = p.pResultSet[i_pMem++];
                        //pMem++;
                        pMem.flags = MemFlags.MEM_Int;
                        pMem.u.i = pOp.p3;
                        ///
                        ///<summary>
                        ///P3 
                        ///</summary>
                        pMem.ValType = FoundationalType.SQLITE_INTEGER;
                        if (p.pResultSet[i_pMem] == null)
                        {
                            p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                        }
                        //-----------------------
                        pMem = p.pResultSet[i_pMem++];
                        //pMem++;
                        //if ( sqlite3VdbeMemGrow( pMem, 32, 0 ) != 0 )
                        //{                                                     /* P4 */
                        //  Debug.Assert( p.db.mallocFailed != 0 );
                        //  return SqlResult.SQLITE_ERROR;
                        //}
                        pMem.Flags = MemFlags.MEM_Dyn | MemFlags.MEM_Str | MemFlags.MEM_Term;
                        z = displayP4(pOp, pMem.z, 32);
                        if (z != pMem.z)
                        {
                            pMem.sqlite3VdbeMemSetStr(z, -1, SqliteEncoding.UTF8, null);
                        }
                        else
                        {
                            Debug.Assert(pMem.z != null);
                            pMem.n = StringExtensions.sqlite3Strlen30(pMem.z);
                            pMem.enc = SqliteEncoding.UTF8;
                        }
                        pMem.type = FoundationalType.SQLITE_TEXT;
                        if (p.pResultSet[i_pMem] == null)
                        {
                            p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                        }
                        //--------------------------------
                        pMem = p.pResultSet[i_pMem++];
                        //pMem++;
                        if (p.explain == 1)
                        {
                            //if ( sqlite3VdbeMemGrow( pMem, 4, 0 ) != 0 )
                            //{
                            //  Debug.Assert( p.db.mallocFailed != 0 );
                            //  return SqlResult.SQLITE_ERROR;
                            //}
                            pMem.flags = MemFlags.MEM_Dyn | MemFlags.MEM_Str | MemFlags.MEM_Term;
                            pMem.n = 2;
                            pMem.z = pOp.p5.ToString("x2");
                            //sqlite3_snprintf( 3, pMem.z, "%.2x", pOp.p5 );   /* P5 */
                            pMem.type = FoundationalType.SQLITE_TEXT;
                            pMem.enc = SqliteEncoding.UTF8;
                            if (p.pResultSet[i_pMem] == null)
                            {
                                p.pResultSet[i_pMem] = malloc_cs.sqlite3Malloc(p.pResultSet[i_pMem]);
                            }
                            //----------------------------------
                            pMem = p.pResultSet[i_pMem++];
                            // pMem++;
#if SQLITE_DEBUG
																																																																																																																																																          if ( pOp.zComment != null )
          {
            pMem.flags = MEM.MEM_Str | MEM.MEM_Term;
            pMem.z = pOp.zComment;
            pMem.n = pMem.z == null ? 0 : StringExtensions.sqlite3Strlen30( pMem.z );
            pMem.enc = SqliteEncoding.UTF8;
            pMem.type = SQLITE_TEXT;
          }
          else
#endif
                            {
                                pMem.flags = MemFlags.MEM_Null;
                                ///
                                ///<summary>
                                ///Comment 
                                ///</summary>
                                pMem.type = FoundationalType.SQLITE_NULL;
                            }
                        }
                        p.nResColumn = (u16)(8 - 4 * (p.explain - 1));
                        p.rc = SqlResult.SQLITE_OK;
                        rc = SqlResult.SQLITE_ROW;
                    }
                return rc;
            }
#endif
#if SQLITE_DEBUG
																																																    /*
** Print the SQL that was used to generate a VDBE program.
*/
    static void sqlite3VdbePrintSql( Vdbe p )
    {
      int nOp = p.nOp;
      VdbeOp pOp;
      if ( nOp < 1 )
        return;
      pOp = p.aOp[0];
      if ( pOp.opcode == OP_Trace && pOp.p4.z != null )
      {
        string z = pOp.p4.z;
        z = z.Trim();// while ( CharExtensions.sqlite3Isspace( *(u8)z ) ) z++;
        Console.Write( "SQL: [%s]\n", z );
      }
    }
#endif
#if !SQLITE_OMIT_TRACE && SQLITE_ENABLE_IOTRACE
																																																/*
** Print an sqliteinth.IOTRACE message showing SQL content.
*/
static void sqlite3VdbeIOTraceSql( Vdbe p )
{
int nOp = p.nOp;
VdbeOp pOp;
if ( SQLite3IoTrace == false ) return;
if ( nOp < 1 ) return;
pOp = p.aOp[0];
if ( pOp.opcode == OP_Trace && pOp.p4.z != null )
{
int i, j;
string z = "";//char z[1000];
io.sqlite3_snprintf( 1000, z, "%s", pOp.p4.z );
//for(i=0; CharExtensions.sqlite3Isspace(z[i]); i++){}
//for(j=0; z[i]; i++){
//if( CharExtensions.sqlite3Isspace(z[i]) ){
//if( z[i-1]!=' ' ){
//z[j++] = ' ';
//}
//}else{
//z[j++] = z[i];
//}
//}
//z[j] = 0;
//z = z.Trim( z );
sqlite3IoTrace( "SQL %s\n", z.Trim() );
}
}
#endif
            ///<summary>
            /// Allocate space from a fixed size buffer and return a pointer to
            /// that space.  If insufficient space is available, return NULL.
            ///
            /// The pBuf parameter is the initial value of a pointer which will
            /// receive the new memory.  pBuf is normally NULL.  If pBuf is not
            /// NULL, it means that memory space has already been allocated and that
            /// this routine should not allocate any new memory.  When pBuf is not
            /// NULL simply return pBuf.  Only allocate new memory space when pBuf
            /// is NULL.
            ///
            /// nByte is the number of bytes of space needed.
            ///
            /// *ppFrom points to available space and pEnd points to the end of the
            /// available space.  When space is allocated, *ppFrom is advanced past
            /// the end of the allocated space.
            ///
            /// *pnByte is a counter of the number of bytes of space that have failed
            /// to allocate.  If there is insufficient space in *ppFrom to satisfy the
            /// request, then increment *pnByte by the amount of the request.
            ///</summary>
            //static void* allocSpace(
            //  void* pBuf,          /* Where return pointer will be stored */
            //  int nByte,           /* Number of bytes to allocate */
            //  u8** ppFrom,         /* IN/OUT: Allocate from *ppFrom */
            //  u8* pEnd,            /* Pointer to 1 byte past the end of *ppFrom buffer */
            //  int* pnByte          /* If allocation cannot be made, increment *pnByte */
            //)
            //{
            //  Debug.Assert(EIGHT_BYTE_ALIGNMENT(*ppFrom));
            //  if (pBuf) return pBuf;
            //  nByte = ROUND8(nByte);
            //  if (&(*ppFrom)[nByte] <= pEnd)
            //  {
            //    pBuf = (void)*ppFrom;
            //    *ppFrom += nByte;
            //  }
            //  else
            //  {
            //    *pnByte += nByte;
            //  }
            //  return pBuf;
            //}
            ///<summary>
            /// Rewind the VDBE back to the beginning in preparation for
            /// running it.
            ///
            ///</summary>
            ///<summary>
            /// Prepare a virtual machine for execution for the first time after
            /// creating the virtual machine.  This involves things such
            /// as allocating stack space and initializing the program counter.
            /// After the VDBE has be prepped, it can be executed by one or more
            /// calls to sqlite3VdbeExec().
            ///
            /// This function may be called exact once on a each virtual machine.
            /// After this routine is called the VM has been "packaged" and is ready
            /// to run.  After this routine is called, futher calls to
            /// sqlite3VdbeAddOp() functions are prohibited.  This routine disconnects
            /// the Vdbe from the Parse object that helped generate it so that the
            /// the Vdbe becomes an independent entity and the Parse object can be
            /// destroyed.
            ///
            /// Use the sqlite3VdbeRewind() procedure to restore a virtual machine back
            /// to its initial state after it has been run.
            ///
            ///</summary>
            public static void sqlite3VdbeMakeReady(Vdbe p,///
                ///<summary>
                ///The VDBE 
                ///</summary>
            Parse pParse///
                ///<summary>
                ///Parsing context 
                ///</summary>
            )
            {
                sqlite3 db;
                ///
                ///<summary>
                ///The database connection 
                ///</summary>
                int nVar;
                ///
                ///<summary>
                ///Number of parameters 
                ///</summary>
                int nMem;
                ///
                ///<summary>
                ///Number of VM memory registers 
                ///</summary>
                int nCursor;
                ///
                ///<summary>
                ///Number of cursors required 
                ///</summary>
                int nArg;
                ///
                ///<summary>
                ///Number of arguments in subprograms 
                ///</summary>
                int n;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                //u8 zCsr;                     /* Memory available for allocation */
                //u8 zEnd;                     /* First byte past allocated memory */
                int nByte;
                ///
                ///<summary>
                ///How much extra memory is needed 
                ///</summary>
                Debug.Assert(p != null);
                Debug.Assert(pParse != null);
                Debug.Assert(p.magic == VdbeMagic.VDBE_MAGIC_INIT);
                db = p.db;
                //Debug.Assert( db.mallocFailed == 0 );
                nVar = pParse.nVar;
                nMem = pParse.nMem;
                nCursor = pParse.nTab;
                nArg = pParse.nMaxArg;
                ///
                ///<summary>
                ///For each cursor required, also allocate a memory cell. Memory
                ///</summary>
                ///<param name="cells (nMem+1">nCursor)..nMem, inclusive, will never be used by</param>
                ///<param name="the vdbe program. Instead they are used to allocate space for">the vdbe program. Instead they are used to allocate space for</param>
                ///<param name="VdbeCursor/BtCursor structures. The blob of memory associated with">VdbeCursor/BtCursor structures. The blob of memory associated with</param>
                ///<param name="cursor 0 is stored in memory cell nMem. Memory cell (nMem">1)</param>
                ///<param name="stores the blob of memory associated with cursor 1, etc.">stores the blob of memory associated with cursor 1, etc.</param>
                ///<param name=""></param>
                ///<param name="See also: allocateCursor().">See also: allocateCursor().</param>
                ///<param name=""></param>
                nMem += nCursor;
                ///
                ///<summary>
                ///Allocate space for memory registers, SQL variables, VDBE cursors and 
                ///an array to marshal SQL function arguments in.
                ///
                ///</summary>
                //zCsr = (u8)&p->aOp[p->nOp];       /* Memory avaliable for allocation */
                //zEnd = (u8)&p->aOp[p->nOpAlloc];  /* First byte past end of zCsr[] */
                p.resolveP2Values(ref nArg);
                p.usesStmtJournal = (pParse.isMultiWrite != 0 && pParse.mayAbort != 0);
                if (pParse.explain != 0 && nMem < 10)
                {
                    nMem = 10;
                }
                //memset(zCsr, 0, zEnd-zCsr);
                //zCsr += ( zCsr - (u8)0 ) & 7;
                //Debug.Assert( EIGHT_BYTE_ALIGNMENT( zCsr ) );
                p.expired = false;
                //
                // C# -- Replace allocation with individual Dims
                //
                ///
                ///<summary>
                ///Memory for registers, parameters, cursor, etc, is allocated in two
                ///passes.  On the first pass, we try to reuse unused space at the 
                ///end of the opcode array.  If we are unable to satisfy all memory
                ///requirements by reusing the opcode array tail, then the second
                ///pass will fill in the rest using a fresh allocation.  
                ///
                ///</summary>
                ///<param name="This two">pass approach that reuses as much memory as possible from</param>
                ///<param name="the leftover space at the end of the opcode array can significantly">the leftover space at the end of the opcode array can significantly</param>
                ///<param name="reduce the amount of memory held by a prepared statement.">reduce the amount of memory held by a prepared statement.</param>
                ///<param name=""></param>
                //do
                //{
                //  nByte = 0;
                //  p->aMem = allocSpace( p->aMem, nMem * sizeof( Mem ), &zCsr, zEnd, &nByte );
                //  p->aVar = allocSpace( p->aVar, nVar * sizeof( Mem ), &zCsr, zEnd, &nByte );
                //  p->apArg = allocSpace( p->apArg, nArg * sizeof( Mem* ), &zCsr, zEnd, &nByte );
                //  p->azVar = allocSpace( p->azVar, nVar * sizeof( char* ), &zCsr, zEnd, &nByte );
                //  p->apCsr = allocSpace( p->apCsr, nCursor * sizeof( VdbeCursor* ),
                //                        &zCsr, zEnd, &nByte );
                //  if ( nByte )
                //  {
                //    p->pFree = sqlite3DbMallocZero( db, nByte );
                //  }
                //  zCsr = p->pFree;
                //  zEnd = zCsr[nByte];
                //} while ( nByte && !db->mallocFailed );
                //p->nCursor = (u16)nCursor;
                //if( p->aVar ){
                //  p->nVar = (ynVar)nVar;
                //  for(n=0; n<nVar; n++){
                //    p->aVar[n].flags = MEM.MEM_Null;
                //    p->aVar[n].db = db;
                //  }
                //}
                //if( p->azVar ){
                //  p->nzVar = pParse->nzVar;
                //  memcpy(p->azVar, pParse->azVar, p->nzVar*sizeof(p->azVar[0]));
                //  memset(pParse->azVar, 0, pParse->nzVar*sizeof(pParse->azVar[0]));
                //}
                p.nzVar = (i16)pParse.nzVar;
                p.azVar = new string[p.nzVar == 0 ? 1 : p.nzVar];
                //p.azVar = (char*)p.apArg[nArg];
                for (n = 0; n < p.nzVar; n++)
                {
                    p.azVar[n] = pParse.azVar[n];
                }
                //
                // C# -- Replace allocation with individual Dims
                // aMem is 1 based, so allocate 1 extra cell under C#
                p.aMem = new Mem[nMem + 1];
                for (n = 0; n <= nMem; n++)
                {
                    p.aMem[n] = malloc_cs.sqlite3Malloc(p.aMem[n]);
                    p.aMem[n].db = db;
                }
                //p.aMem--;         /* aMem[] goes from 1..nMem */
                p.nMem = nMem;
                ///
                ///<summary>
                ///</summary>
                ///<param name="not from 0..nMem">1 </param>
                //
                p.aVar = new Mem[nVar == 0 ? 1 : nVar];
                for (n = 0; n < nVar; n++)
                {
                    p.aVar[n] = malloc_cs.sqlite3Malloc(p.aVar[n]);
                }
                p.nVar = (ynVar)nVar;
                //
                p.apArg = new Mem[nArg == 0 ? 1 : nArg];
                //p.apArg = (Mem*)p.aVar[nVar];
                //
                p.OpenCursors = new VdbeCursor[nCursor == 0 ? 1 : nCursor];
                //p.apCsr = (VdbeCursor*)p.azVar[nVar];
                p.OpenCursors[0] = new VdbeCursor();
                p.nCursor = (u16)nCursor;
                if (p.aVar != null)
                {
                    p.nVar = (ynVar)nVar;
                    //
                    for (n = 0; n < nVar; n++)
                    {
                        p.aVar[n].flags = MemFlags.MEM_Null;
                        p.aVar[n].db = db;
                    }
                }
                if (p.aMem != null)
                {
                    //p.aMem--;                    /* aMem[] goes from 1..nMem */
                    p.nMem = nMem;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="not from 0..nMem">1 </param>
                    for (n = 0; n <= nMem; n++)
                    {
                        p.aMem[n].flags = MemFlags.MEM_Null;
                        p.aMem[n].n = 0;
                        p.aMem[n].z = null;
                        p.aMem[n].zBLOB = null;
                        p.aMem[n].db = db;
                    }
                }
                p.explain = pParse.explain;
                p.sqlite3VdbeRewind();
            }
            ///<summary>
            /// Close a VDBE cursor and release all the resources that cursor
            /// happens to hold.
            ///
            ///</summary>
            public static void sqlite3VdbeFreeCursor(Vdbe p, VdbeCursor pCx)
            {
                if (pCx == null)
                {
                    return;
                }
                if (pCx.pBt != null)
                {
                    BTreeMethods.sqlite3BtreeClose(ref pCx.pBt);
                    ///The pCx.pCursor will be close automatically, if it exists, by
                    ///the call above. 
                }
                else
                    if (pCx.pCursor != null)
                    {
                        pCx.pCursor.sqlite3BtreeCloseCursor();
                    }
#if !SQLITE_OMIT_VIRTUALTABLE
                if (pCx.pVtabCursor != null)
                {
                    sqlite3_vtab_cursor pVtabCursor = pCx.pVtabCursor;
                    sqlite3_module pModule = pCx.pModule;
                    p.inVtabMethod = 1;
                    pModule.xClose(ref pVtabCursor);
                    p.inVtabMethod = 0;
                }
#endif
            }
            ///<summary>
            /// Copy the values stored in the VdbeFrame structure to its Vdbe. This
            /// is used, for example, when a trigger sub-program is halted to restore
            /// control to the main program.
            ///
            ///</summary>
            ///<summary>
            /// Close all cursors.
            ///
            /// Also release any dynamic memory held by the VM in the Vdbe.aMem memory
            /// cell array. This is necessary as the memory cell array may contain
            /// pointers to VdbeFrame objects, which may in turn contain pointers to
            /// open cursors.
            ///
            ///</summary>
            public static void closeAllCursors(Vdbe p)
            {
                if (p.pFrame != null)
                {
                    VdbeFrame pFrame;
                    for (pFrame = p.pFrame; pFrame.pParent != null; pFrame = pFrame.pParent)
                        ;
                    pFrame.sqlite3VdbeFrameRestore();
                }
                p.pFrame = null;
                p.nFrame = 0;
                if (p.OpenCursors != null)
                {
                    int i;
                    for (i = 0; i < p.nCursor; i++)
                    {
                        VdbeCursor pC = p.OpenCursors[i];
                        if (pC != null)
                        {
                            sqlite3VdbeFreeCursor(p, pC);
                            p.OpenCursors[i] = null;
                        }
                    }
                }
                if (p.aMem != null)
                {
                    releaseMemArray(p.aMem, 1, p.nMem);
                }
                while (p.pDelFrame != null)
                {
                    VdbeFrame pDel = p.pDelFrame;
                    p.pDelFrame = pDel.pParent;
                    pDel.sqlite3VdbeFrameDelete();
                }
            }
            ///<summary>
            /// Clean up the VM after execution.
            ///
            /// This routine will automatically close any cursors, lists, and/or
            /// sorters that were left open.  It also deletes the values of
            /// variables in the aVar[] array.
            ///
            ///</summary>
            public static void Cleanup(Vdbe p)
            {
                sqlite3 db = p.db;
#if SQLITE_DEBUG
																																																																								      /* Execute Debug.Assert() statements to ensure that the Vdbe.apCsr[] and 
** Vdbe.aMem[] arrays have already been cleaned up.  */
      int i;
      //TODO for(i=0; i<p.nCursor; i++) Debug.Assert( p.apCsr==null || p.apCsr[i]==null );
      for ( i = 1; i <= p.nMem; i++ )
        Debug.Assert( p.aMem != null || p.aMem[i].flags == MEM.MEM_Null );
#endif
                db.sqlite3DbFree(ref p.zErrMsg);
                p.pResultSet = null;
            }
            ///<summary>
            /// Set the number of result columns that will be returned by this SQL
            /// statement. This is now set at compile time, rather than during
            /// execution of the vdbe program so that vdbeapi.sqlite3_column_count() can
            /// be called on an SQL statement before sqlite3_step().
            ///
            ///</summary>
            ///<summary>
            /// Set the name of the idx'th column to be returned by the SQL statement.
            /// zName must be a pointer to a nul terminated string.
            ///
            /// This call must be made after a call to sqlite3VdbeSetNumCols().
            ///
            /// The final parameter, xDel, must be one of SQLITE_DYNAMIC, SQLITE_STATIC
            /// or SQLITE_TRANSIENT. If it is SQLITE_DYNAMIC, then the buffer pointed
            /// to by zName will be freed by sqlite3DbFree() when the vdbe is destroyed.
            ///
            ///</summary>
            ///
            ///<summary>
            ///A read or write transaction may or may not be active on database handle
            ///db. If a transaction is active, commit it. If there is a
            ///</summary>
            ///<param name="write">transaction spanning more than one database file, this routine</param>
            ///<param name="takes care of the master journal trickery.">takes care of the master journal trickery.</param>
            ///<param name=""></param>
            public static SqlResult vdbeCommit(sqlite3 db, Vdbe p)
            {
                int i;
                int nTrans = 0;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Number of databases with an active write">transaction </param>
                SqlResult rc = SqlResult.SQLITE_OK;
                bool needXcommit = false;
#if SQLITE_OMIT_VIRTUALTABLE
																																																																								      /* With this option, sqlite3VtabSync() is defined to be simply
** SqlResult.SQLITE_OK so p is not used.
*/
      sqliteinth.UNUSED_PARAMETER( p );
#endif
                ///
                ///<summary>
                ///Before doing anything else, call the xSync() callback for any
                ///virtual module tables written in this transaction. This has to
                ///be done before determining whether a master journal file is
                ///required, as an xSync() callback may add an attached database
                ///to the transaction.
                ///</summary>
                rc = vtab.sqlite3VtabSync(db, ref p.zErrMsg);
                ///
                ///<summary>
                ///This loop determines (a) if the commit hook should be invoked and
                ///(b) how many database files have open write transactions, not
                ///including the temp database. (b) is important because if more than
                ///one database file has an open write transaction, a master journal
                ///file is required for an atomic commit.
                ///
                ///</summary>
                for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nDb; i++)
                {
                    Btree pBt = db.aDb[i].pBt;
                    if (pBt.sqlite3BtreeIsInTrans())
                    {
                        needXcommit = true;
                        if (i != 1)
                            nTrans++;
                        rc = pBt.sqlite3BtreePager().sqlite3PagerExclusiveLock();
                    }
                }
                if (rc != SqlResult.SQLITE_OK)
                {
                    return rc;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="If there are any write">transactions at all, invoke the commit hook </param>
                if (needXcommit && db.xCommitCallback != null)
                {
                    rc = db.xCommitCallback(db.pCommitArg);
                    if (rc != 0)
                    {
                        return SqlResult.SQLITE_CONSTRAINT;
                    }
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="The simple case "> no more than one database file (not counting the</param>
                ///<param name="TEMP database) has a transaction active.   There is no need for the">TEMP database) has a transaction active.   There is no need for the</param>
                ///<param name="master">journal.</param>
                ///<param name=""></param>
                ///<param name="If the return value of sqlite3BtreeGetFilename() is a zero length">If the return value of sqlite3BtreeGetFilename() is a zero length</param>
                ///<param name="string, it means the main database is :memory: or a temp file.  In">string, it means the main database is :memory: or a temp file.  In</param>
                ///<param name="that case we do not support atomic multi">file commits, so use the</param>
                ///<param name="simple case then too.">simple case then too.</param>
                ///<param name=""></param>
                if (0 == StringExtensions.sqlite3Strlen30(db.aDb[0].pBt.GetFilename()) || nTrans <= 1)
                {
                    for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nDb; i++)
                    {
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt != null)
                        {
                            rc = pBt.sqlite3BtreeCommitPhaseOne(null);
                        }
                    }
                    ///
                    ///<summary>
                    ///Do the commit only if all databases successfully complete phase 1.
                    ///If one of the BtreeCommitPhaseOne() calls fails, this indicates an
                    ///IO error while deleting or truncating a journal file. It is unlikely,
                    ///but could happen. In this case abandon processing and return the error.
                    ///
                    ///</summary>
                    for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nDb; i++)
                    {
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt != null)
                        {
                            rc = pBt.sqlite3BtreeCommitPhaseTwo(0);
                        }
                    }
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        vtab.sqlite3VtabCommit(db);
                    }
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="The complex case ">transaction active.</param>
                ///<param name="This requires a master journal file to ensure the transaction is">This requires a master journal file to ensure the transaction is</param>
                ///<param name="committed atomicly.">committed atomicly.</param>
                ///<param name=""></param>
#if !SQLITE_OMIT_DISKIO
                else
                {
                    sqlite3_vfs pVfs = db.pVfs;
                    bool needSync = false;
                    string zMaster = "";
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="File">name for the master journal </param>
                    string zMainFile = db.aDb[0].pBt.GetFilename();
                    sqlite3_file pMaster = null;
                    i64 offset = 0;
                    int res = 0;
                    ///
                    ///<summary>
                    ///Select a master journal file name 
                    ///</summary>
                    do
                    {
                        i64 iRandom = 0;
                        db.sqlite3DbFree(ref zMaster);
                        Sqlite3.sqlite3_randomness(sizeof(u32), ref iRandom);
                        //random.Length
                        zMaster = io.sqlite3MPrintf(db, "%s-mj%08X", zMainFile, iRandom & 0x7fffffff);
                        //if (!zMaster)
                        //{
                        //  return SQLITE_NOMEM;
                        //}
                        sqliteinth.sqlite3FileSuffix3(zMainFile, zMaster);
                        rc = os.sqlite3OsAccess(pVfs, zMaster, Sqlite3.SQLITE_ACCESS_EXISTS, ref res);
                    }
                    while (rc == SqlResult.SQLITE_OK && res == 1);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        ///
                        ///<summary>
                        ///Open the master journal. 
                        ///</summary>
                        int flags = Sqlite3.SQLITE_OPEN_READWRITE | Sqlite3.SQLITE_OPEN_CREATE | Sqlite3.SQLITE_OPEN_EXCLUSIVE | Sqlite3.SQLITE_OPEN_MASTER_JOURNAL;
                        rc = os.sqlite3OsOpenMalloc(ref pVfs, zMaster, ref pMaster, flags, ref flags);
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        db.sqlite3DbFree(ref zMaster);
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///Write the name of each database file in the transaction into the new
                    ///master journal file. If an error occurs at this point close
                    ///and delete the master journal file. All the individual journal files
                    ///still have 'null' as the master journal pointer, so they will roll
                    ///back independently if a failure occurs.
                    ///
                    ///</summary>
                    for (i = 0; i < db.nDb; i++)
                    {
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt.sqlite3BtreeIsInTrans())
                        {
                            string zFile = pBt.GetJournalname();
                            if (zFile == null)
                            {
                                continue;
                                ///
                                ///<summary>
                                ///Ignore TEMP and :memory: databases 
                                ///</summary>
                            }
                            Debug.Assert(zFile != "");
                            if (!needSync && 0 == pBt.sqlite3BtreeSyncDisabled())
                            {
                                needSync = true;
                            }
                            rc = os.sqlite3OsWrite(pMaster, Encoding.UTF8.GetBytes(zFile), StringExtensions.sqlite3Strlen30(zFile), offset);
                            offset += StringExtensions.sqlite3Strlen30(zFile);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                os.sqlite3OsCloseFree(pMaster);
                                os.sqlite3OsDelete(pVfs, zMaster, 0);
                                db.sqlite3DbFree(ref zMaster);
                                return rc;
                            }
                        }
                    }
                    ///
                    ///<summary>
                    ///Sync the master journal file. If the IOCAP_SEQUENTIAL device
                    ///flag is set this is not required.
                    ///
                    ///</summary>
                    if (needSync && 0 == (os.sqlite3OsDeviceCharacteristics(pMaster) & Sqlite3.SQLITE_IOCAP_SEQUENTIAL) && SqlResult.SQLITE_OK != (rc = os.sqlite3OsSync(pMaster, Sqlite3.SQLITE_SYNC_NORMAL)))
                    {
                        os.sqlite3OsCloseFree(pMaster);
                        os.sqlite3OsDelete(pVfs, zMaster, 0);
                        db.sqlite3DbFree(ref zMaster);
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///Sync all the db files involved in the transaction. The same call
                    ///sets the master journal pointer in each individual journal. If
                    ///an error occurs here, do not delete the master journal file.
                    ///
                    ///If the error occurs during the first call to
                    ///sqlite3BtreeCommitPhaseOne(), then there is a chance that the
                    ///master journal file will be orphaned. But we cannot delete it,
                    ///in case the master journal file name was written into the journal
                    ///file before the failure occurred.
                    ///
                    ///</summary>
                    for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nDb; i++)
                    {
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt != null)
                        {
                            rc = pBt.sqlite3BtreeCommitPhaseOne(zMaster);
                        }
                    }
                    os.sqlite3OsCloseFree(pMaster);
                    Debug.Assert(rc != SqlResult.SQLITE_BUSY);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        db.sqlite3DbFree(ref zMaster);
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///Delete the master journal file. This commits the transaction. After
                    ///doing this the directory is synced again before any individual
                    ///transaction files are deleted.
                    ///
                    ///</summary>
                    rc = os.sqlite3OsDelete(pVfs, zMaster, 1);
                    db.sqlite3DbFree(ref zMaster);
                    if (rc != 0)
                    {
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///All files and directories have already been synced, so the following
                    ///calls to sqlite3BtreeCommitPhaseTwo() are only closing files and
                    ///deleting or truncating journals. If something goes wrong while
                    ///this is happening we don't really care. The integrity of the
                    ///transaction is already guaranteed, but some stray 'cold' journals
                    ///may be lying around. Returning an error code won't help matters.
                    ///
                    ///</summary>
#if SQLITE_TEST
																																																																																																        disable_simulated_io_errors();
#endif
                    Sqlite3.sqlite3BeginBenignMalloc();
                    for (i = 0; i < db.nDb; i++)
                    {
                        Btree pBt = db.aDb[i].pBt;
                        if (pBt != null)
                        {
                            pBt.sqlite3BtreeCommitPhaseTwo(0);
                        }
                    }
                    Sqlite3.sqlite3EndBenignMalloc();
#if SQLITE_TEST
																																																																																																        enable_simulated_io_errors();
#endif
                    vtab.sqlite3VtabCommit(db);
                }
#endif
                return rc;
            }
            ///<summary>
            /// This routine checks that the sqlite3.activeVdbeCnt count variable
            /// matches the number of vdbe's in the list sqlite3.pVdbe that are
            /// currently active. An Debug.Assertion fails if the two counts do not match.
            /// This is an internal self-check only - it is not an essential processing
            /// step.
            ///
            /// This is a no-op if NDEBUG is defined.
            ///
            ///</summary>
#if !NDEBUG
																																																    static void checkActiveVdbeCnt( sqlite3 db )
    {
      Vdbe p;
      int cnt = 0;
      int nWrite = 0;
      p = db.pVdbe;
      while ( p != null )
      {
        if ( p.magic == VdbeMagic.VDBE_MAGIC_RUN && p.pc >= 0 )
        {
          cnt++;
          if ( p.readOnly == false )
            nWrite++;
        }
        p = p.pNext;
      }
      Debug.Assert( cnt == db.activeVdbeCnt );
      Debug.Assert( nWrite == db.writeVdbeCnt );
    }
#else
            //#define checkActiveVdbeCnt(x)
            public static void checkActiveVdbeCnt(sqlite3 db)
            {
            }
#endif
            ///<summary>
            /// For every Btree that in database connection db which
            /// has been modified, "trip" or invalidate each cursor in
            /// that Btree might have been modified so that the cursor
            /// can never be used again.  This happens when a rollback
            /// occurs.  We have to trip all the other cursors, even
            /// cursor from other VMs in different database connections,
            /// so that none of them try to use the data at which they
            /// were pointing and which now may have been changed due
            /// to the rollback.
            ///
            /// Remember that a rollback can delete tables complete and
            /// reorder rootpages.  So it is not sufficient just to save
            /// the state of the cursor.  We have to invalidate the cursor
            /// so that it is never used again.
            ///</summary>
            public static void invalidateCursorsOnModifiedBtrees(sqlite3 db)
            {
                int i;
                for (i = 0; i < db.nDb; i++)
                {
                    Btree p = db.aDb[i].pBt;
                    if (p != null && p.sqlite3BtreeIsInTrans())
                    {
                        p.sqlite3BtreeTripAllCursors(SqlResult.SQLITE_ABORT);
                    }
                }
            }
            ///
            ///<summary>
            ///</summary>
            ///<param name="If the Vdbe passed as the first argument opened a statement">transaction,</param>
            ///<param name="close it now. Argument eOp must be either sqliteinth.SAVEPOINT_ROLLBACK or">close it now. Argument eOp must be either sqliteinth.SAVEPOINT_ROLLBACK or</param>
            ///<param name="SAVEPOINT_RELEASE. If it is sqliteinth.SAVEPOINT_ROLLBACK, then the statement">SAVEPOINT_RELEASE. If it is sqliteinth.SAVEPOINT_ROLLBACK, then the statement</param>
            ///<param name="transaction is rolled back. If eOp is SAVEPOINT_RELEASE, then the">transaction is rolled back. If eOp is SAVEPOINT_RELEASE, then the</param>
            ///<param name="statement transaction is commtted.">statement transaction is commtted.</param>
            ///<param name=""></param>
            ///<param name="If an IO error occurs, an SQLITE_IOERR_XXX error code is returned.">If an IO error occurs, an SQLITE_IOERR_XXX error code is returned.</param>
            ///<param name="Otherwise SqlResult.SQLITE_OK.">Otherwise SqlResult.SQLITE_OK.</param>
            ///<param name=""></param>
            ///<summary>
            /// This function is called when a transaction opened by the database
            /// handle associated with the VM passed as an argument is about to be
            /// committed. If there are outstanding deferred foreign key constraint
            /// violations, return SqlResult.SQLITE_ERROR. Otherwise, SqlResult.SQLITE_OK.
            ///
            /// If there are outstanding FK violations and this function returns
            /// SqlResult.SQLITE_ERROR, set the result of the VM to SQLITE_CONSTRAINT and write
            /// an error message to it. Then return SqlResult.SQLITE_ERROR.
            ///
            ///</summary>
#if !SQLITE_OMIT_FOREIGN_KEY
#endif
            ///<summary>
            /// This routine is called the when a VDBE tries to halt.  If the VDBE
            /// has made changes and is in autocommit mode, then commit those
            /// changes.  If a rollback is needed, then do the rollback.
            ///
            /// This routine is the only way to move the state of a VM from
            /// SQLITE_MAGIC_RUN to SQLITE_MAGIC_HALT.  It is harmless to
            /// call this on a VM that is in the SQLITE_MAGIC_HALT state.
            ///
            /// Return an error code.  If the commit could not complete because of
            /// lock contention, return SQLITE_BUSY.  If SQLITE_BUSY is returned, it
            /// means the close did not happen and needs to be repeated.
            ///</summary>
            ///<summary>
            /// Each VDBE holds the result of the most recent sqlite3_step() call
            /// in p.rc.  This routine sets that result back to SqlResult.SQLITE_OK.
            ///
            ///</summary>
            ///<summary>
            /// Clean up a VDBE after execution but do not delete the VDBE just yet.
            /// Write any error messages into pzErrMsg.  Return the result code.
            ///
            /// After this routine is run, the VDBE should be ready to be executed
            /// again.
            ///
            /// To look at it another way, this routine resets the state of the
            /// virtual machine from VdbeMagic.VDBE_MAGIC_RUN or VdbeMagic.VDBE_MAGIC_HALT back to
            /// VdbeMagic.VDBE_MAGIC_INIT.
            ///
            ///</summary>
            ///<summary>
            /// Clean up and delete a VDBE after execution.  Return an integer which is
            /// the result code.  Write any error message text into pzErrMsg.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeFinalize(ref Vdbe p)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                if (p.magic == VdbeMagic.VDBE_MAGIC_RUN || p.magic == VdbeMagic.VDBE_MAGIC_HALT)
                {
                    rc = p.sqlite3VdbeReset();
                    Debug.Assert((rc & p.db.errMask) == rc);
                }
                sqlite3VdbeDelete(ref p);
                return rc;
            }
            ///<summary>
            /// Call the destructor for each auxdata entry in pVdbeFunc for which
            /// the corresponding bit in mask is clear.  Auxdata entries beyond 31
            /// are always destroyed.  To destroy all auxdata entries, call this
            /// routine with mask==0.
            ///
            ///</summary>
            public static void sqlite3VdbeDeleteAuxData(VdbeFunc pVdbeFunc, int mask)
            {
                int i;
                for (i = 0; i < pVdbeFunc.nAux; i++)
                {
                    AuxData pAux = pVdbeFunc.apAux[i];
                    if ((i > 31 || (mask & (((u32)1) << i)) == 0 && pAux.pAux != null))
                    {
                        if (pAux.pAux != null && pAux.pAux is IDisposable)
                        {
                            (pAux.pAux as IDisposable).Dispose();
                        }
                        pAux.pAux = null;
                    }
                }
            }
            ///<summary>
            /// Free all memory associated with the Vdbe passed as the second argument.
            /// The difference between this function and sqlite3VdbeDelete() is that
            /// VdbeDelete() also unlinks the Vdbe from the list of VMs associated with
            /// the database connection.
            ///
            ///</summary>
            public static void sqlite3VdbeDeleteObject(sqlite3 db, ref Vdbe p)
            {
                SubProgram pSub, pNext;
                int i;
                Debug.Assert(p.db == null || p.db == db);
                releaseMemArray(p.aVar, p.nVar);
                releaseMemArray(p.aColName, p.nResColumn, Vdbe.COLNAME_N);
                for (pSub = p.pProgram; pSub != null; pSub = pNext)
                {
                    pNext = pSub.pNext;
                    vdbeFreeOpArray(db, ref pSub.aOp, pSub.nOp);
                    db.sqlite3DbFree(ref pSub);
                }
                //for ( i = p->nzVar - 1; i >= 0; i-- )
                //  sqlite3DbFree( db, p.azVar[i] );
                vdbeFreeOpArray(db, ref p.aOp, p.nOp);
                db.sqlite3DbFree(ref p.aLabel);
                db.sqlite3DbFree(ref p.aColName);
                db.sqlite3DbFree(ref p.zSql);
                db.sqlite3DbFree(ref p.pFree);
                // Free memory allocated from db within p
                //sqlite3DbFree( db, p );
            }
            ///<summary>
            /// Delete an entire VDBE.
            ///
            ///</summary>
            public static void sqlite3VdbeDelete(ref Vdbe p)
            {
                if (Sqlite3.NEVER(p == null))
                    return;
                Cleanup(p);
                sqlite3 db = p.db;
                if (p.pPrev != null)
                {
                    p.pPrev.pNext = p.pNext;
                }
                else
                {
                    Debug.Assert(db.pVdbe == p);
                    db.pVdbe = p.pNext;
                }
                if (p.pNext != null)
                {
                    p.pNext.pPrev = p.pPrev;
                }
                p.magic = VdbeMagic.VDBE_MAGIC_DEAD;
                p.db = null;
                sqlite3VdbeDeleteObject(db, ref p);
            }
            ///
            ///<summary>
            ///Make sure the cursor p is ready to read or write the row to which it
            ///was last positioned.  Return an error code if an OOM fault or I/O error
            ///prevents us from positioning the cursor to its correct position.
            ///
            ///If a MoveTo operation is pending on the given cursor, then do that
            ///MoveTo now.  If no move is pending, check to see if the row has been
            ///deleted out from under the cursor and if it has, mark the row as
            ///a NULL row.
            ///
            ///If the cursor is already pointing to the correct row and that row has
            ///</summary>
            ///<param name="not been deleted out from under the cursor, then this routine is a no">op.</param>
            ///<param name=""></param>
            public static SqlResult sqlite3VdbeCursorMoveto(VdbeCursor p)
            {
                if (p.deferredMoveto)
                {
                    int res = 0;
                    SqlResult rc;
#if SQLITE_TEST
																																																																																																        //extern int sqlite3_search_count;
#endif
                    Debug.Assert(p.isTable);
                    rc = p.pCursor.sqlite3BtreeMovetoUnpacked(null, p.movetoTarget, 0, ref res);
                    if (rc != 0)
                        return rc;
                    p.lastRowid = p.movetoTarget;
                    if (res != 0)
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    p.rowidIsValid = true;
#if SQLITE_TEST
#if !TCLSH
																																																																																																        sqlite3_search_count++;
#else
																																																																																																        sqlite3_search_count.iValue++;
#endif
#endif
                    p.deferredMoveto = false;
                    p.cacheStatus = Sqlite3.CACHE_STALE;
                }
                else
                    if (Sqlite3.ALWAYS(p.pCursor != null))
                    {
                        int hasMoved = 0;
                        SqlResult rc = p.pCursor.sqlite3BtreeCursorHasMoved(ref hasMoved);
                        if (rc != 0)
                            return rc;
                        if (hasMoved != 0)
                        {
                            p.cacheStatus = Sqlite3.CACHE_STALE;
                            p.nullRow = true;
                        }
                    }
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// The following functions:
            ///
            /// sqlite3VdbeSerialType()
            /// sqlite3VdbeSerialTypeLen()
            /// sqlite3VdbeSerialLen()
            /// sqlite3VdbeSerialPut()
            /// sqlite3VdbeSerialGet()
            ///
            /// encapsulate the code that serializes values for storage in SQLite
            /// data and index records. Each serialized value consists of a
            /// 'serial-type' and a blob of data. The serial type is an 8-byte unsigned
            /// integer, stored as a varint.
            ///
            /// In an SQLite index record, the serial type is stored directly before
            /// the blob of data that it corresponds to. In a table record, all serial
            /// types are stored at the start of the record, and the blobs of data at
            /// the end. Hence these functions allow the caller to handle the
            /// serial-type and data blob seperately.
            ///
            /// The following table describes the various storage classes for data:
            ///
            ///   serial type        bytes of data      type
            ///   --------------     ---------------    ---------------
            ///      0                     0            NULL
            ///      1                     1            signed integer
            ///      2                     2            signed integer
            ///      3                     3            signed integer
            ///      4                     4            signed integer
            ///      5                     6            signed integer
            ///      6                     8            signed integer
            ///      7                     8            IEEE float
            ///      8                     0            Integer constant 0
            ///      9                     0            Integer constant 1
            ///     10,11                               reserved for expansion
            ///    N>=12 and even       (N-12)/2        BLOB
            ///    N>=13 and odd        (N-13)/2        text
            ///
            /// The 8 and 9 types were added in 3.3.0, file format 4.  Prior versions
            /// of SQLite will not understand those serial types.
            ///
            ///</summary>
            ///
            ///<summary>
            ///</summary>
            ///<param name="Return the serial">type for the value stored in pMem.</param>
            ///<param name=""></param>
            public static u32 sqlite3VdbeSerialType(Mem pMem, int file_format)
            {
                MemFlags flags = pMem.flags;
                int n;
                if ((flags & MemFlags.MEM_Null) != 0)
                {
                    return 0;
                }
                if ((flags & MemFlags.MEM_Int) != 0)
                {
                    ///
                    ///<summary>
                    ///Figure out whether to use 1, 2, 4, 6 or 8 bytes. 
                    ///</summary>
                    const i64 MAX_6BYTE = ((((i64)0x00008000) << 32) - 1);
                    i64 i = pMem.u.i;
                    u64 u;
                    if (file_format >= 4 && (i & 1) == i)
                    {
                        return 8 + (u32)i;
                    }
                    if (i < 0)
                    {
                        if (i < (-MAX_6BYTE))
                            return 6;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Previous test prevents:  u = ">9223372036854775808) </param>
                        u = (u64)(-i);
                    }
                    else
                    {
                        u = (u64)i;
                    }
                    if (u <= 127)
                        return 1;
                    if (u <= 32767)
                        return 2;
                    if (u <= 8388607)
                        return 3;
                    if (u <= 2147483647)
                        return 4;
                    if (u <= MAX_6BYTE)
                        return 5;
                    return 6;
                }
                if ((flags & MemFlags.MEM_Real) != 0)
                {
                    return 7;
                }
                Debug.Assert(///
                    ///<summary>
                    ///pMem.db.mallocFailed != 0 || 
                    ///</summary>
                (flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) != 0);
                n = pMem.n;
                if ((flags & MemFlags.MEM_Zero) != 0)
                {
                    n += pMem.u.nZero;
                }
                else
                    if ((flags & MemFlags.MEM_Blob) != 0)
                    {
                        n = pMem.zBLOB != null ? pMem.zBLOB.Length : pMem.z != null ? pMem.z.Length : 0;
                    }
                    else
                    {
                        if (pMem.z != null)
                            n = Encoding.UTF8.GetByteCount(pMem.n < pMem.z.Length ? pMem.z.Substring(0, pMem.n) : pMem.z);
                        else
                            n = pMem.zBLOB.Length;
                        pMem.n = n;
                    }
                Debug.Assert(n >= 0);
                return (u32)((n * 2) + 12 + (((flags & MemFlags.MEM_Str) != 0) ? 1 : 0));
            }
            ///<summary>
            /// Return the length of the data corresponding to the supplied serial-type.
            ///
            ///</summary>
            static u32[] aSize = new u32[] {
			0,
			1,
			2,
			3,
			4,
			6,
			8,
			8,
			0,
			0,
			0,
			0
		};
            public static u32 sqlite3VdbeSerialTypeLen(u32 serial_type)
            {
                if (serial_type >= 12)
                {
                    return (u32)((serial_type - 12) / 2);
                }
                else
                {
                    return aSize[serial_type];
                }
            }
            ///<summary>
            /// If we are on an architecture with mixed-endian floating
            /// points (ex: ARM7) then swap the lower 4 bytes with the
            /// upper 4 bytes.  Return the result.
            ///
            /// For most architectures, this is a no-op.
            ///
            /// (later):  It is reported to me that the mixed-endian problem
            /// on ARM7 is an issue with GCC, not with the ARM7 chip.  It seems
            /// that early versions of GCC stored the two words of a 64-bit
            /// float in the wrong order.  And that error has been propagated
            /// ever since.  The blame is not necessarily with GCC, though.
            /// GCC might have just copying the problem from a prior compiler.
            /// I am also told that newer versions of GCC that follow a different
            /// ABI get the byte order right.
            ///
            /// Developers using SQLite on an ARM7 should compile and run their
            /// application using -DSQLITE_DEBUG=1 at least once.  With DEBUG
            /// enabled, some Debug.Asserts below will ensure that the byte order of
            /// floating point values is correct.
            ///
            /// (2007-08-30)  Frank van Vugt has studied this problem closely
            /// and has send his findings to the SQLite developers.  Frank
            /// writes that some Linux kernels offer floating point hardware
            /// emulation that uses only 32-bit mantissas instead of a full
            /// 48-bits as required by the IEEE standard.  (This is the
            /// CONFIG_FPE_FASTFPE option.)  On such systems, floating point
            /// byte swapping becomes very complicated.  To avoid problems,
            /// the necessary byte swapping is carried out using a 64-bit integer
            /// rather than a 64-bit float.  Frank assures us that the code here
            /// works for him.  We, the developers, have no way to independently
            /// verify this, but Frank seems to know what he is talking about
            /// so we trust him.
            ///
            ///</summary>
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																//static u64 floatSwap(u64 in){
//  union {
//    u64 r;
//    u32 i[2];
//  } u;
//  u32 t;

//  u.r = in;
//  t = u.i[0];
//  u.i[0] = u.i[1];
//  u.i[1] = t;
//  return u.r;
//}
// define swapMixedEndianFloat(X)  X = floatSwap(X)
#else
            //# define swapMixedEndianFloat(X)
#endif
            ///<summary>
            /// Write the serialized data blob for the value stored in pMem into
            /// buf. It is assumed that the caller has allocated sufficient space.
            /// Return the number of bytes written.
            ///
            /// nBuf is the amount of space left in buf[].  nBuf must always be
            /// large enough to hold the entire field.  Except, if the field is
            /// a blob with a zero-filled tail, then buf[] might be just the right
            /// size to hold everything except for the zero-filled tail.  If buf[]
            /// is only big enough to hold the non-zero prefix, then only write that
            /// prefix into buf[].  But if buf[] is large enough to hold both the
            /// prefix and the tail then write the prefix and set the tail to all
            /// zeros.
            ///
            /// Return the number of bytes actually written into buf[].  The number
            /// of bytes in the zero-filled tail is included in the return value only
            /// if those bytes were zeroed in buf[].
            ///</summary>
            public static u32 sqlite3VdbeSerialPut(byte[] buf, int offset, int nBuf, Mem pMem, int file_format)
            {
                u32 serial_type = sqlite3VdbeSerialType(pMem, file_format);
                u32 len;
                ///
                ///<summary>
                ///Integer and Real 
                ///</summary>
                if (serial_type <= 7 && serial_type > 0)
                {
                    u64 v;
                    u32 i;
                    if (serial_type == 7)
                    {
                        //Debug.Assert( sizeof( v) == sizeof(pMem.r));
#if WINDOWS_PHONE || WINDOWS_MOBILE
																																																																																																																								v = (ulong)BitConverter.ToInt64(BitConverter.GetBytes(pMem.r),0);
#else
                        v = (ulong)BitConverter.DoubleToInt64Bits(pMem.r);
                        // memcpy( &v, pMem.r, v ).Length;
#endif
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																																																																																								swapMixedEndianFloat( v );
#endif
                    }
                    else
                    {
                        v = (ulong)pMem.u.i;
                    }
                    len = i = sqlite3VdbeSerialTypeLen(serial_type);
                    Debug.Assert(len <= (u32)nBuf);
                    while (i-- != 0)
                    {
                        buf[offset + i] = (u8)(v & 0xFF);
                        v >>= 8;
                    }
                    return len;
                }
                ///
                ///<summary>
                ///String or blob 
                ///</summary>
                if (serial_type >= 12)
                {
                    // TO DO -- PASS TESTS WITH THIS ON Debug.Assert( pMem.n + ( ( pMem.flags & MEM.MEM_Zero ) != 0 ? pMem.u.nZero : 0 ) == (int)sqlite3VdbeSerialTypeLen( serial_type ) );
                    Debug.Assert(pMem.n <= nBuf);
                    if ((len = (u32)pMem.n) != 0)
                        if (pMem.zBLOB == null && String.IsNullOrEmpty(pMem.z))
                        {
                        }
                        else
                            if (pMem.zBLOB != null && ((pMem.flags & MemFlags.MEM_Blob) != 0 || pMem.z == null))
                                Buffer.BlockCopy(pMem.zBLOB, 0, buf, offset, (int)len);
                            //memcpy( buf, pMem.z, len );
                            else
                                Buffer.BlockCopy(Encoding.UTF8.GetBytes(pMem.z), 0, buf, offset, (int)len);
                    //memcpy( buf, pMem.z, len );
                    if ((pMem.flags & MemFlags.MEM_Zero) != 0)
                    {
                        len += (u32)pMem.u.nZero;
                        Debug.Assert(nBuf >= 0);
                        if (len > (u32)nBuf)
                        {
                            len = (u32)nBuf;
                        }
                        Array.Clear(buf, offset + pMem.n, (int)(len - pMem.n));
                        // memset( &buf[pMem.n], 0, len - pMem.n );
                    }
                    return len;
                }
                ///
                ///<summary>
                ///NULL or constants 0 or 1 
                ///</summary>
                return 0;
            }
            ///<summary>
            /// Deserialize the data blob pointed to by buf as serial type serial_type
            /// and store the result in pMem.  Return the number of bytes read.
            ///
            ///</summary>
            public static u32 sqlite3VdbeSerialGet(byte[] buf,///
                ///<summary>
                ///Buffer to deserialize from 
                ///</summary>
            int offset,///
                ///<summary>
                ///Offset into Buffer 
                ///</summary>
            u32 serial_type,///
                ///<summary>
                ///Serial type to deserialize 
                ///</summary>
            Mem result///
                ///<summary>
                ///Memory cell to write value into 
                ///</summary>
            )
            {
                switch (serial_type)
                {
                    case 10:
                    ///
                    ///<summary>
                    ///Reserved for future use 
                    ///</summary>
                    case 11:
                    ///
                    ///<summary>
                    ///Reserved for future use 
                    ///</summary>
                    case 0:
                        {
                            ///
                            ///<summary>
                            ///NULL 
                            ///</summary>
                            result.flags = MemFlags.MEM_Null;
                            result.n = 0;
                            result.z = null;
                            result.zBLOB = null;
                            break;
                        }
                    case 1:
                        {
                            ///<param name="1">byte signed integer </param>
                            result.u.i = (sbyte)buf[offset + 0];
                            result.flags = MemFlags.MEM_Int;
                            return 1;
                        }
                    case 2:
                        {
                            ///<param name="2">byte signed integer </param>
                            result.u.i = (int)((((sbyte)buf[offset + 0]) << 8) | buf[offset + 1]);
                            result.flags = MemFlags.MEM_Int;
                            return 2;
                        }
                    case 3:
                        {
                            ///<param name="3">byte signed integer </param>
                            result.u.i = (int)((((sbyte)buf[offset + 0]) << 16) | (buf[offset + 1] << 8) | buf[offset + 2]);
                            result.flags = MemFlags.MEM_Int;
                            return 3;
                        }
                    case 4:
                        {
                            ///<param name="4">byte signed integer </param>
                            result.u.i = (int)(((sbyte)buf[offset + 0] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
                            result.flags = MemFlags.MEM_Int;
                            return 4;
                        }
                    case 5:
                        {
                            ///<param name="6">byte signed integer </param>
                            u64 x = (ulong)((((sbyte)buf[offset + 0]) << 8) | buf[offset + 1]);
                            u32 y = (u32)((buf[offset + 2] << 24) | (buf[offset + 3] << 16) | (buf[offset + 4] << 8) | buf[offset + 5]);
                            x = (x << 32) | y;
                            result.u.i = (i64)x;
                            result.flags = MemFlags.MEM_Int;
                            return 6;
                        }
                    case 6:
                    ///<param name="8">byte signed integer </param>
                    case 7:
                        {
                            ///
                            ///<summary>
                            ///IEEE floating point 
                            ///</summary>
                            u64 x;
                            u32 y;
#if !NDEBUG && !SQLITE_OMIT_FLOATING_POINT
																																																																																																            /* Verify that integers and floating point values use the same
** byte order.  Or, that if SQLITE_MIXED_ENDIAN_64BIT_FLOAT is
** defined that 64-bit floating point values really are mixed
** endian.
*/
            const u64 t1 = ( (u64)0x3ff00000 ) << 32;
            const double r1 = 1.0;
            u64 t2 = t1;
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																																																																swapMixedEndianFloat(t2);
#endif
																																																																																																            Debug.Assert( sizeof( double ) == sizeof( u64 ) && memcmp( BitConverter.GetBytes( r1 ), BitConverter.GetBytes( t2 ), sizeof( double ) ) == 0 );//Debug.Assert( sizeof(r1)==sizeof(t2) && memcmp(&r1, t2, sizeof(r1))==0 );
#endif
                            x = (u64)((buf[offset + 0] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3]);
                            y = (u32)((buf[offset + 4] << 24) | (buf[offset + 5] << 16) | (buf[offset + 6] << 8) | buf[offset + 7]);
                            x = (x << 32) | y;
                            if (serial_type == 6)
                            {
                                result.u.i = (i64)x;
                                result.flags = MemFlags.MEM_Int;
                            }
                            else
                            {
                                Debug.Assert(sizeof(i64) == 8 && sizeof(double) == 8);
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																																																																																								swapMixedEndianFloat(x);
#endif
#if WINDOWS_PHONE || WINDOWS_MOBILE
																																																																																																																								              pMem.r = BitConverter.ToDouble(BitConverter.GetBytes((long)x), 0);
#else
                                result.r = BitConverter.Int64BitsToDouble((long)x);
                                // memcpy(pMem.r, x, sizeof(x))
#endif
                                result.flags = (MathExtensions.sqlite3IsNaN(result.r) ? MemFlags.MEM_Null : MemFlags.MEM_Real);
                            }
                            return 8;
                        }
                    case 8:
                    ///Integer 0 
                    case 9:
                        {
                            ///Integer 1 
                            result.u.i = serial_type - 8;
                            result.flags = MemFlags.MEM_Int;
                            return 0;
                        }
                    default:
                        {
                            u32 len = (serial_type - 12) / 2;
                            result.n = (int)len;
                            result.xDel = null;
                            if ((serial_type & 0x01) != 0)
                            {
                                result.flags = MemFlags.MEM_Str | MemFlags.MEM_Ephem;
                                if (len <= buf.Length - offset)
                                {
                                    result.z = Encoding.UTF8.GetString(buf, offset, (int)len);
                                    //memcpy( buf, pMem.z, len );
                                    result.n = result.z.Length;
                                }
                                else
                                {
                                    result.z = "";
                                    // Corrupted Data
                                    result.n = 0;
                                }
                                result.zBLOB = null;
                            }
                            else
                            {
                                result.z = null;
                                result.zBLOB = malloc_cs.sqlite3Malloc((int)len);
                                result.flags = MemFlags.MEM_Blob | MemFlags.MEM_Ephem;
                                if (len <= buf.Length - offset)
                                {
                                    Buffer.BlockCopy(buf, offset, result.zBLOB, 0, (int)len);
                                    //memcpy( buf, pMem.z, len );
                                }
                                else
                                {
                                    Buffer.BlockCopy(buf, offset, result.zBLOB, 0, buf.Length - offset - 1);
                                }
                            }
                            return len;
                        }
                }
                return 0;
            }
            
            
            public static int sqlite3VdbeSerialGet(byte[] buf,///
                ///<summary>
                ///Buffer to deserialize from 
                ///</summary>
            u32 serial_type,///
                ///<summary>
                ///Serial type to deserialize 
                ///</summary>
            Mem pMem///
                ///<summary>
                ///Memory cell to write value into 
                ///</summary>
            )
            {
                switch (serial_type)
                {
                    case 10:
                    ///
                    ///<summary>
                    ///Reserved for future use 
                    ///</summary>
                    case 11:
                    ///
                    ///<summary>
                    ///Reserved for future use 
                    ///</summary>
                    case 0:
                        {
                            ///
                            ///<summary>
                            ///NULL 
                            ///</summary>
                            pMem.flags = MemFlags.MEM_Null;
                            break;
                        }
                    case 1:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="1">byte signed integer </param>
                            pMem.u.i = (sbyte)buf[0];
                            pMem.flags = MemFlags.MEM_Int;
                            return 1;
                        }
                    case 2:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="2">byte signed integer </param>
                            pMem.u.i = (int)(((buf[0]) << 8) | buf[1]);
                            pMem.flags = MemFlags.MEM_Int;
                            return 2;
                        }
                    case 3:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="3">byte signed integer </param>
                            pMem.u.i = (int)(((buf[0]) << 16) | (buf[1] << 8) | buf[2]);
                            pMem.flags = MemFlags.MEM_Int;
                            return 3;
                        }
                    case 4:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="4">byte signed integer </param>
                            pMem.u.i = (int)((buf[0] << 24) | (buf[1] << 16) | (buf[2] << 8) | buf[3]);
                            pMem.flags = MemFlags.MEM_Int;
                            return 4;
                        }
                    case 5:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="6">byte signed integer </param>
                            u64 x = (ulong)(((buf[0]) << 8) | buf[1]);
                            u32 y = (u32)((buf[2] << 24) | (buf[3] << 16) | (buf[4] << 8) | buf[5]);
                            x = (x << 32) | y;
                            pMem.u.i = (i64)x;
                            pMem.flags = MemFlags.MEM_Int;
                            return 6;
                        }
                    case 6:
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="8">byte signed integer </param>
                    case 7:
                        {
                            ///
                            ///<summary>
                            ///IEEE floating point 
                            ///</summary>
                            u64 x;
                            u32 y;
#if !NDEBUG && !SQLITE_OMIT_FLOATING_POINT
																																																																																																            /* Verify that integers and floating point values use the same
** byte order.  Or, that if SQLITE_MIXED_ENDIAN_64BIT_FLOAT is
** defined that 64-bit floating point values really are mixed
** endian.
*/
            const u64 t1 = ( (u64)0x3ff00000 ) << 32;
            const double r1 = 1.0;
            u64 t2 = t1;
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																																																																swapMixedEndianFloat(t2);
#endif
																																																																																																            Debug.Assert( sizeof( double ) == sizeof( u64 ) && memcmp( BitConverter.GetBytes( r1 ), BitConverter.GetBytes( t2 ), sizeof( double ) ) == 0 );//Debug.Assert( sizeof(r1)==sizeof(t2) && memcmp(&r1, t2, sizeof(r1))==0 );
#endif
                            x = (u64)((buf[0] << 24) | (buf[1] << 16) | (buf[2] << 8) | buf[3]);
                            y = (u32)((buf[4] << 24) | (buf[5] << 16) | (buf[6] << 8) | buf[7]);
                            x = (x << 32) | y;
                            if (serial_type == 6)
                            {
                                pMem.u.i = (i64)x;
                                pMem.flags = MemFlags.MEM_Int;
                            }
                            else
                            {
                                Debug.Assert(sizeof(i64) == 8 && sizeof(double) == 8);
#if SQLITE_MIXED_ENDIAN_64BIT_FLOAT
																																																																																																																								swapMixedEndianFloat(x);
#endif
#if WINDOWS_PHONE || WINDOWS_MOBILE
																																																																																																																								              pMem.r = BitConverter.ToDouble(BitConverter.GetBytes((long)x), 0);
#else
                                pMem.r = BitConverter.Int64BitsToDouble((long)x);
                                // memcpy(pMem.r, x, sizeof(x))
#endif
                                pMem.flags = MemFlags.MEM_Real;
                            }
                            return 8;
                        }
                    case 8:
                    ///
                    ///<summary>
                    ///Integer 0 
                    ///</summary>
                    case 9:
                        {
                            ///
                            ///<summary>
                            ///Integer 1 
                            ///</summary>
                            pMem.u.i = serial_type - 8;
                            pMem.flags = MemFlags.MEM_Int;
                            return 0;
                        }
                    default:
                        {
                            int len = (int)((serial_type - 12) / 2);
                            pMem.xDel = null;
                            if ((serial_type & 0x01) != 0)
                            {
                                pMem.flags = MemFlags.MEM_Str | MemFlags.MEM_Ephem;
                                pMem.z = Encoding.UTF8.GetString(buf, 0, len);
                                //memcpy( buf, pMem.z, len );
                                pMem.n = pMem.z.Length;
                                // len;
                                pMem.zBLOB = null;
                            }
                            else
                            {
                                pMem.flags = MemFlags.MEM_Blob | MemFlags.MEM_Ephem;
                                pMem.zBLOB = malloc_cs.sqlite3Malloc(len);
                                buf.CopyTo(pMem.zBLOB, 0);
                                pMem.n = len;
                                // len;
                                pMem.z = null;
                            }
                            return len;
                        }
                }
                return 0;
            }
            ///<summary>
            /// Given the nKey-byte encoding of a record in pKey[], parse the
            /// record into a UnpackedRecord structure.  Return a pointer to
            /// that structure.
            ///
            /// The calling function might provide szSpace bytes of memory
            /// space at pSpace.  This space can be used to hold the returned
            /// VDbeParsedRecord structure if it is large enough.  If it is
            /// not big enough, space is obtained from malloc_cs.sqlite3Malloc().
            ///
            /// The returned structure should be closed by a call to
            /// sqlite3VdbeDeleteUnpackedRecord().
            ///
            ///</summary>
            public static UnpackedRecord sqlite3VdbeRecordUnpack(KeyInfo pKeyInfo,///
                ///<summary>
                ///Information about the record format 
                ///</summary>
            int nKey,///
                ///<summary>
                ///Size of the binary record 
                ///</summary>
            byte[] pKey,///
                ///<summary>
                ///The binary record 
                ///</summary>
            UnpackedRecord pSpace,//  char *pSpace,          /* Unaligned space available to hold the object */
            int szSpace///
                ///<summary>
                ///Size of pSpace[] in bytes 
                ///</summary>
            )
            {
                byte[] aKey = pKey;
                UnpackedRecord p;
                ///
                ///<summary>
                ///The unpacked record that we will return 
                ///</summary>
                int nByte;
                ///
                ///<summary>
                ///Memory space needed to hold p, in bytes 
                ///</summary>
                int d;
                u32 idx;
                int u;
                ///
                ///<summary>
                ///Unsigned loop counter 
                ///</summary>
                int szHdr = 0;
                Mem pMem;
                int nOff;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Increase pSpace by this much to 8">byte align it </param>
                ///
                ///<summary>
                ///</summary>
                ///<param name="We want to shift the pointer pSpace up such that it is 8">byte aligned.</param>
                ///<param name="Thus, we need to calculate a value, nOff, between 0 and 7, to shift">Thus, we need to calculate a value, nOff, between 0 and 7, to shift</param>
                ///<param name="it by.  If pSpace is already 8">byte aligned, nOff should be zero.</param>
                ///<param name=""></param>
                //nOff = ( 8 - ( SQLITE_PTR_TO_INT( pSpace ) & 7 ) ) & 7;
                //pSpace += nOff;
                //szSpace -= nOff;
                //nByte = ROUND8( sizeof( UnpackedRecord ) ) + sizeof( Mem ) * ( pKeyInfo->nField + 1 );
                //if ( nByte > szSpace)
                //{
                //var  p = new UnpackedRecord();//sqlite3DbMallocRaw(pKeyInfo.db, nByte);
                //  if ( p == null ) return null;
                //  p.flags = UNPACKED_NEED_FREE | UNPACKED_NEED_DESTROY;
                //}
                //else
                {
                    p = pSpace;
                    //(UnpackedRecord)pSpace;
                    p.flags = UnpackedRecordFlags.UNPACKED_NEED_DESTROY;
                    //p->aMem = pMem = (Mem)&( (char)p )[ROUND8( sizeof( UnpackedRecord ) )];
                    //Debug.Assert( EIGHT_BYTE_ALIGNMENT( pMem ) );
                    // GetVarint( aKey, szHdr );
                    // (void)p;
                }
                p.pKeyInfo = pKeyInfo;
                p.nField = (u16)(pKeyInfo.nField + 1);
                p.aMem = new Mem[p.nField + 1];
                idx = (u32)utilc.getVarint32(aKey, 0, out szHdr);
                d = (int)szHdr;
                u = 0;
                while (idx < (int)szHdr && u < p.nField && d <= nKey)
                {
                    p.aMem[u] = malloc_cs.sqlite3Malloc(p.aMem[u]);
                    //---------------------------------
                    pMem = p.aMem[u];
                    u32 serial_type = 0;
                    idx += (u32)utilc.getVarint32(aKey, idx, out serial_type);
                    // GetVarint( aKey + idx, serial_type );
                    pMem.enc = pKeyInfo.enc;
                    pMem.db = pKeyInfo.db;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="pMem">>flags = 0; // sqlite3VdbeSerialGet() will set this for us </param>
                    //pMem.zMalloc = null;
                    d += (int)sqlite3VdbeSerialGet(aKey, d, serial_type, pMem);
                    //pMem++;
                    u++;
                }
                Debug.Assert(u <= pKeyInfo.nField + 1);
                p.nField = (u16)u;
                return p;
            }
            ///
            ///<summary>
            ///This routine destroys a UnpackedRecord object.
            ///
            ///</summary>
            public static void sqlite3VdbeDeleteUnpackedRecord(UnpackedRecord p)
            {
#if SQLITE_DEBUG
																																																																								      int i;
      Mem pMem;
      Debug.Assert( p != null );
      Debug.Assert( ( p.flags & UNPACKED_NEED_DESTROY ) != 0 );
      //for ( i = 0, pMem = p->aMem ; i < p->nField ; i++, pMem++ )
      //{
      //  /* The unpacked record is always constructed by the
      //  ** sqlite3VdbeUnpackRecord() function above, which makes all
      //  ** strings and blobs static.  And none of the elements are
      //  ** ever transformed, so there is never anything to delete.
      //  */
      //  if ( NEVER( pMem->zMalloc ) )  pMem .sqlite3VdbeMemRelease();
      //}
#endif
                if ((p.flags & UnpackedRecordFlags.UNPACKED_NEED_FREE) != 0)
                {
                    p.pKeyInfo.db.sqlite3DbFree(ref p.aMem);
                    p = null;
                }
            }
            ///<summary>
            /// This function compares the two table rows or index records
            /// specified by {nKey1, pKey1} and pPKey2.  It returns a negative, zero
            /// or positive integer if key1 is less than, equal to or
            /// greater than key2.  The {nKey1, pKey1} key must be a blob
            /// created by th OP_MakeRecord opcode of the VDBE.  The pPKey2
            /// key must be a parsed key such as obtained from
            /// sqlite3VdbeParseRecord.
            ///
            /// Key1 and Key2 do not have to contain the same number of fields.
            /// The key with fewer fields is usually compares less than the
            /// longer key.  However if the UNPACKED_INCRKEY flags in pPKey2 is set
            /// and the common prefixes are equal, then key1 is less than key2.
            /// Or if the UNPACKED_MATCH_PREFIX flag is set and the prefixes are
            /// equal, then the keys are considered to be equal and
            /// the parts beyond the common prefix are ignored.
            ///
            /// If the UNPACKED_IGNORE_ROWID flag is set, then the last byte of
            /// the header of pKey1 is ignored.  It is assumed that pKey1 is
            /// an index key, and thus ends with a rowid value.  The last byte
            /// of the header will therefore be the serial type of the rowid:
            /// one of 1, 2, 3, 4, 5, 6, 8, or 9 - the integer serial types.
            /// The serial type of the final rowid will always be a single byte.
            /// By ignoring this last byte of the header, we force the comparison
            /// to ignore the rowid at the end of key1.
            ///
            ///</summary>
            static Mem mem1 = new Mem();
            // ALTERNATE FORM for C#
            public static int sqlite3VdbeRecordCompare(int nKey1, byte[] pKey1,///
                ///<summary>
                ///Left key 
                ///</summary>
            UnpackedRecord pPKey2///
                ///<summary>
                ///Right key 
                ///</summary>
            )
            {
                return sqlite3VdbeRecordCompare(nKey1, pKey1, 0, pPKey2);
            }
            
            public static int sqlite3VdbeRecordCompare(int nKey1, byte[] pKey1,///
                ///<summary>
                ///Left key 
                ///</summary>
            int offset, UnpackedRecord pPKey2///
                ///<summary>
                ///Right key 
                ///</summary>
            )
            {
                int d1;
                ///
                ///<summary>
                ///Offset into aKey[] of next data element 
                ///</summary>
                u32 idx1;
                ///
                ///<summary>
                ///Offset into aKey[] of next header element 
                ///</summary>
                u32 szHdr1;
                ///
                ///<summary>
                ///Number of bytes in header 
                ///</summary>
                int i = 0;
                int nField;
                var rc = 0;
                byte[] aKey1 = new byte[pKey1.Length - offset];
                //Buffer.BlockCopy( pKey1, offset, aKey1, 0, aKey1.Length );
                KeyInfo pKeyInfo;
                pKeyInfo = pPKey2.pKeyInfo;
                mem1.enc = pKeyInfo.enc;
                mem1.db = pKeyInfo.db;
                ///
                ///<summary>
                ///mem1.flags = 0;  // Will be initialized by sqlite3VdbeSerialGet() 
                ///</summary>
                //  VVA_ONLY( mem1.zMalloc = 0; ) /* Only needed by Debug.Assert() statements */
                ///
                ///<summary>
                ///Compilers may complain that mem1.u.i is potentially uninitialized.
                ///We could initialize it, as shown here, to silence those complaints.
                ///But in fact, mem1.u.i will never actually be used uninitialized, and doing 
                ///the unnecessary initialization has a measurable negative performance
                ///impact, since this routine is a very high runner.  And so, we choose
                ///to ignore the compiler warnings and leave this variable uninitialized.
                ///
                ///</summary>
                ///
                ///<summary>
                ///mem1.u.i = 0;  // not needed, here to silence compiler warning 
                ///</summary>
                idx1 = (u32)((szHdr1 = pKey1[offset]) <= 0x7f ? 1 : utilc.getVarint32(pKey1, offset, out szHdr1));
                // GetVarint( aKey1, szHdr1 );
                d1 = (int)szHdr1;
                if ((pPKey2.flags & UnpackedRecordFlags.UNPACKED_IGNORE_ROWID) != 0)
                {
                    szHdr1--;
                }
                nField = pKeyInfo.nField;
                while (idx1 < szHdr1 && i < pPKey2.nField)
                {
                    u32 serial_type1;
                    ///
                    ///<summary>
                    ///Read the serial types for the next element in each key. 
                    ///</summary>
                    idx1 += (u32)((serial_type1 = pKey1[offset + idx1]) <= 0x7f ? 1 : utilc.getVarint32(pKey1, (uint)(offset + idx1), out serial_type1));
                    //GetVarint( aKey1 + idx1, serial_type1 );
                    if (d1 <= 0 || d1 >= nKey1 && sqlite3VdbeSerialTypeLen(serial_type1) > 0)
                        break;
                    ///
                    ///<summary>
                    ///Extract the values to be compared.
                    ///
                    ///</summary>
                    d1 += (int)sqlite3VdbeSerialGet(pKey1, offset + d1, serial_type1, mem1);
                    //sqlite3VdbeSerialGet( aKey1, d1, serial_type1, mem1 );
                    ///
                    ///<summary>
                    ///Do the comparison
                    ///
                    ///</summary>
                    rc = vdbemem_cs.sqlite3MemCompare(mem1, pPKey2.aMem[i], i < nField ? pKeyInfo.aColl[i] : null);
                    if (rc != 0)
                    {
                        //Debug.Assert( mem1.zMalloc==null );  /* See comment below */
                        ///
                        ///<summary>
                        ///Invert the result if we are using DESC sort order. 
                        ///</summary>
                        if (pKeyInfo.aSortOrder != null && i < nField && pKeyInfo.aSortOrder[i] != 0)
                        {
                            rc = -rc;
                        }
                        ///
                        ///<summary>
                        ///If the PREFIX_SEARCH flag is set and all fields except the final
                        ///rowid field were equal, then clear the PREFIX_SEARCH flag and set
                        ///</summary>
                        ///<param name="pPKey2">>rowid to the value of the rowid field in (pKey1, nKey1).</param>
                        ///<param name="This is used by the OP_IsUnique opcode.">This is used by the OP_IsUnique opcode.</param>
                        ///<param name=""></param>
                        if ((pPKey2.flags & UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH) != 0 && i == (pPKey2.nField - 1))
                        {
                            Debug.Assert(idx1 == szHdr1 && rc != 0);
                            Debug.Assert((mem1.flags & MemFlags.MEM_Int) != 0);
                            pPKey2.flags = (pPKey2.flags & ~UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH);
                            pPKey2.rowid = mem1.u.i;
                        }
                        return rc;
                    }
                    i++;
                }
                ///
                ///<summary>
                ///No memory allocation is ever used on mem1.  Prove this using
                ///the following Debug.Assert().  If the Debug.Assert() fails, it indicates a
                ///memory leak and a need to call &mem1.sqlite3VdbeMemRelease().
                ///
                ///</summary>
                //Debug.Assert( mem1.zMalloc==null );
                ///
                ///<summary>
                ///rc==0 here means that one of the keys ran out of fields and
                ///all the fields up to that point were equal. If the UNPACKED_INCRKEY
                ///flag is set, then break the tie by treating key2 as larger.
                ///If the UPACKED_PREFIX_MATCH flag is set, then keys with common prefixes
                ///are considered to be equal.  Otherwise, the longer key is the
                ///larger.  As it happens, the pPKey2 will always be the longer
                ///if there is a difference.
                ///
                ///</summary>
                Debug.Assert(rc == 0);
                if ((pPKey2.flags & UnpackedRecordFlags.UNPACKED_INCRKEY) != 0)
                {
                    rc = -1;
                }
                else
                    if ((pPKey2.flags & UnpackedRecordFlags.UNPACKED_PREFIX_MATCH) != 0)
                    {
                        ///
                        ///<summary>
                        ///Leave rc==0 
                        ///</summary>
                    }
                    else
                        if (idx1 < szHdr1)
                        {
                            rc = 1;
                        }
                return rc;
            }
            ///<summary>
            /// pCur points at an index entry created using the OP_MakeRecord opcode.
            /// Read the rowid (the last field in the record) and store it in *rowid.
            /// Return SqlResult.SQLITE_OK if everything works, or an error code otherwise.
            ///
            /// pCur might be pointing to text obtained from a corrupt database file.
            /// So the content cannot be trusted.  Do appropriate checks on the content.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeIdxRowid(sqlite3 db, BtCursor pCur, ref i64 rowid)
            {
                i64 nCellKey = 0;
                SqlResult rc;
                u32 szHdr = 0;
                ///
                ///<summary>
                ///Size of the header 
                ///</summary>
                u32 typeRowid = 0;
                ///
                ///<summary>
                ///Serial type of the rowid 
                ///</summary>
                u32 lenRowid;
                ///
                ///<summary>
                ///Size of the rowid 
                ///</summary>
                Mem m = null;
                Mem v = null;
                v = malloc_cs.sqlite3Malloc(v);
                sqliteinth.UNUSED_PARAMETER(db);
                ///
                ///<summary>
                ///Get the size of the index entry.  Only indices entries of less
                ///</summary>
                ///<param name="than 2GiB are support "> anything large must be database corruption.</param>
                ///<param name="Any corruption is detected in sqlite3BtreeParseCellPtr(), though, so">Any corruption is detected in sqlite3BtreeParseCellPtr(), though, so</param>
                ///<param name="this code can safely assume that nCellKey is 32">bits  </param>
                ///<param name=""></param>
                Debug.Assert(pCur.sqlite3BtreeCursorIsValid());
                rc = pCur.sqlite3BtreeKeySize(ref nCellKey);
                Debug.Assert(rc == SqlResult.SQLITE_OK);
                ///
                ///<summary>
                ///pCur is always valid so KeySize cannot fail 
                ///</summary>
                Debug.Assert(((u32)nCellKey & sqliteinth.SQLITE_MAX_U32) == (u64)nCellKey);
                ///
                ///<summary>
                ///Read in the complete content of the index entry 
                ///</summary>
                m = malloc_cs.sqlite3Malloc(m);
                // memset(&m, 0, sizeof(m));
                rc = vdbemem_cs.sqlite3VdbeMemFromBtree(pCur, 0, (int)nCellKey, true, m);
                if (rc != 0)
                {
                    return rc;
                }
                ///
                ///<summary>
                ///The index entry must begin with a header size 
                ///</summary>
                utilc.getVarint32(m.zBLOB, 0, out szHdr);
                sqliteinth.testcase(szHdr == 3);
                sqliteinth.testcase(szHdr == m.n);
                if (sqliteinth.unlikely(szHdr < 3 || (int)szHdr > m.n))
                {
                    goto idx_rowid_corruption;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="The last field of the index should be an integer "> the ROWID.</param>
                ///<param name="Verify that the last entry really is an integer. ">Verify that the last entry really is an integer. </param>
                utilc.getVarint32(m.zBLOB, szHdr - 1, out typeRowid);
                sqliteinth.testcase(typeRowid == 1);
                sqliteinth.testcase(typeRowid == 2);
                sqliteinth.testcase(typeRowid == 3);
                sqliteinth.testcase(typeRowid == 4);
                sqliteinth.testcase(typeRowid == 5);
                sqliteinth.testcase(typeRowid == 6);
                sqliteinth.testcase(typeRowid == 8);
                sqliteinth.testcase(typeRowid == 9);
                if (sqliteinth.unlikely(typeRowid < 1 || typeRowid > 9 || typeRowid == 7))
                {
                    goto idx_rowid_corruption;
                }
                lenRowid = (u32)sqlite3VdbeSerialTypeLen(typeRowid);
                sqliteinth.testcase((u32)m.n == szHdr + lenRowid);
                if (sqliteinth.unlikely((u32)m.n < szHdr + lenRowid))
                {
                    goto idx_rowid_corruption;
                }
                ///
                ///<summary>
                ///Fetch the integer off the end of the index record 
                ///</summary>
                sqlite3VdbeSerialGet(m.zBLOB, (int)(m.n - lenRowid), typeRowid, v);
                rowid = v.u.i;
                m.sqlite3VdbeMemRelease();
                return SqlResult.SQLITE_OK;
            ///
            ///<summary>
            ///Jump here if database corruption is detected after m has been
            ///allocated.  Free the m object and return SQLITE_CORRUPT. 
            ///</summary>
            idx_rowid_corruption:
                //sqliteinth.testcase( m.zMalloc != 0 );
                m.sqlite3VdbeMemRelease();
            return sqliteinth.SQLITE_CORRUPT_BKPT();
            }
            ///<summary>
            /// Compare the key of the index entry that cursor pC is pointing to against
            /// the key string in pUnpacked.  Write into *pRes a number
            /// that is negative, zero, or positive if pC is less than, equal to,
            /// or greater than pUnpacked.  Return SqlResult.SQLITE_OK on success.
            ///
            /// pUnpacked is either created without a rowid or is truncated so that it
            /// omits the rowid at the end.  The rowid at the end of the index entry
            /// is ignored as well.  Hence, this routine only compares the prefixes
            /// of the keys prior to the final rowid, not the entire key.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeIdxKeyCompare(VdbeCursor pC,///
                ///<summary>
                ///The cursor to compare against 
                ///</summary>
            UnpackedRecord pUnpacked,///
                ///<summary>
                ///Unpacked version of key to compare against 
                ///</summary>
            ref int res///
                ///<summary>
                ///Write the comparison result here 
                ///</summary>
            )
            {
                i64 nCellKey = 0;
                SqlResult rc;
                BtCursor pCur = pC.pCursor;
                Mem m = null;
                Debug.Assert(pCur.sqlite3BtreeCursorIsValid());
                rc = pCur.sqlite3BtreeKeySize(ref nCellKey);
                Debug.Assert(rc == SqlResult.SQLITE_OK);
                ///
                ///<summary>
                ///pCur is always valid so KeySize cannot fail 
                ///</summary>
                ///
                ///<summary>
                ///nCellKey will always be between 0 and 0xffffffff because of the say
                ///that btreeParseCellPtr() and sqlite3GetVarint32() are implemented 
                ///</summary>
                if (nCellKey <= 0 || nCellKey > 0x7fffffff)
                {
                    res = 0;
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                m = malloc_cs.sqlite3Malloc(m);
                // memset(&m, 0, sizeof(m));
                rc = vdbemem_cs.sqlite3VdbeMemFromBtree(pC.pCursor, 0, (int)nCellKey, true, m);
                if (rc != 0)
                {
                    return rc;
                }
                Debug.Assert((pUnpacked.flags & UnpackedRecordFlags.UNPACKED_IGNORE_ROWID) != 0);
                res = sqlite3VdbeRecordCompare(m.n, m.zBLOB, pUnpacked);
                m.sqlite3VdbeMemRelease();
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// This routine sets the value to be returned by subsequent calls to
            /// sqlite3_changes() on the database handle 'db'.
            ///
            ///</summary>
            public static void sqlite3VdbeSetChanges(sqlite3 db, int nChange)
            {
                Debug.Assert(db.mutex.sqlite3_mutex_held());
                db.nChange = nChange;
                db.nTotalChange += nChange;
            }
            ///<summary>
            /// Set a flag in the vdbe to update the change counter when it is finalised
            /// or reset.
            ///
            ///</summary>
            ///<summary>
            /// Mark every prepared statement associated with a database connection
            /// as expired.
            ///
            /// An expired statement means that recompilation of the statement is
            /// recommend.  Statements expire when things happen that make their
            /// programs obsolete.  Removing user-defined functions or collating
            /// sequences, or changing an authorization function are the types of
            /// things that make prepared statements obsolete.
            ///
            ///</summary>
            public static void sqlite3ExpirePreparedStatements(sqlite3 db)
            {
                Vdbe p;
                for (p = db.pVdbe; p != null; p = p.pNext)
                {
                    p.expired = true;
                }
            }
            ///<summary>
            /// Return the database associated with the Vdbe.
            ///
            ///</summary>
            ///<summary>
            /// Return a pointer to an sqlite3_value structure containing the value bound
            /// parameter iVar of VM v. Except, if the value is an SQL NULL, return
            /// 0 instead. Unless it is NULL, apply affinity aff (one of the SQLITE_AFF_
            /// constants) to the value before returning it.
            ///
            /// The returned value must be freed by the caller using sqlite3ValueFree().
            ///
            ///</summary>
            ///
            ///<summary>
            ///Configure SQL variable iVar so that binding a new value to it signals
            ///</summary>
            ///<param name="to sqlite3_reoptimize() that re">preparing the statement may result</param>
            ///<param name="in a better query plan.">in a better query plan.</param>
            ///<param name=""></param>
        }
    }
//}