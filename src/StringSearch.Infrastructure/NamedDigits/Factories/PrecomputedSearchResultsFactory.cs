using System;
using StringSearch.NamedDigits.Factories;
using StringSearch.Services;

namespace StringSearch.Infrastructure.NamedDigits.Factories
{
    public class PrecomputedSearchResultsFactory : IPrecomputedSearchResultsFactory
    {
        private readonly IPrecomputedSearchResultsFilePathsFactory precomputedSearchResultsFilePathsFactory;
        private readonly IDigitsServices digitsServices;

        public PrecomputedSearchResultsFactory(
            IPrecomputedSearchResultsFilePathsFactory precomputedSearchResultsFilePathsFactory,
            IDigitsServices digitsServices)
        {
            this.precomputedSearchResultsFilePathsFactory = precomputedSearchResultsFilePathsFactory;
            this.digitsServices = digitsServices;
        }

        public IPrecomputedSearchResults Create(string namedDigits)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }

            string[] filePaths = precomputedSearchResultsFilePathsFactory.Create(namedDigits);
            long numDigits = digitsServices.CountDigits(namedDigits);
            return new PrecomputedSearchResults(filePaths, numDigits);
        }
    }
}
