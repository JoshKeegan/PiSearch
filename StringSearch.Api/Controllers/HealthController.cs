using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts;
using StringSearch.Api.Contracts.Health;
using StringSearch.Api.Health;

namespace StringSearch.Api.Controllers
{
    [Route("api/Health")]
    public class HealthController : Controller
    {
        private readonly IHealthResource[] healthResources;
        
        public HealthController(IEnumerable<IHealthResource> healthResources)
        {
            this.healthResources = healthResources?.Where(r => r != null)?.ToArray() ??
                                   throw new ArgumentNullException(nameof(healthResources));
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
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
                HealthState state = await tasks[i];
                summaries[i] = new HealthResourceSummary(resource, state);

                if (resource.Critical && !state.Healthy)
                {
                    allCriticalHealthy = false;
                }
            }

            return StatusCode(allCriticalHealthy ? 200 : 500, summaries);
        }
    }
}