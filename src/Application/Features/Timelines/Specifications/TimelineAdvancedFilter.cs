using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Timelines.Specifications;

public class TimelineAdvancedFilter : PaginationFilter
{
    public required string ParticipantId { get; set; }
    public TimelineTrailListView ListView { get; set; } = TimelineTrailListView.All;
    public UserProfile? CurrentUser { get; set; }
}