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
        public void SearchSuffixArray()
        {
            const string STR = "123456789";

            int[] suffixArray = buildSuffixArray(STR);
            FourBitDigitArray fourBitDigitArray = convertStringTo4BitDigitArray(STR);

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

        private int[] buildSuffixArray(string str)
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

        private FourBitDigitArray convertStringTo4BitDigitArray(string str)
        {
            StreamWriter writer = new StreamWriter("temp.txt");
            writer.Write(str);
            writer.Close();

            Compression.CompressFile4BitDigit("temp.txt", "temp.4bitDigit");

            Stream memStream = Compression.ReadStreamNoComression("temp.4bitDigit");

            File.Delete("temp.txt");
            File.Delete("temp.4bitDigit");

            return new FourBitDigitArray(memStream);
        }
    }
}
