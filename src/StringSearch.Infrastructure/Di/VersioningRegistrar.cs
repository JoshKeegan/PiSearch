using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Infrastructure.Versioning;
using StringSearch.Versioning;

namespace StringSearch.Infrastructure.Di
{
    internal static class VersioningRegistrar
    {
        public static IServiceCollection AddApiVersioning(this IServiceCollection services)
        {
            services.AddSingleton<IVersionProvider, VersionProvider>();

            return services;
        }
    }
}
