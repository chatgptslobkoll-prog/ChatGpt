using System;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.SqlClient;

namespace Amonic.App.Data
{
    public static class Db
    {
        private static readonly string[] ConnectionNames =
        {
            "SessionDb",
            "Session3_01Entities"
        };

        public static SqlConnection OpenConnection()
        {
            var rawConnectionString = ResolveConnectionString();
            var sqlConnectionString = ToSqlConnectionString(rawConnectionString);

            var connection = new SqlConnection(sqlConnectionString);
            connection.Open();
            return connection;
        }

        private static string ResolveConnectionString()
        {
            foreach (var name in ConnectionNames)
            {
                var settings = ConfigurationManager.ConnectionStrings[name];
                if (settings != null && !string.IsNullOrWhiteSpace(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }
            }

            throw new ConfigurationErrorsException(
                "Connection string not found. Add 'SessionDb' (SQL) or 'Session3_01Entities' (EF) to <connectionStrings> in App.config.");
        }

        private static string ToSqlConnectionString(string connectionString)
        {
            if (connectionString.IndexOf("metadata=", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var entityBuilder = new EntityConnectionStringBuilder(connectionString);
                return entityBuilder.ProviderConnectionString;
            }

            return connectionString;
        }
    }
}
