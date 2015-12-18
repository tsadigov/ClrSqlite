using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;
using i64 = System.Int64;

namespace Community.CsharpSqlite.Engine
{
    public class CPU
    {
        public SqlResult rc = SqlResult.SQLITE_OK;
        ///Value to return 
        ///

        ///<summary>
        ///The database connection that owns this statement 
        ///</summary>
        public Connection db ;
        ///The database 
        public u8 resetSchemaOnFault = 0;
        ///Reset schema after an error if positive 
        public SqliteEncoding encoding;//= sqliteinth.ENC(db);
        ///The database encoding 
#if !SQLITE_OMIT_PROGRESS_CALLBACK
        bool checkProgress;
        ///True if progress callbacks are enabled 
        int nProgressOps = 0;
        ///Opcodes executed since progress callback. 
#endif
        public List<Mem> aMem ;
        ///3rd input operand 
        public Mem pOut = null;
        int _opcodeIndex;
        public int opcodeIndex {
            get { return _opcodeIndex; }
            set {
                _opcodeIndex = value;
                ShowDebugInfo();
                //Log.WriteLine("CPU:OpCodeIndex>>>"+value);
            }
        }


        ///
        ///<summary>
        ///VdbeCursor row cache generation counter 
        ///</summary>
        /// <summary>
        /// The program counter 
        /// </summary>
        int _currentOpCodeIndex;
        public int currentOpCodeIndex
        {
            get { return _currentOpCodeIndex; }
            set
            {
                _currentOpCodeIndex = value;
                ShowDebugInfo();
                    //Log.WriteLine("CPU:currentOpCodeIndex>>>" + value);
            }
        }

        public virtual void ShowDebugInfo()
        {
        }

        public Vdbe vdbe;
        public OnConstraintError errorAction;
        public int iCompare;

        public i64 lastRowid;
        ///Saved value of the last insert ROWID 
        ///
        public int[] aPermute = null;
        ///Permutation of columns for  OpCode.OP_Compare 
    }
}
