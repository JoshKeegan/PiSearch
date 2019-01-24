using System;
using NUnit.Framework;
using StringSearch.Legacy.Collections;

namespace UnitTests.libStringSearch.Collections
{
    [TestFixture]
    public class MemoryEfficientComplementBigULongArrayTests
    {
        [Test]
        public void TestConstructor()
        {
            MemoryEfficientComplementBigULongArray a = new MemoryEfficientComplementBigULongArray(12);
        }

        [Test]
        public void TestConstructorWithMaxValue()
        {
            new MemoryEfficientComplementBigULongArray(12, 7);
        }

        [Test]
        public void TestConstructorWithMaxValueAndValues()
        {
            new MemoryEfficientComplementBigULongArray(12, 7, new MemoryEfficientByteAlignedBigULongArray(12, 7));
        }

        [Test]
        public void TestConstructorWithMaxValueAndValuesAndComplements()
        {
            new MemoryEfficientComplementBigULongArray(12, 7, new MemoryEfficientBigULongArray(12, 7), new BigBoolArray(12));
        }

        [Test]
        public void TestConstructorWithNullValuesArray()
        {

            Assert.Throws<ArgumentNullException>(() => new MemoryEfficientComplementBigULongArray(100, 5, null));
        }

        [Test]
        public void TestConstructorWithNullComplementsArray()
        {
            Assert.Throws<ArgumentNullException>(
                () => new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(100), null));
        }

        [Test]
        public void TestConstructorWithTooSmallValuesArray()
        {
            Assert.Throws<ArgumentException>(
                () => new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(99)));
        }

        [Test]
        public void TestConstructorWithTooSMallComplementsArray()
        {
            Assert.Throws<ArgumentException>(
                () =>
                    new MemoryEfficientComplementBigULongArray(100, 5, new MemoryEfficientBigULongArray(100),
                        new BigBoolArray(99)));
        }

        [Test]
        public void TestReadWrite()
        {
            MemoryEfficientComplementBigULongArray a = new MemoryEfficientComplementBigULongArray(10, 5);

            a[3] = 5;
            a[9] = 0;
            a[0] = 4;
            a[2] = ~5ul;
            a[7] = ~0ul;
            a[8] = ~3ul;

            Assert.AreEqual(5, a[3]);
            Assert.AreEqual(0, a[9]);
            Assert.AreEqual(4, a[0]);
            Assert.AreEqual(~5ul, a[2]);
            Assert.AreEqual(~0ul, a[7]);
            Assert.AreEqual(~3ul, a[8]);
        }
    }
}
