using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;

namespace Community.CsharpSqlite
{
	using Op = VdbeOp;

	public partial class Sqlite3
	{
		//typedef struct VdbeCursor VdbeCursor;
		///<summary>
		/// When a sub-program is executed (OP_Program), a structure of this type
		/// is allocated to store the current value of the program counter, as
		/// well as the current memory cell array and various other frame specific
		/// values stored in the Vdbe struct. When the sub-program is finished,
		/// these values are copied back to the Vdbe from the VdbeFrame structure,
		/// restoring the state of the VM to as it was before the sub-program
		/// began executing.
		///
		/// The memory for a VdbeFrame object is allocated and managed by a memory
		/// cell in the parent (calling) frame. When the memory cell is deleted or
		/// overwritten, the VdbeFrame object is not freed immediately. Instead, it
		/// is linked into the Vdbe.pDelFrame list. The contents of the Vdbe.pDelFrame
		/// list is deleted when the VM is reset in VdbeHalt(). The reason for doing
		/// this instead of deleting the VdbeFrame immediately is to avoid recursive
		/// calls to sqlite3VdbeMemRelease() when the memory cells belonging to the
		/// child frame are released.
		///
		/// The currently executing frame is stored in Vdbe.pFrame. Vdbe.pFrame is
		/// set to NULL if the currently executing frame is the main program.
		///
		///</summary>
		//typedef struct VdbeFrame VdbeFrame;
		public class VdbeFrame
		{
			public VdbeFrame ()
			{
			}

			///
///<summary>
///VM this frame belongs to 
///</summary>

			public Vdbe v;

			///
///<summary>
///Program Counter in parent (calling) frame 
///</summary>

			int _currentOpCodeIndex;

			public int currentOpCodeIndex {
				get {
					return _currentOpCodeIndex;
				}
				set {
					_currentOpCodeIndex = value;
				}
			}

			public Op[] aOp;

			///
///<summary>
///Program instructions for parent frame 
///</summary>

			public int nOp;

			///
///<summary>
///Size of aOp array 
///</summary>

			public Mem[] aMem;

			///
///<summary>
///Array of memory cells for parent frame 
///</summary>

			public int nMem;

			///
///<summary>
///Number of entries in aMem 
///</summary>

			public VdbeCursor[] apCsr;

			///
///<summary>
///Array of Vdbe cursors for parent frame 
///</summary>

			public u16 nCursor;

			///
///<summary>
///Number of entries in apCsr 
///</summary>

			public int token;

			///
///<summary>
///Copy of SubProgram.token 
///</summary>

			public int nChildMem;

			///
///<summary>
///Number of memory cells for child frame 
///</summary>

			public int nChildCsr;

			///
///<summary>
///Number of cursors for child frame 
///</summary>

			public i64 lastRowid;

			///
///<summary>
///Last insert rowid (sqlite3.lastRowid) 
///</summary>

			public int nChange;

			///
///<summary>
///Statement changes (Vdbe.nChanges)     
///</summary>

			public VdbeFrame pParent;

			///
///<summary>
///Parent of this frame, or NULL if parent is main 
///</summary>

			//
			// Needed for C# Implementation
			//
			///
///<summary>
///Array of memory cells for child frame 
///</summary>

			public Mem[] aChildMem;

			///
///<summary>
///Array of cursors for child frame 
///</summary>

			public VdbeCursor[] aChildCsr;

			public void sqlite3VdbeFrameDelete ()
			{
				int i;
				//Mem[] aMem = VdbeFrameMem(p);
				VdbeCursor[] apCsr = this.aChildCsr;
				// (VdbeCursor)aMem[p.nChildMem];
				for (i = 0; i < this.nChildCsr; i++) {
					sqlite3VdbeFreeCursor (this.v, apCsr [i]);
				}
				releaseMemArray (this.aChildMem, this.nChildMem);
				//this=null;
				// sqlite3DbFree( p.v.db, p );
			}

			public int sqlite3VdbeFrameRestore ()
			{
				Vdbe v = this.v;
				v.aOp = this.aOp;
				v.nOp = this.nOp;
				v.aMem = this.aMem;
				v.nMem = this.nMem;
				v.apCsr = this.apCsr;
				v.nCursor = this.nCursor;
				v.db.lastRowid = this.lastRowid;
				v.nChange = this.nChange;
				return this.currentOpCodeIndex;
			}
		}
	}
}
