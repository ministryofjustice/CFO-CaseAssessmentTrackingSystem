using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.Initiatives.Queries;

public static class GetActiveInitiativesForTenant
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<InitiativeDto[]>> { }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory, ICurrentUserService currentUserService)
        : IRequestHandler<Query, Result<InitiativeDto[]>>
    {
        public async Task<Result<InitiativeDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            var tenantPattern = (currentUserService.TenantId ?? string.Empty) + "%";

            const string sql = $"""
                SELECT
                    [i].[Id]            AS [{nameof(InitiativeDto.Id)}],
                    [i].[Code]          AS [{nameof(InitiativeDto.Code)}],
                    [i].[Description]   AS [{nameof(InitiativeDto.Description)}],
                    [i].[ContractId]    AS [{nameof(InitiativeDto.ContractId)}],
                    [c].[Description]   AS [{nameof(InitiativeDto.Contract)}],
                    [i].[LifetimeStart] AS [{nameof(InitiativeDto.LifetimeStart)}],
                    [i].[LifetimeEnd]   AS [{nameof(InitiativeDto.LifetimeEnd)}]
                FROM [Configuration].[Initiative] AS [i]
                INNER JOIN [Configuration].[Contract] AS [c]
                    ON [i].[ContractId] = [c].[Id]
                WHERE [i].[LifetimeEnd] >= GETUTCDATE()
                  AND [c].[TenantId] LIKE @TenantPattern
                ORDER BY [i].[Code]
                """;

            var initiatives = await connection.QueryAsync<InitiativeDto>(sql, new { TenantPattern = tenantPattern });
            return initiatives.ToArray();
        }
    }
}
