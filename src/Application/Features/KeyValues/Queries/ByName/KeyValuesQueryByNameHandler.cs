using Cfo.Cats.Application.Features.KeyValues.DTOs;

namespace Cfo.Cats.Application.Features.KeyValues.Queries.ByName;

public class KeyValuesQueryByNameHandler : IRequestHandler<KeyValuesQueryByName, IEnumerable<KeyValueDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public KeyValuesQueryByNameHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper
    )
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<KeyValueDto>> Handle(KeyValuesQueryByName request,
        CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.DbContext.KeyValues.Where(x => x.Name == request.Name)
            .OrderBy(x => x.Text)
            .ProjectTo<KeyValueDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return data;
    }
}