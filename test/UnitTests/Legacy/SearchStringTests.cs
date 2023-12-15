using System;
using System.Collections.Generic;
using System.IO;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;
using SuffixArray;
using UnitTests.TestObjects.Extensions;
using Xunit;

namespace UnitTests.Legacy
{
    public class SearchStringTests
    {
        #region Search(string, string)
        [Fact]
        public void SequentialSearch()
        {
            const string str = "123456789991234";

            Dictionary<string, long[]> answers = new Dictionary<string, long[]>()
            {
                { "1", new long[] { 0, 11 } },
                { "2", new long[] { 1, 12 } },
                { "12", new long[] { 0, 11 } },
                { "5", new long[] { 4 } }
            };

            foreach(KeyValuePair<string, long[]> kvp in answers)
            {
                string find = kvp.Key;
                long[] expected = kvp.Value;

                long[] actual = SearchString.Search(str, find);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void SequentialSearchLastValue()
        {
            const string str = "123456789991234";

            Dictionary<string, long[]> answers = new Dictionary<string, long[]>()
            {
                { "4", new long[] { 3, 14 } }
            };

            foreach (KeyValuePair<string, long[]> kvp in answers)
            {
                string find = kvp.Key;
                long[] expected = kvp.Value;

                long[] actual = SearchString.Search(str, find);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void SequentialSearchFullString()
        {
            const string str = "123456789991234";

            long[] expected = new long[] { 0 };

            long[] actual = SearchString.Search(str, str);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SequentialSearchLookForLongerThanToSearch()
        {
            const string str = "123456789991234";
            const string find = str + "7";

            long[] expected = new long[0];

            long[] actual = SearchString.Search(str, find);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SequentialSearchSearchNull()
        {
            Assert.Throws<ArgumentNullException>(() => SearchString.Search(null, "1"));
        }

        [Fact]
        public void SequentialSearchLookForNull()
        {
            Assert.Throws<ArgumentNullException>(() => SearchString.Search("123", null));
        }

        [Fact]
        public void SequentialSearchLookForEmptyString()
        {
            const string str = "123456789991234";
            const string find = "";

            Assert.Throws<ArgumentException>(() => SearchString.Search(str, find));
        }

        [Fact]
        public void SequentialSearchSearchEmptyString()
        {
            const string str = "";
            const string find = "1";

            long[] expected = new long[0];

            long[] actual = SearchString.Search(str, find);

            Assert.Equal(expected, actual);
        }
        #endregion

        #region Search(int[], FourBitDigitBigArray, string)
        [Fact]
        public void SearchSuffixArray()
        {
            const string str = "123456789";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            for(int i = 0; i < str.Length; i++)
            {
                for(int j = i + 1; j <= str.Length; j++)
                {
                    string find = str.Substring(i, j - i);

                    long[] seqSearchRes = SearchString.Search(str, find);
                    SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
                    long[] suffixArraySearchRes = suffixArrayRange.SortedValues;

                    Assert.Equal(seqSearchRes, suffixArraySearchRes);
                }
            }
        }

        [Fact]
        public void SearchSuffixArrayManualTest()
        {
            const string str = "1234567899912340";

            Dictionary<string, long[]> answers = new Dictionary<string, long[]>()
            {
                { "1", new long[] { 0, 11 } },
                { "2", new long[] { 1, 12 } },
                { "12", new long[] { 0, 11 } },
                { "5", new long[] { 4 } }
            };

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            foreach (KeyValuePair<string, long[]> kvp in answers)
            {
                string find = kvp.Key;
                long[] expected = kvp.Value;

                SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
                long[] actual = suffixArrayRange.SortedValues;

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void SearchSuffixArrayLastDigits()
        {
            const string str = "1234567899912340";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            const string find = "12340";

            long[] expected = new long[] { 11 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SearchSuffixArrayFirstDigits()
        {
            const string str = "1234567899912340";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            const string find = "12345678";

            long[] expected = new long[] { 0 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SearchSuffixArrayAllDigits()
        {
            const string str = "1234567899912340";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            long[] expected = new long[] { 0 };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, str);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SearchSuffixArrayForEmptyString()
        {
            const string str = "123456789";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            Assert.Throws<ArgumentException>(() => SearchString.Search(suffixArray, fourBitDigitArray, ""));
        }

        [Fact]
        public void SearchSuffixArraySearchEmptyString()
        {
            const string str = "";
            const string find = "1";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            long[] expected = new long[0];

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSuffixArrayWrongSize()
        {
            IBigArray<ulong> suffixArray = new int[] { 1, 2, 3 }.ToBigULongArray();
            FourBitDigitBigArray a = "12345".ToFourBitDigitBigArray();

            Assert.Throws<ArgumentException>(() => SearchString.Search(suffixArray, a, "23"));
        }

        [Fact]
        public void TestSuffixArraySearchDigitNotInString()
        {
            const string str = "1234567912340";
            const string find = "8";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            long[] expected = new long[] {  };

            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray, fourBitDigitArray, find);
            long[] actual = suffixArrayRange.SortedValues;

            Assert.False(suffixArrayRange.HasResults);
            Assert.Equal(expected, actual);
        }
        #endregion

        #region FindNextOccurrence(string, string, int)
        [Fact]
        public void TestFindNextOccurrence()
        {
            const string str = "123456789991234";

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

                int actual = SearchString.FindNextOccurrence(str, find, fromIdx);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestFindNextOccurrenceLastDigits()
        {
            const string str = "123456789991234";

            Assert.Equal(10, SearchString.FindNextOccurrence(str, "91234", 0));
        }

        [Fact]
        public void TestFindNextOccurrenceSearchFullString()
        {
            const string str = "123456789991234";

            Assert.Equal(0, SearchString.FindNextOccurrence(str, str, 0));
        }

        [Fact]
        public void TestFindNextOccurrenceLookForLongerThanToSearch()
        {
            const string str = "123456789991234";

            Assert.Equal(-1, SearchString.FindNextOccurrence(str, str + "1", 0));
        }

        [Fact]
        public void TestFindNextOccurrenceLookForLongerThanLeftInToSearch()
        {
            const string str = "123456789991234";

            Assert.Equal(-1, SearchString.FindNextOccurrence(str, "43", str.Length - 1));
        }

        [Fact]
        public void TestFindNextOccurrenceSearchLastDigit()
        {
            const string str = "123456789991234";

            Assert.Equal(str.Length - 1, SearchString.FindNextOccurrence(str, str[str.Length - 1].ToString(), str.Length - 1));
            Assert.Equal(-1, SearchString.FindNextOccurrence(str, "5", str.Length - 1));
        }

        [Fact]
        public void TestFindNextOccurrenceLookForEmptyString()
        {
            const string str = "123456789991234";

            ;
            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence(str, "", 0));
        }

        [Fact]
        public void TestFindNextOccurrenceSearchEmptyString()
        {
            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence("", "1", 0));
        }
        #endregion

        #region FindNextOccurrence4BitDigit(Stream, string int)
        [Fact]
        public void TestFindNextOccurrence4BitDigit()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

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

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitLastDigits()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Equal(10, SearchString.FindNextOccurrence4BitDigit(s, "91234", 0));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitSearchFullString()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Equal(0, SearchString.FindNextOccurrence4BitDigit(s, str, 0));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitLookForLongerThanToSearch()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Equal(-1, SearchString.FindNextOccurrence4BitDigit(s, str + "1", 0));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitLookForLongerThanLeftInToSearch()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Equal(-1, SearchString.FindNextOccurrence4BitDigit(s, "43", str.Length - 1));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitSearchLastDigit()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Equal(str.Length - 1, SearchString.FindNextOccurrence4BitDigit(s, str[str.Length - 1].ToString(), str.Length - 1));
            Assert.Equal(-1, SearchString.FindNextOccurrence4BitDigit(s, "5", str.Length - 1));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitLookForEmptyString()
        {
            const string str = "123456789991234";
            Stream s = str.ToFourBitDigitStream();

            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence4BitDigit(s, "", 0));
        }

        [Fact]
        public void TestFindNextOccurrence4BitDigitSearchEmptyString()
        {
            Stream s = "".ToFourBitDigitStream();

            Assert.Throws<ArgumentException>(() => SearchString.FindNextOccurrence4BitDigit(s, "1", 0));
        }
        #endregion

        #region Internals
        [Fact]
        public void TestDoesStartWithSuffix()
        {
            const string str = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            //Start index
            for (int i = 0; i < str.Length - 1; i++)
            {
                //End index
                for (int j = i + 1; j < str.Length; j++)
                {
                    string strFind = str.Substring(i, j - i);
                    byte[] find = stringToByteArr(strFind);

                    Assert.Equal(0, SearchString.DoesStartWithSuffix(fourBitDigitArray, find, i));
                }
            }
        }

        [Fact]
        public void TestDoesStartWithSuffixTooLow()
        {
            const string str = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            string strToFind = "0" + str.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.Equal(1, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Fact]
        public void TestDoesStartWithSuffixTooHigh()
        {
            const string str = "12345678901234";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            string strToFind = "2" + str.Substring(1);
            byte[] toFind = stringToByteArr(strToFind);

            Assert.Equal(-1, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFind, 0));
        }

        [Fact]
        public void TestDoesStartWithSuffixDigitArrayTooSmallMatchUntilEnd()
        {
            const string str = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            string strToFind = "901";
            byte[] toFind = stringToByteArr(strToFind);

            Assert.Equal(-1, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFind, str.Length - 2));
        }

        [Fact]
        public void TestDoesStartWithSuffixLastDigitsInDigitArray()
        {
            const string str = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            string strToFind = "90";
            byte[] toFind = stringToByteArr(strToFind);

            Assert.Equal(0, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFind, str.Length - 2));
        }

        [Fact]
        public void TestDoesStartWithSuffixDigitArrayDigitArrayTooSmallNotMatchUntilEnd()
        {
            const string str = "1234567890";

            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            string strToFindHigh = "911";
            byte[] toFindHigh = stringToByteArr(strToFindHigh);

            Assert.Equal(-1, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFindHigh, str.Length - 2));

            string strToFindLow = "871";
            byte[] toFindLow = stringToByteArr(strToFindLow);

            Assert.Equal(1, SearchString.DoesStartWithSuffix(fourBitDigitArray, toFindLow, str.Length - 2));
        }

        [Fact]
        public void TestBinarySearchForPrefixSingleChars()
        {
            const string str = "2734981324";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();

            for (int i = 0; i < str.Length; i++)
            {
                byte[] find = new byte[] { (byte)(str[i] - '0') };

                long answer = SearchString.BinarySearchForPrefix(suffixArray, fourBitDigitArray, find, 0, str.Length - 1);

                Assert.Equal(fourBitDigitArray[i], fourBitDigitArray[(long)suffixArray[answer]]);
            }
        }

        [Fact]
        public void TestBinarySearchForPrefixDoNotExist()
        {
            const string str = "8651287431284472619471";

            IBigArray<ulong> suffixArray = buildSuffixArray(str);
            FourBitDigitBigArray fourBitDigitArray = str.ToFourBitDigitBigArray();
            string[] toFind = { "1234", "0", "0987654321", "5676", "10", "111", "33" };

            foreach(string s in toFind)
            {
                byte[] find = stringToByteArr(s);

                long answer = SearchString.BinarySearchForPrefix(suffixArray, fourBitDigitArray, find, 0, fourBitDigitArray.Length - 1);

                Assert.Equal(-1, answer);
            }
        }
        #endregion

        #region Helper Methods
        private static IBigArray<ulong> buildSuffixArray(string str)
        {
            //Initialise the array that will hold the suffix array
            MemoryEfficientBigULongArray suffixArray = new MemoryEfficientBigULongArray(str.Length);

            //Calculate the suffix array
            long status = SAIS.sufsort(str, suffixArray, str.Length);

            if (status != 0)
            {
                string error = String.Format("Error occurred whilst generating the suffix array: {0}", status);
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
