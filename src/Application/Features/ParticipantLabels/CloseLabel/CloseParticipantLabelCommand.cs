using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;

namespace Cfo.Cats.Application.Features.ParticipantLabels.CloseLabel;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class CloseParticipantLabelCommand(ParticipantLabelId labelId) : IRequest<Result>
{
    public ParticipantLabelId ParticipantLabelId { get; } = labelId;
}
