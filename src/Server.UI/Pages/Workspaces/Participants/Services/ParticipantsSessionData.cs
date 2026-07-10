using Cfo.Cats.Application.Features.Participants.Queries;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Participants.Services;

public record ParticipantsSessionData
{
    public ParticipantsSessionData()
    {
    }

    internal static ParticipantsSessionData FromQuery(ParticipantsWithPagination.Query query, bool tabular) 
        => new()
        {
            Locations = query.Locations,
            TenantId = query.TenantId,
            Keyword = query.Keyword,
            OrderBy = query.OrderBy,
            SortDirection = query.SortDirection,
            ListView = query.ListView,
            PageNumber = query.PageNumber,
            LabelId = query.Label,
            OwnerId = query.OwnerId,
            RiskDue = query.RiskDue,
            RecentlyAssigned = query.RecentAction,
            Tabular = tabular
        };

    public required int[] Locations { get; init; }
    public required string? TenantId { get; init; } 
    public required string? Keyword { get; init; }
    public required string OrderBy { get; init; }
    public required string SortDirection { get; init; }
    public required ParticipantListView ListView { get; init; }
    public required int PageNumber { get; init; } 
    public required LabelId? LabelId { get; init; }
    public required string? OwnerId { get; init; } 
    public required DateTime? RiskDue { get; init; } 
    public RecentParticipantFilter RecentlyAssigned { get; init; } = RecentParticipantFilter.All;

    public bool Tabular {get; init; }
    
}
