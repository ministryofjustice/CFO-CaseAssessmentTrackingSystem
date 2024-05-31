using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Tenants.Specifications;

public class TenantAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
}