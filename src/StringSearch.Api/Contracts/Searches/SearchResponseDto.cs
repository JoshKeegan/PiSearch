namespace StringSearch.Api.Contracts.Searches
{
    public abstract class SearchResponseDto
    {
        public long? MinSuffixArrayIdx;
        public long? MaxSuffixArrayIdx;
        public int NumResults;
        public long ProcessingTimeMs;
    }
}
