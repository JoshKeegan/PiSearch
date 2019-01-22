using System.Threading.Tasks;
using StringSearch.Api.Search;

namespace StringSearch.Api.Infrastructure.DataLayer
{
    public interface IDbSearches
    {
        Task Insert(SearchSummary search);
    }
}
