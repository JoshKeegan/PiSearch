using Microsoft.Extensions.DependencyInjection;
using StringSearch.Infrastructure.NamedDigits.Factories;
using StringSearch.NamedDigits.Factories;

namespace StringSearch.Infrastructure.Di
{
    internal static class NamedDigitsRegistrar
    {
        public static IServiceCollection AddNamedDigits(this IServiceCollection services) =>
            services
                .AddSingleton<IDigitsFactory, DigitsFactory>()
                .AddSingleton<ISuffixArrayFactory, SuffixArrayFactory>()
                .AddSingleton<IPrecomputedSearchResultsFilePathsFactory, PrecomputedSearchResultsFilePathsFactory>()
                .AddSingleton<IPrecomputedSearchResultsFactory, PrecomputedSearchResultsFactory>();
    }
}
