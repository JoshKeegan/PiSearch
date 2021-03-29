using StringSearch.Legacy.Collections;

namespace StringSearch.Tests.Unit.TestObjects.Extensions
{
    public static class IntArrayToBigArrayUlongExtensions
    {
        public static IBigArray<ulong> ToBigULongArray(this int[] arr)
        {
            IBigArray<ulong> toRet = new ULongArrayWrapper(arr.Length);

            for (int i = 0; i < arr.Length; i++)
            {
                toRet[i] = (uint) arr[i];
            }

            return toRet;
        }
    }
}
