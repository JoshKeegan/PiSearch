using StringSearch.Models;

namespace StringSearch.Services
{
    public interface IStringSearchServices
    {
        SearchResult Count(string namedDigits, string find);
        LookupResult Lookup(LookupRequest request);
    }
}
