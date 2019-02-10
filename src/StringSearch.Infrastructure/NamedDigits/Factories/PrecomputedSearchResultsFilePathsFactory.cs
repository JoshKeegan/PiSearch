using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using StringSearch.Config;
using StringSearch.NamedDigits.Factories;

namespace StringSearch.Infrastructure.NamedDigits.Factories
{
    public class PrecomputedSearchResultsFilePathsFactory : IPrecomputedSearchResultsFilePathsFactory
    {
        private readonly NamedDigitsConfig namedDigitsConfig;
        private readonly ConcurrentDictionary<string, string[]> cache =
            new ConcurrentDictionary<string, string[]>();

        public PrecomputedSearchResultsFilePathsFactory(NamedDigitsConfig namedDigitsConfig)
        {
            this.namedDigitsConfig = namedDigitsConfig;
        }

        public string[] Create(string namedDigits)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }

            // Check cache
            if (cache.TryGetValue(namedDigits, out string[] filePaths))
            {
                return filePaths;
            }

            // Otherwise, cache miss...
            if (namedDigitsConfig.TryGetValue(namedDigits, out DigitsConfig config))
            {
                filePaths = config.AbsolutePrecomputedResultsDirPath != null
                    ? Directory.GetFiles(config.AbsolutePrecomputedResultsDirPath,
                        "*." + config.PrecomputedResultsFileExtension)
                    : new string[0];

                // ReSharper disable once AssignNullToNotNullAttribute
                filePaths = filePaths.OrderBy(p => int.Parse(Path.GetFileNameWithoutExtension(p))).ToArray();

                // Store in cache
                cache[namedDigits] = filePaths;

                return filePaths;
            }

            throw new ArgumentException("No configuration for the specified named digits", nameof(namedDigits));
        }
    }
}
