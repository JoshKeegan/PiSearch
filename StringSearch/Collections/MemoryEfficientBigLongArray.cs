using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Streams;

namespace StringSearch.Collections
{
    public class MemoryEfficientBigULongArray : BigArray<ulong>
    {
        //Private vars
        Stream stream;
        private byte bitsPerValue;

        //Public accessors & modifiers
        long this[long i]
        {
            get
            {
                //Validation
                if(i >= this.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                long startBitIdx = i * this.bitsPerValue;
                long startByteIdx = startBitIdx / 8;

                //TODO
            }
            set
            {
                //Validation
                if (i >= this.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                //TODO
            }
        }

        public long Length { get; private set; }
        public ulong MaxValue { get; private set; }

        //Constructor
        public MemoryEfficientBigULongArray(long length, ulong maxValue)
        {
            this.Length = length;
            this.MaxValue = maxValue;

            //Calculate the number of bits to leave per value
            this.bitsPerValue = calculateBitsPerValue(MaxValue);

            //Calculate the number of bytes that will be used to store all of the values
            long numBits = length * bitsPerValue;
            long numBytes = numBits / 8;
            //If extra bits are required, assign an extra byte
            if(numBits % 8 != 0)
            {
                numBytes++;
            }

            //Store the array in memory by default
            this.stream = new MemoryTributary(numBytes);
        }

        public MemoryEfficientBigULongArray(long length)
            : this(length, ulong.MaxValue) { }

        //Helpers
        private static byte calculateBitsPerValue(ulong maxValue)
        {
            byte numBits = 1;
            ulong largestPossible = 1;

            while(maxValue < largestPossible)
            {
                numBits++;

                //Add one bit to the largest possible value
                largestPossible = largestPossible << 1;
                largestPossible = largestPossible | 1;
            }

            return numBits;
        }
    }
}
