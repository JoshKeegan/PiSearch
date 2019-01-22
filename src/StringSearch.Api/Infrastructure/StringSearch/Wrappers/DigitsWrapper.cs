using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Collections;

namespace StringSearch.Api.Infrastructure.StringSearch.Wrappers
{
    public class DigitsWrapper
    {
        public readonly IBigArray<byte> Digits;

        public DigitsWrapper(IBigArray<byte> digits)
        {
            Digits = digits ?? throw new ArgumentNullException(nameof(digits));
        }
    }
}
