using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.ReviewOutcomeQualityDipSample;

[RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipReview)]
public class Command : IRequest<Result>
{
    public required Guid SampleId { get; set; }
}
