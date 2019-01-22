using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Collections;

namespace StringSearch.Api.Infrastructure.StringSearch.Wrappers
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
