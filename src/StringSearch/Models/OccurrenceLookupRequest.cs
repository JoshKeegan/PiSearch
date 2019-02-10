using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Models
{
    public class OccurrenceLookupRequest
    {
        public readonly string NamedDigits;
        public readonly int FindLength;
        public readonly long MinSuffixArrayIdx;
        public readonly long MaxSuffixArrayIdx;
        public readonly int ResultId;
        public readonly int NumSurroundingDigits;

        public OccurrenceLookupRequest(string namedDigits, int findLength, long minSuffixArrayIdx,
            long maxSuffixArrayIdx, int resultId, int numSurroundingDigits)
        {
            NamedDigits = namedDigits;
            FindLength = findLength;
            MinSuffixArrayIdx = minSuffixArrayIdx;
            MaxSuffixArrayIdx = maxSuffixArrayIdx;
            ResultId = resultId;
            NumSurroundingDigits = numSurroundingDigits;
        }
    }
}
