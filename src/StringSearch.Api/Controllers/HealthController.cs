using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts;
using StringSearch.Api.Contracts.Health;
using StringSearch.Health;

namespace StringSearch.Api.Controllers
{
    [Route("api/Health")]
    public class HealthController : Controller
    {
        private readonly IHealthCheckServices healthCheckServices;
        private readonly IMapper mapper;

        public HealthController(IHealthCheckServices healthCheckServices, IMapper mapper)
        {
            this.healthCheckServices =
                healthCheckServices ?? throw new ArgumentNullException(nameof(healthCheckServices));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            HealthServiceSummary summary = await healthCheckServices.RunAll();

            HealthCheckResponseDto responseDto = mapper.Map<HealthCheckResponseDto>(summary);

            return StatusCode(summary.AllCriticalHealthy ? 200 : 500, responseDto);
        }
    }
}