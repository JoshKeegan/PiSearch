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
            int lessThan = intPow(10, (uint)stringLength);
            string toStringFormatter = "D" + stringLength;

            int[] lengthChangeIndices = getLengthChangeIndices(stringLength);

            MemoryEfficientBigULongArray precomputedResults = new MemoryEfficientBigULongArray(
                lessThan + lengthChangeIndices.Length, (ulong)fourBitDigitArray.Length, new MemoryStream());

            long suffixArrayIdx = 0;
            int lengthChangeIdx = 0;

            for (int i = 0; i < lessThan; i++)
            {
                if(suffixArrayIdx < suffixArray.Length)
                {
                    //Convert what we're searching for to the digits to be searched for
                    string sSearch = i.ToString(toStringFormatter);
                    byte[] bArrSearch = SearchString.StrToByteArr(sSearch);

                    long suffixArrayVal = (long)suffixArray[suffixArrayIdx];

                    //If we're a length change index (e.g. going from search 09 to 10, we'll have skipped value
                    //  in the suffix array that points to "1", if it exists in the digits being searched)
                    //  These value(s) will need to be skipped here (if they exist)
                    if(lengthChangeIdx < lengthChangeIndices.Length && lengthChangeIndices[lengthChangeIdx] == i)
                    {
                        while (suffixArrayVal < fourBitDigitArray.Length && suffixArrayIdx < suffixArray.Length &&
                        SearchString.doesStartWithSuffix(fourBitDigitArray, bArrSearch, suffixArrayVal) == -1)
                        {
                            suffixArrayIdx++;
                            if (suffixArrayIdx < suffixArray.Length)
                            {
                                suffixArrayVal = (long)suffixArray[suffixArrayIdx];
                            }
                        }

                        //Store the suffix array min idx for this value
                        precomputedResults[lessThan + lengthChangeIdx] = (ulong)suffixArrayIdx;

                        //Stop looking for this length change & start looking for the next
                        lengthChangeIdx++;
                    }

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

        private static int[] getLengthChangeIndices(int stringLength)
        {
            //TODO: I think there should always be a length change at 0, since you're going from length 0 to length 1
            //  and if you were doing 2 digits, the first thing you'd search for is "00", but the string could end with "0"
            //  so that's the same scneario the length change indices were built to handle
            //  Check the maths or run a test to see if this is right
            int[] toRet = new int[stringLength - 1];

            int n = 1;
            for (int i = 0; i < toRet.Length; i++)
            {
                n *= 10;
                toRet[i] = n;
            }

            return toRet;
        }
    }
}
