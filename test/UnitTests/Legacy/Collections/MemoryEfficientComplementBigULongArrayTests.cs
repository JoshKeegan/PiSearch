using System;
using StringSearch.Legacy.Collections;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class MemoryEfficientComplementBigULongArrayTests
    {
        [Fact]
        public void TestConstructor()
        {
            MemoryEfficientComplementBigULongArray a = new MemoryEfficientComplementBigULongArray(12);
        }

        [Fact]
        public void TestConstructorWithMaxValue()
        {
            new MemoryEfficientComplementBigULongArray(12, 7);
        }

        [Fact]
        public void TestConstructorWithMaxValueAndValues()
        {
            new MemoryEfficientComplementBigULongArray(12, 7, new MemoryEfficientByteAlignedBigULongArray(12, 7));
        }

        [Fact]
        public void TestConstructorWithMaxValueAndValuesAndComplements()
        {
            new MemoryEfficientComplementBigULongArray(12, 7, new MemoryEfficientBigULongArray(12, 7), new BigBoolArray(12));
        }

        [Fact]
        public void TestConstructorWithNullValuesArray()
        {

            Assert.Throws<ArgumentNullException>(() => new MemoryEfficientComplementBigULongArray(100, 5, null));
        }

        [Fact]
        public void TestConstructorWithNullComplementsArray()
        {
            Assert.Throws<ArgumentNullException>(
                () => new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(100), null));
        }

        [Fact]
        public void TestConstructorWithTooSmallValuesArray()
        {
            Assert.Throws<ArgumentException>(
                () => new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(99)));
        }

        [Fact]
        public void TestConstructorWithTooSMallComplementsArray()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(100),
                        new BigBoolArray(99)));
        }

        [Fact]
        public void TestReadWrite()
        {
            MemoryEfficientComplementBigULongArray a = new MemoryEfficientComplementBigULongArray(10, 5);

            a[3] = 5;
            a[9] = 0;
            a[0] = 4;
            a[2] = ~5ul;
            a[7] = ~0ul;
            a[8] = ~3ul;

            Assert.Equal(5ul, a[3]);
            Assert.Equal(0ul, a[9]);
            Assert.Equal(4ul, a[0]);
            Assert.Equal(~5ul, a[2]);
            Assert.Equal(~0ul, a[7]);
            Assert.Equal(~3ul, a[8]);
        }
    }
}
