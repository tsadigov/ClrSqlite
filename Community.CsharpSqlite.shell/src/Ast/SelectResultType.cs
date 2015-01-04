using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{

    ///
    ///<summary>
    ///The results of a select can be distributed in several ways.  The
    ///"SRT" prefix means "SELECT Result Type".
    ///
    ///</summary>

    public enum SelectResultType
    {
        Union = 1,
        //#define SelectResultType.Union        1  /* Store result as keys in an index */
        Except = 2,
        //#define SelectResultType.Except      2  /* Remove result from a UNION index */
        Exists = 3,
        //#define SelectResultType.Exists      3  /* Store 1 if the result is not empty */
        Discard = 4,
        //#define SelectResultType.Discard    4  /* Do not save the results anywhere */
        ///
        ///<summary>
        ///The ORDER BY clause is ignored for all of the above 
        ///</summary>

        //#define IgnorableOrderby(X) ((X->eDest)<=SelectResultType.Discard)
        Output = 5,
        //#define SelectResultType.Output      5  /* Output each row of result */
        Mem = 6,
        //#define SelectResultType.Mem            6  /* Store result in a memory cell */
        Set = 7,
        //#define SelectResultType.Set            7  /* Store results as keys in an index */
        Table = 8,
        //#define SelectResultType.Table        8  /* Store result as data with an automatic rowid */
        EphemTab = 9,
        //#define SelectResultType.EphemTab  9  /* Create transient tab and store like SelectResultType.Table /
        Coroutine = 10
        //#define SelectResultType.Coroutine   10  /* Generate a single row of result */
    }

}
