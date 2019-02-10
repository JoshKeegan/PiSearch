using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Models
{
    public class SearchResult
    {
        public long? MinSuffixArrayIdx;
        public long? MaxSuffixArrayIdx;

        public int NumResults
        {
            get
            {
                if (MinSuffixArrayIdx == null || MaxSuffixArrayIdx == null)
                {
                    return 0;
                }

                return (int) (MaxSuffixArrayIdx.Value - MinSuffixArrayIdx.Value + 1);
            }
        }

        public SearchResult() {  }

        public SearchResult(long minSuffixArrayIdx, long maxSuffixArrayIdx)
        {
            if (minSuffixArrayIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minSuffixArrayIdx), "Must be >= 0");
            }
            if (maxSuffixArrayIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSuffixArrayIdx), "Must be >= 0");
            }
            if (minSuffixArrayIdx > maxSuffixArrayIdx)
            {
                throw new ArgumentException(nameof(minSuffixArrayIdx) + " must be <= " + nameof(maxSuffixArrayIdx));
            }

            MinSuffixArrayIdx = minSuffixArrayIdx;
            MaxSuffixArrayIdx = maxSuffixArrayIdx;
        }
    }
}
