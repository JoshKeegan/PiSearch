using System.Threading.Tasks;

namespace StringSearch.Health
{
    public interface IHealthResource
    {
        string Name { get; }
        bool Critical { get; }
        Task<HealthState> CheckState();
    }
}