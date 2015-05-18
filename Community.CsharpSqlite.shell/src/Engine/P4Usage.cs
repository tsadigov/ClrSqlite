using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite//.Engine
{
    public enum P4Usage
    {
        ///<summary>
        ///The P4 parameter is not used 
        ///</summary>
        P4_NOTUSED = 0,


        ///<summary>
        ///Poer to a string obtained from sqliteMalloc=(), 
        ///</summary>

        P4_DYNAMIC = (-1),


        ///<summary>
        ///Poer to a static string 
        ///</summary>
        P4_STATIC = (-2),


        ///<summary>
        ///P4 is a poer to a CollSeq structure 
        ///</summary>
        P4_COLLSEQ = (-4),


        ///<summary>
        ///P4 is a poer to a FuncDef structure 
        ///</summary>
        P4_FUNCDEF = (-5),

        ///<summary>
        ///P4 is a poer to a KeyInfo structure 
        ///</summary>
        P4_KEYINFO = (-6),


        P4_VDBEFUNC = (-7),

        ///
        ///<summary>
        ///P4 is a poer to a VdbeFunc structure 
        ///</summary>

        P4_MEM = (-8),

        ///
        ///<summary>
        ///P4 is a poer to a Mem*    structure 
        ///</summary>

        P4_TRANSIENT = 0,

        ///
        ///<summary>
        ///P4 is a poer to a transient string 
        ///</summary>

        P4_VTAB = (-10),

        ///
        ///<summary>
        ///P4 is a poer to an sqlite3_vtab structure 
        ///</summary>

        P4_MPRINTF = (-11),

        ///
        ///<summary>
        ///P4 is a string obtained from io.sqlite3_mprf=(), 
        ///</summary>

        P4_REAL = (-12),

        ///
        ///<summary>
        ///</summary>
        ///<param name="P4 is a 64">bit floating po value </param>

        P4_INT64 = (-13),

        ///
        ///<summary>
        ///</summary>
        ///<param name="P4 is a 64">bit signed eger </param>

        P4_INT32 = (-14),

        ///
        ///<summary>
        ///</summary>
        ///<param name="P4 is a 32">bit signed eger </param>

        P4_INTARRAY = (-15),

        ///
        ///<summary>
        ///</summary>
        ///<param name="#define  P4_INTARRAY (">bit egers </param>

        P4_SUBPROGRAM = (-18),

        ///
        ///<summary>
        ///</summary>
        ///<param name="#define  P4_SUBPROGRAM  (">18) /* P4 is a poer to a SubProgram structure </param>
        ///



        ///
        ///<summary>
        ///When adding a P4 argument using  P4_KEYINFO, a copy of the KeyInfo structure
        ///is made.  That copy is freed when the Vdbe is finalized.  But if the
        ///argument is  P4_KEYINFO_HANDOFF, the passed in pointer is used.  It still
        ///gets freed when the Vdbe is finalized so it still should be obtained
        ///from a single sqliteMalloc().  But no copy is made and the calling
        ///function should *not* try to free the KeyInfo.
        ///</summary>

        P4_KEYINFO_HANDOFF = (-16),

        // #define  P4_KEYINFO_HANDOFF (-16)
        P4_KEYINFO_STATIC = (-17)

    }
}
