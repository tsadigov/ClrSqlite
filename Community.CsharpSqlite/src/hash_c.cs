using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	public partial class Sqlite3 {
		///<summary>
		/// 2001 September 22
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		/// This is the implementation of generic hash-tables
		/// used in SQLite.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
		///
		///
		///
		///</summary>
		//#include "sqliteInt.h"
		//#include <assert.h>
		///<summary>
		///Turn bulk memory into a hash table object by initializing the
		/// fields of the Hash structure.
		///
		/// "pNew" is a pointer to the hash table that is to be initialized.
		///
		///</summary>
		///<summary>
		///Remove all entries from a hash table.  Reclaim all memory.
		/// Call this routine to delete a hash table or to reset a hash table
		/// to the empty state.
		///
		///</summary>
		public static void sqlite3HashClear(Hash pH) {
			HashElem elem;
			///
			///<summary>
			///For looping over all elements of the table 
			///</summary>
			Debug.Assert(pH!=null);
			elem=pH.first;
			pH.first=null;
			//malloc_cs.sqlite3_free( ref pH.ht );
			pH.ht=null;
			pH.htsize=0;
			while(elem!=null) {
				HashElem next_elem=elem.next;
				////malloc_cs.sqlite3_free(ref  elem );
				elem=next_elem;
			}
			pH.count=0;
		}
		///<summary>
		/// The hashing function.
		///
		///</summary>
		public static u32 strHash(string z,int nKey) {
			int h=0;
			Debug.Assert(nKey>=0);
			int _z=0;
			while(nKey>0) {
				h=(h<<3)^h^((_z<z.Length)?(int)_Custom.sqlite3UpperToLower[(byte)z[_z++]]:0);
				nKey--;
			}
			return (u32)h;
		}
		///<summary>
		///Link pNew element into the hash table pH.  If pEntry!=0 then also
		/// insert pNew into the pEntry hash bucket.
		///
		///</summary>
		static void insertElement(Hash pH,///
		///<summary>
		///The complete hash table 
		///</summary>
		_ht pEntry,///
		///<summary>
		///The entry into which pNew is inserted 
		///</summary>
		HashElem pNew///
		///<summary>
		///The element to be inserted 
		///</summary>
		) {
			HashElem pHead;
			///
			///<summary>
			///First element already in pEntry 
			///</summary>
			if(pEntry!=null) {
				pHead=pEntry.count!=0?pEntry.chain:null;
				pEntry.count++;
				pEntry.chain=pNew;
			}
			else {
				pHead=null;
			}
			if(pHead!=null) {
				pNew.next=pHead;
				pNew.prev=pHead.prev;
				if(pHead.prev!=null) {
					pHead.prev.next=pNew;
				}
				else {
					pH.first=pNew;
				}
				pHead.prev=pNew;
			}
			else {
				pNew.next=pH.first;
				if(pH.first!=null) {
					pH.first.prev=pNew;
				}
				pNew.prev=null;
				pH.first=pNew;
			}
		}
		///<summary>
		///Resize the hash table so that it cantains "new_size" buckets.
		///
		/// The hash table might fail to resize if sqlite3_malloc() fails or
		/// if the new size is the same as the prior size.
		/// Return TRUE if the resize occurs and false if not.
		///
		///</summary>
		static bool rehash(ref Hash pH,u32 new_size) {
			_ht[] new_ht;
			///
			///<summary>
			///The new hash table 
			///</summary>
			HashElem elem;
			HashElem next_elem;
			///
			///<summary>
			///For looping over existing elements 
			///</summary>
			#if SQLITE_MALLOC_SOFT_LIMIT
																																																												if( new_size*sizeof(struct _ht)>SQLITE_MALLOC_SOFT_LIMIT ){
new_size = SQLITE_MALLOC_SOFT_LIMIT/sizeof(struct _ht);
}
if( new_size==pH->htsize ) return false;
#endif
			///
			///<summary>
			///There is a call to malloc_cs.sqlite3Malloc() inside rehash(). If there is
			///already an allocation at pH.ht, then if this malloc() fails it
			///is benign (since failing to resize a hash table is a performance
			///hit only, not a fatal error).
			///</summary>
			sqlite3BeginBenignMalloc();
			new_ht=new _ht[new_size];
			//(struct _ht )malloc_cs.sqlite3Malloc( new_size*sizeof(struct _ht) );
			for(int i=0;i<new_size;i++)
				new_ht[i]=new _ht();
			sqlite3EndBenignMalloc();
			if(new_ht==null)
				return false;
			//malloc_cs.sqlite3_free( ref pH.ht );
			pH.ht=new_ht;
			// pH.htsize = new_size = malloc_cs.sqlite3MallocSize(new_ht)/sizeof(struct _ht);
			//memset(new_ht, 0, new_size*sizeof(struct _ht));
			pH.htsize=new_size;
			for(elem=pH.first,pH.first=null;elem!=null;elem=next_elem) {
				u32 h=strHash(elem.pKey,elem.nKey)%new_size;
				next_elem=elem.next;
				insertElement(pH,new_ht[h],elem);
			}
			return true;
		}
		///<summary>
		///This function (for internal use only) locates an element in an
		/// hash table that matches the given key.  The hash for this key has
		/// already been computed and is passed as the 4th parameter.
		///
		///</summary>
		///<summary>
		///Remove a single entry from the hash table given a pointer to that
		/// element and a hash on the element's key.
		///
		///</summary>
		static void removeElementGivenHash(Hash pH,///
		///<summary>
		///The pH containing "elem" 
		///</summary>
		ref HashElem elem,///
		///<summary>
		///The element to be removed from the pH 
		///</summary>
		u32 h///
		///<summary>
		///Hash value for the element 
		///</summary>
		) {
			_ht pEntry;
			if(elem.prev!=null) {
				elem.prev.next=elem.next;
			}
			else {
				pH.first=elem.next;
			}
			if(elem.next!=null) {
				elem.next.prev=elem.prev;
			}
			if(pH.ht!=null&&pH.ht[h]!=null) {
				pEntry=pH.ht[h];
				if(pEntry.chain==elem) {
					pEntry.chain=elem.next;
				}
				pEntry.count--;
				Debug.Assert(pEntry.count>=0);
			}
			//malloc_cs.sqlite3_free( ref elem );
			pH.count--;
			if(pH.count<=0) {
				Debug.Assert(pH.first==null);
				Debug.Assert(pH.count==0);
				sqlite3HashClear(pH);
			}
		}
		///<summary>
		///Attempt to locate an element of the hash table pH with a key
		/// that matches pKey,nKey.  Return the data for this element if it is
		/// found, or NULL if there is no match.
		///
		///</summary>
		///
		///<summary>
		///Insert an element into the hash table pH.  The key is pKey,nKey
		///and the data is "data".
		///
		///If no element exists with a matching key, then a new
		///element is created and NULL is returned.
		///
		///If another element already exists with the same key, then the
		///new data replaces the old data and the old data is returned.
		///The key is not copied in this instance.  If a malloc fails, then
		///the new data is returned and the hash table is unchanged.
		///
		///If the "data" parameter to this function is NULL, then the
		///element corresponding to "key" is removed from the hash table.
		///
		///</summary>
		public static T sqlite3HashInsert<T>(ref Hash pH,string pKey,int nKey,T data) where T : class {
			u32 h;
			///
			///<summary>
			///the hash of the key modulo hash table size 
			///</summary>
			HashElem elem;
			///
			///<summary>
			///Used to loop thru the element list 
			///</summary>
			HashElem new_elem;
			///
			///<summary>
			///New element added to the pH 
			///</summary>
			Debug.Assert(pH!=null);
			Debug.Assert(pKey!=null);
			Debug.Assert(nKey>=0);
			if(pH.htsize!=0) {
				h=strHash(pKey,nKey)%pH.htsize;
			}
			else {
				h=0;
			}
			elem=pH.findElementGivenHash(pKey,nKey,h);
			if(elem!=null) {
				T old_data=(T)elem.data;
				if(data==null) {
					removeElementGivenHash(pH,ref elem,h);
				}
				else {
					elem.data=data;
					elem.pKey=pKey;
					Debug.Assert(nKey==elem.nKey);
				}
				return old_data;
			}
			if(data==null)
				return data;
			new_elem=new HashElem();
			//(HashElem)malloc_cs.sqlite3Malloc( sizeof(HashElem) );
			if(new_elem==null)
				return data;
			new_elem.pKey=pKey;
			new_elem.nKey=nKey;
			new_elem.data=data;
			pH.count++;
			if(pH.count>=10&&pH.count>2*pH.htsize) {
				if(rehash(ref pH,pH.count*2)) {
					Debug.Assert(pH.htsize>0);
					h=strHash(pKey,nKey)%pH.htsize;
				}
			}
			if(pH.ht!=null) {
				insertElement(pH,pH.ht[h],new_elem);
			}
			else {
				insertElement(pH,null,new_elem);
			}
			return null;
		}
	}
}
