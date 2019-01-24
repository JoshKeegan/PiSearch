using System;
using System.Data;

namespace StringSearch.Infrastructure.DataLayer
{
    public static class DbCommandExtensions
    {
        public static IDbDataParameter AddParameter<T>(this IDbCommand command, string name, T value,
            DbType type = DbType.String)
        {
            if (value is DbType)
            {
                throw new ArgumentException("Received a DbType as a parameter value. Have you missed the value?",
                    nameof(value));
            }

            // Construct the parameter
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = type;

            // Add it to the command
            command.Parameters.Add(parameter);

            // Return it so that other properties can be modified (if required)
            return parameter;
        }
    }
}
