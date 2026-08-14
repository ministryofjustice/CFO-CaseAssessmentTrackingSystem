using Cfo.Cats.Application.Features.PRIs.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public record ActivePRIsSessionData
{
    public ActivePRIsSessionData()
    {
    }

    internal static ActivePRIsSessionData FromQuery(ActivePRIsWithPagination.Query query, bool tabular, PriTypeFilter priTypeFilter) 
        => new()
        {
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber,
            IncludeOutgoing = query.IncludeOutgoing,
            IncludeIncoming = query.IncludeIncoming,
            Tabular = tabular,
            PriTypeFilter = priTypeFilter,
            CustodySupportWorker = query.CustodySupportWorker,
            CommunitySupportWorker = query.CommunitySupportWorker,
            ExpectedReleaseRegionId = query.ExpectedReleaseRegionId,
            ActiveStatus = query.ActiveStatus,
            JustMyPris = query.JustMyPris
        };

    public required string? Keyword { get; init; }
    public required string OrderBy { get; init; }
    public required string SortDirection { get; init; }
    public required int PageNumber { get; init; }
    public required bool IncludeOutgoing { get; init; }
    public required bool IncludeIncoming { get; init; }
    public required bool Tabular { get; init; }
    public required PriTypeFilter PriTypeFilter { get; init; }
    public string? CustodySupportWorker { get; init; }
    public string? CommunitySupportWorker { get; init; }
    public int? ExpectedReleaseRegionId { get; init; }
    public bool? ActiveStatus { get; init; }
    public bool JustMyPris { get; init; }
}

public enum PriTypeFilter
{
    All,
    Outgoing,
    Incoming
}