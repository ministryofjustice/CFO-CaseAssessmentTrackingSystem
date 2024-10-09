using Cfo.Cats.Application.Common.Security;

namespace Cfo.Cats.Application.Features.Notifications.Specifications;

public class NotificationsAdvancedFilter : PaginationFilter
{
    public UserProfile? CurrentUser { get; set; }

    public bool IncludeReadNotifications { get; set; } = false;

    public NotificationsListView ListView { get; set; } = NotificationsListView.Default;

}

public enum NotificationsListView
{
    [Description("Default")] Default = 0,
}