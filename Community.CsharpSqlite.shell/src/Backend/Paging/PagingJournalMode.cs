using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Paging
{
    ///
    ///<summary>
    ///Numeric constants that encode the journalmode.  
    ///
    ///</summary>

    //#define PAGER_JOURNALMODE_QUERY     (-1)  /* Query the value of journalmode */
    //#define PAGER_JOURNALMODE_DELETE      0   /* Commit by deleting journal file */
    //#define PAGER_JOURNALMODE_PERSIST     1   /* Commit by zeroing journal header */
    //#define PAGER_JOURNALMODE_OFF         2   /* Journal omitted.  */
    //#define PAGER_JOURNALMODE_TRUNCATE    3   /* Commit by truncating journal */
    //#define PAGER_JOURNALMODE_MEMORY      4   /* In-memory journal file */
    //#define PAGER_JOURNALMODE_WAL         5   /* Use write-ahead logging */
    public enum JournalMode
    {
        PAGER_JOURNALMODE_QUERY = -1,
        PAGER_JOURNALMODE_DELETE = 0,
        PAGER_JOURNALMODE_PERSIST = 1,
        PAGER_JOURNALMODE_OFF = 2,
        PAGER_JOURNALMODE_TRUNCATE = 3,
        PAGER_JOURNALMODE_MEMORY = 4,
        PAGER_JOURNALMODE_WAL = 5
    }
}
