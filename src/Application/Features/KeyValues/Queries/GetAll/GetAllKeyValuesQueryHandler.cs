using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.GetAll;

public class GetAllKeyValuesQueryHandler : IRequestHandler<GetAllKeyValuesQuery, IEnumerable<KeyValueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllKeyValuesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(GetAllKeyValuesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}