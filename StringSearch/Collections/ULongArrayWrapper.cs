/*
 * ULongArrayWrapper - implementation of BigArray for ULong that is just a wrapper of the standard .NET ulong[]
 * By Josh Keegan 08/12/2014
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
{
    public class ULongArrayWrapper : BigArray<ulong>
    {
        //Private variables
        private ulong[] array;

        //Public variables
        public ulong this[long i]
        {
            get
            {
                validateIndex(i);

                return this.array[i];
            }
            set
            {
                validateIndex(i);

                this.array[i] = value;
            }
        }

        public long Length
        {
            get 
            {
                return this.array.Length;
            }
        }

        //Constructor
        public ULongArrayWrapper(long length)
        {
            if(length < 0 || length > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("0 <= length <= int.MaxValue");
            }

            this.array = new ulong[length];
        }

        //Public methods
        public IEnumerator<ulong> GetEnumerator()
        {
            for (long i = 0; i < this.Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
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
