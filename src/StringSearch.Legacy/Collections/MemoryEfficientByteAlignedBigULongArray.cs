/*
 * PiSearch
 * MemoryEfficientByteAlignedBigULongArray - implementation of BigArray for the ulong data type, fulfilling the following goals:
 *  Memory Efficient - Doesn't use 64 bits per value unless actually necessary to do so
 *  Byte Aligned - Uses the minimum number of whole bytes to store the given values possible. Doesn't optimise
 *      memory usage to a per-bit level, which requires more CPU overhead.
 *  Big - Capable of storing more than int.MaxValue values
 *  ULong Array - simples
 * By Josh Keegan 22/12/2014
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StringSearch.Legacy.IO;

namespace StringSearch.Legacy.Collections
{
    public class MemoryEfficientByteAlignedBigULongArray : UnderlyingStream, IBigArray<ulong>
    {
        // Private vars
        private readonly byte bytesPerValue;

        //Public accessors and modifiers
        public ulong this[long i]
        {
            get
            {
                //Validation
                if(i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                long startByteIdx = i * bytesPerValue;
                
                //Move the stream to where it should be
                Stream.Position = startByteIdx;

                //Read in the bytes for this value. Default to 0 if the underlying stream isn't long enough for that
                //data, so a value has not been written for an index this high
                byte[] bytes = new byte[8];
                if (Stream.Read(bytes, 0, bytesPerValue) != bytesPerValue)
                {
                    return 0L;
                }

                //Convert the bytes into a ulong
                ulong toRet = BitConverter.ToUInt64(bytes, 0);
                return toRet;
            }
            set
            {
                //Validation
                if (i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if (value > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Cannot hold values larger than MaxValue ({0})", MaxValue));
                }

                long startByteIdx = i * bytesPerValue;

                //Move the stream to where it should be
                Stream.Position = startByteIdx;

                //Get the bytes for this value
                byte[] bytes = BitConverter.GetBytes(value);

                //Write the bytes for this value onto the stream
                Stream.Write(bytes, 0, bytesPerValue);
            }
        }

        public long Length { get; }
        public ulong MaxValue { get; }

        //Constructor
        public MemoryEfficientByteAlignedBigULongArray(long length, ulong maxValue = ulong.MaxValue)
        {
            Length = length;
            MaxValue = maxValue;

            //Calculate the number of bytes to leave per value
            bytesPerValue = CalculateBytesPerValue(MaxValue);

            //Calculate the number of bytes that will be used to store all of the values
            long numBytes = length * bytesPerValue;

            //Store the array in memory by default (could be changed to another type of stream later)
            Stream = new BigMemoryStream(numBytes);
        }

        public MemoryEfficientByteAlignedBigULongArray(long length, ulong maxValue, Stream underlyingStream)
        {
            Length = length;
            MaxValue = maxValue;

            //Calculate the number of bytes to leave per value
            bytesPerValue = CalculateBytesPerValue(MaxValue);

            //Use the specified stream to store the values in this array
            Stream = underlyingStream;
        }

        public MemoryEfficientByteAlignedBigULongArray(long length, Stream underlyingStream)
            : this(length, ulong.MaxValue, underlyingStream) {  }

        //Public Methods
        public IEnumerator<ulong> GetEnumerator()
        {
            for (long i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Helpers
        internal static byte CalculateBytesPerValue(ulong maxValue)
        {
            byte numBitsPerValue = MemoryEfficientBigULongArray.CalculateBitsPerValue(maxValue);

            byte numBytesPerValue = (byte)(numBitsPerValue / 8);

            //Do we need to use part of the next byte also
            if(numBitsPerValue % 8 != 0)
            {
                numBytesPerValue++;
            }

            return numBytesPerValue;
        }
    }
}
