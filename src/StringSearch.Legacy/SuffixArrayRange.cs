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
        private bool valuesLoaded = false;
        private long[] values;

        //Public accessors & modifiers
        public readonly bool HasResults;
        public readonly long Min;
        public readonly long Max;
        public readonly IBigArray<ulong> SuffixArray;
        public readonly IBigArray<byte> Digits;
        private long[] Values
        {
            get
            {
                if (!valuesLoaded)
                {
                    if(HasResults)
                    {
                        int length = checked((int)(Max - Min + 1));
                        values = new long[length];
                        long start = Min;
                        for (int i = 0; i < length; i++)
                        {
                            values[i] = (long)SuffixArray[start + i];
                        }
                    }
                    else
                    {
                        values = [];
                    }

                    valuesLoaded = true;
                }
                return values;
            }
        }

        public long GetSorted(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            long[] values = Values;
            if (index >= values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return quickSelect(values, index);
        }

        private static long quickSelect(long[] values, int index)
        {
            int left = 0;
            int right = values.Length - 1;

            while (true)
            {
                if (left == right)
                {
                    return values[left];
                }

                int pivotIndex = selectPivotIndex(values, left, right);
                pivotIndex = partition(values, left, right, pivotIndex);

                if (index == pivotIndex)
                {
                    return values[index];
                }

                if (index < pivotIndex)
                {
                    right = pivotIndex - 1;
                }
                else
                {
                    left = pivotIndex + 1;
                }
            }
        }

        private static int selectPivotIndex(long[] values, int left, int right)
        {
            int mid = left + (right - left) / 2;
            long a = values[left];
            long b = values[mid];
            long c = values[right];

            if (a < b)
            {
                if (b < c)
                {
                    return mid;
                }

                if (a < c)
                {
                    return right;
                }

                return left;
            }

            if (a < c)
            {
                return left;
            }

            if (b < c)
            {
                return right;
            }

            return mid;
        }

        private static int partition(long[] values, int left, int right, int pivotIndex)
        {
            long pivotValue = values[pivotIndex];
            swap(values, pivotIndex, right);

            int storeIndex = left;
            for (int i = left; i < right; i++)
            {
                if (values[i] < pivotValue)
                {
                    swap(values, storeIndex, i);
                    storeIndex++;
                }
            }

            swap(values, right, storeIndex);
            return storeIndex;
        }

        private static void swap(long[] values, int left, int right)
        {
            if (left == right)
            {
                return;
            }

            (values[left], values[right]) = (values[right], values[left]);
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
