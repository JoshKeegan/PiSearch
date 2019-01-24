using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using StringSearch.Legacy.IO;

namespace UnitTests.libStringSearch.IO
{
    [TestFixture]
    [Category("windows")]
    public class WinFileIoTests
    {
        private static readonly byte[] FILE_CONTENT = { 1, 2, 3, 4, 5, 7, 2, 5, 3, 6, 98, 2, 3, 6, 8, 3, 6, 56, 78, 22, 23, 123, 45, 201, 255, 0, 0, 1, 7, 255, 0, 12, 13 };
        private static HashSet<string> fileNames = new HashSet<string>();

        [Test]
        public void TestReadFirstByte()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestReadFirstByte");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            Assert.AreEqual(FILE_CONTENT[0], buffer[0]);
        }

        [Test]
        public void TestReadWholeFile()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestReadWholeFile");

            createFile(filePath);

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            CollectionAssert.AreEqual(FILE_CONTENT, buffer);
        }

        [Test]
        public void TestReadFirst8Bytes()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestReadFirst8Bytes");

            createFile(filePath);

            byte[] buffer = new byte[8];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT.Take(buffer.Length).ToArray();
            CollectionAssert.AreEqual(expected, buffer);
        }

        [Test]
        public void TestWriteFirstByte()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestWriteFirstByte");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            buffer[0] = FILE_CONTENT[0];
            wfio.WriteBlocks(buffer.Length);
            wfio.Close();

            byte[] expected = FILE_CONTENT.Take(1).ToArray();
            byte[] actual = readFile(filePath);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteWholeFile()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestWriteWholeFile");

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
            byte[] actual = readFile(filePath);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestWriteFirst8Bytes()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestWriteFirst8Bytes");

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
            byte[] actual = readFile(filePath);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestInitialPositionRead()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestInitialPositionRead");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.AreEqual(0, wfio.Position);


            wfio.ReadBlocks(buffer.Length);
            wfio.Close();
        }

        [Test]
        public void TestInitialPositionWrite()
        {
            const string fileName = "WinFileIOTests.TestInitialPositionWrite";

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(fileName);

            Assert.AreEqual(0, wfio.Position);

            wfio.Close();

            fileNames.Add(fileName);
        }

        [Test]
        public void TestPositionIncrementsRead()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestPositionIncrementsRead");

            createFile(filePath);

            byte[] buffer = new byte[5];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);
            wfio.ReadBlocks(buffer.Length);

            Assert.AreEqual(5, wfio.Position);

            wfio.Close();
        }

        [Test]
        public void TestPositionIncrementsWrite()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestPositionIncrementsWrite");

            byte[] buffer = new byte[] { 7, 3 };
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            wfio.WriteBlocks(buffer.Length);

            Assert.AreEqual(2, wfio.Position);

            wfio.Close();

            fileNames.Add(filePath);
        }

        [Test]
        public void TestSetPositionRead()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestSetPositionRead");

            createFile(filePath);

            byte[] buffer = new byte[4];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.AreEqual(0, wfio.Position);
            wfio.Position = 5;
            Assert.AreEqual(5, wfio.Position);
            wfio.ReadBlocks(buffer.Length);
            Assert.AreEqual(5 + buffer.Length, wfio.Position);

            byte[] expected = FILE_CONTENT.Skip(5).Take(buffer.Length).ToArray();
            CollectionAssert.AreEqual(expected, buffer);

            wfio.Close();
        }

        [Test]
        public void TestSetPositionWrite()
        {
            //Test setting Position durung a write by writing the contents of the file to disk backwards one byte at a time
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestSetPositionWrite");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);

            Assert.AreEqual(0, wfio.Position);

            for(int i = FILE_CONTENT.Length - 1; i >= 0; i--)
            {
                wfio.Position = i;
                Assert.AreEqual(i, wfio.Position);

                buffer[0] = FILE_CONTENT[i];
                wfio.WriteBlocks(1);
            }
            wfio.Close();

            byte[] actual = readFile(filePath);

            CollectionAssert.AreEqual(FILE_CONTENT, actual);
        }

        [Test]
        public void TestSetPositionReadWrite()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestSetPositionReadWrite");

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReadingWriting(filePath);

            Assert.AreEqual(0, wfio.Position);

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
            Assert.AreEqual(1, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.AreEqual(3, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.AreEqual(3, buffer[0]);
            wfio.ReadBlocks(1);
            Assert.AreEqual(7, buffer[0]);

            wfio.Position = 2;
            buffer[0] = 24;
            wfio.WriteBlocks(1);
            wfio.Position = 0;
            buffer[0] = 10;
            wfio.WriteBlocks(1);
            wfio.Position = 2;
            wfio.ReadBlocks(1);
            Assert.AreEqual(24, buffer[0]);
            wfio.Position = 0;
            wfio.ReadBlocks(1);
            Assert.AreEqual(10, buffer[0]);

            wfio.Close();

            //Now check that the final file is what we expected it to be
            byte[] expected = new byte[] { 10, 3, 24, 7 };
            byte[] actual = readFile(filePath);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestLengthRead()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestLengthRead");

            createFile(filePath);

            byte[] buffer = new byte[1];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForReading(filePath);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Close();
        }

        [Test]
        public void TestLengthWrite()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestLengthWrite");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);
            wfio.Close();

            fileNames.Add(filePath);
        }

        [Test]
        public void TestLengthWriteIncrements()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestLengthWriteIncrements");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);

            wfio.WriteBlocks(1);
            Assert.AreEqual((long)FILE_CONTENT.Length + 1, wfio.Length);

            wfio.Close();

            fileNames.Add(filePath);
        }

        [Test]
        public void TestLengthWriteOverwriteNotIncrement()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestLengthWriteOverwriteNotIncrement");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Position = 1;
            wfio.WriteBlocks(2);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Close();

            fileNames.Add(filePath);
        }

        [Test]
        public void TestLengthWritePartialOverwrite()
        {
            string filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "WinFileIOTests.TestLengthWritePartialOverwrite");

            byte[] buffer = new byte[FILE_CONTENT.Length];
            WinFileIO wfio = new WinFileIO(buffer);
            wfio.OpenForWriting(filePath);
            Array.Copy(FILE_CONTENT, buffer, buffer.Length);
            wfio.WriteBlocks(buffer.Length);

            Assert.AreEqual((long)FILE_CONTENT.Length, wfio.Length);

            wfio.Position--;
            wfio.WriteBlocks(2);

            Assert.AreEqual((long)FILE_CONTENT.Length + 1, wfio.Length);

            wfio.Close();

            fileNames.Add(filePath);
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            foreach(string fileName in fileNames)
            {
                //Delete the file if it still exists
                try
                {
                    File.Delete(fileName);
                }
                catch {  }
            }
            fileNames = null;
        }

        //Helpers
        private static void createFile(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Create);
            stream.Write(FILE_CONTENT, 0, FILE_CONTENT.Length);
            stream.Close();

            //Keep track of the files that have been created so that they can be destroyed later
            fileNames.Add(fileName);
        }

        private static byte[] readFile(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Close();

            //Keep track of the files that have been created so that they can be destroyed later
            fileNames.Add(fileName);

            return buffer;
        }
    }
}
