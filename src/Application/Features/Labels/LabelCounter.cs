using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels;

public class LabelCounter(ISqlConnectionFactory sqlConnectionFactory) : ILabelCounter
{
    public int CountParticipants(LabelId labelId)
    {
        using var connection = sqlConnectionFactory.CreateOpenConnection();

        const string sql = """
                           SELECT Count(*) 
                           FROM [Participant].[Label] as [Label]
                           WHERE 
                              [Label].[LabelId] = @Id
                           """;

        return connection.QuerySingle<int>(sql, new
        {
            Id = @labelId.Value
        });
    }

    public int CountLabelsWithName(string name)
    {
        using var connection = sqlConnectionFactory.CreateOpenConnection();

        const string sql = """
                         SELECT Count(*) 
                         FROM [Configuration].[Label] as [Label]
                         WHERE 
                            [Label].[Name] = @Name
                         """;

        return connection.QuerySingle<int>(sql, new
        {
            Name = name,
        });
    }
}
