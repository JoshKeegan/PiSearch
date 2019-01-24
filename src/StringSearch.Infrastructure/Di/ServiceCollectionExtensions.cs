using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Hosting;
using StringSearch.Api;
using StringSearch.Api.Infrastructure;
using StringSearch.DataLayer;
using StringSearch.Infrastructure.Config;
using StringSearch.Infrastructure.DataLayer;
using StringSearch.Infrastructure.Logging.Extensions;
using StringSearch.Infrastructure.StringSearch;
using StringSearch.Legacy.Collections;
using StringSearch.LegacyWrappers;
using ILogger = Serilog.ILogger;

namespace StringSearch.Infrastructure.Di
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructureDependencies(this IServiceCollection services)
        {
            services
                .registerConfig()
                .registerLogging()
                .registerDb()
                .registerStringSearch()
                .registerApiVersion()
                .RegisterHealthChecks();

            return services;
        }

        private static IServiceCollection registerConfig(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                IConfiguration config = provider.GetService<IConfiguration>();
                MySqlConfig mySqlConfig = config.GetSection("MySql").Get<MySqlConfig>();
                return mySqlConfig;
            });

            services.AddSingleton(provider =>
            {
                IConfiguration config = provider.GetService<IConfiguration>();
                StringSearchConfig stringSearchConfig = config.GetSection("StringSearch").Get<StringSearchConfig>();
                return stringSearchConfig;
            });

            return services;
        }

        private static IServiceCollection registerLogging(this IServiceCollection services)
        {
            services.AddSingleton<LoggerConfiguration>(provider => new LoggerConfiguration()
                .ReadFrom.Configuration(provider.GetService<IConfiguration>())
                .Enrich.FromLogContext()
                .Enrich.WithVersion(provider.GetService<IVersionProvider>()));

            services.AddSingleton<ILogger>(provider => provider.GetService<LoggerConfiguration>().CreateLogger());
            services.AddSingleton<ILoggerFactory>(provider => new SerilogLoggerFactory(provider.GetService<ILogger>()));

            return services;
        }

        private static IServiceCollection registerDb(this IServiceCollection services)
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

        private static IServiceCollection registerStringSearch(this IServiceCollection services)
        {
            services.AddScoped<DigitsStreamWrapper>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                Stream stream = new FileStream(config.AbsoluteDigitsPath, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                return new DigitsStreamWrapper(stream);
            });

            services.AddScoped<DigitsWrapper>(provider =>
            {
                DigitsStreamWrapper digitsStreamWrapper = provider.GetService<DigitsStreamWrapper>();
                IBigArray<byte> digits = new FourBitDigitBigArray(digitsStreamWrapper.Stream);
                return new DigitsWrapper(digits);
            });

            services.AddScoped<SuffixArrayStreamWrapper>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                Stream stream = new FileStream(config.AbsoluteSuffixArrayPath, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                return new SuffixArrayStreamWrapper(stream);
            });

            services.AddScoped<SuffixArrayWrapper>(provider =>
            {
                SuffixArrayStreamWrapper suffixArrayStreamWrapper = provider.GetService<SuffixArrayStreamWrapper>();
                DigitsWrapper digitsWrapper = provider.GetService<DigitsWrapper>();
                IBigArray<ulong> suffixArray =
                    new MemoryEfficientBigULongArray(digitsWrapper.Digits.Length, (ulong) digitsWrapper.Digits.Length,
                        suffixArrayStreamWrapper.Stream);
                return new SuffixArrayWrapper(suffixArray);
            });

            services.AddSingleton<PrecomputedSearchResultsFilePaths>(provider =>
            {
                StringSearchConfig config = provider.GetService<StringSearchConfig>();
                string[] filePaths = config.AbsolutePrecomputedResultsDirPath != null
                    ? Directory.GetFiles(config.AbsolutePrecomputedResultsDirPath,
                        "*." + config.PrecomputedResultsFileExtension)
                    : new string[0];
                return new PrecomputedSearchResultsFilePaths(filePaths);
            });

            services.AddScoped<IPrecomputedSearchResults, PrecomputedSearchResults>();

            return services;
        }

        private static IServiceCollection registerApiVersion(this IServiceCollection services)
        {
            services.AddSingleton<IVersionProvider, VersionProvider>();

            return services;
        }
    }
}
