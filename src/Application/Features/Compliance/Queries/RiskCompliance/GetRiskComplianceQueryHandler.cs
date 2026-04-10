using Cfo.Cats.Application.Common.Interfaces.Contracts;
using Cfo.Cats.Application.Features.Compliance.DTOs;
using Dapper;

namespace Cfo.Cats.Application.Features.Compliance.Queries;

public class GetRiskComplianceQueryHandler(ISqlConnectionFactory sqlConnectionFactory, ICurrentUserService currentUserService, IContractService contractService) : IRequestHandler<GetRiskComplianceQuery, Result<RiskComplianceSummaryDto[]>>
{
    public async Task<Result<RiskComplianceSummaryDto[]>> Handle(GetRiskComplianceQuery request, CancellationToken cancellationToken)
    {
        if(currentUserService.UserId is null)
        {
            throw new InvalidOperationException("Cannot be called without an active user");
        }        

        using var connection = sqlConnectionFactory.CreateOpenConnection();
        var contracts = contractService.GetVisibleContracts(currentUserService.UserId);

        const string sql = "[mi].[RiskComplianceReport]";

        var data = await connection.QueryAsync<RiskComplianceSummaryDto>(sql, new
        {
            Date = DateTime.Now.Date,
            ContractId = (string?)null,
            Aggregate = true
        }, commandType: CommandType.StoredProcedure);

        var visible = contractService.GetVisibleContracts(currentUserService.TenantId!);

        return data.Where(c => visible.Any(v => v.Name.Equals(c.Contract, StringComparison.CurrentCultureIgnoreCase))).ToArray();

    }
}