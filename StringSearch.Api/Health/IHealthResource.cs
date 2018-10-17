using System;
using System.Threading.Tasks;

namespace StringSearch.Api.Health
{
    public interface IHealthResource
    {
        bool Critical { get; }
        Task<HealthState> CheckState();
    }
}