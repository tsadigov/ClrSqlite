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




        ///
        ///<summary>
        ///An instance of the following structure holds information about a
        ///single index record that has already been parsed out into individual
        ///values.
        ///
        ///A record is an object that contains one or more fields of data.
        ///Records are used to store the content of a table row and to store
        ///the key of an index.  A blob encoding of a record is created by
        ///the OP_MakeRecord opcode of the VDBE and is disassembled by the
        ///OP_Column opcode.
        ///
        ///This structure holds a record that has already been disassembled
        ///into its constituent fields.
        ///
        ///</summary>
        public class UnpackedRecord
        {
            public KeyInfo pKeyInfo;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Collation and sort">order information </param>
            public u16 nField;
            ///
            ///<summary>
            ///Number of entries in apMem[] 
            ///</summary>
            public UnpackedRecordFlags flags;
            ///
            ///<summary>
            ///Boolean settings.  UNPACKED_... below 
            ///</summary>
            public i64 rowid;
            ///
            ///<summary>
            ///Used by UNPACKED_PREFIX_SEARCH 
            ///</summary>
            public Community.CsharpSqlite.Mem[] aMem;
            ///
            ///<summary>
            ///Values 
            ///</summary>
        };


        [Flags]
        public enum UnpackedRecordFlags:ushort { 
        ///
        ///<summary>
        ///Allowed values of UnpackedRecord.flags
        ///
        ///</summary>
        //#define UnpackedRecordFlags.UNPACKED_NEED_FREE     0x0001  /* Memory is from malloc_cs.sqlite3Malloc() */
        //#define UnpackedRecordFlags.UNPACKED_NEED_DESTROY  0x0002  /* apMem[]s should all be destroyed */
        //#define UnpackedRecordFlags.UNPACKED_IGNORE_ROWID  0x0004  /* Ignore trailing rowid on key1 */
        //#define UnpackedRecordFlags.UNPACKED_INCRKEY       0x0008  /* Make this key an epsilon larger */
        //#define UnpackedRecordFlags.UNPACKED_PREFIX_MATCH  0x0010  /* A prefix match is considered OK */
        //#define UnpackedRecordFlags.UNPACKED_PREFIX_SEARCH 0x0020  /* A prefix match is considered OK */
        UNPACKED_NEED_FREE = 0x0001,
        ///
        ///<summary>
        ///Memory is from malloc_cs.sqlite3Malloc() 
        ///</summary>
        UNPACKED_NEED_DESTROY = 0x0002,
        ///
        ///<summary>
        ///apMem[]s should all be destroyed 
        ///</summary>
        UNPACKED_IGNORE_ROWID = 0x0004,
        ///
        ///<summary>
        ///Ignore trailing rowid on key1 
        ///</summary>
        UNPACKED_INCRKEY = 0x0008,
        ///
        ///<summary>
        ///Make this key an epsilon larger 
        ///</summary>
        UNPACKED_PREFIX_MATCH = 0x0010,
        ///
        ///<summary>
        ///A prefix match is considered OK 
        ///</summary>
        UNPACKED_PREFIX_SEARCH = 0x0020




    }
}
