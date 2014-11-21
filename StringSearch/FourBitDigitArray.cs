using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch
{
    public class FourBitDigitArray
    {
        //Private vars
        private Stream stream;

        //Public vars
        public int Length { get; private set; }

        //Constructor
        public FourBitDigitArray(Stream stream)
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

            this.Length = (int)length;
        }

        public byte this[int i]
        {
            get
            {
                if(i < 0 || i >= this.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                stream.Position = i / 2;
                int b = stream.ReadByte();

                //If left half of byte
                if(i % 2 == 0)
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
                if (i < 0 || i >= this.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                if(value < 0 || value >= 15)
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
    }
}
