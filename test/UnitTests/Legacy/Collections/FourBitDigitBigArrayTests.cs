using System;
using System.IO;
using StringSearch.Legacy.Collections;
using StringSearch.Legacy.IO;
using UnitTests.TestObjects.Extensions;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class FourBitDigitBigArrayTests : ForceGcBetweenTests
    {
        [Fact]
        public void TestConstructor()
        {
            const string str = "1234";

            Stream memStream = str.ToFourBitDigitStream();

            FourBitDigitBigArray a = new FourBitDigitBigArray(memStream);
        }

        [Fact]
        public void TestEmpty()
        {
            Stream memStream = "".ToFourBitDigitStream();

            FourBitDigitBigArray a = new FourBitDigitBigArray(memStream);

            Assert.Equal(0, a.Length);
        }

        [Fact]
        public void TestOddNumberOfDigits()
        {
            Stream memStream = "123".ToFourBitDigitStream();

            FourBitDigitBigArray a = new FourBitDigitBigArray(memStream);

            Assert.Equal(3, a.Length);
        }

        [Fact]
        public void TestGet()
        {
            const string str = "391";

            FourBitDigitBigArray a = str.ToFourBitDigitBigArray();

            for(int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                byte b = a[i];

                Assert.Equal(c.ToString(), b.ToString());
            }
        }

        [Fact]
        public void TestSetEven()
        {
            const string orig = "391";

            FourBitDigitBigArray a = orig.ToFourBitDigitBigArray();

            a[0] = 7;
            Assert.Equal(7, a[0]);

            for (int i = 1; i < orig.Length; i++)
            {
                Assert.Equal(orig[i].ToString(), a[i].ToString());
            }
        }

        [Fact]
        public void TestSetOdd()
        {
            const string orig = "391";

            FourBitDigitBigArray a = orig.ToFourBitDigitBigArray();

            a[1] = 7;
            Assert.Equal(7, a[1]);

            for (int i = 0; i < orig.Length; i++)
            {
                if(i != 1)
                {
                    Assert.Equal(orig[i].ToString(), a[i].ToString());
                }
            }
        }

        [Fact]
        public void TestAccessOutOfRangeNeg()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                byte b = a[-1];
            });
        }

        [Fact]
        public void TestAccessOutOfRange()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                byte b = a[3];
            });
        }

        [Fact]
        public void TestSetOutOfRangeNeg()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                a[-1] = 3;
            });
        }

        [Fact]
        public void TestSetOutOfRange()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                a[3] = 3;
            });
        }

        [Fact]
        public void TestSetOverflow()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<OverflowException>(() =>
            {
                a[0] = 16;
            });
        }

        [Fact]
        public void TestSetReservedOverflow()
        {
            //Highest possible value in 4 bits (15) reserved for marking that half of the byte as not in use
            //  so it counts as overflow
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();

            Assert.Throws<OverflowException>(() =>
            {
                a[0] = 15;
            });
        }

        [Fact]
        public void TestLength()
        {
            FourBitDigitBigArray a = "123".ToFourBitDigitBigArray();
            Assert.Equal(3, a.Length);
        }

        [Fact]
        public void TestLengthEven()
        {
            FourBitDigitBigArray a = "1234".ToFourBitDigitBigArray();
            Assert.Equal(4, a.Length);
        }

        [Fact]
        public void TestLengthEmpty()
        {
            const long length = 3;
            FourBitDigitBigArray a = makeNew(length);
            Assert.Equal(length, a.Length);
        }

        [Fact]
        public void TestLengthEmptyEven()
        {
            const long length = 4;
            FourBitDigitBigArray a = makeNew(length);
            Assert.Equal(length, a.Length);
        }

        [Fact]
        public void TestLengthBig()
        {
            const long length = 3000000001;
            FourBitDigitBigArray a = makeNew(length);
            Assert.Equal(length, a.Length);
        }

        [Fact]
        public void TestLengthBigEven()
        {
            const long length = 3000000000;
            FourBitDigitBigArray a = makeNew(length);
            Assert.Equal(length, a.Length);
        }

        [Fact]
        public void TestConstructorBigMemoryStreamAsUnderlyingStream()
        {
            BigMemoryStream stream = new BigMemoryStream(5);
            FourBitDigitBigArray a = new FourBitDigitBigArray(stream);
        }

        [Fact]
        public void TestConstructorBigMemoryStreamAsUnderlyingStreamBig()
        {
            BigMemoryStream stream = new BigMemoryStream(3000000000); //3bil bytes ~= 3GB
            FourBitDigitBigArray a = new FourBitDigitBigArray(stream);
        }

        [Fact]
        public void TestGetSetBig()
        {
            FourBitDigitBigArray a = makeNew(3000000000);
            a[2500000000] = 5;
            Assert.Equal(5, a[2500000000]);
        }

        #region Helpers

        private static FourBitDigitBigArray makeNew(long length)
        {
            //If length is odd, add one to it
            bool odd = false;
            if(length % 2 == 1)
            {
                odd = true;
                length++;
            }

            Stream stream;
            long streamLength = length / 2;
            if(length > int.MaxValue)
            {
                stream = new BigMemoryStream(streamLength);
            }
            else
            {
                stream = new MemoryStream((int)streamLength);
            }
            stream.SetLength(streamLength);

            //If the length was odd, set the last byte to 15 (last 4 bits are all 1's)
            if(odd)
            {
                stream.Position = streamLength - 1;
                stream.WriteByte(15);
            }

            FourBitDigitBigArray a = new FourBitDigitBigArray(stream);
            return a;
        }
        #endregion
    }
}
