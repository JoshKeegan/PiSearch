using System;
using System.Collections.Generic;
using System.IO;
using StringSearch.Legacy.Collections;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class MemoryEfficientBigULongArrayTests : ForceGcBetweenTests
    {
        [Fact]
        public void TestConstructor()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10);
        }

        [Fact]
        public void TestConstructorWithMaxValue()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
        }

        [Fact]
        public void TestConstructorWithStream()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000, new MemoryStream());
        }

        [Fact]
        public void TestConstructorWithStreamNoMaxValue()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, new MemoryStream());
        }

        [Fact]
        public void TestLength()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10);
            Assert.Equal(10, arr.Length);
        }

        [Fact]
        public void TestMaxValue()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
            Assert.Equal(1000ul, arr.MaxValue);
        }

        [Fact]
        public void TestGetUnset()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
            Assert.Equal(0ul, arr[5]);
        }

        [Fact]
        public void TestGetUnsetWithSpecifiedStream()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000, new MemoryStream());
            Assert.Equal(0ul, arr[5]);
        }

        [Fact]
        public void TestGetSet()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
            arr[3] = 400;
            Assert.Equal(400ul, arr[3]);
        }

        [Fact]
        public void TestGetSetWithSpecifiedStream()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000, new MemoryStream());
            arr[3] = 400;
            Assert.Equal(400ul, arr[3]);
        }

        [Fact]
        public void TestGetSetByteAligned()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, byte.MaxValue);
            arr[7] = 12;
            Assert.Equal(12ul, arr[7]);
        }

        [Fact]
        public void TestGetSetMultipleValues()
        {
            ulong[] ulongArr = { 23, 47, 1000, 247, 803, 2, 0, 403 };
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(ulongArr.Length, 1000);

            //Populate the arr
            for(int i = 0; i < ulongArr.Length; i++)
            {
                arr[i] = ulongArr[i];
            }

            //Check the values
            for(int i = 0; i < ulongArr.Length; i++)
            {
                Assert.Equal(ulongArr[i], arr[i]);
            }
        }

        [Fact]
        public void TestGetSetMultipleValuesReverseOrder()
        {
            ulong[] ulongArr = { 23, 47, 1000, 247, 803, 2, 0, 403 };
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(ulongArr.Length, 1000);

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
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(len, (ulong)len - 1);

            long setPos = 50;
            ulong setVal = 4;

            arr[setPos] = setVal;

            Assert.Equal(setVal, arr[setPos]);
        }

        [Fact]
        public void TestGetIndexTooSmall()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                ulong val = arr[-1];
            });
        }

        [Fact]
        public void TestGetIndexTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                ulong val = arr[arr.Length];
            });
        }

        [Fact]
        public void TestSetIndexTooSmall()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                arr[-1] = 1;
            });
        }

        [Fact]
        public void TestSetValueTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                arr[1] = arr.MaxValue + 1;
            });
        }

        [Fact]
        public void TestSetIndexTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                arr[arr.Length] = 1;
            });
        }

        [Fact]
        public void TestConstructorBig()
        {
            //number 7 requires minimum 3 bits, so that's (5bil * 3) / 8 bytes ~= 1.75GiB of RAM used
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(5000000000, 7);
        }

        [Fact]
        public void TestConstructorBiggerThan2Gb()
        {
            //Test that an array larger than 2GiB in size can be constructed
            const long length = 3L * 1024L * 1024L * 1024L; // 3GiB at 1byte per el

            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(length, byte.MaxValue);
        }

        [Fact]
        public void TestGetSetBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, ulong.MaxValue);
            arr[3] = ulong.MaxValue;
            Assert.Equal(ulong.MaxValue, arr[3]);
        }

        #region Test Internals
        [Fact]
        public void TestCalculateBitsPerValue()
        {
            Dictionary<ulong, byte> answers = new Dictionary<ulong, byte>()
            {
                { 1, 1 },
                { 2, 2 },
                { 3, 2 },
                { 7, 3 },
                { byte.MaxValue, 8 },
                { uint.MaxValue, 32 },
                { ulong.MaxValue, 64 }
            };

            foreach(KeyValuePair<ulong, byte> kvp in answers)
            {
                ulong num = kvp.Key;
                byte numBits = kvp.Value;

                byte actual = MemoryEfficientBigULongArray.CalculateBitsPerValue(num);
                Assert.Equal(numBits, actual);
            }
        }
        #endregion
    }
}
