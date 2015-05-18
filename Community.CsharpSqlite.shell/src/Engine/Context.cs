using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;



namespace Community.CsharpSqlite
{
    using Vdbe = Community.CsharpSqlite.Sqlite3.Vdbe;
    using sqlite3_value = Mem;
    
    using Parse = Community.CsharpSqlite.Sqlite3.Parse;
    using Community.CsharpSqlite.Metadata;


    ///<summary>
    /// The "context" argument for a installable function.  A pointer to an
    /// instance of this structure is the first argument to the routines used
    /// implement the SQL functions.
    ///
    /// There is a typedef for this structure in sqlite.h.  So all routines,
    /// even the public interface to SQLite, can use a pointer to this structure.
    /// But this file is the only place where the internal details of this
    /// structure are known.
    ///
    /// This structure is defined inside of vdbeInt.h because it uses substructures
    /// (Mem) which are only defined there.
    ///
    ///</summary>
    public class sqlite3_context
    {
        ///
        ///<summary>
        ///Pointer to function information.  MUST BE FIRST 
        ///</summary>

        public FuncDef pFunc;

        ///
        ///<summary>
        ///Auxilary data, if created. 
        ///</summary>
        
        public VdbeFunc pVdbeFunc;

        ///
        ///<summary>
        ///The return value is stored here 
        ///</summary>

        public Mem s = new Mem();

        ///<summary>
        ///Memory cell used to store aggregate context 
        ///</summary>


        public Mem pMem;

        ///<summary>
        ///Error code returned by the function. 
        ///</summary>
        public SqlResult isError;


        ///<summary>
        ///Collating sequence 
        ///</summary>
        public CollSeq pColl;


        public///<summary>
            /// sqlite3_result_  
            /// The following routines are used by user-defined functions to specify
            /// the function result.
            ///
            /// The setStrOrError() funtion calls sqlite3VdbeMemSetStr() to store the
            /// result as a string or blob but if the string or blob is too large, it
            /// then sets the error code to SQLITE_TOOBIG
            ///
            ///</summary>
        void setResultStrOrError(///
            ///<summary>
            ///Function context 
            ///</summary>

        string z, ///
            ///<summary>
            ///String pointer 
            ///</summary>

        int o, ///
            ///<summary>
            ///offset into string 
            ///</summary>

        int n, ///
            ///<summary>
            ///Bytes in string, or negative 
            ///</summary>

        SqliteEncoding enc, ///
            ///<summary>
            ///Encoding of z.  0 for BLOBs 
            ///</summary>

        dxDel xDel//void (*xDel)(void)     /* Destructor function */
        )
        {
            if (this.s.sqlite3VdbeMemSetStr(z, o, n, enc, xDel) == SqlResult.SQLITE_TOOBIG)
            {
                this.sqlite3_result_error_toobig();
            }
        }

        public void setResultStrOrError(///
            ///<summary>
            ///Function context 
            ///</summary>

        string z, ///
            ///<summary>
            ///String pointer 
            ///</summary>

        int n, ///
            ///<summary>
            ///Bytes in string, or negative 
            ///</summary>

        SqliteEncoding enc, ///
            ///<summary>
            ///Encoding of z.  0 for BLOBs 
            ///</summary>

        dxDel xDel//void (*xDel)(void)     /* Destructor function */
        )
        {
            if (this.s.sqlite3VdbeMemSetStr(z, n, enc, xDel) == SqlResult.SQLITE_TOOBIG)
            {
                this.sqlite3_result_error_toobig();
            }
        }

        public void sqlite3_result_blob(string z, int n, dxDel xDel)
        {
            Debug.Assert(n >= 0);
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.setResultStrOrError(z, n, 0, xDel);
        }

        public void sqlite3_result_double(double rVal)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetDouble(rVal);
        }

        public void sqlite3_result_error(string z, int n)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.setResultStrOrError(z, n, SqliteEncoding.UTF8, Sqlite3.SQLITE_TRANSIENT);
            this.isError = SqlResult.SQLITE_ERROR;
        }

        public void sqlite3_result_int(int iVal)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetInt64((i64)iVal);
        }

        public void sqlite3_result_int64(i64 iVal)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetInt64(iVal);
        }

        public void sqlite3_result_null()
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetNull();
        }

        public void sqlite3_result_text(string z, int o, //Offset
        int n, dxDel xDel)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.setResultStrOrError(z, o, n, SqliteEncoding.UTF8, xDel);
        }

        public void sqlite3_result_text(StringBuilder z, int n, dxDel xDel)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.setResultStrOrError(z.ToString(), n, SqliteEncoding.UTF8, xDel);
        }

        public void sqlite3_result_text(string z, int n, dxDel xDel)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.setResultStrOrError(z, n, SqliteEncoding.UTF8, xDel);
        }

        public void sqlite3_result_value(sqlite3_value pValue)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            vdbemem_cs.sqlite3VdbeMemCopy(this.s, pValue);
        }

        public void sqlite3_result_zeroblob(int n)
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetZeroBlob(n);
        }

        public void sqlite3_result_error_code(SqlResult errCode)
        {
            this.isError = errCode;
            if ((this.s.flags & MemFlags.MEM_Null) != 0)
            {
                this.setResultStrOrError(Sqlite3.sqlite3ErrStr(errCode), -1, SqliteEncoding.UTF8, Sqlite3.SQLITE_STATIC);
            }
        }

        ///<summary>
        ///Force an SQLITE_TOOBIG error.
        ///</summary>
        public void sqlite3_result_error_toobig()
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.isError = SqlResult.SQLITE_ERROR;
            this.setResultStrOrError("string or blob too big", -1, SqliteEncoding.UTF8, Sqlite3.SQLITE_STATIC);
        }

        ///<summary>
        ///An SQLITE_NOMEM error.
        ///</summary>
        public void sqlite3_result_error_nomem()
        {
            Debug.Assert(this.s.db.mutex.sqlite3_mutex_held());
            this.s.sqlite3VdbeMemSetNull();
            this.isError = SqlResult.SQLITE_NOMEM;
            //pCtx.s.db.mallocFailed = 1;
        }
    }

}
