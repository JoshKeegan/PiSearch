/*
 * PiSearch
 * BigMemoryStream - An Implementation of Stream that stores its values in Memory.
 *  Is not limited to int.MaxValue bytes like MemoryStream.
 *  For simplicity it does not differentiate between length & capacity; treating them as the same thing
 * By Josh Keegan 02/12/2014
 * Last Edit 08/06/2016
 */

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
        internal const int MEMORY_STREAM_MAX_SIZE = int.MaxValue / 4; //The maximum number of bytes to store in one of the underlying Memory Streams. Actual limit is int.MaxValue, but should be lower to help find continuous empty space on the heap

        //Private Variables
        private bool isClosed;
        private long length;
        private long position;
        private readonly List<MemoryStream> memStreams; //TODO: using List<byte[]> as the underlying memory stores would be more efficient, but would require more code here. Could optimise later by switching data structure

        #region Public Variables
        public override bool CanRead => !isClosed;

        public override bool CanSeek => !isClosed;

        public override bool CanWrite => !isClosed;

        public override void Flush() {  }

        public override long Length
        {
            get 
            {
                throwIfClosed();
                return length;
            }
        }

        public override long Position
        {
            get
            {
                throwIfClosed();
                return position;
            }
            set
            {
                throwIfClosed();

                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Position must be >= 0");
                }

                //TODO: If VERY small values start being used for MEMORY_STREAM_MAX_SIZE, could check if Position < (MEMORY_STREAM_MAX_SIZE * int.MaxValue <-- max no. of underlying memory streams)

                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Validation
            if(buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if(offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "must be >= 0");
            }

            if(count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "must be >= 0");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(nameof(buffer) + " is too small to write " + nameof(count) +
                                            " bytes starting at index " + nameof(offset));
            }

            throwIfClosed();

            if(position >= length || count == 0)
            {
                return 0;
            }

            //If the end of the stream is going to come before reading count bytes, correct count
            if(position > length - count)
            {
                count = (int)(length - position);
            }

            //Read in the requested bytes
            //TODO: Optimise to read blocks of bytes from each Memory Stream at once
            int prevStreamIdx = -1;
            for (int i = 0; i < count; i++)
            {
                long idx = position + i;

                int streamIdx = (int)(idx / MEMORY_STREAM_MAX_SIZE);

                //if we're onto a new stream, update our position in it
                if(streamIdx != prevStreamIdx)
                {
                    memStreams[streamIdx].Position = idx % MEMORY_STREAM_MAX_SIZE;
                }

                buffer[offset + i] = (byte)memStreams[streamIdx].ReadByte();

                prevStreamIdx = streamIdx;
            }

            position += count;
            return count;
        }

        //Manually overriding ReadByte() is unneccessary, but more efficient than the default implementation that 
        //  will use Read() to get a single byte
        public override int ReadByte()
        {
            throwIfClosed();

            if(position >= length)
            {
                return -1;
            }

            //Determine which underlying Memory stream this byte is in
            int streamIdx = (int)(position / MEMORY_STREAM_MAX_SIZE);

            //Set out position in the underlying memory stream to that of this byte
            memStreams[streamIdx].Position = position % MEMORY_STREAM_MAX_SIZE;

            position++;

            return memStreams[streamIdx].ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throwIfClosed();

            if(value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "value must be >= 0");
            }

            if(value > length)
            {
                expand(value);
            }
            else if(value < length)
            {
                shrink(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //Validation
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "must be >= 0");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "must be >= 0");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException(nameof(buffer) + " is too small to read " + nameof(count) +
                                            " bytes starting at index " + nameof(offset));
            }

            throwIfClosed();

            //Expand if necessary
            if(position > length - count)
            {
                expand(position + count);
            }

            //Write these bytes
            //TODO: Optimise to write blocks of bytes to each Memory Stream at once
            int prevStreamIdx = -1;
            for(int i = 0; i < count; i++)
            {
                long idx = position + i;

                int streamIdx = (int)(idx / MEMORY_STREAM_MAX_SIZE);

                //if we're onto a new stream, update our position in it
                if (streamIdx != prevStreamIdx)
                {
                    memStreams[streamIdx].Position = idx % MEMORY_STREAM_MAX_SIZE;
                }

                memStreams[streamIdx].WriteByte(buffer[offset + i]);

                prevStreamIdx = streamIdx;
            }

            position += count;
        }
        #endregion

        #region Constructors
        public BigMemoryStream() : this(0) {  }

        public BigMemoryStream(long capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "must be >= 0");
            }

            isClosed = false;
            length = 0; //Will be set to actual value by expand
            position = 0;

            //Initialise memStreams to be empty, expand will set the actual values
            memStreams = new List<MemoryStream>(0);
            expand(capacity);
        }
        #endregion

        #region Dispose
        ~BigMemoryStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (!isClosed)
            {
                foreach (MemoryStream memStream in memStreams)
                {
                    memStream.Close();
                }
                isClosed = true;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Helper Methods
        private void throwIfClosed()
        {
            if(isClosed)
            {
                throw new ObjectDisposedException(GetType().Name + " has been disposed");
            }
        }

        private void expand(long length)
        {
            int newNumMemStreams = calculateNumMemoryStreamsRequired(length);

            //Resize the current last stream (if there is one)
            if(memStreams.Count != 0)
            {
                int currentLastStreamNewLength = newNumMemStreams == memStreams.Count ?
                    (int)(length % MEMORY_STREAM_MAX_SIZE) :
                    MEMORY_STREAM_MAX_SIZE;
                    memStreams[memStreams.Count - 1].SetLength(currentLastStreamNewLength);
            }

            //Add more streams to the end (if necessary)
            while(newNumMemStreams != memStreams.Count)
            {
                int streamLength;
                //If this will be the last memory stream, work out how big it should be
                if(newNumMemStreams == memStreams.Count + 1)
                {
                    streamLength = (int)(length % MEMORY_STREAM_MAX_SIZE);

                    //If there was no remainder, this last stream should actually be the maximum possible size
                    if(streamLength == 0)
                    {
                        streamLength = MEMORY_STREAM_MAX_SIZE;
                    }
                }
                else //Otherwise this isn't going to be the last stream, it should therefore be the max allowed size
                {
                    streamLength = MEMORY_STREAM_MAX_SIZE;
                }

                MemoryStream stream = new MemoryStream(streamLength);
                stream.SetLength(streamLength);
                memStreams.Add(stream);
            }

            this.length = length;
        }

        private void shrink(long length)
        {
            int newNumMemStreams = calculateNumMemoryStreamsRequired(length);

            //Remove any memory streams that will no longer be required for the new size from the end
            while(newNumMemStreams != memStreams.Count)
            {
                memStreams[memStreams.Count - 1].Dispose();
                memStreams.RemoveAt(memStreams.Count - 1);
            }

            //Resize the (now) last memory stream 
            memStreams[memStreams.Count - 1].SetLength(length % MEMORY_STREAM_MAX_SIZE);

            this.length = length;
            
            if(position >= length)
            {
                position = length;
            }
        }

        private static int calculateNumMemoryStreamsRequired(long capacity)
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
