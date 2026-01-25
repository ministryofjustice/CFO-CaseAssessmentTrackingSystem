using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Labels.DTOs;
using Cfo.Cats.Domain.Labels;
using Dapper;

namespace Cfo.Cats.Application.Features.Labels.Queries;

public static class GetVisibleLabels
{
    [RequestAuthorize]
    public class Query(UserProfile currentUser) : IRequest<Result<LabelDto[]>>
    {
        public UserProfile CurrentUser { get; } = currentUser;
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<Query, Result<LabelDto[]>>
    {
        public async Task<Result<LabelDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var connection = sqlConnectionFactory.GetOpenConnection();

            const string sql = $"""
                                    SELECT 
                                        [Label].[Id] as [{nameof(LabelDto.Id)}],
                                        [Label].[Name] as [{nameof(LabelDto.Name)}],
                                        [Label].[Description] as [{nameof(LabelDto.Description)}],
                                        [Label].[Colour] as [{nameof(LabelDto.Colour)}],
                                        [Label].[Variant] as [{nameof(LabelDto.Variant)}],
                                        NULL as [{nameof(LabelDto.Contract)}],
                                        [Label].[AppIcon] as [{nameof(LabelDto.AppIcon)}]
                                    FROM [Configuration].[Label] as [Label]
                                    WHERE [ContractId] IS NULL
                                    UNION
                                    SELECT 
                                        [Label].[Id] as [{nameof(LabelDto.Id)}],
                                        [Label].[Name] as [{nameof(LabelDto.Name)}],
                                        [Label].[Description] as [{nameof(LabelDto.Description)}],
                                        [Label].[Colour] as [{nameof(LabelDto.Colour)}],
                                        [Label].[Variant] as [{nameof(LabelDto.Variant)}],
                                        [Contract].[Description] as [{nameof(LabelDto.Contract)}],
                                        [Label].[AppIcon] as [{nameof(LabelDto.AppIcon)}]
                                    FROM [Configuration].[Label] as [Label]
                                    INNER JOIN [Configuration].[Contract] as [Contract]
                                        ON [Label].[ContractId] = [Contract].[Id]
                                    INNER JOIN 
                                    (
                                        SELECT DISTINCT ContractId 
                                        FROM [Configuration].[Tenant]
                                        WHERE [Id] like @TenantId + '%'
                                        AND [ContractId] IS NOT NULL
                                    ) as [VisibleContracts] ON [VisibleContracts].[ContractId] = [Label].[ContractId]
                                """;

            var labels = await connection.QueryAsync<LabelDto>(sql, new { TenantId = request.CurrentUser.TenantId! });
            return labels.OrderBy(x => x.Contract).ToArray();
        }
    }
}