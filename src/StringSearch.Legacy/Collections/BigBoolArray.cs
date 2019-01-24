/*
 * PiSearch
 * BigBoolArray - an implementation of BigArray for the bool data type
 * By Josh Keegan 12/02/2015
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using StringSearch.Legacy.IO;

namespace StringSearch.Legacy.Collections
{
    public class BigBoolArray : UnderlyingStream, IBigArray<bool>
    {
        public bool this[long i]
        {
            get
            {
                //Validation
                if(i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                //Set the stream position to that of the byte holding the bit we're after
                stream.Position = i / 8;
                int b = stream.ReadByte();

                //Move the bit we're after to be the LSB
                int bitNum = (int)(i % 8);
                int rightShiftBy = 7 - bitNum;
                int shifted = b >> rightShiftBy;

                //Get just the bit we want
                int val = shifted & 1;

                //Convert the bit to a bool and return
                bool toRet = val == 1;
                return toRet;
            }
            set
            {
                //Validation
                if (i >= Length || i < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                //Set the stream position to that of the byte holding the bit we want to set
                stream.Position = i / 8;
                int b = stream.ReadByte();

                //Stream position will have incremented on read, roll back to write over this byte in a moment
                stream.Position--;

                //Get out the values to the left and right of the bit to be set so that they can be written back onto the stream
                int bitNum = (int)(i % 8);

                //Get the digits to the left
                int numLeft = bitNum;
                int leftMask = byte.MaxValue << 8 - numLeft;
                int left = b & leftMask;

                int numRight = 7 - bitNum;
                int rightMask = byte.MaxValue >> 8 - numRight;
                int right = b & rightMask;

                //Get the bit into the right position
                int val = value ? 1 : 0;
                int shiftedVal = val << 7 - bitNum;

                //Merge the three bit strings
                int merged = left | shiftedVal | right;
                stream.WriteByte((byte)merged);
            }
        }

        public long Length { get; private set; }

        //Constructor
        public BigBoolArray(long length)
            : this(length, createMemoryStream(length)) {  }

        public BigBoolArray(long length, Stream underlyingStream)
        {
            //TODO: Validation (length must be positive)

            Length = length;
            stream = underlyingStream;

            //Calculate the minimum required length of the underlying stream
            long minStreamLength = calculateMinimumStreamLength(Length);

            //If the provided underlying stream isn't long enough, make it bigger
            if(stream.Length < minStreamLength)
            {
                stream.SetLength(minStreamLength);
            }
        }

        public IEnumerator<bool> GetEnumerator()
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

        private static Stream createMemoryStream(long numBools)
        {
            long streamLength = calculateMinimumStreamLength(numBools);

            //TODO: Make this next chunk of code be performed by some mort of static Create method (auto-select MemoryStream or BigMemoryStream)
            Stream toRet;

            //If it'll fit into a regular Memory Stream, use it
            if(streamLength <= int.MaxValue)
            {
                toRet = new MemoryStream((int)streamLength);
            }
            else
            {
                toRet = new BigMemoryStream(streamLength);
            }

            return toRet;
        }

        private static long calculateMinimumStreamLength(long length)
        {
            long streamSize = length / 8;

            if(length % 8 != 0)
            {
                streamSize++;
            }

            return streamSize;
        }
    }
}
