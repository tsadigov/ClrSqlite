using u8=System.Byte;
using u32=System.UInt32;
using System;
using System.Diagnostics;
namespace Community.CsharpSqlite.Utils {
	
		///
		///<summary>
		///2001 September 22
		///
		///The author disclaims copyright to this source code.  In place of
		///a legal notice, here is a blessing:
		///
		///May you do good and not evil.
		///May you find forgiveness for yourself and forgive others.
		///May you share freely, never taking more than you give.
		///
		///
		///</summary>
		///<param name="This is the header file for the generic hash">table implemenation</param>
		///<param name="used in SQLite.">used in SQLite.</param>
		///<param name=""></param>
		///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
		///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
		///<param name=""></param>
		///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
		///<param name=""></param>
		///<param name=""></param>
		///<param name=""></param>
		//#if !_SQLITE_HASH_H_
		//#define _SQLITE_HASH_H_
		///<summary>
		///Forward declarations of structures.
		///</summary>
		//typedef struct Hash Hash;
		//typedef struct HashElem HashElem;
		///<summary>
		///A complete hash table is an instance of the following structure.
		/// The internals of this structure are intended to be opaque -- client
		/// code should not attempt to access or modify the fields of this structure
		/// directly.  Change this structure only by using the routines below.
		/// However, some of the "procedures" and "functions" for modifying and
		/// accessing this structure are really macros, so we can't really make
		/// this structure opaque.
		///
		/// All elements of the hash table are on a single doubly-linked list.
		/// Hash.first points to the head of this list.
		///
		/// There are Hash.htsize buckets.  Each bucket points to a spot in
		/// the global doubly-linked list.  The contents of the bucket are the
		/// element pointed to plus the next _ht.count-1 elements in the list.
		///
		/// Hash.htsize and Hash.ht may be zero.  In that case lookup is done
		/// by a linear search of the global list.  For small tables, the
		/// Hash.ht table is never allocated because if there are few elements
		/// in the table, it is faster to do a linear search than to manage
		/// the hash table.
		///
		///</summary>
		public class _ht {
			///
			///<summary>
			///the hash table 
			///</summary>
			private int _count;
			public int count {
				get {
					return _count;
				}
				set {
					_count=value;
				}
			}
			///
			///<summary>
			///Number of entries with this hash 
			///</summary>
			public HashElem chain;
		///
		///<summary>
		///Pointer to first entry with this hash 
		///</summary>
		};

		public class Hash {
			public u32 htsize=31;
			///
			///<summary>
			///Number of buckets in the hash table 
			///</summary>
			public u32 count;
			///
			///<summary>
			///Number of entries in this table 
			///</summary>
			public HashElem first;
			///<summary>
			///The first element of the array
			///</summary>
			public _ht[] ht;
			public Hash Copy() {
				if(this==null)
					return null;
				else {
					Hash cp=(Hash)MemberwiseClone();
					return cp;
				}
			}
            public T Find<T>(string pKey, int nKey, T nullType) where T : class {
                Debug.Assert(this != null);
                Debug.Assert(pKey != null);
                Debug.Assert(nKey >= 0);
                return sqlite3HashFind(pKey.sub(nKey), nullType);
            }
			public T sqlite3HashFind<T>(Str key,T nullType) where T : class {
				///The element that matches key 
				u32 h;
				///A hash on key 
				if(this.ht!=null) {
					h=key.Hash%this.htsize;
				}
				else {
					h=0;
				}
				var elem=this.findElementGivenHash(key,h);
				return elem!=null?(T)elem.data:nullType;
			}
            public HashElem findElementGivenHash(
                string pKey,///The key we are searching for 
                int nKey,///Bytes in key (not counting zero terminator) 
                u32 h///The hash for this key. 
            ) {
                return findElementGivenHash(pKey.sub(nKey),h);
            }
            public HashElem findElementGivenHash(
			    Str key,
                u32 h///The hash for this key. 
			) {
				HashElem elem;
				///Used to loop thru the element list 
				int count;
				///Number of elements left to test 
				if(this.ht!=null&&this.ht[h]!=null) {
					_ht pEntry=this.ht[h];
					elem=pEntry.chain;
					count=(int)pEntry.count;
				}
				else {
					elem=this.first;
					count=(int)this.count;
				}
				while(count-->0&&Sqlite3.ALWAYS(elem)) {
					if(elem.key==key ) {
						return elem;
					}
					elem=elem.next;
				}
				return null;
			}

