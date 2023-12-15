using System;
using System.Collections.Generic;
using System.IO;
using StringSearch.Legacy.Collections;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class MemoryEfficientByteAlignedBigULongArrayTests : ForceGcBetweenTests
    {
        [Fact]
        public void TestConstructor()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10);
        }

        [Fact]
        public void TestConstructorWithMaxValue()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);
        }

        [Fact]
        public void TestConstructorWithStream()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000, new MemoryStream());
        }

        [Fact]
        public void TestConstructorWithStreamNoMaxValue()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, new MemoryStream());
        }

        [Fact]
        public void TestLength()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10);
            Assert.Equal(10, arr.Length);
        }

        [Fact]
        public void TestMaxValue()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);
            Assert.Equal(1000ul, arr.MaxValue);
        }

        [Fact]
        public void TestGetUnset()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);
            Assert.Equal(0ul, arr[5]);
        }

        [Fact]
        public void TestGetUnsetWithSpecifiedStream()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000, new MemoryStream());
            Assert.Equal(0ul, arr[5]);
        }

        [Fact]
        public void TestGetSet()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);
            arr[3] = 400;
            Assert.Equal(400ul, arr[3]);
        }

        [Fact]
        public void TestGetSetWithSpecifiedStream()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000, new MemoryStream());
            arr[3] = 400;
            Assert.Equal(400ul, arr[3]);
        }

        [Fact]
        public void TestGetSetByteAligned()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, byte.MaxValue);
            arr[7] = 12;
            Assert.Equal(12ul, arr[7]);
        }

        [Fact]
        public void TestGetSetMultipleValues()
        {
            ulong[] ulongArr = { 23, 47, 1000, 247, 803, 2, 0, 403 };
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(ulongArr.Length, 1000);

            //Populate the arr
            for (int i = 0; i < ulongArr.Length; i++)
            {
                arr[i] = ulongArr[i];
            }

            //Check the values
            for (int i = 0; i < ulongArr.Length; i++)
            {
                Assert.Equal(ulongArr[i], arr[i]);
            }
        }

        [Fact]
        public void TestGetSetMultipleValuesReverseOrder()
        {
            ulong[] ulongArr = { 23, 47, 1000, 247, 803, 2, 0, 403 };
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(ulongArr.Length, 1000);

            //Populate the arr
            for (int i = ulongArr.Length - 1; i >= 0; i--)
            {
                arr[i] = ulongArr[i];
            }

            //Check the values
            for (int i = ulongArr.Length - 1; i >= 0; i--)
            {
                Assert.Equal(ulongArr[i], arr[i]);
            }
        }

        [Fact]
        public void TestGetSetBigArray()
        {
            long len = 1000000000;
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(len, (ulong)len - 1);

            long setPos = 50;
            ulong setVal = 4;

            arr[setPos] = setVal;

            Assert.Equal(setVal, arr[setPos]);
        }

        [Fact]
        public void TestGetIndexTooSmall()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                ulong val = arr[-1];
            });
        }

        [Fact]
        public void TestGetIndexTooBig()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                ulong val = arr[arr.Length];
            });
        }

        [Fact]
        public void TestSetIndexTooSmall()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                arr[-1] = 1;
            });
        }

        [Fact]
        public void TestSetValueTooBig()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);


            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                arr[1] = arr.MaxValue + 1;
            });
        }

        [Fact]
        public void TestSetIndexTooBig()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, 1000);

            
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                arr[arr.Length] = 1;
            });
        }

        [Fact]
        public void TestConstructorBiggerThan2Gb()
        {
            //Test that an array larger than 2GiB in size can be constructed
            const long length = 3L * 1024L * 1024L * 1024L; // 3GiB at 1byte per el

            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(length, byte.MaxValue);
        }

        [Fact]
        public void TestGetSetBig()
        {
            MemoryEfficientByteAlignedBigULongArray arr = new MemoryEfficientByteAlignedBigULongArray(10, ulong.MaxValue);
            arr[3] = ulong.MaxValue;
            Assert.Equal(ulong.MaxValue, arr[3]);
        }

        #region Test Internals
        [Fact]
        public void TestCalculateBytesPerValue()
        {
            Dictionary<ulong, byte> answers = new Dictionary<ulong, byte>()
            {
                { 1, 1 },
                { 2, 1 },
                { 3, 1 },
                { 7, 1 },
                { byte.MaxValue, 1 },
                { uint.MaxValue, 4 },
                { ulong.MaxValue, 8 }
            };

            foreach (KeyValuePair<ulong, byte> kvp in answers)
            {
                ulong num = kvp.Key;
                byte numBits = kvp.Value;

                byte actual = MemoryEfficientByteAlignedBigULongArray.CalculateBytesPerValue(num);
                Assert.Equal(numBits, actual);
            }
        }
        #endregion
    }
}
