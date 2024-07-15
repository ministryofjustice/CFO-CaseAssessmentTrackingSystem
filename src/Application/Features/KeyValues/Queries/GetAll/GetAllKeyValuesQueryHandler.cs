using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.GetAll;

public class GetAllKeyValuesQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<GetAllKeyValuesQuery, IEnumerable<KeyValueDto>>
{
    public async Task<IEnumerable<KeyValueDto>> Handle(GetAllKeyValuesQuery request,
        CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.KeyValues.OrderBy(x => x.Name).ThenBy(x => x.Value)
            .ProjectTo<KeyValueDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}