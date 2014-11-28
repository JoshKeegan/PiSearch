using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch;
using SuffixArray;
using System.IO;

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
        public void SequentialSearchLookForEmptyString()
        {
            const string STR = "123456789991234";
            const string FIND = "";

            try
            {
                SearchString.Search(STR, FIND);
                Assert.Fail();
            }
            catch(ArgumentException) { }
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

        #region Search(int[], FourBitDigitArray, string)
        [Test]
        public void SearchSuffixArray()
        {
            const string STR = "123456789";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            for(int i = 0; i < STR.Length; i++)
            {
                for(int j = i + 1; j <= STR.Length; j++)
                {
                    string find = STR.Substring(i, j - i);

                    int[] seqSearchRes = SearchString.Search(STR, find);
                    int[] suffixArraySearchRes = SearchString.Search(suffixArray, fourBitDigitArray, find);

                    CollectionAssert.AreEqual(seqSearchRes, suffixArraySearchRes);
                }
            }
        }

        [Test]
        public void SearchSuffixArrayManualTest()
        {
            const string STR = "1234567899912340";

            Dictionary<string, int[]> answers = new Dictionary<string, int[]>()
            {
                { "1", new int[] { 0, 11 } },
                { "2", new int[] { 1, 12 } },
                { "12", new int[] { 0, 11 } },
                { "5", new int[] { 4 } }
            };

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            foreach (KeyValuePair<string, int[]> kvp in answers)
            {
                string find = kvp.Key;
                int[] expected = kvp.Value;

                int[] actual = SearchString.Search(suffixArray, fourBitDigitArray, find);

                CollectionAssert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SearchSuffixArrayLastDigits()
        {
            const string STR = "1234567899912340";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            const string FIND = "12340";

            int[] expected = new int[] { 11 };

            int[] actual = SearchString.Search(suffixArray, fourBitDigitArray, FIND);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayFirstDigits()
        {
            const string STR = "1234567899912340";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            const string FIND = "12345678";

            int[] expected = new int[] { 0 };

            int[] actual = SearchString.Search(suffixArray, fourBitDigitArray, FIND);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayAllDigits()
        {
            const string STR = "1234567899912340";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            int[] expected = new int[] { 0 };

            int[] actual = SearchString.Search(suffixArray, fourBitDigitArray, STR);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SearchSuffixArrayForEmptyString()
        {
            const string STR = "123456789";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            try
            {
                SearchString.Search(suffixArray, fourBitDigitArray, "");
                Assert.Fail();
            }
            catch (ArgumentException) { }
        }

        [Test]
        public void SearchSuffixArraySearchEmptyString()
        {
            const string STR = "";
            const string FIND = "1";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            int[] expected = new int[0];

            int[] actual = SearchString.Search(suffixArray, fourBitDigitArray, FIND);

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void TestSuffixArrayWrongSize()
        {
            int[] suffixArray = new int[] { 1, 2, 3 };
            FourBitDigitArray a = FourBitDigitArrayTests.convertStringTo4BitDigitArray("12345");

            try
            {
                SearchString.Search(suffixArray, a, "23");
                Assert.Fail();
            }
            catch (ArgumentException) { }
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

            Assert.AreEqual(9, SearchString.FindNextOccurrence(STR, "91234", 0));
        }
        #endregion

        #region FindNextOccurrence4BitDigit(Stream, string int)
        [Test]
        public void TestFindNextOccurrence4BitDigit()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitArrayTests.convertStringTo4BitDigitStream(STR);

            Dictionary<Tuple<string, int>, int> answers = new Dictionary<Tuple<string, int>, int>()
            {
                { Tuple.Create("1", 0), 0 },
                { Tuple.Create("2", 0), 1 },
                { Tuple.Create("123", 0), 0 },
                { Tuple.Create("1", 1), 11 }
            };

            foreach (KeyValuePair<Tuple<string, int>, int> kvp in answers)
            {
                string find = kvp.Key.Item1;
                int fromIdx = kvp.Key.Item2;
                int expected = kvp.Value;

                int actual = SearchString.FindNextOccurrence4BitDigit(s, find, fromIdx);

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void TestFindNextOccurrence4BitDigitLastDigits()
        {
            const string STR = "123456789991234";
            Stream s = FourBitDigitArrayTests.convertStringTo4BitDigitStream(STR);

            Assert.AreEqual(9, SearchString.FindNextOccurrence4BitDigit(s, "91234", 0));
        }
        #endregion

        #region Internals
        [Test]
        public void TestDoesStartWithSuffix()
        {
            const string STR = "12345678901234";

            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

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

            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "0" + STR.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Test]
        public void TestDoesStartWithSuffixTooHigh()
        {
            const string STR = "12345678901234";

            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            string strToFind = "2" + STR.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.AreEqual(-1, SearchString.doesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Test]
        public void TestBinarySearchForPrefixSingleChars()
        {
            const string STR = "2734981324";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = FourBitDigitArrayTests.convertStringTo4BitDigitArray(STR);

            for(int i = 0; i < STR.Length; i++)
            {
                byte[] find = new byte[] { (byte)(STR[i] - '0') };

                int answer = SearchString.binarySearchForPrefix(suffixArray, fourBitDigitArray, find, 0, STR.Length - 1);

                Assert.AreEqual(fourBitDigitArray[i], fourBitDigitArray[suffixArray[answer]]);
            }
        }
        #endregion

        #region Helper Methods
        private static int[] buildSuffixArray(string str)
        {
            //Initialise the aray that will hold the suffix array
            int[] suffixArray = new int[str.Length];

            //Calculate the suffix array
            int status = SAIS.sufsort(str, suffixArray, str.Length);

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
