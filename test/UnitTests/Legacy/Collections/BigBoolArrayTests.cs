using System.IO;
using StringSearch.Legacy.Collections;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class BigBoolArrayTests
    {
        [Fact]
        public void TestConstructor()
        {
            BigBoolArray a = new BigBoolArray(10);
        }

        [Fact]
        public void TestConstructorWithStream()
        {
            BigBoolArray a = new BigBoolArray(10, new MemoryStream(2));
        }

        [Fact]
        public void TestConstructorWithStreamTooSmall()
        {
            BigBoolArray a = new BigBoolArray(10, new MemoryStream(1));
        }

        [Fact]
        public void TestReadWrite()
        {
            BigBoolArray a = new BigBoolArray(50);

            a[0] = false;
            a[1] = true;
            a[43] = true;
            a[47] = false;

            Assert.False(a[0]);
            Assert.True(a[1]);
            Assert.True(a[43]);
            Assert.False(a[47]);
        }

        [Fact]
        public void TestReadWriteAlternating()
        {
            BigBoolArray a = new BigBoolArray(100);

            for(int i = 0; i < a.Length; i++)
            {
                a[i] = i % 2 == 0;
            }

            for(int i = 0; i < a.Length; i++)
            {
                Assert.Equal(i % 2 == 0, a[i]);
            }
        }
    }
}
