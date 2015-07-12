using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///
    ///<summary>
    ///2001 September 15
    ///
    ///The author disclaims copyright to this source code.  In place of
    ///a legal notice, here is a blessing:
    ///
    ///May you do good and not evil.
    ///May you find forgiveness for yourself and forgive others.
    ///May you share freely, never taking more than you give.
    ///
    ///
    ///Header file for the Virtual DataBase Engine (VDBE)
    ///
    ///This header defines the interface to the virtual database engine
    ///or VDBE.  The VDBE implements an abstract machine that runs a
    ///simple program to access and modify the underlying database.
    ///
    ///</summary>
    ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
    ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
    ///<param name=""></param>
    ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
    ///<param name=""></param>
    ///<param name=""></param>
    ///<param name=""></param>

    //#if !_SQLITE_VDBE_H_
    //#define _SQLITE_VDBE_H_
    //#include <stdio.h>
    ///
    ///<summary>
    ///A single VDBE is an opaque structure named "Vdbe".  Only routines
    ///in the source file sqliteVdbe.c are allowed to see the insides
    ///of this structure.
    ///
    ///</summary>

    //typedef struct Vdbe Vdbe;
    ///<summary>
    /// The names of the following types declared in vdbeInt.h are required
    /// for the VdbeOp definition.
    ///
    ///</summary>
    //typedef struct VdbeFunc VdbeFunc;
    //typedef struct Mem Mem;
    //typedef struct SubProgram SubProgram;
    //typedef struct VdbeOp VdbeOp;
    ///<summary>
    /// A sub-routine used to implement a trigger program.
    ///
    ///</summary>
    public class SubProgram : ILinkedListNode<SubProgram>
    {
        public Engine.VdbeOp[] aOp;// { get; set; }

        ///
        ///<summary>
        ///</summary>
        ///<param name="Array of opcodes for sub">program </param>

        public int nOp;// { get; set; }

        ///
        ///<summary>
        ///Elements in aOp[] 
        ///</summary>

        public int nMem { get; set; }

        ///
        ///<summary>
        ///Number of memory cells required 
        ///</summary>


        ///<summary>
        ///Number of cursors required 
        ///</summary>
        public int nCsr { get; set; }

        ///<summary>
        ///id that may be used to recursive triggers 
        ///</summary>
        public int token { get; set; }


        ///<summary>
        ///</summary>
        ///<param name="Next sub">program already visited </param>
        public SubProgram pNext { get; set; }
        
    };

}
