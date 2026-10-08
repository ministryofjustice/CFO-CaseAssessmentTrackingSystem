using Cfo.Cats.Application.Common.Models;

namespace Cfo.Cats.Application.Features.Telemetry.Specifications;

public enum UsageEventListView
{
    [Description("All")]
    All,
    [Description("Today")]
    Today,
    [Description("Last 7 days")]
    Last7Days,
    [Description("Last 30 days")]
    Last30Days
}

public class UsageEventAdvancedFilter : PaginationFilter
{
    public UsageEventListView ListView { get; set; } = UsageEventListView.Last30Days;
    public string? Area { get; set; }
}
