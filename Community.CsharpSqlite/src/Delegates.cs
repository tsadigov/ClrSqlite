///
///<summary>
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>

using System;
using System.Text;
using HANDLE = System.IntPtr;
using i16 = System.Int16;
using sqlite3_int64 = System.Int64;
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{
    using DbPage = Cache.PgHdr;
    using sqlite3_pcache = Cache.PCache1;
    using sqlite3_stmt = Engine.Vdbe;
    using sqlite3_value = Engine.Mem;
    using codec_ctx = crypto.codec_ctx;
    using sqlite3_api_routines = Sqlite3ExtensionModule.sqlite3_api_routines;
    using Community.CsharpSqlite.Ast;
    using Metadata;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Paging;
    using Cache;
    using Compiler.Parser;




    public delegate void dxAuth(object pAuthArg, int b, string c, string d, string e, string f);

    public delegate int dxBusy(object pBtShared, int iValue);

    public delegate void dxFreeAux(object pAuxArg);
    
    public delegate int dxCallback<T>(T pCallbackArg, sqlite3_int64 argc, object p2, object p3);

    public delegate void dxalarmCallback(object pNotUsed, sqlite3_int64 iNotUsed, int size);

    public delegate void dxCollNeeded(object pCollNeededArg, Connection db, int eTextRep, string collationName);

    public delegate SqlResult dxCommitCallback(object pCommitArg);

    public delegate int dxCompare(object pCompareArg, int size1, string Key1, int size2, string Key2);

    public delegate bool dxCompare4(string Key1, int size1, string Key2, int size2);

    public delegate void dxDel(ref string pDelArg);





    //Mem Methods
    public delegate SqlResult dxMemInit(object o);

    public delegate void dxMemShutdown(object o);

    public delegate byte[] dxMalloc(int nSize);

    public delegate int[] dxMallocInt(int nSize);

    public delegate Mem dxMallocMem(Mem pMem);

    public delegate void dxFree(ref byte[] pOld);

    public delegate void dxFreeInt(ref int[] pOld);

    public delegate void dxFreeMem(ref Mem pOld);

    public delegate byte[] dxRealloc(byte[] pOld, int nSize);

    public delegate int dxSize(byte[] pArray);

    public delegate int dxRoundup(int nSize);

		















    //sqlite3_module
    public delegate SqlResult smdxCreateConnect(Connection db, object pAux, int argc, string[] constargv, out sqlite3_vtab ppVTab, out string pError);

    public delegate int smdxBestIndex(sqlite3_vtab pVTab, ref sqlite3_index_info pIndex);

    public delegate int smdxDisconnect(ref object pVTab);

    public delegate SqlResult smdxDestroy(ref object pVTab);

    public delegate SqlResult smdxOpen(sqlite3_vtab pVTab, out sqlite3_vtab_cursor ppCursor);

    public delegate int smdxClose(ref sqlite3_vtab_cursor pCursor);

    public delegate SqlResult smdxFilter(sqlite3_vtab_cursor pCursor, int idxNum, string idxStr, int argc, sqlite3_value[] argv);

    public delegate SqlResult smdxNext(sqlite3_vtab_cursor pCursor);

    public delegate int smdxEof(sqlite3_vtab_cursor pCursor);

    public delegate SqlResult smdxColumn(sqlite3_vtab_cursor pCursor, sqlite3_context p2, int p3);

    public delegate SqlResult smdxRowid(sqlite3_vtab_cursor pCursor, out sqlite3_int64 pRowid);

    public delegate SqlResult smdxUpdate(sqlite3_vtab pVTab, int p1, sqlite3_value[] p2, out sqlite3_int64 p3);

    public delegate SqlResult smdxFunction(sqlite3_vtab pVTab);

    public delegate int smdxFindFunction(sqlite3_vtab pVtab, int nArg, string zName, ref dxFunc pxFunc, ref object ppArg);

    public delegate SqlResult smdxRename(sqlite3_vtab pVtab, string zNew);

    public delegate SqlResult smdxFunctionArg(sqlite3_vtab pVTab, int nArg);






























    // needs ref
    public delegate void dxDelCollSeq(ref object pDelArg);

    // needs ref
    public delegate void dxLog(object pLogArg, int i, string msg);

    public delegate void dxLogcallback(object pCallbackArg, int argc, string p2);

    public delegate void dxProfile(object pProfileArg, string msg, sqlite3_int64 time);

    public delegate int dxProgress(object pProgressArg);

    public delegate void dxRollbackCallback(object pRollbackArg);

    public delegate void dxTrace(object pTraceArg, string msg);

    public delegate void dxUpdateCallback(object pUpdateArg, AuthTarget b, string c, string d, sqlite3_int64 e);

    public delegate int dxWalCallback(object pWalArg, Connection db, string zDb, int nEntry);













		
		

		//
		public delegate string dxColname (sqlite3_value pVal);

		public delegate int dxFuncBtree (Tree.Btree p);

		public delegate int dxExprTreeFunction (ref int pArg, Expr pExpr);

		public delegate int dxExprTreeFunction_NC (NameContext pArg, ref Expr pExpr);

		public delegate int dxExprTreeFunction_OBJ (object pArg, Expr pExpr);

		

		///
///<summary>
///Pager Delegates
///
///</summary>

		public delegate void dxDestructor (DbPage dbPage);

		///
///<summary>
///Call this routine when freeing pages 
///</summary>

		public delegate int dxBusyHandler (object pBusyHandlerArg);

		public delegate void dxReiniter (DbPage dbPage);

		///
///<summary>
///Call this routine when reloading pages 
///</summary>

		public delegate void dxFreeSchema (Schema schema);

		#if SQLITE_HAS_CODEC
		public delegate byte[] dxCodec (codec_ctx pCodec, byte[] D, uint pageNumber, int X);

		//void *(*xCodec)(void*,void*,Pgno,int); /* Routine for en/decoding data */
		public delegate void dxCodecSizeChng (codec_ctx pCodec, int pageSize, i16 nReserve);

		//void (*xCodecSizeChng)(void*,int,int); /* Notify of page size changes */
		public delegate void dxCodecFree (ref codec_ctx pCodec);

		//void (*xCodecFree)(void); /* Destructor for the codec */
		#endif
		//Module
		public delegate void dxDestroy (ref PgHdr pDestroyArg);

		public delegate SqlResult dxStress (object obj, PgHdr pPhHdr);

		
		//AutoExtention
		public delegate int dxInit (Connection db, ref string zMessage, sqlite3_api_routines sar);

		
		//Faults
		public delegate void void_function ();

		

        //MUTEX delegates where here



		public delegate object dxColumn (sqlite3_stmt pStmt, int i);

		public delegate int dxColumn_I (sqlite3_stmt pStmt, int i);

		// Walker Methods
		public delegate WRC dxExprCallback (Walker W, Expr E);

		///
///<summary>
///Callback for expressions 
///</summary>

		public delegate WRC dxSelectCallback (Walker W, Select S);

		///
///<summary>
///Callback for SELECTs 
///</summary>

  

        public partial class Sqlite3
        {
	#if NET_35 || NET_40
																			    //API Simplifications -- Actions
    public static Action<sqlite3_context, String, Int32, dxDel> ResultBlob = sqlite3_result_blob;
    public static Action<sqlite3_context, Double> ResultDouble = sqlite3_result_double;
    public static Action<sqlite3_context, String, Int32> ResultError = sqlite3_result_error;
    public static Action<sqlite3_context, Int32> ResultErrorCode = sqlite3_result_error_code;
    public static Action<sqlite3_context> ResultErrorNoMem = sqlite3_result_error_nomem;
    public static Action<sqlite3_context> ResultErrorTooBig = sqlite3_result_error_toobig;
    public static Action<sqlite3_context, Int32> ResultInt = sqlite3_result_int;
    public static Action<sqlite3_context, Int64> ResultInt64 = sqlite3_result_int64;
    public static Action<sqlite3_context> ResultNull = sqlite3_result_null;
    public static Action<sqlite3_context, String, Int32, dxDel> ResultText = sqlite3_result_text;
    public static Action<sqlite3_context, String, Int32, Int32, dxDel> ResultText_Offset = sqlite3_result_text;
    public static Action<sqlite3_context, sqlite3_value> ResultValue = sqlite3_result_value;
    public static Action<sqlite3_context, Int32> ResultZeroblob = sqlite3_result_zeroblob;
    public static Action<sqlite3_context, Int32, String> SetAuxdata = sqlite3_set_auxdata;

    //API Simplifications -- Functions
    public delegate Int32 FinalizeDelegate( sqlite3_stmt pStmt );
    public static FinalizeDelegate Finalize = sqlite3_finalize;

    public static Func<sqlite3_stmt, Int32> ClearBindings = sqlite3_clear_bindings;
    public static Func<sqlite3_stmt, Int32, Byte[]> ColumnBlob = sqlite3_column_blob;
    public static Func<sqlite3_stmt, Int32, Int32> ColumnBytes = sqlite3_column_bytes;
    public static Func<sqlite3_stmt, Int32, Int32> ColumnBytes16 = sqlite3_column_bytes16;
    public static Func<sqlite3_stmt, Int32> ColumnCount = vdbeapi.sqlite3_column_count;
    public static Func<sqlite3_stmt, Int32, String> ColumnDecltype = sqlite3_column_decltype;
    public static Func<sqlite3_stmt, Int32, Double> ColumnDouble = sqlite3_column_double;
    public static Func<sqlite3_stmt, Int32, Int32> ColumnInt = sqlite3_column_int;
    public static Func<sqlite3_stmt, Int32, Int64> ColumnInt64 = sqlite3_column_int64;
    public static Func<sqlite3_stmt, Int32, String> ColumnName = vdbeapi.sqlite3_column_name;
    public static Func<sqlite3_stmt, Int32, String> ColumnText = sqlite3_column_text;
    public static Func<sqlite3_stmt, Int32, Int32> ColumnType = sqlite3_column_type;
    public static Func<sqlite3_stmt, Int32, sqlite3_value> ColumnValue = sqlite3_column_value;
    public static Func<sqlite3_stmt, Int32> DataCount = sqlite3_data_count;
    public static Func<sqlite3_stmt, Int32> Reset = sqlite3_reset;
    public static Func<sqlite3_stmt, Int32> Step = sqlite3_step;

    public static Func<sqlite3_stmt, Int32, Byte[], Int32, dxDel, Int32> BindBlob = sqlite3_bind_blob;
    public static Func<sqlite3_stmt, Int32, Double, Int32> BindDouble = sqlite3_bind_double;
    public static Func<sqlite3_stmt, Int32, Int32, Int32> BindInt = sqlite3_bind_int;
    public static Func<sqlite3_stmt, Int32, Int64, Int32> BindInt64 = sqlite3_bind_int64;
    public static Func<sqlite3_stmt, Int32, Int32> BindNull = sqlite3_bind_null;
    public static Func<sqlite3_stmt, Int32> BindParameterCount = sqlite3_bind_parameter_count;
    public static Func<sqlite3_stmt, String, Int32> BindParameterIndex = sqlite3_bind_parameter_index;
    public static Func<sqlite3_stmt, Int32, String> BindParameterName = sqlite3_bind_parameter_name;
    public static Func<sqlite3_stmt, Int32, String, Int32, dxDel, Int32> BindText = sqlite3_bind_text;
    public static Func<sqlite3_stmt, Int32, sqlite3_value, Int32> BindValue = sqlite3_bind_value;
    public static Func<sqlite3_stmt, Int32, Int32, Int32> BindZeroblob = sqlite3_bind_zeroblob;

    public delegate Int32 OpenDelegate( string zFilename, out sqlite3 ppDb );
    public static Func<sqlite3, Int32> Close = sqlite3_close;
    public static Func<sqlite3_stmt, sqlite3> DbHandle = sqlite3_db_handle;
    public static Func<sqlite3, String> Errmsg = sqlite3_errmsg;
    public static OpenDelegate Open = sqlite3_open;
    public static Func<sqlite3, sqlite3_stmt, sqlite3_stmt> NextStmt = sqlite3_next_stmt;
    public static Func<Int32> Shutdown = sqlite3_shutdown;
    public static Func<sqlite3_stmt, Int32, Int32, Int32> StmtStatus = sqlite3_stmt_status;

    public delegate Int32 PrepareDelegate( sqlite3 db, String zSql, Int32 nBytes, ref sqlite3_stmt ppStmt, ref string pzTail );
    public delegate Int32 PrepareDelegateNoTail( sqlite3 db, String zSql, Int32 nBytes, ref sqlite3_stmt ppStmt, Int32 iDummy );
    public static PrepareDelegate Prepare = sqlite3_prepare;
    public static PrepareDelegate PrepareV2 = sqlite3_prepare_v2;
    public static PrepareDelegateNoTail PrepareV2NoTail = sqlite3_prepare_v2;

    public static Func<sqlite3_context, Int32, Mem> AggregateContext = vdbeapi.sqlite3_aggregate_context;
    public static Func<sqlite3_context, Int32, Object> GetAuxdata = sqlite3_get_auxdata;
    public static Func<sqlite3_context, sqlite3> ContextDbHandle = vdbeapi.sqlite3_context_db_handle;
    public static Func<sqlite3_context, Object> UserData = sqlite3_user_data;

    public static Func<sqlite3_value, Byte[]> ValueBlob = sqlite3_value_blob;
    public static Func<sqlite3_value, Int32> ValueBytes = vdbeapi.sqlite3_value_bytes;
    public static Func<sqlite3_value, Int32> ValueBytes16 = vdbeapi.sqlite3_value_bytes16;
    public static Func<sqlite3_value, Double> ValueDouble = sqlite3_value_double;
    public static Func<sqlite3_value, Int32> ValueInt = sqlite3_value_int;
    public static Func<sqlite3_value, Int64> ValueInt64 = sqlite3_value_int64;
    public static Func<sqlite3_value, String> ValueText = vdbeapi.sqlite3_value_text;
    public static Func<sqlite3_value, Int32> ValueType = vdbeapi.sqlite3_value_type;
#endif
	}
}
#if ( NET_35 && !NET_40) || WINDOWS_PHONE
namespace System
{
  // Summary:
  //     Encapsulates a method that has four parameters and does not return a value.
  //
  // Parameters:
  //   arg1:
  //     The first parameter of the method that this delegate encapsulates.
  //
  //   arg2:
  //     The second parameter of the method that this delegate encapsulates.
  //
  //   arg3:
  //     The third parameter of the method that this delegate encapsulates.
  //
  //   arg4:
  //     The fourth parameter of the method that this delegate encapsulates.
  //
  //   arg5:
  //     The fifth parameter of the method that this delegate encapsulates.
  //
  // Type parameters:
  //   T1:
  //     The type of the first parameter of the method that this delegate encapsulates.
  //
  //   T2:
  //     The type of the second parameter of the method that this delegate encapsulates.
  //
  //   T3:
  //     The type of the third parameter of the method that this delegate encapsulates.
  //
  //   T4:
  //     The type of the fourth parameter of the method that this delegate encapsulates.
  //
  //   T5:
  //     The type of the fifth parameter of the method that this delegate encapsulates.
  public delegate void Action<T1, T2, T3, T4, T5>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 );

  // Summary:
  //     Encapsulates a method that has three parameters and returns a value of the
  //     type specified by the TResult parameter.
  //
  // Parameters:
  //   arg1:
  //     The first parameter of the method that this delegate encapsulates.
  //
  //   arg2:
  //     The second parameter of the method that this delegate encapsulates.
  //
  //   arg3:
  //     The third parameter of the method that this delegate encapsulates.
  //
  //   arg4:
  //     The fourth parameter of the method that this delegate encapsulates.
  //
  //   arg5:
  //     The fifth parameter of the method that this delegate encapsulates.
  //
  // Type parameters:
  //   T1:
  //     The type of the first parameter of the method that this delegate encapsulates.
  //
  //   T2:
  //     The type of the second parameter of the method that this delegate encapsulates.
  //
  //   T3:
  //     The type of the third parameter of the method that this delegate encapsulates.
  //
  //   T4:
  //     The type of the fourth parameter of the method that this delegate encapsulates.
  //
  //   T5:
  //     The type of the fifth parameter of the method that this delegate encapsulates.
  //
  //   TResult:
  //     The type of the return value of the method that this delegate encapsulates.
  //
  // Returns:
  //     The return value of the method that this delegate encapsulates.
  public delegate TResult Func<T1, T2, T3, T4, TResult>( T1 arg1, T2 arg2, T3 arg3, T4 arg4 );
  public delegate TResult Func<T1, T2, T3, T4, T5, TResult>( T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5 );
}
#endif
