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
            // Digits file (critical)
            services.AddSingleton<IHealthResource>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                return new FileHealthResource("Absolute Digits File", true, config.AbsoluteDigitsPath);
            });

            // Suffix array file (critical)
            services.AddSingleton<IHealthResource>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                return new FileHealthResource("Suffix array file", true, config.AbsoluteSuffixArrayPath);
            });

            // Precomputed search results directory (critical if specified, but optional)
            //  Note: despite being optional, not having this can cause massive performance degradation
            //  when searching a large number of digits for a small string 
            services.AddSingleton<IHealthResource>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                return new OptionalDirectoryHealthResource("Precomputed search results directory", true,
                    config.AbsolutePrecomputedResultsDirPath);
            });
            
            // TODO: Health Resource: MySQL connection?
            
            return services;
        }
    }
}
