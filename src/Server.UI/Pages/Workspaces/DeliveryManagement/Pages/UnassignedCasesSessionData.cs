namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages;

public record UnassignedCasesSessionData
{
    public UnassignedCasesSessionData()
    {
    }

    public bool VisualMode { get; init; } = true;
    public bool IncludeTransferIn { get; init; } = true;
    public string? Keyword { get; init; }
    public int? EnrolmentStatus { get; init; }
    public int? LocationId { get; init; }
    public string? TenantId { get; init; }
    public string? UserId { get; init; }

    internal static UnassignedCasesSessionData FromState(
        bool visualMode,
        bool includeTransferIn,
        string? keyword,
        int? enrolmentStatus,
        int? locationId,
        string? tenantId,
        string? userId)
        => new()
        {
            VisualMode = visualMode,
            IncludeTransferIn = includeTransferIn,
            Keyword = keyword,
            EnrolmentStatus = enrolmentStatus,
            LocationId = locationId,
            TenantId = tenantId,
            UserId = userId
        };
}
