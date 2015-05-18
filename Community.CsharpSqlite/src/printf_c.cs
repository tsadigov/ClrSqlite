using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Community.CsharpSqlite
{
	using etByte = System.Boolean;
	using i64 = System.Int64;
	using u64 = System.UInt64;
	using LONGDOUBLE_TYPE = System.Double;
	using sqlite_u3264 = System.UInt64;
	using va_list = System.Object;
    using _Custom = Sqlite3._Custom;
    using Community.CsharpSqlite.Ast;

    public partial class Sqlite3
    {
    
    }

























    public class io
    {
        ///
        ///<summary>
        ///The "printf" code that follows dates from the 1980's.  It is in
        ///the public domain.  The original comments are included here for
        ///</summary>
        ///<param name="completeness.  They are very out">date but might be useful as</param>
        ///<param name="an historical reference.  Most of the "enhancements" have been backed">an historical reference.  Most of the "enhancements" have been backed</param>
        ///<param name="out so that the functionality is now the same as standard printf().">out so that the functionality is now the same as standard printf().</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name="The following modules is an enhanced replacement for the "printf" subroutines">The following modules is an enhanced replacement for the "printf" subroutines</param>
        ///<param name="found in the standard C library.  The following enhancements are">found in the standard C library.  The following enhancements are</param>
        ///<param name="supported:">supported:</param>
        ///<param name=""></param>
        ///<param name="+  Additional functions.  The standard set of "printf" functions">+  Additional functions.  The standard set of "printf" functions</param>
        ///<param name="includes printf, fprintf, sprintf, vprintf, vfprintf, and">includes printf, fprintf, sprintf, vprintf, vfprintf, and</param>
        ///<param name="vsprintf.  This module adds the following:">vsprintf.  This module adds the following:</param>
        ///<param name=""></param>
        ///<param name="snprintf "> Works like sprintf, but has an extra argument</param>
        ///<param name="which is the size of the buffer written to.">which is the size of the buffer written to.</param>
        ///<param name=""></param>
        ///<param name="mprintf ">  Similar to sprintf.  Writes output to memory</param>
        ///<param name="obtained from malloc.">obtained from malloc.</param>
        ///<param name=""></param>
        ///<param name="xprintf ">  Calls a function to dispose of output.</param>
        ///<param name=""></param>
        ///<param name="nprintf ">  No output, but returns the number of characters</param>
        ///<param name="that would have been output by printf.">that would have been output by printf.</param>
        ///<param name=""></param>
        ///<param name="A v"> version (ex: vsnprintf) of every function is also</param>
        ///<param name="supplied.">supplied.</param>
        ///<param name=""></param>
        ///<param name="+  A few extensions to the formatting notation are supported:">+  A few extensions to the formatting notation are supported:</param>
        ///<param name=""></param>
        ///<param name="The "=" flag (similar to "">") causes the output to be</param>
        ///<param name="be centered in the appropriately sized field.">be centered in the appropriately sized field.</param>
        ///<param name=""></param>
        ///<param name="The %b field outputs an integer in binary notation.">The %b field outputs an integer in binary notation.</param>
        ///<param name=""></param>
        ///<param name="The %c field now accepts a precision.  The character output">The %c field now accepts a precision.  The character output</param>
        ///<param name="is repeated by the number of times the precision specifies.">is repeated by the number of times the precision specifies.</param>
        ///<param name=""></param>
        ///<param name="The %' field works like %c, but takes as its character the">The %' field works like %c, but takes as its character the</param>
        ///<param name="next character of the format string, instead of the next">next character of the format string, instead of the next</param>
        ///<param name="argument.  For example,  printf("%.78'">")  prints 78 minus</param>
        ///<param name="signs, the same as  printf("%.78c",'">').</param>
        ///<param name=""></param>
        ///<param name="+  When compiled using GCC on a SPARC, this version of printf is">+  When compiled using GCC on a SPARC, this version of printf is</param>
        ///<param name="faster than the library printf for SUN OS 4.1.">faster than the library printf for SUN OS 4.1.</param>
        ///<param name=""></param>
        ///<param name="+  All functions are fully reentrant.">+  All functions are fully reentrant.</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
        ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
        ///<param name=""></param>
        ///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>

        //#include "sqliteInt.h"
        ///
        ///<summary>
        ///Conversion types fall into various categories as defined by the
        ///following enumeration.
        ///
        ///</summary>

        //#define etRADIX       1 /* Integer types.  %d, %x, %o, and so forth */
        //#define etFLOAT       2 /* Floating point.  %f */
        //#define etEXP         3 /* Exponentional notation. %e and %E */
        //#define etGENERIC     4 /* Floating or exponential, depending on exponent. %g */
        //#define etSIZE        5 /* Return number of characters processed so far. %n */
        //#define etSTRING      6 /* Strings. %s */
        //#define etDYNSTRING   7 /* Dynamically allocated strings. %z */
        //#define etPERCENT     8 /* Percent symbol. %% */
        //#define etCHARX       9 /* Characters. %c */
        ///* The rest are extensions, not normally found in printf() */
        //#define etSQLESCAPE  10 /* Strings with '\'' doubled.  %q */
        //#define etSQLESCAPE2 11 /* Strings with '\'' doubled and enclosed in '',
        //                          NULL pointers replaced by SQL NULL.  %Q */
        //#define etTOKEN      12 /* a pointer to a Token structure */
        //#define etSRCLIST    13 /* a pointer to a SrcList */
        //#define etPOINTER    14 /* The %p conversion */
        //#define etSQLESCAPE3 15 /* %w -> Strings with '\"' doubled */
        //#define etORDINAL    16 /* %r -> 1st, 2nd, 3rd, 4th, etc.  English only */
        //#define etINVALID     0 /* Any unrecognized conversion type */
        const int etRADIX = 1;

        ///
        ///<summary>
        ///Integer types.  %d, %x, %o, and so forth 
        ///</summary>

        const int etFLOAT = 2;

        ///
        ///<summary>
        ///Floating point.  %f 
        ///</summary>

        const int etEXP = 3;

        ///
        ///<summary>
        ///Exponentional notation. %e and %E 
        ///</summary>

        const int etGENERIC = 4;

        ///
        ///<summary>
        ///Floating or exponential, depending on exponent. %g 
        ///</summary>

        const int etSIZE = 5;

        ///
        ///<summary>
        ///Return number of characters processed so far. %n 
        ///</summary>

        const int etSTRING = 6;

        ///
        ///<summary>
        ///Strings. %s 
        ///</summary>

        const int etDYNSTRING = 7;

        ///
        ///<summary>
        ///Dynamically allocated strings. %z 
        ///</summary>

        const int etPERCENT = 8;

        ///
        ///<summary>
        ///Percent symbol. %% 
        ///</summary>

        const int etCHARX = 9;

        ///
        ///<summary>
        ///Characters. %c 
        ///</summary>

        ///
        ///<summary>
        ///The rest are extensions, not normally found in printf() 
        ///</summary>

        const int etSQLESCAPE = 10;

        ///
        ///<summary>
        ///Strings with '\'' doubled.  %q 
        ///</summary>

        const int etSQLESCAPE2 = 11;

        ///
        ///<summary>
        ///Strings with '\'' doubled and enclosed in '',
        ///NULL pointers replaced by SQL NULL.  %Q 
        ///</summary>

        const int etTOKEN = 12;

        ///
        ///<summary>
        ///a pointer to a Token structure 
        ///</summary>

        const int etSRCLIST = 13;

        ///
        ///<summary>
        ///a pointer to a SrcList 
        ///</summary>

        const int etPOINTER = 14;

        ///
        ///<summary>
        ///The %p conversion 
        ///</summary>

        const int etSQLESCAPE3 = 15;

        ///
        ///<summary>
        ///%w . Strings with '\"' doubled 
        ///</summary>

        const int etORDINAL = 16;

        ///
        ///<summary>
        ///%r . 1st, 2nd, 3rd, 4th, etc.  English only 
        ///</summary>

        const int etINVALID = 0;

        ///
        ///<summary>
        ///Any unrecognized conversion type 
        ///</summary>

        ///<summary>
        /// An "etByte" is an 8-bit unsigned value.
        ///
        ///</summary>
        //typedef unsigned char etByte;
        ///
        ///<summary>
        ///Each builtin conversion character (ex: the 'd' in "%d") is described
        ///by an instance of the following structure
        ///
        ///</summary>

        public class et_info
        {
            ///
            ///<summary>
            ///Information about each format field 
            ///</summary>

            public char fmttype;

            ///
            ///<summary>
            ///The format field code letter 
            ///</summary>

            public byte _base;

            ///
            ///<summary>
            ///The _base for radix conversion 
            ///</summary>

            public byte flags;

            ///
            ///<summary>
            ///One or more of FLAG_ constants below 
            ///</summary>

            public byte type;

            ///
            ///<summary>
            ///Conversion paradigm 
            ///</summary>

            public byte charset;

            ///
            ///<summary>
            ///Offset into aDigits[] of the digits string 
            ///</summary>

            public byte prefix;

            ///
            ///<summary>
            ///Offset into aPrefix[] of the prefix string 
            ///</summary>

            ///
            ///<summary>
            ///Constructor
            ///
            ///</summary>

            public et_info(char fmttype, byte _base, byte flags, byte type, byte charset, byte prefix)
            {
                this.fmttype = fmttype;
                this._base = _base;
                this.flags = flags;
                this.type = type;
                this.charset = charset;
                this.prefix = prefix;
            }
        }

        ///
        ///<summary>
        ///Allowed values for et_info.flags
        ///
        ///</summary>

        const byte FLAG_SIGNED = 1;

        ///
        ///<summary>
        ///True if the value to convert is signed 
        ///</summary>

        const byte FLAG_INTERN = 2;

        ///
        ///<summary>
        ///True if for internal use only 
        ///</summary>

        const byte FLAG_STRING = 4;

        ///
        ///<summary>
        ///Allow infinity precision 
        ///</summary>

        ///
        ///<summary>
        ///The following table is searched linearly, so it is good to put the
        ///most frequently used conversion types first.
        ///
        ///</summary>

        static string aDigits = "0123456789ABCDEF0123456789abcdef";

        static string aPrefix = "-x0\000X0";

        static et_info[] fmtinfo = new et_info[] {
            new et_info ('d', 10, 1, etRADIX, 0, 0),
            new et_info ('s', 0, 4, etSTRING, 0, 0),
            new et_info ('g', 0, 1, etGENERIC, 30, 0),
            new et_info ('z', 0, 4, etDYNSTRING, 0, 0),
            new et_info ('q', 0, 4, etSQLESCAPE, 0, 0),
            new et_info ('Q', 0, 4, etSQLESCAPE2, 0, 0),
            new et_info ('w', 0, 4, etSQLESCAPE3, 0, 0),
            new et_info ('c', 0, 0, etCHARX, 0, 0),
            new et_info ('o', 8, 0, etRADIX, 0, 2),
            new et_info ('u', 10, 0, etRADIX, 0, 0),
            new et_info ('x', 16, 0, etRADIX, 16, 1),
            new et_info ('X', 16, 0, etRADIX, 0, 4),
#if !SQLITE_OMIT_FLOATING_POINT
            new et_info ('f', 0, 1, etFLOAT, 0, 0),
            new et_info ('e', 0, 1, etEXP, 30, 0),
            new et_info ('E', 0, 1, etEXP, 14, 0),
            new et_info ('G', 0, 1, etGENERIC, 14, 0),
#endif
            new et_info ('i', 10, 1, etRADIX, 0, 0),
            new et_info ('n', 0, 0, etSIZE, 0, 0),
            new et_info ('%', 0, 0, etPERCENT, 0, 0),
            new et_info ('p', 16, 0, etPOINTER, 0, 1),
            ///
            ///<summary>
            ///All the rest have the FLAG_INTERN bit set and are thus for internal
            ///use only 
            ///</summary>

            new et_info ('T', 0, 2, etTOKEN, 0, 0),
            new et_info ('S', 0, 2, etSRCLIST, 0, 0),
            new et_info ('r', 10, 3, etORDINAL, 0, 0),
        };

        ///<summary>
        /// If SQLITE_OMIT_FLOATING_POINT is defined, then none of the floating point
        /// conversions will work.
        ///
        ///</summary>
#if !SQLITE_OMIT_FLOATING_POINT
        ///<summary>
        /// "*val" is a double such that 0.1 <= *val < 10.0
        /// Return the ascii code for the leading digit of *val, then
        /// multiply "*val" by 10.0 to renormalize.
        ///
        /// Example:
        ///     input:     *val = 3.14159
        ///     output:    *val = 1.4159    function return = '3'
        ///
        /// The counter *cnt is incremented each time.  After counter exceeds
        /// 16 (the number of significant digits in a 64-bit float) '0' is
        /// always returned.
        ///</summary>
        static char et_getdigit(ref LONGDOUBLE_TYPE val, ref int cnt)
        {
            int digit;
            LONGDOUBLE_TYPE d;
            if (cnt++ >= 16)
                return '\0';
            digit = (int)val;
            d = digit;
            //digit += '0';
            val = (val - d) * 10.0;
            return (char)digit;
        }

#endif
        ///
        ///<summary>
        ///Append N space characters to the given string buffer.
        ///</summary>

        static void appendSpace(StrAccum pAccum, int N)
        {
            //static const char zSpaces[] = "                             ";
            //while( N>=zSpaces.Length-1 ){
            //  sqlite3StrAccumAppend(pAccum, zSpaces, zSpaces.Length-1);
            //  N -= zSpaces.Length-1;
            //}
            //if( N>0 ){
            //  sqlite3StrAccumAppend(pAccum, zSpaces, N);
            //}
            pAccum.zText.AppendFormat("{0," + N + "}", "");
        }

        ///
        ///<summary>
        ///On machines with a small stack size, you can redefine the
        ///SQLITE_PRINT_BUF_SIZE to be less than 350.
        ///
        ///</summary>

#if !SQLITE_PRINT_BUF_SIZE
#if (SQLITE_SMALL_STACK)
																																								const int SQLITE_PRINT_BUF_SIZE = 50;
#else
        const int SQLITE_PRINT_BUF_SIZE = 350;

#endif
#endif
        const int etBUFSIZE = SQLITE_PRINT_BUF_SIZE;

        ///
        ///<summary>
        ///Size of the output buffer 
        ///</summary>

        ///<summary>
        /// The root program.  All variations call this core.
        ///
        /// INPUTS:
        ///   func   This is a pointer to a function taking three arguments
        ///            1. A pointer to anything.  Same as the "arg" parameter.
        ///            2. A pointer to the list of characters to be output
        ///               (Note, this list is NOT null terminated.)
        ///            3. An integer number of characters to be output.
        ///               (Note: This number might be zero.)
        ///
        ///   arg    This is the pointer to anything which will be passed as the
        ///          first argument to "func".  Use it for whatever you like.
        ///
        ///   fmt    This is the format string, as in the usual print.
        ///
        ///   ap     This is a pointer to a list of arguments.  Same as in
        ///          vfprint.
        ///
        /// OUTPUTS:
        ///          The return value is the total number of characters sent to
        ///          the function "func".  Returns -1 on a error.
        ///
        /// Note that the order in which automatic variables are declared below
        /// seems to make a big difference in determining how fast this beast
        /// will run.
        ///
        ///</summary>
        static char[] buf = new char[etBUFSIZE];

        ///
        ///<summary>
        ///Conversion buffer 
        ///</summary>

        public static void sqlite3VXPrintf(StrAccum pAccum, ///
                                                            ///Accumulate results here 

        int useExtended, ///
                         ///<param name="Allow extended %">conversions </param>

        string fmt, ///
                    ///Format string 

        va_list[] ap///
                    ///arguments 

        )
        {
            int c;
            ///
            ///<summary>
            ///Next character in the format string 
            ///</summary>

            int bufpt;
            ///
            ///<summary>
            ///Pointer to the conversion buffer 
            ///</summary>

            int precision;
            ///
            ///<summary>
            ///Precision of the current field 
            ///</summary>

            int length;
            ///
            ///<summary>
            ///Length of the field 
            ///</summary>

            int idx;
            ///
            ///<summary>
            ///A general purpose loop counter 
            ///</summary>

            int width;
            ///
            ///<summary>
            ///Width of the current field 
            ///</summary>

            etByte flag_leftjustify;
            ///
            ///<summary>
            ///</summary>
            ///<param name="True if "">" flag is present </param>

            etByte flag_plussign;
            ///
            ///<summary>
            ///True if "+" flag is present 
            ///</summary>

            etByte flag_blanksign;
            ///
            ///<summary>
            ///True if " " flag is present 
            ///</summary>

            etByte flag_alternateform;
            ///
            ///<summary>
            ///True if "#" flag is present 
            ///</summary>

            etByte flag_altform2;
            ///
            ///<summary>
            ///True if "!" flag is present 
            ///</summary>

            etByte flag_zeropad;
            ///
            ///<summary>
            ///True if field width constant starts with zero 
            ///</summary>

            etByte flag_long;
            ///
            ///<summary>
            ///True if "l" flag is present 
            ///</summary>

            etByte flag_longlong;
            ///
            ///<summary>
            ///True if the "ll" flag is present 
            ///</summary>

            etByte done;
            ///
            ///<summary>
            ///Loop termination flag 
            ///</summary>

            i64 longvalue;
            LONGDOUBLE_TYPE realvalue;
            ///
            ///<summary>
            ///Value for real types 
            ///</summary>

            et_info infop;
            ///
            ///<summary>
            ///Pointer to the appropriate info structure 
            ///</summary>

            char[] buf = new char[etBUFSIZE];
            ///
            ///<summary>
            ///Conversion buffer 
            ///</summary>

            char prefix;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Prefix character.  "+" or "">" or " " or '\0'. </param>

            byte xtype = 0;
            ///
            ///<summary>
            ///Conversion paradigm 
            ///</summary>

            // Not used in C# -- string zExtra;              /* Extra memory used for etTCLESCAPE conversions */
#if !SQLITE_OMIT_FLOATING_POINT
            int exp, e2;
            ///
            ///<summary>
            ///exponent of real numbers 
            ///</summary>

            double rounder;
            ///
            ///<summary>
            ///Used for rounding floating point values 
            ///</summary>

            etByte flag_dp;
            ///
            ///<summary>
            ///True if decimal point should be shown 
            ///</summary>

            etByte flag_rtz;
            ///
            ///<summary>
            ///True if trailing zeros should be removed 
            ///</summary>

            etByte flag_exp;
            ///
            ///<summary>
            ///True to force display of the exponent 
            ///</summary>

            int nsd;
            ///
            ///<summary>
            ///Number of significant digits returned 
            ///</summary>

#endif
            length = 0;
            bufpt = 0;
            int _fmt = 0;
            // Work around string pointer
            fmt += '\0';
            for (; _fmt <= fmt.Length && (c = fmt[_fmt]) != 0; ++_fmt)
            {
                if (c != '%')
                {
                    int amt;
                    bufpt = _fmt;
                    amt = 1;
                    while (_fmt < fmt.Length && (c = (fmt[++_fmt])) != '%' && c != 0)
                        amt++;
                    pAccum.sqlite3StrAccumAppend(fmt.Substring(bufpt, amt), amt);
                    if (c == 0)
                        break;
                }
                if (_fmt < fmt.Length && (c = (fmt[++_fmt])) == 0)
                {
                    pAccum.sqlite3StrAccumAppend("%", 1);
                    break;
                }
                ///
                ///<summary>
                ///Find out what flags are present 
                ///</summary>

                flag_leftjustify = flag_plussign = flag_blanksign = flag_alternateform = flag_altform2 = flag_zeropad = false;
                done = false;
                do
                {
                    switch (c)
                    {
                        case '-':
                            flag_leftjustify = true;
                            break;
                        case '+':
                            flag_plussign = true;
                            break;
                        case ' ':
                            flag_blanksign = true;
                            break;
                        case '#':
                            flag_alternateform = true;
                            break;
                        case '!':
                            flag_altform2 = true;
                            break;
                        case '0':
                            flag_zeropad = true;
                            break;
                        default:
                            done = true;
                            break;
                    }
                }
                while (!done && _fmt < fmt.Length - 1 && (c = (fmt[++_fmt])) != 0);
                ///
                ///<summary>
                ///Get the field width 
                ///</summary>

                width = 0;
                if (c == '*')
                {
                    width = _Custom.va_arg(ap, (Int32)0);
                    if (width < 0)
                    {
                        flag_leftjustify = true;
                        width = -width;
                    }
                    c = fmt[++_fmt];
                }
                else
                {
                    while (c >= '0' && c <= '9')
                    {
                        width = width * 10 + c - '0';
                        c = fmt[++_fmt];
                    }
                }
                if (width > etBUFSIZE - 10)
                {
                    width = etBUFSIZE - 12;
                }
                ///
                ///<summary>
                ///Get the precision 
                ///</summary>

                if (c == '.')
                {
                    precision = 0;
                    c = fmt[++_fmt];
                    if (c == '*')
                    {
                        precision = _Custom.va_arg(ap, (Int32)0);
                        if (precision < 0)
                            precision = -precision;
                        c = fmt[++_fmt];
                    }
                    else
                    {
                        while (c >= '0' && c <= '9')
                        {
                            precision = precision * 10 + c - '0';
                            c = fmt[++_fmt];
                        }
                    }
                }
                else
                {
                    precision = -1;
                }
                ///
                ///<summary>
                ///Get the conversion type modifier 
                ///</summary>

                if (c == 'l')
                {
                    flag_long = true;
                    c = fmt[++_fmt];
                    if (c == 'l')
                    {
                        flag_longlong = true;
                        c = fmt[++_fmt];
                    }
                    else
                    {
                        flag_longlong = false;
                    }
                }
                else
                {
                    flag_long = flag_longlong = false;
                }
                ///
                ///<summary>
                ///Fetch the info entry for the field 
                ///</summary>

                infop = fmtinfo[0];
                xtype = etINVALID;
                for (idx = 0; idx < Sqlite3.ArraySize(fmtinfo); idx++)
                {
                    if (c == fmtinfo[idx].fmttype)
                    {
                        infop = fmtinfo[idx];
                        if (useExtended != 0 || (infop.flags & FLAG_INTERN) == 0)
                        {
                            xtype = infop.type;
                        }
                        else
                        {
                            return;
                        }
                        break;
                    }
                }
                //zExtra = null;
                ///
                ///<summary>
                ///Limit the precision to prevent overflowing buf[] during conversion 
                ///</summary>

                if (precision > etBUFSIZE - 40 && (infop.flags & FLAG_STRING) == 0)
                {
                    precision = etBUFSIZE - 40;
                }
                ///
                ///<summary>
                ///At this point, variables are initialized as follows:
                ///
                ///flag_alternateform          TRUE if a '#' is present.
                ///flag_altform2               TRUE if a '!' is present.
                ///flag_plussign               TRUE if a '+' is present.
                ///</summary>
                ///<param name="flag_leftjustify            TRUE if a '">' is present or if the</param>
                ///<param name="field width was negative.">field width was negative.</param>
                ///<param name="flag_zeropad                TRUE if the width began with 0.">flag_zeropad                TRUE if the width began with 0.</param>
                ///<param name="flag_long                   TRUE if the letter 'l' (ell) prefixed">flag_long                   TRUE if the letter 'l' (ell) prefixed</param>
                ///<param name="the conversion character.">the conversion character.</param>
                ///<param name="flag_longlong               TRUE if the letter 'll' (ell ell) prefixed">flag_longlong               TRUE if the letter 'll' (ell ell) prefixed</param>
                ///<param name="the conversion character.">the conversion character.</param>
                ///<param name="flag_blanksign              TRUE if a ' ' is present.">flag_blanksign              TRUE if a ' ' is present.</param>
                ///<param name="width                       The specified field width.  This is">width                       The specified field width.  This is</param>
                ///<param name="always non">negative.  Zero is the default.</param>
                ///<param name="precision                   The specified precision.  The default">precision                   The specified precision.  The default</param>
                ///<param name="is ">1.</param>
                ///<param name="xtype                       The class of the conversion.">xtype                       The class of the conversion.</param>
                ///<param name="infop                       Pointer to the appropriate info struct.">infop                       Pointer to the appropriate info struct.</param>
                ///<param name=""></param>

                switch (xtype)
                {
                    case etPOINTER:
                        flag_longlong = true;
                        // char*.Length == sizeof(i64);
                        flag_long = false;
                        // char*.Length == sizeof(long);
                        ///
                        ///<summary>
                        ///Fall through into the next case 
                        ///</summary>

                        goto case etRADIX;
                    case etORDINAL:
                    case etRADIX:
                        if ((infop.flags & FLAG_SIGNED) != 0)
                        {
                            i64 v;
                            if (flag_longlong)
                            {
                                v = (Int64)_Custom.va_arg(ap, (Int64)0);
                            }
                            else
                                if (flag_long)
                            {
                                v = (Int64)_Custom.va_arg(ap, (Int64)0);
                            }
                            else
                            {
                                v = (Int32)_Custom.va_arg(ap, (Int32)0);
                            }
                            if (v < 0)
                            {
                                if (v == IntegerExtensions.SMALLEST_INT64)
                                {
                                    longvalue = ((long)((u64)1) << 63);
                                }
                                else
                                {
                                    longvalue = -v;
                                }
                                prefix = '-';
                            }
                            else
                            {
                                longvalue = v;
                                if (flag_plussign)
                                    prefix = '+';
                                else
                                    if (flag_blanksign)
                                    prefix = ' ';
                                else
                                    prefix = '\0';
                            }
                        }
                        else
                        {
                            if (flag_longlong)
                            {
                                longvalue = _Custom.va_arg(ap, (Int64)0);
                            }
                            else
                                if (flag_long)
                            {
                                longvalue = _Custom.va_arg(ap, (Int64)0);
                            }
                            else
                            {
                                longvalue = _Custom.va_arg(ap, (Int64)0);
                            }
                            prefix = '\0';
                        }
                        if (longvalue == 0)
                            flag_alternateform = false;
                        if (flag_zeropad && precision < width - ((prefix != '\0') ? 1 : 0))
                        {
                            precision = width - ((prefix != '\0') ? 1 : 0);
                        }
                        bufpt = buf.Length;
                        //[etBUFSIZE-1];
                        char[] _bufOrd = null;
                        if (xtype == etORDINAL)
                        {
                            char[] zOrd = "thstndrd".ToCharArray();
                            int x = (int)(longvalue % 10);
                            if (x >= 4 || (longvalue / 10) % 10 == 1)
                            {
                                x = 0;
                            }
                            _bufOrd = new char[2];
                            _bufOrd[0] = zOrd[x * 2];
                            _bufOrd[1] = zOrd[x * 2 + 1];
                            //bufpt -= 2;
                        }
                        {
                            char[] _buf;
                            switch (infop._base)
                            {
                                case 16:
                                    _buf = longvalue.ToString("x").ToCharArray();
                                    break;
                                case 8:
                                    _buf = Convert.ToString((long)longvalue, 8).ToCharArray();
                                    break;
                                default:
                                    {
                                        if (flag_zeropad)
                                            _buf = longvalue.ToString(new string('0', width - ((prefix != '\0') ? 1 : 0))).ToCharArray();
                                        else
                                            _buf = longvalue.ToString().ToCharArray();
                                    }
                                    break;
                            }
                            bufpt = buf.Length - _buf.Length - (_bufOrd == null ? 0 : 2);
                            Array.Copy(_buf, 0, buf, bufpt, _buf.Length);
                            if (_bufOrd != null)
                            {
                                buf[buf.Length - 1] = _bufOrd[1];
                                buf[buf.Length - 2] = _bufOrd[0];
                            }
                            //char* cset;      /* Use registers for speed */
                            //int _base;
                            //cset = aDigits[infop.charset];
                            //_base = infop._base;
                            //do
                            //{ /* Convert to ascii */
                            //   *(--bufpt) = cset[longvalue % (ulong)_base];
                            //  longvalue = longvalue / (ulong)_base;
                            //} while (longvalue > 0);
                        }
                        length = buf.Length - bufpt;
                        //length = (int)(&buf[etBUFSIZE-1]-bufpt);
                        for (idx = precision - length; idx > 0; idx--)
                        {
                            buf[(--bufpt)] = '0';
                            ///
                            ///<summary>
                            ///Zero pad 
                            ///</summary>

                        }
                        if (prefix != '\0')
                            buf[--bufpt] = prefix;
                        ///
                        ///<summary>
                        ///Add sign 
                        ///</summary>

                        if (flag_alternateform && infop.prefix != 0)
                        {
                            ///
                            ///<summary>
                            ///Add "0" or "0x" 
                            ///</summary>

                            int pre;
                            char x;
                            pre = infop.prefix;
                            for (; (x = aPrefix[pre]) != 0; pre++)
                                buf[--bufpt] = x;
                        }
                        length = buf.Length - bufpt;
                        //length = (int)(&buf[etBUFSIZE-1]-bufpt);
                        break;
                    case etFLOAT:
                    case etEXP:
                    case etGENERIC:
                        realvalue = _Custom.va_arg(ap, (Double)0);
#if SQLITE_OMIT_FLOATING_POINT
																																																																																	length = 0;
#else
                        if (precision < 0)
                            precision = 6;
                        ///
                        ///<summary>
                        ///Set default precision 
                        ///</summary>

                        if (precision > etBUFSIZE / 2 - 10)
                            precision = etBUFSIZE / 2 - 10;
                        if (realvalue < 0.0)
                        {
                            realvalue = -realvalue;
                            prefix = '-';
                        }
                        else
                        {
                            if (flag_plussign)
                                prefix = '+';
                            else
                                if (flag_blanksign)
                                prefix = ' ';
                            else
                                prefix = '\0';
                        }
                        if (xtype == etGENERIC && precision > 0)
                            precision--;
#if FALSE
																																																																																	/* Rounding works like BSD when the constant 0.4999 is used.  Wierd! */
for(idx=precision, rounder=0.4999; idx>0; idx--, rounder*=0.1);
#else
                        ///
                        ///<summary>
                        ///It makes more sense to use 0.5 
                        ///</summary>

                        for (idx = precision, rounder = 0.5; idx > 0; idx--, rounder *= 0.1)
                        {
                        }
#endif
                        if (xtype == etFLOAT)
                            realvalue += rounder;
                        ///
                        ///<summary>
                        ///Normalize realvalue to within 10.0 > realvalue >= 1.0 
                        ///</summary>

                        exp = 0;
                        double d = 0;
#if WINDOWS_MOBILE
																																																																																	            //alxwest: Tryparse doesn't exist on Windows Moble and what will Tryparsing a double do?
            if ( Double.IsNaN( realvalue ))
#else
                        if (Double.IsNaN(realvalue) || !(Double.TryParse(Convert.ToString(realvalue), out d)))//if( MathExtensions.sqlite3IsNaN((double)realvalue) )
#endif
                        {
                            buf[0] = 'N';
                            buf[1] = 'a';
                            buf[2] = 'N';
                            // "NaN"
                            length = 3;
                            break;
                        }
                        if (realvalue > 0.0)
                        {
                            while (realvalue >= 1e32 && exp <= 350)
                            {
                                realvalue *= 1e-32;
                                exp += 32;
                            }
                            while (realvalue >= 1e8 && exp <= 350)
                            {
                                realvalue *= 1e-8;
                                exp += 8;
                            }
                            while (realvalue >= 10.0 && exp <= 350)
                            {
                                realvalue *= 0.1;
                                exp++;
                            }
                            while (realvalue < 1e-8)
                            {
                                realvalue *= 1e8;
                                exp -= 8;
                            }
                            while (realvalue < 1.0)
                            {
                                realvalue *= 10.0;
                                exp--;
                            }
                            if (exp > 350)
                            {
                                if (prefix == '-')
                                {
                                    buf[0] = '-';
                                    buf[1] = 'I';
                                    buf[2] = 'n';
                                    buf[3] = 'f';
                                    // "-Inf"
                                    bufpt = 4;
                                }
                                else
                                    if (prefix == '+')
                                {
                                    buf[0] = '+';
                                    buf[1] = 'I';
                                    buf[2] = 'n';
                                    buf[3] = 'f';
                                    // "+Inf"
                                    bufpt = 4;
                                }
                                else
                                {
                                    buf[0] = 'I';
                                    buf[1] = 'n';
                                    buf[2] = 'f';
                                    // "Inf"
                                    bufpt = 3;
                                }
                                length = StringExtensions.sqlite3Strlen30(bufpt);
                                // StringExtensions.sqlite3Strlen30(bufpt);
                                bufpt = 0;
                                break;
                            }
                        }
                        bufpt = 0;
                        ///
                        ///<summary>
                        ///If the field type is etGENERIC, then convert to either etEXP
                        ///or etFLOAT, as appropriate.
                        ///
                        ///</summary>

                        flag_exp = xtype == etEXP;
                        if (xtype != etFLOAT)
                        {
                            realvalue += rounder;
                            if (realvalue >= 10.0)
                            {
                                realvalue *= 0.1;
                                exp++;
                            }
                        }
                        if (xtype == etGENERIC)
                        {
                            flag_rtz = !flag_alternateform;
                            if (exp < -4 || exp > precision)
                            {
                                xtype = etEXP;
                            }
                            else
                            {
                                precision = precision - exp;
                                xtype = etFLOAT;
                            }
                        }
                        else
                        {
                            flag_rtz = false;
                        }
                        if (xtype == etEXP)
                        {
                            e2 = 0;
                        }
                        else
                        {
                            e2 = exp;
                        }
                        nsd = 0;
                        flag_dp = (precision > 0 ? true : false) | flag_alternateform | flag_altform2;
                        ///
                        ///<summary>
                        ///The sign in front of the number 
                        ///</summary>

                        if (prefix != '\0')
                        {
                            buf[bufpt++] = prefix;
                        }
                        ///
                        ///<summary>
                        ///Digits prior to the decimal point 
                        ///</summary>

                        if (e2 < 0)
                        {
                            buf[bufpt++] = '0';
                        }
                        else
                        {
                            for (; e2 >= 0; e2--)
                            {
                                buf[bufpt++] = (char)(et_getdigit(ref realvalue, ref nsd) + '0');
                                // *(bufpt++) = et_getdigit(ref realvalue, ref nsd);
                            }
                        }
                        ///
                        ///<summary>
                        ///The decimal point 
                        ///</summary>

                        if (flag_dp)
                        {
                            buf[bufpt++] = '.';
                        }
                        ///
                        ///<summary>
                        ///"0" digits after the decimal point but before the first
                        ///significant digit of the number 
                        ///</summary>

                        for (e2++; e2 < 0; precision--, e2++)
                        {
                            Debug.Assert(precision > 0);
                            buf[bufpt++] = '0';
                        }
                        ///
                        ///<summary>
                        ///Significant digits after the decimal point 
                        ///</summary>

                        while ((precision--) > 0)
                        {
                            buf[bufpt++] = (char)(et_getdigit(ref realvalue, ref nsd) + '0');
                            // *(bufpt++) = et_getdigit(&realvalue, nsd);
                        }
                        ///
                        ///<summary>
                        ///Remove trailing zeros and the "." if no digits follow the "." 
                        ///</summary>

                        if (flag_rtz && flag_dp)
                        {
                            while (buf[bufpt - 1] == '0')
                                buf[--bufpt] = '\0';
                            Debug.Assert(bufpt > 0);
                            if (buf[bufpt - 1] == '.')
                            {
                                if (flag_altform2)
                                {
                                    buf[(bufpt++)] = '0';
                                }
                                else
                                {
                                    buf[(--bufpt)] = '0';
                                }
                            }
                        }
                        ///
                        ///<summary>
                        ///Add the "eNNN" suffix 
                        ///</summary>

                        if (flag_exp || xtype == etEXP)
                        {
                            buf[bufpt++] = aDigits[infop.charset];
                            if (exp < 0)
                            {
                                buf[bufpt++] = '-';
                                exp = -exp;
                            }
                            else
                            {
                                buf[bufpt++] = '+';
                            }
                            if (exp >= 100)
                            {
                                buf[bufpt++] = (char)(exp / 100 + '0');
                                ///
                                ///<summary>
                                ///100's digit 
                                ///</summary>

                                exp %= 100;
                            }
                            buf[bufpt++] = (char)(exp / 10 + '0');
                            ///
                            ///<summary>
                            ///10's digit 
                            ///</summary>

                            buf[bufpt++] = (char)(exp % 10 + '0');
                            ///
                            ///<summary>
                            ///1's digit 
                            ///</summary>

                        }
                        //bufpt = 0;
                        ///
                        ///<summary>
                        ///The converted number is in buf[] and zero terminated. Output it.
                        ///Note that the number is in the usual order, not reversed as with
                        ///integer conversions. 
                        ///</summary>

                        length = bufpt;
                        //length = (int)(bufpt-buf);
                        bufpt = 0;
                        ///
                        ///<summary>
                        ///Special case:  Add leading zeros if the flag_zeropad flag is
                        ///set and we are not left justified 
                        ///</summary>

                        if (flag_zeropad && !flag_leftjustify && length < width)
                        {
                            int i;
                            int nPad = width - length;
                            for (i = width; i >= nPad; i--)
                            {
                                buf[bufpt + i] = buf[bufpt + i - nPad];
                            }
                            i = (prefix != '\0' ? 1 : 0);
                            while (nPad-- != 0)
                                buf[(bufpt++) + i] = '0';
                            length = width;
                            bufpt = 0;
                        }
#endif
                        break;
                    case etSIZE:
                        ap[0] = pAccum.nChar;
                        // *(_Custom.va_arg(ap,int)) = pAccum.nChar;
                        length = width = 0;
                        break;
                    case etPERCENT:
                        buf[0] = '%';
                        bufpt = 0;
                        length = 1;
                        break;
                    case etCHARX:
                        c = _Custom.va_arg(ap, (Char)0);
                        buf[0] = (char)c;
                        if (precision >= 0)
                        {
                            for (idx = 1; idx < precision; idx++)
                                buf[idx] = (char)c;
                            length = precision;
                        }
                        else
                        {
                            length = 1;
                        }
                        bufpt = 0;
                        break;
                    case etSTRING:
                    case etDYNSTRING:
                        bufpt = 0;
                        //
                        string bufStr = (string)_Custom.va_arg(ap, "string");
                        if (bufStr.Length > buf.Length)
                            buf = new char[bufStr.Length];
                        bufStr.ToCharArray().CopyTo(buf, 0);
                        bufpt = bufStr.Length;
                        if (bufpt == 0)
                        {
                            buf[0] = '\0';
                        }
                        else
                            if (xtype == etDYNSTRING)
                        {
                            //              zExtra = bufpt;
                        }
                        if (precision >= 0)
                        {
                            for (length = 0; length < precision && length < bufStr.Length && buf[length] != 0; length++)
                            {
                            }
                            //length += precision;
                        }
                        else
                        {
                            length = StringExtensions.sqlite3Strlen30(bufpt);
                        }
                        bufpt = 0;
                        break;
                    case etSQLESCAPE:
                    case etSQLESCAPE2:
                    case etSQLESCAPE3:
                        {
                            int i;
                            int j;
                            int k;
                            int n;
                            bool isnull;
                            bool needQuote;
                            char ch;
                            char q = ((xtype == etSQLESCAPE3) ? '"' : '\'');
                            ///
                            ///<summary>
                            ///Quote character 
                            ///</summary>

                            string escarg = (string)_Custom.va_arg(ap, "char*") + '\0';
                            isnull = (escarg == "" || escarg == "NULL\0");
                            if (isnull)
                                escarg = (xtype == etSQLESCAPE2) ? "NULL\0" : "(NULL)\0";
                            k = precision;
                            for (i = n = 0; k != 0 && (ch = escarg[i]) != 0; i++, k--)
                            {
                                if (ch == q)
                                    n++;
                            }
                            needQuote = !isnull && (xtype == etSQLESCAPE2);
                            n += i + 1 + (needQuote ? 2 : 0);
                            if (n > etBUFSIZE)
                            {
                                buf = new char[n];
                                //bufpt = zExtra = malloc_cs.sqlite3Malloc(n);
                                //if ( bufpt == 0 )
                                //{
                                //  pAccum->mallocFailed = 1;
                                //  return;
                                //}
                                bufpt = 0;
                                //Start of Buffer
                            }
                            else
                            {
                                //bufpt = buf;
                                bufpt = 0;
                                //Start of Buffer
                            }
                            j = 0;
                            if (needQuote)
                                buf[bufpt + j++] = q;
                            k = i;
                            for (i = 0; i < k; i++)
                            {
                                buf[bufpt + j++] = ch = escarg[i];
                                if (ch == q)
                                    buf[bufpt + j++] = ch;
                            }
                            if (needQuote)
                                buf[bufpt + j++] = q;
                            buf[bufpt + j] = '\0';
                            length = j;
                            ///
                            ///<summary>
                            ///The precision in %q and %Q means how many input characters to
                            ///consume, not the length of the output...
                            ///if( precision>=0 && precision<length ) length = precision; 
                            ///</summary>

                            break;
                        }
                    case etTOKEN:
                        {
                            Token pToken;
                            if (ap[_Custom.vaNEXT] is String)
                            {
                                pToken = new Token();
                                pToken.zRestSql = _Custom.va_arg(ap, (String)null);
                                pToken.Length = pToken.zRestSql.Length;
                            }
                            else
                                pToken = _Custom.va_arg(ap, (Token)null);
                            if (pToken != null)
                            {
                                pAccum.sqlite3StrAccumAppend(pToken.zRestSql.ToString(), (int)pToken.Length);
                            }
                            length = width = 0;
                            break;
                        }
                    case etSRCLIST:
                        {
                            SrcList pSrc = _Custom.va_arg(ap, (SrcList)null);
                            int k = _Custom.va_arg(ap, (Int32)0);
                            SrcList_item pItem = pSrc.a[k];
                            Debug.Assert(k >= 0 && k < pSrc.nSrc);
                            if (pItem.zDatabase != null)
                            {
                                pAccum.sqlite3StrAccumAppend(pItem.zDatabase, -1);
                                pAccum.sqlite3StrAccumAppend(".", 1);
                            }
                            pAccum.sqlite3StrAccumAppend(pItem.zName, -1);
                            length = width = 0;
                            break;
                        }
                    default:
                        {
                            Debug.Assert(xtype == etINVALID);
                            return;
                        }
                }
                ///
                ///<summary>
                ///End switch over the format type 
                ///</summary>

                ///
                ///<summary>
                ///The text of the conversion is pointed to by "bufpt" and is
                ///"length" characters long.  The field width is "width".  Do
                ///the output.
                ///
                ///</summary>

                if (!flag_leftjustify)
                {
                    int nspace;
                    nspace = width - length;
                    // -2;
                    if (nspace > 0)
                    {
                        appendSpace(pAccum, nspace);
                    }
                }
                if (length > 0)
                {
                    pAccum.sqlite3StrAccumAppend(new string(buf, bufpt, length), length);
                }
                if (flag_leftjustify)
                {
                    int nspace;
                    nspace = width - length;
                    if (nspace > 0)
                    {
                        appendSpace(pAccum, nspace);
                    }
                }
                //if( zExtra ){
                //  sqlite3DbFree(db,ref  zExtra);
                //}
            }
            ///
            ///<summary>
            ///End for loop over the format string 
            ///</summary>

        }


        ///<summary>
        /// Finish off a string by making sure it is zero-terminated.
        /// Return a pointer to the resulting string.  Return a NULL
        /// pointer if any kind of error was encountered.
        ///
        ///</summary>
        public static string sqlite3StrAccumFinish(StrAccum p)
        {
            //if ( p->zText )
            //{
            //  p->zText[p->nChar] = 0;
            //  if ( p->useMalloc && p->zText == p->zBase )
            //  {
            //    if ( p->useMalloc == 1 )
            //    {
            //      p->zText = sqlite3DbMallocRaw( p->db, p->nChar + 1 );
            //    }
            //    else
            //    {
            //      p->zText = sqlite3_malloc( p->nChar + 1 );
            //    }
            //    if ( p->zText )
            //    {
            //      memcpy( p->zText, p->zBase, p->nChar + 1 );
            //    }
            //    else
            //    {
            //      p->mallocFailed = 1;
            //    }
            //  }
            //}
            return p.zText.ToString();
        }

        ///<summary>
        /// Reset an StrAccum string.  Reclaim all malloced memory.
        ///
        ///</summary>
        public static void sqlite3StrAccumReset(StrAccum p)
        {
            //if ( p.zText.ToString() != p.zBase.ToString() )
            //{
            //  if ( p.useMalloc == 1 )
            //  {
            //    sqlite3DbFree( p.db, ref p.zText );
            //  }
            //  else
            //  {
            //    malloc_cs.sqlite3_free( ref p.zText );
            //  }
            //}
            p.zText.Length = 0;
        }

        ///
        ///<summary>
        ///Initialize a string accumulator
        ///
        ///</summary>

        public static void sqlite3StrAccumInit(StrAccum p, StringBuilder zBase, int n, int mx)
        {
            //p.zBase.Length = 0;
            //if ( p.zBase.Capacity < n )
            //  p.zBase.Capacity = n;
            p.zText.Length = 0;
            if (p.zText.Capacity < n)
                p.zText.Capacity = n;
            p.db = null;
            //p.nChar = 0;
            //p.nAlloc = n;
            p.mxAlloc = mx;
            //p.useMalloc = 1;
            //p.tooBig = 0;
            //p.mallocFailed = 0;
        }

        ///<summary>
        /// Print into memory obtained from sqliteMalloc().  Use the internal
        /// %-conversion extensions.
        ///
        ///</summary>
        static StrAccum acc = new StrAccum(SQLITE_PRINT_BUF_SIZE);

        public static string sqlite3VMPrintf(sqlite3 db, string zFormat, params va_list[] ap)
        {
            if (zFormat == null)
                return null;
            if (ap.Length == 0)
                return zFormat;
            //string z;
            Debug.Assert(db != null);
            sqlite3StrAccumInit(acc, null, SQLITE_PRINT_BUF_SIZE, db.aLimit[Globals.SQLITE_LIMIT_LENGTH]);
            acc.db = db;
            acc.zText.Length = 0;
            io.sqlite3VXPrintf(acc, 1, zFormat, ap);
            //      if ( acc.mallocFailed != 0 )
            //      {
            //////        db.mallocFailed = 1;
            //      }
            return io.sqlite3StrAccumFinish(acc);
        }

        ///<summary>
        /// Print into memory obtained from sqliteMalloc().  Use the internal
        /// %-conversion extensions.
        ///
        ///</summary>
        public static string sqlite3MPrintf(sqlite3 db, string zFormat, params va_list[] ap)
        {
            string z;
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                _Custom.va_start(ap, zFormat);
                z = io.sqlite3VMPrintf(db, zFormat, ap);
                _Custom.va_end(ref ap);
            }
            return z;
        }

        ///<summary>
        /// Like io.sqlite3MPrintf(), but call sqlite3DbFree() on zStr after formatting
        /// the string and before returnning.  This routine is intended to be used
        /// to modify an existing string.  For example:
        ///
        ///       x = io.sqlite3MPrintf(db, x, "prefix %s suffix", x);
        ///
        ///
        ///</summary>
        public static string sqlite3MAppendf(sqlite3 db, string zStr, string zFormat, params va_list[] ap)
        {
            string z;
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                _Custom.va_start(ap, zFormat);
                z = io.sqlite3VMPrintf(db, zFormat, ap);
                _Custom.va_end(ref ap);
                db.sqlite3DbFree(ref zStr);
            }
            return z;
        }

        ///<summary>
        /// Print into memory obtained from malloc_cs.sqlite3Malloc().  Omit the internal
        /// %-conversion extensions.
        ///
        ///</summary>
        static string sqlite3_vmprintf(string zFormat, params va_list[] ap)
        {
            //StrAccum acc = new StrAccum( SQLITE_PRINT_BUF_SIZE );
#if !SQLITE_OMIT_AUTOINIT
            if (Sqlite3.sqlite3_initialize() != 0)
                return "";
#endif
            sqlite3StrAccumInit(acc, null, SQLITE_PRINT_BUF_SIZE, SQLITE_PRINT_BUF_SIZE);
            //zBase).Length;
            //acc.useMalloc = 2;
            io.sqlite3VXPrintf(acc, 0, zFormat, ap);
            return io.sqlite3StrAccumFinish(acc);
        }

        ///<summary>
        /// Print into memory obtained from malloc_cs.sqlite3Malloc()().  Omit the internal
        /// %-conversion extensions.
        ///
        ///</summary>
        static public string sqlite3_mprintf(string zFormat, params va_list[] ap)
        {
            //, ...){
            string z;
#if !SQLITE_OMIT_AUTOINIT
            if (Sqlite3.sqlite3_initialize() != 0)
                return "";
#endif
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                _Custom.va_start(ap, zFormat);
                z = sqlite3_vmprintf(zFormat, ap);
                _Custom.va_end(ref ap);
            }
            return z;
        }

        ///<summary>
        /// io.sqlite3_snprintf() works like snprintf() except that it ignores the
        /// current locale settings.  This is important for SQLite because we
        /// are not able to use a "," as the decimal point in place of "." as
        /// specified by some locales.
        ///
        /// Oops:  The first two arguments of io.sqlite3_snprintf() are backwards
        /// from the snprintf() standard.  Unfortunately, it is too late to change
        /// this without breaking compatibility, so we just have to live with the
        /// mistake.
        ///
        /// sqlite3_vsnprintf() is the varargs version.
        ///
        ///</summary>
        static public void sqlite3_vsnprintf(int n, StringBuilder zBuf, string zFormat, params va_list[] ap)
        {
            //StrAccum acc = new StrAccum( SQLITE_PRINT_BUF_SIZE );
            if (n <= 0)
                return;
            sqlite3StrAccumInit(acc, null, n, 0);
            //acc.useMalloc = 0;
            io.sqlite3VXPrintf(acc, 0, zFormat, ap);
            zBuf.Length = 0;
            if (n > 1 && n <= acc.zText.Length)
                acc.zText.Length = n - 1;
            zBuf.Append(io.sqlite3StrAccumFinish(acc));
            return;
        }

        static public void sqlite3_snprintf(int n, StringBuilder zBuf, string zFormat, params va_list[] ap)
        {
            //string z;
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                //StrAccum acc = new StrAccum( SQLITE_PRINT_BUF_SIZE );
                zBuf.EnsureCapacity(SQLITE_PRINT_BUF_SIZE);
                _Custom.va_start(ap, zFormat);
                sqlite3_vsnprintf(n, zBuf, zFormat, ap);
                _Custom.va_end(ref ap);
            }
            return;
        }

        //static public string io.sqlite3_snprintf( int n, ref string zBuf, string zFormat, params va_list[] ap )
        //{
        //  string z;
        //  //va_list ap;
        //  StrAccum acc = new StrAccum( SQLITE_PRINT_BUF_SIZE );
        //  if ( n <= 0 )
        //  {
        //    return zBuf;
        //  }
        //  sqlite3StrAccumInit( acc, null, n, 0 );
        //  //acc.useMalloc = 0;
        //  _Custom.va_start( ap, zFormat );
        //  io.sqlite3VXPrintf( acc, 0, zFormat, ap );
        //  _Custom.va_end( ap );
        //  z = sqlite3StrAccumFinish( acc );
        //  return ( zBuf = z );
        //}
        ///<summary>
        /// This is the routine that actually formats the io.sqlite3_log() message.
        /// We house it in a separate routine from io.sqlite3_log() to avoid using
        /// stack space on small-stack systems when logging is disabled.
        ///
        /// io.sqlite3_log() must render into a static buffer.  It cannot dynamically
        /// allocate memory because it might be called while the memory allocator
        /// mutex is held.
        ///
        ///</summary>
        static void renderLogMsg(int iErrCode, string zFormat, params object[] ap)
        {
            //StrAccum acc;                          /* String accumulator */
            //char zMsg[SQLITE_PRINT_BUF_SIZE*3];    /* Complete log message */
            sqlite3StrAccumInit(acc, null, SQLITE_PRINT_BUF_SIZE * 3, 0);
            //acc.useMalloc = 0;
            io.sqlite3VXPrintf(acc, 0, zFormat, ap);
            sqliteinth.sqlite3GlobalConfig.xLog(sqliteinth.sqlite3GlobalConfig.pLogArg, iErrCode, io.sqlite3StrAccumFinish(acc));
        }

        ///<summary>
        /// Format and write a message to the log if logging is enabled.
        ///
        ///</summary>
        public static void sqlite3_log(int iErrCode, string zFormat, params va_list[] ap)
        {
            if (sqliteinth.sqlite3GlobalConfig.xLog != null)
            {
                //va_list ap;                             /* Vararg list */
                lock (_Custom.lock_va_list)
                {
                    _Custom.va_start(ap, zFormat);
                    renderLogMsg(iErrCode, zFormat, ap);
                    _Custom.va_end(ref ap);
                }
            }
        }
        public static void sqlite3_log(SqlResult err, string zFormat, params va_list[] ap)
        {
            io.sqlite3_log((int)err, zFormat, ap);
        }

