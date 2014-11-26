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
        public ulong this[long i]
        {
            get
            {
                //Validation
                if(i >= this.Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                long startBitIdx = i * this.bitsPerValue;
                long endBitIdx = startBitIdx + this.bitsPerValue;
                long startByteIdx = startBitIdx / 8;

                //Move the stream to where it should be
                this.stream.Position = startByteIdx;
                byte b = (byte)this.stream.ReadByte();

                long bitIdx = startBitIdx;
                int thisByteIdx = (int)(bitIdx % 8);
                ulong val = 0;
                while(bitIdx < endBitIdx)
                {
                    //Get this bit from this byte
                    int bit = b >> 7 - thisByteIdx; //Move this bit to be the LSB
                    bit = bit & 1; //Mask out all but the LSB

                    //Append this bit onto the end of the value (as the LSB)
                    val = val << 1;
                    val = val | (ulong)(long)bit; //Must sign extend the value of bit first from int to signed long & the cast the sign away to make it ulong

                    //Increment the counters
                    bitIdx++;
                    thisByteIdx++;

                    //If at the end of this byte (and there's still more to read), load the next one
                    if(thisByteIdx == 8 && bitIdx < endBitIdx)
                    {
                        b = (byte)this.stream.ReadByte();
                        thisByteIdx = 0;
                    }
                }
                return val;
            }
            set
            {
                //Validation
                if (i >= this.Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if(value > this.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Cannot hold values larger than MaxValue ({0})", this.MaxValue));
                }

                long startBitIdx = i * this.bitsPerValue;
                int startBitByteOffset = (int)(startBitIdx % 8);
                long endBitIdx = startBitIdx + this.bitsPerValue;
                int endBitByteOffset = (int)(endBitIdx % 8);
                long startByteIdx = startBitIdx / 8;
                long endByteIdx = endBitIdx / 8;

                //Increment the end byte idx if necessary so that it is one higher than the index of the last byte that will be effected
                if(endBitIdx % 8 != 0) //If the end bit idx isn't the first of this byte (in which case nothing will be written to this byte because the last bit to be effected is one below it), increment the end byte idx
                {
                    endByteIdx++;
                }

                //Move the stream to where it should be
                this.stream.Position = startByteIdx;

                //For each byte that will be effected by this
                long lastByteEffectedIdx = endByteIdx - 1;
                long bitIdx = startBitIdx;
                byte valueBitIdx = (byte)(64 - this.bitsPerValue); //The starting index on the value will be dependant on how many preceding zeroes it has (because we won't always be using the full 64 bits in a long)
                for(long j = startByteIdx; j < endByteIdx; j++)
                {
                    byte toStore;

                    //Calculate what this byte is from the value we've been supplied
                    ulong shifted = value >> (64 - valueBitIdx - 8);
                    byte valueThisByte = (byte)(shifted & byte.MinValue);

                    //If this isn't the first or last byte then just copy the whole byte
                    if((j != startByteIdx && j != lastByteEffectedIdx) ||
                    //Of if this is the only byte to be written and we'll be writing to all of it
                    (j == startByteIdx && j == lastByteEffectedIdx && startBitByteOffset == 0 && endBitByteOffset == 0) ||
                    //Else if this is the first byte (but not the last), but we'll be starting from index 0 then also copy the whole byte
                    (j == startByteIdx && j != lastByteEffectedIdx && startBitByteOffset == 0) ||
                    //Else if this is the last byte (but not the first), and we'll finish at position 0 (meaning all 8 bits 0-7 will have been written)
                    (j == lastByteEffectedIdx && j != startByteIdx && endBitByteOffset == 0))
                    {
                        //Copy the whole byte over
                        toStore = valueThisByte;

                        //Increment the bit index
                        bitIdx += 8;
                    }
                    else //Otherwise only part of this byte needs copying over what's there already
                    {
                        //Get the original byte
                        byte origByte = (byte)this.stream.ReadByte();

                        //Reset the stream position back to account for the byte that has just bean read
                        this.stream.Position--;

                        //Calculate the start and end indices (to be replaced) within this byte
                        int startOffset = 0;
                        int endOffset = 8;

                        //If first byte for this value
                        if(j == startByteIdx)
                        {
                            //Calculate start index
                            startOffset = startBitByteOffset;
                        }

                        //If last byte for this value
                        if(j == lastByteEffectedIdx)
                        {
                            //Calculate end index (one highed than the last bit that will actually be effected)
                            endOffset = endBitByteOffset;

                            //If 0, it's been wrapped around from 8. Undo that
                            if(endOffset == 0)
                            {
                                endOffset = 8;
                            }
                        }

                        //Get each bit
                        toStore = 0;
                        for(int k = 0; k < 8; k++, bitIdx++)
                        {
                            byte bit;
                            //If getting this bit from the passed value
                            if(k >= startOffset && k < endOffset)
                            {
                                bit = (byte)(valueThisByte >> 7 - k);
                            }
                            else //Otherwise retain the original value for this bit
                            {
                                bit = (byte)(origByte >> 7 - k);
                            }

                            //Mask out all but the LSB
                            bit = (byte)(bit & 1);

                            //Append this bit to the byte to be stored (as the LSB)
                            toStore = (byte)(toStore << 1);
                            toStore = (byte)(toStore | bit);
                        }
                    }
                    
                    //Write this byte to the stream
                    this.stream.WriteByte(toStore);
                }
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
