using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch.Api.Contracts.Health
{
    public class HealthCheckResponseDto
    {
        public HealthResourceSummaryDto[] SharedResourceSummaries { get; set; }
        public IDictionary<string, HealthResourceSummary[]> PerDigitsResourceSummaries { get; set; }
        public bool AllCriticalHealthy { get; set; }
    }
}
