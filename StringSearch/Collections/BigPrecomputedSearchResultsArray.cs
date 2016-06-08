/*
 * PiSearch
 * BigPrecomputedSearchResultsArray
 * By Josh Keegan 28/01/2015
 * Last Edit 08/06/2016
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringSearch.Collections
{
    public class BigPrecomputedSearchResultsArray : IBigArray<PrecomputedSearchResult>
    {
        //Private vars
        private IBigArray<ulong> underlyingArray;

        //Public accessors & modifiers
        public PrecomputedSearchResult this[long i]
        {
            get
            {
                ulong minSuffixArrayIdx = underlyingArray[i * 2];
                ulong maxSuffixArrayIdx = underlyingArray[(i * 2) + 1];

                PrecomputedSearchResult result = new PrecomputedSearchResult(
                    (long)minSuffixArrayIdx, (long)maxSuffixArrayIdx);
                return result;
            }
            set
            {
                //Validation
                if(value == null)
                {
                    throw new ArgumentNullException();
                }

                ulong minSuffixArrayIdx = (ulong)value.MinSuffixArrayIdx;
                ulong maxSuffixArrayIdx = (ulong)value.MaxSuffixArrayIdx;

                underlyingArray[i * 2] = minSuffixArrayIdx;
                underlyingArray[(i * 2) + 1] = maxSuffixArrayIdx;
            }
        }

        public long Length
        {
            get
            {
                return underlyingArray.Length / 2;
            }
        }

        //Constructor
        public BigPrecomputedSearchResultsArray(IBigArray<ulong> underlyingArray)
        {
            this.underlyingArray = underlyingArray;
        }

        //Public Methods
        public IEnumerator<PrecomputedSearchResult> GetEnumerator()
        {
            for(long i = 0; i < this.Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}
