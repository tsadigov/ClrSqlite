using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///
///<summary>
///One or more of the following flags are set to indicate the validOK
///representations of the value stored in the Mem struct.
///
///If the MEM_Null flag is set, then the value is an SQL NULL value.
///No other flags may be set in this case.
///
///If the MEM_Str flag is set then Mem.z points at a string representation.
///Usually this is encoded in the same unicode encoding as the main
///database (see below for exceptions). If the MEM_Term flag is also
///set, then the string is nul terminated. The MEM_Int and MEM_Real
///flags may coexist with the MEM_Str flag.
///
///</summary>

		//#define MEM_Null      0x0001   /* Value is NULL */
		//#define MEM_Str       0x0002   /* Value is a string */
		//#define MEM_Int       0x0004   /* Value is an integer */
		//#define MEM_Real      0x0008   /* Value is a real number */
		//#define MEM_Blob      0x0010   /* Value is a BLOB */
		//#define MEM_RowSet    0x0020   /* Value is a RowSet object */
		//#define MEM_Frame     0x0040   /* Value is a VdbeFrame object */
		//#define MEM_Invalid   0x0080   /* Value is undefined */
		//#define MEM_TypeMask  0x00ff   /* Mask of type bits */
		const int MEM_Null = 0x0001;

		const int MEM_Str = 0x0002;

		const int MEM_Int = 0x0004;

		const int MEM_Real = 0x0008;

		const int MEM_Blob = 0x0010;

		const int MEM_RowSet = 0x0020;

		const int MEM_Frame = 0x0040;

		const int MEM_Invalid = 0x0080;

		const int MEM_TypeMask = 0x00ff;

		///
///<summary>
///Whenever Mem contains a valid string or blob representation, one of
///the following flags must be set to determine the memory management
///policy for Mem.z.  The MEM_Term flag tells us whether or not the
///string is \000 or \u0000 terminated
/////    
///</summary>

		//#define MEM_Term      0x0200   /* String rep is nul terminated */
		//#define MEM_Dyn       0x0400   /* Need to call sqliteFree() on Mem.z */
		//#define MEM_Static    0x0800   /* Mem.z points to a static string */
		//#define MEM_Ephem     0x1000   /* Mem.z points to an ephemeral string */
		//#define MEM_Agg       0x2000   /* Mem.z points to an agg function context */
		//#define MEM_Zero      0x4000   /* Mem.i contains count of 0s appended to blob */
		//#if SQLITE_OMIT_INCRBLOB
		//  #undef MEM_Zero
		//  #define MEM_Zero 0x0000
		//#endif
		const int MEM_Term = 0x0200;

		const int MEM_Dyn = 0x0400;

		const int MEM_Static = 0x0800;

		const int MEM_Ephem = 0x1000;

		const int MEM_Agg = 0x2000;

		#if !SQLITE_OMIT_INCRBLOB
																																																const int MEM_Zero = 0x4000;  
#else
		const int MEM_Zero = 0x0000;
	#endif



        //typedef struct sqlite3_mem_methods sqlite3_mem_methods;
        //struct sqlite3_mem_methods {
        //  void *(*xMalloc)(int);         /* Memory allocation function */
        //  void (*xFree)(void);          /* Free a prior allocation */
        //  void *(*xRealloc)(void*,int);  /* Resize an allocation */
        //  int (*xSize)(void);           /* Return the size of an allocation */
        //  int (*xRoundup)(int);          /* Round up request size to allocation size */
        //  int (*xInit)(void);           /* Initialize the memory allocator */
        //  void (*xShutdown)(void);      /* Deinitialize the memory allocator */
        //  void *pAppData;                /* Argument to xInit() and xShutdown() */
        //};
        public class sqlite3_mem_methods
        {
            public dxMalloc xMalloc;

            //void *(*xMalloc)(int);         /* Memory allocation function */
            public dxMallocInt xMallocInt;

            //void *(*xMalloc)(int);         /* Memory allocation function */
            public dxMallocMem xMallocMem;

            //void *(*xMalloc)(int);         /* Memory allocation function */
            public dxFree xFree;

            //void (*xFree)(void);          /* Free a prior allocation */
            public dxFreeInt xFreeInt;

            //void (*xFree)(void);          /* Free a prior allocation */
            public dxFreeMem xFreeMem;

            //void (*xFree)(void);          /* Free a prior allocation */
            public dxRealloc xRealloc;

            //void *(*xRealloc)(void*,int);  /* Resize an allocation */
            public dxSize xSize;

            //int (*xSize)(void);           /* Return the size of an allocation */
            public dxRoundup xRoundup;

            //int (*xRoundup)(int);          /* Round up request size to allocation size */
            public dxMemInit xInit;

            //int (*xInit)(void);           /* Initialize the memory allocator */
            public dxMemShutdown xShutdown;

            //void (*xShutdown)(void);      /* Deinitialize the memory allocator */
            public object pAppData;

            ///
            ///<summary>
            ///Argument to xInit() and xShutdown() 
            ///</summary>

            public sqlite3_mem_methods()
            {
            }

            public sqlite3_mem_methods(dxMalloc xMalloc, dxMallocInt xMallocInt, dxMallocMem xMallocMem, dxFree xFree, dxFreeInt xFreeInt, dxFreeMem xFreeMem, dxRealloc xRealloc, dxSize xSize, dxRoundup xRoundup, dxMemInit xInit, dxMemShutdown xShutdown, object pAppData)
            {
                this.xMalloc = xMalloc;
                this.xMallocInt = xMallocInt;
                this.xMallocMem = xMallocMem;
                this.xFree = xFree;
                this.xFreeInt = xFreeInt;
                this.xFreeMem = xFreeMem;
                this.xRealloc = xRealloc;
                this.xSize = xSize;
                this.xRoundup = xRoundup;
                this.xInit = xInit;
                this.xShutdown = xShutdown;
                this.pAppData = pAppData;
            }
        }
	}
}
