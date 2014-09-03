using u8 = System.Byte;
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
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
		public class _ht
		{
			///
///<summary>
///the hash table 
///</summary>

			public int count;

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


		public class Hash
		{
			public u32 htsize = 31;

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

			public Hash Copy ()
			{
				if (this == null)
					return null;
				else {
					Hash cp = (Hash)MemberwiseClone ();
					return cp;
				}
			}
		};


		///
///<summary>
///Each element in the hash table is an instance of the following
///</summary>
///<param name="structure.  All elements are stored on a single doubly">linked list.</param>
///<param name=""></param>
///<param name="Again, this structure is intended to be opaque, but it can't really">Again, this structure is intended to be opaque, but it can't really</param>
///<param name="be opaque because it is used by macros.">be opaque because it is used by macros.</param>
///<param name=""></param>

		public class HashElem
		{
			public HashElem next;

			public HashElem prev;

			///
///<summary>
///Next and previous elements in the table 
///</summary>

			public object data;

			///
///<summary>
///Data associated with this element 
///</summary>

			public string pKey;

			public int nKey;
		///
///<summary>
///Key associated with this element 
///</summary>

		};


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
		static HashElem sqliteHashFirst (Hash H)
		{
			return H.first;
		}

		//#define sqliteHashNext(E)   ((E).next)
		static HashElem sqliteHashNext (HashElem E)
		{
			return E.next;
		}

		//#define sqliteHashData(E)   ((E).data)
		static object sqliteHashData (HashElem E)
		{
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
