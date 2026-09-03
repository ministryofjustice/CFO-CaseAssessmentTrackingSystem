namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public record LatestEngagementsByLocationSessionData
{
    public LatestEngagementsByLocationSessionData()
    {
    }

    public bool VisualMode { get; init; } = true;
    public bool HideRecentEngagements { get; init; }
    public int LocationId { get; init; }
    public string? LocationName { get; init; }
    public string? EngagementType { get; init; }
    public string? TenantId { get; init; }
    public string? TenantName { get; init; }
    public string? CurrentSupportWorker { get; init; }

    internal static LatestEngagementsByLocationSessionData FromState(
        bool visualMode,
        bool hideRecentEngagements,
        int locationId,
        string? locationName,
        string? engagementType,
        string? tenantId,
        string? tenantName,
        string? currentSupportWorker)
        => new()
        {
            VisualMode = visualMode,
            HideRecentEngagements = hideRecentEngagements,
            LocationId = locationId,
            LocationName = locationName,
            EngagementType = engagementType,
            TenantId = tenantId,
            TenantName = tenantName,
            CurrentSupportWorker = currentSupportWorker
        };
}
