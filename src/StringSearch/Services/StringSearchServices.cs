using StringSearch.Models;
using StringSearch.SearchRunners;
using StringSearch.Validators;

namespace StringSearch.Services
{
    public class StringSearchServices : IStringSearchServices
    {
        private readonly ILookupRequestValidator lookupRequestValidator;
        private readonly IDigitsSearchRunner digitsSearchRunner;
        private readonly IOccurrenceLookupRunner occurrenceLookupRunner;

        public StringSearchServices(
            ILookupRequestValidator lookupRequestValidator, 
            IDigitsSearchRunner digitsSearchRunner,
            IOccurrenceLookupRunner occurrenceLookupRunner)
        {
            this.lookupRequestValidator = lookupRequestValidator;
            this.digitsSearchRunner = digitsSearchRunner;
            this.occurrenceLookupRunner = occurrenceLookupRunner;
        }

        public SearchResult Count(string namedDigits, string find) => digitsSearchRunner.Search(namedDigits, find);

        public LookupResult Lookup(LookupRequest request)
        {
            lookupRequestValidator.ThrowIfInvalid(request);

            LookupResult result = new LookupResult();
            // If the suffix array indices have been specified, we don't need to run a search for them
            if (request.MinSuffixArrayIdx != null)
            {
                result.MinSuffixArrayIdx = request.MinSuffixArrayIdx;
                result.MaxSuffixArrayIdx = request.MaxSuffixArrayIdx;
            }
            else // Otherwise, run the search to find the indices
            {
                // TODO: LookupResult could contain a SearchResult??
                SearchResult searchResult = digitsSearchRunner.Search(request.NamedDigits, request.Find);
                result.MinSuffixArrayIdx = searchResult.MinSuffixArrayIdx;
                result.MaxSuffixArrayIdx = searchResult.MaxSuffixArrayIdx;
            }

            // If there are results, lookup the requested occurrence
            if (result.NumResults > 0)
            {
                OccurrenceLookupRequest occurrenceLookupRequest = new OccurrenceLookupRequest(
                    request.NamedDigits,
                    request.Find.Length,
                    // ReSharper disable PossibleInvalidOperationException
                    result.MinSuffixArrayIdx.Value, 
                    result.MaxSuffixArrayIdx.Value,
                    // ReSharper restore PossibleInvalidOperationException
                    request.ResultId,
                    request.NumSurroundingDigits);
                OccurrenceLookupResult occurrence = occurrenceLookupRunner.Lookup(occurrenceLookupRequest);

                // TODO: LookupResult could contain an OccurrenceLookupResult??
                result.ResultId = request.ResultId;
                result.ResultStringIdx = occurrence.ResultStringIndex;
                result.SurroundingDigits = occurrence.SurroundingDigits;
            }

            return result;
        }
    }
}
