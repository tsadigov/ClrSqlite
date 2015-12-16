using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;
namespace Community.CsharpSqlite
{
    using Community.CsharpSqlite.Engine;
    using System.Diagnostics;
    using Utils;
    public static class Extensions
    {
        public static Engine.VdbeFrame GetRoot(this Engine.VdbeFrame _this)
        {
            Engine.VdbeFrame pFrame = null;
            if (null != pFrame)
                pFrame = pFrame.path( x => x.pParent).Last();

            return pFrame;
        }
        ///<summary>
        /// Try to convert a value into a numeric representation if we can
        /// do so without loss of information.  In other words, if the string
        /// looks like a number, convert it into a number.  If it does not
        /// look like a number, leave it alone.
        ///
        ///</summary>
        public static void applyNumericAffinity(this Mem pRec)
        {
            if ((pRec.flags & (MemFlags.MEM_Real | MemFlags.MEM_Int)) == 0)
            {
                double rValue = 0.0;
                i64 iValue = 0;
                SqliteEncoding enc = pRec.enc;
                if ((pRec.flags & MemFlags.MEM_Str) == 0)
                    return;
                if (Converter.sqlite3AtoF(pRec.z, ref rValue, pRec.n, enc) == false)
                    return;
                if (0 == Converter.sqlite3Atoi64(pRec.z, ref iValue, pRec.n, enc))
                {
                    pRec.u.i = iValue;
                    pRec.flags |= MemFlags.MEM_Int;
                }
                else
                {
                    pRec.r = rValue;
                    pRec.flags |= MemFlags.MEM_Real;
                }
            }
        }

        ///<summary>
        /// Call sqlite3VdbeMemExpandBlob() on the supplied value (type Mem)
        /// P if required.
        ///
        ///</summary>
        //#define ExpandBlob(P) (((P).flags&MEM.MEM_Zero)?sqlite3VdbeMemExpandBlob(P):0)
        public static SqlResult ExpandBlob(this Mem P)
        {
            return (P.flags & MemFlags.MEM_Zero) != 0 ? P.sqlite3VdbeMemExpandBlob() : (SqlResult)0;
        }

#if SQLITE_DEBUG
																																																		    // define memAboutToChange(P,M) sqlite3VdbeMemPrepareToChange(P,M)
    static void memAboutToChange( Vdbe P, Mem M )
    {
      sqlite3VdbeMemPrepareToChange( P, M );
    }
#else
        //# define memAboutToChange(P,M)
        public static void memAboutToChange(this Vdbe P, Mem M)
        {
        }
#endif

        ///<summary>
        /// Processing is determine by the affinity parameter:
        ///
        /// SQLITE_AFF_INTEGER:
        /// SQLITE_AFF_REAL:
        /// SQLITE_AFF_NUMERIC:
        ///    Try to convert pRec to an integer representation or a
        ///    floating-point representation if an integer representation
        ///    is not possible.  Note that the integer representation is
        ///    always preferred, even if the affinity is REAL, because
        ///    an integer representation is more space efficient on disk.
        ///
        /// sqliteinth.SQLITE_AFF_TEXT:
        ///    Convert pRec to a text representation.
        ///
        /// SQLITE_AFF_NONE:
        ///    No-op.  pRec is unchanged.
        ///
        ///</summary>
        public static void applyAffinity(
            this Mem pRec,///The value to apply affinity to 
            char affinity,///The affinity to be applied 
            SqliteEncoding enc///Use this text encoding 
        )
        {
            if (affinity == sqliteinth.SQLITE_AFF_TEXT)
            {
                ///Only attempt the conversion to TEXT if there is an integer or real
                ///representation (blob and NULL do not get converted) but no string
                ///representation.
                if (0 == (pRec.flags & MemFlags.MEM_Str) && (pRec.flags & (MemFlags.MEM_Real | MemFlags.MEM_Int)) != 0)
                {
                    vdbemem_cs.sqlite3VdbeMemStringify(pRec, enc);
                }
                if ((pRec.flags & (MemFlags.MEM_Blob | MemFlags.MEM_Str)) == (MemFlags.MEM_Blob | MemFlags.MEM_Str))
                {
                    StringBuilder sb = new StringBuilder(pRec.zBLOB.Length);
                    for (int i = 0; i < pRec.zBLOB.Length; i++)
                        sb.Append((char)pRec.zBLOB[i]);
                    pRec.z = sb.ToString();
                    malloc_cs.sqlite3_free(ref pRec.zBLOB);
                    pRec.flags = (pRec.flags & ~MemFlags.MEM_Blob);
                }
                pRec.flags = (pRec.flags & ~(MemFlags.MEM_Real | MemFlags.MEM_Int));
            }
            else
                if (affinity != sqliteinth.SQLITE_AFF_NONE)
                {
                    Debug.Assert(affinity == sqliteinth.SQLITE_AFF_INTEGER || affinity == sqliteinth.SQLITE_AFF_REAL || affinity == sqliteinth.SQLITE_AFF_NUMERIC);
                    pRec.applyNumericAffinity();
                    if ((pRec.flags & MemFlags.MEM_Real) != 0)
                    {
                        pRec.sqlite3VdbeIntegerAffinity();
                    }
                }
        }
    }
}
