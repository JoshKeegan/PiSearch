using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Api.Health;

namespace StringSearch.Api
{
    public interface IHealthCheckServices
    {
        Task<HealthServiceSummary> RunAll();
    }
}
