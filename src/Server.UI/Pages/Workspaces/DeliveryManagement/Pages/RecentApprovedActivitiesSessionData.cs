namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public record RecentApprovedActivitiesSessionData
{
    public RecentApprovedActivitiesSessionData()
    {
    }

    public required DateTime? StartDate { get; init; }
    public required DateTime? EndDate { get; init; }
    public required bool VisualMode { get; init; }
    public required string? TenantId { get; init; }
    public required string? UserId { get; init; }

    internal static RecentApprovedActivitiesSessionData FromState(
        DateRange dateRange,
        bool visualMode,
        string? tenantId,
        string? userId)
        => new()
        {
            StartDate = dateRange.Start,
            EndDate = dateRange.End,
            VisualMode = visualMode,
            TenantId = tenantId,
            UserId = userId
        };
}
