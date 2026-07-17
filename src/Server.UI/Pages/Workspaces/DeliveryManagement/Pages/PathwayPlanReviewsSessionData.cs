namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public record PathwayPlanReviewsSessionData
{
    public PathwayPlanReviewsSessionData()
    {
    }

    public bool VisualMode { get; init; } = true;
    public bool ShowOverdueOnly { get; init; }
    public string? TenantId { get; init; }
    public string? UserId { get; init; }

    internal static PathwayPlanReviewsSessionData FromState(
        bool visualMode,
        bool showOverdueOnly,
        string? tenantId,
        string? userId)
        => new()
        {
            VisualMode = visualMode,
            ShowOverdueOnly = showOverdueOnly,
            TenantId = tenantId,
            UserId = userId
        };
}
