/*
 * PiSearch
 * Precompute Search Results - for precomputing the suffix array indices that would be returned by
 *  performing the full search
 * By Josh Keegan 25/01/2015
 * Last Edit 28/01/2015
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
            int lessThan = NumPrecomputedResults(stringLength);
            string toStringFormatter = "D" + stringLength;

            MemoryEfficientBigULongArray precomputedResults = new MemoryEfficientBigULongArray(
                lessThan * 2, (ulong)fourBitDigitArray.Length, new MemoryStream());

            long suffixArrayIdx = 0;

            for (int i = 0; i < lessThan; i++)
            {
                if(suffixArrayIdx < suffixArray.Length)
                {
                    //Convert what we're searching for to the digits to be searched for
                    string sSearch = i.ToString(toStringFormatter);
                    byte[] bArrSearch = SearchString.StrToByteArr(sSearch);

                    long suffixArrayVal = (long)suffixArray[suffixArrayIdx];

                    //Find when this string starts
                    while (suffixArrayVal < fourBitDigitArray.Length &&
                        suffixArrayIdx < suffixArray.Length &&
                        SearchString.doesStartWithSuffix(fourBitDigitArray, bArrSearch, suffixArrayVal) == -1)
                    {
                        suffixArrayIdx++;
                        if (suffixArrayIdx < suffixArray.Length)
                        {
                            suffixArrayVal = (long)suffixArray[suffixArrayIdx];
                        }
                    }

                    precomputedResults[i * 2] = (ulong)suffixArrayIdx;

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

                    //Noe that here the exclusive maximum is stored, so if min == max the string wasn't found
                    precomputedResults[(i * 2) + 1] = (ulong)suffixArrayIdx;
                }
                else
                {
                    precomputedResults[i * 2] = (ulong)suffixArray.Length;
                    precomputedResults[(i * 2) + 1] = (ulong)suffixArray.Length;
                }
            }

            return precomputedResults;
        }

        public static int NumPrecomputedResults(int searchStringLength)
        {
            return intPow(10, (uint)searchStringLength);
        }

        //Very simple implementation of a Pow function for integers. Has very tight range of inputs that it'll
        //  work on & no overflow checking, but will do fine for this usage
        private static int intPow(int n, uint pow)
        {
            int toRet = 1;
            for(int i = 0; i < pow; i++)
            {
                toRet *= n;
            }
            return toRet;
        }
    }
}
