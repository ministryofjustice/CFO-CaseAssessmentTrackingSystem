using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class QAActivitiesResultsAdvancedFilter : PaginationFilter
{
    public required UserProfile CurentActiveUser { get; set; }
    public string? ParticipantId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? ObjectiveId { get; set; }
    public DateTime? CommencedStart { get; set; }
    public DateTime? CommencedEnd { get; set; }
    public LocationDto? Location { get; set; }
    public ActivityStatus? Status { get; set; }
    public List<ActivityType>? IncludeTypes { get; set; }
}

public enum QAActivitiesResultsListView
{
    [Description("Default")] Default = 0,
}