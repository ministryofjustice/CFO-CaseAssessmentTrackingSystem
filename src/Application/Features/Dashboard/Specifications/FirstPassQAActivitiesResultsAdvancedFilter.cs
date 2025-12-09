using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Dashboard.Specifications;

public class FirstPassQAActivitiesResultsAdvancedFilter : PaginationFilter
{
    /// <summary>
    /// The currently logged in user
    /// </summary>
    public UserProfile? CurrentUser { get; set; }

    /// <summary>
    /// Flag to indicate that you only want to see your own Participants.
    /// </summary>
    [Description("Just My Participants")]
    public bool JustMyParticipants { get; set; } = true;
}