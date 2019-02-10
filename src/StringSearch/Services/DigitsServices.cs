using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using StringSearch.Legacy.Collections;
using StringSearch.NamedDigits;
using StringSearch.NamedDigits.Factories;

namespace StringSearch.Services
{
    public class DigitsServices : IDigitsServices
    {
        private readonly ConcurrentDictionary<string, long> cache = new ConcurrentDictionary<string, long>();
        private readonly IDigitsFactory digitsFactory;

        public DigitsServices(IDigitsFactory digitsFactory)
        {
            this.digitsFactory = digitsFactory;
        }

        public long CountDigits(string namedDigits)
        {
            // Validation
            if (namedDigits == null)
            {
                throw new ArgumentNullException(nameof(namedDigits));
            }

            // Check cache
            if (cache.TryGetValue(namedDigits, out long numDigits))
            {
                return numDigits;
            }

            // Otherwise, cache miss...
            using (ObjectWithStream<IBigArray<byte>> digits = digitsFactory.Create(namedDigits))
            {
                // Store in cache
                cache[namedDigits] = digits.Object.Length;

                return digits.Object.Length;
            }
        }
    }
}
