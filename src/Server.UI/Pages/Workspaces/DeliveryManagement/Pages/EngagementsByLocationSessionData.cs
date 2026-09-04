namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public record EngagementsByLocationSessionData
{
    public EngagementsByLocationSessionData()
    {
    }

    public bool VisualMode { get; init; } = true;
    public int LocationId { get; init; }
    public int Month { get; init; }
    public int Year { get; init; }
    public string? LocationName { get; init; }
    public string? EngagementType { get; init; }
    public string? LocationType { get; init; }
    public string? TenantId { get; init; }
    public string? TenantName { get; init; }

    internal static EngagementsByLocationSessionData FromState(
        bool visualMode,
        int month,
        int year,
        int locationId,
        string? locationName,
        string? engagementType,
        string? locationType,
        string? tenantId,
        string? tenantName)
        => new()
        {
            VisualMode = visualMode,
            Month = month,
            Year = year,
            LocationId = locationId,
            LocationName = locationName,
            EngagementType = engagementType,
            LocationType = locationType,
            TenantId = tenantId,
            TenantName = tenantName
        };
}