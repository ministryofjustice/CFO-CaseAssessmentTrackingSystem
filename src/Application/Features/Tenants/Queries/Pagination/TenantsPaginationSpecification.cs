using Cfo.Cats.Domain.Entities.Administration;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Pagination;

public class TenantsPaginationSpecification : Specification<Tenant>
{
    public TenantsPaginationSpecification(TenantsWithPaginationQuery query)
    {
        Query
            .Where(q => q.Name != null)
            .Where(
                q => q.Name!.Contains(query.Keyword!) || q.Description!.Contains(query.Keyword!),
                !string.IsNullOrEmpty(query.Keyword)
            );
    }
}