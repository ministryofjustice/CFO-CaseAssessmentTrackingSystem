using Cfo.Cats.Application.Common.Models;
using Cfo.Cats.Application.Features.Telemetry.DTOs;
using Cfo.Cats.Application.Features.Telemetry.Queries.GetUsageWithPagination;
using Cfo.Cats.Application.Features.Telemetry.Specifications;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.Administration.Pages;

public partial class UsageTelemetry
{
    private GetUsageWithPaginationQuery Query { get; } = new();

    protected override IQuery<Result<PaginatedData<UsageEventDto>>> CreateQuery() => Query;

    private async Task OnChangedListView(UsageEventListView listView)
    {
        Query.ListView = listView;
        Query.PageNumber = 1;
        await LoadDataAsync();
    }

    private async Task OnSearch(string text)
    {
        Query.Keyword = text;
        Query.PageNumber = 1;
        await LoadDataAsync();
    }

    private async Task OnRefresh()
    {
        Query.Keyword = string.Empty;
        Query.PageNumber = 1;
        await LoadDataAsync();
    }

    private Task OnPaginationChanged(int pageNumber)
    {
        Query.PageNumber = pageNumber;
        return LoadDataAsync();
    }

    private async Task OnSort(string property)
    {
        if (Query.OrderBy == property)
        {
            Query.SortDirection = Query.SortDirection == nameof(SortDirection.Ascending)
                ? nameof(SortDirection.Descending)
                : nameof(SortDirection.Ascending);
        }
        else
        {
            Query.OrderBy = property;
            Query.SortDirection = nameof(SortDirection.Ascending);
        }

        await LoadDataAsync();
    }

    private static string GetListViewLabel(UsageEventListView listView) => listView switch
    {
        UsageEventListView.All => "All",
        UsageEventListView.Today => "Today",
        UsageEventListView.Last7Days => "Last 7 days",
        UsageEventListView.Last30Days => "Last 30 days",
        _ => listView.ToString()
    };

    private static string GetSortLabel(string orderBy) => orderBy switch
    {
        nameof(UsageEventDto.OccurredOn) => "When",
        nameof(UsageEventDto.Area) => "Area",
        nameof(UsageEventDto.Activity) => "Activity",
        nameof(UsageEventDto.UserName) => "User",
        nameof(UsageEventDto.TenantId) => "Tenant",
        nameof(UsageEventDto.Context) => "Context",
        _ => orderBy
    };
}
