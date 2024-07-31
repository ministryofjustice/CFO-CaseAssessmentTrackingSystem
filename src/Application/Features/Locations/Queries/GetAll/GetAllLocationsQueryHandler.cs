using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

public class GetAllLocationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllLocationsQuery, Result<LocationDto[]>>
{
    public async Task<Result<LocationDto[]>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.Locations.ApplySpecification(request.Specification)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return data;
    }
}