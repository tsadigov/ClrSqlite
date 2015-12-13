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

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Engine;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite.Metadata
{
        ///<summary>
        /// At least one instance of the following structure is created for each
        /// trigger that may be fired while parsing an INSERT, UPDATE or DELETE
        /// statement. All such objects are stored in the linked list headed at
        /// Parse.pTriggerPrg and deleted once statement compilation has been
        /// completed.
        ///
        /// A Vdbe sub-program that implements the body and WHEN clause of trigger
        /// TriggerPrg.pTrigger, assuming a default ON CONFLICT clause of
        /// TriggerPrg.orconf, is stored in the TriggerPrg.pProgram variable.
        /// The Parse.pTriggerPrg list never contains two entries with the same
        /// values for both pTrigger and orconf.
        ///
        /// The TriggerPrg.aColmask[0] variable is set to a mask of old.* columns
        /// accessed (or set to 0 for triggers fired as a result of INSERT
        /// statements). Similarly, the TriggerPrg.aColmask[1] variable is set to
        /// a mask of new.* columns used by the program.
        ///</summary>
        public class TriggerPrg : ILinkedListNode<TriggerPrg>
        {
            ///<summary>
            ///Trigger this program was coded from 
            ///</summary>
            public Trigger pTrigger;
            
            ///<summary>
            ///Default ON CONFLICT policy 
            ///</summary>
            public OnConstraintError orconf;


            ///<summary>
            ///Program implementing pTrigger/orconf 
            ///</summary>
            public SubProgram pProgram;


            ///<summary>
            ///Masks of old.*, new.* columns accessed 
            ///</summary>
            public u32[] aColmask = new u32[2];


            ///<summary>
            ///Next entry in Parse.pTriggerPrg list 
            ///</summary>
            public TriggerPrg pNext { get; set; }
            
        };


        ///<summary>
        /// A trigger is either a BEFORE or an AFTER trigger.  The following constants
        /// determine which.
        ///
        /// If there are multiple triggers, you might of some BEFORE and some AFTER.
        /// In that cases, the constants below can be ORed together.
        ///
        ///</summary>
        public enum TriggerType : byte
        {
            TRIGGER_BEFORE = 1,

            //#define TriggerType.TRIGGER_BEFORE  1
            TRIGGER_AFTER = 2
        }

        //#define TriggerType.TRIGGER_AFTER   2
        ///
        ///<summary>
        ///An instance of struct TriggerStep is used to store a single SQL statement
        ///that is a part of a trigger-program.
        ///Instances of struct TriggerStep are stored in a singly linked list (linked">
        ///using the "pNext" member) referenced by the "step_list" member of the
        ///associated struct Trigger instance. The first element of the linked list is
        ///the first step of the trigger-program.        
        ///The "op" member indicates whether this is a "DELETE", "INSERT", "UPDATE" or">The "op" member indicates whether this is a "DELETE", "INSERT", "UPDATE" or</param>
        ///"SELECT" statement. The meanings of the other members is determined by the
        ///"value of "op" as follows:"
        ///
        ///(op == Sqlite3.TK_INSERT)</param>
        ///orconf    ">> stores the ON CONFLICT algorithm</param>
        ///pSelect   ">> If this is an INSERT INTO ... SELECT ... statement, then</param>
        ///this stores a pointer to the SELECT statement. Otherwise NULL.</param>
        ///target    ">> A token holding the quoted name of the table to insert into.</param>
        ///pExprList ">> If this is an INSERT INTO ... VALUES ... statement, then</param>
        ///this stores values to be inserted. Otherwise NULL.">this stores values to be inserted. Otherwise NULL.</param>
        ///pIdList   ">names>) VALUES ...</param>
        ///statement, then this stores the column-names to be</param>
        ///inserted into.">inserted into.</param>
        ///
        ///(op == Sqlite3.TK_DELETE)</param>
        ///target    ">> A token holding the quoted name of the table to delete from.</param>
        ///pWhere    ">> The WHERE clause of the DELETE statement if one is specified.</param>
        ///Otherwise NULL.">Otherwise NULL.</param>
        ///
        ///(op == Sqlite3.TK_UPDATE)</param>
        ///target    ">> A token holding the quoted name of the table to update rows of.</param>
        ///pWhere    ">> The WHERE clause of the UPDATE statement if one is specified.</param>
        ///Otherwise NULL.</param>
        ///pExprList ">> A list of the columns to update and the expressions to update</param>
        ///them to. See sqlite3Update() documentation of "pChanges"</param>
        ///argument.</param>
        ///</summary>
        

        public class TriggerStep : ILinkedListNode<TriggerStep>
        {
            ///<summary>
            ///One of Sqlite3.TK_DELETE, Sqlite3.TK_UPDATE, Sqlite3.TK_INSERT, Sqlite3.TK_SELECT 
            ///</summary>
            public u8 op;

            ///<summary>
            ///OE_Rollback etc. 
            ///</summary>
            public OnConstraintError orconf;


            ///<summary>
            ///The trigger that this step is a part of 
            ///</summary>
            public Trigger pTrig;


            ///<summary>
            ///SELECT statment or RHS of INSERT INTO .. SELECT ... 
            ///</summary>
            public Select pSelect;


            ///<summary>
            ///Target table for DELETE, UPDATE, INSERT 
            ///</summary>
            public Token target;


            ///<summary>
            ///The WHERE clause for DELETE or UPDATE steps 
            ///</summary>
            public Expr pWhere;


            ///<summary>
            ///SET clause for UPDATE.  VALUES clause for INSERT 
            ///</summary>
            public ExprList pExprList;

            ///<summary>
            ///Column names for INSERT 
            ///</summary>
            public IdList pIdList;


            ///<param name="Next in the link">list </param>
            public TriggerStep pNext { get; set; }


            ///<summary>
            ///Last element in link-list. Valid for 1st elem only
            ///</summary>
            public TriggerStep pLast;

            public TriggerStep()
            {
                target = new Token();
            }

            public TriggerStep Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    TriggerStep cp = (TriggerStep)MemberwiseClone();
                    return cp;
                }
            }
        };


        ///
        ///<summary>
        ///Each trigger present in the database schema is stored as an instance of
        ///struct Trigger.
        ///
        ///Pointers to instances of struct Trigger are stored in two ways.
        ///1. In the "trigHash" hash table (part of the sqlite3* that represents the
        ///database). This allows Trigger structures to be retrieved by name.
        ///2. All triggers associated with a single table form a linked list, using the
        ///pNext member of struct Trigger. A pointer to the first element of the
        ///linked list is stored as the "pTrigger" member of the associated
        ///struct Table.
        ///
        ///The "step_list" member points to the first element of a linked list
        ///containing the SQL statements specified as the trigger program.
        ///
        ///</summary>

        public class Trigger:ILinkedListNode<Trigger>
        {
            ///<summary>
            ///The name of the trigger                        
            ///</summary>
            public string zName;

            ///<summary>
            ///The table or view to which the trigger applies 
            ///</summary>
            public string table;

            ///<summary>
            ///One of Sqlite3.TK_DELETE, Sqlite3.TK_UPDATE, Sqlite3.TK_INSERT         
            ///</summary>
            public u8 op;


            ///<summary>
            ///One of TriggerType.TRIGGER_BEFORE, TriggerType.TRIGGER_AFTER 
            ///</summary>
            public TriggerType tr_tm;


            ///<summary>
            ///The WHEN clause of the expression (may be NULL) 
            ///</summary>
            public Expr pWhen;


            ///If this is an UPDATE OF <column-list> trigger,
            ///the column list is stored here 
            public IdList pColumns;


            ///<summary>
            ///Schema containing the trigger 
            ///</summary>
            public Schema pSchema;



            ///<summary>
            ///Schema containing the table 
            ///</summary>
            public Schema pTabSchema;


            ///<summary>
            ///Link list of trigger program steps
            ///</summary>
            public TriggerStep step_list;

            ///<summary>
            ///Next trigger associated with the table 
            ///</summary>
            public Trigger pNext { get; set; }

            
            public Trigger Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Trigger cp = (Trigger)MemberwiseClone();
                    if (pWhen != null)
                        cp.pWhen = pWhen.Clone();
                    if (pColumns != null)
                        cp.pColumns = pColumns.Copy();
                    if (pSchema != null)
                        cp.pSchema = pSchema.Copy();
                    if (pTabSchema != null)
                        cp.pTabSchema = pTabSchema.Copy();
                    if (step_list != null)
                        cp.step_list = step_list.Copy();
                    if (pNext != null)
                        cp.pNext = pNext.Copy();
                    return cp;
                }
            }
        };

}
