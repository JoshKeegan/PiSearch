using System;
using System.Collections.Generic;
using System.Text;
using StringSearch.Collections;

namespace UnitTests.TestObjects.Extensions
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
