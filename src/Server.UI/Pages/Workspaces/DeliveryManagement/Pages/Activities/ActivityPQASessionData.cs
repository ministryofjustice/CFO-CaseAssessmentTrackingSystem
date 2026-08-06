using Cfo.Cats.Application.Features.Activities.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.DeliveryManagement.Pages.Activities;

public record ActivityPQASessionData
{
    public ActivityPQASessionData()
    {
    }

    public required string? SupportWorkerId { get; init; }
    public required string? TenantId { get; init; }
    public required int? ActivityTypeId { get; init; }
    public required string? Keyword { get; init; }
    public required string OrderBy { get; init; }
    public required string SortDirection { get; init; }
    public required int PageNumber { get; init; }

    internal static ActivityPQASessionData FromQuery(ActivityQueueEntryFilter query)
        => new()
        {
            SupportWorkerId = query.SupportWorkerId,
            TenantId = query.TenantId,
            ActivityTypeId = query.ActivityTypeId,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber
        };
}
