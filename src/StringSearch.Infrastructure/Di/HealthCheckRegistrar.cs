using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StringSearch.Health;
using StringSearch.Health.Factories;
using StringSearch.Infrastructure.Config;
using StringSearch.Infrastructure.Health;
using StringSearch.Infrastructure.Health.Factories;

namespace StringSearch.Infrastructure.Di
{
    internal static class HealthCheckRegistrar
    {
        internal static IServiceCollection AddHealthChecks(this IServiceCollection services)
        {
            services.addSharedHealthChecks();
            services.AddSingleton<IDigitsHealthChecksFactory, DigitsHealthChecksFactory>();

            return services;
        }

        private static IServiceCollection addSharedHealthChecks(this IServiceCollection services)
        {
            // Database connection (critical)
            services.AddSingleton<IHealthResource>(provider =>
            {
                ILogger log = provider.GetService<ILogger>();
                MySqlConfig config = provider.GetService<MySqlConfig>();
                return new MySqlHealthResource("Database", true, log, config.ConnectionString);
            });

            return services;
        }
    }
}
