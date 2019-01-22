using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch.Collections;

namespace UnitTests.StringSearch.Collections
{
    [TestFixture]
    public class BigBoolArrayTests
    {
        [Test]
        public void TestConstructor()
        {
            BigBoolArray a = new BigBoolArray(10);
        }

        [Test]
        public void TestConstructorWithStream()
        {
            BigBoolArray a = new BigBoolArray(10, new MemoryStream(2));
        }

        [Test]
        public void TestConstructorWithStreamTooSmall()
        {
            BigBoolArray a = new BigBoolArray(10, new MemoryStream(1));
        }

        [Test]
        public void TestReadWrite()
        {
            BigBoolArray a = new BigBoolArray(50);

            a[0] = false;
            a[1] = true;
            a[43] = true;
            a[47] = false;

            Assert.AreEqual(false, a[0]);
            Assert.AreEqual(true, a[1]);
            Assert.AreEqual(true, a[43]);
            Assert.AreEqual(false, a[47]);
        }

        [Test]
        public void TestReadWriteAlternating()
        {
            BigBoolArray a = new BigBoolArray(100);

            for(int i = 0; i < a.Length; i++)
            {
                a[i] = i % 2 == 0;
            }

            for(int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(i % 2 == 0, a[i]);
            }
        }
    }
}
