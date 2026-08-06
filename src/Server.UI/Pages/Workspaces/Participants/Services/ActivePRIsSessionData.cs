using Cfo.Cats.Application.Features.PRIs.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public record ActivePRIsSessionData
{
    public ActivePRIsSessionData()
    {
    }

    internal static ActivePRIsSessionData FromQuery(GetActivePRIsByUserId.Query query, bool tabular, PriTypeFilter priTypeFilter) 
        => new()
        {
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            PageNumber = query.PageNumber,
            IncludeOutgoing = query.IncludeOutgoing,
            IncludeIncoming = query.IncludeIncoming,
            Tabular = tabular,
            PriTypeFilter = priTypeFilter
        };

    public required string? Keyword { get; init; }
    public required string OrderBy { get; init; }
    public required string SortDirection { get; init; }
    public required int PageNumber { get; init; }
    public required bool IncludeOutgoing { get; init; }
    public required bool IncludeIncoming { get; init; }
    public required bool Tabular { get; init; }
    public required PriTypeFilter PriTypeFilter { get; init; }
}

public enum PriTypeFilter
{
    All,
    Outgoing,
    Incoming
}