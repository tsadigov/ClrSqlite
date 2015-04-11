using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	public static partial class HashExtensions {
		
		public static void sqlite3HashClear(this Hash pH) {
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
				h=(h<<3)^h^((_z<z.Length)?(int)Sqlite3._Custom.sqlite3UpperToLower[(byte)z[_z++]]:0);
				nKey--;
			}
			return (u32)h;
		}

		
		
		public static T sqlite3HashInsert<T>(ref Hash pH,string pKey,int nKey,T data) where T : class {
			
			///New element added to the pH 
			Debug.Assert(pH!=null);
			Debug.Assert(pKey!=null);
			Debug.Assert(nKey>=0);

            T result = pH.HashInsert(pKey.sub(nKey),data);
            return result;
		}
	}
}
