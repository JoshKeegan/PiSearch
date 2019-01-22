using System.Data;
using System.Data.Common;

namespace StringSearch.Api.Infrastructure.DataLayer
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }
}
