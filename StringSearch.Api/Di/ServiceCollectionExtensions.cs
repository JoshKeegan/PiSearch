using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Api.Health;
using StringSearch.Api.Infrastructure.Config;
using StringSearch.Api.Infrastructure.Di;

namespace StringSearch.Api.Di
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApiDependencies(this IServiceCollection services)
        {
            services
                .RegisterInfrastructureDependencies()
                .registerHealthDependencies();

            return services;
        }

        private static IServiceCollection registerHealthDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IHealthResource>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                return new FileHealthResource(config.AbsoluteDigitsPath, true);
            });

            services.AddSingleton<IHealthResource>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                return new FileHealthResource(config.AbsoluteSuffixArrayPath, true);
            });
            
            // TODO: Health Resource: Directory for precomputed digits?
            
            // TODO: Health Resource: MySQL connection?
            
            return services;
        }
    }
}
