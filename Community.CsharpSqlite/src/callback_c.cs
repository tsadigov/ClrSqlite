using System;
using System.Diagnostics;
using System.Text;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
using System.Linq;

namespace Community.CsharpSqlite {
    using sqlite3_value = Engine.Mem;
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
	public partial class Sqlite3 {
		///<summary>
		/// 2005 May 23
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		///
		/// This file contains functions used to access the internal hash tables
		/// of user defined functions and collation sequences.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2011-05-19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e
		///
		///
		///
		///</summary>
		//#include "sqliteInt.h"
		///<summary>
		/// Invoke the 'collation needed' callback to request a collation sequence
		/// in the encoding enc of name zName, length nName.
		///
		///</summary>
		static void callCollNeeded(Connection db,SqliteEncoding enc,string zName) {
			Debug.Assert(db.xCollNeeded==null||db.xCollNeeded16==null);
			if(db.xCollNeeded!=null) {
				string zExternal=zName;
				// sqlite3DbStrDup(db, zName);
				if(zExternal==null)
					return;
				db.xCollNeeded(db.pCollNeededArg,db,(int)enc,zExternal);
				db.sqlite3DbFree(ref zExternal);
			}
			#if !SQLITE_OMIT_UTF16
																																																															if( db.xCollNeeded16!=null ){
string zExternal;
sqlite3_value pTmp = sqlite3ValueNew(db);
sqlite3ValueSetStr(pTmp, -1, zName, SqliteEncoding.UTF8, SQLITE_STATIC);
zExternal = sqlite3ValueText(pTmp, SqliteEncoding.UTF16NATIVE);
if( zExternal!="" ){
db.xCollNeeded16( db.pCollNeededArg, db, db.aDbStatic[0].pSchema.enc, zExternal );//(int)ENC(db), zExternal);
}
sqlite3ValueFree(ref pTmp);
}
#endif
		}
		///<summary>
		/// This routine is called if the collation factory fails to deliver a
		/// collation function in the best encoding but there may be other versions
		/// of this collation function (for other text encodings) available. Use one
		/// of these instead if they exist. Avoid a UTF-8 <. UTF-16 conversion if
		/// possible.
		///
		///</summary>
		static SqlResult synthCollSeq(Connection db,CollSeq pColl) {
			CollSeq pColl2;
			string z=pColl.zName;
			int i;
			SqliteEncoding[] aEnc= {
				SqliteEncoding.UTF16BE,
				SqliteEncoding.UTF16LE,
				SqliteEncoding.UTF8
			};
			for(i=0;i<3;i++) {
				pColl2=db.sqlite3FindCollSeq(aEnc[i],z,0);
				if(pColl2.xCmp!=null) {
					pColl=pColl2.Copy();
					//memcpy(pColl, pColl2, sizeof(CollSeq));
					pColl.xDel=null;
					///
					///<summary>
					///Do not copy the destructor 
					///</summary>
					return SqlResult.SQLITE_OK;
				}
			}
			return SqlResult.SQLITE_ERROR;
		}
		///<summary>
		/// This function is responsible for invoking the collation factory callback
		/// or substituting a collation sequence of a different encoding when the
		/// requested collation sequence is not available in the desired encoding.
		///
		/// If it is not NULL, then pColl must point to the database native encoding
		/// collation sequence with name zName, length nName.
		///
		/// The return value is either the collation sequence to be used in database
		/// db for collation type name zName, length nName, or NULL, if no collation
		/// sequence can be found.
		///
		/// See also: build.sqlite3LocateCollSeq(), sqlite3FindCollSeq()
		///
		///</summary>
		public static CollSeq sqlite3GetCollSeq(Connection db,///
		///<summary>
		///The database connection 
		///</summary>
		SqliteEncoding enc,///
		///<summary>
		///The desired encoding for the collating sequence 
		///</summary>
		CollSeq pColl,///
		///<summary>
		///Collating sequence with native encoding, or NULL 
		///</summary>
		string zName///
		///<summary>
		///Collating sequence name 
		///</summary>
		) {
			CollSeq p;
			p=pColl;
			if(p==null) {
				p=db.sqlite3FindCollSeq(enc,zName,0);
			}
			if(p==null||p.xCmp==null) {
				///
				///<summary>
				///No collation sequence of this type for this encoding is registered.
				///Call the collation factory to see if it can supply us with one.
				///
				///</summary>
				callCollNeeded(db,enc,zName);
                p = db.sqlite3FindCollSeq(enc, zName, 0);
			}
			if(p!=null&&p.xCmp==null&&synthCollSeq(db,p)!=0) {
				p=null;
			}
			Debug.Assert(p==null||p.xCmp!=null);
			return p;
		}
		///<summary>
		/// This routine is called on a collation sequence before it is used to
		/// check that it is defined. An undefined collation sequence exists when
		/// a database is loaded that contains references to collation sequences
		/// that have not been defined by sqlite3_create_collation() etc.
		///
		/// If required, this routine calls the 'collation needed' callback to
		/// request a definition of the collating sequence. If this doesn't work,
		/// an equivalent collating sequence that uses a text encoding different
		/// from the main database is substituted, if one is available.
		///
		///</summary>
		static SqlResult sqlite3CheckCollSeq(Parse pParse,CollSeq pColl) {
			if(pColl!=null) {
				string zName=pColl.zName;
				Connection db=pParse.db;
                CollSeq p = sqlite3GetCollSeq(db, sqliteinth.ENC(db), pColl, zName);
				if(null==p) {
					utilc.sqlite3ErrorMsg(pParse,"no such collation sequence: %s",zName);
					pParse.nErr++;
					return SqlResult.SQLITE_ERROR;
				}
				//
				//Debug.Assert(p == pColl);
				if(p!=pColl)// Had to lookup appropriate sequence
				 {
					pColl.enc=p.enc;
					pColl.pUser=p.pUser;
					pColl.type=p.type;
					pColl.xCmp=p.xCmp;
					pColl.xDel=p.xDel;
				}
			}
			return SqlResult.SQLITE_OK;
		}
		///<summary>
		/// Locate and return an entry from the db.aCollSeq hash table. If the entry
		/// specified by zName and nName is not found and parameter 'create' is
		/// true, then create a new entry. Otherwise return NULL.
		///
		/// Each pointer stored in the sqlite3.aCollSeq hash table contains an
		/// array of three CollSeq structures. The first is the collation sequence
		/// prefferred for UTF-8, the second UTF-16le, and the third UTF-16be.
		///
		/// Stored immediately after the three collation sequences is a copy of
		/// the collation sequence name. A pointer to this string is stored in
		/// each collation sequence structure.
		///
		///</summary>
		public static CollSeq[] findCollSeqEntry(Connection db,///
		///<summary>
		///Database connection 
		///</summary>
		string zName,///
		///<summary>
		///Name of the collating sequence 
		///</summary>
		int create///
		///<summary>
		///Create a new entry if true 
		///</summary>
		) {
			CollSeq[] pColl;
			int nName=StringExtensions.Strlen30(zName);
			pColl=db.aCollSeq.Find(zName,nName,(CollSeq[])null);
			if((null==pColl)&&create!=0) {
				pColl=new CollSeq[3];
				//sqlite3DbMallocZero(db, 3*sizeof(*pColl) + nName + 1 );
				if(pColl!=null) {
					CollSeq pDel=null;
					pColl[0]=new CollSeq();
					pColl[0].zName=zName;
					pColl[0].enc=SqliteEncoding.UTF8;
					pColl[1]=new CollSeq();
					pColl[1].zName=zName;
					pColl[1].enc=SqliteEncoding.UTF16LE;
					pColl[2]=new CollSeq();
					pColl[2].zName=zName;
					pColl[2].enc=SqliteEncoding.UTF16BE;
					//memcpy(pColl[0].zName, zName, nName);
					//pColl[0].zName[nName] = 0;
					CollSeq[] pDelArray=HashExtensions.sqlite3HashInsert(ref db.aCollSeq,pColl[0].zName,nName,pColl);
					if(pDelArray!=null)
						pDel=pDelArray[0];
					///
					///<summary>
					///If a malloc() failure occurred in sqlite3HashInsert(), it will
					///return the pColl pointer to be deleted (because it wasn't added
					///to the hash table).
					///
					///</summary>
					Debug.Assert(pDel==null||pDel==pColl[0]);
					if(pDel!=null) {
						////        db.mallocFailed = 1;
						pDel=null;
						//was  sqlite3DbFree(db,ref  pDel);
						pColl=null;
					}
				}
			}
			return pColl;
		}
		
		
	}


    
}
