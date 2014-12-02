using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.IO
{
    public class BigMemoryStream : Stream
    {
        //Constants
        private const int MEMORY_STREAM_MAX_SIZE = int.MaxValue; //The maximum number of bytes to store in one of the underlying Memory Streams. Actual limit is int.MaxValue, but could be lowered to help find continuous empty space on the heap

        //Private Variables
        private bool isClosed;
        private long length;
        private long position;
        private List<MemoryStream> memStreams; //TODO: using List<byte[]> as the underlying memory stores would be more efficient, but would require more code here. Could optimise later by switching data structure

        #region Public Variables
        public override bool CanRead
        {
            get 
            {
                return !this.isClosed;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return !this.isClosed;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return !this.isClosed;
            }
        }

        public override void Flush() {  }

        public override long Length
        {
            get 
            {
                this.throwIfClosed();
                return this.length;
            }
        }

        public override long Position
        {
            get
            {
                throwIfClosed();
                return this.position;
            }
            set
            {
                throwIfClosed();

                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("Position must be >= 0");
                }

                //TODO: If VERY small values start being used for MEMORY_STREAM_MAX_SIZE, could check if Position < (MEMORY_STREAM_MAX_SIZE * int.MaxValue <-- max no. of underlying memory streams)

                this.position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Validation
            if(buffer == null)
            {
                throw new ArgumentNullException("buffer cannot be null");
            }

            if(offset < 0)
            {
                throw new ArgumentException("offset must be >= 0");
            }

            if(count < 0)
            {
                throw new ArgumentException("count must be >= 0");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("The buffer is too small to write count bytes starting at index offset");
            }

            throwIfClosed();

            if(this.position >= this.length || count == 0)
            {
                return 0;
            }

            //If the end of the stream is going to come before reading count bytes, correct count
            if(this.position > this.length - count)
            {
                count = (int)(this.length - this.position);
            }

            //Read in the requested bytes
            //TODO: Optimise to read blocks of bytes from each Memory Stream at once
            int prevStreamIdx = -1;
            for (int i = 0; i < count; i++)
            {
                long idx = this.position + i;

                int streamIdx = (int)(idx / MEMORY_STREAM_MAX_SIZE);

                //if we're onto a new stream, update our position in it
                if(streamIdx != prevStreamIdx)
                {
                    memStreams[memStreams.Count - 1].Position = idx % MEMORY_STREAM_MAX_SIZE;
                }

                buffer[offset + i] = (byte)this.memStreams[streamIdx].ReadByte();

                prevStreamIdx = streamIdx;
            }

            position += count;
            return count;
        }

        //TODO: Manually overriding ReadByte() should be more efficient than the default implementation that will use Read() to get a single byte

        public override long Seek(long offset, SeekOrigin origin)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throwIfClosed();

            if(value < 0)
            {
                throw new ArgumentOutOfRangeException("value must be >= 0");
            }

            if(value > this.length)
            {
                this.expand(value);
            }
            else if(value < this.length)
            {
                this.shrink(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //Validation
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer cannot be null");
            }

            if (offset < 0)
            {
                throw new ArgumentException("offset must be >= 0");
            }

            if (count < 0)
            {
                throw new ArgumentException("count must be >= 0");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("The buffer is too small to read count bytes starting at index offset");
            }

            throwIfClosed();

            //Expand if necessary
            if(this.position > this.length - count)
            {
                this.expand(this.position + count);
            }

            //Write these bytes
            //TODO: Optimise to write blocks of bytes to each Memory Stream at once
            int prevStreamIdx = -1;
            for(int i = 0; i < count; i++)
            {
                long idx = this.position + i;

                int streamIdx = (int)(idx / MEMORY_STREAM_MAX_SIZE);

                //if we're onto a new stream, update our position in it
                if (streamIdx != prevStreamIdx)
                {
                    memStreams[memStreams.Count - 1].Position = idx % MEMORY_STREAM_MAX_SIZE;
                }

                this.memStreams[streamIdx].WriteByte(buffer[offset + i]);

                prevStreamIdx = streamIdx;
            }

            this.position += count;
        }
        #endregion

        #region Constructors
        public BigMemoryStream() : this(0) {  }

        public BigMemoryStream(long capacity)
        {
            if(capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity must be >= 0");
            }

            this.isClosed = false;
            this.length = capacity;
            this.position = 0;

            this.memStreams = new List<MemoryStream>(numMemoryStreamsRequired(capacity));
        }
        #endregion

        #region Helper Methods
        private void throwIfClosed()
        {
            if(this.isClosed)
            {
                throw new ObjectDisposedException(this.GetType().Name + " has been disposed");
            }
        }

        private void expand(long length)
        {
            int newNumMemStreams = numMemoryStreamsRequired(length);

            //Resize the current last stream
            int currentLastStreamNewLength = newNumMemStreams == this.memStreams.Count ?
                (int)(length % MEMORY_STREAM_MAX_SIZE) :
                MEMORY_STREAM_MAX_SIZE;
            this.memStreams[this.memStreams.Count - 1].SetLength(currentLastStreamNewLength);

            //Add more streams to the end (if necessary)
            while(newNumMemStreams != this.memStreams.Count)
            {
                //If this will be the last memory stream to be added, check the size required for it. Otherwise pick the largest
                int streamLength = newNumMemStreams == this.memStreams.Count + 1 ? 
                    (int)(length % MEMORY_STREAM_MAX_SIZE) : 
                    MEMORY_STREAM_MAX_SIZE;

                MemoryStream stream = new MemoryStream(streamLength);
                this.memStreams.Add(stream);
            }

            this.length = length;
        }

        private void shrink(long length)
        {
            int newNumMemStreams = numMemoryStreamsRequired(length);

            //Remove any memory streams that will no longer be required for the new size from the end
            while(newNumMemStreams != this.memStreams.Count)
            {
                this.memStreams[this.memStreams.Count - 1].Dispose();
                this.memStreams.RemoveAt(this.memStreams.Count - 1);
            }

            //Resize the (now) last memory stream 
            memStreams[this.memStreams.Count - 1].SetLength(length % MEMORY_STREAM_MAX_SIZE);

            this.length = length;
            
            if(this.position >= length)
            {
                this.position = length;
            }
        }

        private int numMemoryStreamsRequired(long capacity)
        {
            int numMemStreams = (int)(capacity / MEMORY_STREAM_MAX_SIZE);
            if (capacity != 0 && capacity % MEMORY_STREAM_MAX_SIZE != 0)
            {
                numMemStreams++;
            }
            return numMemStreams;
        }
        #endregion
    }
}
