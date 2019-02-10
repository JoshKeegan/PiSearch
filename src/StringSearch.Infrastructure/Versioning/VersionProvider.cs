using System;
using StringSearch.Versioning;

namespace StringSearch.Infrastructure.Versioning
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
