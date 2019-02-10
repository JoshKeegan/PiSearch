using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.SearchRunners;

namespace StringSearch.Di
{
    internal static class SearchRunnersRegistrar
    {
        public static IServiceCollection AddSearchRunners(this IServiceCollection services) =>
            services
                .AddSingleton<IDigitsSearchRunner, DigitsSearchRunner>()
                .AddSingleton<IOccurrenceLookupRunner, OccurrenceLookupRunner>();
    }
}
