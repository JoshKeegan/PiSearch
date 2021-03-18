using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using StringSearch.Infrastructure.Logging.Extensions;
using StringSearch.Versioning;
using ILogger = Serilog.ILogger;

namespace StringSearch.Infrastructure.Di
{
    internal static class LoggingRegistrar
    {
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddSingleton<LoggerConfiguration>(provider => new LoggerConfiguration()
                .ReadFrom.Configuration(provider.GetService<IConfiguration>())
                .Enrich.FromLogContext()
                .Enrich.WithVersion(provider.GetService<IVersionProvider>()));

            services.AddSingleton<ILogger>(provider =>
            {
                // Also set static logger so that it can be accessed from outside of DI
                Log.Logger = provider.GetService<LoggerConfiguration>().CreateLogger();
                return Log.Logger;
            });
            services.AddSingleton<ILoggerFactory>(provider => new SerilogLoggerFactory(provider.GetService<ILogger>()));

            return services;
        }
    }
}
