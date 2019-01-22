namespace StringSearch.Api.Contracts.Searches.Lookups
{
    public class LookupResponse : SearchResponse
    {
        public int? ResultId;
        public long? ResultStringIndex;
        public SurroundingDigits SurroundingDigits;

        /*public VmLookupResult(long? suffixArrayMinIdx, long? suffixArrayMaxIdx, int numResults, long processingTimeMs,
            int? resultId, long? resultsStringIndex, VmSurroundingDigits surroundingDigits)
            : base(suffixArrayMinIdx, suffixArrayMaxIdx, numResults, processingTimeMs)
        {
            ResultId = resultId;
            ResultStringIndex = resultsStringIndex;
            SurroundingDigits = surroundingDigits;
        }*/
    }
}
