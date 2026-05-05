using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.AmendInnovationFundLifetime;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class AmendInnovationFundLifetimeCommand : IRequest<Result>
{
    public required Guid Id { get; set; }

    [Display(Name = "Start Date", Description = "The date from which this innovation fund is active")]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End Date", Description = "The date on which this innovation fund expires")]
    public DateTime? EndDate { get; set; }
}
