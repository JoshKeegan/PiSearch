using System;
using System.IO;
using StringSearch.Config;
using StringSearch.Legacy.Collections;
using StringSearch.NamedDigits;
using StringSearch.NamedDigits.Factories;
using StringSearch.Services;

namespace StringSearch.Infrastructure.NamedDigits.Factories
{
    public class SuffixArrayFactory : ISuffixArrayFactory
    {
        private readonly NamedDigitsConfig namedDigitsConfig;
        private readonly IDigitsServices digitsServices;

        public SuffixArrayFactory(
            NamedDigitsConfig namedDigitsConfig,
            IDigitsServices digitsServices)
        {
            this.namedDigitsConfig = namedDigitsConfig;
            this.digitsServices = digitsServices;
        }

        public ObjectWithStream<IBigArray<ulong>> Create(string namedDigits)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }

            if (namedDigitsConfig.TryGetValue(namedDigits, out DigitsConfig config))
            {
                long numDigits = digitsServices.CountDigits(namedDigits);

                Stream stream = new FileStream(config.AbsoluteSuffixArrayPath, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                IBigArray<ulong> suffixArray = new MemoryEfficientBigULongArray(numDigits, (ulong) numDigits, stream);
                return new ObjectWithStream<IBigArray<ulong>>(suffixArray, stream);
            }

            throw new ArgumentException("No configuration for the specified named digits", nameof(namedDigits));
        }
    }
}
