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
    public int ParticipantLocationId { get; init; }
    public string? ParticipantLocationName { get; init; }
    public int ReviewLocationId { get; init; }
    public string? ReviewLocationName { get; init; }
    public int? ReviewReason { get; init; }

    internal static PathwayPlanReviewsSessionData FromState(
        bool visualMode,
        bool showOverdueOnly,
        string? tenantId,
        string? userId,
        int participantLocationId,
        string? participantLocationName,
        int reviewLocationId,
        string? reviewLocationName,
        int? reviewReason)
        => new()
        {
            VisualMode = visualMode,
            ShowOverdueOnly = showOverdueOnly,
            TenantId = tenantId,
            UserId = userId,
            ParticipantLocationId = participantLocationId,
            ParticipantLocationName = participantLocationName,
            ReviewLocationId = reviewLocationId,
            ReviewLocationName = reviewLocationName,
            ReviewReason = reviewReason
        };
}
