using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Initiatives.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.Initiatives.Queries;

public static class GetInitiatives
{
    [RequestAuthorize(Policy = SecurityPolicies.Initiatives)]
    public class Query : IRequest<Result<InitiativeDto[]>>
    {
        public bool IncludeExpired { get; init; }
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<Query, Result<InitiativeDto[]>>
    {
        public async Task<Result<InitiativeDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                    SELECT
                                        [f].[Id]            AS [{nameof(InitiativeDto.Id)}],
                                        [f].[Code]          AS [{nameof(InitiativeDto.Code)}],
                                        [f].[Description]   AS [{nameof(InitiativeDto.Description)}],
                                        [f].[ContractId]    AS [{nameof(InitiativeDto.ContractId)}],
                                        [c].[Description]   AS [{nameof(InitiativeDto.Contract)}],
                                        [f].[LifetimeStart] AS [{nameof(InitiativeDto.LifetimeStart)}],
                                        [f].[LifetimeEnd]   AS [{nameof(InitiativeDto.LifetimeEnd)}]
                                    FROM [Configuration].[Initiative] AS [f]
                                    INNER JOIN [Configuration].[Contract] AS [c]
                                        ON [f].[ContractId] = [c].[Id]
                                    WHERE @IncludeExpired = 1 OR [f].[LifetimeEnd] >= GETUTCDATE()
                                    ORDER BY [f].[Created] desc
                                """;

            var funds = await connection.QueryAsync<InitiativeDto>(sql, new { request.IncludeExpired });
            return funds.ToArray();
        }
    }
}
