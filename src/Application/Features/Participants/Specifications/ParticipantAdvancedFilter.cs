using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Participants.Specifications;

public class ParticipantAdvancedFilter
    : PaginationFilter
{
    /// <summary>
    /// The filter for the list (based on the status)
    /// </summary>
    public ParticipantListView ListView { get; set; } = ParticipantListView.Default;

    /// <summary>
    /// The currently logged in user
    /// </summary>
    public UserProfile? CurrentUser { get; set; }

    /// <summary>
    /// Flag to indicate that you only want to see your own cases.
    /// </summary>
    [Description("Just My Cases")]
    public bool JustMyCases { get; set; } = true;

    /// <summary>
    /// The current location of the participant
    /// </summary>
    public int[] Locations { get; set; } = [];

}

public enum ParticipantListView
{
    [Description("Default")] Default,
    [Description("Identified")] Identified,
    [Description("Enrolling")] Enrolling,
    [Description("Submitted To Provider")] SubmittedToProvider,
    [Description("Submitted To QA")] SubmittedToQa,
    [Description("Any QA")] SubmittedToAny,
    [Description("Approved")] Approved,
    [Description("Dormant")] Dormant,
    [Description("Archived")] Archived,
    [Description("All")] All
}

