using Community.CsharpSqlite;
using Community.CsharpSqlite.Paging;
using Community.CsharpSqlite.tree;
using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AnalysingClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            Sqlite3.sqlite3_initialize();


            SqlResult r;
            var fs = Sqlite3.winVfs;


            Connection pSrc = new Connection();
            var zSrcFile = "hehehe";
            Btree btree = null;
            r = Btree.Open(fs, "hehehe", pSrc, ref btree, 0, 262);
            

            r = Sqlite3.sqlite3_open(zSrcFile, out pSrc);
            String m="";
            pSrc.init.busy = 1;
            Sqlite3.sqlite3InitOne(pSrc, 0, ref m);
            var schema = btree.sqlite3SchemaGet(pSrc);


            var zMaster = sqliteinth.SCHEMA_TABLE(1);
            var initData = new InitData();
            initData.db = pSrc;
            initData.iDb = 1;
            //initData.pzErrMsg = vdbe.zErrMsg;
            var zSql = Community.CsharpSqlite.Os.io.sqlite3MPrintf(pSrc, "SELECT name, rootpage, sql FROM '%q'.%s WHERE %s ORDER BY rowid", pSrc.Backends[1].Name, zMaster, " 1=1 ");
            if (String.IsNullOrEmpty(zSql))
            {
                
            }
            else
            {
                //Debug.Assert(0 == pSrc.init.busy);
                using (pSrc.init.scope())
                {
                    initData.rc = SqlResult.SQLITE_OK;
                    //Debug.Assert( 0 == db.mallocFailed );
                    r = legacy.sqlite3_exec(pSrc, zSql, (dxCallback)Sqlite3.sqlite3InitCallback, (object)initData, 0);
                }
            }






            Pager p =null;
            
            PagerMethods.PagerOpen(fs, out p, "db123", Globals.BTree.EXTRA_SIZE,
                Sqlite3.BTREE_OMIT_JOURNAL | Sqlite3.BTREE_SINGLE, 
                262, BTreeMethods.pageReinit);



            Console.WriteLine(p.dbFileSize);
            r=p.LockDbFile(Community.CsharpSqlite.Os.LockType.EXCLUSIVE_LOCK);
            p.pager_open_journal();
            p.writeMasterJournal(null);
            //p.writeJournalHdr();
            r =p.UnlockDbFile(Community.CsharpSqlite.Os.LockType.EXCLUSIVE_LOCK);
            Console.WriteLine(p.journalHdrOffset());
            p.sqlite3PagerCommitPhaseOne(null, true);
            
            
        }
    }



}
