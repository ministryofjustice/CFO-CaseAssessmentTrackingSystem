using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Compliance.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Compliance.Queries;

[RequestAuthorize(Policy = SecurityPolicies.ContractData)]
public class GetRiskComplianceQuery : IRequest<Result<RiskComplianceSummaryDto[]>>
{
    public DateTime Date { get; set; }
}

