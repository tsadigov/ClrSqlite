using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine.Op
{
    public class VirtualTable
    {


        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        //(Community.CsharpSqlite.Vdbe vdbe, OpCode opcode, ref int opcodeIndex,Mem [] aMem,VdbeOp pOp,ref SqlResult rc)
        {
            var vdbe = cpu.vdbe;
            var aMem = vdbe.aMem;

            switch (opcode)
            {
                
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
                                case OpCode.OP_VBegin:
                                    {
                                        VTable pVTab;
                                        pVTab = pOp.p4.pVtab;
                                        rc = VTableMethodsExtensions.sqlite3VtabBegin(db, pVTab);
                                        if (pVTab != null)
                                            Sqlite3.importVtabErrMsg(this, pVTab.pVtab);
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
                                case OpCode.OP_VCreate:
                                    {
                                        rc = VTableMethodsExtensions.sqlite3VtabCallCreate(db, pOp.p1, pOp.p4.z, ref this.zErrMsg);
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
                                case OpCode.OP_VDestroy:
                                    {
                                        this.inVtabMethod = 2;
                                        rc = VTableMethodsExtensions.sqlite3VtabCallDestroy(db, pOp.p1, pOp.p4.z);
                                        this.inVtabMethod = 0;
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
                                case OpCode.OP_VOpen:
                                    {
                                        VdbeCursor pCur;
                                        sqlite3_vtab_cursor pVtabCursor;
                                        sqlite3_vtab pVtab;
                                        sqlite3_module pModule;
                                        pCur = null;
                                        pVtab = pOp.p4.pVtab.pVtab;
                                        pModule = (sqlite3_module)pVtab.pModule;
                                        Debug.Assert(pVtab != null && pModule != null);
                                        rc = pModule.xOpen(pVtab, out pVtabCursor);
                                        Sqlite3.importVtabErrMsg(this, pVtab);
                                        if (SqlResult.SQLITE_OK == rc)
                                        {
                                            ///
                                            ///<summary>
                                            ///Initialize sqlite3_vtab_cursor base class 
                                            ///</summary>
                                            pVtabCursor.pVtab = pVtab;
                                            ///
                                            ///<summary>
                                            ///Initialise vdbe cursor object 
                                            ///</summary>
                                            pCur = Sqlite3.allocateCursor(this, pOp.p1, 0, -1, 0);
                                            if (pCur != null)
                                            {
                                                pCur.pVtabCursor = pVtabCursor;
                                                pCur.pModule = pVtabCursor.pVtab.pModule;
                                            }
                                            else
                                            {
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
                                case OpCode.OP_VFilter:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump 
                                        ///</summary>
                                        int nArg;
                                        int iQuery;
                                        sqlite3_module pModule;
                                        Mem pQuery;
                                        Mem pArgc = null;
                                        sqlite3_vtab_cursor pVtabCursor;
                                        sqlite3_vtab pVtab;
                                        VdbeCursor pCur;
                                        int res;
                                        int i;
                                        Mem[] apArg;
                                        pQuery = aMem[pOp.p3];
                                        pArgc = aMem[pOp.p3 + 1];
                                        // pQuery[1];
                                        pCur = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pQuery.memIsValid());
                                        Sqlite3.REGISTER_TRACE(this, pOp.p3, pQuery);
                                        Debug.Assert(pCur.pVtabCursor != null);
                                        pVtabCursor = pCur.pVtabCursor;
                                        pVtab = pVtabCursor.pVtab;
                                        pModule = pVtab.pModule;
                                        ///
                                        ///<summary>
                                        ///Grab the index number and argc parameters 
                                        ///</summary>
                                        Debug.Assert((pQuery.flags & MemFlags.MEM_Int) != 0 && pArgc.flags == MemFlags.MEM_Int);
                                        nArg = (int)pArgc.u.i;
                                        iQuery = (int)pQuery.u.i;
                                        ///
                                        ///<summary>
                                        ///Invoke the xFilter method 
                                        ///</summary>
                                        {
                                            res = 0;
                                            apArg = this.apArg;
                                            for (i = 0; i < nArg; i++)
                                            {
                                                apArg[i] = aMem[(pOp.p3 + 1) + i + 1];
                                                //apArg[i] = pArgc[i + 1];
                                                Sqlite3.sqlite3VdbeMemStoreType(apArg[i]);
                                            }
                                            this.inVtabMethod = 1;
                                            rc = pModule.xFilter(pVtabCursor, iQuery, pOp.p4.z, nArg, apArg);
                                            this.inVtabMethod = 0;
                                            Sqlite3.importVtabErrMsg(this, pVtab);
                                            if (rc == SqlResult.SQLITE_OK)
                                            {
                                                res = pModule.xEof(pVtabCursor);
                                            }
                                            if (res != 0)
                                            {
                                                opcodeIndex = pOp.p2 - 1;
                                            }
                                        }
                                        pCur.nullRow = false;
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
                                case OpCode.OP_VColumn:
                                    {
                                        sqlite3_vtab pVtab;
                                        sqlite3_module pModule;
                                        Mem pDest;
                                        sqlite3_context sContext;
                                        VdbeCursor pCur = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pCur.pVtabCursor != null);
                                        Debug.Assert(pOp.p3 > 0 && pOp.p3 <= this.nMem);
                                        pDest = aMem[pOp.p3];
                                        this.memAboutToChange(pDest);
                                        if (pCur.nullRow)
                                        {
                                            pDest.sqlite3VdbeMemSetNull();
                                            break;
                                        }
                                        pVtab = pCur.pVtabCursor.pVtab;
                                        pModule = pVtab.pModule;
                                        Debug.Assert(pModule.xColumn != null);
                                        sContext = new sqlite3_context();
                                        //memset( &sContext, 0, sizeof( sContext ) );
                                        ///
                                        ///<summary>
                                        ///The output cell may already have a buffer allocated. Move
                                        ///</summary>
                                        ///<param name="the current contents to sContext.s so in case the user">function</param>
                                        ///<param name="can use the already allocated buffer instead of allocating a">can use the already allocated buffer instead of allocating a</param>
                                        ///<param name="new one.">new one.</param>
                                        ///<param name=""></param>
                                        vdbemem_cs.sqlite3VdbeMemMove(sContext.s, pDest);
                                        sContext.s.MemSetTypeFlag(MemFlags.MEM_Null);
                                        rc = pModule.xColumn(pCur.pVtabCursor, sContext, pOp.p2);
                                        Sqlite3.importVtabErrMsg(this, pVtab);
                                        if (sContext.isError != 0)
                                        {
                                            rc = sContext.isError;
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
                                        Sqlite3.REGISTER_TRACE(this, pOp.p3, pDest);
                                        Sqlite3.UPDATE_MAX_BLOBSIZE(pDest);
                                        if (pDest.IsTooBig())
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
                                case OpCode.OP_VNext:
                                    {
                                        ///
                                        ///<summary>
                                        ///jump 
                                        ///</summary>
                                        sqlite3_vtab pVtab;
                                        sqlite3_module pModule;
                                        int res;
                                        VdbeCursor pCur;
                                        res = 0;
                                        pCur = this.OpenCursors[pOp.p1];
                                        Debug.Assert(pCur.pVtabCursor != null);
                                        if (pCur.nullRow)
                                        {
                                            break;
                                        }
                                        pVtab = pCur.pVtabCursor.pVtab;
                                        pModule = pVtab.pModule;
                                        Debug.Assert(pModule.xNext != null);
                                        ///
                                        ///<summary>
                                        ///Invoke the xNext() method of the module. There is no way for the
                                        ///underlying implementation to return an error if one occurs during
                                        ///xNext(). Instead, if an error occurs, true is returned (indicating that
                                        ///data is available) and the error code returned when xColumn or
                                        ///some other method is next invoked on the save virtual table cursor.
                                        ///
                                        ///</summary>
                                        this.inVtabMethod = 1;
                                        rc = pModule.xNext(pCur.pVtabCursor);
                                        this.inVtabMethod = 0;
                                        Sqlite3.importVtabErrMsg(this, pVtab);
                                        if (rc == SqlResult.SQLITE_OK)
                                        {
                                            res = pModule.xEof(pCur.pVtabCursor);
                                        }
                                        if (0 == res)
                                        {
                                            ///
                                            ///<summary>
                                            ///If there is data, jump to P2 
                                            ///</summary>
                                            opcodeIndex = pOp.p2 - 1;
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
                                case OpCode.OP_VRename:
                                    {
                                        sqlite3_vtab pVtab;
                                        Mem pName;
                                        pVtab = pOp.p4.pVtab.pVtab;
                                        pName = aMem[pOp.p1];
                                        Debug.Assert(pVtab.pModule.xRename != null);
                                        Debug.Assert(pName.memIsValid());
                                        Sqlite3.REGISTER_TRACE(this, pOp.p1, pName);
                                        Debug.Assert((pName.flags & MemFlags.MEM_Str) != 0);
                                        rc = pVtab.pModule.xRename(pVtab, pName.z);
                                        Sqlite3.importVtabErrMsg(this, pVtab);
                                        this.expired = false;
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
                                case OpCode.OP_VUpdate:
                                    {
                                        sqlite3_vtab pVtab;
                                        sqlite3_module pModule;
                                        int nArg;
                                        int i;
                                        sqlite_int64 rowid = 0;
                                        Mem[] apArg;
                                        Mem pX;
                                        Debug.Assert(pOp.p2 == 1 ||
                                            ((OnConstraintError)pOp.p5)
                                            .In(OnConstraintError.OE_Fail
                                                , OnConstraintError.OE_Rollback
                                                , OnConstraintError.OE_Abort
                                                , OnConstraintError.OE_Ignore
                                                , OnConstraintError.OE_Replace));

                                        pVtab = pOp.p4.pVtab.pVtab;
                                        pModule = (sqlite3_module)pVtab.pModule;
                                        nArg = pOp.p2;
                                        Debug.Assert(pOp.p4type == P4Usage.P4_VTAB);
                                        if (Sqlite3.ALWAYS(pModule.xUpdate))
                                        {
                                            u8 vtabOnConflict = db.vtabOnConflict;
                                            apArg = this.apArg;
                                            //pX = aMem[pOp.p3];
                                            for (i = 0; i < nArg; i++)
                                            {
                                                pX = aMem[pOp.p3 + i];
                                                Debug.Assert(pX.memIsValid());
                                                this.memAboutToChange(pX);
                                                Sqlite3.sqlite3VdbeMemStoreType(pX);
                                                apArg[i] = pX;
                                                //pX++;
                                            }
                                            db.vtabOnConflict = pOp.p5;
                                            rc = pModule.xUpdate(pVtab, nArg, apArg, out rowid);
                                            db.vtabOnConflict = vtabOnConflict;
                                            Sqlite3.importVtabErrMsg(this, pVtab);
                                            if (rc == SqlResult.SQLITE_OK && pOp.p1 != 0)
                                            {
                                                Debug.Assert(nArg > 1 && apArg[0] != null && (apArg[0].flags & MemFlags.MEM_Null) != 0);
                                                db.lastRowid = lastRowid = rowid;
                                            }
                                            if (rc == SqlResult.SQLITE_CONSTRAINT && pOp.p4.pVtab.bConstraint != 0)
                                            {
                                                if ((OnConstraintError)pOp.p5 == OnConstraintError.OE_Ignore)
                                                {
                                                    rc = SqlResult.SQLITE_OK;
                                                }
                                                else
                                                {
                                                    this.errorAction = ((OnConstraintError)pOp.p5).Filter(OnConstraintError.OE_Replace, OnConstraintError.OE_Abort);
                                                }
                                            }
                                            else
                                            {
                                                this.nChange++;
                                            }
                                        }
                                        break;
                                    }
#endif
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
