using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Health
{
    public class HealthServiceSummary
    {
        public readonly HealthResourceSummary[] ResourceSummaries;
        public readonly bool AllCriticalHealthy;

        public HealthServiceSummary(HealthResourceSummary[] resourceSummaries, bool allCriticalHealthy)
        {
            ResourceSummaries = resourceSummaries ?? throw new ArgumentNullException(nameof(resourceSummaries));
            AllCriticalHealthy = allCriticalHealthy;
        }
    }
}
