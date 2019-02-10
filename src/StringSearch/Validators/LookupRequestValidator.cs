using System;
using System.Collections.Generic;
using System.Text;
using StringSearch.Models;
using StringSearch.Services;

namespace StringSearch.Validators
{
    public class LookupRequestValidator : ILookupRequestValidator
    {
        private readonly IFindValidator findValidator;
        private readonly IDigitsServices digitsServices;

        public LookupRequestValidator(IFindValidator findValidator, IDigitsServices digitsServices)
        {
            this.findValidator = findValidator;
            this.digitsServices = digitsServices;
        }

        public void ThrowIfInvalid(LookupRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.NamedDigits == null)
            {
                throw new ArgumentException(nameof(request.NamedDigits) + " must be specified");
            }

            findValidator.ThrowIfInvalid(request.Find);

            if (request.ResultId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.ResultId), "Must be >= 0");
            }

            if (request.NumSurroundingDigits < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.NumSurroundingDigits), "Must be >= 0");
            }

            if ((request.MaxSuffixArrayIdx != null && request.MinSuffixArrayIdx == null) ||
                (request.MaxSuffixArrayIdx == null && request.MinSuffixArrayIdx != null))
            {
                throw new ArgumentException("Min and max suffix array indices must be supplied as a pair",
                    nameof(request));
            }

            if (request.MinSuffixArrayIdx != null)
            {
                if (request.MinSuffixArrayIdx.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.MinSuffixArrayIdx), "Must be >= 0");
                }

                // ReSharper disable once PossibleInvalidOperationException
                if (request.MaxSuffixArrayIdx.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request.MaxSuffixArrayIdx), "Must be >= 0");
                }

                if (request.MaxSuffixArrayIdx.Value < request.MinSuffixArrayIdx.Value)
                {
                    throw new ArgumentException(nameof(request.MinSuffixArrayIdx) + " must be <= " +
                                                nameof(request.MaxSuffixArrayIdx), nameof(request));
                }

                long numResults = request.MaxSuffixArrayIdx.Value - request.MinSuffixArrayIdx.Value + 1;
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
}
