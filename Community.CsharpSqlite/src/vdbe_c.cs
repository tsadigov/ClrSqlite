using System.Diagnostics;
using System.IO;
using System.Text;
using FILE=System.IO.TextWriter;
using i32=System.Int32;
using i64=System.Int64;
using sqlite_int64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using sqlite3_int64=System.Int64;
using Pgno=System.UInt32;
///
///<summary>
///The yDbMask datatype for the bitmask of all attached databases.
///</summary>
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask=System.Int32;
#endif
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	using Op=VdbeOp;
	using System;
	using System.Collections.Generic;
	public partial class Sqlite3 {
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
		///The code in this file implements execution method of the
		///Virtual Database Engine (VDBE).  A separate file ("vdbeaux.c")
		///handles housekeeping details such as creating and deleting
		///VDBE instances.  This file is solely interested in executing
		///the VDBE program.
		///
		///In the external interface, an "sqlite3_stmt*" is an opaque pointer
		///to a VDBE.
		///
		///The SQL parser generates a program which is then executed by
		///the VDBE to do the work of the SQL statement.  VDBE programs are
		///similar in form to assembly language.  The program consists of
		///a linear sequence of operations.  Each operation has an opcode
		///and 5 operands.  Operands P1, P2, and P3 are integers.  Operand P4
		///</summary>
		///<param name="is a null">terminated string.  Operand P5 is an unsigned character.</param>
		///<param name="Few opcodes use all 5 operands.">Few opcodes use all 5 operands.</param>
		///<param name=""></param>
		///<param name="Computation results are stored on a set of registers numbered beginning">Computation results are stored on a set of registers numbered beginning</param>
		///<param name="with 1 and going up to Vdbe.nMem.  Each register can store">with 1 and going up to Vdbe.nMem.  Each register can store</param>
		///<param name="either an integer, a null">terminated string, a floating point</param>
		///<param name="number, or the SQL "NULL" value.  An implicit conversion from one">number, or the SQL "NULL" value.  An implicit conversion from one</param>
		///<param name="type to the other occurs as necessary.">type to the other occurs as necessary.</param>
		///<param name=""></param>
		///<param name="Most of the code in this file is taken up by the sqlite3VdbeExec()">Most of the code in this file is taken up by the sqlite3VdbeExec()</param>
		///<param name="function which does the work of interpreting a VDBE program.">function which does the work of interpreting a VDBE program.</param>
		///<param name="But other routines are also provided to help in building up">But other routines are also provided to help in building up</param>
		///<param name="a program instruction by instruction.">a program instruction by instruction.</param>
		///<param name=""></param>
		///<param name="Various scripts scan this source file in order to generate HTML">Various scripts scan this source file in order to generate HTML</param>
		///<param name="documentation, headers files, or other derived files.  The formatting">documentation, headers files, or other derived files.  The formatting</param>
		///<param name="of the code in this file is, therefore, important.  See other comments">of the code in this file is, therefore, important.  See other comments</param>
		///<param name="in this file for details.  If in doubt, do not deviate from existing">in this file for details.  If in doubt, do not deviate from existing</param>
		///<param name="commenting and indentation practices when changing or adding code.">commenting and indentation practices when changing or adding code.</param>
		///<param name=""></param>
		///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
		///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
		///<param name=""></param>
		///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
		///<param name=""></param>
		///<param name=""></param>
		///<param name=""></param>
		//#include "sqliteInt.h"
		//#include "vdbeInt.h"
		///<summary>
		/// Invoke this macro on memory cells just prior to changing the
		/// value of the cell.  This macro verifies that shallow copies are
		/// not misused.
		///
		///</summary>
		#if SQLITE_DEBUG
																																																		    // define memAboutToChange(P,M) sqlite3VdbeMemPrepareToChange(P,M)
    static void memAboutToChange( Vdbe P, Mem M )
    {
      sqlite3VdbeMemPrepareToChange( P, M );
    }
#else
		//# define memAboutToChange(P,M)
		static void memAboutToChange(Vdbe P,Mem M) {
		}
		#endif
		///
		///<summary>
		///The following global variable is incremented every time a cursor
		///moves, either by the OP_SeekXX, OP_Next, or OP_Prev opcodes.  The test
		///procedures use this information to make sure that indices are
		///working correctly.  This variable has no function other than to
		///help verify the correct operation of the library.
		///</summary>
		#if SQLITE_TEST
																																																		#if !TCLSH
																																																		    static int sqlite3_search_count = 0;
#else
																																																		    static tcl.lang.Var.SQLITE3_GETSET sqlite3_search_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_search_count" );
#endif
																																																		#endif
		///
		///<summary>
		///When this global variable is positive, it gets decremented once before
		///each instruction in the VDBE.  When reaches zero, the u1.isInterrupted
		///field of the sqlite3 structure is set in order to simulate and interrupt.
		///
		///This facility is used for testing purposes only.  It does not function
		///in an ordinary build.
		///</summary>
		#if SQLITE_TEST
																																																		#if !TCLSH
																																																		    static int sqlite3_interrupt_count = 0;
#else
																																																		    static tcl.lang.Var.SQLITE3_GETSET sqlite3_interrupt_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_interrupt_count" );
#endif
																																																		#endif
		///
		///<summary>
		///The next global variable is incremented each type the OP_Sort opcode
		///is executed.  The test procedures use this information to make sure that
		///sorting is occurring or not occurring at appropriate times.   This variable
		///has no function other than to help verify the correct operation of the
		///library.
		///</summary>
		#if SQLITE_TEST
																																																		#if !TCLSH
																																																		    static int sqlite3_sort_count = 0;
#else
																																																		    static tcl.lang.Var.SQLITE3_GETSET sqlite3_sort_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_sort_count" );
#endif
																																																		#endif
		///
		///<summary>
		///The next global variable records the size of the largest MEM_Blob
		///or MEM_Str that has been used by a VDBE opcode.  The test procedures
		///</summary>
		///<param name="use this information to make sure that the zero">blob functionality</param>
		///<param name="is working correctly.   This variable has no function other than to">is working correctly.   This variable has no function other than to</param>
		///<param name="help verify the correct operation of the library.">help verify the correct operation of the library.</param>
		#if SQLITE_TEST
																																																		#if !TCLSH
																																																		    static int sqlite3_max_blobsize = 0;
#else
																																																		    static tcl.lang.Var.SQLITE3_GETSET sqlite3_max_blobsize = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_max_blobsize" );
#endif
																																																		
    static void updateMaxBlobsize( Mem p )
    {
#if !TCLSH
																																																		      if ( ( p.flags & ( MEM_Str | MEM_Blob ) ) != 0 && p.n > sqlite3_max_blobsize )
      {
        sqlite3_max_blobsize = p.n;
      }
#else
																																																		      if ( ( p.flags & ( MEM_Str | MEM_Blob ) ) != 0 && p.n > sqlite3_max_blobsize.iValue )
      {
        sqlite3_max_blobsize.iValue = p.n;
      }
#endif
																																																		    }
#endif
		///
		///<summary>
		///The next global variable is incremented each type the OP_Found opcode
		///is executed. This is used to test whether or not the foreign key
		///operation implemented using OP_FkIsZero is working. This variable
		///has no function other than to help verify the correct operation of the
		///library.
		///</summary>
		#if SQLITE_TEST
																																																		#if !TCLSH
																																																		    static int sqlite3_found_count = 0;
#else
																																																		    static tcl.lang.Var.SQLITE3_GETSET sqlite3_found_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_found_count" );
#endif
																																																		#endif
		///<summary>
		////
		/// Test a register to see if it exceeds the current maximum blob size.
		/// If it does, record the new maximum blob size.
		///</summary>
		#if SQLITE_TEST && !SQLITE_OMIT_BUILTIN_TEST
																																																		    static void UPDATE_MAX_BLOBSIZE( Mem P )
    {
      updateMaxBlobsize( P );
    }
#else
		//# define UPDATE_MAX_BLOBSIZE(P)
		static void UPDATE_MAX_BLOBSIZE(Mem P) {
		}
		#endif
		///
		///<summary>
		///Convert the given register into a string if it isn't one
		///</summary>
		///<param name="already. Return non">zero if a malloc() fails.</param>
		//#define Stringify(P, enc) \
		//   if(((P).flags&(MEM_Str|MEM_Blob))==0 && sqlite3VdbeMemStringify(P,enc)) \
		//     { goto no_mem; }
		///<summary>
		/// An ephemeral string value (signified by the MEM_Ephem flag) contains
		/// a pointer to a dynamically allocated string where some other entity
		/// is responsible for deallocating that string.  Because the register
		/// does not control the string, it might be deleted without the register
		/// knowing it.
		///
		/// This routine converts an ephemeral string into a dynamically allocated
		/// string that the register itself controls.  In other words, it
		/// converts an MEM_Ephem string into an MEM_Dyn string.
		///
		///</summary>
		//#define Deephemeralize(P) \
		//   if( ((P).flags&MEM_Ephem)!=0 \
		//       && sqlite3VdbeMemMakeWriteable(P) ){ goto no_mem;}
		static void Deephemeralize(Mem P) {
		}
		///<summary>
		/// Call sqlite3VdbeMemExpandBlob() on the supplied value (type Mem)
		/// P if required.
		///
		///</summary>
		//#define ExpandBlob(P) (((P).flags&MEM_Zero)?sqlite3VdbeMemExpandBlob(P):0)
		static int ExpandBlob(Mem P) {
			return (P.flags&MEM_Zero)!=0?P.sqlite3VdbeMemExpandBlob():0;
		}
		///<summary>
		/// Argument pMem points at a register that will be passed to a
		/// user-defined function or returned to the user as the result of a query.
		/// This routine sets the pMem.type variable used by the sqlite3_value_*()
		/// routines.
		///
		///</summary>
		static void sqlite3VdbeMemStoreType(Mem pMem) {
			if((pMem.Flags&MemFlags.MEM_Null)!=0) {
				pMem.ValType=ValType.SQLITE_NULL;
				pMem.z=null;
				pMem.zBLOB=null;
			}
			else
				if((pMem.Flags&MemFlags.MEM_Int)!=0) {
					pMem.ValType=ValType.SQLITE_INTEGER;
				}
				else
					if((pMem.Flags&MemFlags.MEM_Real)!=0) {
						pMem.ValType=ValType.SQLITE_FLOAT;
					}
					else
						if((pMem.Flags&MemFlags.MEM_Str)!=0) {
							pMem.ValType=ValType.SQLITE_TEXT;
						}
						else {
							pMem.ValType=ValType.SQLITE_BLOB;
						}
		}
		///<summary>
		/// Allocate VdbeCursor number iCur.  Return a pointer to it.  Return NULL
		/// if we run out of memory.
		///
		///</summary>
		static VdbeCursor allocateCursor(Vdbe p,///
		///<summary>
		///The virtual machine 
		///</summary>
		int iCur,///
		///<summary>
		///Index of the new VdbeCursor 
		///</summary>
		int nField,///
		///<summary>
		///Number of fields in the table or index 
		///</summary>
		int iDb,///
		///<summary>
		///</summary>
		///<param name="When database the cursor belongs to, or ">1 </param>
		int isBtreeCursor///
		///<summary>
		///</summary>
		///<param name="True for B">table or vtab </param>
		) {
			///
			///<summary>
			///Find the memory cell that will be used to store the blob of memory
			///required for this VdbeCursor structure. It is convenient to use a
			///vdbe memory cell to manage the memory allocation required for a
			///VdbeCursor structure for the following reasons:
			///
			///Sometimes cursor numbers are used for a couple of different
			///purposes in a vdbe program. The different uses might require
			///different sized allocations. Memory cells provide growable
			///allocations.
			///
			///When using ENABLE_MEMORY_MANAGEMENT, memory cell buffers can
			///be freed lazily via the sqlite3_release_memory() API. This
			///minimizes the number of malloc calls made by the system.
			///
			///Memory cells for cursors are allocated at the top of the address
			///space. Memory cell (p.nMem) corresponds to cursor 0. Space for
			///</summary>
			///<param name="cursor 1 is managed by memory cell (p.nMem">1), etc.</param>
			///<param name=""></param>
			//Mem pMem = p.aMem[p.nMem - iCur];
			//int nByte;
			VdbeCursor pCx=null;
			//ROUND8(sizeof(VdbeCursor)) +
			//( isBtreeCursor ? sqlite3BtreeCursorSize() : 0 ) +
			//2 * nField * sizeof( u32 );
			Debug.Assert(iCur<p.nCursor);
			if(p.apCsr[iCur]!=null) {
                vdbeaux.sqlite3VdbeFreeCursor(p, p.apCsr[iCur]);
				p.apCsr[iCur]=null;
			}
			//if ( SQLITE_OK == sqlite3VdbeMemGrow( pMem, nByte, 0 ) )
			{
				p.apCsr[iCur]=pCx=new VdbeCursor();
				// (VdbeCursor)pMem.z;
				//memset(pCx, 0, sizeof(VdbeCursor));
				pCx.iDb=iDb;
				pCx.nField=nField;
				if(nField!=0) {
					pCx.aType=new u32[nField];
					// (u32)&pMem.z[ROUND8(sizeof( VdbeCursor ))];
				}
				if(isBtreeCursor!=0) {
					pCx.pCursor=mempoolMethods.sqlite3MemMallocBtCursor(pCx.pCursor);
					// (BtCursor)&pMem.z[ROUND8(sizeof( VdbeCursor )) + 2 * nField * sizeof( u32 )];
					pCx.pCursor.sqlite3BtreeCursorZero();
				}
			}
			return pCx;
		}
		///<summary>
		/// Try to convert a value into a numeric representation if we can
		/// do so without loss of information.  In other words, if the string
		/// looks like a number, convert it into a number.  If it does not
		/// look like a number, leave it alone.
		///
		///</summary>
		static void applyNumericAffinity(Mem pRec) {
			if((pRec.flags&(MEM_Real|MEM_Int))==0) {
				double rValue=0.0;
				i64 iValue=0;
				SqliteEncoding enc=pRec.enc;
				if((pRec.flags&MEM_Str)==0)
					return;
				if(Converter.sqlite3AtoF(pRec.z,ref rValue,pRec.n,enc)==false)
					return;
				if(0==Converter.sqlite3Atoi64(pRec.z,ref iValue,pRec.n,enc)) {
					pRec.u.i=iValue;
					pRec.flags|=MEM_Int;
				}
				else {
					pRec.r=rValue;
					pRec.flags|=MEM_Real;
				}
			}
		}
		///<summary>
		/// Processing is determine by the affinity parameter:
		///
		/// SQLITE_AFF_INTEGER:
		/// SQLITE_AFF_REAL:
		/// SQLITE_AFF_NUMERIC:
		///    Try to convert pRec to an integer representation or a
		///    floating-point representation if an integer representation
		///    is not possible.  Note that the integer representation is
		///    always preferred, even if the affinity is REAL, because
		///    an integer representation is more space efficient on disk.
		///
		/// sqliteinth.SQLITE_AFF_TEXT:
		///    Convert pRec to a text representation.
		///
		/// SQLITE_AFF_NONE:
		///    No-op.  pRec is unchanged.
		///
		///</summary>
		static void applyAffinity(Mem pRec,///
		///<summary>
		///The value to apply affinity to 
		///</summary>
		char affinity,///
		///<summary>
		///The affinity to be applied 
		///</summary>
		SqliteEncoding enc///
		///<summary>
		///Use this text encoding 
		///</summary>
		) {
			if(affinity==sqliteinth.SQLITE_AFF_TEXT) {
				///
				///<summary>
				///Only attempt the conversion to TEXT if there is an integer or real
				///representation (blob and NULL do not get converted) but no string
				///representation.
				///
				///</summary>
				if(0==(pRec.flags&MEM_Str)&&(pRec.flags&(MEM_Real|MEM_Int))!=0) {
					sqlite3VdbeMemStringify(pRec,enc);
				}
				if((pRec.flags&(MEM_Blob|MEM_Str))==(MEM_Blob|MEM_Str)) {
					StringBuilder sb=new StringBuilder(pRec.zBLOB.Length);
					for(int i=0;i<pRec.zBLOB.Length;i++)
						sb.Append((char)pRec.zBLOB[i]);
					pRec.z=sb.ToString();
					sqlite3_free(ref pRec.zBLOB);
					pRec.flags=(u16)(pRec.flags&~MEM_Blob);
				}
				pRec.flags=(u16)(pRec.flags&~(MEM_Real|MEM_Int));
			}
			else
                if (affinity != sqliteinth.SQLITE_AFF_NONE)
                {
                    Debug.Assert(affinity == sqliteinth.SQLITE_AFF_INTEGER || affinity == sqliteinth.SQLITE_AFF_REAL || affinity == sqliteinth.SQLITE_AFF_NUMERIC);
					applyNumericAffinity(pRec);
					if((pRec.flags&MEM_Real)!=0) {
						sqlite3VdbeIntegerAffinity(pRec);
					}
				}
		}
		///<summary>
		/// Try to convert the type of a function argument or a result column
		/// into a numeric representation.  Use either INTEGER or REAL whichever
		/// is appropriate.  But only do the conversion if it is possible without
		/// loss of information and return the revised type of the argument.
		///
		///</summary>
		static int sqlite3_value_numeric_type(sqlite3_value pVal) {
			Mem pMem=(Mem)pVal;
			if(pMem.type==SQLITE_TEXT) {
				applyNumericAffinity(pMem);
				sqlite3VdbeMemStoreType(pMem);
			}
			return pMem.type;
		}
		///<summary>
		/// Exported version of applyAffinity(). This one works on sqlite3_value*,
		/// not the internal Mem type.
		///
		///</summary>
		static void sqlite3ValueApplyAffinity(sqlite3_value pVal,char affinity,SqliteEncoding enc) {
			applyAffinity((Mem)pVal,affinity,enc);
		}
		#if SQLITE_DEBUG
																																																		    /*
** Write a nice string representation of the contents of cell pMem
** into buffer zBuf, length nBuf.
*/
    static StringBuilder zCsr = new StringBuilder( 100 );
    static void sqlite3VdbeMemPrettyPrint( Mem pMem, StringBuilder zBuf )
    {
      zBuf.Length = 0;
      zCsr.Length = 0;
      int f = pMem.flags;

      string[] encnames = new string[] { "(X)", "(8)", "(16LE)", "(16BE)" };

      if ( ( f & MEM_Blob ) != 0 )
      {
        int i;
        char c;
        if ( ( f & MEM_Dyn ) != 0 )
        {
          c = 'z';
          Debug.Assert( ( f & ( MEM_Static | MEM_Ephem ) ) == 0 );
        }
        else if ( ( f & MEM_Static ) != 0 )
        {
          c = 't';
          Debug.Assert( ( f & ( MEM_Dyn | MEM_Ephem ) ) == 0 );
        }
        else if ( ( f & MEM_Ephem ) != 0 )
        {
          c = 'e';
          Debug.Assert( ( f & ( MEM_Static | MEM_Dyn ) ) == 0 );
        }
        else
        {
          c = 's';
        }

        io.sqlite3_snprintf( 100, zCsr, "%c", c );
        zBuf.Append( zCsr );//zCsr += StringExtensions.sqlite3Strlen30(zCsr);
        io.sqlite3_snprintf( 100, zCsr, "%d[", pMem.n );
        zBuf.Append( zCsr );//zCsr += StringExtensions.sqlite3Strlen30(zCsr);
        for ( i = 0; i < 16 && i < pMem.n; i++ )
        {
          io.sqlite3_snprintf( 100, zCsr, "%02X", ( (int)pMem.zBLOB[i] & 0xFF ) );
          zBuf.Append( zCsr );//zCsr += StringExtensions.sqlite3Strlen30(zCsr);
        }
        for ( i = 0; i < 16 && i < pMem.n; i++ )
        {
          char z = (char)pMem.zBLOB[i];
          if ( z < 32 || z > 126 )
            zBuf.Append( '.' );//*zCsr++ = '.';
          else
            zBuf.Append( z );//*zCsr++ = z;
        }

        io.sqlite3_snprintf( 100, zCsr, "]%s", encnames[pMem.enc] );
        zBuf.Append( zCsr );//zCsr += StringExtensions.sqlite3Strlen30(zCsr);
        if ( ( f & MEM_Zero ) != 0 )
        {
          io.sqlite3_snprintf( 100, zCsr, "+%dz", pMem.u.nZero );
          zBuf.Append( zCsr );//zCsr += StringExtensions.sqlite3Strlen30(zCsr);
        }
        //*zCsr = '\0';
      }
      else if ( ( f & MEM_Str ) != 0 )
      {
        int j;//, k;
        zBuf.Append( ' ' );
        if ( ( f & MEM_Dyn ) != 0 )
        {
          zBuf.Append( 'z' );
          Debug.Assert( ( f & ( MEM_Static | MEM_Ephem ) ) == 0 );
        }
        else if ( ( f & MEM_Static ) != 0 )
        {
          zBuf.Append( 't' );
          Debug.Assert( ( f & ( MEM_Dyn | MEM_Ephem ) ) == 0 );
        }
        else if ( ( f & MEM_Ephem ) != 0 )
        {
          zBuf.Append( 's' ); //zBuf.Append( 'e' );
          Debug.Assert( ( f & ( MEM_Static | MEM_Dyn ) ) == 0 );
        }
        else
        {
          zBuf.Append( 's' );
        }
        //k = 2;
        io.sqlite3_snprintf( 100, zCsr, "%d", pMem.n );//zBuf[k], "%d", pMem.n );
        zBuf.Append( zCsr );
        //k += StringExtensions.sqlite3Strlen30( &zBuf[k] );
        zBuf.Append( '[' );// zBuf[k++] = '[';
        for ( j = 0; j < 15 && j < pMem.n; j++ )
        {
          u8 c = pMem.z != null ? (u8)pMem.z[j] : pMem.zBLOB[j];
          if ( c >= 0x20 && c < 0x7f )
          {
            zBuf.Append( (char)c );//zBuf[k++] = c;
          }
          else
          {
            zBuf.Append( '.' );//zBuf[k++] = '.';
          }
        }
        zBuf.Append( ']' );//zBuf[k++] = ']';
        io.sqlite3_snprintf( 100, zCsr, encnames[pMem.enc] );//& zBuf[k], encnames[pMem.enc] );
        zBuf.Append( zCsr );
        //k += StringExtensions.sqlite3Strlen30( &zBuf[k] );
        //zBuf[k++] = 0;
      }
    }
#endif
		#if SQLITE_DEBUG
																																																		    /*
** Print the value of a register for tracing purposes:
*/
    static void memTracePrint( FILE _out, Mem p )
    {
      if ( ( p.flags & MEM_Null ) != 0 )
      {
        fprintf( _out, " NULL" );
      }
      else if ( ( p.flags & ( MEM_Int | MEM_Str ) ) == ( MEM_Int | MEM_Str ) )
      {
        fprintf( _out, " si:%lld", p.u.i );
#if !SQLITE_OMIT_FLOATING_POINT
																																																		      }
      else if ( ( p.flags & MEM_Int ) != 0 )
      {
        fprintf( _out, " i:%lld", p.u.i );
#endif
																																																		      }
      else if ( ( p.flags & MEM_Real ) != 0 )
      {
        fprintf( _out, " r:%g", p.r );
      }
      else if ( ( p.flags & MEM_RowSet ) != 0 )
      {
        fprintf( _out, " (rowset)" );
      }
      else
      {
        StringBuilder zBuf = new StringBuilder( 200 );
        sqlite3VdbeMemPrettyPrint( p, zBuf );
        fprintf( _out, " " );
        fprintf( _out, "%s", zBuf );
      }
    }
    static void registerTrace( FILE _out, int iReg, Mem p )
    {
      fprintf( _out, "reg[%d] = ", iReg );
      memTracePrint( _out, p );
      fprintf( _out, "\n" );
    }
#endif
		#if SQLITE_DEBUG
																																																		    //  define REGISTER_TRACE(R,M) if(p.trace)registerTrace(p.trace,R,M)
    static void REGISTER_TRACE( Vdbe p, int R, Mem M )
    {
      if ( p.trace != null )
        registerTrace( p.trace, R, M );
    }
#else
		//#  define REGISTER_TRACE(R,M)
		static void REGISTER_TRACE(Vdbe p,int R,Mem M) {
		}
		#endif
		#if VDBE_PROFILE
																																																		
/*
** hwtime.h contains inline assembler code for implementing
** high-performance timing routines.
*/
//include "hwtime.h"

#endif
		///<summary>
		/// The CHECK_FOR_INTERRUPT macro defined here looks to see if the
		/// sqlite3_interrupt() routine has been called.  If it has been, then
		/// processing of the VDBE program is interrupted.
		///
		/// This macro added to every instruction that does a jump in order to
		/// implement a loop.  This test used to be on every single instruction,
		/// but that meant we more testing that we needed.  By only testing the
		/// flag on jump instructions, we get a (small) speed improvement.
		///</summary>
		//#define CHECK_FOR_INTERRUPT \
		//   if( db.u1.isInterrupted ) goto abort_due_to_interrupt;
		#if !NDEBUG
																																																		    ///<summary>
/// This function is only called from within an Debug.Assert() expression. It
/// checks that the sqlite3.nTransaction variable is correctly set to
/// the number of non-transaction savepoints currently in the
/// linked list starting at sqlite3.pSavepoint.
///
/// Usage:
///
///     Debug.Assert( checkSavepointCount(db) );
///</summary>
    static int checkSavepointCount( sqlite3 db )
    {
      int n = 0;
      Savepoint p;
      for ( p = db.pSavepoint; p != null; p = p.pNext )
        n++;
      Debug.Assert( n == ( db.nSavepoint + db.isTransactionSavepoint ) );
      return 1;
    }
#else
		static int checkSavepointCount(sqlite3 db) {
			return 1;
		}
		#endif
		///<summary>
		/// Transfer error message text from an sqlite3_vtab.zErrMsg (text stored
		/// in memory obtained from sqlite3_malloc) into a Vdbe.zErrMsg (text stored
		/// in memory obtained from sqlite3DbMalloc).
		///</summary>
		static void importVtabErrMsg(Vdbe p,sqlite3_vtab pVtab) {
			sqlite3 db=p.db;
			db.sqlite3DbFree(ref p.zErrMsg);
			p.zErrMsg=pVtab.zErrMsg;
			// sqlite3DbStrDup( db, pVtab.zErrMsg );
			//sqlite3_free( pVtab.zErrMsg );
			pVtab.zErrMsg=null;
		}
	/**
    ** Execute as much of a VDBE program as we can then return.
    **
    ** sqlite3VdbeMakeReady() must be called before this routine in order to
    ** close the program with a final OP_Halt and to set up the callbacks
    ** and the error message pointer.
    **
    ** Whenever a row or result data is available, this routine will either
    ** invoke the result callback (if there is one) or return with
    ** SQLITE_ROW.
    **
    ** If an attempt is made to open a locked database, then this routine
    ** will either invoke the busy callback (if there is one) or it will
    ** return SQLITE_BUSY.
    **
    ** If an error occurs, an error message is written to memory obtained
    ** from sqlite3Malloc() and p.zErrMsg is made to point to that memory.
    ** The error code is stored in p.rc and this routine returns SQLITE_ERROR.
    **
    ** If the callback ever returns non-zero, then the program exits
    ** immediately.  There will be no error message but the p.rc field is
    ** set to SQLITE_ABORT and this routine will return SQLITE_ERROR.
    **
    ** A memory allocation error causes p.rc to be set to SQLITE_NOMEM and this
    ** routine to return SQLITE_ERROR.
    **
    ** Other fatal errors return SQLITE_ERROR.
    **
    ** After this routine has finished, sqlite3VdbeFinalize() should be
    ** used to clean up the mess that was left behind.
    */}
}
