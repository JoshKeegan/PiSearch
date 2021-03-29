using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using StringSearch.Legacy;

namespace StringSearch.Tests.Unit.Legacy
{
    [TestFixture]
    public class CompressionTests
    {
#region Four Bit Digit
        [Test]
        public void CompressFile4BitDigit()
        {
            const string str = "1234567890";
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testCompressFile4BitDigit.");

            writeStringFile(filePath + "txt", str);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.AreEqual(str, readBack);
            Console.WriteLine(readBack);
        }

        [Test]
        public void CompressFile4BitDigitOddLength()
        {
            const string str = "12345678905";
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "testCompressFile4BitDigitOddLength.");

            writeStringFile(filePath + "txt", str);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.AreEqual(str, readBack);
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
