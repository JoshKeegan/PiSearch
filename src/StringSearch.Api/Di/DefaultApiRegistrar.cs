using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace StringSearch.Api.Di
{
    public static class DefaultApiRegistrar
    {
        /// <summary>
        /// Default wire up for the API
        /// </summary>
        public static IServiceCollection RegisterApiDependencies(this IServiceCollection services)
        {
            services
                .RegisterHealthChecks()
                .AddAutoMapper();

            return services;
        }
    }
}
