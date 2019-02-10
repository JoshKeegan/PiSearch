using System;
using System.Text;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;
using StringSearch.Models;
using StringSearch.NamedDigits;
using StringSearch.NamedDigits.Factories;
using StringSearch.Validators;

namespace StringSearch.SearchRunners
{
    public class OccurrenceLookupRunner : IOccurrenceLookupRunner
    {
        private readonly IDigitsFactory digitsFactory;
        private readonly ISuffixArrayFactory suffixArrayFactory;
        private readonly IOccurrenceLookupRequestValidator occurrenceLookupRequestValidator;

        public OccurrenceLookupRunner(IDigitsFactory digitsFactory,
            ISuffixArrayFactory suffixArrayFactory, 
            IOccurrenceLookupRequestValidator occurrenceLookupRequestValidator)
        {
            this.digitsFactory = digitsFactory;
            this.suffixArrayFactory = suffixArrayFactory;
            this.occurrenceLookupRequestValidator = occurrenceLookupRequestValidator;
        }

        public OccurrenceLookupResult Lookup(OccurrenceLookupRequest request)
        {
            occurrenceLookupRequestValidator.ThrowIfInvalid(request);

            using (ObjectWithStream<IBigArray<ulong>> suffixArray = suffixArrayFactory.Create(request.NamedDigits))
            using (ObjectWithStream<IBigArray<byte>> digits = digitsFactory.Create(request.NamedDigits))
            {
                // TODO: Remove dependency on legacy code
                SuffixArrayRange suffixArrayRange = new SuffixArrayRange(request.MinSuffixArrayIdx,
                    request.MaxSuffixArrayIdx, suffixArray.Object, digits.Object);

                long resultIdx = suffixArrayRange.SortedValues[request.ResultId];
                SurroundingDigits surroundingDigits = null;

                if (request.NumSurroundingDigits > 0)
                {
                    StringBuilder beforeBuilder = new StringBuilder();
                    StringBuilder afterBuilder = new StringBuilder();

                    long beforeStartIdx = Math.Max(0, resultIdx - request.NumSurroundingDigits);
                    for (long i = beforeStartIdx; i < resultIdx; i++)
                    {
                        beforeBuilder.Append(digits.Object[i]);
                    }

                    long afterStartIdx = Math.Min(digits.Object.Length, resultIdx + request.FindLength);
                    long afterEndIdx = Math.Min(digits.Object.Length,
                        afterStartIdx + request.NumSurroundingDigits);
                    for (long i = afterStartIdx; i < afterEndIdx; i++)
                    {
                        afterBuilder.Append(digits.Object[i]);
                    }

                    surroundingDigits = new SurroundingDigits(beforeBuilder.ToString(), afterBuilder.ToString());
                }

                return new OccurrenceLookupResult(resultIdx, surroundingDigits);
            }
        }
    }
}
