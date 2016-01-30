using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine.Metadata
{
    using CsharpSqlite.Metadata;
    public class VdbeFunc : FuncDef
    {
        ///<summary>
        ///The definition of the function 
        ///</summary>
        public FuncDef pFunc;

        ///<summary>
        ///Number of entries allocated for apAux[] 
        ///</summary>
        public int nAux;

        ///<summary>
        ///One slot for each function argument 
        ///</summary>
        public AuxData[] apAux = new AuxData[2];
    };

    ///<summary>
    ///A VdbeFunc is just a FuncDef (defined in sqliteInt.h) that contains
    /// additional information about auxiliary information bound to arguments
    /// of the function.  This is used to implement the sqlite3_get_auxdata()
    /// and sqlite3_set_auxdata() APIs.  The "auxdata" is some auxiliary data
    /// that can be associated with a constant argument to a function.  This
    /// allows functions such as "regexp" to compile their constant regular
    /// expression argument once and reused the compiled code for multiple
    /// invocations.
    ///</summary>
    public class AuxData
    {
        /// <summary>
        /// ///Aux data for the i-th argument
        /// </summary>
        public object pAux;
    };
}
