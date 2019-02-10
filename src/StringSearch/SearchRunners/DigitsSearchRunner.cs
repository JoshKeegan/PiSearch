using System;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;
using StringSearch.Models;
using StringSearch.NamedDigits;
using StringSearch.NamedDigits.Factories;
using StringSearch.Validators;

namespace StringSearch.SearchRunners
{
    public class DigitsSearchRunner : IDigitsSearchRunner
    {
        private readonly IDigitsFactory digitsFactory;
        private readonly ISuffixArrayFactory suffixArrayFactory;
        private readonly IPrecomputedSearchResultsFactory precomputedSearchResultsFactory;
        private readonly IFindValidator findValidator;

        public DigitsSearchRunner(
            IDigitsFactory digitsFactory, 
            ISuffixArrayFactory suffixArrayFactory,
            IPrecomputedSearchResultsFactory precomputedSearchResultsFactory,
            IFindValidator findValidator)
        {
            this.digitsFactory = digitsFactory;
            this.suffixArrayFactory = suffixArrayFactory;
            this.precomputedSearchResultsFactory = precomputedSearchResultsFactory;
            this.findValidator = findValidator;
        }

        public SearchResult Search(string namedDigits, string find)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }
            findValidator.ThrowIfInvalid(find);

            using (ObjectWithStream<IBigArray<ulong>> suffixArray = suffixArrayFactory.Create(namedDigits))
            using (ObjectWithStream<IBigArray<byte>> digits = digitsFactory.Create(namedDigits))
            using (IPrecomputedSearchResults precomputedSearchResults = precomputedSearchResultsFactory.Create(namedDigits))
            {
                SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArray.Object, digits.Object, find,
                    precomputedSearchResults.Results);

                return suffixArrayRange.HasResults
                    ? new SearchResult(suffixArrayRange.Min, suffixArrayRange.Max)
                    : new SearchResult();
            }                
        }
    }
}
