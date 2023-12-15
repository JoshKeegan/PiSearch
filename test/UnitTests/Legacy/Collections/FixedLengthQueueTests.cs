using System;
using StringSearch.Legacy.Collections;
using Xunit;

namespace UnitTests.Legacy.Collections
{
    public class FixedLengthQueueTests
    {
        [Fact]
        public void TestConstructorLength()
        {
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(5);
            Assert.Equal(5, q.Length);
        }

        [Fact]
        public void TestConstructorInitialValues()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };

            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues);

            Assert.Equal(initialValues.Length, q.Length);

            for(int i = 0; i < initialValues.Length; i++)
            {
                Assert.Equal(initialValues[i], q[i]);
            }
        }

        [Fact]
        public void TestConstructorInitialValuesAndHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            Assert.Equal(initialValues.Length, q.Length);

            for (int i = 0; i < initialValues.Length; i++)
            {
                int j = (i + head) % initialValues.Length;

                Assert.Equal(initialValues[j], q[i]);
            }
        }

        [Fact]
        public void TestConstructorNegativeLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedLengthQueue<int>(-1));
        }

        [Fact]
        public void TestConstructorNegativeHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedLengthQueue<int>(initialValues, head));
        }

        [Fact]
        public void TestConstructorHeadTooBig()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = initialValues.Length;

            Assert.Throws<ArgumentOutOfRangeException>(() => new FixedLengthQueue<int>(initialValues, head));
        }

        [Fact]
        public void TestAccessIndexHeadZero()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 0;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            for(int i = 0; i < q.Length; i++)
            {
                Assert.Equal(initialValues[i], q[i]);
            }
        }

        [Fact]
        public void TestAccessIndexArbitraryHead()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            Assert.Equal(initialValues[head], q[0]);
            Assert.Equal(initialValues[0], q[initialValues.Length - head]);
        }

        [Fact]
        public void TestEnqueue()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            q.Enqueue(5);

            Assert.Equal(5, q[q.Length - 1]);
        }

        [Fact]
        public void TestEnqueueUnsetValues()
        {
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(12);

            q.Enqueue(5);

            Assert.Equal(5, q[q.Length - 1]);
        }

        [Fact]
        public void TestEnqueueDoesntChangeLength()
        {
            int length = 12;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(length);

            Assert.Equal(length, q.Length);
            q.Enqueue(5);
            Assert.Equal(length, q.Length);
        }

        [Fact]
        public void TestEnqueueDequeue()
        {
            int[] initialValues = new int[] { 4, 7, 3, 11, 215, -4 };
            int head = 3;
            FixedLengthQueue<int> q = new FixedLengthQueue<int>(initialValues, head);

            int newVal = 12;

            int expected = initialValues[head];
            int actual = q.DequeueEnqueue(newVal);

            Assert.Equal(expected, actual);
            Assert.Equal(newVal, q[q.Length - 1]);
        }
    }
}
