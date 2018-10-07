using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.ViewModels
{
    public class VmLookupResult : VmSearchResult
    {
        public int? ResultId;
        public long? ResultStringIndex;
        public VmSurroundingDigits SurroundingDigits;

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
