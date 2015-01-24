/*
 * PiSearch
 * SuffixArrayRange - Represents a range of values in a suffix array
 * By Josh Keegan 23/01/2015
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
    }
}
