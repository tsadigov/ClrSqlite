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



        ///
        ///<summary>
        ///Each SQL table is represented in memory by an instance of the
        ///following structure.
        ///
        ///Table.zName is the name of the table.  The case of the original
        ///CREATE TABLE statement is stored, but case is not significant for
        ///comparisons.
        ///
        ///Table.nCol is the number of columns in this table.  Table.aCol is a
        ///pointer to an array of Column structures, one for each column.
        ///
        ///If the table has an INTEGER PRIMARY KEY, then Table.iPKey is the index of
        ///the column that is that key.   Otherwise Table.iPKey is negative.  Note
        ///that the datatype of the PRIMARY KEY must be INTEGER for this field to
        ///be set.  An INTEGER PRIMARY KEY is used as the rowid for each row of
        ///the table.  If a table has no INTEGER PRIMARY KEY, then a random rowid
        ///is generated for each row of the table.  TF_HasPrimaryKey is set if
        ///the table has any PRIMARY KEY, INTEGER or otherwise.
        ///
        ///Table.tnum is the page number for the root BTree page of the table in the
        ///database file.  If Table.iDb is the index of the database table backend
        ///in sqlite.aDb[].  0 is for the main database and 1 is for the file that
        ///holds temporary tables and indices.  If TF_Ephemeral is set
        ///then the table is stored in a file that is automatically deleted
        ///when the VDBE cursor to the table is closed.  In this case Table.tnum
        ///refers VDBE cursor number that holds the table open, not to the root
        ///page number.  Transient tables are used to hold the results of a
        ///</summary>
        ///<param name="sub">query that appears instead of a real table name in the FROM clause</param>
        ///<param name="of a SELECT statement.">of a SELECT statement.</param>
        ///<param name=""></param>

        public class Table
        {
            public string zName;

            ///
            ///<summary>
            ///Name of the table or view 
            ///</summary>

            public int iPKey;

            ///
            ///<summary>
            ///If not negative, use aCol[iPKey] as the primary key 
            ///</summary>

            public int nCol;

            ///
            ///<summary>
            ///Number of columns in this table 
            ///</summary>

            public Column[] aCol;

            ///
            ///<summary>
            ///Information about each column 
            ///</summary>

            public Index pIndex;

            ///
            ///<summary>
            ///List of SQL indexes on this table. 
            ///</summary>

            public int tnum;

            ///
            ///<summary>
            ///Root BTree node for this table (see note above) 
            ///</summary>

            public u32 nRowEst;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Estimated rows in table "> from sqlite_stat1 table </param>

            public Select pSelect;

            ///
            ///<summary>
            ///NULL for tables.  Points to definition if a view. 
            ///</summary>

            public u16 nRef;

            ///
            ///<summary>
            ///Number of pointers to this Table 
            ///</summary>

            public u8 tabFlags;

            ///
            ///<summary>
            ///Mask of TF_* values 
            ///</summary>

            public u8 keyConf;

            ///
            ///<summary>
            ///What to do in case of uniqueness conflict on iPKey 
            ///</summary>

            public FKey pFKey;

            ///
            ///<summary>
            ///Linked list of all foreign keys in this table 
            ///</summary>

            public string zColAff;

            ///
            ///<summary>
            ///String defining the affinity of each column 
            ///</summary>

#if !SQLITE_OMIT_CHECK
            public Expr pCheck;

            ///
            ///<summary>
            ///The AND of all CHECK constraints 
            ///</summary>

#endif
#if !SQLITE_OMIT_ALTERTABLE
            public int addColOffset;

            ///
            ///<summary>
            ///Offset in CREATE TABLE stmt to add a new column 
            ///</summary>

#endif
#if !SQLITE_OMIT_VIRTUALTABLE
            public VTable pVTable;

            ///
            ///<summary>
            ///List of VTable objects. 
            ///</summary>

            public int nModuleArg;

            ///
            ///<summary>
            ///Number of arguments to the module 
            ///</summary>

            public string[] azModuleArg;

            ///
            ///<summary>
            ///Text of all module args. [0] is module name 
            ///</summary>

#endif
            public Trigger pTrigger;

            ///
            ///<summary>
            ///List of SQL triggers on this table 
            ///</summary>

            public Schema pSchema;

            ///<summary>
            ///Schema that contains this table
            ///</summary>
            public Table pNextZombie;

            ///
            ///<summary>
            ///Next on the Parse.pZombieTab list 
            ///</summary>

            public Table Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Table cp = (Table)MemberwiseClone();
                    if (pIndex != null)
                        cp.pIndex = pIndex.Copy();
                    if (pSelect != null)
                        cp.pSelect = pSelect.Copy();
                    if (pTrigger != null)
                        cp.pTrigger = pTrigger.Copy();
                    if (pFKey != null)
                        cp.pFKey = pFKey.Copy();
#if !SQLITE_OMIT_CHECK
                    // Don't Clone Checks, only copy reference via Memberwise Clone above --
                    //if ( pCheck != null ) cp.pCheck = pCheck.Copy();
#endif
                    // Don't Clone Schema, only copy reference via Memberwise Clone above --
                    // if ( pSchema != null ) cp.pSchema=pSchema.Copy();
                    // Don't Clone pNextZombie, only copy reference via Memberwise Clone above --
                    // if ( pNextZombie != null ) cp.pNextZombie=pNextZombie.Copy();
                    return cp;
                }
            }
        };


        ///
        ///<summary>
        ///Allowed values for Tabe.tabFlags.
        ///
        ///</summary>

        //#define TF_Readonly        0x01    /* Read-only system table */
        //#define TF_Ephemeral       0x02    /* An ephemeral table */
        //#define TF_HasPrimaryKey   0x04    /* Table has a primary key */
        //#define TF_Autoincrement   0x08    /* Integer primary key is autoincrement */
        //#define TF_Virtual         0x10    /* Is a virtual table */
        //#define TF_NeedMetadata    0x20    /* aCol[].zType and aCol[].pColl missing */
        ///
        ///<summary>
        ///Allowed values for Tabe.tabFlags.
        ///
        ///</summary>

        private const int TF_Readonly = 0x01;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Read">only system table </param>

        private const int TF_Ephemeral = 0x02;

        ///
        ///<summary>
        ///An ephemeral table 
        ///</summary>

        private const int TF_HasPrimaryKey = 0x04;

        ///
        ///<summary>
        ///Table has a primary key 
        ///</summary>

        private const int TF_Autoincrement = 0x08;

        ///
        ///<summary>
        ///Integer primary key is autoincrement 
        ///</summary>

        private const int TF_Virtual = 0x10;

        ///
        ///<summary>
        ///Is a virtual table 
        ///</summary>

        private const int TF_NeedMetadata = 0x20;



    }
}
