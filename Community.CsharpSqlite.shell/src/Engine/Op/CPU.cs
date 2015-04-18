using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;

namespace Community.CsharpSqlite.Engine
{
    public class CPU
    {
        public SqlResult rc = SqlResult.SQLITE_OK;
        ///Value to return 
        public sqlite3 db ;
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
        public Mem[] aMem ;
        ///3rd input operand 
        public Mem pOut = null;
        public int opcodeIndex;
        public Community.CsharpSqlite.Sqlite3.Vdbe vdbe;
        public OnConstraintError errorAction;
        public int iCompare;
    }
}
