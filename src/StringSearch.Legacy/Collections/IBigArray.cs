/*
 * PiSearch
 * BigArray (generic) Interface - should be implemented by any generic array that can contain more than int.MaxValue values
 * By Josh Keegan 25/11/2014
 */

using System.Collections.Generic;

namespace StringSearch.Legacy.Collections
{
    public interface IBigArray<T> : IEnumerable<T>
    {
        T this[long i] 
        {
            get;
            set;
        }

        long Length { get; }
    }
}
