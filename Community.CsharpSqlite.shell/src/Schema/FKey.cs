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
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Ast;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
using Community.CsharpSqlite.Metadata;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite.Metadata
{
    
        ///
        ///<summary>
        ///Each foreign key constraint is an instance of the following structure.
        ///
        ///A foreign key is associated with two tables.  The "from" table is
        ///the table that contains the REFERENCES clause that creates the foreign
        ///key.  The "to" table is the table that is named in the REFERENCES clause.
        ///Consider this example:
        ///
        ///CREATE TABLE ex1(
        ///a INTEGER PRIMARY KEY,
        ///b INTEGER CONSTRAINT fk1 REFERENCES ex2(x)
        ///);
        ///
        ///</summary>
        ///<param name="For foreign key "fk1", the from">table is "ex2".</param>
        ///<param name=""></param>
        ///<param name="Each REFERENCES clause generates an instance of the following structure">Each REFERENCES clause generates an instance of the following structure</param>
        ///<param name="which is attached to the from">table need not exist when</param>
        ///<param name="the from">table is not checked.</param>
        public class FKey: ILinkedListNode<FKey>
        {
            public Table pFrom;
            ///Table containing the REFERENCES clause (aka: Child) 
            public FKey pNextFrom;
            ///Next foreign key in pFrom 
            public string zTo;
            ///Name of table that the key points to (aka: Parent) 
            public FKey pNextTo;
            ///Next foreign key on table named zTo 
            public FKey pPrevTo;
            ///Previous foreign key on table named zTo 
            public int nCol;
            ///Number of columns in this key 
            ///
            ///<param name="EV: R">21917 </param>
            public u8 isDeferred;
            ///True if constraint checking is deferred till COMMIT 
            public OnConstraintError[] aAction = new OnConstraintError[2];
            ///ON DELETE and ON UPDATE actions, respectively
            public Trigger[] apTrigger = new Trigger[2];
            ///Triggers for aAction[] actions
            public class sColMap
            {
                ///Mapping of columns in pFrom to columns in zTo 
                public int iFrom;
                ///Index of column in pFrom 
                public string zCol;
                ///Name of column in zTo.  If 0 use PRIMARY KEY 
            };

            public sColMap[] aCol;

        public FKey pNext
        {
            get
            {
                return ((ILinkedListNode<FKey>)pNextFrom).pNext;
            }

            set
            {
                ((ILinkedListNode<FKey>)pNextFrom).pNext = value;
            }
        }

        ///One entry for each of nCol column s 
        public FKey Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    FKey cp = (FKey)MemberwiseClone();
                    return cp;
                }
            }
        };

    }
