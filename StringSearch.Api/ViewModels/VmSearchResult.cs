using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.ViewModels
{
    public abstract class VmSearchResult
    {
        public long? SuffixArrayMinIdx;
        public long? SuffixArrayMaxIdx;
        public int NumResults;
        public long ProcessingTimeMs;

        /*protected VmSearchResult(long? suffixArrayMinIdx, long? suffixArrayMaxIdx, int numResults, long processingTimeMs)
        {
            SuffixArrayMinIdx = suffixArrayMinIdx;
            SuffixArrayMaxIdx = suffixArrayMaxIdx;
            NumResults = numResults;
            ProcessingTimeMs = processingTimeMs;
        }*/
    }
}
