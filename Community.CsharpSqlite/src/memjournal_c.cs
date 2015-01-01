using System;
using System.Diagnostics;
using System.Text;
using Bitmask = System.UInt64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{
	using sqlite3_int64 = System.Int64;
	using MemJournal = Sqlite3.sqlite3_file;

	public partial class Sqlite3
	{
		///
///<summary>
///2007 August 22
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///
///</summary>
///<param name="This file contains code use to implement an in">memory rollback journal.</param>
///<param name="The in">memory rollback journal is used to journal transactions for</param>
///<param name="":memory:" databases and when the journal_mode=MEMORY pragma is used.">":memory:" databases and when the journal_mode=MEMORY pragma is used.</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">07 20:14:09 a586a4deeb25330037a49df295b36aaf624d0f45</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#include "sqliteInt.h"
		///
///<summary>
///Forward references to internal structures 
///</summary>

		//typedef struct MemJournal MemJournal;
		//typedef struct FilePoint FilePoint;
		//typedef struct FileChunk FileChunk;
		///<summary>
		///Space to hold the rollback journal is allocated in increments of
		/// this many bytes.
		///
		/// The size chosen is a little less than a power of two.  That way,
		/// the FileChunk object will have a size that almost exactly fills
		/// a power-of-two allocation.  This mimimizes wasted space in power-of-two
		/// memory allocators.
		///
		///</summary>
		//#define JOURNAL_CHUNKSIZE ((int)(1024-sizeof(FileChunk*)))
		const int JOURNAL_CHUNKSIZE = 4096;


		///<summary>
		/// The rollback journal is composed of a linked list of these structures.
		///
		///</summary>
		public class FileChunk
		{
			public FileChunk pNext;

			///
///<summary>
///Next chunk in the journal 
///</summary>

			public byte[] zChunk = new byte[JOURNAL_CHUNKSIZE];
		///
///<summary>
///Content of this chunk 
///</summary>

		};


		///<summary>
		/// An instance of this object serves as a cursor into the rollback journal.
		/// The cursor can be either for reading or writing.
		///
		///</summary>
		public class FilePoint
		{
			public long iOffset;

			///
///<summary>
///Offset from the beginning of the file 
///</summary>

			public FileChunk pChunk;
		///
///<summary>
///Specific chunk into which cursor points 
///</summary>

		};


		///<summary>
		/// This subclass is a subclass of sqlite3_file.  Each open memory-journal
		/// is an instance of this class.
		///
		///</summary>
		public partial class sqlite3_file
		{
			//public sqlite3_io_methods pMethods; /* Parent class. MUST BE FIRST */
			public FileChunk pFirst;

			///
///<summary>
///</summary>
///<param name="Head of in">list </param>

			public FilePoint endpoint;

			///
///<summary>
///Pointer to the end of the file 
///</summary>

			public FilePoint readpoint;

			///
///<summary>
///Pointer to the end of the last xRead() 
///</summary>

			public///<summary>
			/// If pFile is currently larger than iSize bytes, then truncate it to
			/// exactly iSize bytes. If pFile is not larger than iSize bytes, then
			/// this function is a no-op.
			///
			/// Return SQLITE_OK if everything is successful, or an SQLite error
			/// code if an error occurs.
			///</summary>
			int backupTruncateFile (int iSize)
			{
				long iCurrent = 0;
				int rc = os.sqlite3OsFileSize (this, ref iCurrent);
				if (rc == SQLITE_OK && iCurrent > iSize) {
					rc = os.sqlite3OsTruncate (this, iSize);
				}
				return rc;
			}
		}


        public class memjrnl {

            ///<summary>
            /// Read data from the in-memory journal file.  This is the implementation
            /// of the sqlite3_vfs.xRead method.
            ///</summary>
            public static int memjrnlRead(/*The journal file from which to read */sqlite3_file pJfd, /*Put the results here */byte[] zBuf,
                /*Number of bytes to read */int iAmt, /*Begin reading at this offset */sqlite3_int64 iOfst)
            {
                MemJournal p = (MemJournal)pJfd;
                byte[] zOut = zBuf;
                int nRead = iAmt;
                int iChunkOffset;
                FileChunk pChunk;
                ///SQLite never tries to read past the end of a rollback journal file 

                Debug.Assert(iOfst + iAmt <= p.endpoint.iOffset);
                if (p.readpoint.iOffset != iOfst || iOfst == 0)
                {
                    int iOff = 0;
                    for (pChunk = p.pFirst; Sqlite3.ALWAYS(pChunk != null) && (iOff + JOURNAL_CHUNKSIZE) <= iOfst; pChunk = pChunk.pNext)
                    {
                        iOff += JOURNAL_CHUNKSIZE;
                    }
                }
                else
                {
                    pChunk = p.readpoint.pChunk;
                }
                iChunkOffset = (int)(iOfst % JOURNAL_CHUNKSIZE);
                int izOut = 0;
                do
                {
                    int iSpace = JOURNAL_CHUNKSIZE - iChunkOffset;
                    int nCopy = MathExtensions.MIN(nRead, (JOURNAL_CHUNKSIZE - iChunkOffset));
                    Buffer.BlockCopy(pChunk.zChunk, iChunkOffset, zOut, izOut, nCopy);
                    //memcpy( zOut, pChunk.zChunk[iChunkOffset], nCopy );
                    izOut += nCopy;
                    // zOut += nCopy;
                    nRead -= iSpace;
                    iChunkOffset = 0;
                }
                while (nRead >= 0 && (pChunk = pChunk.pNext) != null && nRead > 0);
                p.readpoint.iOffset = (int)(iOfst + iAmt);
                p.readpoint.pChunk = pChunk;
                return SQLITE_OK;
            }

            ///<summary>
            /// Write data to the file.
            ///
            ///</summary>
            public static int memjrnlWrite(sqlite3_file pJfd, ///
                ///The journal file into which to write 

            byte[] zBuf, ///
                ///Take data to be written from here 

            int iAmt, ///
                ///Number of bytes to write 

            sqlite3_int64 iOfst///
                ///Begin writing at this offset into the file 

            )
            {
                MemJournal p = (MemJournal)pJfd;
                int nWrite = iAmt;
                byte[] zWrite = zBuf;
                int izWrite = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="An in">memory journal file should only ever be appended to. Random</param>
                ///<param name="access writes are not required by sqlite.">access writes are not required by sqlite.</param>
                ///<param name=""></param>

                Debug.Assert(iOfst == p.endpoint.iOffset);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(iOfst);
                while (nWrite > 0)
                {
                    FileChunk pChunk = p.endpoint.pChunk;
                    int iChunkOffset = (int)(p.endpoint.iOffset % JOURNAL_CHUNKSIZE);
                    int iSpace = MathExtensions.MIN(nWrite, JOURNAL_CHUNKSIZE - iChunkOffset);
                    if (iChunkOffset == 0)
                    {
                        ///
                        ///<summary>
                        ///New chunk is required to extend the file. 
                        ///</summary>

                        FileChunk pNew = new FileChunk();
                        // sqlite3_malloc( sizeof( FileChunk ) );
                        if (null == pNew)
                        {
                            return SQLITE_IOERR_NOMEM;
                        }
                        pNew.pNext = null;
                        if (pChunk != null)
                        {
                            Debug.Assert(p.pFirst != null);
                            pChunk.pNext = pNew;
                        }
                        else
                        {
                            Debug.Assert(null == p.pFirst);
                            p.pFirst = pNew;
                        }
                        p.endpoint.pChunk = pNew;
                    }
                    Buffer.BlockCopy(zWrite, izWrite, p.endpoint.pChunk.zChunk, iChunkOffset, iSpace);
                    //memcpy( &p.endpoint.pChunk.zChunk[iChunkOffset], zWrite, iSpace );
                    izWrite += iSpace;
                    //zWrite += iSpace;
                    nWrite -= iSpace;
                    p.endpoint.iOffset += iSpace;
                }
                return SQLITE_OK;
            }

            ///<summary>
            /// Truncate the file.
            ///
            ///</summary>
            public static int memjrnlTruncate(sqlite3_file pJfd, sqlite3_int64 size)
            {
                MemJournal p = (MemJournal)pJfd;
                FileChunk pChunk;
                Debug.Assert(size == 0);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(size);
                pChunk = p.pFirst;
                while (pChunk != null)
                {
                    FileChunk pTmp = pChunk;
                    pChunk = pChunk.pNext;
                    //sqlite3_free( ref pTmp );
                }
                sqlite3MemJournalOpen(pJfd);
                return SQLITE_OK;
            }

            ///<summary>
            /// Close the file.
            ///
            ///</summary>
            public static int memjrnlClose(MemJournal pJfd)
            {
                memjrnlTruncate(pJfd, 0);
                return SQLITE_OK;
            }

            ///<summary>
            /// Sync the file.
            ///
            /// Syncing an in-memory journal is a no-op.  And, in fact, this routine
            /// is never called in a working implementation.  This implementation
            /// exists purely as a contingency, in case some malfunction in some other
            /// part of SQLite causes Sync to be called by mistake.
            ///
            ///</summary>
            public static int memjrnlSync(sqlite3_file NotUsed, int NotUsed2)
            {
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                return SQLITE_OK;
            }

            ///
            ///<summary>
            ///Query the size of the file in bytes.
            ///
            ///</summary>

            public static int memjrnlFileSize(sqlite3_file pJfd, ref long pSize)
            {
                MemJournal p = (MemJournal)pJfd;
                pSize = p.endpoint.iOffset;
                return SQLITE_OK;
            }

            ///<summary>
            /// Open a journal file.
            ///
            ///</summary>
            public static void sqlite3MemJournalOpen(sqlite3_file pJfd)
            {
                MemJournal p = (MemJournal)pJfd;
                //memset( p, 0, sqlite3MemJournalSize() );
                p.pFirst = null;
                p.endpoint = new FilePoint();
                p.readpoint = new FilePoint();
                p.pMethods = MemJournalMethods;
                //(sqlite3_io_methods*)&MemJournalMethods;
            }

            ///<summary>
            /// Return true if the file-handle passed as an argument is
            /// an in-memory journal
            ///
            ///</summary>
            public static bool sqlite3IsMemJournal(sqlite3_file pJfd)
            {
                return pJfd.pMethods == MemJournalMethods;
            }

            ///
            ///<summary>
            ///Return the number of bytes required to store a MemJournal file descriptor.
            ///
            ///</summary>

            public static int sqlite3MemJournalSize()
            {
                return 3096;
                // sizeof( MemJournal );
            }
        }


		///<summary>
		/// Table of methods for MemJournal sqlite3_file object.
		///
		///</summary>
		static sqlite3_io_methods MemJournalMethods = new sqlite3_io_methods (
                1, ///iVersion 

                (dxClose)memjrnl.memjrnlClose, ///xClose 

                (dxRead)memjrnl.memjrnlRead, ///xRead 

                (dxWrite)memjrnl.memjrnlWrite, ///xWrite 

                (dxTruncate)memjrnl.memjrnlTruncate, ///xTruncate 

                (dxSync)memjrnl.memjrnlSync, ///xSync 

                (dxFileSize)memjrnl.memjrnlFileSize, ///xFileSize 

		        null, ///xLock 

		        null, ///xUnlock 

		        null, ///xCheckReservedLock 

		        null, ///xFileControl 

		        null, ///xSectorSize 

		        null,///xDeviceCharacteristics 

		        null,///xShmMap 

		        null,///xShmLock 

		        null,///xShmBarrier 

		        null///xShmUnlock 

		);

		
	}
}
