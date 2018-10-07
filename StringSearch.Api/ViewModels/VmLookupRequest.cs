using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Api.Attributes.Validation;

namespace StringSearch.Api.ViewModels
{
    public class VmLookupRequest : VmSearchRequest
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
         * - maxSuffixArrayIdx must be >= minSuffixArrayIdx
         * - Enforce a max suffix array range to prevent very broad/expensive searches from running (100,000 in v1)
         * - minSuffixArrayIdx and maxSuffixArrayIdx must be specified when resultId is (as they will have been returned from the
         *      first request).
         * - Bounds check result ID against the suffix array range (numResults = (maxSuffixArrayIdx - minSuffixArrayIdx) + 1)
         * 
         * Optional further validation rules (not in API v1):
         * - Check min & max suffix array indices are less than the number of digits being searched (this would
         *  require the digits to be loaded somewhere already, or at least open the file to store the length and
         *  close, otherwise the length would be hardcoded).
         */
    }
}
