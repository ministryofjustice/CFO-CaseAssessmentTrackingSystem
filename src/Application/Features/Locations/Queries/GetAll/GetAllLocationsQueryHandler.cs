using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Locations.Queries.GetAll;

public class GetAllLocationsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllLocationsQuery, Result<LocationDto[]>>
{
    public async Task<Result<LocationDto[]>> Handle(GetAllLocationsQuery request, CancellationToken cancellationToken)
    {
        // holding pen for the locations
        List<LocationDto> locations = [];
        
        locations.AddRange(await unitOfWork.DbContext.Locations
            .Where(cl => cl.Contract!.Tenant!.Id.StartsWith(request.UserProfile!.TenantId!))
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken));

        return locations.ToArray();
    }
}