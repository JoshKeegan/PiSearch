using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Api.Infrastructure.Di;

namespace StringSearch.Api.Di
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApiDependencies(this IServiceCollection services)
        {
            services.RegisterInfrastructureDependencies();

            return services;
        }
    }
}
