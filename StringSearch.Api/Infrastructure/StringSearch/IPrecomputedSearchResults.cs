using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Collections;

namespace StringSearch.Api.Infrastructure.StringSearch
{
    public interface IPrecomputedSearchResults
    {
        IBigArray<PrecomputedSearchResult>[] Results { get; }
    }
}
