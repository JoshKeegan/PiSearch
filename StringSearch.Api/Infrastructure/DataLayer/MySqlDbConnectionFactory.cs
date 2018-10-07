using System;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace StringSearch.Api.Infrastructure.DataLayer
{
    internal class MySqlDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string connStr;

        public MySqlDbConnectionFactory(string connStr)
        {
            this.connStr = connStr ?? throw new ArgumentNullException(nameof(connStr));
        }

        public DbConnection GetConnection()
        {
            return new MySqlConnection(connStr);
        }
    }
}
