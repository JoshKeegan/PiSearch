using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
{
    public interface BigArray<T>
    {
        T this[long i] 
        {
            get;
            set;
        }

        long Length { get; }
    }
}
