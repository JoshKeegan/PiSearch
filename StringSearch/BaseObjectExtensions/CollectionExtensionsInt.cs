/*
 * CollectionExtensionsInt - Extensions Methods for ICollection<int>
 * By Josh Keegan 08/12/2014
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.BaseObjectExtensions
{
    public static class CollectionExtensionsInt
    {
        public static long[] ToLongArr(this ICollection<int> collection)
        {
            long[] toRet = new long[collection.Count];

            int i = 0; 
            foreach(int num in collection)
            {
                toRet[i] = num;

                i++;
            }
            return toRet;
        }
    }
}
