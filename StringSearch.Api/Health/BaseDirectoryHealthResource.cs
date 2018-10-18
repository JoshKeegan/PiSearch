using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Health
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
