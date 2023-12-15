using System.IO;
using System.Reflection;
using System.Text;
using StringSearch.Legacy;
using Xunit;

namespace UnitTests.Legacy
{
    public class CompressionTests
    {
        private static string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        #region Four Bit Digit
        [Fact]
        public void CompressFile4BitDigit()
        {
            const string str = "1234567890";
            string filePath = Path.Combine(workingDirectory, "testCompressFile4BitDigit.");

            writeStringFile(filePath + "txt", str);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.Equal(str, readBack);
        }

        [Fact]
        public void CompressFile4BitDigitOddLength()
        {
            const string str = "12345678905";
            string filePath = Path.Combine(workingDirectory, "testCompressFile4BitDigitOddLength.");

            writeStringFile(filePath + "txt", str);
            Compression.CompressFile4BitDigit(filePath + "txt", filePath + "4bitDigit");

            string readBack = read4BitDigitFileAsString(filePath + "4bitDigit");

            File.Delete(filePath + ".txt");
            File.Delete(filePath + ".4bitDigit");

            Assert.Equal(str, readBack);
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
