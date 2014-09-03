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
        public class TriggerPrg
        {
            public Trigger pTrigger;

            ///
            ///<summary>
            ///Trigger this program was coded from 
            ///</summary>

            public int orconf;

            ///
            ///<summary>
            ///Default ON CONFLICT policy 
            ///</summary>

            public SubProgram pProgram;

            ///
            ///<summary>
            ///Program implementing pTrigger/orconf 
            ///</summary>

            public u32[] aColmask = new u32[2];

            ///
            ///<summary>
            ///Masks of old.*, new.* columns accessed 
            ///</summary>

            public TriggerPrg pNext;
            ///
            ///<summary>
            ///Next entry in Parse.pTriggerPrg list 
            ///</summary>

        };


        ///<summary>
        /// A trigger is either a BEFORE or an AFTER trigger.  The following constants
        /// determine which.
        ///
        /// If there are multiple triggers, you might of some BEFORE and some AFTER.
        /// In that cases, the constants below can be ORed together.
        ///
        ///</summary>
        private const u8 TRIGGER_BEFORE = 1;

        //#define TRIGGER_BEFORE  1
        private const u8 TRIGGER_AFTER = 2;

        //#define TRIGGER_AFTER   2
        ///
        ///<summary>
        ///An instance of struct TriggerStep is used to store a single SQL statement
        ///</summary>
        ///<param name="that is a part of a trigger">program.</param>
        ///<param name=""></param>
        ///<param name="Instances of struct TriggerStep are stored in a singly linked list (linked">Instances of struct TriggerStep are stored in a singly linked list (linked</param>
        ///<param name="using the "pNext" member) referenced by the "step_list" member of the">using the "pNext" member) referenced by the "step_list" member of the</param>
        ///<param name="associated struct Trigger instance. The first element of the linked list is">associated struct Trigger instance. The first element of the linked list is</param>
        ///<param name="the first step of the trigger">program.</param>
        ///<param name=""></param>
        ///<param name="The "op" member indicates whether this is a "DELETE", "INSERT", "UPDATE" or">The "op" member indicates whether this is a "DELETE", "INSERT", "UPDATE" or</param>
        ///<param name=""SELECT" statement. The meanings of the other members is determined by the">"SELECT" statement. The meanings of the other members is determined by the</param>
        ///<param name="value of "op" as follows:">value of "op" as follows:</param>
        ///<param name=""></param>
        ///<param name="(op == TK_INSERT)">(op == TK_INSERT)</param>
        ///<param name="orconf    ">> stores the ON CONFLICT algorithm</param>
        ///<param name="pSelect   ">> If this is an INSERT INTO ... SELECT ... statement, then</param>
        ///<param name="this stores a pointer to the SELECT statement. Otherwise NULL.">this stores a pointer to the SELECT statement. Otherwise NULL.</param>
        ///<param name="target    ">> A token holding the quoted name of the table to insert into.</param>
        ///<param name="pExprList ">> If this is an INSERT INTO ... VALUES ... statement, then</param>
        ///<param name="this stores values to be inserted. Otherwise NULL.">this stores values to be inserted. Otherwise NULL.</param>
        ///<param name="pIdList   ">names>) VALUES ...</param>
        ///<param name="statement, then this stores the column">names to be</param>
        ///<param name="inserted into.">inserted into.</param>
        ///<param name=""></param>
        ///<param name="(op == TK_DELETE)">(op == TK_DELETE)</param>
        ///<param name="target    ">> A token holding the quoted name of the table to delete from.</param>
        ///<param name="pWhere    ">> The WHERE clause of the DELETE statement if one is specified.</param>
        ///<param name="Otherwise NULL.">Otherwise NULL.</param>
        ///<param name=""></param>
        ///<param name="(op == TK_UPDATE)">(op == TK_UPDATE)</param>
        ///<param name="target    ">> A token holding the quoted name of the table to update rows of.</param>
        ///<param name="pWhere    ">> The WHERE clause of the UPDATE statement if one is specified.</param>
        ///<param name="Otherwise NULL.">Otherwise NULL.</param>
        ///<param name="pExprList ">> A list of the columns to update and the expressions to update</param>
        ///<param name="them to. See sqlite3Update() documentation of "pChanges"">them to. See sqlite3Update() documentation of "pChanges"</param>
        ///<param name="argument.">argument.</param>
        ///<param name=""></param>
        ///<param name=""></param>

        public class TriggerStep
        {
            public u8 op;

            ///
            ///<summary>
            ///One of TK_DELETE, TK_UPDATE, TK_INSERT, TK_SELECT 
            ///</summary>

            public u8 orconf;

            ///
            ///<summary>
            ///OE_Rollback etc. 
            ///</summary>

            public Trigger pTrig;

            ///
            ///<summary>
            ///The trigger that this step is a part of 
            ///</summary>

            public Select pSelect;

            ///
            ///<summary>
            ///SELECT statment or RHS of INSERT INTO .. SELECT ... 
            ///</summary>

            public Token target;

            ///
            ///<summary>
            ///Target table for DELETE, UPDATE, INSERT 
            ///</summary>

            public Expr pWhere;

            ///
            ///<summary>
            ///The WHERE clause for DELETE or UPDATE steps 
            ///</summary>

            public ExprList pExprList;

            ///
            ///<summary>
            ///SET clause for UPDATE.  VALUES clause for INSERT 
            ///</summary>

            public IdList pIdList;

            ///
            ///<summary>
            ///Column names for INSERT 
            ///</summary>

            public TriggerStep pNext;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Next in the link">list </param>

            public TriggerStep pLast;

            ///<summary>
            ///Last element in link-list. Valid for 1st elem only
            ///</summary>
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

        public class Trigger
        {
            public string zName;

            ///
            ///<summary>
            ///The name of the trigger                        
            ///</summary>

            public string table;

            ///
            ///<summary>
            ///The table or view to which the trigger applies 
            ///</summary>

            public u8 op;

            ///
            ///<summary>
            ///One of TK_DELETE, TK_UPDATE, TK_INSERT         
            ///</summary>

            public u8 tr_tm;

            ///
            ///<summary>
            ///One of TRIGGER_BEFORE, TRIGGER_AFTER 
            ///</summary>

            public Expr pWhen;

            ///
            ///<summary>
            ///The WHEN clause of the expression (may be NULL) 
            ///</summary>

            public IdList pColumns;

            ///
            ///<summary>
            ///</summary>
            ///<param name="If this is an UPDATE OF <column">list> trigger,</param>
            ///<param name="the <column">list> is stored here </param>

            public Schema pSchema;

            ///
            ///<summary>
            ///Schema containing the trigger 
            ///</summary>

            public Schema pTabSchema;

            ///
            ///<summary>
            ///Schema containing the table 
            ///</summary>

            public TriggerStep step_list;

            ///<summary>
            ///Link list of trigger program steps
            ///</summary>
            public Trigger pNext;

            ///
            ///<summary>
            ///Next trigger associated with the table 
            ///</summary>

            public Trigger Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Trigger cp = (Trigger)MemberwiseClone();
                    if (pWhen != null)
                        cp.pWhen = pWhen.Copy();
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
}
