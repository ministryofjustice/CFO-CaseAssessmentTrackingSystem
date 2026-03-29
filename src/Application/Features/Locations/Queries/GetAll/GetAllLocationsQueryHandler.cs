using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

public class GetAllLocationsQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllLocationsQuery, Result<LocationDto[]>>
{
    public async Task<Result<LocationDto[]>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.DbContext.Locations.ApplySpecification(request.Specification)
            .Select(LocationMappings.ToDto)
            .AsNoTracking()
            .ToArrayAsync(cancellationToken);

        return data;
    }
}