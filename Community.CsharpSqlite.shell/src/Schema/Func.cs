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
        ///Each SQL function is defined by an instance of the following
        ///structure.  A pointer to this structure is stored in the sqlite.aFunc
        ///hash table.  When multiple functions have the same name, the hash table
        ///points to a linked list of these structures.
        ///
        ///</summary>
        public class FuncDef
        {
            public i16 nArg;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Number of arguments.  ">1 means unlimited </param>
            public SqliteEncoding iPrefEnc;
            ///
            ///<summary>
            ///Preferred text encoding (SqliteEncoding.UTF8, 16LE, 16BE) 
            ///</summary>
            public u8 flags;
            ///
            ///<summary>
            ///Some combination of SQLITE_FUNC_* 
            ///</summary>
            public object pUserData;
            ///
            ///<summary>
            ///User data parameter 
            ///</summary>
            public FuncDef pNext;
            ///
            ///<summary>
            ///Next function with same name 
            ///</summary>
            public dxFunc xFunc;
            //)(sqlite3_context*,int,sqlite3_value*); /* Regular function */
            public dxStep xStep;
            //)(sqlite3_context*,int,sqlite3_value*); /* Aggregate step */
            public dxFinal xFinalize;
            //)(sqlite3_context);                /* Aggregate finalizer */
            public string zName;
            ///
            ///<summary>
            ///SQL name of the function. 
            ///</summary>
            public FuncDef pHash;
            ///
            ///<summary>
            ///Next with a different name but the same hash 
            ///</summary>
            public FuncDestructor pDestructor;
            ///<summary>
            ///Reference counted destructor function
            ///</summary>
            public FuncDef()
            {
            }
            public FuncDef(i16 nArg, SqliteEncoding iPrefEnc, u8 iflags, object pUserData, FuncDef pNext, dxFunc xFunc, dxStep xStep, dxFinal xFinalize, string zName, FuncDef pHash, FuncDestructor pDestructor)
            {
                this.nArg = nArg;
                this.iPrefEnc = iPrefEnc;
                this.flags = iflags;
                this.pUserData = pUserData;
                this.pNext = pNext;
                this.xFunc = xFunc;
                this.xStep = xStep;
                this.xFinalize = xFinalize;
                this.zName = zName;
                this.pHash = pHash;
                this.pDestructor = pDestructor;
            }
            public FuncDef(string zName, SqliteEncoding iPrefEnc, i16 nArg, int iArg, u8 iflags, dxFunc xFunc)
            {
                this.nArg = nArg;
                this.iPrefEnc = iPrefEnc;
                this.flags = iflags;
                this.pUserData = iArg;
                this.pNext = null;
                this.xFunc = xFunc;
                this.xStep = null;
                this.xFinalize = null;
                this.zName = zName;
            }
            public FuncDef(string zName, SqliteEncoding iPrefEnc, i16 nArg, int iArg, u8 iflags, dxStep xStep, dxFinal xFinal)
            {
                this.nArg = nArg;
                this.iPrefEnc = iPrefEnc;
                this.flags = iflags;
                this.pUserData = iArg;
                this.pNext = null;
                this.xFunc = null;
                this.xStep = xStep;
                this.xFinalize = xFinal;
                this.zName = zName;
            }
            public FuncDef(string zName, SqliteEncoding iPrefEnc, i16 nArg, object arg, dxFunc xFunc, u8 flags)
            {
                this.nArg = nArg;
                this.iPrefEnc = iPrefEnc;
                this.flags = flags;
                this.pUserData = arg;
                this.pNext = null;
                this.xFunc = xFunc;
                this.xStep = null;
                this.xFinalize = null;
                this.zName = zName;
            }
            public FuncDef Copy()
            {
                FuncDef c = new FuncDef();
                c.nArg = nArg;
                c.iPrefEnc = iPrefEnc;
                c.flags = flags;
                c.pUserData = pUserData;
                c.pNext = pNext;
                c.xFunc = xFunc;
                c.xStep = xStep;
                c.xFinalize = xFinalize;
                c.zName = zName;
                c.pHash = pHash;
                c.pDestructor = pDestructor;
                return c;
            }
        };

        ///<summary>
        /// This structure encapsulates a user-function destructor callback (as
        /// configured using create_function_v2()) and a reference counter. When
        /// create_function_v2() is called to create a function with a destructor,
        /// a single object of this type is allocated. FuncDestructor.nRef is set to
        /// the number of FuncDef objects created (either 1 or 3, depending on whether
        /// or not the specified encoding is SqliteEncoding.ANY). The FuncDef.pDestructor
        /// member of each of the new FuncDef objects is set to point to the allocated
        /// FuncDestructor.
        ///
        /// Thereafter, when one of the FuncDef objects is deleted, the reference
        /// count on this object is decremented. When it reaches 0, the destructor
        /// is invoked and the FuncDestructor structure freed.
        ///
        ///</summary>
        //struct FuncDestructor {
        //  int nRef;
        //  void (*xDestroy)(void );
        //  void *pUserData;
        //};
        public class FuncDestructor
        {
            public int nRef;
            public dxFDestroy xDestroy;
            // (*xDestroy)(void );
            public object pUserData;
        };

        ///
        ///<summary>
        ///Possible values for FuncDef.flags
        ///
        ///</summary>
        //#define SQLITE_FUNC_LIKE     0x01  /* Candidate for the LIKE optimization */
        //#define SQLITE_FUNC_CASE     0x02  /* Case-sensitive LIKE-type function */
        //#define SQLITE_FUNC_EPHEM    0x04  /* Ephemeral.  Delete with VDBE */
        //#define SQLITE_FUNC_NEEDCOLL 0x08 /* sqlite3GetFuncCollSeq() might be called */
        //#define SQLITE_FUNC_PRIVATE  0x10 /* Allowed for internal use only */
        //#define SQLITE_FUNC_COUNT    0x20 /* Built-in count() aggregate */
        //#define SQLITE_FUNC_COALESCE 0x40 /* Built-in coalesce() or ifnull() function */
        private const int SQLITE_FUNC_LIKE = 0x01;
        ///
        ///<summary>
        ///Candidate for the LIKE optimization 
        ///</summary>
        private const int SQLITE_FUNC_CASE = 0x02;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Case">type function </param>
        private const int SQLITE_FUNC_EPHEM = 0x04;
        ///
        ///<summary>
        ///Ephermeral.  Delete with VDBE 
        ///</summary>
        private const int SQLITE_FUNC_NEEDCOLL = 0x08;
        ///
        ///<summary>
        ///sqlite3GetFuncCollSeq() might be called 
        ///</summary>
        private const int SQLITE_FUNC_PRIVATE = 0x10;
        ///
        ///<summary>
        ///Allowed for internal use only 
        ///</summary>
        private const int SQLITE_FUNC_COUNT = 0x20;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Built">in count() aggregate </param>
        private const int SQLITE_FUNC_COALESCE = 0x40;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Built">in coalesce() or ifnull() function </param>
        ///<summary>
        /// The following three macros, FUNCTION(), LIKEFUNC() and AGGREGATE() are
        /// used to create the initializers for the FuncDef structures.
        ///
        ///   FUNCTION(zName, nArg, iArg, bNC, xFunc)
        ///     Used to create a scalar function definition of a function zName
        ///     implemented by C function xFunc that accepts nArg arguments. The
        ///     value passed as iArg is cast to a (void) and made available
        ///     as the user-data (sqlite3_user_data()) for the function. If
        ///     argument bNC is true, then the SQLITE_FUNC_NEEDCOLL flag is set.
        ///
        ///   AGGREGATE(zName, nArg, iArg, bNC, xStep, xFinal)
        ///     Used to create an aggregate function definition implemented by
        ///     the C functions xStep and xFinal. The first four parameters
        ///     are interpreted in the same way as the first 4 parameters to
        ///     FUNCTION().
        ///
        ///   LIKEFUNC(zName, nArg, pArg, flags)
        ///     Used to create a scalar function definition of a function zName
        ///     that accepts nArg arguments and is implemented by a call to C
        ///     function likeFunc. Argument pArg is cast to a (void ) and made
        ///     available as the function user-data (sqlite3_user_data()). The
        ///     FuncDef.flags variable is set to the value passed as the flags
        ///     parameter.
        ///
        ///</summary>
        //#define FUNCTION(zName, nArg, iArg, bNC, xFunc) \
        //  {nArg, SqliteEncoding.UTF8, bNC*SQLITE_FUNC_NEEDCOLL, \
        //SQLITE_INT_TO_PTR(iArg), 0, xFunc, 0, 0, #zName, 0, 0}
        private static FuncDef FUNCTION(string zName, i16 nArg, int iArg, u8 bNC, dxFunc xFunc)
        {
            return new FuncDef(zName, SqliteEncoding.UTF8, nArg, iArg, (u8)(bNC * SQLITE_FUNC_NEEDCOLL), xFunc);
        }
        //#define STR_FUNCTION(zName, nArg, pArg, bNC, xFunc) \
        //  {nArg, SqliteEncoding.UTF8, bNC*SQLITE_FUNC_NEEDCOLL, \
        //pArg, 0, xFunc, 0, 0, #zName, 0, 0}
        //#define LIKEFUNC(zName, nArg, arg, flags) \
        //  {nArg, SqliteEncoding.UTF8, flags, (void )arg, 0, likeFunc, 0, 0, #zName, 0, 0}
        private static FuncDef LIKEFUNC(string zName, i16 nArg, object arg, u8 flags)
        {
            return new FuncDef(zName, SqliteEncoding.UTF8, nArg, arg, likeFunc, flags);
        }
        //#define AGGREGATE(zName, nArg, arg, nc, xStep, xFinal) \
        //  {nArg, SqliteEncoding.UTF8, nc*SQLITE_FUNC_NEEDCOLL, \
        //SQLITE_INT_TO_PTR(arg), 0, 0, xStep,xFinal,#zName,0,0}
        private static FuncDef AGGREGATE(string zName, i16 nArg, int arg, u8 nc, dxStep xStep, dxFinal xFinal)
        {
            return new FuncDef(zName, SqliteEncoding.UTF8, nArg, arg, (u8)(nc * SQLITE_FUNC_NEEDCOLL), xStep, xFinal);
        }




    }
}
