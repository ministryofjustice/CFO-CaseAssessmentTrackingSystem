namespace Cfo.Cats.Application.Features.Participants.Specifications;

public enum RecentParticipantFilter
{
    [Description("All")]
    All,
    
    [Description("Assigned (Last 10 Days)")]
    AssignedLast10Days,
    
    [Description("Assigned (Last 30 Days)")]
    AssignedLast30Days,
    
    [Description("Visited (Last 7 Days)")]
    VisitedLast7Days,

    [Description("Archived (Last 30 Days)")]
    ArchivedLast30Days,

    [Description("In Licence End Period")]
    LicenceEndPeriod,

}