#if SQLITE_DEBUG || DEBUG || TRACE
																																								    /*
** A version of printf() that understands %lld.  Used for debugging.
** The printf() built into some versions of windows does not understand %lld
** and segfaults if you give it a long long int.
*/
    static void sqlite3DebugPrintf( string zFormat, params va_list[] ap )
    {
      //va_list ap;
      lock ( _Custom.lock_va_list )
      {
        //StrAccum acc = new StrAccum( SQLITE_PRINT_BUF_SIZE );
        sqlite3StrAccumInit( acc, null, SQLITE_PRINT_BUF_SIZE, 0 );
        //acc.useMalloc = 0;
        _Custom.va_start( ap, zFormat );
        io.sqlite3VXPrintf( acc, 0, zFormat, ap );
        _Custom.va_end( ref ap );
      }
      Console.Write( sqlite3StrAccumFinish( acc ) );
      //fflush(stdout);
    }
#endif
#if !SQLITE_OMIT_TRACE
        ///
        ///<summary>
        ///</summary>
        ///<param name="variable">argument wrapper around io.sqlite3VXPrintf().</param>

        public static void sqlite3XPrintf(StrAccum p, string zFormat, params object[] ap)
        {
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                _Custom.va_start(ap, zFormat);
                io.sqlite3VXPrintf(p, 1, zFormat, ap);
                _Custom.va_end(ref ap);
            }
        }
#endif
    }
}
