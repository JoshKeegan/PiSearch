using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Health;
using StringSearch.Health.Factories;

namespace StringSearch.Services
{
    public class HealthCheckServices : IHealthCheckServices
    {
        private readonly IHealthResource[] sharedHealthResources;
        private readonly IDictionary<string, IHealthResource[]> digitsHealthChecks;

        public HealthCheckServices(IEnumerable<IHealthResource> sharedHealthResources,
            IDigitsHealthChecksFactory digitsHealthChecksFactory)
        {
            this.sharedHealthResources = sharedHealthResources?.Where(r => r != null)?.ToArray() ??
                                   throw new ArgumentNullException(nameof(sharedHealthResources));

            digitsHealthChecks = digitsHealthChecksFactory.Create();
        }

        public async Task<HealthServiceSummary> RunAll()
        {
            // Run the checks in parallel
            // Shared
            Task<HealthState>[] sharedResourceTasks = new Task<HealthState>[sharedHealthResources.Length];
            for (int i = 0; i < sharedResourceTasks.Length; i++)
            {
                sharedResourceTasks[i] = sharedHealthResources[i].CheckState();
            }

            // Per digits
            Dictionary<string, Task<HealthState>[]> perDigitsTasks = new Dictionary<string, Task<HealthState>[]>();
            foreach (KeyValuePair<string, IHealthResource[]> kvp in digitsHealthChecks)
            {
                string namedDigits = kvp.Key;
                IHealthResource[] digitsResources = kvp.Value;

                Task<HealthState>[] digitsTasks = new Task<HealthState>[digitsResources.Length];
                for (int i = 0; i < digitsTasks.Length; i++)
                {
                    digitsTasks[i] = digitsResources[i].CheckState();
                }

                perDigitsTasks[namedDigits] = digitsTasks;
            }

            // Await the results
            bool allCriticalHealthy = true;

            // Shared
            HealthResourceSummary[] sharedResourceSummaries = new HealthResourceSummary[sharedResourceTasks.Length];
            for (int i = 0; i < sharedResourceTasks.Length; i++)
            {
                IHealthResource resource = sharedHealthResources[i];
                HealthState state = await sharedResourceTasks[i].ConfigureAwait(false);
                sharedResourceSummaries[i] = new HealthResourceSummary(resource, state);

                if (resource.Critical && !state.Healthy)
                {
                    allCriticalHealthy = false;
                }
            }

            // Per digits
            Dictionary<string, HealthResourceSummary[]> perDigitsResourceSummaries =
                new Dictionary<string, HealthResourceSummary[]>();
            foreach (KeyValuePair<string, Task<HealthState>[]> kvp in perDigitsTasks)
            {
                string namedDigits = kvp.Key;
                Task<HealthState>[] digitsTasks = kvp.Value;
                IHealthResource[] digitsResources = digitsHealthChecks[namedDigits];

                HealthResourceSummary[] digitsSummaries = new HealthResourceSummary[digitsTasks.Length];
                for (int i = 0; i < digitsSummaries.Length; i++)
                {
                    IHealthResource resource = digitsResources[i];
                    HealthState state = await digitsTasks[i].ConfigureAwait(false);
                    digitsSummaries[i] = new HealthResourceSummary(resource, state);

                    if (resource.Critical && !state.Healthy)
                    {
                        allCriticalHealthy = false;
                    }
                }

                perDigitsResourceSummaries.Add(namedDigits, digitsSummaries);
            }

            return new HealthServiceSummary(sharedResourceSummaries, perDigitsResourceSummaries, allCriticalHealthy);
        }
    }
}
