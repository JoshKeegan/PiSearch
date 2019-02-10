using System;
using System.Collections.Generic;
using System.Text;
using StringSearch.Models;
using StringSearch.Services;

namespace StringSearch.Validators
{
    public class OccurrenceLookupRequestValidator : IOccurrenceLookupRequestValidator
    {
        private readonly IDigitsServices digitsServices;

        public OccurrenceLookupRequestValidator(IDigitsServices digitsServices)
        {
            this.digitsServices = digitsServices;
        }

        public void ThrowIfInvalid(OccurrenceLookupRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.NamedDigits == null)
            {
                throw new ArgumentException(nameof(request.NamedDigits) + " must be specified");
            }

            if (request.FindLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.FindLength) + " must be > 0");
            }

            if (request.MinSuffixArrayIdx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.MinSuffixArrayIdx) + " must be >= 0");
            }
            if (request.MaxSuffixArrayIdx < request.MinSuffixArrayIdx)
            {
                throw new ArgumentOutOfRangeException(nameof(request.MaxSuffixArrayIdx) + " must be >= " +
                                                      nameof(request.MinSuffixArrayIdx));
            }

            long numResults = request.MaxSuffixArrayIdx - request.MinSuffixArrayIdx + 1;
            if (request.ResultId > numResults)
            {
                throw new ArgumentOutOfRangeException(nameof(request.ResultId),
                    "The specified suffix array range has fewer results than the requested result ID");
            }

            long numDigits = digitsServices.CountDigits(request.NamedDigits);
            if (request.MaxSuffixArrayIdx >= numDigits)
            {
                throw new InvalidOperationException(String.Format(
                    "There are {0} digits loaded for named digits {1}. The requested suffix array range exceeds the bounds",
                    numDigits, request.NamedDigits));
            }
        }
    }
}
