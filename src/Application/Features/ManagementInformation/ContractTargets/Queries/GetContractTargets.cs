using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Dapper;

namespace Cfo.Cats.Application.Features.ManagementInformation.ContractTargets.Queries;

public static class GetContractTargets
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Query : IQuery<Result<ContractTargetListItemDto[]>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
    }

    public class Handler(ISqlConnectionFactory sqlConnectionFactory)
        : IQueryHandler<Query, Result<ContractTargetListItemDto[]>>
    {
        public async Task<Result<ContractTargetListItemDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            using var connection = sqlConnectionFactory.CreateOpenConnection();

            const string sql = $"""
                                    SELECT
                                        [t].[Id]                   AS [{nameof(ContractTargetListItemDto.Id)}],
                                        [t].[ContractId]           AS [{nameof(ContractTargetListItemDto.ContractId)}],
                                        [c].[Description]          AS [{nameof(ContractTargetListItemDto.ContractName)}],
                                        [t].[Year]                 AS [{nameof(ContractTargetListItemDto.Year)}],
                                        [t].[Month]                AS [{nameof(ContractTargetListItemDto.Month)}],
                                        [t].[Prison]               AS [{nameof(ContractTargetListItemDto.Prison)}],
                                        [t].[Community]            AS [{nameof(ContractTargetListItemDto.Community)}],
                                        [t].[Wings]                AS [{nameof(ContractTargetListItemDto.Wings)}],
                                        [t].[Hubs]                 AS [{nameof(ContractTargetListItemDto.Hubs)}],
                                        [t].[PreReleaseSupport]    AS [{nameof(ContractTargetListItemDto.PreReleaseSupport)}],
                                        [t].[ThroughTheGate]       AS [{nameof(ContractTargetListItemDto.ThroughTheGate)}],
                                        [t].[SupportWork]          AS [{nameof(ContractTargetListItemDto.SupportWork)}],
                                        [t].[HumanCitizenship]     AS [{nameof(ContractTargetListItemDto.HumanCitizenship)}],
                                        [t].[CommunityAndSocial]   AS [{nameof(ContractTargetListItemDto.CommunityAndSocial)}],
                                        [t].[Interventions]        AS [{nameof(ContractTargetListItemDto.Interventions)}],
                                        [t].[Employment]           AS [{nameof(ContractTargetListItemDto.Employment)}],
                                        [t].[TrainingAndEducation] AS [{nameof(ContractTargetListItemDto.TrainingAndEducation)}]
                                    FROM [Mi].[ContractTarget] AS [t]
                                    INNER JOIN [Configuration].[Contract] AS [c]
                                        ON [t].[ContractId] = [c].[Id]
                                    WHERE (@Year IS NULL OR [t].[Year] = @Year)
                                      AND (@Month IS NULL OR [t].[Month] = @Month)
                                    ORDER BY [t].[Year], [t].[Month], [c].[Description]
                                """;

            var targets = await connection.QueryAsync<ContractTargetListItemDto>(
                sql, new { request.Year, request.Month });

            return targets.ToArray();
        }
    }
}
