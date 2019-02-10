using System;
using System.Collections.Generic;

namespace StringSearch.Health
{
    public class HealthServiceSummary
    {
        public readonly HealthResourceSummary[] SharedResourceSummaries;
        public readonly IDictionary<string, HealthResourceSummary[]> PerDigitsResourceSummaries;
        public readonly bool AllCriticalHealthy;

        public HealthServiceSummary(HealthResourceSummary[] sharedResourceSummaries,
            IDictionary<string, HealthResourceSummary[]> perDigitsResourceSummaries, bool allCriticalHealthy)
        {
            SharedResourceSummaries = sharedResourceSummaries ??
                                      throw new ArgumentNullException(nameof(sharedResourceSummaries));
            this.PerDigitsResourceSummaries = perDigitsResourceSummaries;
            AllCriticalHealthy = allCriticalHealthy;
        }
    }
}
