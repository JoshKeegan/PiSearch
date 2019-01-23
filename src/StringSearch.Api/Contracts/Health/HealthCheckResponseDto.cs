using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Contracts.Health
{
    public class HealthCheckResponseDto
    {
        public HealthResourceSummaryDto[] ResourceSummaries { get; set; }
    }
}
