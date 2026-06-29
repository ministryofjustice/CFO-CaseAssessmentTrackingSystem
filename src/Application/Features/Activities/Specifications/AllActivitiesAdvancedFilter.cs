using Cfo.Cats.Application.Common.Security;

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

    public int? LocationId { get; set; }

    /// <summary>
    /// Display name for the selected location (not used for filtering).
    /// </summary>
    public string? LocationName { get; set; }

    public int? Status { get; set; }

    public int? TypeFilter { get; set; }

    /// <summary>
    /// When set, restrict to activities that were returned to the provider within the
    /// last N days, regardless of whether they have since been actioned.
    /// </summary>
    public int? ReturnedWithinDays { get; set; }

    /// <summary>
    /// When set, restrict to activities that were approved within the last N days.
    /// </summary>
    public int? ApprovedWithinDays { get; set; }

    public bool IncludeInternalNotes { get; set; }
}
