/*
 * PiSearch
 * WinFileIO - High speed file IO using the windows API
 *  P/Invoke methods in kernel32 for better performance
 * 
 * By Robert G. Bryan in Feb, 2011.
 * Modified by "Steve" for use in TVRename
 * First included in PiSearch (& modified) 02/01/2015 by Josh Keegan
 * 
 * This file is under a "free for any use" license, not the same license as is used elsewhere in the PiSearch project.
 */

// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// TVRename code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
//
// This file is under a "free for any use" license.
//
// Original code is from: http://designingefficientsoftware.wordpress.com/2011/03/03/efficient-file-io-from-csharp/
//
// Modified and streamlined by Steve, for TVRename use, specifically to have separate read/write file handles

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace StringSearch.IO
{
    unsafe class WinFileIO : IDisposable
    {
        // This class provides the capability to utilize the ReadFile and Writefile windows IO functions.  These functions
        // are the most efficient way to perform file I/O from C# or even C++.  The constructor with the buffer and buffer
        // size should usually be called to init this class.  PinBuffer is provided as an alternative.  The reason for this
        // is because a pointer needs to be obtained before the ReadFile or WriteFile functions are called.
        //
        // Error handling - In each public function of this class where an error can occur, an ApplicationException is
        // thrown with the Win32Exception message info if an error is detected.  If no exception is thrown, then a normal
        // return is considered success.
        // 
        // This code is not thread safe.  Thread control primitives need to be added if running this in a multi-threaded
        // environment.
        //
        // The recommended and fastest function for reading from a file is to call the ReadBlocks method.
        // The recommended and fastest function for writing to a file is to call the WriteBlocks method.
        //
        // License and disclaimer:
        // This software is free to use by any individual or entity for any endeavor for profit or not.
        // Even though this code has been tested and automated unit tests are provided, there is no gaurantee that
        // it will run correctly with your system or environment.  I am not responsible for any failure and you agree
        // that you accept any and all risk for using this software.
        //
        //
        // Written by Robert G. Bryan in Feb, 2011.
        //
        // Constants required to handle file I/O:
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_READ = 0x00000001;

        private const uint CREATE_NEW = 1;
        private const uint CREATE_ALWAYS = 2;
        private const uint OPEN_EXISTING = 3;
        private const uint OPEN_ALWAYS = 4;
        private const uint TRUNCATE_EXISTING = 5;
        
        internal const int BLOCK_SIZE = 65536;

        private const uint FILE_BEGIN = 0;
        private const uint FILE_CURRENT = 1;
        private const uint FILE_END = 2;
        
        private GCHandle gchBuf; // Handle to GCHandle object used to pin the I/O buffer in memory.
        private IntPtr pHandleRead;
        private IntPtr pHandleWrite;
        private void* pBuffer; // Pointer to the buffer used to perform I/O.
        private long position;
        private long length = -1;

        // Define the Windows system functions that are called by this class via COM Interop:
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateFile
        (
             string fileName,           // file name
             uint desiredAccess,        // access mode
             uint shareMode,            // share mode
             UIntPtr securityAttributes,   // Security Attributes
             uint creationDisposition,  // how to create
             uint flagsAndAttributes,   // file attributes
             IntPtr hTemplateFile          // handle to template file
        );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool ReadFile
        (
             IntPtr hFile,              // handle to file
             void* pBuffer,             // data buffer
             int numberOfBytesToRead,   // number of bytes to read
             int* pNumberOfBytesRead,   // number of bytes read
             IntPtr overlapped             // overlapped buffer which is used for async I/O.  Not used here.
        );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool WriteFile
        (
            IntPtr handle,              // handle to file
            void* pBuffer,              // data buffer
            int numberOfBytesToWrite,   // Number of bytes to write.
            int* pNumberOfBytesWritten, // Number of bytes that were written..
            IntPtr overlapped              // Overlapped buffer which is used for async I/O.  Not used here.
        );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool CloseHandle
        (
             IntPtr hObject             // handle to object
        );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetFilePointerEx
        (
            IntPtr hFile,               // handle to file
            long liDistanceToMove,      // no. of bytes to move the file pointer
            long* lpNewFilePointer,     // a pointer to a variable to receive the new file pointer. If null, new file pointer is not returned
            uint dwMoveMethod           // The starting point for the file pointer to move (FILE_BEGIN, FILE_CURRENT, FILE_END)
        );

        [DllImport("kernel32", SetLastError = true)]
        private static extern bool GetFileSizeEx
        (
            IntPtr hFile,               // handle to file
            long* lpFileSize            // a pointer to a large integer that will be set to the file size
        );

        public long Length
        {
            get
            {
                //If we have already fetched the length of the file open, use that
                if(length != -1)
                {
                    return length;
                }

                //Use the handle for the file being read by defualt
                IntPtr hFile = pHandleRead;
                //If there is no file open for read, use the one open for write
                if(hFile == IntPtr.Zero)
                {
                    hFile = pHandleWrite;

                    //If there isn't a file open for write either, throw an exception
                    if(hFile == IntPtr.Zero)
                    {
                        throw new ApplicationException("WinFileIO:Length - No file open");
                    }
                }

                long fileSize;
                if(!GetFileSizeEx(hFile, &fileSize))
                {
                    Win32Exception we = new Win32Exception();
                    throw new ArgumentException("WinFileIO:Length - Error occurred whilst reading file size. - " 
                        + we.Message);
                }
                length = fileSize;
                return length;
            }
        }

        public long Position
        {
            get
            {
                return position;
            }
            set
            {
                //Validation
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Position must be >= 0");
                }

                //If there is a read handle, update it's position
                if(pHandleRead != IntPtr.Zero)
                {
                    if(!SetFilePointerEx(pHandleRead, value, null, FILE_BEGIN))
                    {
                        Win32Exception we = new Win32Exception();
                        ApplicationException ae = new ApplicationException("WinFileIO:Position - Error occurred seeking. - "
                            + we.Message);
                        throw ae;
                    }
                }

                //If there is a write handle, and it is not the same as the read handle (which has already been updated), update it's position
                if(pHandleWrite != IntPtr.Zero && pHandleWrite != pHandleRead)
                {
                    if (!SetFilePointerEx(pHandleWrite, value, null, FILE_BEGIN))
                    {
                        Win32Exception we = new Win32Exception();
                        ApplicationException ae = new ApplicationException("WinFileIO:Position - Error occurred seeking. - "
                            + we.Message);
                        throw ae;
                    }
                }

                position = value;
            }
        }

        public WinFileIO(Array buffer)
        {
            // This constructor is provided so that the buffer can be pinned in memory.
            // Cleanup must be called in order to unpin the buffer.
            pinBuffer(buffer);
            pHandleRead = IntPtr.Zero;
            pHandleWrite = IntPtr.Zero;
        }

        protected void dispose(bool disposing)
        {
            // This function frees up the unmanaged resources of this class.
            Close();
            UnpinBuffer();
        }

        public void Dispose()
        {
            // This method should be called to clean everything up.
            dispose(true);
            // Tell the GC not to finalize since clean up has already been done.
            GC.SuppressFinalize(this);
        }

        ~WinFileIO()
        {
            // Finalizer gets called by the garbage collector if the user did not call Dispose.
            dispose(false);
        }

        private void pinBuffer(Array buffer)
        {
            // This function must be called to pin the buffer in memory before any file I/O is done.
            // This shows how to pin a buffer in memory for an extended period of time without using
            // the "Fixed" statement.  Pinning a buffer in memory can take some cycles, so this technique
            // is helpful when doing quite a bit of file I/O.
            //
            // Make sure we don't leak memory if this function was called before and the UnPinBuffer was not called.
            UnpinBuffer();
            gchBuf = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr pAddr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            // pBuffer is the pointer used for all of the I/O functions in this class.
            pBuffer = pAddr.ToPointer();
        }

        public void UnpinBuffer()
        {
            // This function unpins the buffer and needs to be called before a new buffer is pinned or
            // when disposing of this object.  It does not need to be called directly since the code in Dispose
            // or PinBuffer will automatically call this function.
            if (gchBuf.IsAllocated)
                gchBuf.Free();
        }

        public void OpenForReading(string fileName)
        {
            OpenForReading(fileName, 0);
        }

        public void OpenForReading(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open an existing file for reading.
            // A return value of true indicates success.
            // It allows other processes to read the file
            Close(true, false);
            pHandleRead = CreateFile(fileName, GENERIC_READ, FILE_SHARE_READ, UIntPtr.Zero, OPEN_EXISTING, (uint)flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if (pHandleRead == IntPtr.Zero)
            {
                Win32Exception we = new Win32Exception();
                ApplicationException ae = new ApplicationException("WinFileIO:OpenForReading - Could not open file " +
                  fileName + " - " + we.Message);
                throw ae;
            }
        }

        public void OpenForWriting(string fileName)
        {
            OpenForWriting(fileName, 0);
        }

        public void OpenForWriting(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open a file for writing.
            // If it doesn't exist, it will be created.
            // If it does exist, it will be loaded.
            // It does not allow other processes to access the file
            Close(false, true);
            pHandleWrite = CreateFile(fileName, GENERIC_WRITE, 0, UIntPtr.Zero, OPEN_ALWAYS, (uint)flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if (pHandleWrite == IntPtr.Zero)
            {
                Win32Exception we = new Win32Exception();
                ApplicationException ae = new ApplicationException("WinFileIO:OpenForWriting - Could not open file " +
                    fileName + " - " + we.Message);
                throw ae;
            }
        }

        public void OpenForReadingWriting(string fileName)
        {
            OpenForReadingWriting(fileName, 0);
        }

        public void OpenForReadingWriting(string fileName, WinFileFlagsAndAttributes flagsAndAttributes)
        {
            // This function uses the Windows API CreateFile function to open a file for reading and writing.
            // If it doesn't exist, it will be created.
            // If it does exist, it will be loaded.
            // It does not allow other processes to access the file
            Close(true, true);
            pHandleRead = pHandleWrite = CreateFile(fileName, GENERIC_READ | GENERIC_WRITE, 0, UIntPtr.Zero,
                OPEN_ALWAYS, (uint)flagsAndAttributes, IntPtr.Zero);
            position = 0;

            if(pHandleRead == IntPtr.Zero)
            {
                Win32Exception we = new Win32Exception();
                ApplicationException ae = new ApplicationException("WinFileIO:OpenForReadingWriting - Could not open file " +
                  fileName + " - " + we.Message);
                throw ae;
            }
        }

        /*
        public int Read(int BytesToRead)
        {
            // This function reads in a file up to BytesToRead using the Windows API function ReadFile.  The return value
            // is the number of bytes read.
            int BytesRead = 0;
            if (!ReadFile(pHandleRead, pBuffer, BytesToRead, &BytesRead, 0))
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:Read - Error occurred reading a file. - " +
                    WE.Message);
                throw AE;
            }
            return BytesRead;
        }

        public int ReadUntilEOF()
        {
            // This function reads in chunks at a time instead of the entire file.  Make sure the file is <= 2GB.
            // Also, if the buffer is not large enough to read the file, then an ApplicationException will be thrown.
            // No check is made to see if the buffer is large enough to hold the file.  If this is needed, then
            // use the ReadBlocks function below.
            int BytesReadInBlock = 0, BytesRead = 0;
            byte* pBuf = (byte*)pBuffer;
            // Do until there are no more bytes to read or the buffer is full.
            for (; ; )
            {
                if (!ReadFile(pHandleWrite, pBuf, BlockSize, &BytesReadInBlock, 0))
                {
                    // This is an error condition.  The error msg can be obtained by creating a Win32Exception and
                    // using the Message property to obtain a description of the error that was encountered.
                    Win32Exception WE = new Win32Exception();
                    ApplicationException AE = new ApplicationException("WinFileIO:ReadUntilEOF - Error occurred reading a file. - "
                        + WE.Message);
                    throw AE;
                }
                if (BytesReadInBlock == 0)
                    break;
                BytesRead += BytesReadInBlock;
                pBuf += BytesReadInBlock;
            }
            return BytesRead;
        }
        */
        public int ReadBlocks(int bytesToRead)
        {
            // This function reads a total of BytesToRead at a time.  There is a limit of 2gb per call.
            int bytesReadInBlock = 0, bytesRead = 0;
            byte* pBuf = (byte*)pBuffer;
            // Do until there are no more bytes to read or the buffer is full.
            do
            {
                int blockByteSize = Math.Min(BLOCK_SIZE, bytesToRead - bytesRead);
                if (!ReadFile(pHandleRead, pBuf, blockByteSize, &bytesReadInBlock, IntPtr.Zero))
                {
                    Win32Exception we = new Win32Exception();
                    ApplicationException ae = new ApplicationException("WinFileIO:ReadBytes - Error occurred reading a file. - "
                        + we.Message);
                    throw ae;
                }
                if (bytesReadInBlock == 0)
                    break;
                bytesRead += bytesReadInBlock;
                pBuf += bytesReadInBlock;
            } while (bytesRead < bytesToRead);

            position += bytesRead;

            return bytesRead;
        }
        /*
        public int Write(int BytesToWrite)
        {
            // Writes out the file in one swoop using the Windows WriteFile function.
            int NumberOfBytesWritten;
            if (!WriteFile(pHandleWrite, pBuffer, BytesToWrite, &NumberOfBytesWritten, 0))
            {
                Win32Exception WE = new Win32Exception();
                ApplicationException AE = new ApplicationException("WinFileIO:Write - Error occurred writing a file. - " +
                    WE.Message);
                throw AE;
            }
            return NumberOfBytesWritten;
        }
        */
        public int WriteBlocks(int numBytesToWrite)
        {
            // This function writes out chunks at a time instead of the entire file.  This is the fastest write function,
            // perhaps because the block size is an even multiple of the sector size.
            int bytesWritten = 0;
            int bytesOutput = 0;
            byte* pBuf = (byte*)pBuffer;
            int remainingBytes = numBytesToWrite;
            // Do until there are no more bytes to write.
            do
            {
                int bytesToWrite = Math.Min(remainingBytes, BLOCK_SIZE);
                if (!WriteFile(pHandleWrite, pBuf, bytesToWrite, &bytesWritten, IntPtr.Zero))
                {
                    // This is an error condition.  The error msg can be obtained by creating a Win32Exception and
                    // using the Message property to obtain a description of the error that was encountered.
                    Win32Exception we = new Win32Exception();
                    ApplicationException ae = new ApplicationException("WinFileIO:WriteBlocks - Error occurred writing a file. - "
                        + we.Message);
                    throw ae;
                }
                pBuf += bytesToWrite;
                bytesOutput += bytesToWrite;
                remainingBytes -= bytesToWrite;
            } while (remainingBytes > 0);

            position += bytesOutput;

            //If we know the length of this file, increment it if we'll have changed it
            if (length != -1 && position > length)
            {
                length = position;
            }

            return bytesOutput;
        }

        public bool Close()
        {
            //Close read & write file handles (if they exist)
            return Close(true, true);
        }

        public bool Close(bool read, bool write)
        {
            // This function closes the file handle.
            bool success = true;

            //If read and write are using the same handle, require both to be closed at once
            if(pHandleRead == pHandleWrite && pHandleWrite != IntPtr.Zero)
            {
                if(read && write)
                {
                    success = CloseHandle(pHandleWrite);
                    pHandleWrite = pHandleRead = IntPtr.Zero;
                    length = -1;
                }
                else
                {
                    throw new ArgumentException("read and write handles are the same, must close both at the same time");
                }
            }
            else //Otherwise there are separate read & write handles; close the specified one
            {
                if (write && pHandleWrite != IntPtr.Zero)
                {
                    success = CloseHandle(pHandleWrite) && success;
                    pHandleWrite = IntPtr.Zero;
                    length = -1;
                }

                if (read && pHandleRead != IntPtr.Zero)
                {
                    success = CloseHandle(pHandleRead) && success;
                    pHandleRead = IntPtr.Zero;
                    length = -1;
                }
            }
            
            return success;
        }
    }
}
