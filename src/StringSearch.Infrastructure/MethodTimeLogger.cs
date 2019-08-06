using System;
using System.Reflection;

namespace StringSearch.Infrastructure
{
    /// <summary>
    /// For MethodTimer.Fody to log to
    /// </summary>
    public static class MethodTimeLogger
    {
        public static void Log(MethodBase method, TimeSpan elapsed, string message) =>
            Serilog.Log.Information("Method {method} completed in {elapsed}. Optional message: {message}", method,
                elapsed, message);
    }
}
