using System;
using StringSearch.Legacy.Collections;

namespace StringSearch.LegacyWrappers
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
