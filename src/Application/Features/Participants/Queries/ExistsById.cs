using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class CheckParticipantExistsById
{
    [RequestAuthorize(Policy = PolicyNames.AllowEnrol)]
    public class Query : ICacheableRequest<bool>
    {
        public required string Id { get; set; }
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{Id}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;
    }

    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, bool>
    {
        
        public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Participants
                .AnyAsync(p => p.Id == request.Id, cancellationToken);
        }
    }

}

