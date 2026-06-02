namespace Cfo.Cats.Application.Features.Participants.Specifications;

public enum RecentlyAssignedFilter
{
    [Description("All")]
    All,
    
    [Description("Last 10 Days")]
    Last10Days,
    
    [Description("Last 30 Days")]
    Last30Days,

}
