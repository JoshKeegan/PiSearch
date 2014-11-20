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
            const string FILE_NAME = "testCompressFile4BitDigit.";

            writeStringFile(FILE_NAME + "txt", STR);
            Compression.CompressFile4BitDigit(FILE_NAME + "txt", FILE_NAME + "4bitDigit");

            string readBack = read4BitDigitFileAsString(FILE_NAME + "4bitDigit");

            File.Delete(FILE_NAME + ".txt");
            File.Delete(FILE_NAME + ".4bitDigit");

            Assert.AreEqual(STR, readBack);
            Console.WriteLine(readBack);
        }

        [Test]
        public void CompressFile4BitDigitOddLength()
        {
            const string STR = "12345678905";
            const string FILE_NAME = "testCompressFile4BitDigitOddLength.";

            writeStringFile(FILE_NAME + "txt", STR);
            Compression.CompressFile4BitDigit(FILE_NAME + "txt", FILE_NAME + "4bitDigit");

            string readBack = read4BitDigitFileAsString(FILE_NAME + "4bitDigit");

            File.Delete(FILE_NAME + ".txt");
            File.Delete(FILE_NAME + ".4bitDigit");

            Assert.AreEqual(STR, readBack);
            Console.WriteLine(readBack);
        }
#endregion

        private void writeStringFile(string filePath, string content)
        {
            StreamWriter writer = new StreamWriter(filePath);
            writer.Write(content);
            writer.Close();
        }

        private string read4BitDigitFileAsString(string filePath)
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
