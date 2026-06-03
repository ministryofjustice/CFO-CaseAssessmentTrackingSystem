using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class ActivityQueueEntryFilter
: PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }
    public string? SupportWorkerId { get; set; }
    public string? TenantId { get; set; }
    public int? ActivityTypeId { get; set; }
}