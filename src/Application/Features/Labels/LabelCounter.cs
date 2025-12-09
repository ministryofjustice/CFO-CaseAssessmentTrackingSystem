using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels;

public class LabelCounter(ISqlConnectionFactory sqlConnectionFactory, ILogger<LabelCounter> logger) : ILabelCounter
{
    public int CountParticipants(LabelId labelId)
    {
        logger.LogWarning("Counting participants for label {LabelId} is currently not implemented", labelId.Value);
        return 0;
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