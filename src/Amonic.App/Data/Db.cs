using System.Configuration;
using System.Data.SqlClient;

namespace Amonic.App.Data
{
    public static class Db
    {
        public static SqlConnection OpenConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["SessionDb"].ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
