namespace Cfo.Cats.Application.Common.Interfaces;

public interface ISqlConnectionFactory
{
    IDbConnection CreateOpenConnection();

    string GetConnectionString();
}