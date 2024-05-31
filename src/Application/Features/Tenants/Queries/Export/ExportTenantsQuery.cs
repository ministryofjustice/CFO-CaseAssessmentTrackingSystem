using Cfo.Cats.Application.Features.Tenants.Specifications;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Export;

public class ExportTenantsQuery : TenantAdvancedFilter, IRequest<Result<byte[]>>
{
    public TenantAdvancedSpecification Specification => new(this);
}