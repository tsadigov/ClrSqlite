using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pgno = System.UInt32;
using i64 = System.Int64;
using u32 = System.UInt32;
using BITVEC_TELEM = System.Byte;
using System.Diagnostics;

namespace Community.CsharpSqlite
{
	public static class BitvecExtensions
	{
		public static int sqlite3BitvecTest (Community.CsharpSqlite.Sqlite3.Bitvec _this, u32 i)
		{
			if (_this == null || i == 0)
				return 0;
			if (i > _this.iSize)
				return 0;
			i--;
			while (_this.iDivisor != 0) {
				u32 bin = i / _this.iDivisor;
				i = i % _this.iDivisor;
				_this = _this.u.apSub [bin];
				if (null == _this) {
					return 0;
				}
			}
			if (_this.iSize <= Sqlite3.BITVEC_NBIT) {
				return ((_this.u.aBitmap [i / Sqlite3.BITVEC_SZELEM] & (1 << (int)(i & (Sqlite3.BITVEC_SZELEM - 1)))) != 0) ? 1 : 0;
			}
			else {
				u32 h = Sqlite3.BITVEC_HASH (i++);
				while (_this.u.aHash [h] != 0) {
					if (_this.u.aHash [h] == i)
						return 1;
					h = (h + 1) % Sqlite3.BITVEC_NINT;
				}
				return 0;
			}
		}

		public static int sqlite3BitvecSet (this Community.CsharpSqlite.Sqlite3.Bitvec _this, u32 i)
		{
			u32 h;
			if (_this == null)
				return Sqlite3.SQLITE_OK;
			Debug.Assert (i > 0);
			Debug.Assert (i <= _this.iSize);
			i--;
			while ((_this.iSize > Sqlite3.BITVEC_NBIT) && _this.iDivisor != 0) {
				u32 bin = i / _this.iDivisor;
				i = i % _this.iDivisor;
				if (_this.u.apSub [bin] == null) {
					_this.u.apSub [bin] = Sqlite3.sqlite3BitvecCreate (_this.iDivisor);
					//if ( p.u.apSub[bin] == null )
					//  return SQLITE_NOMEM;
				}
				_this = _this.u.apSub [bin];
			}
			if (_this.iSize <= Sqlite3.BITVEC_NBIT) {
				_this.u.aBitmap [i / Sqlite3.BITVEC_SZELEM] |= (byte)(1 << (int)(i & (Sqlite3.BITVEC_SZELEM - 1)));
				return Sqlite3.SQLITE_OK;
			}
			h = Sqlite3.BITVEC_HASH (i++);
			///
///<summary>
///if there wasn't a hash collision, and _this doesn't 
///</summary>

			///
///<summary>
///completely fill the hash, then just add it without 
///</summary>

			///
///<summary>
///</summary>
///<param name="worring about sub">hashing. </param>

			if (0 == _this.u.aHash [h]) {
				if (_this.nSet < (Sqlite3.BITVEC_NINT - 1)) {
					goto bitvec_set_end;
				}
				else {
					goto bitvec_set_rehash;
				}
			}
			///
///<summary>
///there was a collision, check to see if it's already 
///</summary>

			///
///<summary>
///in hash, if not, try to find a spot for it 
///</summary>

			do {
				if (_this.u.aHash [h] == i)
					return Sqlite3.SQLITE_OK;
				h++;
				if (h >= Sqlite3.BITVEC_NINT)
					h = 0;
			}
			while (_this.u.aHash [h] != 0);
			///
///<summary>
///we didn't find it in the hash.  h points to the first 
///</summary>

			///
///<summary>
///available free spot. check to see if _this is going to 
///</summary>

			///
///<summary>
///make our hash too "full".  
///</summary>

			bitvec_set_rehash:
			if (_this.nSet >= Sqlite3.BITVEC_MXHASH) {
				u32 j;
				int rc;
				u32[] aiValues = new u32[Sqlite3.BITVEC_NINT];
				// = sqlite3StackAllocRaw(0, sizeof(p->u.aHash));
				//if ( aiValues == null )
				//{
				//  return SQLITE_NOMEM;
				//}
				//else
				{
					Buffer.BlockCopy (_this.u.aHash, 0, aiValues, 0, aiValues.Length * (sizeof(u32)));
					// memcpy(aiValues, p->u.aHash, sizeof(p->u.aHash));
					_this.u.apSub = new Community.CsharpSqlite.Sqlite3.Bitvec[Sqlite3.BITVEC_NPTR];
					//memset(p->u.apSub, 0, sizeof(p->u.apSub));
					_this.iDivisor = (u32)((_this.iSize + Sqlite3.BITVEC_NPTR - 1) / Sqlite3.BITVEC_NPTR);
					rc = _this.sqlite3BitvecSet (i);
					for (j = 0; j < Sqlite3.BITVEC_NINT; j++) {
						if (aiValues [j] != 0)
							rc |= _this.sqlite3BitvecSet (aiValues [j]);
					}
					//sqlite3StackFree( null, aiValues );
					return rc;
				}
			}
			bitvec_set_end:
			_this.nSet++;
			_this.u.aHash [h] = i;
			return Sqlite3.SQLITE_OK;
		}

		public static void sqlite3BitvecClear (this Community.CsharpSqlite.Sqlite3.Bitvec _this, u32 i, u32[] pBuf)
		{
			if (_this == null)
				return;
			Debug.Assert (i > 0);
			i--;
			while (_this.iDivisor != 0) {
				u32 bin = i / _this.iDivisor;
				i = i % _this.iDivisor;
				_this = _this.u.apSub [bin];
				if (null == _this) {
					return;
				}
			}
			if (_this.iSize <= Sqlite3.BITVEC_NBIT) {
				_this.u.aBitmap [i / Sqlite3.BITVEC_SZELEM] &= (byte)~((1 << (int)(i & (Sqlite3.BITVEC_SZELEM - 1))));
			}
			else {
				u32 j;
				u32[] aiValues = pBuf;
				Array.Copy (_this.u.aHash, aiValues, _this.u.aHash.Length);
				//memcpy(aiValues, p->u.aHash, sizeof(p->u.aHash));
				_this.u.aHash = new u32[aiValues.Length];
				// memset(p->u.aHash, 0, sizeof(p->u.aHash));
				_this.nSet = 0;
				for (j = 0; j < Sqlite3.BITVEC_NINT; j++) {
					if (aiValues [j] != 0 && aiValues [j] != (i + 1)) {
						u32 h = Sqlite3.BITVEC_HASH (aiValues [j] - 1);
						_this.nSet++;
						while (_this.u.aHash [h] != 0) {
							h++;
							if (h >= Sqlite3.BITVEC_NINT)
								h = 0;
						}
						_this.u.aHash [h] = aiValues [j];
					}
				}
			}
		}
	}
}
