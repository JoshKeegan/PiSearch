using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Infrastructure
{
    public class VersionProvider : IVersionProvider
    {
        public string Get()
        {
            string version = Environment.GetEnvironmentVariable("UNIQUEIFIER")?.Trim();
            return string.IsNullOrEmpty(version) ? null : version;
        }
    }
}
