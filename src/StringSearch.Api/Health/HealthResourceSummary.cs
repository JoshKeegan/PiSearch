using System;

namespace StringSearch.Api.Health
{
    public class HealthResourceSummary
    {
        public readonly IHealthResource Resource;
        public readonly HealthState State;

        public HealthResourceSummary(IHealthResource resource, HealthState state)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }
}