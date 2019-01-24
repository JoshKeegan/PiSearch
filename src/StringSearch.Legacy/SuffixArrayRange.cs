/*
 * PiSearch
 * SuffixArrayRange - Represents a range of values in a suffix array
 * By Josh Keegan 23/01/2015
 */

using System;
using StringSearch.Legacy.Collections;

namespace StringSearch.Legacy
{
    public class SuffixArrayRange
    {
        //Private variables
        private bool calculatedSortedValues = false;
        private long[] sortedValues;

        //Public accessors & modifiers
        public readonly bool HasResults;
        public readonly long Min;
        public readonly long Max;
        public readonly IBigArray<ulong> SuffixArray;
        public readonly IBigArray<byte> Digits;
        public long[] SortedValues
        {
            get
            {
                if (!calculatedSortedValues)
                {
                    if(HasResults)
                    {
                        sortedValues = new long[Max - Min + 1];
                        for (long i = Min; i <= Max; i++)
                        {
                            sortedValues[i - Min] = (long)SuffixArray[i];
                        }

                        //Sort the array of string indices (Array.Sort implements Quicksort)
                        Array.Sort(sortedValues);
                    }
                    else
                    {
                        sortedValues = new long[0];
                    }

                    calculatedSortedValues = true;
                }
                return sortedValues;
            }
        }

        // Constructors
        public SuffixArrayRange(long min, long max, IBigArray<ulong> suffixArray, IBigArray<byte> digits)
        {
            HasResults = true;
            Min = min;
            Max = max;
            SuffixArray = suffixArray;
            Digits = digits;
        }

        public SuffixArrayRange(bool hasResults)
        {
            HasResults = hasResults;
        }

        public SuffixArrayRange(PrecomputedSearchResult precomputedResult, IBigArray<ulong> suffixArray,
            IBigArray<byte> digits)
        {
            //If there are no results
            if(precomputedResult.MinSuffixArrayIdx == precomputedResult.MaxSuffixArrayIdx)
            {
                HasResults = false;
            }
            else //Otherwise there are search results
            {
                HasResults = true;
                Min = precomputedResult.MinSuffixArrayIdx;
                //Note that the precomputed results are stored with the max value exclusive so that it can also encode HasResults
                //  whereas this class uses inclusive, so correct for that by taking 1
                Max = precomputedResult.MaxSuffixArrayIdx - 1;
                SuffixArray = suffixArray;
                Digits = digits;
            }
        }
    }
}
