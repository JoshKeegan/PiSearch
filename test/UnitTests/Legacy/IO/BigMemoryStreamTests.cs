using System;
using System.Collections.Generic;
using System.Linq;
using StringSearch.Legacy.IO;
using Xunit;

namespace UnitTests.Legacy.IO
{
    public class BigMemoryStreamTests : ForceGcBetweenTests
    {
        [Fact]
        public void TestConstructorNoParams()
        {
            BigMemoryStream stream = new BigMemoryStream();

            Assert.Equal(0, stream.Length);
            Assert.True(stream.CanWrite);
        }

        [Fact]
        public void TestConstructorWithCapacity()
        {
            BigMemoryStream stream = new BigMemoryStream(10);

            Assert.Equal(10, stream.Length); //Big difference to MemoryStream => length == capacity
        }

        [Fact]
        public void TestConstructorNegCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new BigMemoryStream(-1));
        }

        [Fact]
        public void TestConstructorWithBigCapacity()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB

            BigMemoryStream stream = new BigMemoryStream(length);

            Assert.Equal(length, stream.Length);
        }

        [Fact]
        public void TestPositionStart()
        {
            BigMemoryStream stream = new BigMemoryStream();

            Assert.Equal(0, stream.Position);
        }

        [Fact]
        public void TestSetPosition()
        {
            BigMemoryStream stream = new BigMemoryStream(10);

            for(long i = 0; i < 10; i++)
            {
                stream.Position = i;

                Assert.Equal(i, stream.Position);
            }
        }

        [Fact]
        public void TestSetPositionBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB

            BigMemoryStream stream = new BigMemoryStream(length);

            //Small position on big stream
            stream.Position = 10;
            Assert.Equal(10, stream.Position);

            //Big position on big stream
            stream.Position = length - 5;
            Assert.Equal(length - 5, stream.Position);
        }

        [Fact]
        public void TestSetPositionNeg()
        {
            BigMemoryStream stream = new BigMemoryStream();

            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);
        }

        [Fact]
        public void TestPositionMovesOnRead()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;
            byte[] buffer = new byte[5];
            stream.ReadExactly(buffer, 0, 5);

            Assert.Equal(10, stream.Position);
        }

        [Fact]
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

            Assert.Equal(15, stream.Position);
        }

        [Fact]
        public void TestPositionMovesOnReadByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;

            stream.ReadByte();

            Assert.Equal(6, stream.Position);
        }

        [Fact]
        public void TesPositionMovesOnWriteByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 5;

            stream.WriteByte(3);

            Assert.Equal(6, stream.Position);
        }

        [Fact]
        public void TestSetPositionToLength()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            stream.Position = 5; //Should be fine to set position, but fail on read/write
        }

        [Fact]
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
            stream.ReadExactly(buffer, 0, length);

            Assert.Equal(values, buffer);
        }

        [Fact]
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
            stream.ReadExactly(buffer, 0, values.Length);

            Assert.Equal(values, buffer);
        }

        [Fact]
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
            stream.ReadExactly(buffer, 0, values.Length);

            Assert.Equal(values, buffer);
        }

        [Fact]
        public void TestReadWriteOffset1()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            byte[] values = new byte[] { 1, 3, 7, 6, 8, 9, 2, 3, 5, 76, 34, 12, 55, 4 };

            stream.Write(values, 3, 4);

            byte[] expected = values.Skip(3).Take(4).ToArray();

            //Get just the bytes that were written
            byte[] actual1 = new byte[4];
            stream.Position = 0;
            stream.ReadExactly(actual1, 0, 4);

            Assert.Equal(expected, actual1);

            //Get the bytes back using an offset on read too
            byte[] actual2 = new byte[values.Length];
            stream.Position = 0;
            stream.ReadExactly(actual2, 3, 4);

            for(int i = 3; i < 7; i++)
            {
                Assert.Equal(values[i], actual2[i]);
            }
        }

        [Fact]
        public void TestReadWriteOffset2()
        {
            int length = 100;
            int writeFrom = 27;

            BigMemoryStream stream = new BigMemoryStream(length);

            byte[] values = new byte[length];
            for (byte i = 0; i < values.Length; i++)
            {
                values[i] = (byte)(i + 100);
            }

            List<byte> expected = Enumerable.Repeat((byte)0, writeFrom).ToList();
            List<byte> expectedValues = values.Skip(writeFrom).Take(length - writeFrom).ToList();
            expected.AddRange(expectedValues);

            stream.Write(values, writeFrom, length - writeFrom);

            //Back to the start to read the values back
            stream.Position = 0;

            byte[] buffer = new byte[length];
            int numRead = stream.Read(buffer, writeFrom, length - writeFrom);

            Assert.Equal(expected, buffer);
            Assert.Equal(length - writeFrom, numRead);
        }

        [Fact]
        public void TestReadBufferNull()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = null;
            Assert.Throws<ArgumentNullException>(() => stream.Read(buffer, 0, 1));
        }

        [Fact]
        public void TestReadNegOffset()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[5];
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, -1, 1));
        }

        [Fact]
        public void TestReadNegCount()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[5];
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 1, -1));
        }

        [Fact]
        public void TestReadBufferTooSmall()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[4];
            Assert.Throws<ArgumentException>(() => stream.Read(buffer, 0, 5));
        }

        [Fact]
        public void TestReadBufferTooSmallDueToOffset()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[4];
            Assert.Throws<ArgumentException>(() => stream.Read(buffer, 1, 4));
        }

        [Fact]
        public void TestReadEos()
        {
            BigMemoryStream stream = new BigMemoryStream(5);

            byte[] buffer = new byte[1];
            stream.Position = 5;
            int bytesRead = stream.Read(buffer, 0, 1);

            Assert.Equal(0, bytesRead);
        }

        [Fact]
        public void TestReadLastUnset()
        {
            byte[] expected = new byte[] { 0 };

            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = stream.Length - 1;

            byte[] buffer = new byte[1];

            int bytesRead = stream.Read(buffer, 0, 1);

            Assert.Equal(1, bytesRead);
            Assert.Equal(expected, buffer);
        }

        [Fact]
        public void TestReadLastUnsetWithMaxSizeOfUnderlyingStream()
        {
            byte[] expected = new byte[] { 0 };

            BigMemoryStream stream = new BigMemoryStream(BigMemoryStream.MEMORY_STREAM_MAX_SIZE);

            stream.Position = stream.Length - 1;

            byte[] buffer = new byte[1];

            int bytesRead = stream.Read(buffer, 0, 1);

            Assert.Equal(1, bytesRead);
            Assert.Equal(expected, buffer);
        }

        [Fact]
        public void TestReadWriteByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.WriteByte(5);

            stream.Position = 0;

            Assert.Equal(5, stream.ReadByte());
        }

        [Fact]
        public void TestReadByteEos()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 100;
            int b = stream.ReadByte();

            Assert.Equal(-1, b);
        }

        [Fact]
        public void TestReadByteEosBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = length;
            int b = stream.ReadByte();

            Assert.Equal(-1, b);
        }

        [Fact]
        public void TestReadWriteLastByte()
        {
            BigMemoryStream stream = new BigMemoryStream(100);

            stream.Position = 99;
            stream.WriteByte(5);

            stream.Position = 99;
            int b = stream.ReadByte();
            Assert.Equal(5, b);
        }

        [Fact]
        public void TestReadWriteLastByteBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = length - 1;
            stream.WriteByte(5);

            stream.Position = length - 1;
            int b = stream.ReadByte();
            Assert.Equal(5, b);
        }

        [Fact]
        public void TestReadWriteLastByteInUnderlyingStream()
        {
            long length = BigMemoryStream.MEMORY_STREAM_MAX_SIZE + 5;
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            int b = stream.ReadByte();
            Assert.Equal(5, b);
        }

        [Fact]
        public void TestReadWriteFirstByteInSecondUnderlyingStream()
        {
            long length = BigMemoryStream.MEMORY_STREAM_MAX_SIZE + 5;
            BigMemoryStream stream = new BigMemoryStream(length);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE;
            int b = stream.ReadByte();
            Assert.Equal(5, b);
        }

        [Fact]
        public void TestReadWriteLastByteInStreamAtMaxOfUnderlyingStream()
        {
            BigMemoryStream stream = new BigMemoryStream(BigMemoryStream.MEMORY_STREAM_MAX_SIZE);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            stream.WriteByte(5);

            stream.Position = BigMemoryStream.MEMORY_STREAM_MAX_SIZE - 1;
            int b = stream.ReadByte();
            Assert.Equal(5, b);
        }

        [Fact]
        public void TestReadByteUnset()
        {
            BigMemoryStream stream = new BigMemoryStream(100);
            stream.Position = 50;

            int b = stream.ReadByte();
            Assert.Equal(0, b);
        }

        [Fact]
        public void TestReadLastByteUnset()
        {
            BigMemoryStream stream = new BigMemoryStream(100);
            stream.Position = stream.Length - 1;

            int b = stream.ReadByte();
            Assert.Equal(0, b);
        }

        [Fact]
        public void TestReadLastByteUnsetBig()
        {
            long length = 3L * 1024L * 1024L * 1024L; //3GiB
            BigMemoryStream stream = new BigMemoryStream(length);
            stream.Position = stream.Length - 1;

            int b = stream.ReadByte();
            Assert.Equal(0, b);
        }

        [Fact]
        public void TestReadLastByteUnsetWithMaxSizeOfUnderlyingStream()
        {
            BigMemoryStream stream = new BigMemoryStream(BigMemoryStream.MEMORY_STREAM_MAX_SIZE);

            stream.Position = stream.Length - 1;

            int b = stream.ReadByte();
            Assert.Equal(0, b);
        }

        [Fact]
        public void TestDisposed()
        {
            BigMemoryStream stream = new BigMemoryStream(5);
            stream.Close();
            Assert.Throws<ObjectDisposedException>(() => stream.Position = 5);
        }

        //TODO: Test SetLength

        //TODO: Test Seek (once implemented)
    }
}
