using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    ///<summary>
    /// CAPI3REF: Virtual Table Indexing Information
    /// KEYWORDS: sqlite3_index_info
    ///
    /// The sqlite3_index_info structure and its substructures is used as part
    /// of the [virtual table] interface to
    /// pass information into and receive the reply from the [xBestIndex]
    /// method of a [virtual table module].  The fields under **Inputs** are the
    /// inputs to xBestIndex and are read-only.  xBestIndex inserts its
    /// results into the **Outputs** fields.
    ///
    /// ^(The aConstraint[] array records WHERE clause constraints of the form:
    ///
    /// <blockquote>column OP expr</blockquote>
    ///
    /// where OP is =, &lt;, &lt;=, &gt;, or &gt;=.)^  ^(The particular operator is
    /// stored in aConstraint[].op using one of the
    /// [SQLITE_INDEX_CONSTRAINT_EQ | SQLITE_INDEX_CONSTRAINT_ values].)^
    /// ^(The index of the column is stored in
    /// aConstraint[].iColumn.)^  ^(aConstraint[].usable is TRUE if the
    /// expr on the right-hand side can be evaluated (and thus the constraint
    /// is usable) and false if it cannot.)^
    ///
    /// ^The optimizer automatically inverts terms of the form "expr OP column"
    /// and makes other simplifications to the WHERE clause in an attempt to
    /// get as many WHERE clause terms into the form shown above as possible.
    /// ^The aConstraint[] array only reports WHERE clause terms that are
    /// relevant to the particular virtual table being queried.
    ///
    /// ^Information about the ORDER BY clause is stored in aOrderBy[].
    /// ^Each term of aOrderBy records a column of the ORDER BY clause.
    ///
    /// The [xBestIndex] method must fill aConstraintUsage[] with information
    /// about what parameters to pass to xFilter.  ^If argvIndex>0 then
    /// the right-hand side of the corresponding aConstraint[] is evaluated
    /// and becomes the argvIndex-th entry in argv.  ^(If aConstraintUsage[].omit
    /// is true, then the constraint is assumed to be fully handled by the
    /// virtual table and is not checked again by SQLite.)^
    ///
    /// ^The idxNum and idxPtr values are recorded and passed into the
    /// [xFilter] method.
    /// ^[malloc_cs.sqlite3_free()] is used to free idxPtr if and only if
    /// needToFreeIdxPtr is true.
    ///
    /// ^The orderByConsumed means that output from [xFilter]/[xNext] will occur in
    /// the correct order to satisfy the ORDER BY clause so that no separate
    /// sorting step is required.
    ///
    /// ^The estimatedCost value is an estimate of the cost of doing the
    /// particular lookup.  A full scan of a table with N entries should have
    /// a cost of N.  A binary search of a table of N entries should have a
    /// cost of approximately log(N).
    ///
    ///</summary>
    //struct sqlite3_index_info {
    //  /* Inputs */
    //  int nConstraint;           /* Number of entries in aConstraint */
    //  struct sqlite3_index_constraint {
    //     int iColumn;              /* Column on left-hand side of constraint */
    //     unsigned char op;         /* Constraint operator */
    //     unsigned char usable;     /* True if this constraint is usable */
    //     int iTermOffset;          /* Used internally - xBestIndex should ignore */
    //  } *aConstraint;            /* Table of WHERE clause constraints */
    //  int nOrderBy;              /* Number of terms in the ORDER BY clause */
    //  struct sqlite3_index_orderby {
    //     int iColumn;              /* Column number */
    //     unsigned char desc;       /* True for DESC.  False for ASC. */
    //  } *aOrderBy;               /* The ORDER BY clause */
    //  /* Outputs */
    //  struct sqlite3_index_constraint_usage {
    //    int argvIndex;           /* if >0, constraint is part of argv to xFilter */
    //    unsigned char omit;      /* Do not code a test for this constraint */
    //  } *aConstraintUsage;
    //  int idxNum;                /* Number used to identify the index */
    //  char *idxStr;              /* String, possibly obtained from sqlite3_malloc */
    //  int needToFreeIdxStr;      /* Free idxStr using malloc_cs.sqlite3_free() if true */
    //  int orderByConsumed;       /* True if output is already ordered */
    //  double estimatedCost;      /* Estimated cost of using this index */
    //};
    public class sqlite3_index_constraint
    {
        public int iColumn;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Column on left">hand side of constraint </param>

        public int op;

        ///
        ///<summary>
        ///Constraint operator 
        ///</summary>

        public bool usable;

        ///
        ///<summary>
        ///True if this constraint is usable 
        ///</summary>

        public int iTermOffset;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Used internally "> xBestIndex should ignore </param>

    }

    public class sqlite3_index_orderby
    {
        public int iColumn;

        ///
        ///<summary>
        ///Column number 
        ///</summary>

        public bool desc;
        ///
        ///<summary>
        ///True for DESC.  False for ASC. 
        ///</summary>

    }

    public class sqlite3_index_constraint_usage
    {
        public int argvIndex;

        ///
        ///<summary>
        ///if >0, constraint is part of argv to xFilter 
        ///</summary>

        public bool omit;
        ///
        ///<summary>
        ///Do not code a test for this constraint 
        ///</summary>

    }

    public class sqlite3_index_info
    {
        ///
        ///<summary>
        ///Inputs 
        ///</summary>

        public int nConstraint;

        ///
        ///<summary>
        ///Number of entries in aConstraint 
        ///</summary>

        public sqlite3_index_constraint[] aConstraint;

        ///
        ///<summary>
        ///Table of WHERE clause constraints 
        ///</summary>

        public int nOrderBy;

        ///
        ///<summary>
        ///Number of terms in the ORDER BY clause 
        ///</summary>

        public sqlite3_index_orderby[] aOrderBy;

        ///
        ///<summary>
        ///The ORDER BY clause 
        ///</summary>

        ///
        ///<summary>
        ///Outputs 
        ///</summary>

        public sqlite3_index_constraint_usage[] aConstraintUsage;

        public int idxNum;

        ///
        ///<summary>
        ///Number used to identify the index 
        ///</summary>

        public string idxStr;

        ///
        ///<summary>
        ///String, possibly obtained from malloc_cs.sqlite3Malloc 
        ///</summary>

        public int needToFreeIdxStr;

        ///
        ///<summary>
        ///Free idxStr using sqlite3DbFree(db,) if true 
        ///</summary>

        public bool orderByConsumed;

        ///
        ///<summary>
        ///True if output is already ordered 
        ///</summary>

        public double estimatedCost;
        ///
        ///<summary>
        ///Estimated cost of using this index 
        ///</summary>

    }
}
