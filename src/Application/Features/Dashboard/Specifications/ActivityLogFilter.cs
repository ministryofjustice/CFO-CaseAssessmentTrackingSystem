using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Dashboard.Specifications;

public class ActivityLogFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get;set; }
    public ActivityLogListView ListView { get; set; } = ActivityLogListView.All;
}