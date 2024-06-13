using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;

public class KeyValuesQueryHandler : IRequestHandler<KeyValuesWithPaginationQuery, PaginatedData<KeyValueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public KeyValuesQueryHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await _context.KeyValues.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<KeyValue, KeyValueDto>(request.Specification, request.PageNumber,
            request.PageSize, _mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}