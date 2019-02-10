using System;
using System.IO;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;

namespace StringSearch
{
    public class PrecomputedSearchResults : IPrecomputedSearchResults
    {
        private Stream[] streams;

        public IBigArray<PrecomputedSearchResult>[] Results { get; private set; }

        public PrecomputedSearchResults(string[] filePaths, long numDigits)
        {
            streams = new Stream[filePaths.Length];
            Results = new IBigArray<PrecomputedSearchResult>[filePaths.Length];

            for (int i = 0; i < Results.Length; i++)
            {
                FileStream s = new FileStream(filePaths[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                streams[i] = s;

                int searchStringLength = i + 1;
                IBigArray<ulong> underlyingArray = new MemoryEfficientBigULongArray(
                    PrecomputeSearchResults.NumPrecomputedResults(searchStringLength) * 2,
                    (ulong) numDigits, s);

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
