/*
 * PiSearch
 * SuffixArrayRange - Represents a range of values in a suffix array
 * By Josh Keegan 23/01/2015
 * Last Edit 28/01/2015
 */

using StringSearch.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch
{
    public class SuffixArrayRange
    {
        //Private variables
        private bool calculatedSortedValues = false;
        private long[] sortedValues;

        //Public accessors & modifiers
        public bool HasResults { get; private set; }
        public long Min { get; private set; }
        public long Max { get; private set; }
        public BigArray<ulong> SuffixArray { get; private set; }
        public FourBitDigitBigArray Digits { get; private set; }
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

        public SuffixArrayRange(long min, long max, BigArray<ulong> suffixArray, FourBitDigitBigArray digits)
        {
            this.HasResults = true;
            this.Min = min;
            this.Max = max;
            this.SuffixArray = suffixArray;
            this.Digits = digits;
        }

        public SuffixArrayRange(bool hasResults)
        {
            this.HasResults = hasResults;
        }

        public SuffixArrayRange(PrecomputedSearchResult precomputedResult, BigArray<ulong> suffixArray, 
            FourBitDigitBigArray digits)
        {
            //If there are no results
            if(precomputedResult.MinSuffixArrayIdx == precomputedResult.MaxSuffixArrayIdx)
            {
                this.HasResults = false;
            }
            else //Otherwise there are search results
            {
                this.HasResults = true;
                this.Min = precomputedResult.MinSuffixArrayIdx;
                //Note that the precomputed results are stored with the max value exclusive so that it can also encode HasResults
                //  whereas this class uses inclusive, so correct for that by taking 1
                this.Max = precomputedResult.MaxSuffixArrayIdx - 1;
                this.SuffixArray = suffixArray;
                this.Digits = digits;
            }
        }
    }
}
