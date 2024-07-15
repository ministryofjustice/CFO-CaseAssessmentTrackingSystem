using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.PaginationQuery;

public class KeyValuesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<KeyValuesWithPaginationQuery, PaginatedData<KeyValueDto>>
{
 
    public async Task<PaginatedData<KeyValueDto>> Handle(KeyValuesWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.KeyValues.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<KeyValue, KeyValueDto>(request.Specification, request.PageNumber,
            request.PageSize, mapper.ConfigurationProvider, cancellationToken);

        return data;
    }
}