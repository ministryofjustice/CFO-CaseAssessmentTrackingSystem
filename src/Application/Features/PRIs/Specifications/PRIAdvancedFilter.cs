using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.PRIs.Specifications;

public class PRIAdvancedFilter : PaginationFilter
{
    //Leave in if they want a list at some point
    /// <summary>
    /// The filter for the list (based on the something )
    /// </summary>
    //public PRIListView ListView { get; set; } = PRIListView.Default;

    //public required string ParticipantId { get; set; }
    //public Guid? PRIId { get; set; }
    //public DateTime? ExpectedDateOfRelease { get; set; }
    //public DateTime? ActualDateOfRelease { get; set; }
    public UserProfile? CurrentUser { get; set; }

    /// <summary>    
    /// Flag to indicate that you only want to see your incoming PRI's.
    /// </summary>
    [Description("Incoming PRIs")]
    public bool IncludeIncoming { get; set; } = false;

    /// <summary>    
    /// Flag to indicate that you only want to see your outgoing PRI's.
    /// </summary>
    [Description("Outgoing PRIs")]
    public bool IncludeOutgoing { get; set; } = false;
}

//Leave in if they want a list at some point
//public enum PRIListView
//{
//    //[Description("Default")] Default = 0,
//    [Description("Default")] Default,
//    [Description("All")] All
//}