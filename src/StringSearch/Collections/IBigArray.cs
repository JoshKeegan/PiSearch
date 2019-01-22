/*
 * PiSearch
 * BigArray (generic) Interface - should be implemented by any generic array that can contain more than int.MaxValue values
 * By Josh Keegan 25/11/2014
 * Last Edit 08/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
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
