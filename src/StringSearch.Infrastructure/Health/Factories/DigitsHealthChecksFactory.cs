using System.Collections.Generic;
using StringSearch.Config;
using StringSearch.Health;
using StringSearch.Health.Factories;

namespace StringSearch.Infrastructure.Health.Factories
{
    public class DigitsHealthChecksFactory : IDigitsHealthChecksFactory
    {
        private readonly Dictionary<string, IHealthResource[]> digitsHealthChecks;

        public DigitsHealthChecksFactory(NamedDigitsConfig namedDigitsConfig)
        {
            digitsHealthChecks = new Dictionary<string, IHealthResource[]>();
            foreach (KeyValuePair<string, DigitsConfig> kvp in namedDigitsConfig)
            {
                string namedDigits = kvp.Key;
                DigitsConfig config = kvp.Value;

                IHealthResource[] healthChecks = new IHealthResource[]
                {
                    // Digits file (critical)
                    new FileHealthResource("Digits File", true, config.AbsoluteDigitsPath),

                    // Suffix array file (critical)
                    new FileHealthResource("Suffix array file", true, config.AbsoluteSuffixArrayPath),

                    // Precomputed search results directory (critical if specified, but optional)
                    //  Note: despite being optional, not having this can cause massive performance degradation
                    //  when searching a large number of digits for a small string 
                    new OptionalDirectoryHealthResource("Precomputed search results directory", true, 
                        config.AbsolutePrecomputedResultsDirPath)
                };

                digitsHealthChecks.Add(namedDigits, healthChecks);
            }
        }

        public IDictionary<string, IHealthResource[]> Create() => digitsHealthChecks;
    }
}
