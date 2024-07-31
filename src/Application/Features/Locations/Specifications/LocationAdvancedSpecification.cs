using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Locations.Specifications;

public class LocationAdvancedSpecification: Specification<Location>
{
    public LocationAdvancedSpecification(LocationAdvancedFilter filter)
    {

        Query.Where(location => location.Tenants
            .Any(tenant => tenant.Id.StartsWith(filter.UserProfile!.TenantId!)));

        if (filter.TenantId is not null)
        {
            Query.Where(location => location.Tenants
                .Any(tenant => tenant.Id.StartsWith(filter.TenantId)));
        }
    }
}