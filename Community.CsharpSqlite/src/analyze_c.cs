using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
namespace Community.CsharpSqlite {
	using sqlite3_int64=System.Int64;
	using sqlite3_stmt=Sqlite3.Vdbe;
	public partial class Sqlite3 {
		///<summary>
		/// 2005 July 8
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		/// This file contains code associated with the ANALYZE command.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2011-05-19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e
		///
		///
		///</summary>
		#if !SQLITE_OMIT_ANALYZE
		//#include "sqliteInt.h"
		///<summary>
		/// This routine generates code that opens the sqlite_stat1 table for
		/// writing with cursor iStatCur. If the library was built with the
		/// SQLITE_ENABLE_STAT2 macro defined, then the sqlite_stat2 table is
		/// opened for writing using cursor (iStatCur+1)
		///
		/// If the sqlite_stat1 tables does not previously exist, it is created.
		/// Similarly, if the sqlite_stat2 table does not exist and the library
		/// is compiled with SQLITE_ENABLE_STAT2 defined, it is created.
		///
		/// Argument zWhere may be a pointer to a buffer containing a table name,
		/// or it may be a NULL pointer. If it is not NULL, then all entries in
		/// the sqlite_stat1 and (if applicable) sqlite_stat2 tables associated
		/// with the named table are deleted. If zWhere==0, then code is generated
		/// to delete all stat table entries.
		///</summary>
		public struct _aTable {
			public string zName;
			public string zCols;
			public _aTable(string zName,string zCols) {
				this.zName=zName;
				this.zCols=zCols;
			}
		}
		static _aTable[] aTable=new _aTable[] {
			new _aTable("sqlite_stat1","tbl,idx,stat"),
		#if SQLITE_ENABLE_STAT2
																				new _aTable( "sqlite_stat2", "tbl,idx,sampleno,sample" ),
#endif
		};
		///<summary>
		/// Used to pass information from the analyzer reader through to the
		/// callback routine.
		///</summary>
		//typedef struct analysisInfo analysisInfo;
		public struct analysisInfo {
			public sqlite3 db;
			public string zDatabase;
		}
		///<summary>
		/// This callback is invoked once for each index when reading the
		/// sqlite_stat1 table.
		///
		///     argv[0] = name of the table
		///     argv[1] = name of the index (might be NULL)
		///     argv[2] = results of analysis - on integer for each column
		///
		/// Entries for which argv[1]==NULL simply record the number of rows in
		/// the table.
		///</summary>
		static int analysisLoader(object pData,sqlite3_int64 argc,object Oargv,object NotUsed) {
			string[] argv=(string[])Oargv;
			analysisInfo pInfo=(analysisInfo)pData;
			Index pIndex;
			Table pTable;
			int i,c,n;
			int v;
			string z;
			Debug.Assert(argc==3);
			UNUSED_PARAMETER2(NotUsed,argc);
			if(argv==null||argv[0]==null||argv[2]==null) {
				return 0;
			}
			pTable=sqlite3FindTable(pInfo.db,argv[0],pInfo.zDatabase);
			if(pTable==null) {
				return 0;
			}
			if(!String.IsNullOrEmpty(argv[1])) {
				pIndex=sqlite3FindIndex(pInfo.db,argv[1],pInfo.zDatabase);
			}
			else {
				pIndex=null;
			}
			n=pIndex!=null?pIndex.nColumn:0;
			z=argv[2];
			int zIndex=0;
			for(i=0;z!=null&&i<=n;i++) {
				v=0;
				while(zIndex<z.Length&&(c=z[zIndex])>='0'&&c<='9') {
					v=v*10+c-'0';
					zIndex++;
				}
				if(i==0)
					pTable.nRowEst=(uint)v;
				if(pIndex==null)
					break;
				pIndex.aiRowEst[i]=v;
				if(zIndex<z.Length&&z[zIndex]==' ')
					zIndex++;
				if(z.Substring(zIndex).CompareTo("unordered")==0)//memcmp( z, "unordered", 10 ) == 0 )
				 {
					pIndex.bUnordered=1;
					break;
				}
			}
			return 0;
		}
		///<summary>
		/// If the Index.aSample variable is not NULL, delete the aSample[] array
		/// and its contents.
		///</summary>
		static void sqlite3DeleteIndexSamples(sqlite3 db,Index pIdx) {
			#if SQLITE_ENABLE_STAT2
																														  if ( pIdx.aSample != null )
  {
    int j;
    for ( j = 0; j < SQLITE_INDEX_SAMPLES; j++ )
    {
      IndexSample p = pIdx.aSample[j];
      if ( p.eType == SQLITE_TEXT || p.eType == SQLITE_BLOB )
      {
        p.u.z = null;//sqlite3DbFree(db, p.u.z);
        p.u.zBLOB = null;
      }
    }
    sqlite3DbFree( db, ref pIdx.aSample );
  }
#else
			UNUSED_PARAMETER(db);
			UNUSED_PARAMETER(pIdx);
			#endif
		}
		/*
** Load the content of the sqlite_stat1 and sqlite_stat2 tables. The
** contents of sqlite_stat1 are used to populate the Index.aiRowEst[]
** arrays. The contents of sqlite_stat2 are used to populate the
** Index.aSample[] arrays.
**
** If the sqlite_stat1 table is not present in the database, SQLITE_ERROR
** is returned. In this case, even if SQLITE_ENABLE_STAT2 was defined 
** during compilation and the sqlite_stat2 table is present, no data is 
** read from it.
**
** If SQLITE_ENABLE_STAT2 was defined during compilation and the 
** sqlite_stat2 table is not present in the database, SQLITE_ERROR is
** returned. However, in this case, data is read from the sqlite_stat1
** table (if it is present) before returning.
**
** If an OOM error occurs, this function always sets db.mallocFailed.
** This means if the caller does not care about other errors, the return
** code may be ignored.
*/static int sqlite3AnalysisLoad(sqlite3 db,int iDb) {
			analysisInfo sInfo;
			HashElem i;
			string zSql;
			int rc;
			Debug.Assert(iDb>=0&&iDb<db.nDb);
			Debug.Assert(db.aDb[iDb].pBt!=null);
			/* Clear any prior statistics */Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
			//for(i=sqliteHashFirst(&db.aDb[iDb].pSchema.idxHash);i;i=sqliteHashNext(i)){
			for(i=db.aDb[iDb].pSchema.idxHash.first;i!=null;i=i.next) {
				Index pIdx=(Index)i.data;
				// sqliteHashData( i );
				sqlite3DefaultRowEst(pIdx);
				sqlite3DeleteIndexSamples(db,pIdx);
				pIdx.aSample=null;
			}
			/* Check to make sure the sqlite_stat1 table exists */sInfo.db=db;
			sInfo.zDatabase=db.aDb[iDb].zName;
			if(sqlite3FindTable(db,"sqlite_stat1",sInfo.zDatabase)==null) {
				return SQLITE_ERROR;
			}
			/* Load new statistics out of the sqlite_stat1 table */zSql=sqlite3MPrintf(db,"SELECT tbl, idx, stat FROM %Q.sqlite_stat1",sInfo.zDatabase);
			//if ( zSql == null )
			//{
			//  rc = SQLITE_NOMEM;
			//}
			//else
			{
				rc=sqlite3_exec(db,zSql,(dxCallback)analysisLoader,sInfo,0);
				sqlite3DbFree(db,ref zSql);
				/* Load the statistics from the sqlite_stat2 table. */
				#if SQLITE_ENABLE_STAT2
																																							  if ( rc == SQLITE_OK && null == sqlite3FindTable( db, "sqlite_stat2", sInfo.zDatabase ) )
  {
    rc = SQLITE_ERROR;
  }
  if ( rc == SQLITE_OK )
  {
    sqlite3_stmt pStmt = null;

    zSql = sqlite3MPrintf( db,
    "SELECT idx,sampleno,sample FROM %Q.sqlite_stat2", sInfo.zDatabase );
    //if( null==zSql ){
    //rc = SQLITE_NOMEM;
    //}else{
    rc = sqlite3_prepare( db, zSql, -1, ref pStmt, 0 );
    sqlite3DbFree( db, ref zSql );
    //}

    if ( rc == SQLITE_OK )
    {
      while ( sqlite3_step( pStmt ) == SQLITE_ROW )
      {
        string zIndex;   /* Index name */
        Index pIdx;    /* Pointer to the index object */
        zIndex = sqlite3_column_text( pStmt, 0 );
        pIdx = !String.IsNullOrEmpty( zIndex ) ? sqlite3FindIndex( db, zIndex, sInfo.zDatabase ) : null;
        if ( pIdx != null )
        {
          int iSample = sqlite3_column_int( pStmt, 1 );
          if ( iSample < SQLITE_INDEX_SAMPLES && iSample >= 0 )
          {
            int eType = sqlite3_column_type( pStmt, 2 );

            if ( pIdx.aSample == null )
            {
              //static const int sz = sizeof(IndexSample)*SQLITE_INDEX_SAMPLES;
              //pIdx->aSample = (IndexSample )sqlite3DbMallocRaw(0, sz);
              //if( pIdx.aSample==0 ){
              //db.mallocFailed = 1;
              //break;
              //}
              pIdx.aSample = new IndexSample[SQLITE_INDEX_SAMPLES];//memset(pIdx->aSample, 0, sz);
            }

            //Debug.Assert( pIdx.aSample != null );
            if ( pIdx.aSample[iSample] == null )
              pIdx.aSample[iSample] = new IndexSample();
            IndexSample pSample = pIdx.aSample[iSample];
            {
              pSample.eType = (u8)eType;
              if ( eType == SQLITE_INTEGER || eType == SQLITE_FLOAT )
              {
                pSample.u.r = sqlite3_column_double( pStmt, 2 );
              }
              else if ( eType == SQLITE_TEXT || eType == SQLITE_BLOB )
              {
                string z = null;
                byte[] zBLOB = null;
                //string z = (string )(
                //(eType==SQLITE_BLOB) ?
                //sqlite3_column_blob(pStmt, 2):
                //sqlite3_column_text(pStmt, 2)
                //);
                if ( eType == SQLITE_BLOB )
                  zBLOB = sqlite3_column_blob( pStmt, 2 );
                else
                  z = sqlite3_column_text( pStmt, 2 );
                int n = sqlite3_column_bytes( pStmt, 2 );
                if ( n > 24 )
                {
                  n = 24;
                }
                pSample.nByte = (u8)n;
                if ( n < 1 )
                {
                  pSample.u.z = null;
                  pSample.u.zBLOB = null;
                }
                else
                {
                  pSample.u.z = z;
                  pSample.u.zBLOB = zBLOB;
                  //pSample->u.z = sqlite3DbMallocRaw(dbMem, n);
                  //if( pSample->u.z ){
                  //  memcpy(pSample->u.z, z, n);
                  //}else{
                  //  db->mallocFailed = 1;
                  //  break;
                  //}
                }
              }
            }
          }
        }
      }
      rc = sqlite3_finalize( pStmt );
    }
  }
#endif
				//if( rc==SQLITE_NOMEM ){
				//  db.mallocFailed = 1;
				//}
			}
			return rc;
		}
	#endif
	}
}
