using Microsoft.Extensions.DependencyInjection;

namespace StringSearch.Infrastructure.Di
{
    public static class Bootstrap
    {
        public static IServiceCollection RegisterInfrastructureDependencies(this IServiceCollection services)
        {
            services
                .AddConfig()
                .AddLogging()
                .AddDb()
                .AddApiVersioning()
                .AddHealthChecks()
                .AddNamedDigits();

            return services;
        }
    }
}
