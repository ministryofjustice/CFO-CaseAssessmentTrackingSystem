using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Locations.Specifications;

public class LocationAdvancedFilter
{
    //The current user
    public UserProfile? UserProfile { get; set; }
    
    //The Id of the tenant you specifically want to look at.
    public string? TenantId { get; set; }
}
