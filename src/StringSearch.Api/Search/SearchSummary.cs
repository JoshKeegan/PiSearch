using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace StringSearch.Api.Search
{
    /// <summary>
    /// The summary of a search to be recorded in the DB.
    /// Note that this is backwards compatible with the original PiSearch API, so isn't completely consistent with other parts of the new API design.
    /// </summary>
    public class SearchSummary
    {
        private const string UNDETERMINED_IP = "UNDETERMINED";
        private const int NO_RESULT_ID = -1;
        private const int NO_SUFFIX_ARRAY_INDICES = -1;

        public readonly string Find;
        public readonly int ResultId;
        public readonly long MinSuffixArrayIdx;
        public readonly long MaxSuffixArrayIdx;
        public readonly bool JustCount;
        public readonly int? NumSurroundingDigits;
        public readonly string ClientIp;
        public readonly DateTime SearchDate;
        public long ProcessingTimeMs;

        public bool HasSuffixArrayIndices => MinSuffixArrayIdx != NO_SUFFIX_ARRAY_INDICES &&
                                             MaxSuffixArrayIdx != NO_SUFFIX_ARRAY_INDICES;

        public SearchSummary(string find, int? resultId, long? minSuffixArrayIdx, long? maxSuffixArrayIdx,
            bool justCount, int? numSurroundingDigits, IPAddress clientIp)
        {
            Find = find ?? throw new ArgumentNullException(nameof(find));
            ResultId = resultId ?? NO_RESULT_ID;
            MinSuffixArrayIdx = minSuffixArrayIdx ?? NO_SUFFIX_ARRAY_INDICES;
            MaxSuffixArrayIdx = maxSuffixArrayIdx ?? NO_SUFFIX_ARRAY_INDICES;
            JustCount = justCount;
            NumSurroundingDigits = numSurroundingDigits;
            ClientIp = clientIp != null ? clientIp.ToString() : UNDETERMINED_IP;
            SearchDate = DateTime.UtcNow;
        }
    }
}
