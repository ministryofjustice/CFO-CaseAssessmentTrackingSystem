using Cfo.Cats.Application.Features.Activities.Queries;
using Cfo.Cats.Application.Features.Locations.DTOs;
using Cfo.Cats.Server.UI.Services;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public class ActivitiesSessionStorage(ProtectedSessionStorage protectedSessionStorage, ICurrentUserService currentUserService)
    : CatsSessionStorage<ActivitiesSessionData>(protectedSessionStorage, currentUserService)
{
}

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
            Location = query.Location,
            Status = query.Status?.Value,
            IncludeTypes = query.IncludeTypes?.Select(t => t.Value).ToArray(),
            ReturnedWithinDays = query.ReturnedWithinDays,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber,
            Tabular = tabular
        };

    public string? TenantId { get; init; }
    public string? OwnerId { get; init; }
    public LocationDto? Location { get; init; }
    public int? Status { get; init; }
    public int[]? IncludeTypes { get; init; }
    public int? ReturnedWithinDays { get; init; }
    public string? Keyword { get; init; }
    public string? OrderBy { get; init; }
    public string? SortDirection { get; init; }
    public int PageNumber { get; init; } = 1;
    public bool Tabular { get; init; }
}
