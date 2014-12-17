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
    public class FixedLengthQueueTests
    {
        [Test]
        public void TestConstructorLength()
        {
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(5);
            Assert.AreEqual(5, q.Length);
        }

        [Test]
        public void TestConstructorInitialValues()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };

            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues);

            Assert.AreEqual(initialValues.Length, q.Length);

            for(int i = 0; i < initialValues.Length; i++)
            {
                Assert.AreEqual(initialValues[i], q[i]);
            }
        }

        [Test]
        public void TestConstructorInitialValuesAndHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            Assert.AreEqual(initialValues.Length, q.Length);

            for (int i = 0; i < initialValues.Length; i++)
            {
                int j = (i + head) % initialValues.Length;

                Assert.AreEqual(initialValues[j], q[i]);
            }
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void TestConstructorNegativeLength()
        {
            new FixedLengthQueue<int>(-1);
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void TestConstructorNegativeHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = -1;

            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);
        }

        [Test]
        [ExpectedException (typeof(ArgumentOutOfRangeException))]
        public void TestConstructorHeadTooBig()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = initialValues.Length;

            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);
        }

        [Test]
        public void TestAccessIndexHeadZero()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 0;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            for(int i = 0; i < q.Length; i++)
            {
                Assert.AreEqual(initialValues[i], q[i]);
            }
        }

        [Test]
        public void TestAccessIndexArbitraryHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            Assert.AreEqual(initialValues[head], q[0]);
            Assert.AreEqual(initialValues[0], q[initialValues.Length - head]);
        }

        [Test]
        public void TestEnqueue()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            q.Enqueue(5);

            Assert.AreEqual(5, q[q.Length - 1]);
        }

        [Test]
        public void TestEnqueueUnsetValues()
        {
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(12);

            q.Enqueue(5);

            Assert.AreEqual(5, q[q.Length - 1]);
        }

        [Test]
        public void TestEnqueueDoesntChangeLength()
        {
            int length = 12;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(length);

            Assert.AreEqual(length, q.Length);
            q.Enqueue(5);
            Assert.AreEqual(length, q.Length);
        }

        [Test]
        public void TestEnqueueDequeue()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            int newVal = 12;

            int expected = initialValues[head];
            int actual = q.DequeueEnqueue(newVal);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(newVal, q[q.Length - 1]);
        }
    }
}
