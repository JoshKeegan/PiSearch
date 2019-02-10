using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.DataLayer;
using StringSearch.Infrastructure.Config;
using StringSearch.Infrastructure.DataLayer;

namespace StringSearch.Infrastructure.Di
{
    internal static class DbRegistrar
    {
        public static IServiceCollection AddDb(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory>(provider =>
            {
                MySqlConfig config = provider.GetService<MySqlConfig>();
                MySqlDbConnectionFactory connFact = new MySqlDbConnectionFactory(config.ConnectionString);
                return connFact;
            });

            services.AddSingleton<IDbSearches, DbSearches>();

            return services;
        }
    }
}
