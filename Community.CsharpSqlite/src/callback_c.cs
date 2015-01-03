using System;
using System.Diagnostics;
using System.Text;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
namespace Community.CsharpSqlite {
	using sqlite3_value=Mem;
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
		static void callCollNeeded(sqlite3 db,SqliteEncoding enc,string zName) {
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
		static int synthCollSeq(sqlite3 db,CollSeq pColl) {
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
					return Sqlite3.SQLITE_OK;
				}
			}
			return Sqlite3.SQLITE_ERROR;
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
		static CollSeq sqlite3GetCollSeq(sqlite3 db,///
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
		static int sqlite3CheckCollSeq(Parse pParse,CollSeq pColl) {
			if(pColl!=null) {
				string zName=pColl.zName;
				sqlite3 db=pParse.db;
                CollSeq p = sqlite3GetCollSeq(db, sqliteinth.ENC(db), pColl, zName);
				if(null==p) {
					utilc.sqlite3ErrorMsg(pParse,"no such collation sequence: %s",zName);
					pParse.nErr++;
					return Sqlite3.SQLITE_ERROR;
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
			return Sqlite3.SQLITE_OK;
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
		public static CollSeq[] findCollSeqEntry(sqlite3 db,///
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
			int nName=StringExtensions.sqlite3Strlen30(zName);
			pColl=db.aCollSeq.sqlite3HashFind(zName,nName,(CollSeq[])null);
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
					CollSeq[] pDelArray=sqlite3HashInsert(ref db.aCollSeq,pColl[0].zName,nName,pColl);
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
		static int matchQuality(FuncDef p,int nArg,SqliteEncoding enc) {
			int match=0;
			if(p.nArg==-1||p.nArg==nArg||(nArg==-1&&(p.xFunc!=null||p.xStep!=null))) {
				match=1;
				if(p.nArg==nArg||nArg==-1) {
					match=4;
				}
				if(enc==p.iPrefEnc) {
					match+=2;
				}
				else
					if((enc==SqliteEncoding.UTF16LE&&p.iPrefEnc==SqliteEncoding.UTF16BE)||(enc==SqliteEncoding.UTF16BE&&p.iPrefEnc==SqliteEncoding.UTF16LE)) {
						match+=1;
					}
			}
			return match;
		}
		///<summary>
		/// Search a FuncDefHash for a function with the given name.  Return
		/// a pointer to the matching FuncDef if found, or 0 if there is no match.
		///
		///</summary>
		static FuncDef functionSearch(FuncDefHash pHash,///
		///<summary>
		///Hash table to search 
		///</summary>
		int h,///
		///<summary>
		///Hash of the name 
		///</summary>
		string zFunc,///
		///<summary>
		///Name of function 
		///</summary>
		int nFunc///
		///<summary>
		///Number of bytes in zFunc 
		///</summary>
		) {
			FuncDef p;
			for(p=pHash.a[h];p!=null;p=p.pHash) {
				if(p.zName.Length==nFunc&&p.zName.StartsWith(zFunc,StringComparison.InvariantCultureIgnoreCase)) {
					return p;
				}
			}
			return null;
		}
		///<summary>
		/// Insert a new FuncDef into a FuncDefHash hash table.
		///
		///</summary>
		static void sqlite3FuncDefInsert(FuncDefHash pHash,///
		///<summary>
		///The hash table into which to insert 
		///</summary>
		FuncDef pDef///
		///<summary>
		///The function definition to insert 
		///</summary>
		) {
			FuncDef pOther;
			int nName=StringExtensions.sqlite3Strlen30(pDef.zName);
			u8 c1=(u8)pDef.zName[0];
			int h=(_Custom.sqlite3UpperToLower[c1]+nName)%Sqlite3.ArraySize(pHash.a);
			pOther=functionSearch(pHash,h,pDef.zName,nName);
			if(pOther!=null) {
				Debug.Assert(pOther!=pDef&&pOther.pNext!=pDef);
				pDef.pNext=pOther.pNext;
				pOther.pNext=pDef;
			}
			else {
				pDef.pNext=null;
				pDef.pHash=pHash.a[h];
				pHash.a[h]=pDef;
			}
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
		static FuncDef sqlite3FindFunction(sqlite3 db,///
		///<summary>
		///An open database 
		///</summary>
		string zName,///
		///<summary>
		///</summary>
		///<param name="Name of the function.  Not null">terminated </param>
		int nName,///
		///<summary>
		///Number of characters in the name 
		///</summary>
		int nArg,///
		///<summary>
		///</summary>
		///<param name="Number of arguments.  ">1 means any number </param>
		SqliteEncoding enc,///
		///<summary>
		///Preferred text encoding 
		///</summary>
		u8 createFlag///
		///<summary>
		///Create new entry if true and does not otherwise exist 
		///</summary>
		) {
			FuncDef p;
			///
			///<summary>
			///Iterator variable 
			///</summary>
			FuncDef pBest=null;
			///
			///<summary>
			///Best match found so far 
			///</summary>
			int bestScore=0;
			int h;
			///
			///<summary>
			///Hash value 
			///</summary>
			Debug.Assert(enc==SqliteEncoding.UTF8||enc==SqliteEncoding.UTF16LE||enc==SqliteEncoding.UTF16BE);
			h=(_Custom.sqlite3UpperToLower[(u8)zName[0]]+nName)%Sqlite3.ArraySize(db.aFunc.a);
			///
			///<summary>
			///</summary>
			///<param name="First search for a match amongst the application">defined functions.</param>
			///<param name=""></param>
			p=functionSearch(db.aFunc,h,zName,nName);
			while(p!=null) {
				int score=matchQuality(p,nArg,enc);
				if(score>bestScore) {
					pBest=p;
					bestScore=score;
				}
				p=p.pNext;
			}
			///
			///<summary>
			///</summary>
			///<param name="If no match is found, search the built">in functions.</param>
			///<param name=""></param>
			///<param name="If the SQLITE_PreferBuiltin flag is set, then search the built">in</param>
			///<param name="functions even if a prior app">defined function was found.  And give</param>
			///<param name="priority to built">in functions.</param>
			///<param name=""></param>
			///<param name="Except, if createFlag is true, that means that we are trying to">Except, if createFlag is true, that means that we are trying to</param>
			///<param name="install a new function.  Whatever FuncDef structure is returned it will">install a new function.  Whatever FuncDef structure is returned it will</param>
			///<param name="have fields overwritten with new information appropriate for the">have fields overwritten with new information appropriate for the</param>
			///<param name="new function.  But the FuncDefs for built">only.</param>
			///<param name="So we must not search for built">ins when creating a new function.</param>
			///<param name=""></param>
            if (0 == createFlag && (pBest == null || (db.flags & SqliteFlags.SQLITE_PreferBuiltin) != 0))
            {
				#if SQLITE_OMIT_WSD
																																																																																				FuncDefHash pHash = GLOBAL( FuncDefHash, sqlite3GlobalFunctions );
#else
				FuncDefHash pHash=sqlite3GlobalFunctions;
				#endif
				bestScore=0;
				p=functionSearch(pHash,h,zName,nName);
				while(p!=null) {
					int score=matchQuality(p,nArg,enc);
					if(score>bestScore) {
						pBest=p;
						bestScore=score;
					}
					p=p.pNext;
				}
			}
			///
			///<summary>
			///If the createFlag parameter is true and the search did not reveal an
			///exact match for the name, number of arguments and encoding, then add a
			///new entry to the hash table and return it.
			///
			///</summary>
			if(createFlag!=0&&(bestScore<6||pBest.nArg!=nArg)&&(pBest=new FuncDef())!=null) {
				//sqlite3DbMallocZero(db, sizeof(*pBest)+nName+1))!=0 ){
				//pBest.zName = (char *)&pBest[1];
				pBest.nArg=(i16)nArg;
				pBest.iPrefEnc=enc;
				pBest.zName=zName;
				//memcpy(pBest.zName, zName, nName);
				//pBest.zName[nName] = 0;
				sqlite3FuncDefInsert(db.aFunc,pBest);
			}
			if(pBest!=null&&(pBest.xStep!=null||pBest.xFunc!=null||createFlag!=0)) {
				return pBest;
			}
			return null;
		}
		///<summary>
		/// Free all resources held by the schema structure. The void* argument points
		/// at a Schema struct. This function does not call sqlite3DbFree(db, ) on the
		/// pointer itself, it just cleans up subsidiary resources (i.e. the contents
		/// of the schema hash tables).
		///
		/// The Schema.cache_size variable is not cleared.
		///
		///</summary>
		static void sqlite3SchemaClear(Schema p) {
			Hash temp1;
			Hash temp2;
			HashElem pElem;
			Schema pSchema=p;
			temp1=pSchema.tblHash;
			temp2=pSchema.trigHash;
			pSchema.trigHash.sqlite3HashInit();
			sqlite3HashClear(pSchema.idxHash);
			for(pElem=sqliteHashFirst(temp2);pElem!=null;pElem=sqliteHashNext(pElem)) {
				Trigger pTrigger=(Trigger)sqliteHashData(pElem);
				sqlite3DeleteTrigger(null,ref pTrigger);
			}
			sqlite3HashClear(temp2);
			pSchema.trigHash.sqlite3HashInit();
			for(pElem=temp1.first;pElem!=null;pElem=pElem.next)//sqliteHashFirst(&temp1); pElem; pElem = sqliteHashNext(pElem))
			 {
				Table pTab=(Table)pElem.data;
				//sqliteHashData(pElem);
				build.sqlite3DeleteTable(null,ref pTab);
			}
			sqlite3HashClear(temp1);
			sqlite3HashClear(pSchema.fkeyHash);
			pSchema.pSeqTab=null;
            if ((pSchema.flags & sqliteinth.DB_SchemaLoaded) != 0)
            {
				pSchema.iGeneration++;
                pSchema.flags = (u16)(pSchema.flags & (~sqliteinth.DB_SchemaLoaded));
			}
			p.Clear();
		}
		///
		///<summary>
		///Find and return the schema associated with a BTree.  Create
		///a new one if necessary.
		///
		///</summary>
		static Schema sqlite3SchemaGet(sqlite3 db,Btree pBt) {
			Schema p;
			if(pBt!=null) {
				p=pBt.sqlite3BtreeSchema(-1,(dxFreeSchema)sqlite3SchemaClear);
				//Schema.Length, sqlite3SchemaFree);
			}
			else {
				p=new Schema();
				// (Schema *)sqlite3DbMallocZero(0, sizeof(Schema));
			}
			if(p==null) {
				////        db.mallocFailed = 1;
			}
			else
				if(0==p.file_format) {
					p.tblHash.sqlite3HashInit();
					p.idxHash.sqlite3HashInit();
					p.trigHash.sqlite3HashInit();
					p.fkeyHash.sqlite3HashInit();
					p.enc=SqliteEncoding.UTF8;
				}
			return p;
		}
	}
}
