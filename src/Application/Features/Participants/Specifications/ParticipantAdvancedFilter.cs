using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public class ParticipantAdvancedFilter
    : PaginationFilter
{
    public ParticipantListView ListView { get; set; } = ParticipantListView.Default;
    public UserProfile? CurrentUser { get; set; }
}

public enum ParticipantListView
{
    [Description("Default")] Default,
    [Description("Pending")] Pending,
    [Description("Enrolment Confirmed")] EnrolmentConfirmed,
    [Description("Submitted To Provider")] SubmittedToProvider,
    [Description("Submitted To QA")] SubmittedToQa,
    [Description("Any QA")] SubmittedToAny,
    [Description("Approved")] Approved,
    [Description("Dormant")] Dormant,
    [Description("Archived")] Archived,
    [Description("All")] All
}

