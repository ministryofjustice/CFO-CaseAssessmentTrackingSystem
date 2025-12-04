using System.Data;
using Microsoft.Data.SqlClient;

namespace Cfo.Cats.Infrastructure.Persistence;

public class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory, IDisposable
{
    private IDbConnection? _connection;

    public IDbConnection GetOpenConnection()
    {
        if (this._connection is { State: ConnectionState.Open })
        {
            return _connection;
        }

        _connection = new SqlConnection(connectionString);
        _connection.Open();

        return _connection;
    }
    
    public IDbConnection CreateNewConnection()
    {
        var connection = new SqlConnection(connectionString);
        connection.Open();

        return connection;
    }

    public string GetConnectionString() => connectionString;

    public void Dispose()
    {
        if (this._connection != null && this._connection.State == ConnectionState.Open)
        {
            this._connection.Dispose();
        }
    }
}