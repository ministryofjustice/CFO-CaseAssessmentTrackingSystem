using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Tenants.Caching;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Tenants.Queries.GetAll;

[RequestAuthorize(Policy = SecurityPolicies.SystemFunctionsRead)]
public class GetAllTenantsQuery : IRequest<Result<IEnumerable<TenantDto>>>
{
    public UserProfile? UserProfile { get; set; }
}