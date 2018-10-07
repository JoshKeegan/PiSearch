using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Api.Infrastructure.StringSearch.Wrappers;
using StringSearch.Collections;

namespace StringSearch.Api.Infrastructure.StringSearch
{
    public class PrecomputedSearchResults : IPrecomputedSearchResults, IDisposable
    {
        private Stream[] streams;

        public IBigArray<PrecomputedSearchResult>[] Results { get; private set; }

        public PrecomputedSearchResults(PrecomputedSearchResultsFilePaths filePaths, DigitsWrapper digitsWrapper)
        {
            streams = new Stream[filePaths.Paths.Length];
            Results = new IBigArray<PrecomputedSearchResult>[filePaths.Paths.Length];

            for (int i = 0; i < Results.Length; i++)
            {
                FileStream s = new FileStream(filePaths.Paths[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                streams[i] = s;

                int searchStringLength = i + 1;
                IBigArray<ulong> underlyingArray = new MemoryEfficientBigULongArray(
                    PrecomputeSearchResults.NumPrecomputedResults(searchStringLength) * 2,
                    (ulong) digitsWrapper.Digits.Length, s);

                IBigArray<PrecomputedSearchResult> singleLengthPrecomputedSearchResults = new BigPrecomputedSearchResultsArray(underlyingArray);
                Results[i] = singleLengthPrecomputedSearchResults;
            }
        }

        private void dispose(bool disposing)
        {
            foreach (Stream s in streams)
            {
                s.Dispose();
            }
            streams = null;

            if (disposing)
            {
                Results = null;
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PrecomputedSearchResults()
        {
            dispose(false);
        }
    }
}
