using System.Data.Common;

namespace StringSearch.Infrastructure.DataLayer
{
    public interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }
}
