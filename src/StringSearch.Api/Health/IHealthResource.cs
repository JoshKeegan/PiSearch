using System;
using System.Threading.Tasks;

namespace StringSearch.Api.Health
{
    public interface IHealthResource
    {
        string Name { get; }
        bool Critical { get; }
        Task<HealthState> CheckState();
    }
}