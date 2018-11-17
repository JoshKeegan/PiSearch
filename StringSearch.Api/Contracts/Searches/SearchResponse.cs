namespace StringSearch.Api.Contracts.Searches
{
    public abstract class SearchResponse
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
