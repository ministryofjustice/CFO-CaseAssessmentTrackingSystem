using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels.Queries;

public static class GetLabelById
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query(Guid id) : IQuery<Result<LabelDto>>
    {
        public Guid LabelId { get; } = id;
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<Query, Result<LabelDto>>
    {
        public async Task<Result<LabelDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                SELECT 
                                    [Label].[Id] as [{nameof(LabelDto.Id)}],
                                    [Label].[Name] as [{nameof(LabelDto.Name)}],
                                    [Label].[Description] as [{nameof(LabelDto.Description)}],
                                    [Label].[Colour] as [{nameof(LabelDto.Colour)}],
                                    [Label].[Variant] as [{nameof(LabelDto.Variant)}],
                                    [Label].[AppIcon] as [{nameof(LabelDto.AppIcon)}],
                                    [Label].[Scope] as [{nameof(LabelDto.Scope)}],
                                    COUNT([LabelContract].[ContractId]) as [{nameof(LabelDto.ContractCount)}],
                                    STRING_AGG(CAST([LabelContract].[ContractId] AS NVARCHAR(MAX)), ',') as [{nameof(LabelDto.ContractIdsRaw)}]
                                FROM [Configuration].[Label] as [Label]
                                LEFT JOIN [Configuration].[LabelContract] as [LabelContract]
                                    on [LabelContract].[LabelId] = [Label].[Id]
                                WHERE [Label].[Id] = @LabelId
                                GROUP BY 
                                    [Label].[Id],
                                    [Label].[Name],
                                    [Label].[Description],
                                    [Label].[Colour],
                                    [Label].[Variant],
                                    [Label].[AppIcon],
                                    [Label].[Scope]
                                """;
            
            var label = await connection.QuerySingleAsync<LabelDto>(sql, new {request.LabelId});
            return label;
        }
    }

}
