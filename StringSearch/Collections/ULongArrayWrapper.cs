/*
 * PiSearch
 * ULongArrayWrapper - implementation of BigArray for ULong that is just a wrapper of the standard .NET ulong[]
 * By Josh Keegan 08/12/2014
 * Last Edit 08/06/2016
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
{
    public class ULongArrayWrapper : IBigArray<ulong>
    {
        //Private variables
        private ulong[] array;

        //Public variables
        public ulong this[long i]
        {
            get
            {
                validateIndex(i);

                return array[i];
            }
            set
            {
                validateIndex(i);

                array[i] = value;
            }
        }

        public long Length => array.Length;

        //Constructor
        public ULongArrayWrapper(long length)
        {
            if(length < 0 || length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("0 <= length <= int.MaxValue");
            }

            array = new ulong[length];
        }

        //Public methods
        public IEnumerator<ulong> GetEnumerator()
        {
            for (long i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //Private helpers
        private static void validateIndex(long i)
        {
            if(i < 0 || i >= int.MaxValue)
            {
                throw new IndexOutOfRangeException("0 <= i < int.MaxValue");
            }
        }
    }
}
