using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StringSearch.Config;
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

            // Logging database (non-critical)
            services.AddSingleton<IHealthResource>(provider =>
            {
                // Get the connection string of the Serilog MySQL sink
                // Note: this assumes that there won't be more than one MySQL sink
                IConfiguration config = provider.GetService<IConfiguration>();
                string connStr = config.GetSection("Serilog")?.GetSection("WriteTo")?.GetChildren()
                    ?.Where(s => s.GetValue<string>("Name") == "MySQL")?.FirstOrDefault()?.GetSection("Args")
                    ?.GetValue<string>("connectionString");

                if (connStr == null)
                {
                    // Note: null will still get registered as an IHealthResource.
                    //  It must be filtered out whenever they are injected.
                    return null;
                }

                ILogger log = provider.GetService<ILogger>();
                return new MySqlHealthResource("Logs Database", false, log, connStr);
            });

            return services;
        }
    }
}
