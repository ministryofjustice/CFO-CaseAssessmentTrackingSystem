using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

[RequestAuthorize(Policy = "Permissions.Participants.View")]
public class ParticipantsWithPaginationQuery
    : ParticipantAdvancedFilter, ICacheableRequest<PaginatedData<ParticipantDto>>
{

    public ParticipantAdvancedSpecification Specification => new(this);
    
    public string CacheKey => ParticipantCacheKey.GetCacheKey($"{this}");
    public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;

    public override string ToString() =>
        $"ListView:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {CurrentUser!.UserId}";
}

public class ParticipantsPaginationQueryHandler
    : IRequestHandler<ParticipantsWithPaginationQuery, PaginatedData<ParticipantDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public ParticipantsPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<ParticipantDto>> Handle(ParticipantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Participants.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<Participant, ParticipantDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}
