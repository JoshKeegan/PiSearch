using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch
{
    public class HealthCheckServices : IHealthCheckServices
    {
        private readonly IHealthResource[] healthResources;

        public HealthCheckServices(IEnumerable<IHealthResource> healthResources)
        {
            this.healthResources = healthResources?.Where(r => r != null)?.ToArray() ??
                                   throw new ArgumentNullException(nameof(healthResources));
        }

        public async Task<HealthServiceSummary> RunAll()
        {
            // Run the checks in parallel
            Task<HealthState>[] tasks = new Task<HealthState>[healthResources.Length];
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = healthResources[i].CheckState();
            }

            HealthResourceSummary[] summaries = new HealthResourceSummary[tasks.Length];
            bool allCriticalHealthy = true;
            for (int i = 0; i < tasks.Length; i++)
            {
                IHealthResource resource = healthResources[i];
                HealthState state = await tasks[i].ConfigureAwait(false);
                summaries[i] = new HealthResourceSummary(resource, state);

                if (resource.Critical && !state.Healthy)
                {
                    allCriticalHealthy = false;
                }
            }

            return new HealthServiceSummary(summaries, allCriticalHealthy);
        }
    }
}
