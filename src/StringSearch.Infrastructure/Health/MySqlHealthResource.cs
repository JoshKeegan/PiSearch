﻿using System;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Serilog;
using StringSearch.Health;

namespace StringSearch.Infrastructure.Health
{
    public class MySqlHealthResource : IHealthResource
    {
        private readonly ILogger log;
        private readonly string connStr;

        public string Name { get; }
        public bool Critical { get; }

        public MySqlHealthResource(string name, bool critical, ILogger log, string connStr)
        {
            Name = name;
            Critical = critical;
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.connStr = connStr ?? throw new ArgumentNullException(nameof(connStr));
        }

        public async Task<HealthState> CheckState()
        {
            log.Debug("Performing MySQL health check for \"{name}\"", Name);

            try
            {
                using (DbConnection conn = new MySqlConnection(connStr))
                {
                    await conn.OpenAsync();

                    log.Debug("MySQL health check for \"{name}\" completed successfully", Name);

                    return new HealthState(true, null);
                }
            }
            catch (Exception e)
            {
                log.Error(e, "An error occurred during a MySQL health check for \"{name}\"", Name);

                return new HealthState(false, "Failed to connect to database");
            }
        }
    }
}
