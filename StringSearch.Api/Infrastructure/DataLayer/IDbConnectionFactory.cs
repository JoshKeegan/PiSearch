using System.Data;
using System.Data.Common;

namespace StringSearch.Api.Infrastructure.DataLayer
{
    internal interface IDbConnectionFactory
    {
        DbConnection GetConnection();
    }
}
