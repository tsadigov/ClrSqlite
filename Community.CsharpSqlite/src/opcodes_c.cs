///
///<summary>
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
///<param name=""></param>
///<param name=""></param>

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///
///<summary>
///Automatically generated.  Do not edit 
///</summary>

		///<summary>
		///See the mkopcodec.awk script for details.
		///</summary>
		#if !SQLITE_OMIT_EXPLAIN || !NDEBUG || VDBE_PROFILE || SQLITE_DEBUG
		public static string sqlite3OpcodeName (OpCode opcode)
		{
			return sqlite3OpcodeName ((int)opcode);
		}

		public static string sqlite3OpcodeName (int i)
		{
			string[] azName =  {
				"?",
				///
///<summary>
///1 
///</summary>

				"Goto",
				///
///<summary>
///2 
///</summary>

				"Gosub",
				///
///<summary>
///3 
///</summary>

				"Return",
				///
///<summary>
///4 
///</summary>

				"Yield",
				///
///<summary>
///5 
///</summary>

				"HaltIfNull",
				///
///<summary>
///6 
///</summary>

				"Halt",
				///
///<summary>
///7 
///</summary>

				"Integer",
				///
///<summary>
///8 
///</summary>

				"Int64",
				///
///<summary>
///9 
///</summary>

				"String",
				///
///<summary>
///10 
///</summary>

				"Null",
				///
///<summary>
///11 
///</summary>

				"Blob",
				///
///<summary>
///12 
///</summary>

				"Variable",
				///
///<summary>
///13 
///</summary>

				"Move",
				///
///<summary>
///14 
///</summary>

				"Copy",
				///
///<summary>
///15 
///</summary>

				"SCopy",
				///
///<summary>
///16 
///</summary>

				"ResultRow",
				///
///<summary>
///17 
///</summary>

				"CollSeq",
				///
///<summary>
///18 
///</summary>

				"Function",
				///
///<summary>
///19 
///</summary>

				"Not",
				///
///<summary>
///20 
///</summary>

				"AddImm",
				///
///<summary>
///21 
///</summary>

				"MustBeInt",
				///
///<summary>
///22 
///</summary>

				"RealAffinity",
				///
///<summary>
///23 
///</summary>

				"Permutation",
				///
///<summary>
///24 
///</summary>

				"Compare",
				///
///<summary>
///25 
///</summary>

				"Jump",
				///
///<summary>
///26 
///</summary>

				"If",
				///
///<summary>
///27 
///</summary>

				"IfNot",
				///
///<summary>
///28 
///</summary>

				"Column",
				///
///<summary>
///29 
///</summary>

				"Affinity",
				///
///<summary>
///30 
///</summary>

				"MakeRecord",
				///
///<summary>
///31 
///</summary>

				"Count",
				///
///<summary>
///32 
///</summary>

				"Savepoint",
				///
///<summary>
///33 
///</summary>

				"AutoCommit",
				///
///<summary>
///34 
///</summary>

				"Transaction",
				///
///<summary>
///35 
///</summary>

				"ReadCookie",
				///
///<summary>
///36 
///</summary>

				"SetCookie",
				///
///<summary>
///37 
///</summary>

				"VerifyCookie",
				///
///<summary>
///38 
///</summary>

				"OpenRead",
				///
///<summary>
///39 
///</summary>

				"OpenWrite",
				///
///<summary>
///40 
///</summary>

				"OpenAutoindex",
				///
///<summary>
///41 
///</summary>

				"OpenEphemeral",
				///
///<summary>
///42 
///</summary>

				"OpenPseudo",
				///
///<summary>
///43 
///</summary>

				"Close",
				///
///<summary>
///44 
///</summary>

				"SeekLt",
				///
///<summary>
///45 
///</summary>

				"SeekLe",
				///
///<summary>
///46 
///</summary>

				"SeekGe",
				///
///<summary>
///47 
///</summary>

				"SeekGt",
				///
///<summary>
///48 
///</summary>

				"Seek",
				///
///<summary>
///49 
///</summary>

				"NotFound",
				///
///<summary>
///50 
///</summary>

				"Found",
				///
///<summary>
///51 
///</summary>

				"IsUnique",
				///
///<summary>
///52 
///</summary>

				"NotExists",
				///
///<summary>
///53 
///</summary>

				"Sequence",
				///
///<summary>
///54 
///</summary>

				"NewRowid",
				///
///<summary>
///55 
///</summary>

				"Insert",
				///
///<summary>
///56 
///</summary>

				"InsertInt",
				///
///<summary>
///57 
///</summary>

				"Delete",
				///
///<summary>
///58 
///</summary>

				"ResetCount",
				///
///<summary>
///59 
///</summary>

				"RowKey",
				///
///<summary>
///60 
///</summary>

				"RowData",
				///
///<summary>
///61 
///</summary>

				"Rowid",
				///
///<summary>
///62 
///</summary>

				"NullRow",
				///
///<summary>
///63 
///</summary>

				"Last",
				///
///<summary>
///64 
///</summary>

				"Sort",
				///
///<summary>
///65 
///</summary>

				"Rewind",
				///
///<summary>
///66 
///</summary>

				"Prev",
				///
///<summary>
///67 
///</summary>

				"Next",
				///
///<summary>
///68 
///</summary>

				"Or",
				///
///<summary>
///69 
///</summary>

				"And",
				///
///<summary>
///70 
///</summary>

				"IdxInsert",
				///
///<summary>
///71 
///</summary>

				"IdxDelete",
				///
///<summary>
///72 
///</summary>

				"IdxRowid",
				///
///<summary>
///73 
///</summary>

				"IsNull",
				///
///<summary>
///74 
///</summary>

				"NotNull",
				///
///<summary>
///75 
///</summary>

				"Ne",
				///
///<summary>
///76 
///</summary>

				"Eq",
				///
///<summary>
///77 
///</summary>

				"Gt",
				///
///<summary>
///78 
///</summary>

				"Le",
				///
///<summary>
///79 
///</summary>

				"Lt",
				///
///<summary>
///80 
///</summary>

				"Ge",
				///
///<summary>
///81 
///</summary>

				"IdxLT",
				///
///<summary>
///82 
///</summary>

				"BitAnd",
				///
///<summary>
///83 
///</summary>

				"BitOr",
				///
///<summary>
///84 
///</summary>

				"ShiftLeft",
				///
///<summary>
///85 
///</summary>

				"ShiftRight",
				///
///<summary>
///86 
///</summary>

				"Add",
				///
///<summary>
///87 
///</summary>

				"Subtract",
				///
///<summary>
///88 
///</summary>

				"Multiply",
				///
///<summary>
///89 
///</summary>

				"Divide",
				///
///<summary>
///90 
///</summary>

				"Remainder",
				///
///<summary>
///91 
///</summary>

				"Concat",
				///
///<summary>
///92 
///</summary>

				"IdxGE",
				///
///<summary>
///93 
///</summary>

				"BitNot",
				///
///<summary>
///94 
///</summary>

				"String8",
				///
///<summary>
///95 
///</summary>

				"Destroy",
				///
///<summary>
///96 
///</summary>

				"Clear",
				///
///<summary>
///97 
///</summary>

				"CreateIndex",
				///
///<summary>
///98 
///</summary>

				"CreateTable",
				///
///<summary>
///99 
///</summary>

				"ParseSchema",
				///
///<summary>
///100 
///</summary>

				"LoadAnalysis",
				///
///<summary>
///101 
///</summary>

				"DropTable",
				///
///<summary>
///102 
///</summary>

				"DropIndex",
				///
///<summary>
///103 
///</summary>

				"DropTrigger",
				///
///<summary>
///104 
///</summary>

				"IntegrityCk",
				///
///<summary>
///105 
///</summary>

				"RowSetAdd",
				///
///<summary>
///106 
///</summary>

				"RowSetRead",
				///
///<summary>
///107 
///</summary>

				"RowSetTest",
				///
///<summary>
///108 
///</summary>

				"Program",
				///
///<summary>
///109 
///</summary>

				"Param",
				///
///<summary>
///110 
///</summary>

				"FkCounter",
				///
///<summary>
///111 
///</summary>

				"FkIfZero",
				///
///<summary>
///112 
///</summary>

				"MemMax",
				///
///<summary>
///113 
///</summary>

				"IfPos",
				///
///<summary>
///114 
///</summary>

				"IfNeg",
				///
///<summary>
///115 
///</summary>

				"IfZero",
				///
///<summary>
///116 
///</summary>

				"AggStep",
				///
///<summary>
///117 
///</summary>

				"AggFinal",
				///
///<summary>
///118 
///</summary>

				"Checkpoint",
				///
///<summary>
///119 
///</summary>

				"JournalMode",
				///
///<summary>
///120 
///</summary>

				"Vacuum",
				///
///<summary>
///121 
///</summary>

				"IncrVacuum",
				///
///<summary>
///122 
///</summary>

				"Expire",
				///
///<summary>
///123 
///</summary>

				"TableLock",
				///
///<summary>
///124 
///</summary>

				"VBegin",
				///
///<summary>
///125 
///</summary>

				"VCreate",
				///
///<summary>
///126 
///</summary>

				"VDestroy",
				///
///<summary>
///127 
///</summary>

				"VOpen",
				///
///<summary>
///128 
///</summary>

				"VFilter",
				///
///<summary>
///129 
///</summary>

				"VColumn",
				///
///<summary>
///130 
///</summary>

				"Real",
				///
///<summary>
///131 
///</summary>

				"VNext",
				///
///<summary>
///132 
///</summary>

				"VRename",
				///
///<summary>
///133 
///</summary>

				"VUpdate",
				///
///<summary>
///134 
///</summary>

				"Pagecount",
				///
///<summary>
///135 
///</summary>

				"MaxPgcnt",
				///
///<summary>
///136 
///</summary>

				"Trace",
				///
///<summary>
///137 
///</summary>

				"Noop",
				///
///<summary>
///138 
///</summary>

				"Explain",
				///
///<summary>
///139 
///</summary>

				"NotUsed_139",
				///
///<summary>
///140 
///</summary>

				"NotUsed_140",
				///
///<summary>
///141 
///</summary>

				"ToText",
				///
///<summary>
///142 
///</summary>

				"ToBlob",
				///
///<summary>
///143 
///</summary>

				"ToNumeric",
				///
///<summary>
///144 
///</summary>

				"ToInt",
				///
///<summary>
///145 
///</summary>

				"ToReal",
			};
			return azName [i];
		}
	#endif
	}
}
