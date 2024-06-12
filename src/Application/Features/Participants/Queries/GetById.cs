using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantById
{
    [RequestAuthorize(Roles = "Admin")]
    public class Query : ICacheableRequest<ParticipantDto>
    {
        public required string Id { get; set; }
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{Id}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;
    }

    public class Handler : IRequestHandler<Query, ParticipantDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IStringLocalizer<Query> _localizer;
        private readonly IMapper _mapper;

        public Handler(IApplicationDbContext context, IStringLocalizer<Query> localizer, IMapper mapper)
        {
            _context = context;
            _localizer = localizer;
            _mapper = mapper;
        }
        
        public async Task<ParticipantDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await _context.Participants.ApplySpecification(new ParticipantByIdSpecification(request.Id))
                           .ProjectTo<ParticipantDto>(_mapper.ConfigurationProvider)
                           .SingleOrDefaultAsync(cancellationToken)
                       ?? throw new NotFoundException($"Participant with id: [{request.Id}] not found");
            return data;
        }
    }

}
