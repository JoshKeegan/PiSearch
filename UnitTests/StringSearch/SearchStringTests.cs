using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using StringSearch;
using SuffixArray;

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
        //TODO: Expect to have the same bug as previously found in Search() when searching the last digit
        #endregion

        #region FindNextOccurrence4BitDigit(Stream, string int)
        //TODO: Expect to have the same bug as previously found in Search() when searching the last digit
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
        #endregion
    }
}
