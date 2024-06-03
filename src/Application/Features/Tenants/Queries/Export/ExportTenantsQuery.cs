using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Specifications;

namespace Cfo.Cats.Application.Features.Tenants.Queries.Export;

[RequestAuthorize(Roles = "Admin")]
public class ExportTenantsQuery : TenantAdvancedFilter, IRequest<Result<byte[]>>
{
    public TenantAdvancedSpecification Specification => new(this);
}