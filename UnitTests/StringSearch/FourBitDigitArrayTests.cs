using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch;
using System.IO;

namespace UnitTests.StringSearch
{
    [TestFixture]
    public class FourBitDigitArrayTests
    {
        [Test]
        public void TestConstructor()
        {
            const string STR = "1234";

            Stream memStream = convertStringTo4BitDigitStream(STR);

            FourBitDigitArray a = new FourBitDigitArray(memStream);
        }

        [Test]
        public void TestEmpty()
        {
            Stream memStream = convertStringTo4BitDigitStream("");

            FourBitDigitArray a = new FourBitDigitArray(memStream);

            Assert.AreEqual(0, a.Length);
        }

        [Test]
        public void TestOddNumberOfDigits()
        {
            Stream memStream = convertStringTo4BitDigitStream("123");

            FourBitDigitArray a = new FourBitDigitArray(memStream);

            Assert.AreEqual(3, a.Length);
        }

        [Test]
        public void TestGet()
        {
            const string STR = "391";

            FourBitDigitArray a = convertStringTo4BitDigitArray(STR);

            for(int i = 0; i < STR.Length; i++)
            {
                char c = STR[i];
                byte b = a[i];

                Assert.AreEqual(c.ToString(), b.ToString());
            }
        }

        [Test]
        public void TestSetEven()
        {
            const string ORIG = "391";

            FourBitDigitArray a = convertStringTo4BitDigitArray(ORIG);

            a[0] = 7;
            Assert.AreEqual(7, a[0]);

            for (int i = 1; i < ORIG.Length; i++)
            {
                Assert.AreEqual(ORIG[i].ToString(), a[i].ToString());
            }
        }

        [Test]
        public void TestSetOdd()
        {
            const string ORIG = "391";

            FourBitDigitArray a = convertStringTo4BitDigitArray(ORIG);

            a[1] = 7;
            Assert.AreEqual(7, a[1]);

            for (int i = 0; i < ORIG.Length; i++)
            {
                if(i != 1)
                {
                    Assert.AreEqual(ORIG[i].ToString(), a[i].ToString());
                }
            }
        }

        [Test]
        public void TestAccessOutOfRangeNeg()
        {
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                byte b = a[-1];
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestAccessOutOfRange()
        {
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                byte b = a[3];
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestSetOutOfRangeNeg()
        {
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                a[-1] = 3;
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestSetOutOfRange()
        {
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                a[3] = 3;
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestSetOverflow()
        {
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                a[0] = 16;
                Assert.Fail();
            }
            catch (OverflowException) { }
        }

        [Test]
        public void TestSetReservedOverflow()
        {
            //Highest possible value in 4 bits (15) reserved for marking that half of the byte as not in use
            //  so it counts as overflow
            FourBitDigitArray a = convertStringTo4BitDigitArray("123");

            try
            {
                a[0] = 15;
                Assert.Fail();
            }
            catch (OverflowException) { }
        }

        #region Helpers
        public static FourBitDigitArray convertStringTo4BitDigitArray(string str)
        {
            Stream memStream = convertStringTo4BitDigitStream(str);

            return new FourBitDigitArray(memStream);
        }

        public static Stream convertStringTo4BitDigitStream(string str)
        {
            StreamWriter writer = new StreamWriter("temp.txt");
            writer.Write(str);
            writer.Close();

            Compression.CompressFile4BitDigit("temp.txt", "temp.4bitDigit");

            Stream memStream = Compression.ReadStreamNoComression("temp.4bitDigit");

            File.Delete("temp.txt");
            File.Delete("temp.4bitDigit");

            return memStream;
        }
        #endregion
    }
}
