using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch
{
    public interface IHealthCheckServices
    {
        Task<HealthServiceSummary> RunAll();
    }
}
