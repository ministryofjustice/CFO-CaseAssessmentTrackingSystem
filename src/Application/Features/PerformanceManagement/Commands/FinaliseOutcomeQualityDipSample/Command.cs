using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.FinaliseOutcomeQualityDipSample;

[RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipFinalise)]
public record Command : IRequest<Result>
{
    public required Guid SampleId { get; set; }
}