using Cfo.Cats.Application.Features.Initiatives.DTOs;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Provider.Pages;

public record InitiativesSessionData
{
    public InitiativesSessionData()
    {
    }

    public bool VisualMode { get; init; } = true;
    public bool ShowActiveOnly { get; init; }
    public InitiativeDto? InitiativeFilter { get; init; }
    public string? TenantId { get; init; }
    public string? UserId { get; init; }

    internal static InitiativesSessionData FromState(
        bool visualMode,
        bool showActiveOnly,
        InitiativeDto? initiativeFilter,
        string? tenantId,
        string? userId)
        => new()
        {
            VisualMode = visualMode,
            ShowActiveOnly = showActiveOnly,
            InitiativeFilter = initiativeFilter,
            TenantId = tenantId,
            UserId = userId
        };
}
