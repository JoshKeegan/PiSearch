/*
 * PiSearch
 * Compression Unit Tests
 * By Josh Keegan 20/11/2014
 * Last Edit 21/11/2014
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch;

namespace UnitTests.StringSearch
{
    [TestFixture]
    public class CompressionTests
    {
#region Four Bit Digit
        [Test]
        public void CompressFile4BitDigit()
        {
            const string STR = "1234567890";
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testCompressFile4BitDigit.");

            writeStringFile(filePath + "txt", STR);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.AreEqual(STR, readBack);
            Console.WriteLine(readBack);
        }

        [Test]
        public void CompressFile4BitDigitOddLength()
        {
            const string STR = "12345678905";
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testCompressFile4BitDigitOddLength.");

            writeStringFile(filePath + "txt", STR);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.AreEqual(STR, readBack);
            Console.WriteLine(readBack);
        }
#endregion

        private static void writeStringFile(string filePath, string content)
        {
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        private static string read4BitDigitFileAsString(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);

            StringBuilder builder = new StringBuilder((int)stream.Length * 2);

            while(true)
            {
                int b = stream.ReadByte();

                if(b == -1)
                {
                    break;
                }

                int left = b >> 4;
                int right = b & 15; // AND 1111

                builder.Append(left);

                if(right != 15)
                {
                    builder.Append(right);
                }
            }

            stream.Close();
            return builder.ToString();
        }
    }
}
