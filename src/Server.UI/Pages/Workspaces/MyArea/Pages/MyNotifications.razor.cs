using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Notifications.Command;
using Cfo.Cats.Application.Features.Notifications.DTOs;
using Cfo.Cats.Application.Features.Notifications.Queries;

namespace Cfo.Cats.Server.UI.Pages.Workspaces.MyArea.Pages;

public partial class MyNotifications
{
    private int _pageNumber = 1;
    private bool _showUnread = false;
    [CascadingParameter] public UserProfile UserProfile { get; set; } = null!;

    protected override IQuery<Result<PaginatedData<NotificationDto>>> CreateQuery() => 
        new NotificationsWithPaginationQuery.Query()
        {
            PageSize = 10,
            OrderBy = "NotificationDate",
            SortDirection = nameof(SortDirection.Descending),
            PageNumber = _pageNumber,
            ShowReadNotifications = _showUnread,
            CurrentUser = UserProfile,
        };  

    private async Task DismissNotification(Guid id)
    {
        Loading = true;
        await Service.Send(new MarkAsRead.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsRead = [id]
        });
        await LoadDataAsync();
    }

    private async Task UnreadNotification(Guid id)
    {
        Loading = true;
        await Service.Send(new MarkAsUnread.Command
        {
            CurrentUser = UserProfile,
            NotificationsToMarkAsUnread = [id]
        });
        await LoadDataAsync();
    }
    private Task OnPaginationChanged(int arg)
    {
        _pageNumber = arg;
        return LoadDataAsync();
    }

    private Task OnShowUnreadChanged(bool value)
    {
        _showUnread = value;
        return LoadDataAsync();
    }

    private void GotoNotification(string? itemLink)
    {
        if (itemLink is not null)
        {
            Navigation.NavigateTo(itemLink);
        }
    }    

}