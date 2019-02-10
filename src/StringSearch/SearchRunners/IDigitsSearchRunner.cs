using StringSearch.Models;

namespace StringSearch.SearchRunners
{
    public interface IDigitsSearchRunner
    {
        SearchResult Search(string namedDigits, string find);
    }
}
