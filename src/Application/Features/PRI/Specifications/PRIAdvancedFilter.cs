using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.PRI.Specifications;

public class PRIAdvancedFilter : PaginationFilter
{
    //public required string ParticipantId { get; set; }
    //public Guid? PRIId { get; set; }
    //public DateTime? ExpectedDateOfRelease { get; set; }
    //public DateTime? ActualDateOfRelease { get; set; }
    public UserProfile? CurrentUser { get; set; }
}

public enum PRIsListView
{
    [Description("Default")] Default = 0,
}