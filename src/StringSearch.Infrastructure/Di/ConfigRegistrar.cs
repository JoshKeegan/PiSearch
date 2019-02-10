using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Infrastructure.Config;

namespace StringSearch.Infrastructure.Di
{
    internal static class ConfigRegistrar
    {
        public static IServiceCollection AddConfig(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                IConfiguration config = provider.GetService<IConfiguration>();
                MySqlConfig mySqlConfig = config.GetSection("MySql").Get<MySqlConfig>();
                return mySqlConfig;
            });

            return services;
        }
    }
}
