using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch.Collections;

namespace UnitTests.StringSearch.Collections
{
    [TestFixture]
    public class MemoryEfficientBigULongArrayTests
    {
        [Test]
        public void TestConstructor()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10);
        }

        [Test]
        public void TestConstructorWithMaxValue()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
        }

        [Test]
        public void TestLength()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10);
            Assert.AreEqual(10, arr.Length);
        }

        [Test]
        public void TestMaxValue()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
            Assert.AreEqual(1000, arr.MaxValue);
        }

        [Test]
        public void TestGetSet()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);
            arr[3] = 400;
            Assert.AreEqual(400, arr[3]);
        }

        [Test]
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
                Assert.AreEqual(ulongArr[i], arr[i]);
            }
        }

        [Test]
        public void TestGetIndexTooSmall()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            try
            {
                ulong val = arr[-1];
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestGetIndexTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            try
            {
                ulong val = arr[arr.Length];
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestSetIndexTooSmall()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            try
            {
                arr[-1] = 1;
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestSetValueTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            try
            {
                arr[1] = arr.MaxValue + 1;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException) { }
        }

        [Test]
        public void TestSetIndexTooBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, 1000);

            try
            {
                arr[arr.Length] = 1;
                Assert.Fail();
            }
            catch (IndexOutOfRangeException) { }
        }

        [Test]
        public void TestConstructorBig()
        {
            //number 7 requires minimum 3 bits, so thats (5bil * 3) / 8 bytes ~= 1.75GiB of RAM used
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(5000000000, 7);
        }

        [Test]
        public void TestConstructorBiggerThan2Gb()
        {
            //Test that an array larger than 2GiB in size can be constructed
            const long LENGTH = 3L * 1024L * 1024L * 1024L; // 3GiB at 1byte per el

            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(LENGTH, byte.MaxValue);
        }

        [Test]
        public void TestGetSetBig()
        {
            MemoryEfficientBigULongArray arr = new MemoryEfficientBigULongArray(10, ulong.MaxValue);
            arr[3] = ulong.MaxValue;
            Assert.AreEqual(ulong.MaxValue, arr[3]);
        }
    }
}
