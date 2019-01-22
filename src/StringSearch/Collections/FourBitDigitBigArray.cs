/*
 * PiSearch
 * FourBitDigitArray - an implementation of BigArray designed to hold integers in the range 0 <= x < 15
 *  i.e. 4 bits per value, with 1111 (15) being reserved
 * By Josh Keegan 27/11/2014
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
{
    public class FourBitDigitBigArray : IBigArray<byte>
    {
        //Private vars
        private readonly Stream stream;

        //Public vars
        public long Length { get; private set; }

        public byte this[long i]
        {
            get
            {
                if (i < 0 || i >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                stream.Position = i / 2;
                int b = stream.ReadByte();

                //If left half of byte
                if (i % 2 == 0)
                {
                    b = b >> 4;
                }
                else //Otherwise right half
                {
                    b = b & 15; // mask 0000 1111
                }

                return (byte)b;
            }
            set
            {
                if (i < 0 || i >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                if (value >= 15)
                {
                    throw new OverflowException();
                }

                stream.Position = i / 2;
                int b = stream.ReadByte();

                //If replacing left half of byte
                if (i % 2 == 0)
                {
                    //Get the right half so we can keep that the same
                    int right = b & 15; // mask 0000 1111
                    int left = value << 4;

                    b = left | right;
                }
                else //Otherwise replacing right half
                {
                    //Get the left half so we can keep that the same
                    int left = b & 240; //mask 1111 0000

                    b = left | value;
                }

                stream.Position = i / 2;
                stream.WriteByte((byte)b);
            }
        }

        //Constructor
        public FourBitDigitBigArray(Stream stream)
        {
            this.stream = stream;

            //Calculate the length
            long length = stream.Length * 2;

            //If there are any digits in the stream, check if there is an odd number of digits
            if(length > 0)
            {
                //If there is 1111 as the last digit, then it isn't actually the last digit, there because cannot write half a byte
                stream.Position = stream.Length - 1;
                int b = stream.ReadByte();
                int right = b & 15; // mask 0000 1111

                if (right == 15)
                {
                    length--;
                }
            }

            Length = length;
        }

        //Public methods
        public IEnumerator<byte> GetEnumerator()
        {
            for(long i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
