using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Application.Features.ParticipantLabels.AddParticipantLabel;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class AddParticipantLabelCommand(ParticipantId participantId, LabelId labelId) : IRequest<Result>
{
    public ParticipantId ParticipantId { get; } = participantId;
    public LabelId LabelId { get; } = labelId;
}