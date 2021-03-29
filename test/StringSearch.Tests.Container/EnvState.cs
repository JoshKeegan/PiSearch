using System;

namespace StringSearch.Tests.Container
{
    public static class EnvState
    {
        public static readonly string ApiBaseUri =
            Environment.GetEnvironmentVariable("API_BASE_URI") ?? "http://localhost:5000";
    }
}
