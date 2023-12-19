using System.IO;
using StringSearch.Legacy.Collections;

namespace UnitTests.TestObjects.Extensions
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
