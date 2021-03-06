﻿using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MethodTimer;
using StringSearch.DataLayer;

namespace StringSearch.Infrastructure.DataLayer
{
    public class DbSearches : IDbSearches
    {
        private const DbType ID_TYPE = DbType.Int64;
        private const DbType FIND_TYPE = DbType.String;
        private const DbType MIN_SUFFIX_ARRAY_IDX_TYPE = DbType.Int64;
        private const DbType MAX_SUFFIX_ARRAY_IDX_TYPE = DbType.Int64;
        private const DbType RESULT_ID_TYPE = DbType.Int32;
        private const DbType JUST_COUNT_TYPE = DbType.Boolean;
        private const DbType CLIENT_IP_TYPE = DbType.String;
        private const DbType SEARCH_DATE_TYPE = DbType.DateTime;
        private const DbType PROCESSING_TIME_MS_TYPE = DbType.Int64;
        private const DbType NUM_SURROUNDING_DIGITS_TYPE = DbType.Int32;
        private const DbType NAMED_DIGITS_TYPE = DbType.String;

        private readonly IDbConnectionFactory dbConnFact;

        public DbSearches(IDbConnectionFactory dbConnFact)
        {
            this.dbConnFact = dbConnFact;
        }

        [Time]
        public async Task Insert(SearchSummary search)
        {
            using (DbConnection conn = dbConnFact.GetConnection())
            using (DbCommand command = conn.CreateCommand())
            {
                await conn.OpenAsync();

                // Build the command
                command.CommandText = 
@"INSERT INTO searches (find, minSuffixArrayIdx, maxSuffixArrayIdx, resultId, justCount, clientIp, searchDate, 
    processingTimeMs, numSurroundingDigits, namedDigits)
VALUES (@find, @minSuffixArrayIdx, @maxSuffixArrayIdx, @resultId, @justCount, @clientIp, @searchDate, 
    @processingTimeMs, @numSurroundingDigits, @namedDigits)";

                // Add the parameters
                command.AddParameter("@find", search.Find, FIND_TYPE);
                command.AddParameter("@minSuffixArrayIdx", search.MinSuffixArrayIdx, MIN_SUFFIX_ARRAY_IDX_TYPE);
                command.AddParameter("@maxSuffixArrayIdx", search.MaxSuffixArrayIdx, MAX_SUFFIX_ARRAY_IDX_TYPE);
                command.AddParameter("@resultId", search.ResultId, RESULT_ID_TYPE);
                command.AddParameter("@justCount", search.JustCount, JUST_COUNT_TYPE);
                command.AddParameter("@clientIp", search.ClientIp, CLIENT_IP_TYPE);
                command.AddParameter("@searchDate", search.SearchDate, SEARCH_DATE_TYPE);
                command.AddParameter("@processingTimeMs", search.ProcessingTimeMs, PROCESSING_TIME_MS_TYPE);
                command.AddParameter("@numSurroundingDigits", search.NumSurroundingDigits, NUM_SURROUNDING_DIGITS_TYPE);
                command.AddParameter("@namedDigits", search.NamedDigits, NAMED_DIGITS_TYPE);

                // Run the command
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
