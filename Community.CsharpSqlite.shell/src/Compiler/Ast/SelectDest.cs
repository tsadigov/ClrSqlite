using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite.Ast
{
    ///<summary>
    /// A structure used to customize the behavior of sqlite3Select(). See
    /// comments above sqlite3Select() for details.
    ///
    ///</summary>
    //typedef struct SelectDest SelectDest;
    public class SelectDest
    {
        public SelectResultType eDest;

        ///
        ///<summary>
        ///How to dispose of the results 
        ///</summary>

        public char affinity;

        ///
        ///<summary>
        ///Affinity used when eDest==SelectResultType.Set 
        ///</summary>

        public int iParm;

        ///
        ///<summary>
        ///A parameter used by the eDest disposal method 
        ///</summary>

        int _iMem;

        public int iMem
        {
            get { return _iMem; }
            set { _iMem = value; }
        }

        ///
        ///<summary>
        ///Base register where results are written 
        ///</summary>

        int _nMem;

        public int nMem
        {
            get { return _nMem; }
            set { _nMem = value; }
        }

        ///
        ///<summary>
        ///Number of registers allocated 
        ///</summary>

        public SelectDest()
        {
            this.eDest = 0;
            this.affinity = '\0';
            this.iParm = 0;
            this.iMem = 0;
            this.nMem = 0;
        }

        ///
        ///<summary>
        ///Initialize a SelectDest structure.
        ///
        ///</summary>
        public void Init( SelectResultType eDest, int iParm)
        {
            SelectDest pDest = this;
            pDest.eDest = eDest;
            pDest.iParm = iParm;
            pDest.affinity = '\0';
            pDest.iMem = 0;
            pDest.nMem = 0;
        }

        public SelectDest(SelectResultType eDest, char affinity, int iParm)
        {
            this.eDest = eDest;
            this.affinity = affinity;
            this.iParm = iParm;
            this.iMem = 0;
            this.nMem = 0;
        }

        public SelectDest(SelectResultType eDest, char affinity, int iParm, int iMem, int nMem)
        {
            this.eDest = eDest;
            this.affinity = affinity;
            this.iParm = iParm;
            this.iMem = iMem;
            this.nMem = nMem;
        }
    };

}
