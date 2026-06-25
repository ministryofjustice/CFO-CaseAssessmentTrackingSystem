using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public abstract class ParticipantDetailsQuery<TResult> : IQuery<Result<TResult>>
{
    public required string ParticipantId { get; init; }
    public required UserProfile CurrentUser { get; init; }
}
