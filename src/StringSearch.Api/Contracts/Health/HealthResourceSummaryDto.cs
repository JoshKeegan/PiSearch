using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Contracts.Health
{
    public class HealthResourceSummaryDto
    {
        public HealthResourceDto Resource { get; set; }
        public HealthStateDto State { get; set; }
    }
}
