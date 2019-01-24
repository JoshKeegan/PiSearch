using System.Threading.Tasks;

namespace StringSearch.DataLayer
{
    public interface IDbSearches
    {
        Task Insert(SearchSummary search);
    }
}
