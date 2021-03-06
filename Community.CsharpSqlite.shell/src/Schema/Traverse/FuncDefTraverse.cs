﻿using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;
using i16 = System.Int16;

namespace Community.CsharpSqlite.Metadata.Traverse
{
    using Utils;
    public static class FuncDefTraverse
    {
        ///<summary>
        ///During the search for the best function definition, this procedure
        /// is called to test how well the function passed as the first argument
        /// matches the request for a function with nArg arguments in a system
        /// that uses encoding enc. The value returned indicates how well the
        /// request is matched. A higher value indicates a better match.
        ///
        /// The returned value is always between 0 and 6, as follows:
        ///
        /// 0: Not a match, or if nArg<0 and the function is has no implementation.
        /// 1: A variable arguments function that prefers UTF-8 when a UTF-16
        ///    encoding is requested, or vice versa.
        /// 2: A variable arguments function that uses UTF-16BE when UTF-16LE is
        ///    requested, or vice versa.
        /// 3: A variable arguments function using the same text encoding.
        /// 4: A function with the exact number of arguments requested that
        ///    prefers UTF-8 when a UTF-16 encoding is requested, or vice versa.
        /// 5: A function with the exact number of arguments requested that
        ///    prefers UTF-16LE when UTF-16BE is requested, or vice versa.
        /// 6: An exact match.
        ///
        ///
        ///</summary>
        static int matchQuality(FuncDef p, int nArg, SqliteEncoding enc)
        {
            int match = 0;
            if (p.nArg == -1 || p.nArg == nArg || (nArg == -1 && (p.xFunc != null || p.xStep != null)))
            {
                match = 1;
                if (p.nArg == nArg || nArg == -1)
                {
                    match = 4;
                }
                if (enc == p.iPrefEnc)
                {
                    match += 2;
                }
                else
                    if ((enc == SqliteEncoding.UTF16LE && p.iPrefEnc == SqliteEncoding.UTF16BE) || (enc == SqliteEncoding.UTF16BE && p.iPrefEnc == SqliteEncoding.UTF16LE))
                    {
                        match += 1;
                    }
            }
            return match;
        }
        ///<summary>
        /// Search a FuncDefHash for a function with the given name.  Return
        /// a pointer to the matching FuncDef if found, or 0 if there is no match.
        ///
        ///</summary>
        static FuncDef functionSearch(
            FuncDefHash pHash,///Hash table to search 
            int h,///Hash of the name 
            string zFunc,///Name of function 
            int nFunc///Number of bytes in zFunc 
        )
        {
            return pHash.a[h]
                .path(f => f.pHash)
                .FirstOrDefault(
                    f => f.zName.Length == nFunc && f.zName.StartsWith(zFunc, StringComparison.InvariantCultureIgnoreCase)
                 );
        }
        ///<summary>
        /// Insert a new FuncDef into a FuncDefHash hash table.
        ///
        ///</summary>
        public static void sqlite3FuncDefInsert(FuncDefHash pHash,///
            ///The hash table into which to insert 
        FuncDef pDef///
            ///The function definition to insert 
        )
        {
            int nName = StringExtensions.Strlen30(pDef.zName);
            u8 c1 = (u8)pDef.zName[0];
            int h = (_Custom.sqlite3UpperToLower[c1] + nName) % Sqlite3.ArraySize(pHash.a);
            var pOther = functionSearch(pHash, h, pDef.zName, nName);
            if (pOther != null)
            {
                Debug.Assert(pOther != pDef && pOther.pNext != pDef);
                pDef.pNext = pOther.pNext;
                pOther.pNext = pDef;
            }
            else
            {
                pDef.pNext = null;
                pDef.pHash = pHash.a[h];
                pHash.a[h] = pDef;
            }
        }


        struct FuncScore {
            public FuncDef Func;
            public int Score;
        }
        ///<summary>
        /// Locate a user function given a name, a number of arguments and a flag
        /// indicating whether the function prefers UTF-16 over UTF-8.  Return a
        /// pointer to the FuncDef structure that defines that function, or return
        /// NULL if the function does not exist.
        ///
        /// If the createFlag argument is true, then a new (blank) FuncDef
        /// structure is created and liked into the "db" structure if a
        /// no matching function previously existed.  When createFlag is true
        /// and the nArg parameter is -1, then only a function that accepts
        /// any number of arguments will be returned.
        ///
        /// If createFlag is false and nArg is -1, then the first valid
        /// function found is returned.  A function is valid if either xFunc
        /// or xStep is non-zero.
        ///
        /// If createFlag is false, then a function with the required name and
        /// number of arguments may be returned even if the eTextRep flag does not
        /// match that requested.
        ///
        ///</summary>
        public static FuncDef FindFunction(
            Connection db,///An open database 
            string zName,///<param name="Name of the function.  Not null">terminated </param>
            int nName,///Number of characters in the name 
            int nArg,///<param name="Number of arguments.  ">1 means any number </param>
            SqliteEncoding enc,///Preferred text encoding 
            u8 createFlag///Create new entry if true and does not otherwise exist 
        )
        {   
            Debug.Assert(enc == SqliteEncoding.UTF8 || enc == SqliteEncoding.UTF16LE || enc == SqliteEncoding.UTF16BE);
            var h = (_Custom.sqlite3UpperToLower[(u8)zName[0]] + nName) % Sqlite3.ArraySize(db.aFunc.a);///Hash value 
            ///First search for a match amongst the application-defined functions.
            
            var func = functionSearch(db.aFunc, h, zName, nName);///Iterator variable 
            var winner=func.linkedList()
                .Select(f => new FuncScore { Score = matchQuality(f, nArg, enc), Func = f })
                .WinnerBy(r=>r.Score);


            /* If no match is found, search the built-in functions.
            **
            ** If the SQLITE_PreferBuiltin flag is set, then search the built-in
            ** functions even if a prior app-defined function was found.  And give
            ** priority to built-in functions.
            **
            ** Except, if createFlag is true, that means that we are trying to
            ** install a new function.  Whatever FuncDef structure is returned it will
            ** have fields overwritten with new information appropriate for the
            ** new function.  But the FuncDefs for built-in functions are read-only.
            ** So we must not search for built-ins when creating a new function.
            */
            if (0 == createFlag && (winner.Func == null || (db.flags & SqliteFlags.SQLITE_PreferBuiltin) != 0))
            {
#if SQLITE_OMIT_WSD
																																																																																				FuncDefHash pHash = GLOBAL( FuncDefHash, sqlite3GlobalFunctions );
#else
                FuncDefHash pHash = Sqlite3.sqlite3GlobalFunctions;
#endif
                winner.Score = 0;
                func = functionSearch(pHash, h, zName, nName);
                winner = func.linkedList()
                    .Select(f => new FuncScore { Score = matchQuality(f, nArg, enc), Func = f })
                    .WinnerBy(r => r.Score);
            }
            ///
            ///<summary>
            ///If the createFlag parameter is true and the search did not reveal an
            ///exact match for the name, number of arguments and encoding, then add a
            ///new entry to the hash table and return it.
            ///
            ///</summary>
            if (createFlag != 0 && (winner.Score < 6 || winner.Func.nArg != nArg) )
            {
                winner.Func = new FuncDef() {
                    nArg = (i16)nArg,
                    iPrefEnc = enc,
                    zName = zName
                };               
                sqlite3FuncDefInsert(db.aFunc, winner.Func);
            }
            if (winner.Func != null && (winner.Func.xStep != null || winner.Func.xFunc != null || createFlag != 0))
            {
                return winner.Func;
            }
            return null;
        }
		
    }
}
