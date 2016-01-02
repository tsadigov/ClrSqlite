using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static class Error
    {
        ///<summary>
		/// Return a static string that describes the kind of error specified in the
		/// argument.
		///
		///</summary>
		public static string sqlite3ErrStr(this int rc)
        {
            string[] aMsg = new string[] {
				///
				///<summary>
				///SqlResult.SQLITE_OK          
				///</summary>
				"not an error",
				///
				///<summary>
				///SqlResult.SQLITE_ERROR       
				///</summary>
				"SQL logic error or missing database",
				///
				///<summary>
				///SQLITE_INTERNAL    
				///</summary>
				"",
				///
				///<summary>
				///SQLITE_PERM        
				///</summary>
				"access permission denied",
				///
				///<summary>
				///SQLITE_ABORT       
				///</summary>
				"callback requested query abort",
				///
				///<summary>
				///SQLITE_BUSY        
				///</summary>
				"database is locked",
				///
				///<summary>
				///SQLITE_LOCKED      
				///</summary>
				"database table is locked",
				///
				///<summary>
				///SQLITE_NOMEM       
				///</summary>
				"out of memory",
				///
				///<summary>
				///SQLITE_READONLY    
				///</summary>
				"attempt to write a readonly database",
				///
				///<summary>
				///SQLITE_INTERRUPT   
				///</summary>
				"interrupted",
				///
				///<summary>
				///SQLITE_IOERR       
				///</summary>
				"disk I/O error",
				///
				///<summary>
				///SQLITE_CORRUPT     
				///</summary>
				"database disk image is malformed",
				///
				///<summary>
				///SQLITE_NOTFOUND    
				///</summary>
				"unknown operation",
				///
				///<summary>
				///SQLITE_FULL        
				///</summary>
				"database or disk is full",
				///
				///<summary>
				///SQLITE_CANTOPEN    
				///</summary>
				"unable to open database file",
				///
				///<summary>
				///SQLITE_PROTOCOL    
				///</summary>
				"locking protocol",
				///
				///<summary>
				///SQLITE_EMPTY       
				///</summary>
				"table contains no data",
				///
				///<summary>
				///SQLITE_SCHEMA      
				///</summary>
				"database schema has changed",
				///
				///<summary>
				///SQLITE_TOOBIG      
				///</summary>
				"string or blob too big",
				///
				///<summary>
				///SQLITE_CONSTRAINT  
				///</summary>
				"constraint failed",
				///
				///<summary>
				///SQLITE_MISMATCH    
				///</summary>
				"datatype mismatch",
				///
				///<summary>
				///SQLITE_MISUSE      
				///</summary>
				"library routine called out of sequence",
				///
				///<summary>
				///SQLITE_NOLFS       
				///</summary>
				"large file support is disabled",
				///
				///<summary>
				///SQLITE_AUTH        
				///</summary>
				"authorization denied",
				///
				///<summary>
				///SQLITE_FORMAT      
				///</summary>
				"auxiliary database format error",
				///
				///<summary>
				///SQLITE_RANGE       
				///</summary>
				"bind or column index out of range",
				///
				///<summary>
				///SQLITE_NOTADB      
				///</summary>
				"file is encrypted or is not a database",
            };
            rc &= 0xff;
            if (Sqlite3.ALWAYS(rc >= 0) && rc < aMsg.Length && aMsg[rc] != "")//(int)(sizeof(aMsg)/sizeof(aMsg[0]))
            {
                return aMsg[rc];
            }
            else {
                return "unknown error";
            }
        }
        public static string sqlite3ErrStr(this SqlResult rc)
        {
            return sqlite3ErrStr((int)rc);
        }
    }
}
