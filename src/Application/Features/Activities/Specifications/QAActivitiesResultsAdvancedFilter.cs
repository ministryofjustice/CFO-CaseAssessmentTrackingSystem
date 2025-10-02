using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Locations.DTOs;

namespace Cfo.Cats.Application.Features.Activities.Specifications;

public class QAActivitiesResultsAdvancedFilter : PaginationFilter
{
    public required UserProfile UserProfile { get; set; }
    public DateTime? CommencedStart { get; set; }
    public DateTime? CommencedEnd { get; set; }
    public LocationDto? Location { get; set; }
    public ActivityStatus? Status { get; set; }
    public List<ActivityType>? IncludeTypes { get; set; }
    public bool JustMyParticipants { get; set; }
    public bool IncludeInternalNotes { get; set; }
}

public enum QAActivitiesResultsListView
{
    [Description("Default")] Default = 0,
}