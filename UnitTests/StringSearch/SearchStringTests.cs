/*
 * PiSearch
 * SearchString Unit Tests
 * By Josh Keegan 20/11/2014
 * Last Edit 24/03/2016
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch;
using StringSearch.BaseObjectExtensions;
using StringSearch.Collections;
using StringSearchConsole;
using SuffixArray;
using UnitTests.StringSearch.Collections;

namespace UnitTests.StringSearch
{
    [TestFixture]
    public class SearchStringTests
    {
        #region Search(string, string)
        [Test]
        public void SequentialSearch()
        {
            const string STR = "123456789991234";

            Dictionary<string, int[]> answers = new Dictionary<string, int[]>()
            {
                { "1", new int[] { 0, 11 } },
                { "2", new int[] { 1, 12 } },
                { "12", new int[] { 0, 11 } },
                { "5", new int[] { 4 } }
            };

            foreach(KeyValuePair<string, int[]> kvp in answers)
            {
                string find = kvp.Key;
                int[] expected = kvp.Value;

                int[] actual = SearchString.Search(STR, find);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SequentialSearchLastValue()
        {
            const string STR = "123456789991234";

            Dictionary<string, int[]> answers = new Dictionary<string, int[]>()
            {
                { "4", new int[] { 3, 14 } }
            };

            foreach (KeyValuePair<string, int[]> kvp in answers)
            {
                string find = kvp.Key;
                int[] expected = kvp.Value;

                int[] actual = SearchString.Search(STR, find);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SequentialSearchFullString()
        {
            const string STR = "123456789991234";

            int[] expected = new int[] { 0 };

            int[] actual = SearchString.Search(STR, STR);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SequentialSearchLookForLongerThanToSearch()
        {
            const string STR = "123456789991234";
            const string FIND = STR + "7";

            int[] expected = new int[0];

            int[] actual = SearchString.Search(STR, FIND);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SequentialSearchSearchNull()
        {
            Assert.Throws<ArgumentNullException>(() => SearchString.Search(null, "1"));
        }

        [Test]
        public void SequentialSearchLookForNull()
        {
            Assert.Throws<ArgumentNullException>(() => SearchString.Search("123", null));
        }

        [Test]
        public void SequentialSearchLookForEmptyString()
        {
            const string STR = "123456789991234";
            const string FIND = "";

            Assert.Throws<ArgumentException>(() => SearchString.Search(STR, FIND));
        }

        [Test]
        public void SequentialSearchSearchEmptyString()
        {
            const string STR = "";
            const string FIND = "1";

            int[] expected = new int[0];

            int[] actual = SearchString.Search(STR, FIND);

            CollectionAssert.AreEqual(expected, actual);
        }
        #endregion

        #region Search(int[], FourBitDigitBigArray, string)
        [Test]
        public void SearchSuffixArray()
        {
            const string STR = "123456789";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            for(int i = 0; i < STR.Length; i++)
            {
                for(int j = i + 1; j <= STR.Length; j++)
                {
                    string find = STR.Substring(i, j - i);

                    long[] seqSearchRes = SearchString.Search(STR, find).ToLongArr();
                    SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
                    long[] suffixArraySearchRes = suffixArrayRange.SortedValues;

                    CollectionAssert.AreEqual(seqSearchRes, suffixArraySearchRes);
                }
            }
        }

        [Test]
        public void SearchSuffixArrayManualTest()
        {
            const string STR = "1234567899912340";

            Dictionary<string, long[]> answers = new Dictionary<string, long[]>()
            {
                { "1", new long[] { 0, 11 } },
                { "2", new long[] { 1, 12 } },
                { "12", new long[] { 0, 11 } },
                { "5", new long[] { 4 } }
            };

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            foreach (KeyValuePair<string, long[]> kvp in answers)
            {
                string find = kvp.Key;
                long[] expected = kvp.Value;

                SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
                long[] actual = suffixArrayRange.SortedValues;

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SearchSuffixArrayLastDigits()
        {
            const string STR = "1234567899912340";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            const string FIND = "12340";

            long[] expected = new long[] { 11 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, FIND);
            long[] actual = suffixArrayRange.SortedValues;

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayFirstDigits()
        {
            const string STR = "1234567899912340";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            const string FIND = "12345678";

            long[] expected = new long[] { 0 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, FIND);
            long[] actual = suffixArrayRange.SortedValues;

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayAllDigits()
        {
            const string STR = "1234567899912340";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            long[] expected = new long[] { 0 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, STR);
            long[] actual = suffixArrayRange.SortedValues;

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayForEmptyString()
        {
            const string STR = "123456789";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            Assert.Throws<ArgumentException>(() => SearchString.Search(suffixArray, fourBitDigitArray, ""));
        }

        [Test]
        public void SearchSuffixArraySearchEmptyString()
        {
            const string STR = "";
            const string FIND = "1";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            long[] expected = new long[0];

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, FIND);
            long[] actual = suffixArrayRange.SortedValues;

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSuffixArrayWrongSize()
        {
            BigArray<ulong> suffixArray = Program.convertIntArrayToBigUlongArray(new int[] { 1, 2, 3 });
            FourBitDigitBigArray a = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray("12345");

            Assert.Throws<ArgumentException>(() => SearchString.Search(suffixArray, a, "23"));
        }

        [Test]
        public void TestSuffixArraySearchDigitNotInString()
        {
            const string STR = "1234567912340";
            const string FIND = "8";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            long[] expected = new long[] {  };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, FIND);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.AreEqual(false, suffixArrayRange.HasResults);
            CollectionAssert.AreEqual(expected, actual);
        }
        #endregion

        #region FindNextOccurrence(string, string, int)
        [Test]
        public void TestFindNextOccurrence()
        {
            const string STR = "123456789991234";

            Dictionary<Tuple<string, int>, int> answers = new Dictionary<Tuple<string, int>, int>()
            {
                { Tuple.Create("1", 0), 0 },
                { Tuple.Create("2", 0), 1 },
                { Tuple.Create("123", 0), 0 },
                { Tuple.Create("1", 1), 11 }
            };

            foreach(KeyValuePair<Tuple<string, int>, int> kvp in answers)
            {
                string find = kvp.Key.Item1;
                int fromIdx = kvp.Key.Item2;
                int expected = kvp.Value;

                int actual = SearchString.FindNextOccurrence(STR, find, fromIdx);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TestFindNextOccurrenceLastDigits()
        {
            const string STR = "123456789991234";

            Assert.AreEqual(10, SearchString.FindNextOccurrence(STR, "91234", 0));
        }

        [Test]
        public void TestFindNextOccurrenceSearchFullString()
        {
            const string STR = "123456789991234";

            Assert.AreEqual(0, SearchString.FindNextOccurrence(STR, STR, 0));
        }

        [Test]
        public void TestFindNextOccurrenceLookForLongerThanToSearch()
        {
            const string STR = "123456789991234";

            Assert.AreEqual(-1, SearchString.FindNextOccurrence(STR, STR + "1", 0));
        }

        [Test]
        public void TestFindNextOccurrenceLookForLongerThanLeftInToSearch()
        {
            const string STR = "123456789991234";

            Assert.AreEqual(-1, SearchString.FindNextOccurrence(STR, "43", STR.Length - 1));
        }

        [Test]
        public void TestFindNextOccurrenceSearchLastDigit()
        {
            const string STR = "123456789991234";

            Assert.AreEqual(STR.Length - 1, SearchString.FindNextOccurrence(STR, STR[STR.Length - 1].ToString(), STR.Length - 1));
            Assert.AreEqual(-1, SearchString.FindNextOccurrence(STR, "5", STR.Length - 1));
        }

        [Test]
        public void TestFindNextOccurrenceLookForEmptyString()
        {
            const string STR = "123456789991234";

            ;
            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence(STR, "", 0));
        }

        [Test]
        public void TestFindNextOccurrenceSearchEmptyString()
        {
            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence("", "1", 0));
        }
        #endregion

        #region FindNextOccurrence4BitDigit(Stream, string int)
        [Test]
        public void TestFindNextOccurrence4BitDigit()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Dictionary<Tuple<string, long>, long> answers = new Dictionary<Tuple<string, long>, long>()
            {
                { Tuple.Create("1", 0L), 0 },
                { Tuple.Create("2", 0L), 1 },
                { Tuple.Create("123", 0L), 0 },
                { Tuple.Create("1", 1L), 11 }
            };

            foreach (KeyValuePair<Tuple<string, long>, long> kvp in answers)
            {
                string find = kvp.Key.Item1;
                long fromIdx = kvp.Key.Item2;
                long expected = kvp.Value;

                long actual = SearchString.FindNextOccurrence4BitDigit(s, find, fromIdx);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitLastDigits()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(10, SearchString.FindNextOccurrence4BitDigit(s, "91234", 0));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitSearchFullString()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(0, SearchString.FindNextOccurrence4BitDigit(s, STR, 0));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitLookForLongerThanToSearch()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(-1, SearchString.FindNextOccurrence4BitDigit(s, STR + "1", 0));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitLookForLongerThanLeftInToSearch()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(-1, SearchString.FindNextOccurrence4BitDigit(s, "43", STR.Length - 1));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitSearchLastDigit()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(STR.Length - 1, SearchString.FindNextOccurrence4BitDigit(s, STR[STR.Length - 1].ToString(), STR.Length - 1));
            Assert.AreEqual(-1, SearchString.FindNextOccurrence4BitDigit(s, "5", STR.Length - 1));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitLookForEmptyString()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence4BitDigit(s, "", 0));
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitSearchEmptyString()
        {
            Stream s = FourBitDigitBigArrayTests.convertStringTo4BitDigitStream("");

            ;
            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence4BitDigit(s, "1", 0));
        }
        #endregion

        #region Internals
        [Test]
        public void TestDoesStartWithSuffix()
        {
            const string STR = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            //Start index
            for(int i = 0; i < STR.Length - 1; i++)
            {
                //End index
                for (int j = i + 1; j < STR.Length; j++)
                {
                    string strFind = STR.Substring(i, j - i);
                    byte[] find = stringToByteArr(strFind);

                    Assert.AreEqual(0, SearchString.doesStartWithSuffix(fourBitDigitArray, find, i));
                }
            }
        }

        [Test]
        public void TestDoesStartWithSuffixTooLow()
        {
            const string STR = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "0" + STR.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Test]
        public void TestDoesStartWithSuffixTooHigh()
        {
            const string STR = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "2" + STR.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(-1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Test]
        public void TestDoesStartWithSuffixDigitArrayTooSmallMatchUntilEnd()
        {
            const string STR = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "901";
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(-1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, STR.Length - 2));
        }

        [Test]
        public void TestDoesStartWithSuffixLastDigitsInDigitArray()
        {
            const string STR = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "90";
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(0, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, STR.Length - 2));
        }

        [Test]
        public void TestDoesStartWithSuffixDigitArrayDigitArrayTooSmallNotMatchUntilEnd()
        {
            const string STR = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFindHigh = "911";
            byte[] toFindHigh = stringToByteArr(strToFindHigh);

            Assert.AreEqual(-1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFindHigh, STR.Length - 2));

            string strToFindLow = "871";
            byte[] toFindLow = stringToByteArr(strToFindLow);

            Assert.AreEqual(1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFindLow, STR.Length - 2));
        }

        [Test]
        public void TestBinarySearchForPrefixSingleChars()
        {
            const string STR = "2734981324";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);

            for(int i = 0; i < STR.Length; i++)
            {
                byte[] find = new byte[] { (byte)(STR[i] - '0') };

                long answer = SearchString.binarySearchForPrefix(suffixArray, fourBitDigitArray, find, 0, STR.Length - 1);

                Assert.AreEqual(fourBitDigitArray[i], fourBitDigitArray[(long)suffixArray[answer]]);
            }
        }

        [Test]
        public void TestBinarySearchForPrefixDontExist()
        {
            const string STR = "8651287431284472619471";

            BigArray<ulong> suffixArray = buildSuffixArray(STR);
            FourBitDigitBigArray fourBitDigitArray = FourBitDigitBigArrayTests.convertStringTo4BitDigitArray(STR);
            string[] toFind = { "1234", "0", "0987654321", "5676", "10", "111", "33" };

            foreach(string s in toFind)
            {
                byte[] find = stringToByteArr(s);

                long answer = SearchString.binarySearchForPrefix(suffixArray, fourBitDigitArray, find, 0, fourBitDigitArray.Length - 1);

                Assert.AreEqual(-1, answer);
            }
        }
        #endregion

        #region Helper Methods
        private static BigArray<ulong> buildSuffixArray(string str)
        {
            //Initialise the array that will hold the suffix array
            MemoryEfficientBigULongArray suffixArray = new MemoryEfficientBigULongArray(str.Length);

            //Calculate the suffix array
            long status = SAIS.sufsort(str, suffixArray, str.Length);

            if (status != 0)
            {
                string error = String.Format("Error occurred whilst generating the suffix array: {0}", status);
                Console.WriteLine(error);
                throw new Exception(error);
            }

            return suffixArray;
        }

        private static byte[] stringToByteArr(string str)
        {
            byte[] toRet = new byte[str.Length];

            for(int i = 0; i < toRet.Length; i++)
            {
                toRet[i] = byte.Parse(str[i].ToString());
            }

            return toRet;
        }
        #endregion
    }
}
