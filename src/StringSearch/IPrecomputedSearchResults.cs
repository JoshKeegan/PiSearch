using System;
using StringSearch.Legacy;
using StringSearch.Legacy.Collections;

namespace StringSearch
{
    public interface IPrecomputedSearchResults : IDisposable
    {
        IBigArray<PrecomputedSearchResult>[] Results { get; }
    }
}
