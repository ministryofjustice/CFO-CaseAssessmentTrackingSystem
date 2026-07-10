using Cfo.Cats.Application.Features.Activities.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public record ActivitiesSessionData
{
    public ActivitiesSessionData()
    {
    }

    internal static ActivitiesSessionData FromQuery(AllActivitiesWithPagination.Query query, bool tabular)
        => new()
        {
            TenantId = query.TenantId,
            OwnerId = query.OwnerId,
            LocationId = query.LocationId,
            LocationName = query.LocationName,
            Status = query.Status,
            TypeFilter = query.TypeFilter,
            ReturnedWithinDays = query.ReturnedWithinDays,
            ApprovedWithinDays = query.ApprovedWithinDays,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber,
            Tabular = tabular
        };

    public string? TenantId { get; init; }
    public string? OwnerId { get; init; }
    public int? LocationId { get; init; }
    public string? LocationName { get; init; }
    public int? Status { get; init; }
    public int? TypeFilter { get; init; }
    public int? ReturnedWithinDays { get; init; }
    public int? ApprovedWithinDays { get; init; }
    public string? Keyword { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDirection { get; init; }
    public int PageNumber { get; init; } = 1;
    public bool Tabular { get; init; }
}
