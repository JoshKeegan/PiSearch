using StringSearch.Api.Attributes.Validation;

namespace StringSearch.Api.Contracts.Searches.Lookups
{
    public class LookupRequestDto : SearchRequestDto
    {
        [PositiveInt]
        public int ResultId { get; set; } = 0;

        [PositiveLong]
        public long? MinSuffixArrayIdx { get; set; }

        [PositiveLong]
        public long? MaxSuffixArrayIdx { get; set; }

        [PositiveInt]
        public int NumSurroundingDigits { get; set; } = 20;

        /*
         * TODO:
         * More validation rules to add (are in API v1):
         * - Enforce a max suffix array range to prevent very broad/expensive searches from running (100,000 in v1)
         */
    }
}
