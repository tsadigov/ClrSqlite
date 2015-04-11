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
