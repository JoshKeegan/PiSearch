using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StringSearch.Legacy.IO;
using Xunit;

namespace UnitTests.Legacy.IO
{
    [Trait("os", "windows")]
    public class WinFileIoTests : IDisposable
    {
        private static string workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private static readonly byte[] FILE_CONTENT = { 1, 2, 3, 4, 5, 7, 2, 5, 3, 6, 98, 2, 3, 6, 8, 3, 6, 56, 78, 22, 23, 123, 45, 201, 255, 0, 0, 1, 7, 255, 0, 12, 13 };
        private string filePath;

        [Fact]
        public void TestReadFirstByte()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestReadFirstByte");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            Assert.Equal(FILE_CONTENT[0], buffer[0]);
        }

        [Fact]
        public void TestReadWholeFile()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestReadWholeFile");

            createFile(filePath);

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            Assert.Equal(FILE_CONTENT, buffer);
        }

        [Fact]
        public void TestReadFirst8Bytes()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestReadFirst8Bytes");

            createFile(filePath);

            byte[] buffer = new byte[8];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT.Take(buffer.Length).ToArray();
            Assert.Equal(expected, buffer);
        }

        [Fact]
        public void TestWriteFirstByte()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestWriteFirstByte");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            buffer[0] = FILE_CONTENT[0];
            wfio.WriteBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT.Take(1).ToArray();
            byte[] actual = File.ReadAllBytes(filePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestWriteWholeFile()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestWriteWholeFile");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = FILE_CONTENT[i];
            }
            wfio.WriteBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT;
            byte[] actual = File.ReadAllBytes(filePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestWriteFirst8Bytes()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestWriteFirst8Bytes");

            byte[] buffer = new byte[8];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = FILE_CONTENT[i];
            }
            wfio.WriteBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT.Take(buffer.Length).ToArray();
            byte[] actual = File.ReadAllBytes(filePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestInitialPositionRead()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestInitialPositionRead");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.Equal(0, wfio.Position);


            wfio.ReadBlocks(buffer.Length);
            wfio.Close();
        }

        [Fact]
        public void TestInitialPositionWrite()
        {
            filePath = "WinFileIOTests.TestInitialPositionWrite";

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            Assert.Equal(0, wfio.Position);

            wfio.Close();
        }

        [Fact]
        public void TestPositionIncrementsRead()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestPositionIncrementsRead");

            createFile(filePath);

            byte[] buffer = new byte[5];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);

            Assert.Equal(5, wfio.Position);

            wfio.Close();
        }

        [Fact]
        public void TestPositionIncrementsWrite()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestPositionIncrementsWrite");

            byte[] buffer = new byte[] { 7, 3 };
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            wfio.WriteBlocks(buffer.Length);

            Assert.Equal(2, wfio.Position);

            wfio.Close();
        }

        [Fact]
        public void TestSetPositionRead()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestSetPositionRead");

            createFile(filePath);

            byte[] buffer = new byte[4];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.Equal(0, wfio.Position);
            wfio.Position = 5;
            Assert.Equal(5, wfio.Position);
            wfio.ReadBlocks(buffer.Length);
            Assert.Equal(5 + buffer.Length, wfio.Position);

            byte[] expected = FILE_CONTENT.Skip(5).Take(buffer.Length).ToArray();
            Assert.Equal(expected, buffer);

            wfio.Close();
        }

        [Fact]
        public void TestSetPositionWrite()
        {
            //Test setting Position durung a write by writing the contents of the file to disk backwards one byte at a time
            string filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestSetPositionWrite");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            Assert.Equal(0, wfio.Position);

            for (int i = FILE_CONTENT.Length - 1; i >= 0; i--)
            {
                wfio.Position = i;
                Assert.Equal(i, wfio.Position);

                buffer[0] = FILE_CONTENT[i];
                wfio.WriteBlocks(1);
            }
            wfio.Close();

            byte[] actual = File.ReadAllBytes(filePath);

            Assert.Equal(FILE_CONTENT, actual);
        }

        [Fact]
        public void TestSetPositionReadWrite()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestSetPositionReadWrite");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReadingWriting(filePath);

            Assert.Equal(0, wfio.Position);

            //Write stuff, move about writing more, then come back in & read it in etc...
            buffer[0] = 1;
            wfio.WriteBlocks(1);
            buffer[0] = 3;
            wfio.WriteBlocks(1);
            buffer[0] = 3;
            wfio.WriteBlocks(1);
            buffer[0] = 7;
            wfio.WriteBlocks(1);

            wfio.Position = 0;
            wfio.ReadBlocks(1);
            Assert.Equal(1, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.Equal(3, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.Equal(3, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.Equal(7, buffer[0]);

            wfio.Position = 2;
            buffer[0] = 24;
            wfio.WriteBlocks(1);
            wfio.Position = 0;
            buffer[0] = 10;
            wfio.WriteBlocks(1);
            wfio.Position = 2;
            wfio.ReadBlocks(1);
            Assert.Equal(24, buffer[0]);
            wfio.Position = 0;
            wfio.ReadBlocks(1);
            Assert.Equal(10, buffer[0]);

            wfio.Close();

            //Now check that the final file is what we expected it to be
            byte[] expected = new byte[] { 10, 3, 24, 7 };
            byte[] actual = File.ReadAllBytes(filePath);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLengthRead()
        {
            string filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestLengthRead");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Close();
        }

        [Fact]
        public void TestLengthWrite()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestLengthWrite");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);
            wfio.Close();
        }

        [Fact]
        public void TestLengthWriteIncrements()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestLengthWriteIncrements");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);

            wfio.WriteBlocks(1);
            Assert.Equal((long)FILE_CONTENT.Length + 1, wfio.Length);

            wfio.Close();
        }

        [Fact]
        public void TestLengthWriteOverwriteNotIncrement()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestLengthWriteOverwriteNotIncrement");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Position = 1;
            wfio.WriteBlocks(2);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Close();
        }

        [Fact]
        public void TestLengthWritePartialOverwrite()
        {
            filePath = Path.Combine(workingDirectory, "WinFileIOTests.TestLengthWritePartialOverwrite");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.Equal((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Position--;
            wfio.WriteBlocks(2);

            Assert.Equal((long)FILE_CONTENT.Length + 1, wfio.Length);

            wfio.Close();
        }

        public void Dispose()
        {
            if (filePath != null)
            {
                File.Delete(filePath);
            }
        }

        //Helpers
        private static void createFile(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create);
            stream.Write(FILE_CONTENT, 0, FILE_CONTENT.Length);
        }
    }
}
