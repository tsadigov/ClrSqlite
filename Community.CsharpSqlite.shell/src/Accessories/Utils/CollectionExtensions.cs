using Community.CsharpSqlite.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static class CollectionExtensions
    {
        //public ExprList sqlite3ExprListAppend(int null_2, Expr pExpr)
        //{
        //    return this.sqlite3ExprListAppend(null, pExpr);
        //}
        public static T WinnerBy<T>(this IEnumerable<T> src,Func<T, int> point) {
            int max = int.MinValue;
            T winner = default(T);
            foreach (var item in src)
            {
                var ptValue = point(item);
                if (ptValue > max) {
                    max = ptValue;
                    winner = item;
                }
            }
            return winner;
        }

        public static  ExprList Append(
              this ExprList pList,///
                               ///<summary>
                               ///List to which to append. Might be NULL 
                               ///</summary>
             Expr pExpr///
                          ///<summary>
                          ///Expression to be appended. Might be NULL 
                          ///</summary>
          )
        {
            
            if (pList == null)
            {
                pList = new ExprList();
                //sqlite3DbMallocZero(db, ExprList).Length;
                //if ( pList == null )
                //{
                //  goto no_mem;
                //}
                Debug.Assert(pList.nAlloc == 0);
            }

            Debug.Assert(pList.a != null);
            pList.a.Add(new ExprList_item() { pExpr = pExpr });

            return pList;
            //no_mem:
            //  /* Avoid leaking memory if malloc has failed. */
            //  exprc.sqlite3ExprDelete( db, ref pExpr );
            //  exprc.sqlite3ExprListDelete( db, ref pList );
            //  return null;
        }

        ///
        ///<summary>
        ///sqlite3SrcListEnlarge
        ///
        ///Expand the space allocated for the given SrcList object by
        ///creating nExtra new slots beginning at iStart.  iStart is zero based.
        ///New slots are zeroed.
        ///
        ///For example, suppose a SrcList initially contains two entries: A,B.
        ///To append 3 new entries onto the end, do this:
        ///
        ///build.sqlite3SrcListEnlarge(db, pSrclist, 3, 2);
        ///
        ///After the call above it would contain:  A, B, nil, nil, nil.
        ///If the iStart argument had been 1 instead of 2, then the result
        ///would have been:  A, nil, nil, nil, B.  To prepend the new slots,
        ///the iStart value would be 0.  The result then would
        ///be: nil, nil, nil, A, B.
        ///
        ///If a memory allocation fails the SrcList is unchanged.  The
        ///db.mallocFailed flag will be set to true.
        ///
        ///</summary>
        public static TCollection Enlarge<TCollection, T> (
                ///<summary>
                ///Database connection to notify of OOM errors 
                ///</summary>
                //Connection db,

                ///<summary>
                ///The SrcList to be enlarged 
                ///</summary>
                this TCollection pSrc,///

                ///<summary>
                ///Number of new slots to add to pSrc.a[] 
                ///</summary>
                int nExtra,///

                ///<summary>
                ///Index in pSrc.a[] of first new slot 
                ///</summary>
                int iStart///
                ,
            Func<T> factory
            ) where T : class 
            where TCollection : MyCollection<T>  ,new()
        {
            if (null == pSrc)
                pSrc = new TCollection();
            if (-1 == iStart)
                iStart = pSrc.Count;

            pSrc.a.InsertRange(iStart, Enumerable.Range(0, nExtra).Select(x => factory()));
            #region refactored
            /*
                int i;
                ///
                ///<summary>
                ///Sanity checking on calling parameters 
                ///</summary>
                Debug.Assert(iStart >= 0);
                Debug.Assert(nExtra >= 1);
                Debug.Assert(pSrc != null);
                Debug.Assert(iStart <= pSrc.nSrc);
                ///
                ///<summary>
                ///Allocate additional space if needed 
                ///</summary>
                if (pSrc.nSrc + nExtra > pSrc.nAlloc)
                {
                    int nAlloc = pSrc.nSrc + nExtra;
                    int nGot;
                    // sqlite3DbRealloc(db, pSrc,
                    //     sizeof(*pSrc) + (nAlloc-1)*sizeof(pSrc.a[0]) );
                    pSrc.nAlloc = (i16)nAlloc;
                    Array.Resize(ref pSrc.a, nAlloc);
                    //    nGot = (sqlite3DbMallocSize(db, pNew) - sizeof(*pSrc))/sizeof(pSrc->a[0])+1;
                    //pSrc->nAlloc = (u16)nGot;
                }
                ///
                ///<summary>
                ///Move existing slots that come after the newly inserted slots
                ///out of the way 
                ///</summary>
                for (i = pSrc.nSrc - 1; i >= iStart; i--)
                {
                    pSrc.a[i + nExtra] = pSrc.a[i];
                }
                pSrc.nSrc += (i16)nExtra;
                ///
                ///<summary>
                ///Zero the newly allocated slots 
                ///</summary>
                //memset(&pSrc.a[iStart], 0, sizeof(pSrc.a[0])*nExtra);
                for (i = iStart; i < iStart + nExtra; i++)
                {
                    pSrc.a[i] = new SrcList_item();
                    pSrc.a[i].iCursor = -1;
                }
                ///
                ///<summary>
                ///Return a pointer to the enlarged SrcList 
                ///</summary>*/
            #endregion
            return pSrc;
        }

        public static TCollection Append<TCollection, T>(                
                ///<summary>
                ///The SrcList to be enlarged 
                ///</summary>
                this TCollection pSrc,
                Func<T> factory

                
            ) where TCollection : MyCollection<T> ,new()
            where T : class ,new()
        {

            pSrc=Enlarge(pSrc,1,-1,factory);            
            return pSrc;
        }
    }
}
