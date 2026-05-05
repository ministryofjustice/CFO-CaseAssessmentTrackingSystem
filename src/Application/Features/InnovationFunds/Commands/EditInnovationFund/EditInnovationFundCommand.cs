using System.ComponentModel.DataAnnotations;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Contracts.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.InnovationFunds.Commands.EditInnovationFund;

[RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
public class EditInnovationFundCommand : IRequest<Result>
{
    public required Guid Id { get; set; }

    [Display(Name = "Code", Description = "A unique 8-character code identifying this innovation fund (e.g. IF-01-01)")]
    public required string Code { get; set; }

    [Display(Name = "Description", Description = "A description of the innovation fund")]
    public required string Description { get; set; }

    [Display(Name = "Contract", Description = "The contract this innovation fund is associated with")]
    public ContractDto? Contract { get; set; }
}
