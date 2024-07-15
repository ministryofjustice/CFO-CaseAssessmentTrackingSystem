using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsWithPagination
{
    [RequestAuthorize(Policy = PolicyNames.AllowCandidateSearch)]
    public class Query : ParticipantAdvancedFilter, ICacheableRequest<PaginatedData<ParticipantDto>>
    {

        public ParticipantAdvancedSpecification Specification => new(this);
    
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;

        public override string ToString() =>
            $"ListView:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {CurrentUser!.UserId}";
    }
    
    internal class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<ParticipantDto>>
    {
        public async Task<PaginatedData<ParticipantDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Participants.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Participant, ParticipantDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
    }
    
}





