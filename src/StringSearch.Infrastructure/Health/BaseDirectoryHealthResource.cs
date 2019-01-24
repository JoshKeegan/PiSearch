using System.IO;
using System.Threading.Tasks;
using StringSearch.Health;

namespace StringSearch.Infrastructure.Health
{
    public abstract class BaseDirectoryHealthResource : IHealthResource
    {
        protected readonly string path;

        public string Name { get; }
        public bool Critical { get; }

        protected BaseDirectoryHealthResource(string name, bool critical, string path)
        {
            Name = name;
            Critical = critical;
            this.path = path;
        }

#pragma warning disable 1998
        public virtual async Task<HealthState> CheckState() => Directory.Exists(path)
#pragma warning restore 1998
            ? new HealthState(true, "Directory exists")
            : new HealthState(false, "Directory does not exist");
    }
}
