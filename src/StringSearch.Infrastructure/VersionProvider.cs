using System;

namespace StringSearch.Infrastructure
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
