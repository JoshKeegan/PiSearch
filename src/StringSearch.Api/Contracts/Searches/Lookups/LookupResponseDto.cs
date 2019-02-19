namespace StringSearch.Api.Contracts.Searches.Lookups
{
    public class LookupResponseDto : SearchResponseDto
    {
        public int? ResultId;
        public long? ResultStringIdx;
        public SurroundingDigitsDto SurroundingDigits;
    }
}
