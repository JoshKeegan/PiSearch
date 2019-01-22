using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api
{
    public interface IApiVersionProvider
    {
        string Get();
    }
}
