using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Notifications.Command;
using Cfo.Cats.Application.Features.Notifications.DTOs;
using Cfo.Cats.Application.Features.Notifications.Queries;

namespace Cfo.Cats.Server.UI.Pages.Dashboard.Components;

public partial class MyNotifications
{
    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    private NotificationsWithPaginationQuery.Query Query { get; } = new()
    {
        PageSize = 5
    };

    protected override async Task OnInitializedAsync() => await OnRefresh();

    private async Task DismissNotification(Guid id)
    {
        await GetNewMediator().Send(new MarkAsRead.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsRead = [id]
        });
        await OnRefresh();
    }

    private async Task UnreadNotification(Guid id)
    {
        await GetNewMediator().Send(new MarkAsUnread.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsUnread = [id]
        });
        await OnRefresh();
    }

    private async Task OnRefresh()
    {
        Query.CurrentUser = UserProfile;
        Query.OrderBy = "NotificationDate";
        Query.SortDirection = nameof(SortDirection.Descending);
        var result = await GetNewMediator().Send(Query);
        
        if (result.Succeeded)
        {
            Results = result.Data;
        }
        else
        {
            Snackbar.Add(result.ErrorMessage, Severity.Error);
            Results = null;
        }
    }

    private PaginatedData<NotificationDto>? Results { get; set; }

    private Task OnPaginationChanged(int arg)
    {
        Query.PageNumber = arg;
        return OnRefresh();
    }

    private async Task OnShowUnreadChanged(bool value)
    {
        Query.ShowReadNotifications = value;
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