using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.DeactivateInitiative;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class DeactivateInitiativeCommand : IRequest<Result>
{
    public required Guid Id { get; set; }
}
