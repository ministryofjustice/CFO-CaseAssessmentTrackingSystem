using Cfo.Cats.Application.Features.PathwayPlans.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public record TasksDueSessionData
{
    public TasksDueSessionData()
    {
    }

    internal static TasksDueSessionData FromQuery(TasksDueWithPagination.Query query, bool tabular)
        => new()
        {
            OwnerId = query.OwnerId,
            TenantId = query.TenantId,
            Category = query.Category,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber,
            Tabular = tabular
        };

    public string? OwnerId { get; init; }
    public string? TenantId { get; init; }
    public TaskDueCategory? Category { get; init; }
    public string? Keyword { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDirection { get; init; }
    public int PageNumber { get; init; } = 1;
    public bool Tabular { get; init; }
}
