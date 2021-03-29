using System.IO;
using StringSearch.Legacy.Collections;

namespace StringSearch.Tests.Unit.TestObjects.Extensions
{
    public static class StringToFourBitDigitArrayExtensions
    {
        public static FourBitDigitBigArray ToFourBitDigitBigArray(this string str)
        {
            Stream s = str.ToFourBitDigitStream();
            return new FourBitDigitBigArray(s);
        }
    }
}
