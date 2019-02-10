using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Models
{
    public class LookupRequest
    {
        public string NamedDigits;
        public string Find;
        public int ResultId = 0;
        public long? MinSuffixArrayIdx;
        public long? MaxSuffixArrayIdx;
        public int NumSurroundingDigits;
    }
}
