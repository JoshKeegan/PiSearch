using System;
using System.ComponentModel;

namespace StringSearch.Api.Health
{
    public class VmHealthResourceSummary
    {
        public readonly IHealthResource Resource;
        public readonly HealthState State;

        public VmHealthResourceSummary(IHealthResource resource, HealthState state)
        {
            Resource = resource ?? throw new ArgumentNullException(nameof(resource));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }
}