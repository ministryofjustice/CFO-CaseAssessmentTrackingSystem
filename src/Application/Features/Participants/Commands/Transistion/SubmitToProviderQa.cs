using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Candidates.Caching;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Commands.Transistion;

public static class SubmitToProviderQa
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Command : ICacheInvalidatorRequest<Result>
    {
        public required string ParticipantId { get; set; }
        
        public string[] CacheKeys => [
            ParticipantCacheKey.GetCacheKey(ParticipantId),
            ParticipantCacheKey.GetSummaryCacheKey(ParticipantId)
        ];
        
        public CancellationTokenSource? SharedExpiryTokenSource => CandidatesCacheKey.SharedExpiryTokenSource();
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants.FindAsync(request.ParticipantId);
            participant!.TransitionTo(EnrolmentStatus.SubmittedToProviderStatus);
            // ReSharper disable once MethodHasAsyncOverload
            return Result.Success();
        }
    }

}
