﻿using Serilog.Core;
using Serilog.Events;

namespace StringSearch.Infrastructure.Logging
{
    public class ApiVersionEnricher : ILogEventEnricher
    {
        public const string API_VERSION_PROPERTY_NAME = "Version";

        private readonly LogEventProperty property;

        public ApiVersionEnricher(string apiVersion)
        {
            property = apiVersion == null
                ? null
                : new LogEventProperty(API_VERSION_PROPERTY_NAME, new ScalarValue(apiVersion));
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (property != null)
            {
                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}
