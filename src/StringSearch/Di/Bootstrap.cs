using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace StringSearch.Di
{
    public static class Bootstrap
    {
        public static IServiceCollection AddStringSearch(this IServiceCollection services) =>
            services
                .AddConfig()
                .AddSearchRunners()
                .AddServices()
                .AddValidators();
    }
}
