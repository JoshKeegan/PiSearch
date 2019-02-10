using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts.Health;
using StringSearch.Health;
using StringSearch.Services;

namespace StringSearch.Api.Controllers
{
    [Route("api/v1/Health")]
    public class HealthController : Controller
    {
        private readonly IHealthCheckServices healthCheckServices;
        private readonly IMapper mapper;

        public HealthController(IHealthCheckServices healthCheckServices, IMapper mapper)
        {
            this.healthCheckServices = healthCheckServices;
            this.mapper = mapper;
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