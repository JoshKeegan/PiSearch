/*
 * PiSearch
 * FastFileStream - a fast implementation of Stream, with the underlying data being stored on a file (like FileStream)
 * By Josh Keegan 02/01/2015
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.IO
{
    public class FastFileStream : Stream
    {
        //Private variables
        private byte[] buffer;
        private WinFileIO wfio = null;
        private FileAccess fileAccess;

        public override bool CanRead
        {
            get 
            {
                return wfio != null && fileAccess != FileAccess.Write;
            }
        }

        public override bool CanSeek
        {
            get 
            {
                return wfio != null;
            }
        }

        public override bool CanWrite
        {
            get 
            {
                return wfio != null && fileAccess != FileAccess.Read;
            }
        }

        public override void Flush() 
        {
            throw new NotImplementedException();
            //TODO: Check if there is a Win API call that could be made by WinFileIO to flush
        }

        public override long Length
        {
            get 
            { 
                throw new NotImplementedException(); 
                //TODO
            }
        }

        public override long Position
        {
            get
            {
                throwIfClosed();

                return wfio.Position;
            }
            set
            {
                throwIfClosed();

                wfio.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
            //TODO
        }

        //TODO: ReadByte() Implementation would be more efficient than relying on Read(byte[], int, int) & would get used

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
            //TODO
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
            //TODO
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
            //TODO
        }

        ~FastFileStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if(wfio != null)
            {
                wfio.Close();
            }

            base.Dispose(disposing);
        }

        public FastFileStream(string path, FileAccess fileAccess, int maxReadWriteCallSize)
        {
            if(path == null)
            {
                throw new ArgumentException("path");
            }

            if(!path.Any())
            {
                throw new ArgumentException("path is empty");
            }

            if(fileAccess < FileAccess.Read || fileAccess > FileAccess.ReadWrite)
            {
                throw new ArgumentOutOfRangeException("fileAccess must contain a valid value for the FileAccess enum");
            }

            if(maxReadWriteCallSize < 1)
            {
                throw new ArgumentOutOfRangeException("maxReadWriteCallSize must be > 0");
            }

            if(path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException("path contains invalid characters");
            }

            if(Directory.Exists(path))
            {
                //Seems an odd choice of exception, but it's what FileStream does
                throw new UnauthorizedAccessException("The specified path points to a directory, must be a file");
            }

            if(!File.Exists(path))
            {
                throw new FileNotFoundException("path not found");
            }

            //TODO: Could still be problems accessing the file due to permissions or it being locked by another process
            //  In these cases, currently the Exception thrown by WinFileIO will get propagated. Could improve this with better Exceptions
            buffer = new byte[maxReadWriteCallSize];
            wfio =  new WinFileIO(buffer);
            
            switch(fileAccess)
            {
                case FileAccess.Read:
                    wfio.OpenForReading(path);
                    break;
                case FileAccess.Write:
                    wfio.OpenForWriting(path);
                    break;
                case FileAccess.ReadWrite:
                    wfio.OpenForReadingWriting(path);
                    break;
            }

            this.fileAccess = fileAccess;
        }

        public FastFileStream(string path, FileAccess fileAccess)
            : this(path, fileAccess, WinFileIO.BlockSize) {  }

        //Helpers
        private void throwIfClosed()
        {
            if(wfio == null)
            {
                throw new ObjectDisposedException("Stream has been closed");
            }
        }
    }
}
