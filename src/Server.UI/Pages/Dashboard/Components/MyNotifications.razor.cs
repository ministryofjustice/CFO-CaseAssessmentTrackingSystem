using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Notifications.Command;
using Cfo.Cats.Application.Features.Notifications.DTOs;
using Cfo.Cats.Application.Features.Notifications.Queries;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyNotifications
{
    [CascadingParameter] public UserProfile UserProfile { get; set; } = default!;

    private NotificationsWithPaginationQuery.Query Query { get; set; } = new()
    {
        PageSize = 5
    };

    protected override async Task OnInitializedAsync()
    {
        await OnRefresh();
    }

    private async Task DismissNotification(Guid id)
    {
        var result = await GetNewMediator().Send(new MarkAsRead.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsRead = [id]
        });
        await OnRefresh();
    }

    private async Task UnreadNotification(Guid id)
    {
        var result = await GetNewMediator().Send(new MarkAsUnread.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsUnread = [id]
        });
        await OnRefresh();
    }

    private async Task OnShowClosedChanged(bool value)
    {
        Query!.ShowReadNotifications = value;
        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;
        Query.OrderBy = "NotificationDate";
        Query.SortDirection = SortDirection.Descending.ToString();
        Results = await GetNewMediator().Send(Query);
    }

    private PaginatedData<NotificationDto>? Results { get; set; }

    private Task OnPaginationChanged(int arg)
    {
        Query.PageNumber = arg;
        return OnRefresh();
    }

    private async Task OnShowUnreadChanged(bool value)
    {
        Query!.ShowReadNotifications = value;
        await OnRefresh();
    }

    private void GotoNotification(string? itemLink)
    {
        if (itemLink is not null)
        {
            Navigation.NavigateTo(itemLink);
        }
    }
}