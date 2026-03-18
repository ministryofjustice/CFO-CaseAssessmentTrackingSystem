using System.Data;
using Microsoft.Data.SqlClient;

namespace Cfo.Cats.Infrastructure.Persistence;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateOpenConnection()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();

        return connection;
    }

    public string GetConnectionString() => connectionString;
}