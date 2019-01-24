using System;
using StringSearch.Legacy.Collections;

namespace StringSearch.LegacyWrappers
{
    public class SuffixArrayWrapper
    {
        public readonly IBigArray<ulong> SuffixArray;

        public SuffixArrayWrapper(IBigArray<ulong> suffixArray)
        {
            SuffixArray = suffixArray ?? throw new ArgumentNullException(nameof(suffixArray));
        }
    }
}
