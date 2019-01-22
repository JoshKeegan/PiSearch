using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;

namespace StringSearch.Api.Infrastructure.Logging.Extensions
{
    public static class LoggerEnrichmentConfigurationExtensions
    {
        public static LoggerConfiguration WithApiVersion(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            IApiVersionProvider apiVersionProvider)
        {
            if (enrichmentConfiguration == null)
            {
                throw new ArgumentNullException(nameof(enrichmentConfiguration));
            }
            if (apiVersionProvider == null)
            {
                throw new ArgumentNullException(nameof(apiVersionProvider));
            }

            return enrichmentConfiguration.WithApiVersion(apiVersionProvider.Get());
        }

        public static LoggerConfiguration WithApiVersion(this LoggerEnrichmentConfiguration enrichmentConfiguration,
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
