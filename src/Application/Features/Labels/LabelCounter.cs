using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels;

public class LabelCounter(ISqlConnectionFactory sqlConnectionFactory) : ILabelCounter
{
    public int CountParticipants(LabelId labelId)
    {
        var connection = sqlConnectionFactory.GetOpenConnection();
        
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

    public int CountVisibleLabels(string name, string? contractId)
    {
        var connection = sqlConnectionFactory.GetOpenConnection();
        
        const string sql = """
                         SELECT Count(*) 
                         FROM [Configuration].[Label] as [Label]
                         WHERE 
                            [Label].[Name] = @Name
                            AND (
                                @ContractId IS NULL
                                OR
                                ContractId = @ContractId
                            )
                         """;

        return connection.QuerySingle<int>(sql, new
        {
            Name = name,
            ContractId = contractId,
        });
    }
}