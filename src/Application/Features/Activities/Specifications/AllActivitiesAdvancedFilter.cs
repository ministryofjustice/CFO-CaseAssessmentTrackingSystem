using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class AllActivitiesAdvancedFilter : PaginationFilter
{
    public required UserProfile UserProfile { get; set; }

    /// <summary>
    /// Restrict to a tenant (hierarchical, dot-notation prefix match). When null the
    /// user's own tenant hierarchy is used.
    /// </summary>
    public string? TenantId { get; set; }

    /// <summary>
    /// Restrict to activities submitted by (owned by) a specific user.
    /// </summary>
    public string? OwnerId { get; set; }

    public LocationDto? Location { get; set; }

    public ActivityStatus? Status { get; set; }

    public List<ActivityType>? IncludeTypes { get; set; }

    /// <summary>
    /// When set, restrict to activities that were returned to the provider within the
    /// last N days, regardless of whether they have since been actioned.
    /// </summary>
    public int? ReturnedWithinDays { get; set; }

    public bool IncludeInternalNotes { get; set; }
}
