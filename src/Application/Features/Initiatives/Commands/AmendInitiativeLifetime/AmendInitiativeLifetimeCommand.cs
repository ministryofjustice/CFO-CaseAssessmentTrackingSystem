using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Initiatives.Commands.AmendInitiativeLifetime;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class AmendInitiativeLifetimeCommand : IRequest<Result>
{
    public required Guid Id { get; set; }

    [Display(Name = "Start Date", Description = "The date from which this initiative is active")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date", Description = "The date on which this initiative expires")]
    public DateTime? EndDate { get; set; }
}