			public void sqlite3HashInit() {
				Debug.Assert(this!=null);
				this.first=null;
				this.count=0;
				this.htsize=0;
				this.ht=null;
			}

            ///<summary>
            ///This function (for internal use only) locates an element in an
            /// hash table that matches the given key.  The hash for this key has
            /// already been computed and is passed as the 4th parameter.
            ///
            ///Remove a single entry from the hash table given a pointer to that
            /// element and a hash on the element's key.
            ///
            ///</summary>
            void removeElementGivenHash(
                ref HashElem elem,///The element to be removed from the pH 
                u32 h///Hash value for the element 
            )
            {
                Hash pH = this;

                _ht pEntry;
                if (elem.prev != null)
                {
                    elem.prev.next = elem.next;
                }
                else
                {
                    pH.first = elem.next;
                }
                if (elem.next != null)
                {
                    elem.next.prev = elem.prev;
                }
                if (pH.ht != null && pH.ht[h] != null)
                {
                    pEntry = pH.ht[h];
                    if (pEntry.chain == elem)
                    {
                        pEntry.chain = elem.next;
                    }
                    pEntry.count--;
                    Debug.Assert(pEntry.count >= 0);
                }
                //malloc_cs.sqlite3_free( ref elem );
                pH.count--;
                if (pH.count <= 0)
                {
                    Debug.Assert(pH.first == null);
                    Debug.Assert(pH.count == 0);
                    pH.sqlite3HashClear();
                }
            }

            public T HashInsert<T>(Str str, T data) where T:class
            {
                var pH = this;
                u32 h;
                if (pH.htsize != 0)
                {
                    h = str.Hash % pH.htsize;
                }
                else
                {
                    h = 0;
                }

                ///the hash of the key modulo hash table size 
                var elem = pH.findElementGivenHash(str.buffer, str.length, h);
                if (elem != null)
                {
                    T old_data = (T)elem.data;
                    if (data == null)
                    {
                        removeElementGivenHash(ref elem, h);
                    }
                    else
                    {
                        elem.data = data;
                        elem.pKey = str.buffer;
                        Debug.Assert(str.length == elem.key.length);
                    }
                    return old_data;
                }
                if (data == null)
                    return data;

                ///Used to loop thru the element list 
                var new_elem = new HashElem();
                //(HashElem)malloc_cs.sqlite3Malloc( sizeof(HashElem) );
                if (new_elem == null)
                    return data;
                new_elem.key = str;
                new_elem.data = data;
                pH.count++;
                if (pH.count >= 10 && pH.count > 2 * pH.htsize)
                {
                    if (pH.rehash(pH.count * 2))
                    {
                        Debug.Assert(pH.htsize > 0);
                        h = str.Hash % pH.htsize;
                    }
                }
                if (pH.ht != null)
                {
                    insertElement( pH.ht[h], new_elem);
                }
                else
                {
                    insertElement( null, new_elem);
                }
                return null;
            }
            ///<summary>
            ///Resize the hash table so that it cantains "new_size" buckets.
            ///
            /// The hash table might fail to resize if sqlite3_malloc() fails or
            /// if the new size is the same as the prior size.
            /// Return TRUE if the resize occurs and false if not.
            ///
            ///</summary>
            public bool rehash( u32 new_size)
            {
                Hash pH = this;
                _ht[] new_ht;
                ///The new hash table 
                HashElem next_elem;
                ///For looping over existing elements 
#if SQLITE_MALLOC_SOFT_LIMIT
																																																												if( new_size*sizeof(struct _ht)>SQLITE_MALLOC_SOFT_LIMIT ){
new_size = SQLITE_MALLOC_SOFT_LIMIT/sizeof(struct _ht);
}
if( new_size==pH->htsize ) return false;
#endif
                ///There is a call to malloc_cs.sqlite3Malloc() inside rehash(). If there is
                ///already an allocation at pH.ht, then if this malloc() fails it
                ///is benign (since failing to resize a hash table is a performance
                ///hit only, not a fatal error).
                Sqlite3.sqlite3BeginBenignMalloc();
                new_ht = new _ht[new_size];
                //(struct _ht )malloc_cs.sqlite3Malloc( new_size*sizeof(struct _ht) );
                for (int i = 0; i < new_size; i++)
                    new_ht[i] = new _ht();
                Sqlite3.sqlite3EndBenignMalloc();
                if (new_ht == null)
                    return false;
                //malloc_cs.sqlite3_free( ref pH.ht );
                pH.ht = new_ht;
                // pH.htsize = new_size = malloc_cs.sqlite3MallocSize(new_ht)/sizeof(struct _ht);
                //memset(new_ht, 0, new_size*sizeof(struct _ht));
                pH.htsize = new_size;
                var oldstart=pH.first;
                pH.first =null;
                //for (var elem = pH.first, pH.first = null; elem != null; elem = next_elem)
                foreach (var elem in oldstart.path(x => x.next))
                {
                    u32 h = elem.key.Hash % new_size;
                    insertElement( new_ht[h], elem);
                }
                return true;
            }


