using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    using u8 = System.Byte;


    ///<summary>
    ///A smaller version of VdbeOp used for the VdbeAddOpList() function because
    ///it takes up less space.
    ///</summary>

    //typedef struct VdbeOpList VdbeOpList;
    ///
    ///<summary>
    ///Allowed values of VdbeOp.p4type
    ///
    ///</summary>

    public struct VdbeOpList
    {
        //TODO: OpCOde
        public u8 opcode;

        ///
        ///<summary>
        ///What operation to perform 
        ///</summary>

        public int p1;

        ///
        ///<summary>
        ///First operand 
        ///</summary>

        public int p2;

        ///
        ///<summary>
        ///Second parameter (often the jump destination) 
        ///</summary>

        public int p3;

        ///
        ///<summary>
        ///Third parameter 
        ///</summary>

        public VdbeOpList(OpCode opcode, int p1, int p2, int p3)
        {
            this.opcode = (u8)opcode;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }


}
