using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Config;

namespace StringSearch.Di
{
    internal static class ConfigRegistrar
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                IConfiguration config = provider.GetService<IConfiguration>();
                return config.GetSection("StringSearch:NamedDigits").Get<NamedDigitsConfig>();
            });

            return services;
        }
    }
}
