using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Services;

namespace StringSearch.Di
{
    internal static class ServicesRegistrar
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services
                .AddSingleton<IStringSearchServices, StringSearchServices>()
                .AddSingleton<IDigitsServices, DigitsServices>()
                .AddSingleton<IHealthCheckServices, HealthCheckServices>();
    }
}
