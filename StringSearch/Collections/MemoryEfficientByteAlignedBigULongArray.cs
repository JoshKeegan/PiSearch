﻿/*
 * MemoryEfficientByteAlignedBigULongArray -implementation of BigArray for the ulong data type, fulfilling the following goals:
 *  Memory Efficient - Doesn't use 64 bits per value unless actually necessary to do so
 *  Byte Aligned - Uses the minimum number of whole bytes to store the given values possible. Doesn't optimise
 *      memory usage to a per-bit level, which requires more CPU overhead.
 *  Big - Capable of storing more than int.MaxValue values
 *  ULong Array - simples
 * By Josh Keegan 22/12/2014
 * Last Edit 30/12/2014
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StringSearch.IO;

namespace StringSearch.Collections
{
    public class MemoryEfficientByteAlignedBigULongArray : BigArray<ulong>
    {
        // Private vars
        private Stream stream;
        private byte bytesPerValue;

        //Public accessors and modifiers
        public ulong this[long i]
        {
            get
            {
                //Validation
                if(i >= this.Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                long startByteIdx = i * bytesPerValue;
                
                //Move the stream to where it should be
                stream.Position = startByteIdx;

                //Read in the bytes for this value
                byte[] bytes = new byte[8];
                stream.Read(bytes, 0, bytesPerValue);

                //Convert the bytes into a ulong
                ulong toRet = BitConverter.ToUInt64(bytes, 0);
                return toRet;
            }
            set
            {
                //Validation
                if (i >= this.Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if (value > this.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Cannot hold values larger than MaxValue ({0})", this.MaxValue));
                }

                long startByteIdx = i * bytesPerValue;

                //Move the stream to where it should be
                stream.Position = startByteIdx;

                //Get the bytes for this value
                byte[] bytes = BitConverter.GetBytes(value);

                //Write the bytes for this value onto the stream
                stream.Write(bytes, 0, bytesPerValue);
            }
        }

        public long Length { get; private set; }
        public ulong MaxValue { get; private set; }

        //Constructor
        public MemoryEfficientByteAlignedBigULongArray(long length, ulong maxValue)
        {
            this.Length = length;
            this.MaxValue = maxValue;

            //Calculate the number of bytes to leave per value
            this.bytesPerValue = calculateBytesPerValue(MaxValue);

            //Calculate the number of bytes that will be used to store all of the values
            long numBytes = length * bytesPerValue;

            //Store the array in memory by default (could be changed to another type of stream later)
            this.stream = new BigMemoryStream(numBytes);
        }

        public MemoryEfficientByteAlignedBigULongArray(long length, ulong maxValue, Stream underlyingStream)
        {
            this.Length = length;
            this.MaxValue = maxValue;

            //Calculate the number of bytes to leave per value
            this.bytesPerValue = calculateBytesPerValue(MaxValue);

            //Use the specified stream to store the values in this array
            this.stream = underlyingStream;
        }

        public MemoryEfficientByteAlignedBigULongArray(long length)
            : this(length, ulong.MaxValue) {  }

        //Public Methods
        public IEnumerator<ulong> GetEnumerator()
        {
            for (long i = 0; i < this.Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        //Helpers
        internal static byte calculateBytesPerValue(ulong maxValue)
        {
            byte numBitsPerValue = MemoryEfficientBigULongArray.calculateBitsPerValue(maxValue);

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
