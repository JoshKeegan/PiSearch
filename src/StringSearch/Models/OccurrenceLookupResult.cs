using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Models
{
    public class OccurrenceLookupResult
    {
        public readonly long ResultStringIndex;
        public readonly SurroundingDigits SurroundingDigits;

        public OccurrenceLookupResult(long resultStringIndex, SurroundingDigits surroundingDigits)
        {
            ResultStringIndex = resultStringIndex;
            SurroundingDigits = surroundingDigits;
        }
    }
}
