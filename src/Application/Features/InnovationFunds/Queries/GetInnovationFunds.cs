using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.InnovationFunds.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.InnovationFunds.Queries;

public static class GetInnovationFunds
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IRequest<Result<InnovationFundDto[]>>
    {
        public bool IncludeExpired { get; init; }
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory) : IRequestHandler<Query, Result<InnovationFundDto[]>>
    {
        public async Task<Result<InnovationFundDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                    SELECT
                                        [f].[Id]            AS [{nameof(InnovationFundDto.Id)}],
                                        [f].[Code]          AS [{nameof(InnovationFundDto.Code)}],
                                        [f].[Description]   AS [{nameof(InnovationFundDto.Description)}],
                                        [f].[ContractId]    AS [{nameof(InnovationFundDto.ContractId)}],
                                        [c].[Description]   AS [{nameof(InnovationFundDto.Contract)}],
                                        [f].[LifetimeStart] AS [{nameof(InnovationFundDto.LifetimeStart)}],
                                        [f].[LifetimeEnd]   AS [{nameof(InnovationFundDto.LifetimeEnd)}]
                                    FROM [Configuration].[InnovationFund] AS [f]
                                    INNER JOIN [Configuration].[Contract] AS [c]
                                        ON [f].[ContractId] = [c].[Id]
                                    WHERE @IncludeExpired = 1 OR [f].[LifetimeEnd] >= GETUTCDATE()
                                    ORDER BY [c].[Description], [f].[Code]
                                """;

            var funds = await connection.QueryAsync<InnovationFundDto>(sql, new { request.IncludeExpired });
            return funds.ToArray();
        }
    }
}
