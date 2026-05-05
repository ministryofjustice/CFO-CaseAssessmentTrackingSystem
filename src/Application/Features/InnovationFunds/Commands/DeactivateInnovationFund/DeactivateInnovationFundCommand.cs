using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.DeactivateInnovationFund;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class DeactivateInnovationFundCommand : IRequest<Result>
{
    public required Guid Id { get; set; }
}
