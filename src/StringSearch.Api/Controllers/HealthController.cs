using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
        private readonly ILogger logger;

        public HealthController(IHealthCheckServices healthCheckServices, IMapper mapper, ILogger logger)
        {
            this.healthCheckServices = healthCheckServices;
            this.mapper = mapper;
            this.logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            logger.Debug("Request {protocol}, {scheme}, {host} and {@headers}", Request.Protocol, Request.Scheme,
                Request.Host, Request.Headers);

            HealthServiceSummary summary = await healthCheckServices.RunAll();

            HealthCheckResponseDto responseDto = mapper.Map<HealthCheckResponseDto>(summary);

            return StatusCode(summary.AllCriticalHealthy ? 200 : 500, responseDto);
        }
    }
}