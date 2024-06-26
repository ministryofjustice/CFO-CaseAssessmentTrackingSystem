using Cfo.Cats.Application.Features.Candidates.Caching;
using Cfo.Cats.Application.Features.Candidates.DTOs;

namespace Cfo.Cats.Application.Features.Candidates.Queries.Search;

public class CandidateSearchQueryHandler : IRequestHandler<CandidateSearchQuery, IEnumerable<CandidateDto>>
{
    private readonly IApplicationDbContext dbContext;
    private readonly IMapper mapper;

    public CandidateSearchQueryHandler(IApplicationDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<CandidateDto>> Handle(CandidateSearchQuery request, CancellationToken cancellationToken)
    {
        var query = from c in dbContext.Candidates
            where 
                c.LastName == request.LastName && c.DateOfBirth == request.DateOfBirth
                || c.Identifiers.Any(i => i.IdentifierValue == request.ExternalIdentifier)
            select c;
        
        
        return await query.ProjectTo<CandidateDto>(mapper.ConfigurationProvider).ToArrayAsync(cancellationToken);

    }
}