            ///<summary>
            ///Link pNew element into the hash table pH.  If pEntry!=0 then also
            /// insert pNew into the pEntry hash bucket.
            ///
            ///</summary>
            void insertElement(
                _ht pEntry,///The entry into which pNew is inserted 
                HashElem pNew///The element to be inserted 
            )
            {
                Hash pH = this;
                HashElem pHead;
                ///First element already in pEntry 
                if (pEntry != null)
                {
                    pHead = pEntry.count != 0 ? pEntry.chain : null;
                    pEntry.count++;
                    pEntry.chain = pNew;
                }
                else
                {
                    pHead = null;
                }
                if (pHead != null)
                {
                    pNew.next = pHead;
                    pNew.prev = pHead.prev;
                    if (pHead.prev != null)
                    {
                        pHead.prev.next = pNew;
                    }
                    else
                    {
                        pH.first = pNew;
                    }
                    pHead.prev = pNew;
                }
                else
                {
                    pNew.next = pH.first;
                    if (pH.first != null)
                    {
                        pH.first.prev = pNew;
                    }
                    pNew.prev = null;
                    pH.first = pNew;
                }
            }
        }


		
        ///
		///<summary>
		///Each element in the hash table is an instance of the following
		///</summary>
		///<param name="structure.  All elements are stored on a single doubly">linked list.</param>
		///<param name=""></param>
		///<param name="Again, this structure is intended to be opaque, but it can't really">Again, this structure is intended to be opaque, but it can't really</param>
		///<param name="be opaque because it is used by macros.">be opaque because it is used by macros.</param>
		///<param name=""></param>
		public class HashElem {
			public HashElem next;
			public HashElem prev;
			///
			///<summary>
			///Next and previous elements in the table 
			///</summary>
			private object _data;
			public object data {
				get {
					return _data;
				}
				set {
					_data=value;
				}
			}

            public Str key = new Str("",0);
			///
			///<summary>
			///Data associated with this element 
			///</summary>
			public string pKey {
				get {
					return key.buffer;
				}
				set {
                    key.buffer = value; ;
				}
			}

            ///<summary>
            ///Key associated with this element 
            ///</summary>
            public int nKey {
				get {
                    return key.length;
				}
				set {
                    key.length = value;
				}
			}
		
		};
        public static partial class HashExtensions
        {
		///
		///<summary>
		///Access routines.  To delete, insert a NULL pointer.
		///
		///</summary>
		//void sqlite3HashInit(Hash);
		//void *sqlite3HashInsert(Hash*, string pKey, int nKey, object  *pData);
		//void *sqlite3HashFind(const Hash*, string pKey, int nKey);
		//void sqlite3HashClear(Hash);
		///<summary>
		/// Macros for looping over all elements of a hash table.  The idiom is
		/// like this:
		///
		///   Hash h;
		///   HashElem p;
		///   ...
		///   for(p=sqliteHashFirst(&h); p; p=sqliteHashNext(p)){
		///     SomeStructure pData = sqliteHashData(p);
		///     // do something with pData
		///   }
		///
		///</summary>
		//#define sqliteHashFirst(H)  ((H).first)
		public static HashElem sqliteHashFirst(this Hash H) {
			return H.first;
		}
		//#define sqliteHashNext(E)   ((E).next)
		public static HashElem sqliteHashNext(this HashElem E) {
			return E.next;
		}
		//#define sqliteHashData(E)   ((E).data)
		public static object sqliteHashData(this HashElem E) {
			return E.data;
		}
	///
	///<summary>
	///</summary>
	///<param name="#define sqliteHashKey(E)    ((E)">>pKey) // NOT USED </param>
	///
	///<summary>
	///</summary>
	///<param name="#define sqliteHashKeysize(E) ((E)">>nKey)  // NOT USED </param>
	///
	///<summary>
	///Number of entries in a hash table
	///
	///</summary>
	///
	///<summary>
	///</summary>
	///<param name="#define sqliteHashCount(H)  ((H)">>count) // NOT USED </param>
	//#endif // * _SQLITE_HASH_H_ */
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