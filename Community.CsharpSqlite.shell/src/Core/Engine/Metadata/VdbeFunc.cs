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
        public FuncDef pFunc;

        ///
        ///<summary>
        ///The definition of the function 
        ///</summary>

        public int nAux;

        ///
        ///<summary>
        ///Number of entries allocated for apAux[] 
        ///</summary>

        public AuxData[] apAux = new AuxData[2];
        ///
        ///<summary>
        ///One slot for each function argument 
        ///</summary>

    };
}
