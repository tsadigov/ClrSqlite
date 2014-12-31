namespace Community.CsharpSqlite
{
	public enum OpCode : byte
	{
		OP_Goto = 1,
		OP_Gosub = 2,
		OP_Return = 3,
		OP_Yield = 4,
		OP_HaltIfNull = 5,
		OP_Halt = 6,
		OP_Integer = 7,
		OP_Int64 = 8,
		OP_Real = 130///
///<summary>
///same as TK_FLOAT    
///</summary>

		,
		OP_String8 = 94///
///<summary>
///same as TK_STRING   
///</summary>

		,
		OP_String = 9,
		OP_Null = 10,
		OP_Blob = 11,
		OP_Variable = 12,
		OP_Move = 13,
		OP_Copy = 14,
		OP_SCopy = 15,
		OP_ResultRow = 16,
		OP_Concat = 91///
///<summary>
///same as TK_CONCAT   
///</summary>

		,
		OP_Add = 86///
///<summary>
///same as TK_PLUS     
///</summary>

		,
		OP_Subtract = 87///
///<summary>
///same as TK_MINUS    
///</summary>

		,
		OP_Multiply = 88///
///<summary>
///same as TK_STAR     
///</summary>

		,
		OP_Divide = 89///
///<summary>
///same as TK_SLASH    
///</summary>

		,
		OP_Remainder = 90///
///<summary>
///same as TK_REM      
///</summary>

		,
		OP_CollSeq = 17,
		OP_Function = 18,
		OP_BitAnd = 82///
///<summary>
///same as TK_BITAND   
///</summary>

		,
		OP_BitOr = 83///
///<summary>
///same as TK_BITOR    
///</summary>

		,
		OP_ShiftLeft = 84///
///<summary>
///same as TK_LSHIFT   
///</summary>

		,
		OP_ShiftRight = 85///
///<summary>
///same as TK_RSHIFT   
///</summary>

		,
		OP_AddImm = 20,
		OP_MustBeInt = 21,
		OP_RealAffinity = 22,
		OP_ToText = 141///
///<summary>
///same as TK_TO_TEXT  
///</summary>

		,
		OP_ToBlob = 142///
///<summary>
///same as TK_TO_BLOB  
///</summary>

		,
		OP_ToNumeric = 143///
///<summary>
///same as TK_TO_NUMERIC
///</summary>

		,
		OP_ToInt = 144///
///<summary>
///same as TK_TO_INT   
///</summary>

		,
		OP_ToReal = 145///
///<summary>
///same as TK_TO_REAL  
///</summary>

		,
		OP_Eq = 76///
///<summary>
///same as TK_EQ       
///</summary>

		,
		OP_Ne = 75///
///<summary>
///same as TK_NE       
///</summary>

		,
		OP_Lt = 79///
///<summary>
///same as TK_LT       
///</summary>

		,
		OP_Le = 78///
///<summary>
///same as TK_LE       
///</summary>

		,
		OP_Gt = 77///
///<summary>
///same as TK_GT       
///</summary>

		,
		OP_Ge = 80///
///<summary>
///same as TK_GE       
///</summary>

		,
		OP_Permutation = 23,
		OP_Compare = 24,
		OP_Jump = 25,
		OP_And = 69///
///<summary>
///same as TK_AND      
///</summary>

		,
		OP_Or = 68///
///<summary>
///same as TK_OR       
///</summary>

		,
		OP_Not = 19///
///<summary>
///same as TK_NOT      
///</summary>

		,
		OP_BitNot = 93///
///<summary>
///same as TK_BITNOT   
///</summary>

		,
		OP_If = 26,
		OP_IfNot = 27,
		OP_IsNull = 73///
///<summary>
///same as TK_ISNULL   
///</summary>

		,
		OP_NotNull = 74///
///<summary>
///same as TK_NOTNULL  
///</summary>

		,
		OP_Column = 28,
		OP_Affinity = 29,
		OP_MakeRecord = 30,
		OP_Count = 31,
		OP_Savepoint = 32,
		OP_AutoCommit = 33,
		OP_Transaction = 34,
		OP_ReadCookie = 35,
		OP_SetCookie = 36,
		OP_VerifyCookie = 37,
		OP_OpenRead = 38,
		OP_OpenWrite = 39,
		OP_OpenAutoindex = 40,
		OP_OpenEphemeral = 41,
		OP_OpenPseudo = 42,
		OP_Close = 43,
		OP_SeekLt = 44,
		OP_SeekLe = 45,
		OP_SeekGe = 46,
		OP_SeekGt = 47,
		OP_Seek = 48,
		OP_NotFound = 49,
		OP_Found = 50,
		OP_IsUnique = 51,
		OP_NotExists = 52,
		OP_Sequence = 53,
		OP_NewRowid = 54,
		OP_Insert = 55,
		OP_InsertInt = 56,
		OP_Delete = 57,
		OP_ResetCount = 58,
		OP_RowKey = 59,
		OP_RowData = 60,
		OP_Rowid = 61,
		OP_NullRow = 62,
		OP_Last = 63,
		OP_Sort = 64,
		OP_Rewind = 65,
		OP_Prev = 66,
		OP_Next = 67,
		OP_IdxInsert = 70,
		OP_IdxDelete = 71,
		OP_IdxRowid = 72,
		OP_IdxLT = 81,
		OP_IdxGE = 92,
		OP_Destroy = 95,
		OP_Clear = 96,
		OP_CreateIndex = 97,
		OP_CreateTable = 98,
		OP_ParseSchema = 99,
		OP_LoadAnalysis = 100,
		OP_DropTable = 101,
		OP_DropIndex = 102,
		OP_DropTrigger = 103,
		OP_IntegrityCk = 104,
		OP_RowSetAdd = 105,
		OP_RowSetRead = 106,
		OP_RowSetTest = 107,
		OP_Program = 108,
		OP_Param = 109,
		OP_FkCounter = 110,
		OP_FkIfZero = 111,
		OP_MemMax = 112,
		OP_IfPos = 113,
		OP_IfNeg = 114,
		OP_IfZero = 115,
		OP_AggStep = 116,
		OP_AggFinal = 117,
		OP_Checkpoint = 118,
		OP_JournalMode = 119,
		OP_Vacuum = 120,
		OP_IncrVacuum = 121,
		OP_Expire = 122,
		OP_TableLock = 123,
		OP_VBegin = 124,
		OP_VCreate = 125,
		OP_VDestroy = 126,
		OP_VOpen = 127,
		OP_VFilter = 128,
		OP_VColumn = 129,
		OP_VNext = 131,
		OP_VRename = 132,
		OP_VUpdate = 133,
		OP_Pagecount = 134,
		OP_MaxPgcnt = 135,
		OP_Trace = 136,
		OP_Noop = 137,
		OP_Explain = 138,
		///
///<summary>
///The following opcode values are never used 
///</summary>

		//#define OP_NotUsed_139                        139
		//#define OP_NotUsed_140                        140
		///
///<summary>
///The following opcode values are never used 
///</summary>

		OP_NotUsed_138 = 138,
		OP_NotUsed_139 = 139,
		OP_NotUsed_140 = 140
	}
	public partial class Sqlite3
	{



        //#define OpCode.OP_Goto                                 1
        
        
        

        public const int OP_Real = 130///
            ///<summary>
            ///same as TK_FLOAT    
            ///</summary>

        ;

        public const int OP_String8 = 94///
            ///<summary>
            ///same as TK_STRING   
            ///</summary>

        ;

        

        public const int OP_Concat = 91///
            ///<summary>
            ///same as TK_CONCAT   
            ///</summary>

        ;

        public const int OP_Add = 86///
            ///<summary>
            ///same as TK_PLUS     
            ///</summary>

        ;

        public const int OP_Subtract = 87///
            ///<summary>
            ///same as TK_MINUS    
            ///</summary>

        ;

        public const int OP_Multiply = 88///
            ///<summary>
            ///same as TK_STAR     
            ///</summary>

        ;

        public const int OP_Divide = 89///
            ///<summary>
            ///same as TK_SLASH    
            ///</summary>

        ;

        public const int OP_Remainder = 90///
            ///<summary>
            ///same as TK_REM      
            ///</summary>

        ;

        public const int OP_CollSeq = 17;

        public const int OP_Function = 18;

        public const int OP_BitAnd = 82///
            ///<summary>
            ///same as TK_BITAND   
            ///</summary>

        ;

        public const int OP_BitOr = 83///
            ///<summary>
            ///same as TK_BITOR    
            ///</summary>

        ;

        public const int OP_ShiftLeft = 84///
            ///<summary>
            ///same as TK_LSHIFT   
            ///</summary>

        ;

        public const int OP_ShiftRight = 85///
            ///<summary>
            ///same as TK_RSHIFT   
            ///</summary>

        ;

        public const int OP_AddImm = 20;

        public const int OP_MustBeInt = 21;

        public const int OP_RealAffinity = 22;

        public const int OP_ToText = 141///
            ///<summary>
            ///same as TK_TO_TEXT  
            ///</summary>

        ;

        public const int OP_ToBlob = 142///
            ///<summary>
            ///same as TK_TO_BLOB  
            ///</summary>

        ;

        public const int OP_ToNumeric = 143///
            ///<summary>
            ///same as TK_TO_NUMERIC
            ///</summary>

        ;

        public const int OP_ToInt = 144///
            ///<summary>
            ///same as TK_TO_INT   
            ///</summary>

        ;

        public const int OP_ToReal = 145///
            ///<summary>
            ///same as TK_TO_REAL  
            ///</summary>

        ;

        public const int OP_Eq = 76///
            ///<summary>
            ///same as TK_EQ       
            ///</summary>

        ;

        public const int OP_Ne = 75///
            ///<summary>
            ///same as TK_NE       
            ///</summary>

        ;

        public const int OP_Lt = 79///
            ///<summary>
            ///same as TK_LT       
            ///</summary>

        ;

        public const int OP_Le = 78///
            ///<summary>
            ///same as TK_LE       
            ///</summary>

        ;

        public const int OP_Gt = 77///
            ///<summary>
            ///same as TK_GT       
            ///</summary>

        ;

        public const int OP_Ge = 80///
            ///<summary>
            ///same as TK_GE       
            ///</summary>

        ;

        public const int OP_Permutation = 23;

        
        
        public const int OP_And = 69///
            ///<summary>
            ///same as TK_AND      
            ///</summary>

        ;

        public const int OP_Or = 68///
            ///<summary>
            ///same as TK_OR       
            ///</summary>

        ;

        public const int OP_Not = 19///
            ///<summary>
            ///same as TK_NOT      
            ///</summary>

        ;

        public const int OP_BitNot = 93///
            ///<summary>
            ///same as TK_BITNOT   
            ///</summary>

        ;

        public const int OP_If = 26;

        public const int OP_IfNot = 27;

        public const int OP_IsNull = 73///
            ///<summary>
            ///same as TK_ISNULL   
            ///</summary>

        ;

        public const int OP_NotNull = 74///
            ///<summary>
            ///same as TK_NOTNULL  
            ///</summary>

        ;

        public const int OP_Column = 28;

        public const int OP_Affinity = 29;

        public const int OP_MakeRecord = 30;

        public const int OP_Count = 31;

        public const int OP_Savepoint = 32;

        public const int OP_AutoCommit = 33;

        public const int OP_Transaction = 34;

        public const int OP_ReadCookie = 35;

        public const int OP_SetCookie = 36;

        public const int OP_VerifyCookie = 37;

        public const int OP_OpenRead = 38;

        public const int OP_OpenWrite = 39;

        public const int OP_OpenAutoindex = 40;

        public const int OP_OpenEphemeral = 41;

        public const int OP_OpenPseudo = 42;

        public const int OP_Close = 43;

        
        public const int OP_NotFound = 49;

        public const int OP_Found = 50;

        public const int OP_IsUnique = 51;

        public const int OP_NotExists = 52;

        public const int OP_Sequence = 53;

        public const int OP_NewRowid = 54;

        public const int OP_Insert = 55;

        public const int OP_InsertInt = 56;

        public const int OP_Delete = 57;

        public const int OP_ResetCount = 58;

        public const int OP_RowKey = 59;

        public const int OP_RowData = 60;

        public const int OP_Rowid = 61;

        public const int OP_NullRow = 62;

        public const int OP_Last = 63;

        public const int OP_Sort = 64;

        public const int OP_Rewind = 65;

        public const int OP_Prev = 66;

        public const int OP_Next = 67;

        public const int OP_IdxInsert = 70;

        public const int OP_IdxDelete = 71;

        public const int OP_IdxRowid = 72;

        public const int OP_IdxLT = 81;

        public const int OP_IdxGE = 92;

        public const int OP_Destroy = 95;

        public const int OP_Clear = 96;

        public const int OP_CreateIndex = 97;

        public const int OP_CreateTable = 98;

        public const int OP_ParseSchema = 99;

        public const int OP_LoadAnalysis = 100;

        public const int OP_DropTable = 101;

        public const int OP_DropIndex = 102;

        public const int OP_DropTrigger = 103;

        public const int OP_IntegrityCk = 104;

        public const int OP_RowSetAdd = 105;

        public const int OP_RowSetRead = 106;

        public const int OP_RowSetTest = 107;

        public const int OP_Program = 108;

        public const int OP_Param = 109;

        public const int OP_FkCounter = 110;

        public const int OP_FkIfZero = 111;

        public const int OP_MemMax = 112;

        public const int OP_IfPos = 113;

        public const int OP_IfNeg = 114;

        public const int OP_IfZero = 115;

        public const int OP_AggStep = 116;

        public const int OP_AggFinal = 117;

        public const int OP_Checkpoint = 118;

        public const int OP_JournalMode = 119;

        public const int OP_Vacuum = 120;

        public const int OP_IncrVacuum = 121;

        public const int OP_Expire = 122;

        

        
        

        public const int OP_Pagecount = 134;

        public const int OP_MaxPgcnt = 135;



        ///
        ///<summary>
        ///The following opcode values are never used 
        ///</summary>

        //#define OP_NotUsed_139                        139
        //#define OP_NotUsed_140                        140
        ///
        ///<summary>
        ///The following opcode values are never used 
        ///</summary>

        public const int OP_NotUsed_138 = 138;

        public const int OP_NotUsed_139 = 139;

        public const int OP_NotUsed_140 = 140;


		///
///<summary>
///Automatically generated.  Do not edit 
///</summary>

		///
///<summary>
///See the mkopcodeh.awk script for details 
///</summary>

		///
///<summary>
///Automatically generated.  Do not edit 
///</summary>

		///
///<summary>
///See the mkopcodeh.awk script for details 
///</summary>

		///
///<summary>
///Properties such as "out2" or "jump" that are specified in
///comments following the "case" for each opcode in the vdbe.c
///are encoded into bitvectors as follows:
///
///</summary>

		//#define OPFLG_JUMP            0x0001  /* jump:  P2 holds jmp target */
		//#define OPFLG_OUT2_PRERELEASE 0x0002  /* out2-prerelease: */
		//#define OPFLG_IN1             0x0004  /* in1:   P1 is an input */
		//#define OPFLG_IN2             0x0008  /* in2:   P2 is an input */
		//#define OPFLG_IN3             0x0010  /* in3:   P3 is an input */
		//#define OPFLG_OUT2            0x0020  /* out2:  P2 is an output */
		//#define OPFLG_OUT3            0x0040  /* out3:  P3 is an output */
		public const int OPFLG_JUMP = 0x0001;

		///
///<summary>
///jump:  P2 holds jmp target 
///</summary>

		public const int OPFLG_OUT2_PRERELEASE = 0x0002;

		///
///<summary>
///</summary>
///<param name="out2">prerelease: </param>

		public const int OPFLG_IN1 = 0x0004;

		///
///<summary>
///in1:   P1 is an input 
///</summary>

		public const int OPFLG_IN2 = 0x0008;

		///
///<summary>
///in2:   P2 is an input 
///</summary>

		public const int OPFLG_IN3 = 0x0010;

		///
///<summary>
///in3:   P3 is an input 
///</summary>

		public const int OPFLG_OUT2 = 0x0020;

		///
///<summary>
///out2:  P2 is an output 
///</summary>

		public const int OPFLG_OUT3 = 0x0040;

		///
///<summary>
///out3:  P3 is an output 
///</summary>

		public static int[] OPFLG_INITIALIZER = new int[] {
			///
///<summary>
///0 
///</summary>

			0x00,
			0x01,
			0x05,
			0x04,
			0x04,
			0x10,
			0x00,
			0x02,
			///
///<summary>
///8 
///</summary>

			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x00,
			0x24,
			0x24,
			///
///<summary>
///16 
///</summary>

			0x00,
			0x00,
			0x00,
			0x24,
			0x04,
			0x05,
			0x04,
			0x00,
			///
///<summary>
///24 
///</summary>

			0x00,
			0x01,
			0x05,
			0x05,
			0x00,
			0x00,
			0x00,
			0x02,
			///
///<summary>
///32 
///</summary>

			0x00,
			0x00,
			0x00,
			0x02,
			0x10,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///40 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x11,
			0x11,
			0x11,
			0x11,
			///
///<summary>
///48 
///</summary>

			0x08,
			0x11,
			0x11,
			0x11,
			0x11,
			0x02,
			0x02,
			0x00,
			///
///<summary>
///56 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x02,
			0x00,
			0x01,
			///
///<summary>
///64 
///</summary>

			0x01,
			0x01,
			0x01,
			0x01,
			0x4c,
			0x4c,
			0x08,
			0x00,
			///
///<summary>
///72 
///</summary>

			0x02,
			0x05,
			0x05,
			0x15,
			0x15,
			0x15,
			0x15,
			0x15,
			///
///<summary>
///80 
///</summary>

			0x15,
			0x01,
			0x4c,
			0x4c,
			0x4c,
			0x4c,
			0x4c,
			0x4c,
			///
///<summary>
///88 
///</summary>

			0x4c,
			0x4c,
			0x4c,
			0x4c,
			0x01,
			0x24,
			0x02,
			0x02,
			///
///<summary>
///96 
///</summary>

			0x00,
			0x02,
			0x02,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///104 
///</summary>

			0x00,
			0x0c,
			0x45,
			0x15,
			0x01,
			0x02,
			0x00,
			0x01,
			///
///<summary>
///112 
///</summary>

			0x08,
			0x05,
			0x05,
			0x05,
			0x00,
			0x00,
			0x00,
			0x02,
			///
///<summary>
///120 
///</summary>

			0x00,
			0x01,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///128 
///</summary>

			0x01,
			0x00,
			0x02,
			0x01,
			0x00,
			0x00,
			0x02,
			0x02,
			///
///<summary>
///136 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x04,
			0x04,
			0x04,
			///
///<summary>
///144 
///</summary>

			0x04,
			0x04
		};
	}
}
