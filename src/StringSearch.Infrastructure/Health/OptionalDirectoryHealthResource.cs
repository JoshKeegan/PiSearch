using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch.Infrastructure.Health
{
    public class OptionalDirectoryHealthResource : BaseDirectoryHealthResource
    {
        public OptionalDirectoryHealthResource(string name, bool critical, string path) 
            : base(name, critical, path) {  }

        public override async Task<HealthState> CheckState()
        {
            if (path == null)
            {
                return new HealthState(true, "Optional directory path not specified");
            }

            return await base.CheckState();
        }
    }
}
