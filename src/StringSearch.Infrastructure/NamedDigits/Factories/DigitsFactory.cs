using System;
using System.IO;
using StringSearch.Config;
using StringSearch.Legacy.Collections;
using StringSearch.NamedDigits;
using StringSearch.NamedDigits.Factories;

namespace StringSearch.Infrastructure.NamedDigits.Factories
{
    public class DigitsFactory : IDigitsFactory
    {
        private readonly NamedDigitsConfig namedDigitsConfig;

        public DigitsFactory(NamedDigitsConfig namedDigitsConfig)
        {
            this.namedDigitsConfig = namedDigitsConfig;
        }

        public ObjectWithStream<IBigArray<byte>> Create(string namedDigits)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }

            if (namedDigitsConfig.TryGetValue(namedDigits, out DigitsConfig config))
            {
                Stream stream = new FileStream(config.AbsoluteDigitsPath, FileMode.Open, FileAccess.Read,
                    FileShare.Read);
                IBigArray<byte> digits = new FourBitDigitBigArray(stream);

                return new ObjectWithStream<IBigArray<byte>>(digits, stream);
            }

            throw new ArgumentException("No configuration for the specified named digits", nameof(namedDigits));
        }
    }
}
