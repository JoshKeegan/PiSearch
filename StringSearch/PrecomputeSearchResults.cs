/*
 * PiSearch
 * Precompute Search Results - for precomputing the suffix array indices that would be returned by
 *  performing the full search
 * By Josh Keegan 25/01/2015
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using StringSearch.Collections;

namespace StringSearch
{
    public static class PrecomputeSearchResults
    {
        public static MemoryEfficientBigULongArray GenerateSearchResults(FourBitDigitBigArray fourBitDigitArray,
            BigArray<ulong> suffixArray, int stringLength)
        {
            int lessThan = stringLength * 10;
            string toStringFormatter = "D" + stringLength;

            MemoryEfficientBigULongArray precomputedResults = new MemoryEfficientBigULongArray(
                lessThan, (ulong)fourBitDigitArray.Length, new MemoryStream());

            long suffixArrayIdx = 0;

            for (int i = 0; i < lessThan; i++)
            {
                if(suffixArrayIdx < suffixArray.Length)
                {
                    //Convert what we're searching for to the digits to be searched for
                    string sSearch = i.ToString(toStringFormatter);
                    byte[] bArrSearch = SearchString.StrToByteArr(sSearch);

                    long suffixArrayVal = (long)suffixArray[suffixArrayIdx];

                    //Find when this string ends
                    while (suffixArrayVal < fourBitDigitArray.Length &&
                        suffixArrayIdx < suffixArray.Length &&
                        SearchString.doesStartWithSuffix(fourBitDigitArray, bArrSearch, suffixArrayVal) == 0)
                    {
                        suffixArrayIdx++;
                        if(suffixArrayIdx < suffixArray.Length)
                        {
                            suffixArrayVal = (long)suffixArray[suffixArrayIdx];
                        }
                    }

                    precomputedResults[i] = (ulong)suffixArrayIdx;
                }
                else
                {
                    precomputedResults[i] = (ulong)suffixArray.Length;
                }
            }

            return precomputedResults;
        }
    }
}
