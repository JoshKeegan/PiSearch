using System;
using Serilog;
using Serilog.Configuration;
using StringSearch.Api;
using StringSearch.Api.Infrastructure.Logging;

namespace StringSearch.Infrastructure.Logging.Extensions
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithVersion(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IVersionProvider apiVersionProvider)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            if (apiVersionProvider == null)
            {
                throw new ArgumentNullException(nameof(apiVersionProvider));
            }

            return enrichmentConfiguration.WithVersion(apiVersionProvider.Get());
        }

        public static LoggerConfiguration WithVersion(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string apiVersion)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }

            return enrichmentConfiguration.With(new ApiVersionEnricher(apiVersion));
        }
    }
}
