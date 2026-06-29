using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

/// <summary>
/// The shared filter for the participants view. Both the flat paginated list
/// (<c>ParticipantsWithPagination.Query</c>) and the grouped summary (<c>GetGroupedParticipants.Query</c>)
/// inherit this, so the UI binds a single filter and the same fields drive both queries.
/// Pagination/sort/keyword come from <see cref="PaginationFilter"/>.
/// </summary>
public class ParticipantFilter : PaginationFilter
{
    public ParticipantFilter() { }

    /// <summary>Copy constructor — lets a derived query be built from an existing filter without copying field-by-field.</summary>
    protected ParticipantFilter(ParticipantFilter source)
    {
        Keyword = source.Keyword;
        PageNumber = source.PageNumber;
        PageSize = source.PageSize;
        OrderBy = source.OrderBy;
        SortDirection = source.SortDirection;
        ListView = source.ListView;
        CurrentUser = source.CurrentUser;
        JustMyCases = source.JustMyCases;
        Locations = source.Locations;
        Label = source.Label;
        OwnerId = source.OwnerId;
        TenantId = source.TenantId;
        RiskDue = source.RiskDue;
        RecentAction = source.RecentAction;
        GroupBy = source.GroupBy;
    }

    /// <summary>The filter for the list (based on the status).</summary>
    public ParticipantListView ListView { get; set; } = ParticipantListView.Default;

    /// <summary>The currently logged in user.</summary>
    public UserProfile? CurrentUser { get; set; }

    /// <summary>Flag to indicate that you only want to see your own cases.</summary>
    [Description("Just My Cases")]
    public bool JustMyCases { get; set; } = true;

    /// <summary>The current location of the participant.</summary>
    public int[] Locations { get; set; } = [];

    public LabelId? Label { get; set; }
    public string? OwnerId { get; set; }
    public string? TenantId { get; set; }
    public DateTime? RiskDue { get; set; }
    public RecentParticipantFilter RecentAction { get; set; } = RecentParticipantFilter.All;

    /// <summary>How the list should be grouped (e.g. by assignee). Defaults to no grouping.</summary>
    public ParticipantGroupBy GroupBy { get; set; } = ParticipantGroupBy.None;
}
