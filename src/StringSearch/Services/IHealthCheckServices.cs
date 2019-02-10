using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch.Services
{
    public interface IHealthCheckServices
    {
        Task<HealthServiceSummary> RunAll();
    }
}
