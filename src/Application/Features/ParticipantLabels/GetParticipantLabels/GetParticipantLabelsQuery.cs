using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Participants;

namespace Cfo.Cats.Application.Features.ParticipantLabels.GetParticipantLabels;

[RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
public class GetParticipantLabelsQuery(ParticipantId participantId) : IRequest<Result<GetParticipantLabelsDto>>
{
    public ParticipantId ParticipantId { get; } = participantId;
}