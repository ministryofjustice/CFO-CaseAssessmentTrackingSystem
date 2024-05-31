using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Specifications;

public class TenantAdvancedSpecification : Specification<Tenant>
{
    public TenantAdvancedSpecification(TenantAdvancedFilter filter)
    {
        Query.Where(t => t.Name != null)
            .Where(t => t.Name!.Contains(filter.Keyword!) || t.Description!.Contains(filter.Keyword!),
                string.IsNullOrEmpty(filter.Keyword) == false)
            .Where(t => t.Id.StartsWith(filter.CurrentUser!.TenantId!), 
                filter.CurrentUser != null);

    }
}