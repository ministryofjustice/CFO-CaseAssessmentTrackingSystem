using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels.Queries;

public static class GetVisibleLabels
{
    [RequestAuthorize]
    public class Query(UserProfile currentUser) : IQuery<Result<LabelDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IQueryHandler<Query, Result<LabelDto[]>>
    {
        public async Task<Result<LabelDto[]>> Handle(Query request, CancellationToken cancellationToken)
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
                                        COUNT([VisibleContracts].[ContractId]) as [{nameof(LabelDto.ContractCount)}],
                                        STRING_AGG(CAST([VisibleContracts].[ContractId] AS NVARCHAR(MAX)), ',') as [{nameof(LabelDto.ContractIdsRaw)}]
                                    FROM [Configuration].[Label] as [Label]
                                    INNER JOIN [Configuration].[LabelContract] as [LabelContract]
                                        ON [LabelContract].[LabelId] = [Label].[Id]
                                    INNER JOIN 
                                    (
                                        SELECT DISTINCT ContractId 
                                        FROM [Configuration].[Tenant]
                                        WHERE [Id] like @TenantId + '%'
                                        AND [ContractId] IS NOT NULL
                                    ) as [VisibleContracts] ON [VisibleContracts].[ContractId] = [LabelContract].[ContractId]
                                    GROUP BY 
                                        [Label].[Id],
                                        [Label].[Name],
                                        [Label].[Description],
                                        [Label].[Colour],
                                        [Label].[Variant],
                                        [Label].[AppIcon],
                                        [Label].[Scope]
                                """;

            var labels = await connection.QueryAsync<LabelDto>(sql, new { TenantId = request.CurrentUser.TenantId! });
            return labels.OrderBy(x => x.Name).ToArray();
        }
    }
}
