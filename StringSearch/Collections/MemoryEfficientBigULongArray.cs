/*
 * PiSearch
 * MemoryEfficientBigULongArray - an implementation of BigArray for the ulong data type, fulfilling the following goals:
 *  Memory Efficient - Doesn't use 64 bits per value unless actually necessary to do so
 *  Bit Aligned - Uses the minimum number of bits to store each value (without compression)
 *  Big - Capable of storing more than int.MaxValue values
 *  ULong Array - simples
 * By Josh Keegan 26/11/2014
 * Last Edit 08/06/2016
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
    public class MemoryEfficientBigULongArray : UnderlyingStream, IBigArray<ulong>
    {
        //Private vars
        private readonly byte bitsPerValue;

        //Public accessors & modifiers
        public ulong this[long i]
        {
            get
            {
                //Validation
                if(i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                long startBitIdx = i * bitsPerValue;
                long endBitIdx = startBitIdx + bitsPerValue;
                long startByteIdx = startBitIdx / 8;

                //Move the stream to where it should be
                stream.Position = startByteIdx;
                byte b = (byte)stream.ReadByte();

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
                    // TODO: Check if this cast is now redundant. C#6 might have changed the behaviour, but fairly 
                    //  confident you used to have to sign extend by going via long first
                    // ReSharper disable once RedundantCast
                    val = val | (ulong)(long)bit; //Must sign extend the value of bit first from int to signed long & the cast the sign away to make it ulong

                    //Increment the counters
                    bitIdx++;
                    thisByteIdx++;

                    //If at the end of this byte (and there's still more to read), load the next one
                    if(thisByteIdx == 8 && bitIdx < endBitIdx)
                    {
                        b = (byte)stream.ReadByte();
                        thisByteIdx = 0;
                    }
                }
                return val;
            }
            set
            {
                //Validation
                if (i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                if(value > MaxValue)
                {
                    throw new ArgumentOutOfRangeException(String.Format("Cannot hold values larger than MaxValue ({0})", MaxValue));
                }

                long startBitIdx = i * bitsPerValue;
                int startBitByteOffset = (int)(startBitIdx % 8);
                long endBitIdx = startBitIdx + bitsPerValue;
                int endBitByteOffset = (int)(endBitIdx % 8);
                long startByteIdx = startBitIdx / 8;
                long endByteIdx = endBitIdx / 8;

                //Increment the end byte idx if necessary so that it is one higher than the index of the last byte that will be effected
                if(endBitIdx % 8 != 0) //If the end bit idx isn't the first of this byte (in which case nothing will be written to this byte because the last bit to be effected is one below it), increment the end byte idx
                {
                    endByteIdx++;
                }

                //Move the stream to where it should be
                stream.Position = startByteIdx;

                //For each byte that will be effected by this
                long lastByteEffectedIdx = endByteIdx - 1;
                byte valueBitIdx = (byte)(64 - bitsPerValue); //The starting index on the value will be dependant on how many preceding zeroes it has (because we won't always be using the full 64 bits in a long)
                for(long j = startByteIdx; j < endByteIdx; j++)
                {
                    byte toStore;

                    //Calculate what this byte is from the value we've been supplied
                    //To get the byte we're interesting in working with from the value, shift it's bits so that the 8 LSB's are the byte we're after. We want this
                    //  byte to line up with the bits as stored in the stream, so pad left and right with zero's if necessary.
                    //  We know the index of the first bit of this byte and the number of bits in a long (64), so first we can do 64 - valueBitIdx to get how
                    //  much we'd have to shift the value by to get the value of the bit at valueBitIdx. This is actually one too high and would need a -1, but not in the formula
                    //  because it's accounted for in the next part.
                    //  But we want a whole byte, not just a single bit so look 8 earlier than this bit (actually 7, but 8 accounts for not having taken the 1 off in the previous step)
                    //  to get the preceding byte with this index being the LSB. However, we need the byte to line up with how it will be stored
                    //  in the stream, so instead of looking 8 earlier, look 8 - startBitByteOffset earlier. By subtracting the number of bits into the first byte
                    //  the value is stored, this will align the returned byte with how it is stored in the stream.
                    //  If this yields a negative value to shift right by, shift left by this value (padding with zero's to the right).
                    //  Padding left will be automatic because they will be zero's in the long value
                    /*
                     * Worked Example:
                     * Max Value: 1000
                     * bitsPerValue: 10
                     * i (array index): 3
                     * value: 400
                     * binary value: 01 10010000
                     * startBitIdx: 30 (bitsPerValue * i)
                     * startBitByteOffset: 6 (startBitIdx % 8)
                     * 
                     * First byte:
                     * expected: 00000001 (6 zero's padding because of startBitByteOffset and then the beginning of the actual bits making up the value)
                     * j: 0
                     * valueBitIdx: 54 (64 - bitsPerValue) + (j * 8)
                     * rightShiftBy: 8 (64 - valueBitIdx - (8 - startBitByteOffset))
                     * 
                     * When writing this byte to the stream, the first 6 bits will be taken from the existing byte value and then the last two from this
                     * 
                     * 
                     * Second byte:
                     * expected: 10010000
                     * j: 1
                     * valueBitIdx: 62 (64 - bitsPerValue) + (j * 8)
                     * rightShiftBy: 0 (64 - valueBitIdx - (8 - startBitByteOffset))
                     * 
                     * When writing this byte to the stream, the whole byte will be written, without having to even read the existing data
                     * 
                     */
                    int rightShiftBy = (64 - valueBitIdx - (8 - startBitByteOffset));
                    ulong shifted;
                    //If actually right shifting
                    if(rightShiftBy >= 0)
                    {
                        shifted = value >> rightShiftBy;
                    }
                    else //Otherwise this is the last bit and it doesn't align with the end of a byte, so we effectively want to right shift by a -ve amount
                    {
                        shifted = value << -rightShiftBy;
                    }
                    byte valueThisByte = (byte)(shifted & byte.MaxValue);

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
                    }
                    else //Otherwise only part of this byte needs copying over what's there already
                    {
                        //Get the original byte
                        byte origByte = (byte)stream.ReadByte();

                        //Reset the stream position back to account for the byte that has just bean read
                        stream.Position--;

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
                        for(int k = 0; k < 8; k++)
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
                    stream.WriteByte(toStore);

                    //Increment the value bit index
                    valueBitIdx += 8;
                }
            }
        }

        public long Length { get; private set; }
        public ulong MaxValue { get; private set; }

        //Constructor
        public MemoryEfficientBigULongArray(long length, ulong maxValue)
        {
            Length = length;
            MaxValue = maxValue;

            //Calculate the number of bits to leave per value
            bitsPerValue = calculateBitsPerValue(MaxValue);

            //Calculate the number of bytes that will be used to store all of the values
            long numBytes = calculateMinimumStreamLength();

            //Store the array in memory by default
            stream = new BigMemoryStream(numBytes);
        }

        public MemoryEfficientBigULongArray(long length, ulong maxValue, Stream underlyingStream)
        {
            Length = length;
            MaxValue = maxValue;

            //Calculate the number of bits to leave per value
            bitsPerValue = calculateBitsPerValue(MaxValue);

            //Use the specified stream to store the values in this array
            stream = underlyingStream;

            //Calculate the required minimum length of the underlying stream
            long minStreamLength = calculateMinimumStreamLength();

            //If the provided underlying stream isn't long enough, make it bigger
            if(stream.Length < minStreamLength)
            {
                stream.SetLength(minStreamLength);
            }
        }

        public MemoryEfficientBigULongArray(long length)
            : this(length, ulong.MaxValue) {  }
        
        public MemoryEfficientBigULongArray(long length, Stream underlyingStream)
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
        internal static byte calculateBitsPerValue(ulong maxValue)
        {
            byte numBits = 1;
            ulong largestPossible = 1;

            while(maxValue > largestPossible)
            {
                numBits++;

                //Add one bit to the largest possible value
                largestPossible = largestPossible << 1;
                largestPossible = largestPossible | 1;
            }

            return numBits;
        }

        private long calculateMinimumStreamLength()
        {
            long numBits = Length * bitsPerValue;
            long numBytes = numBits / 8;
            //If extra bits are required, assign an extra byte
            if (numBits % 8 != 0)
            {
                numBytes++;
            }

            return numBytes;
        }
    }
}
