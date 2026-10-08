using Cfo.Cats.Domain.Entities.ManagementInformation;

namespace Cfo.Cats.Application.Features.Telemetry.Specifications;

public class UsageEventAdvancedSpecification : Specification<UsageEvent>
{
    public UsageEventAdvancedSpecification(UsageEventAdvancedFilter filter)
    {
        var today = DateTime.UtcNow.Date;
        var last7day = today.AddDays(-7);
        var last30day = today.AddDays(-30);

        Query
            .Where(p => p.Area == filter.Area, string.IsNullOrEmpty(filter.Area) == false)
            .Where(p => p.OccurredOn >= today, filter.ListView == UsageEventListView.Today)
            .Where(p => p.OccurredOn >= last7day, filter.ListView == UsageEventListView.Last7Days)
            .Where(p => p.OccurredOn >= last30day, filter.ListView == UsageEventListView.Last30Days)
            .Where(
                p => p.Area.Contains(filter.Keyword!)
                     || p.Activity.Contains(filter.Keyword!)
                     || p.UserName!.Contains(filter.Keyword!)
                     || p.UserId!.Contains(filter.Keyword!)
                     || p.Context!.Contains(filter.Keyword!),
                string.IsNullOrEmpty(filter.Keyword) == false);
    }
}
