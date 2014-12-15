/*
 * Unit Tests for BigMemoryStream
 * By Josh Keegan 15/12/2014
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch.IO;

namespace UnitTests.StringSearch.IO
{
    [TestFixture]
    public class BigMemoryStreamTests
    {
        [Test]
        public void TestConstructorNoParams()
        {
            BigMemoryStream stream = new BigMemoryStream();

            Assert.AreEqual(0, stream.Length);
            Assert.AreEqual(true, stream.CanWrite);
        }

        [Test]
        public void TestConstructorWithCapacity()
        {
            BigMemoryStream stream = new BigMemoryStream(10);

            Assert.AreEqual(10, stream.Length); //Big difference to MemoryStream => length == capacity
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void TestConstructorNegCapacity()
        {
            BigMemoryStream stream = new BigMemoryStream(-1);
        }

        [Test]
        public void TestConstructorWithBigCapacity()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB

            BigMemoryStream stream = new BigMemoryStream(length);

            Assert.AreEqual(length, stream.Length);
        }

        [Test]
        public void TestPositionStart()
        {
            BigMemoryStream stream = new BigMemoryStream();

            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void TestSetPosition()
        {
            BigMemoryStream stream = new BigMemoryStream(10);

            for(long i = 0; i < 10; i++)
            {
                stream.Position = i;

                Assert.AreEqual(i, stream.Position);
            }
        }

        [Test]
        public void TestSetPositionBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB

            BigMemoryStream stream = new BigMemoryStream(length);

            //Small position on big stream
            stream.Position = 10;
            Assert.AreEqual(10, stream.Position);

            //Big position on big stream
            stream.Position = length - 5;
            Assert.AreEqual(length - 5, stream.Position);
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void TestSetPositionNeg()
        {
            BigMemoryStream stream = new BigMemoryStream();

            stream.Position = -1;
        }

        [Test]
        public void TestPositionMovesOnRead()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;
            byte[] buffer = new byte[5];
            stream.Read(buffer, 0, 5);

            Assert.AreEqual(10, stream.Position);
        }

        [Test]
        public void TestPositionMovesOnWrite()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;

            byte[] toWrite = new byte[10];
            for (int i = 0; i < toWrite.Length; i++)
            {
                toWrite[i] = (byte)i;
            }

            stream.Write(toWrite, 0, toWrite.Length);

            Assert.AreEqual(15, stream.Position);
        }

        [Test]
        public void TestPositionMovesOnReadByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;

            stream.ReadByte();

            Assert.AreEqual(6, stream.Position);
        }

        [Test]
        public void TesPositionMovesOnWriteByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;

            stream.WriteByte(3);

            Assert.AreEqual(6, stream.Position);
        }

        [Test]
        public void TestSetPositionToLength()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            stream.Position = 5; //Should be fine to set position, but fail on read/write
        }

        [Test]
        public void TestReadWrite()
        {
            int length = 100;

            BigMemoryStream stream = new BigMemoryStream(length);

            byte[] values = new byte[length];
            for(byte i = 0; i < values.Length; i++)
            {
                values[i] = (byte)(i + 100);
            }

            stream.Write(values, 0, length);

            //Back to the start to read the values back
            stream.Position = 0;

            byte[] buffer = new byte[length];
            stream.Read(buffer, 0, length);

            CollectionAssert.AreEqual(values, buffer);
        }

        [Test]
        public void TestReadWriteBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB

            BigMemoryStream stream = new BigMemoryStream(length);

            byte[] values = new byte[100];
            for (byte i = 0; i < values.Length; i++)
            {
                values[i] = (byte)(i + 100);
            }

            stream.Write(values, 0, values.Length);

            //Back to the start to read the values back
            stream.Position = 0;

            byte[] buffer = new byte[values.Length];
            stream.Read(buffer, 0, values.Length);

            CollectionAssert.AreEqual(values, buffer);
        }

        [Test]
        public void TestReadWriteBigAboveIntegerIndex()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            long fromStreamPos = (long)int.MaxValue + 1000;

            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = fromStreamPos;

            byte[] values = new byte[100];
            for (byte i = 0; i < values.Length; i++)
            {
                values[i] = (byte)(i + 100);
            }

            stream.Write(values, 0, values.Length);

            //Back to the start (fromStreamPos) to read the values back
            stream.Position = fromStreamPos;

            byte[] buffer = new byte[values.Length];
            stream.Read(buffer, 0, values.Length);

            CollectionAssert.AreEqual(values, buffer);
        }

        [Test]
        public void TestReadWriteOffset()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            byte[] values = new byte[] { 1, 3, 7, 6, 8, 9, 2, 3, 5, 76, 34, 12, 55, 4 };

            stream.Write(values, 3, 4);

            byte[] expected1 = values.Skip(3).Take(4).ToArray();

            //Get just the bytes that were written
            byte[] justBytes = new byte[4];
            stream.Position = 0;
            stream.Read(justBytes, 0, 4);

            CollectionAssert.AreEqual(expected1, justBytes);

            //Get the bytes back using an offset on read too
            byte[] expected2 = new byte[values.Length];
            stream.Position = 0;
            stream.Read(expected2, 3, 4);

            for(int i = 3; i < 7; i++)
            {
                Assert.AreEqual(values[i], expected2[i]);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentNullException))]
        public void TestReadBufferNull()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = null;
            stream.Read(buffer, 0, 1);
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void TestReadNegOffset()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[5];
            stream.Read(buffer, -1, 1);
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void TestReadNegCount()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[5];
            stream.Read(buffer, 1, -1);
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void TestReadBufferTooSmall()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 5);
        }

        [Test]
        [ExpectedException (typeof(ArgumentException))]
        public void TestReadBufferTooSmallDueToOffset()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[4];
            stream.Read(buffer, 1, 4);
        }

        [Test]
        public void TestReadEOS()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[1];
            stream.Position = 5;
            int bytesRead = stream.Read(buffer, 0, 1);

            Assert.AreEqual(0, bytesRead);
        }

        [Test]
        public void TestReadWriteByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.WriteByte(5);

            stream.Position = 0;

            Assert.AreEqual(5, stream.ReadByte());
        }

        [Test]
        public void TestReadByteEOS()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 100;
            int b = stream.ReadByte();

            Assert.AreEqual(-1, b);
        }

        [Test]
        public void TestReadByteEOSBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = length;
            int b = stream.ReadByte();

            Assert.AreEqual(-1, b);
        }

        [Test]
        public void TestReadWriteLastByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 99;
            stream.WriteByte(5);

            stream.Position = 99;
            int b = stream.ReadByte();
            Assert.AreEqual(5, b);
        }

        [Test]
        public void TestReadWriteLastByteBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = length - 1;
            stream.WriteByte(5);

            stream.Position = length - 1;
            int b = stream.ReadByte();
            Assert.AreEqual(5, b);
        }

        [Test]
        public void TestReadWriteLastByteInUnderlyingStream()
        {
            long length = BigMemoryStream.MEMORY_STREAM_MAX_SIZE + 5;
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            int b = stream.ReadByte();
            Assert.AreEqual(5, b);
        }

        [Test]
        public void TestReadWriteFirstByteInSecondUnderlyingStream()
        {
            long length = BigMemoryStream.MEMORY_STREAM_MAX_SIZE + 5;
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE;
            int b = stream.ReadByte();
            Assert.AreEqual(5, b);
        }

        [Test]
        public void TestReadWriteLastByteInStreamAtMaxOfUnderlyingStream()
        {
            BigMemoryStream stream = new BigMemoryStream(BigMemoryStream.MEMORY_STREAM_MAX_SIZE);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            int b = stream.ReadByte();
            Assert.AreEqual(5, b);
        }

        [Test]
        public void TestReadByteUnset()
        {
            BigMemoryStream stream = new BigMemoryStream(100);
            stream.Position = 50;

            int b = stream.ReadByte();
            Assert.AreEqual(0, b);
        }

        [Test]
        public void TestReadLastByteUnset()
        {
            BigMemoryStream stream = new BigMemoryStream(100);
            stream.Position = stream.Length - 1;

            int b = stream.ReadByte();
            Assert.AreEqual(0, b);
        }

        [Test]
        public void TestReadLastByteUnsetBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);
            stream.Position = stream.Length - 1;

            int b = stream.ReadByte();
            Assert.AreEqual(0, b);
        }

        //TODO: Test SetLength

        //TODO: Test Seek (once implemented)
    }
}
