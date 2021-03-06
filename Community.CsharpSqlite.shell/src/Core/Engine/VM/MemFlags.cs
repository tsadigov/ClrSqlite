﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine
{


    ///<summary>
    ///One or more of the following flags are set to indicate the validOK
    ///representations of the value stored in the Mem struct.
    ///
    ///If the MEM.MEM_Null flag is set, then the value is an SQL NULL value.
    ///No other flags may be set in this case.
    ///
    ///If the MEM.MEM_Str flag is set then Mem.z points at a string representation.
    ///Usually this is encoded in the same unicode encoding as the main
    ///database (see below for exceptions). If the MEM.MEM_Term flag is also
    ///set, then the string is nul terminated. The MEM.MEM_Int and MEM.MEM_Real
    ///flags may coexist with the MEM.MEM_Str flag.
    ///
    ///</summary>

    [Flags]
    public enum MemFlags : short
    {
        MEM_Null = 0x0001,

        MEM_Str = 0x0002,

        MEM_Int = 0x0004,

        MEM_Real = 0x0008,

        MEM_Blob = 0x0010,

        MEM_RowSet = 0x0020,

        MEM_Frame = 0x0040,

        MEM_Invalid = 0x0080,

        MEM_TypeMask = 0x00ff,

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

        MEM_Term = 0x0200,
        MEM_Dyn = 0x0400,
        MEM_Static = 0x0800,
        MEM_Ephem = 0x1000,
        MEM_Agg = 0x2000,

#if !SQLITE_OMIT_INCRBLOB
																																																const int MEM_Zero = 0x4000;  
#else
        MEM_Zero = 0x0000
#endif
        // TODO -- Convert back to inline for speed
    }

}
