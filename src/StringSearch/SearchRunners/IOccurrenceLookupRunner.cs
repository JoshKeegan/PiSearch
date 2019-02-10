using StringSearch.Models;

namespace StringSearch.SearchRunners
{
    public interface IOccurrenceLookupRunner
    {
        OccurrenceLookupResult Lookup(OccurrenceLookupRequest request);
    }
}
